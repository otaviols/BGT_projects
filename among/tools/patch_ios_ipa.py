"""Acerta o Info.plist do .ipa que o NVGT gera, para um teste em iPhone.

    python tools/patch_ios_ipa.py AmongUs.ipa AmongUs-ios.ipa

O NVGT não tem chave de configuração para o Info.plist do iOS (no Android há o build.android_manifest),
e o que ele grava deixa de fora três coisas que este jogo precisa:

* NSMicrophoneUsageDescription - sem ela o iOS FECHA o aplicativo no instante em que a voz toca no
  microfone. Não pergunta, não avisa: fecha.
* Só horizontal - pelo mesmo motivo do Android (o estéreo dos alto-falantes do celular, recado #165).
  O NVGT grava as quatro orientações.
* MinimumOSVersion - o binário do stub diz iOS 16.0 (LC_BUILD_VERSION). Sem a chave, o instalador
  pode recusar ou deixar instalar onde não roda.

Funciona porque o .ipa sai SEM assinatura: mexer nele depois não invalida nada. Quem assina é a
ferramenta de instalação (Sideloadly, AltStore), com o Apple ID de quem instala.
"""
import plistlib
import sys
import zipfile

MIC_TEXT = ("O chat de voz usa o microfone para os outros jogadores ouvirem você. "
            "The voice chat uses the microphone so other players can hear you.")
LANDSCAPE = ["UIInterfaceOrientationLandscapeLeft", "UIInterfaceOrientationLandscapeRight"]
MIN_IOS = "16.0"


def patch(src, dst):
    with zipfile.ZipFile(src) as zin, zipfile.ZipFile(dst, "w", zipfile.ZIP_DEFLATED) as zout:
        plists = [n for n in zin.namelist() if n.startswith("Payload/") and n.endswith(".app/Info.plist")]
        if len(plists) != 1:
            sys.exit(f"esperava um Info.plist do app, achei {plists}")
        for item in zin.infolist():
            data = zin.read(item.filename)
            if item.filename == plists[0]:
                info = plistlib.loads(data)
                info["NSMicrophoneUsageDescription"] = MIC_TEXT
                info["UISupportedInterfaceOrientations"] = LANDSCAPE
                info["UISupportedInterfaceOrientations~ipad"] = LANDSCAPE
                info["MinimumOSVersion"] = MIN_IOS
                data = plistlib.dumps(info)
                print(f"Info.plist: {info['CFBundleIdentifier']} {info['CFBundleShortVersionString']}, "
                      f"microfone, horizontal, iOS {MIN_IOS}+")
            # O executável precisa continuar executável: o atributo do zip carrega o modo Unix.
            zout.writestr(item, data)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        sys.exit(__doc__)
    patch(sys.argv[1], sys.argv[2])
