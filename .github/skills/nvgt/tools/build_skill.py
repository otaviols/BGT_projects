#!/usr/bin/env python3
"""Rebuilds the bundled reference material for the standalone NVGT skill.

The skill ships its own copy of NVGT's documentation so it works in any
project, on any machine, without the NVGT source tree present. That copy is
frozen at whatever NVGT version it was generated from, so rerun this script
after upgrading NVGT:

    python tools/build_skill.py D:/git/nvgt

The one step that needs a working NVGT interpreter is the engine API dump. If
release/nvgt.exe (or bin/nvgt on other platforms) is missing, the script skips
that step and keeps any previously generated dump.

NVGT is zlib licensed; its license file is copied alongside the docs.
"""

import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

SKILL_ROOT = Path(__file__).resolve().parent.parent
REFERENCE = SKILL_ROOT / "reference"


# --------------------------------------------------------------------------
# Engine API dump
# --------------------------------------------------------------------------

def find_interpreter(repo):
	for candidate in ("release/nvgt.exe", "release/nvgt", "bin/nvgt.exe", "bin/nvgt"):
		p = repo / candidate
		if p.exists():
			return p
	return None


def dump_engine_config(repo, interpreter):
	"""Ask the engine itself to serialize its registered API.

	script_dump_engine_configuration writes AngelScript's own configuration
	format, which is the authoritative list of every type, method, function,
	property and enum the scripting environment exposes.
	"""
	work = Path(tempfile.mkdtemp(prefix="nvgt_skill_"))
	script = work / "dump_api.nvgt"
	script.write_text(
		'void main() {\n'
		'\tfile f;\n'
		'\tf.open("engine_config_dump.txt", "wb");\n'
		'\tscript_dump_engine_configuration(f);\n'
		'\tf.close();\n'
		'}\n',
		encoding="utf-8",
	)
	subprocess.run([str(interpreter), "-q", str(script)], cwd=work, check=True)
	out = work / "engine_config_dump.txt"
	data = out.read_text(encoding="utf-8", errors="replace")
	shutil.rmtree(work, ignore_errors=True)
	return data


def unquote(s):
	return s.replace('\\"', '"')


DUMP_PATTERNS = {
	"objtype": re.compile(r'^objtype "((?:[^"\\]|\\.)*)"'),
	"objmthd": re.compile(r'^objmthd "((?:[^"\\]|\\.)*)" "((?:[^"\\]|\\.)*)"'),
	"objprop": re.compile(r'^objprop "((?:[^"\\]|\\.)*)" "((?:[^"\\]|\\.)*)"'),
	"objbeh": re.compile(r'^objbeh "((?:[^"\\]|\\.)*)" (\d+) "((?:[^"\\]|\\.)*)"'),
	"func": re.compile(r'^func "((?:[^"\\]|\\.)*)"'),
	"prop": re.compile(r'^prop "((?:[^"\\]|\\.)*)"'),
	"funcdef": re.compile(r'^funcdef "((?:[^"\\]|\\.)*)"'),
	"intf": re.compile(r'^intf "((?:[^"\\]|\\.)*)"'),
	"intfmthd": re.compile(r'^intfmthd "((?:[^"\\]|\\.)*)" "((?:[^"\\]|\\.)*)"'),
	"enumval": re.compile(r'^enumval (\S+) (\S+) (-?\d+)'),
	"namespace": re.compile(r'^namespace "([^"]*)"'),
}

# AngelScript behaviour ids. Only the ones worth showing a script author.
BEHAVIOUR_CONSTRUCT = {3}  # factory / constructor


def parse_engine_dump(text):
	types = {}       # name -> {"methods": [], "props": [], "ctors": [], "ns": str}
	functions = []   # (ns, signature)
	globals_ = []    # (ns, declaration)
	funcdefs = []
	enums = {}       # name -> [(value_name, value)]
	interfaces = {}
	ns = ""

	def type_entry(name):
		return types.setdefault(name, {"methods": [], "props": [], "ctors": [], "ns": ns})

	for line in text.splitlines():
		if not line or line.startswith("//") or line.startswith("ep ") or line.startswith("access"):
			continue
		kind = line.split(" ", 1)[0]
		pattern = DUMP_PATTERNS.get(kind)
		if not pattern:
			continue
		m = pattern.match(line)
		if not m:
			continue
		if kind == "namespace":
			ns = m.group(1)
		elif kind == "objtype":
			type_entry(unquote(m.group(1)))
		elif kind == "objmthd":
			type_entry(unquote(m.group(1)))["methods"].append(unquote(m.group(2)))
		elif kind == "objprop":
			type_entry(unquote(m.group(1)))["props"].append(unquote(m.group(2)))
		elif kind == "objbeh":
			if int(m.group(2)) in BEHAVIOUR_CONSTRUCT:
				type_entry(unquote(m.group(1)))["ctors"].append(unquote(m.group(3)))
		elif kind == "func":
			functions.append((ns, unquote(m.group(1))))
		elif kind == "prop":
			globals_.append((ns, unquote(m.group(1))))
		elif kind == "funcdef":
			funcdefs.append(unquote(m.group(1)))
		elif kind == "intf":
			interfaces.setdefault(unquote(m.group(1)), [])
		elif kind == "intfmthd":
			interfaces.setdefault(unquote(m.group(1)), []).append(unquote(m.group(2)))
		elif kind == "enumval":
			enums.setdefault(m.group(1), []).append((m.group(2), int(m.group(3))))

	return {
		"types": types,
		"functions": functions,
		"globals": globals_,
		"funcdefs": funcdefs,
		"enums": enums,
		"interfaces": interfaces,
	}


def qualify(ns, text):
	return f"{ns}::{text}" if ns else text


def write_engine_api(api, version, out_path):
	lines = [
		"# NVGT engine API (complete, generated)",
		"",
		f"Generated from NVGT {version} by asking the engine to serialize its own",
		"registered API. Every signature here is exact. This file is the authority",
		"when the prose documentation is silent, incomplete or out of date.",
		"",
		"Search it with Grep rather than reading it start to finish.",
		"",
		"---",
		"",
		"## Global functions",
		"",
	]
	for ns, sig in sorted(api["functions"], key=lambda x: (x[0], x[1])):
		lines.append(f"- `{qualify(ns, sig)};`")

	lines += ["", "## Global properties", ""]
	for ns, decl in sorted(api["globals"], key=lambda x: (x[0], x[1])):
		lines.append(f"- `{qualify(ns, decl)};`")

	lines += ["", "## Funcdefs (callback signatures)", ""]
	for fd in sorted(api["funcdefs"]):
		lines.append(f"- `funcdef {fd};`")

	lines += ["", "## Enums", ""]
	for name in sorted(api["enums"]):
		lines.append(f"### {name}")
		lines.append("")
		for value_name, value in api["enums"][name]:
			lines.append(f"- `{value_name} = {value}`")
		lines.append("")

	if api["interfaces"]:
		lines += ["## Interfaces", ""]
		for name in sorted(api["interfaces"]):
			lines.append(f"### {name}")
			lines.append("")
			for sig in api["interfaces"][name]:
				lines.append(f"- `{sig};`")
			lines.append("")

	lines += ["## Types", ""]
	for name in sorted(api["types"], key=str.lower):
		entry = api["types"][name]
		lines.append(f"### {qualify(entry['ns'], name)}")
		lines.append("")
		if entry["ctors"]:
			lines.append("Construction:")
			lines.append("")
			for sig in entry["ctors"]:
				lines.append(f"- `{sig};`")
			lines.append("")
		if entry["props"]:
			lines.append("Properties:")
			lines.append("")
			for sig in entry["props"]:
				lines.append(f"- `{sig};`")
			lines.append("")
		if entry["methods"]:
			lines.append("Methods:")
			lines.append("")
			for sig in entry["methods"]:
				lines.append(f"- `{sig};`")
			lines.append("")

	out_path.write_text("\n".join(lines) + "\n", encoding="utf-8")
	return len(api["types"]), len(api["functions"])


# --------------------------------------------------------------------------
# Documentation copy + index
# --------------------------------------------------------------------------

def copy_tree(src, dst):
	if dst.exists():
		shutil.rmtree(dst)
	shutil.copytree(src, dst)


def first_prose_line(path):
	"""Pull a one-line description out of an NVGT doc file.

	.md files lead with a '# Title' then prose; .nvgt files wrap their docs in
	a /** ... */ block whose first line is the summary.
	"""
	try:
		text = path.read_text(encoding="utf-8", errors="replace")
	except OSError:
		return ""
	for raw in text.splitlines():
		line = raw.strip()
		if not line or line in ("/**", "*/", "```"):
			continue
		if line.startswith("#"):
			continue
		line = line.lstrip("*").strip()
		if not line or line.startswith("/*"):
			continue
		return line[:160]
	return ""


def build_api_index(docs_root, out_path):
	"""Map every documented symbol to its file, so lookups are one Grep away."""
	entries = []
	for base in ("references/builtin", "references/include", "references/plugin"):
		root = docs_root / base
		if not root.exists():
			continue
		for dirpath, _dirnames, filenames in os.walk(root):
			for fn in sorted(filenames):
				if not fn.lower().endswith((".md", ".nvgt")):
					continue
				full = Path(dirpath) / fn
				rel = full.relative_to(docs_root).as_posix()
				symbol = os.path.splitext(fn)[0].lstrip("!-_")
				if symbol.startswith("."):
					continue
				category = Path(dirpath).relative_to(root).as_posix() or "(root)"
				entries.append((base.split("/")[-1], symbol, category, rel))

	by_area = {}
	for area, symbol, category, rel in entries:
		by_area.setdefault(area, []).append((symbol, category, rel))

	lines = [
		"# API index",
		"",
		"Every documented symbol and the file that documents it, relative to",
		"`reference/docs/`. Grep this file for a name, then Read the file it points to.",
		"",
		"Signatures in those files are prose-written and occasionally lag behind the",
		"engine. `reference/engine-api.md` is generated from the engine itself and wins",
		"on any disagreement.",
		"",
	]
	titles = {
		"builtin": "Built-in engine API",
		"include": "Standard include library (`#include \"...\"`)",
		"plugin": "Plugins (require `#pragma plugin`)",
	}
	for area in ("builtin", "include", "plugin"):
		if area not in by_area:
			continue
		lines += [f"## {titles[area]}", ""]
		lines += ["| Symbol | Area | File |", "| --- | --- | --- |"]
		for symbol, category, rel in sorted(by_area[area], key=lambda x: (x[1].lower(), x[0].lower())):
			lines.append(f"| `{symbol}` | {category} | {rel} |")
		lines.append("")

	out_path.write_text("\n".join(lines) + "\n", encoding="utf-8")
	return len(entries)


def build_stdlib_index(stdlib_dir, out_path):
	lines = [
		"# Standard include library",
		"",
		"These ship with NVGT and are included by name: `#include \"menu.nvgt\"`.",
		"The full source of each is bundled here — read it when the reference docs",
		"do not answer the question, since these are plain NVGT scripts.",
		"",
		"| Include | Lines | Summary |",
		"| --- | --- | --- |",
	]
	for path in sorted(stdlib_dir.glob("*.nvgt")):
		text = path.read_text(encoding="utf-8", errors="replace")
		summary = ""
		for raw in text.splitlines():
			line = raw.strip()
			if line.startswith("//") and len(line) > 4:
				summary = line.lstrip("/ ").strip()
				break
			if line and not line.startswith(("/*", "*", "#")):
				break
		lines.append(f"| `{path.name}` | {len(text.splitlines())} | {summary[:120]} |")
	lines.append("")
	out_path.write_text("\n".join(lines) + "\n", encoding="utf-8")


# --------------------------------------------------------------------------

def main():
	if len(sys.argv) < 2:
		print(__doc__)
		print("error: path to the NVGT source repository is required")
		return 1
	repo = Path(sys.argv[1]).resolve()
	docs_src = repo / "doc" / "src"
	includes_src = repo / "release" / "include"
	if not docs_src.is_dir():
		print(f"error: {docs_src} not found -- is {repo} really the NVGT repo?")
		return 1

	REFERENCE.mkdir(parents=True, exist_ok=True)
	version = (repo / "version").read_text(encoding="utf-8").strip() if (repo / "version").exists() else "unknown"
	try:
		commit = subprocess.run(
			["git", "-C", str(repo), "rev-parse", "--short", "HEAD"],
			capture_output=True, text=True, check=True,
		).stdout.strip()
	except (subprocess.CalledProcessError, FileNotFoundError):
		commit = "unknown"

	print(f"NVGT {version} ({commit}) at {repo}")

	print("copying documentation...")
	copy_tree(docs_src, REFERENCE / "docs")
	n_docs = sum(len(f) for _, _, f in os.walk(REFERENCE / "docs"))
	print(f"  {n_docs} files")

	print("copying standard includes...")
	stdlib = REFERENCE / "stdlib"
	if stdlib.exists():
		shutil.rmtree(stdlib)
	stdlib.mkdir(parents=True)
	if includes_src.is_dir():
		for path in includes_src.iterdir():
			if path.is_file():
				shutil.copy2(path, stdlib / path.name)
	print(f"  {len(list(stdlib.glob('*.nvgt')))} includes")

	license_src = repo / "license.md"
	if license_src.exists():
		shutil.copy2(license_src, REFERENCE / "NVGT-LICENSE.md")

	print("building indexes...")
	n_index = build_api_index(REFERENCE / "docs", REFERENCE / "api-index.md")
	build_stdlib_index(stdlib, REFERENCE / "stdlib-index.md")
	print(f"  {n_index} documented symbols indexed")

	interpreter = find_interpreter(repo)
	if interpreter:
		print(f"dumping engine API via {interpreter.name}...")
		try:
			dump = dump_engine_config(repo, interpreter)
			api = parse_engine_dump(dump)
			n_types, n_funcs = write_engine_api(api, version, REFERENCE / "engine-api.md")
			print(f"  {n_types} types, {n_funcs} global functions")
		except subprocess.CalledProcessError as e:
			print(f"  warning: interpreter failed ({e}); keeping any existing engine-api.md")
	else:
		print("warning: no nvgt interpreter found; keeping any existing engine-api.md")

	(REFERENCE / "SOURCE.md").write_text(
		"# Bundled source revision\n\n"
		f"- NVGT version: `{version}`\n"
		f"- Commit: `{commit}`\n"
		f"- Repository: `{repo}`\n\n"
		"Regenerate with `python tools/build_skill.py <path-to-nvgt-repo>` after\n"
		"upgrading NVGT. If the user's installed NVGT is newer than the version\n"
		"above, treat anything surprising as possibly stale and say so.\n\n"
		"NVGT is zlib licensed; see `NVGT-LICENSE.md`.\n",
		encoding="utf-8",
	)
	print("done")
	return 0


if __name__ == "__main__":
	sys.exit(main())
