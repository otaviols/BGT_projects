# API index

Every documented symbol and the file that documents it, relative to
`reference/docs/`. Grep this file for a name, then Read the file it points to.

Signatures in those files are prose-written and occasionally lag behind the
engine. `reference/engine-api.md` is generated from the engine itself and wins
on any disagreement.

## Built-in engine API

| Symbol | Area | File |
| --- | --- | --- |
| `Containers` | !Containers | references/builtin/!Containers/!Containers.md |
| `array` | !Containers/Classes/array | references/builtin/!Containers/Classes/array/!array.md |
| `back` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/back.nvgt |
| `empty` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/empty.nvgt |
| `erase` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/erase.nvgt |
| `extend` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/extend.nvgt |
| `find` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/find.nvgt |
| `find_by_ref` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/find_by_ref.nvgt |
| `front` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/front.nvgt |
| `insert` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/insert.nvgt |
| `insert_last` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/insert_last.nvgt |
| `length` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/length.nvgt |
| `random` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/random.nvgt |
| `remove_last` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/remove_last.md |
| `remove_range` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/remove_range.nvgt |
| `reserve` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/reserve.md |
| `resize` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/resize.md |
| `reverse` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/reverse.nvgt |
| `shuffle` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/shuffle.nvgt |
| `sort` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/sort.md |
| `sort_ascending` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/sort_ascending.md |
| `sort_descending` | !Containers/Classes/array/Methods | references/builtin/!Containers/Classes/array/Methods/sort_descending.md |
| `opAssign` | !Containers/Classes/array/Operators | references/builtin/!Containers/Classes/array/Operators/opAssign.nvgt |
| `opEquals` | !Containers/Classes/array/Operators | references/builtin/!Containers/Classes/array/Operators/opEquals.nvgt |
| `opFor` | !Containers/Classes/array/Operators | references/builtin/!Containers/Classes/array/Operators/opFor.nvgt |
| `opIndex` | !Containers/Classes/array/Operators | references/builtin/!Containers/Classes/array/Operators/opIndex.nvgt |
| `dictionary` | !Containers/Classes/dictionary | references/builtin/!Containers/Classes/dictionary/!dictionary.md |
| `delete` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/delete.nvgt |
| `delete_all` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/delete_all.md |
| `exists` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/exists.nvgt |
| `get` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/get.nvgt |
| `get_keys` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/get_keys.nvgt |
| `get_size` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/get_size.nvgt |
| `is_empty` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/is_empty.nvgt |
| `serialize` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/serialize.md |
| `set` | !Containers/Classes/dictionary/Methods | references/builtin/!Containers/Classes/dictionary/Methods/set.md |
| `opIndex` | !Containers/Classes/dictionary/Operators | references/builtin/!Containers/Classes/dictionary/Operators/opIndex.md |
| `Grid` | !Containers/Classes/grid | references/builtin/!Containers/Classes/grid/!Grid.nvgt |
| `height` | !Containers/Classes/grid/Methods | references/builtin/!Containers/Classes/grid/Methods/height.nvgt |
| `resize` | !Containers/Classes/grid/Methods | references/builtin/!Containers/Classes/grid/Methods/resize.nvgt |
| `width` | !Containers/Classes/grid/Methods | references/builtin/!Containers/Classes/grid/Methods/width.nvgt |
| `opIndex` | !Containers/Classes/grid/Operators | references/builtin/!Containers/Classes/grid/Operators/opIndex.nvgt |
| `deserialize` | !Containers/Functions | references/builtin/!Containers/Functions/deserialize.md |
| `datatypes` | !Datatypes | references/builtin/!Datatypes/!datatypes.md |
| `any` | !Datatypes/any | references/builtin/!Datatypes/any/!any.nvgt |
| `retrieve` | !Datatypes/any/Methods | references/builtin/!Datatypes/any/Methods/retrieve.md |
| `store` | !Datatypes/any/Methods | references/builtin/!Datatypes/any/Methods/store.nvgt |
| `ref` | !Datatypes/ref | references/builtin/!Datatypes/ref/!ref.nvgt |
| `count` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/count.nvgt |
| `empty` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/empty.nvgt |
| `ends_with` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/ends_with.nvgt |
| `erase` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/erase.nvgt |
| `escape` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/escape.nvgt |
| `find` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find.md |
| `find_first` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_first.md |
| `find_first_not_of` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_first_not_of.md |
| `find_first_of` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_first_of.md |
| `find_last` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_last.md |
| `find_last_not_of` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_last_not_of.md |
| `find_last_of` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/find_last_of.md |
| `format` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/format.nvgt |
| `insert` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/insert.md |
| `is_alphabetic` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_alphabetic.nvgt |
| `is_alphanumeric` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_alphanumeric.nvgt |
| `is_digits` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_digits.nvgt |
| `is_lower` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_lower.nvgt |
| `is_punctuation` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_punctuation.nvgt |
| `is_upper` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/is_upper.nvgt |
| `length` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/length.nvgt |
| `lower` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/lower.nvgt |
| `lower_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/lower_this.nvgt |
| `remove_UTF8_BOM` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/remove_UTF8_BOM.nvgt |
| `replace` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/replace.md |
| `replace_characters` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/replace_characters.md |
| `replace_characters_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/replace_characters_this.md |
| `replace_range` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/replace_range.md |
| `replace_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/replace_this.md |
| `resize` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/resize.nvgt |
| `reverse` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/reverse.nvgt |
| `reverse_bytes` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/reverse_bytes.nvgt |
| `rfind` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/rfind.md |
| `slice` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/slice.nvgt |
| `split` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/split.nvgt |
| `starts_with` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/starts_with.nvgt |
| `substr` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/substr.nvgt |
| `trim_whitespace` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace.nvgt |
| `trim_whitespace_left` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace_left.nvgt |
| `trim_whitespace_left_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace_left_this.nvgt |
| `trim_whitespace_right` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace_right.nvgt |
| `trim_whitespace_right_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace_right_this.nvgt |
| `trim_whitespace_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/trim_whitespace_this.nvgt |
| `unpacket` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/unpacket.md |
| `upper` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/upper.nvgt |
| `upper_this` | !Datatypes/string/Methods | references/builtin/!Datatypes/string/Methods/upper_this.nvgt |
| `Streams` | !Streams | references/builtin/!Streams/!Streams.md |
| `datastream` | !Streams/datastream | references/builtin/!Streams/datastream/!datastream.nvgt |
| `close` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/close.nvgt |
| `close_all` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/close_all.nvgt |
| `get_pos` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/get_pos.nvgt |
| `open` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/open.nvgt |
| `read` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read.nvgt |
| `read_double` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_double.nvgt |
| `read_float` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_float.nvgt |
| `read_int` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_int.nvgt |
| `read_int16` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_int16.nvgt |
| `read_int64` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_int64.nvgt |
| `read_int8` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_int8.nvgt |
| `read_string` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_string.nvgt |
| `read_uint` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_uint.nvgt |
| `read_uint16` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_uint16.nvgt |
| `read_uint64` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_uint64.nvgt |
| `read_uint8` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/read_uint8.nvgt |
| `seek` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/seek.nvgt |
| `write` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write.nvgt |
| `write_double` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_double.nvgt |
| `write_float` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_float.nvgt |
| `write_int` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_int.nvgt |
| `write_int16` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_int16.nvgt |
| `write_int64` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_int64.nvgt |
| `write_int8` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_int8.nvgt |
| `write_string` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_string.nvgt |
| `write_uint` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_uint.nvgt |
| `write_uint16` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_uint16.nvgt |
| `write_uint64` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_uint64.nvgt |
| `write_uint8` | !Streams/datastream/Methods | references/builtin/!Streams/datastream/Methods/write_uint8.nvgt |
| `active` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/active.md |
| `available` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/available.nvgt |
| `bad` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/bad.nvgt |
| `binary` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/binary.md |
| `eof` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/eof.nvgt |
| `fail` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/fail.nvgt |
| `good` | !Streams/datastream/Properties | references/builtin/!Streams/datastream/Properties/good.nvgt |
| `file` | !Streams/file | references/builtin/!Streams/file/!file.md |
| `open` | !Streams/file/Methods | references/builtin/!Streams/file/Methods/open.nvgt |
| `size` | !Streams/file/Properties | references/builtin/!Streams/file/Properties/size.nvgt |
| `close` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/close.nvgt |
| `load` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/load.md |
| `pause` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/pause.md |
| `play` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/play.md |
| `play_looped` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/play_looped.md |
| `play_wait` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/play_wait.md |
| `seek` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/seek.md |
| `set_position` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/set_position.md |
| `stop` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/stop.md |
| `stream_url` | Audio/Classes/sound/Methods | references/builtin/Audio/Classes/sound/Methods/stream_url.nvgt |
| `active` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/active.md |
| `length` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/length.md |
| `loaded_filename` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/loaded_filename.md |
| `paused` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/paused.md |
| `playing` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/playing.md |
| `sliding` | Audio/Classes/sound/Properties | references/builtin/Audio/Classes/sound/Properties/sliding.md |
| `get_sound_input_devices` | Audio/Functions | references/builtin/Audio/Functions/get_sound_input_devices.nvgt |
| `get_sound_output_devices` | Audio/Functions | references/builtin/Audio/Functions/get_sound_output_devices.nvgt |
| `sound_default_mixer` | Audio/Global Properties | references/builtin/Audio/Global Properties/sound_default_mixer.md |
| `sound_default_pack` | Audio/Global Properties | references/builtin/Audio/Global Properties/sound_default_pack.md |
| `sound_global_hrtf` | Audio/Global Properties | references/builtin/Audio/Global Properties/sound_global_hrtf.md |
| `Concurrency` | Concurrency | references/builtin/Concurrency/!Concurrency.md |
| `async` | Concurrency/Classes/async | references/builtin/Concurrency/Classes/async/!async.nvgt |
| `try_wait` | Concurrency/Classes/async/Methods | references/builtin/Concurrency/Classes/async/Methods/try_wait.nvgt |
| `wait` | Concurrency/Classes/async/Methods | references/builtin/Concurrency/Classes/async/Methods/wait.nvgt |
| `complete` | Concurrency/Classes/async/Properties | references/builtin/Concurrency/Classes/async/Properties/complete.nvgt |
| `exception` | Concurrency/Classes/async/Properties | references/builtin/Concurrency/Classes/async/Properties/exception.nvgt |
| `failed` | Concurrency/Classes/async/Properties | references/builtin/Concurrency/Classes/async/Properties/failed.nvgt |
| `value` | Concurrency/Classes/async/Properties | references/builtin/Concurrency/Classes/async/Properties/value.nvgt |
| `atomic_flag` | Concurrency/Classes/atomic_flag | references/builtin/Concurrency/Classes/atomic_flag/atomic_flag.md |
| `clear` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/clear.md |
| `notify_all` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/notify_all.md |
| `notify_one` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/notify_one.md |
| `test` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/test.md |
| `test_and_set` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/test_and_set.md |
| `wait` | Concurrency/Classes/atomic_flag/methods | references/builtin/Concurrency/Classes/atomic_flag/methods/wait.md |
| `atomic_t` | Concurrency/Classes/atomic_T | references/builtin/Concurrency/Classes/atomic_T/!atomic_t.md |
| `compare_exchange_strong` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/compare_exchange_strong.md |
| `compare_exchange_weak` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/compare_exchange_weak.md |
| `exchange` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/exchange.md |
| `fetch_add` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/fetch_add.md |
| `fetch_and` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/fetch_and.md |
| `fetch_or` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/fetch_or.md |
| `fetch_sub` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/fetch_sub.md |
| `fetch_xor` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/fetch_xor.md |
| `is_lock_free` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/is_lock_free.md |
| `load` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/load.md |
| `notify_all` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/notify_all.md |
| `notify_one` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/notify_one.md |
| `store` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/store.md |
| `wait` | Concurrency/Classes/atomic_T/Methods | references/builtin/Concurrency/Classes/atomic_T/Methods/wait.md |
| `opAddAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opAddAssign.md |
| `opAndAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opAndAssign.md |
| `opAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opAssign.md |
| `opImplConv` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opImplConv.md |
| `opOrAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opOrAssign.md |
| `opPostDec` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opPostDec.md |
| `opPostInc` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opPostInc.md |
| `opPreDec` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opPreDec.md |
| `opPreInc` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opPreInc.md |
| `opSubAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opSubAssign.md |
| `opXorAssign` | Concurrency/Classes/atomic_T/Operators | references/builtin/Concurrency/Classes/atomic_T/Operators/opXorAssign.md |
| `is_always_lock_free` | Concurrency/Classes/atomic_T/Properties | references/builtin/Concurrency/Classes/atomic_T/Properties/is_always_lock_free.md |
| `mutex` | Concurrency/Classes/mutex | references/builtin/Concurrency/Classes/mutex/!mutex.nvgt |
| `mutex_lock` | Concurrency/Classes/mutex | references/builtin/Concurrency/Classes/mutex/mutex_lock.md |
| `lock` | Concurrency/Classes/mutex/methods | references/builtin/Concurrency/Classes/mutex/methods/lock.md |
| `try_lock` | Concurrency/Classes/mutex/methods | references/builtin/Concurrency/Classes/mutex/methods/try_lock.nvgt |
| `unlock` | Concurrency/Classes/mutex/methods | references/builtin/Concurrency/Classes/mutex/methods/unlock.md |
| `thread_event` | Concurrency/Classes/thread_event | references/builtin/Concurrency/Classes/thread_event/!thread_event.nvgt |
| `reset` | Concurrency/Classes/thread_event/Methods | references/builtin/Concurrency/Classes/thread_event/Methods/reset.md |
| `set` | Concurrency/Classes/thread_event/Methods | references/builtin/Concurrency/Classes/thread_event/Methods/set.md |
| `try_wait` | Concurrency/Classes/thread_event/Methods | references/builtin/Concurrency/Classes/thread_event/Methods/try_wait.nvgt |
| `wait` | Concurrency/Classes/thread_event/Methods | references/builtin/Concurrency/Classes/thread_event/Methods/wait.md |
| `memory_order` | Concurrency/Enums | references/builtin/Concurrency/Enums/memory_order.md |
| `thread_event_type` | Concurrency/Enums | references/builtin/Concurrency/Enums/thread_event_type.md |
| `thread_priority` | Concurrency/Enums | references/builtin/Concurrency/Enums/thread_priority.md |
| `thread_current_id` | Concurrency/Functions | references/builtin/Concurrency/Functions/thread_current_id.nvgt |
| `thread_sleep` | Concurrency/Functions | references/builtin/Concurrency/Functions/thread_sleep.md |
| `thread_yield` | Concurrency/Functions | references/builtin/Concurrency/Functions/thread_yield.md |
| `add` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/add.nvgt |
| `clear` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/clear.nvgt |
| `is_array` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/is_array.nvgt |
| `is_null` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/is_null.nvgt |
| `is_object` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/is_object.nvgt |
| `remove` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/remove.nvgt |
| `size` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/size.nvgt |
| `stringify` | Data Manipulation/Classes/json_array/Methods | references/builtin/Data Manipulation/Classes/json_array/Methods/stringify.nvgt |
| `get_opIndex` | Data Manipulation/Classes/json_array/Operators | references/builtin/Data Manipulation/Classes/json_array/Operators/get_opIndex.nvgt |
| `opCall` | Data Manipulation/Classes/json_array/Operators | references/builtin/Data Manipulation/Classes/json_array/Operators/opCall.nvgt |
| `set_opIndex` | Data Manipulation/Classes/json_array/Operators | references/builtin/Data Manipulation/Classes/json_array/Operators/set_opIndex.nvgt |
| `empty` | Data Manipulation/Classes/json_array/Properties | references/builtin/Data Manipulation/Classes/json_array/Properties/empty.nvgt |
| `escape_unicode` | Data Manipulation/Classes/json_array/Properties | references/builtin/Data Manipulation/Classes/json_array/Properties/escape_unicode.md |
| `clear` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/clear.nvgt |
| `exists` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/exists.nvgt |
| `get_keys` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/get_keys.nvgt |
| `is_array` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/is_array.nvgt |
| `is_null` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/is_null.nvgt |
| `is_object` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/is_object.nvgt |
| `remove` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/remove.nvgt |
| `set` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/set.nvgt |
| `size` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/size.nvgt |
| `stringify` | Data Manipulation/Classes/json_object/Methods | references/builtin/Data Manipulation/Classes/json_object/Methods/stringify.nvgt |
| `get_opIndex` | Data Manipulation/Classes/json_object/Operators | references/builtin/Data Manipulation/Classes/json_object/Operators/get_opIndex.nvgt |
| `opCall` | Data Manipulation/Classes/json_object/Operators | references/builtin/Data Manipulation/Classes/json_object/Operators/opCall.nvgt |
| `set_opIndex` | Data Manipulation/Classes/json_object/Operators | references/builtin/Data Manipulation/Classes/json_object/Operators/set_opIndex.nvgt |
| `escape_unicode` | Data Manipulation/Classes/json_object/Properties | references/builtin/Data Manipulation/Classes/json_object/Properties/escape_unicode.md |
| `add_file` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/add_file.md |
| `add_memory` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/add_memory.md |
| `close` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/close.md |
| `delete_file` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/delete_file.md |
| `file_exists` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/file_exists.md |
| `get_file_name` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/get_file_name.md |
| `get_file_offset` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/get_file_offset.md |
| `get_file_size` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/get_file_size.md |
| `list_files` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/list_files.md |
| `open` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/open.md |
| `read_file` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/read_file.md |
| `set_pack_identifier` | Data Manipulation/Classes/pack/methods | references/builtin/Data Manipulation/Classes/pack/methods/set_pack_identifier.md |
| `regexp` | Data Manipulation/Classes/regexp | references/builtin/Data Manipulation/Classes/regexp/!regexp.nvgt |
| `match` | Data Manipulation/Classes/regexp/methods | references/builtin/Data Manipulation/Classes/regexp/methods/match.md |
| `pack_open_modes` | Data Manipulation/Enums | references/builtin/Data Manipulation/Enums/pack_open_modes.md |
| `regexp_options` | Data Manipulation/Enums | references/builtin/Data Manipulation/Enums/regexp_options.md |
| `ascii_to_character` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/ascii_to_character.nvgt |
| `character_to_ascii` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/character_to_ascii.nvgt |
| `join` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/join.nvgt |
| `number_to_words` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/number_to_words.nvgt |
| `pack_set_global_identifier` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/pack_set_global_identifier.md |
| `regexp_match` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/regexp_match.nvgt |
| `string_base32_decode` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/string_base32_decode.nvgt |
| `string_base32_encode` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/string_base32_encode.nvgt |
| `string_base32_normalize` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/string_base32_normalize.nvgt |
| `string_base64_decode` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/string_base64_decode.nvgt |
| `string_base64_encode` | Data Manipulation/Functions | references/builtin/Data Manipulation/Functions/string_base64_encode.nvgt |
| `pack_global_identifier` | Data Manipulation/Global Properties | references/builtin/Data Manipulation/Global Properties/pack_global_identifier.md |
| `datetime` | Date and Time/Classes/datetime | references/builtin/Date and Time/Classes/datetime/!datetime.md |
| `format` | Date and Time/Classes/datetime/Methods | references/builtin/Date and Time/Classes/datetime/Methods/format.nvgt |
| `reset` | Date and Time/Classes/datetime/Methods | references/builtin/Date and Time/Classes/datetime/Methods/reset.nvgt |
| `set` | Date and Time/Classes/datetime/Methods | references/builtin/Date and Time/Classes/datetime/Methods/set.nvgt |
| `week` | Date and Time/Classes/datetime/Methods | references/builtin/Date and Time/Classes/datetime/Methods/week.nvgt |
| `opAdd` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opAdd.md |
| `opAddAssign` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opAddAssign.md |
| `opCmp` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opCmp.md |
| `opEquals` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opEquals.md |
| `opSub` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opSub.md |
| `opSubAssign` | Date and Time/Classes/datetime/Operators | references/builtin/Date and Time/Classes/datetime/Operators/opSubAssign.md |
| `AM` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/AM.nvgt |
| `day` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/day.nvgt |
| `hour` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/hour.nvgt |
| `hour12` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/hour12.nvgt |
| `julian_day` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/julian_day.nvgt |
| `microsecond` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/microsecond.nvgt |
| `millisecond` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/millisecond.nvgt |
| `minute` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/minute.nvgt |
| `month` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/month.nvgt |
| `PM` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/PM.nvgt |
| `second` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/second.nvgt |
| `timestamp` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/timestamp.nvgt |
| `UTC_time` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/UTC_time.nvgt |
| `weekday` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/weekday.nvgt |
| `year` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/year.nvgt |
| `yearday` | Date and Time/Classes/datetime/Properties | references/builtin/Date and Time/Classes/datetime/Properties/yearday.nvgt |
| `timestamp` | Date and Time/Classes/timestamp | references/builtin/Date and Time/Classes/timestamp/!timestamp.md |
| `has_elapsed` | Date and Time/Classes/timestamp/Methods | references/builtin/Date and Time/Classes/timestamp/Methods/has_elapsed.nvgt |
| `update` | Date and Time/Classes/timestamp/Methods | references/builtin/Date and Time/Classes/timestamp/Methods/update.nvgt |
| `opImplConv` | Date and Time/Classes/timestamp/Operators | references/builtin/Date and Time/Classes/timestamp/Operators/opImplConv.md |
| `elapsed` | Date and Time/Classes/timestamp/Properties | references/builtin/Date and Time/Classes/timestamp/Properties/elapsed.nvgt |
| `UTC_time` | Date and Time/Classes/timestamp/Properties | references/builtin/Date and Time/Classes/timestamp/Properties/UTC_time.nvgt |
| `datetime_days_of_month` | Date and Time/Functions | references/builtin/Date and Time/Functions/datetime_days_of_month.nvgt |
| `datetime_is_leap_year` | Date and Time/Functions | references/builtin/Date and Time/Functions/datetime_is_leap_year.nvgt |
| `datetime_is_valid` | Date and Time/Functions | references/builtin/Date and Time/Functions/datetime_is_valid.nvgt |
| `parse_datetime` | Date and Time/Functions | references/builtin/Date and Time/Functions/parse_datetime.nvgt |
| `timestamp_from_UTC_time` | Date and Time/Functions | references/builtin/Date and Time/Functions/timestamp_from_UTC_time.nvgt |
| `DATE_DAY` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_DAY.nvgt |
| `DATE_MONTH` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_MONTH.nvgt |
| `DATE_MONTH_NAME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_MONTH_NAME.nvgt |
| `DATE_WEEKDAY` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_WEEKDAY.nvgt |
| `DATE_WEEKDAY_NAME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_WEEKDAY_NAME.nvgt |
| `DATE_YEAR` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/DATE_YEAR.nvgt |
| `SCRIPT_BUILD_TIME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/SCRIPT_BUILD_TIME.nvgt |
| `TIME_HOUR` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIME_HOUR.nvgt |
| `TIME_MINUTE` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIME_MINUTE.nvgt |
| `TIME_SECOND` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIME_SECOND.nvgt |
| `TIME_SYSTEM_RUNNING_MILLISECONDS` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIME_SYSTEM_RUNNING_MILLISECONDS.nvgt |
| `timer_default_accuracy` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/timer_default_accuracy.nvgt |
| `TIMEZONE_BASE_OFFSET` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_BASE_OFFSET.nvgt |
| `TIMEZONE_DST_NAME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_DST_NAME.nvgt |
| `TIMEZONE_DST_OFFSET` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_DST_OFFSET.nvgt |
| `TIMEZONE_NAME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_NAME.nvgt |
| `TIMEZONE_OFFSET` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_OFFSET.nvgt |
| `TIMEZONE_STANDARD_NAME` | Date and Time/Global Properties | references/builtin/Date and Time/Global Properties/TIMEZONE_STANDARD_NAME.nvgt |
| `Environment` | Environment | references/builtin/Environment/!Environment.md |
| `system_power_state` | Environment/Enums | references/builtin/Environment/Enums/system_power_state.md |
| `chdir` | Environment/Functions | references/builtin/Environment/Functions/chdir.nvgt |
| `cwdir` | Environment/Functions | references/builtin/Environment/Functions/cwdir.nvgt |
| `environment_variable_exists` | Environment/Functions | references/builtin/Environment/Functions/environment_variable_exists.nvgt |
| `expand_environment_variables` | Environment/Functions | references/builtin/Environment/Functions/expand_environment_variables.nvgt |
| `get_preferred_locales` | Environment/Functions | references/builtin/Environment/Functions/get_preferred_locales.nvgt |
| `system_power_info` | Environment/Functions | references/builtin/Environment/Functions/system_power_info.nvgt |
| `write_environment_variable` | Environment/Functions | references/builtin/Environment/Functions/write_environment_variable.nvgt |
| `COMMAND_LINE` | Environment/Global Properties | references/builtin/Environment/Global Properties/COMMAND_LINE.nvgt |
| `PLATFORM` | Environment/Global Properties | references/builtin/Environment/Global Properties/PLATFORM.nvgt |
| `PLATFORM_ARCHITECTURE` | Environment/Global Properties | references/builtin/Environment/Global Properties/PLATFORM_ARCHITECTURE.nvgt |
| `PLATFORM_DISPLAY_NAME` | Environment/Global Properties | references/builtin/Environment/Global Properties/PLATFORM_DISPLAY_NAME.nvgt |
| `PLATFORM_VERSION` | Environment/Global Properties | references/builtin/Environment/Global Properties/PLATFORM_VERSION.nvgt |
| `PROCESSOR_COUNT` | Environment/Global Properties | references/builtin/Environment/Global Properties/PROCESSOR_COUNT.nvgt |
| `system_is_chromebook` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_chromebook.nvgt |
| `system_is_DeX_mode` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_DeX_mode.nvgt |
| `system_is_mobile` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_mobile.nvgt |
| `system_is_tablet` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_tablet.nvgt |
| `system_is_unix` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_unix.nvgt |
| `system_is_windows` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_is_windows.nvgt |
| `system_node_id` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_node_id.nvgt |
| `system_node_name` | Environment/Global Properties | references/builtin/Environment/Global Properties/system_node_name.nvgt |
| `Filesystem` | Filesystem | references/builtin/Filesystem/!Filesystem.md |
| `directory_create` | Filesystem/Functions | references/builtin/Filesystem/Functions/directory_create.nvgt |
| `directory_delete` | Filesystem/Functions | references/builtin/Filesystem/Functions/directory_delete.md |
| `directory_exists` | Filesystem/Functions | references/builtin/Filesystem/Functions/directory_exists.nvgt |
| `file_copy` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_copy.md |
| `file_delete` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_delete.md |
| `file_exists` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_exists.md |
| `file_get_contents` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_get_contents.nvgt |
| `file_get_date_created` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_get_date_created.nvgt |
| `file_get_date_modified` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_get_date_modified.nvgt |
| `file_get_size` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_get_size.nvgt |
| `file_put_contents` | Filesystem/Functions | references/builtin/Filesystem/Functions/file_put_contents.nvgt |
| `get_preferences_path` | Filesystem/Functions | references/builtin/Filesystem/Functions/get_preferences_path.nvgt |
| `glob` | Filesystem/Functions | references/builtin/Filesystem/Functions/glob.nvgt |
| `DIRECTORY_APPDATA` | Filesystem/Global Properties | references/builtin/Filesystem/Global Properties/DIRECTORY_APPDATA.nvgt |
| `DIRECTORY_TEMP` | Filesystem/Global Properties | references/builtin/Filesystem/Global Properties/DIRECTORY_TEMP.nvgt |
| `aabb` | Math/Classes/aabb | references/builtin/Math/Classes/aabb/aabb.nvgt |
| `apply_scale` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/apply_scale.nvgt |
| `contains` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/contains.nvgt |
| `inflate` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/inflate.nvgt |
| `inflate_with_point` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/inflate_with_point.nvgt |
| `merge` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/merge.nvgt |
| `merge_with` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/merge_with.nvgt |
| `raycast` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/raycast.nvgt |
| `test_collision` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/test_collision.nvgt |
| `test_collision_triangle_aabb` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/test_collision_triangle_aabb.nvgt |
| `test_ray_intersect` | Math/Classes/aabb/Methods | references/builtin/Math/Classes/aabb/Methods/test_ray_intersect.nvgt |
| `center` | Math/Classes/aabb/Properties | references/builtin/Math/Classes/aabb/Properties/center.nvgt |
| `extent` | Math/Classes/aabb/Properties | references/builtin/Math/Classes/aabb/Properties/extent.nvgt |
| `max` | Math/Classes/aabb/Properties | references/builtin/Math/Classes/aabb/Properties/max.nvgt |
| `min` | Math/Classes/aabb/Properties | references/builtin/Math/Classes/aabb/Properties/min.nvgt |
| `volume` | Math/Classes/aabb/Properties | references/builtin/Math/Classes/aabb/Properties/volume.nvgt |
| `complex` | Math/Classes/complex | references/builtin/Math/Classes/complex/complex.nvgt |
| `abs` | Math/Classes/complex/Functions | references/builtin/Math/Classes/complex/Functions/abs.nvgt |
| `opAdd` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opAdd.nvgt |
| `opAddAssign` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opAddAssign.nvgt |
| `opDiv` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opDiv.nvgt |
| `opDivAssign` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opDivAssign.nvgt |
| `opEquals` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opEquals.nvgt |
| `opMul` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opMul.nvgt |
| `opMulAssign` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opMulAssign.nvgt |
| `opSub` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opSub.nvgt |
| `opSubAssign` | Math/Classes/complex/Operators | references/builtin/Math/Classes/complex/Operators/opSubAssign.nvgt |
| `i` | Math/Classes/complex/Properties | references/builtin/Math/Classes/complex/Properties/i.nvgt |
| `ir` | Math/Classes/complex/Properties | references/builtin/Math/Classes/complex/Properties/ir.nvgt |
| `ri` | Math/Classes/complex/Properties | references/builtin/Math/Classes/complex/Properties/ri.nvgt |
| `vector` | Math/Classes/vector | references/builtin/Math/Classes/vector/!vector.nvgt |
| `cross` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/cross.nvgt |
| `dot` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/dot.nvgt |
| `length` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/length.nvgt |
| `length_square` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/length_square.nvgt |
| `normalize` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/normalize.nvgt |
| `set` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/set.nvgt |
| `setToZero` | Math/Classes/vector/Methods | references/builtin/Math/Classes/vector/Methods/setToZero.nvgt |
| `opAdd` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opAdd.nvgt |
| `opAddAssign` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opAddAssign.nvgt |
| `opAssign` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opAssign.nvgt |
| `opDiv` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opDiv.nvgt |
| `opDivAssign` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opDivAssign.nvgt |
| `opEquals` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opEquals.nvgt |
| `opImplConv` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opImplConv.nvgt |
| `opIndex` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opIndex.nvgt |
| `opMul` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opMul.nvgt |
| `opMulAssign` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opMulAssign.nvgt |
| `opSub` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opSub.nvgt |
| `opSubAssign` | Math/Classes/vector/Operators | references/builtin/Math/Classes/vector/Operators/opSubAssign.nvgt |
| `is_finite` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/is_finite.nvgt |
| `is_unit` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/is_unit.nvgt |
| `is_zero` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/is_zero.nvgt |
| `max_axis` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/max_axis.nvgt |
| `max_value` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/max_value.nvgt |
| `min_axis` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/min_axis.nvgt |
| `min_value` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/min_value.nvgt |
| `x` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/x.nvgt |
| `y` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/y.nvgt |
| `z` | Math/Classes/vector/Properties | references/builtin/Math/Classes/vector/Properties/z.nvgt |
| `abs` | Math/Functions | references/builtin/Math/Functions/abs.nvgt |
| `acos` | Math/Functions | references/builtin/Math/Functions/acos.nvgt |
| `asin` | Math/Functions | references/builtin/Math/Functions/asin.nvgt |
| `atan` | Math/Functions | references/builtin/Math/Functions/atan.nvgt |
| `atan2` | Math/Functions | references/builtin/Math/Functions/atan2.nvgt |
| `ceil` | Math/Functions | references/builtin/Math/Functions/ceil.nvgt |
| `cos` | Math/Functions | references/builtin/Math/Functions/cos.nvgt |
| `cosh` | Math/Functions | references/builtin/Math/Functions/cosh.nvgt |
| `floor` | Math/Functions | references/builtin/Math/Functions/floor.nvgt |
| `fraction` | Math/Functions | references/builtin/Math/Functions/fraction.nvgt |
| `log` | Math/Functions | references/builtin/Math/Functions/log.nvgt |
| `log10` | Math/Functions | references/builtin/Math/Functions/log10.nvgt |
| `pow` | Math/Functions | references/builtin/Math/Functions/pow.nvgt |
| `round` | Math/Functions | references/builtin/Math/Functions/round.nvgt |
| `sin` | Math/Functions | references/builtin/Math/Functions/sin.nvgt |
| `sinh` | Math/Functions | references/builtin/Math/Functions/sinh.nvgt |
| `sqrt` | Math/Functions | references/builtin/Math/Functions/sqrt.nvgt |
| `tan` | Math/Functions | references/builtin/Math/Functions/tan.nvgt |
| `tanh` | Math/Functions | references/builtin/Math/Functions/tanh.nvgt |
| `tinyexpr` | Math/Functions | references/builtin/Math/Functions/tinyexpr.nvgt |
| `http` | Networking/classes/http | references/builtin/Networking/classes/http/http.nvgt |
| `get` | Networking/classes/http/Methods | references/builtin/Networking/classes/http/Methods/get.nvgt |
| `reset` | Networking/classes/http/Methods | references/builtin/Networking/classes/http/Methods/reset.nvgt |
| `wait` | Networking/classes/http/Methods | references/builtin/Networking/classes/http/Methods/wait.md |
| `progress` | Networking/classes/http/Properties | references/builtin/Networking/classes/http/Properties/progress.md |
| `response_body` | Networking/classes/http/Properties | references/builtin/Networking/classes/http/Properties/response_body.md |
| `connect` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/connect.md |
| `destroy` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/destroy.md |
| `disconnect_peer` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/disconnect_peer.md |
| `disconnect_peer_forcefully` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/disconnect_peer_forcefully.md |
| `disconnect_peer_softly` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/disconnect_peer_softly.md |
| `get_peer_address` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/get_peer_address.md |
| `get_peer_list` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/get_peer_list.md |
| `request` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/request.md |
| `send` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/send.md |
| `send_reliable` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/send_reliable.md |
| `send_unreliable` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/send_unreliable.md |
| `set_bandwidth_limits` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/set_bandwidth_limits.md |
| `setup_client` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/setup_client.md |
| `setup_local_server` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/setup_local_server.md |
| `setup_server` | Networking/classes/network/methods | references/builtin/Networking/classes/network/methods/setup_server.md |
| `active` | Networking/classes/network/properties | references/builtin/Networking/classes/network/properties/active.md |
| `bytes_received` | Networking/classes/network/properties | references/builtin/Networking/classes/network/properties/bytes_received.md |
| `bytes_sent` | Networking/classes/network/properties | references/builtin/Networking/classes/network/properties/bytes_sent.md |
| `connected_peers` | Networking/classes/network/properties | references/builtin/Networking/classes/network/properties/connected_peers.md |
| `network_event` | Networking/classes/network_event | references/builtin/Networking/classes/network_event/!network_event.md |
| `opAssign` | Networking/classes/network_event/methods | references/builtin/Networking/classes/network_event/methods/opAssign.md |
| `channel` | Networking/classes/network_event/properties | references/builtin/Networking/classes/network_event/properties/channel.md |
| `message` | Networking/classes/network_event/properties | references/builtin/Networking/classes/network_event/properties/message.md |
| `peer_id` | Networking/classes/network_event/properties | references/builtin/Networking/classes/network_event/properties/peer_id.md |
| `type` | Networking/classes/network_event/properties | references/builtin/Networking/classes/network_event/properties/type.md |
| `Event Types` | Networking/Constants | references/builtin/Networking/Constants/Event Types.md |
| `assert` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/assert.nvgt |
| `c_debug_message` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/c_debug_message.nvgt |
| `garbage_collect` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/garbage_collect.nvgt |
| `generate_profile` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/generate_profile.nvgt |
| `get_call_stack` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_call_stack.nvgt |
| `get_call_stack_size` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_call_stack_size.nvgt |
| `get_exception_file` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_exception_file.nvgt |
| `get_exception_function` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_exception_function.nvgt |
| `get_exception_info` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_exception_info.nvgt |
| `get_exception_line` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_exception_line.nvgt |
| `get_last_error` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/get_last_error.nvgt |
| `is_debugger_present` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/is_debugger_present.nvgt |
| `reset_profiler` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/reset_profiler.nvgt |
| `start_profiling` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/start_profiling.nvgt |
| `stop_profiling` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/stop_profiling.nvgt |
| `throw` | Profiling and Debugging/Functions | references/builtin/Profiling and Debugging/Functions/throw.nvgt |
| `garbage_collect_auto_frequency` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/garbage_collect_auto_frequency.nvgt |
| `garbage_collect_mode` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/garbage_collect_mode.nvgt |
| `last_exception_call_stack` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/last_exception_call_stack.nvgt |
| `SCRIPT_COMPILED` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_COMPILED.nvgt |
| `SCRIPT_CURRENT_FILE` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_CURRENT_FILE.nvgt |
| `SCRIPT_CURRENT_FUNCTION` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_CURRENT_FUNCTION.nvgt |
| `SCRIPT_CURRENT_LINE` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_CURRENT_LINE.nvgt |
| `SCRIPT_EXECUTABLE` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_EXECUTABLE.nvgt |
| `SCRIPT_MAIN_PATH` | Profiling and Debugging/Global Properties | references/builtin/Profiling and Debugging/Global Properties/SCRIPT_MAIN_PATH.nvgt |
| `random_gamerand` | Pseudorandom Generation/Classes | references/builtin/Pseudorandom Generation/Classes/random_gamerand.md |
| `random_pcg` | Pseudorandom Generation/Classes | references/builtin/Pseudorandom Generation/Classes/random_pcg.md |
| `random_well` | Pseudorandom Generation/Classes | references/builtin/Pseudorandom Generation/Classes/random_well.md |
| `random_xorshift` | Pseudorandom Generation/Classes | references/builtin/Pseudorandom Generation/Classes/random_xorshift.md |
| `random interface` | Pseudorandom Generation/Classes/!random_interface | references/builtin/Pseudorandom Generation/Classes/!random_interface/!random interface.md |
| `next` | Pseudorandom Generation/Classes/!random_interface/Methods | references/builtin/Pseudorandom Generation/Classes/!random_interface/Methods/next.nvgt |
| `nextf` | Pseudorandom Generation/Classes/!random_interface/Methods | references/builtin/Pseudorandom Generation/Classes/!random_interface/Methods/nextf.nvgt |
| `range` | Pseudorandom Generation/Classes/!random_interface/Methods | references/builtin/Pseudorandom Generation/Classes/!random_interface/Methods/range.nvgt |
| `seed` | Pseudorandom Generation/Classes/!random_interface/Methods | references/builtin/Pseudorandom Generation/Classes/!random_interface/Methods/seed.nvgt |
| `random` | Pseudorandom Generation/Functions | references/builtin/Pseudorandom Generation/Functions/random.nvgt |
| `random_bool` | Pseudorandom Generation/Functions | references/builtin/Pseudorandom Generation/Functions/random_bool.nvgt |
| `random_character` | Pseudorandom Generation/Functions | references/builtin/Pseudorandom Generation/Functions/random_character.nvgt |
| `random_seed` | Pseudorandom Generation/Functions | references/builtin/Pseudorandom Generation/Functions/random_seed.nvgt |
| `string_aes_decrypt` | Security/Functions | references/builtin/Security/Functions/string_aes_decrypt.nvgt |
| `string_aes_encrypt` | Security/Functions | references/builtin/Security/Functions/string_aes_encrypt.nvgt |
| `memory_scan_detected` | Security/Global Properties | references/builtin/Security/Global Properties/memory_scan_detected.md |
| `speed_hack_detected` | Security/Global Properties | references/builtin/Security/Global Properties/speed_hack_detected.md |
| `Text-To-Speech` | Text-To-Speech | references/builtin/Text-To-Speech/!Text-To-Speech.md |
| `tts_voice` | Text-To-Speech/classes/tts_voice | references/builtin/Text-To-Speech/classes/tts_voice/!tts_voice.nvgt |
| `get_speaking` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/get_speaking.nvgt |
| `get_voice_count` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/get_voice_count.nvgt |
| `get_voice_name` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/get_voice_name.nvgt |
| `get_volume` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/get_volume.nvgt |
| `list_voices` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/list_voices.nvgt |
| `refresh` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/refresh.md |
| `set_rate` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/set_rate.nvgt |
| `set_voice` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/set_voice.md |
| `set_volume` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/set_volume.nvgt |
| `speak` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/speak.nvgt |
| `speak_interrupt` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/speak_interrupt.nvgt |
| `speak_to_file` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/speak_to_file.nvgt |
| `stop` | Text-To-Speech/classes/tts_voice/Methods | references/builtin/Text-To-Speech/classes/tts_voice/Methods/stop.nvgt |
| `speaking` | Text-To-Speech/classes/tts_voice/Properties | references/builtin/Text-To-Speech/classes/tts_voice/Properties/speaking.nvgt |
| `voice` | Text-To-Speech/classes/tts_voice/Properties | references/builtin/Text-To-Speech/classes/tts_voice/Properties/voice.nvgt |
| `voice_count` | Text-To-Speech/classes/tts_voice/Properties | references/builtin/Text-To-Speech/classes/tts_voice/Properties/voice_count.nvgt |
| `screen_reader_braille` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_braille.nvgt |
| `screen_reader_detect` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_detect.nvgt |
| `screen_reader_has_braille` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_has_braille.nvgt |
| `screen_reader_has_speech` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_has_speech.nvgt |
| `screen_reader_is_speaking` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_is_speaking.nvgt |
| `screen_reader_output` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_output.nvgt |
| `screen_reader_speak` | Text-To-Speech/Functions | references/builtin/Text-To-Speech/Functions/screen_reader_speak.nvgt |
| `SCREEN_READER_AVAILABLE` | Text-To-Speech/Global Properties | references/builtin/Text-To-Speech/Global Properties/SCREEN_READER_AVAILABLE.nvgt |
| `User Interface` | User Interface | references/builtin/User Interface/!User Interface.md |
| `key_code` | User Interface/Enums | references/builtin/User Interface/Enums/key_code.md |
| `key_modifier` | User Interface/Enums | references/builtin/User Interface/Enums/key_modifier.nvgt |
| `message_box_flags` | User Interface/Enums | references/builtin/User Interface/Enums/message_box_flags.md |
| `touch_device_type` | User Interface/Enums | references/builtin/User Interface/Enums/touch_device_type.md |
| `window_flags` | User Interface/Enums | references/builtin/User Interface/Enums/window_flags.md |
| `alert` | User Interface/Functions | references/builtin/User Interface/Functions/alert.nvgt |
| `android_request_permission` | User Interface/Functions | references/builtin/User Interface/Functions/android_request_permission.md |
| `android_show_toast` | User Interface/Functions | references/builtin/User Interface/Functions/android_show_toast.md |
| `clipboard_get_text` | User Interface/Functions | references/builtin/User Interface/Functions/clipboard_get_text.nvgt |
| `clipboard_set_raw_text` | User Interface/Functions | references/builtin/User Interface/Functions/clipboard_set_raw_text.nvgt |
| `clipboard_set_text` | User Interface/Functions | references/builtin/User Interface/Functions/clipboard_set_text.nvgt |
| `destroy_window` | User Interface/Functions | references/builtin/User Interface/Functions/destroy_window.nvgt |
| `exit` | User Interface/Functions | references/builtin/User Interface/Functions/exit.nvgt |
| `focus_window` | User Interface/Functions | references/builtin/User Interface/Functions/focus_window.nvgt |
| `get_characters` | User Interface/Functions | references/builtin/User Interface/Functions/get_characters.nvgt |
| `get_touch_device_name` | User Interface/Functions | references/builtin/User Interface/Functions/get_touch_device_name.md |
| `get_touch_device_type` | User Interface/Functions | references/builtin/User Interface/Functions/get_touch_device_type.md |
| `get_touch_devices` | User Interface/Functions | references/builtin/User Interface/Functions/get_touch_devices.nvgt |
| `get_window_height` | User Interface/Functions | references/builtin/User Interface/Functions/get_window_height.nvgt |
| `get_window_os_handle` | User Interface/Functions | references/builtin/User Interface/Functions/get_window_os_handle.nvgt |
| `get_window_text` | User Interface/Functions | references/builtin/User Interface/Functions/get_window_text.nvgt |
| `get_window_width` | User Interface/Functions | references/builtin/User Interface/Functions/get_window_width.nvgt |
| `HIDE_WINDOW` | User Interface/Functions | references/builtin/User Interface/Functions/HIDE_WINDOW.NVGt |
| `idle_ticks` | User Interface/Functions | references/builtin/User Interface/Functions/idle_ticks.nvgt |
| `info_box` | User Interface/Functions | references/builtin/User Interface/Functions/info_box.nvgt |
| `input_box` | User Interface/Functions | references/builtin/User Interface/Functions/input_box.nvgt |
| `install_keyhook` | User Interface/Functions | references/builtin/User Interface/Functions/install_keyhook.nvgt |
| `is_console_available` | User Interface/Functions | references/builtin/User Interface/Functions/is_console_available.nvgt |
| `is_window_active` | User Interface/Functions | references/builtin/User Interface/Functions/is_window_active.nvgt |
| `is_window_hidden` | User Interface/Functions | references/builtin/User Interface/Functions/is_window_hidden.nvgt |
| `key_down` | User Interface/Functions | references/builtin/User Interface/Functions/key_down.nvgt |
| `key_pressed` | User Interface/Functions | references/builtin/User Interface/Functions/key_pressed.nvgt |
| `key_released` | User Interface/Functions | references/builtin/User Interface/Functions/key_released.nvgt |
| `key_repeating` | User Interface/Functions | references/builtin/User Interface/Functions/key_repeating.nvgt |
| `key_up` | User Interface/Functions | references/builtin/User Interface/Functions/key_up.nvgt |
| `keys_down` | User Interface/Functions | references/builtin/User Interface/Functions/keys_down.md |
| `keys_pressed` | User Interface/Functions | references/builtin/User Interface/Functions/keys_pressed.md |
| `keys_released` | User Interface/Functions | references/builtin/User Interface/Functions/keys_released.md |
| `message_box` | User Interface/Functions | references/builtin/User Interface/Functions/message_box.nvgt |
| `mouse_down` | User Interface/Functions | references/builtin/User Interface/Functions/mouse_down.nvgt |
| `mouse_pressed` | User Interface/Functions | references/builtin/User Interface/Functions/mouse_pressed.nvgt |
| `open_file_dialog` | User Interface/Functions | references/builtin/User Interface/Functions/open_file_dialog.nvgt |
| `query_touch_device` | User Interface/Functions | references/builtin/User Interface/Functions/query_touch_device.nvgt |
| `question` | User Interface/Functions | references/builtin/User Interface/Functions/question.nvgt |
| `refresh_window` | User Interface/Functions | references/builtin/User Interface/Functions/refresh_window.md |
| `run` | User Interface/Functions | references/builtin/User Interface/Functions/run.nvgt |
| `save_file_dialog` | User Interface/Functions | references/builtin/User Interface/Functions/save_file_dialog.nvgt |
| `select_folder_dialog` | User Interface/Functions | references/builtin/User Interface/Functions/select_folder_dialog.nvgt |
| `set_application_name` | User Interface/Functions | references/builtin/User Interface/Functions/set_application_name.md |
| `show_window` | User Interface/Functions | references/builtin/User Interface/Functions/show_window.nvgt |
| `total_keys_down` | User Interface/Functions | references/builtin/User Interface/Functions/total_keys_down.nvgt |
| `uninstall_keyhook` | User Interface/Functions | references/builtin/User Interface/Functions/uninstall_keyhook.nvgt |
| `urlopen` | User Interface/Functions | references/builtin/User Interface/Functions/urlopen.nvgt |
| `wait` | User Interface/Functions | references/builtin/User Interface/Functions/wait.nvgt |

## Standard include library (`#include "..."`)

| Symbol | Area | File |
| --- | --- | --- |
| `audio_form` | Auditory User Interface (form.nvgt)/Classes/audio_form | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/!audio_form.md |
| `activate_progress_timer` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/activate_progress_timer.md |
| `add_list_item` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/add_list_item.md |
| `clear_list` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/clear_list.md |
| `create_button` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_button.md |
| `create_checkbox` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_checkbox.md |
| `create_input_box` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_input_box.md |
| `create_keyboard_area` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_keyboard_area.md |
| `create_link` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_link.md |
| `create_list` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_list.md |
| `create_progress_bar` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_progress_bar.md |
| `create_slider` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_slider.md |
| `create_status_bar` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_status_bar.md |
| `create_subform` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_subform.nvgt |
| `create_window` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/create_window.nvgt |
| `delete_control` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/delete_control.md |
| `delete_list_item` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/delete_list_item.md |
| `delete_list_selections` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/delete_list_selections.md |
| `edit_list_item` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/edit_list_item.md |
| `edit_list_item_id` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/edit_list_item_id.md |
| `focus` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/focus.md |
| `focus_interrupt` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/focus_interrupt.md |
| `focus_silently` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/focus_silently.md |
| `get_cancel_button` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_cancel_button.md |
| `get_caption` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_caption.md |
| `get_checked_list_items` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_checked_list_items.md |
| `get_control_count` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_control_count.md |
| `get_control_type` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_control_type.md |
| `get_current_focus` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_current_focus.md |
| `get_custom_type` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_custom_type.md |
| `get_default_button` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_default_button.md |
| `get_last_error` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_last_error.md |
| `get_line_column` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_line_column.md |
| `get_line_number` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_line_number.md |
| `get_link_url` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_link_url.md |
| `get_list_count` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_count.md |
| `get_list_index_by_id` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_index_by_id.md |
| `get_list_item` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_item.md |
| `get_list_item_id` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_item_id.md |
| `get_list_position` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_position.md |
| `get_list_selections` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_list_selections.md |
| `get_progress` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_progress.md |
| `get_slider` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_slider.md |
| `get_slider_maximum_value` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_slider_maximum_value.md |
| `get_slider_minimum_value` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_slider_minimum_value.md |
| `get_text` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/get_text.md |
| `has_custom_type` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/has_custom_type.md |
| `is_checked` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_checked.md |
| `is_disallowed_char` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_disallowed_char.md |
| `is_enabled` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_enabled.md |
| `is_list_item_checked` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_list_item_checked.md |
| `is_multiline` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_multiline.md |
| `is_pressed` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_pressed.md |
| `is_read_only` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_read_only.md |
| `is_visible` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/is_visible.md |
| `monitor` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/monitor.md |
| `pause_progress_timer` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/pause_progress_timer.md |
| `set_button_attributes` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_button_attributes.md |
| `set_caption` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_caption.md |
| `set_checkbox_mark` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_checkbox_mark.md |
| `set_custom_type` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_custom_type.md |
| `set_default_controls` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_default_controls.md |
| `set_default_keyboard_echo` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_default_keyboard_echo.md |
| `set_disallowed_chars` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_disallowed_chars.md |
| `set_enable_go_to_index` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_enable_go_to_index.md |
| `set_enable_search` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_enable_search.md |
| `set_keyboard_echo` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_keyboard_echo.md |
| `set_link_url` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_link_url.md |
| `set_list_multinavigation` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_list_multinavigation.md |
| `set_list_position` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_list_position.md |
| `set_list_properties` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_list_properties.md |
| `set_progress` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_progress.md |
| `set_slider` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_slider.md |
| `set_state` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_state.md |
| `set_subform` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_subform.nvgt |
| `set_text` | Auditory User Interface (form.nvgt)/Classes/audio_form/Methods | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Methods/set_text.md |
| `active` | Auditory User Interface (form.nvgt)/Classes/audio_form/Properties | references/include/Auditory User Interface (form.nvgt)/Classes/audio_form/Properties/active.md |
| `audioform_errorcodes` | Auditory User Interface (form.nvgt)/Enums | references/include/Auditory User Interface (form.nvgt)/Enums/audioform_errorcodes.md |
| `control_event_type` | Auditory User Interface (form.nvgt)/Enums | references/include/Auditory User Interface (form.nvgt)/Enums/control_event_type.md |
| `control_types` | Auditory User Interface (form.nvgt)/Enums | references/include/Auditory User Interface (form.nvgt)/Enums/control_types.md |
| `text_edit_mode_constants` | Auditory User Interface (form.nvgt)/Enums | references/include/Auditory User Interface (form.nvgt)/Enums/text_edit_mode_constants.md |
| `text_entry_speech_flags` | Auditory User Interface (form.nvgt)/Enums | references/include/Auditory User Interface (form.nvgt)/Enums/text_entry_speech_flags.md |
| `audioform_input_disable_ralt` | Auditory User Interface (form.nvgt)/Global Properties | references/include/Auditory User Interface (form.nvgt)/Global Properties/audioform_input_disable_ralt.md |
| `audioform_keyboard_echo` | Auditory User Interface (form.nvgt)/Global Properties | references/include/Auditory User Interface (form.nvgt)/Global Properties/audioform_keyboard_echo.md |
| `audioform_word_separators` | Auditory User Interface (form.nvgt)/Global Properties | references/include/Auditory User Interface (form.nvgt)/Global Properties/audioform_word_separators.md |
| `clear_compiled_basename` | Basename Clearing (clear_compiled_basename.nvgt) | references/include/Basename Clearing (clear_compiled_basename.nvgt)/!clear_compiled_basename.md |
| `BGT Compatibility Layer` | BGT Compatibility Layer (bgt_compat.nvgt) | references/include/BGT Compatibility Layer (bgt_compat.nvgt)/!BGT Compatibility Layer.md |
| `basic_character_controller` | Character Controller Subsystem (basic_character_controller.nvgt) | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/basic_character_controller.md |
| `realign_to_nearest_degree` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/realign_to_nearest_degree.md |
| `rotate_left_by` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/rotate_left_by.md |
| `rotate_right_by` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/rotate_right_by.md |
| `toggle_crouch` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/toggle_crouch.md |
| `turn_around` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/turn_around.md |
| `update` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Methods/update.md |
| `grounded` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties/grounded.md |
| `handedness` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties/handedness.md |
| `target_yaw` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties/target_yaw.md |
| `yaw` | Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Classes/basic_character_controller/Properties/yaw.md |
| `coordinate_handedness` | Character Controller Subsystem (basic_character_controller.nvgt)/Enums | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Enums/coordinate_handedness.md |
| `nearest_compass_point` | Character Controller Subsystem (basic_character_controller.nvgt)/Functions | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Functions/nearest_compass_point.md |
| `snap_to_degree` | Character Controller Subsystem (basic_character_controller.nvgt)/Functions | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Functions/snap_to_degree.md |
| `translate_yaw_to_direction` | Character Controller Subsystem (basic_character_controller.nvgt)/Functions | references/include/Character Controller Subsystem (basic_character_controller.nvgt)/Functions/translate_yaw_to_direction.md |
| `Character Rotation` | Character Rotation (rotation.nvgt) | references/include/Character Rotation (rotation.nvgt)/!Character Rotation.md |
| `calculate_theta` | Character Rotation (rotation.nvgt)/Functions | references/include/Character Rotation (rotation.nvgt)/Functions/calculate_theta.nvgt |
| `get_1d_distance` | Character Rotation (rotation.nvgt)/Functions | references/include/Character Rotation (rotation.nvgt)/Functions/get_1d_distance.nvgt |
| `get_2d_distance` | Character Rotation (rotation.nvgt)/Functions | references/include/Character Rotation (rotation.nvgt)/Functions/get_2d_distance.nvgt |
| `get_3d_distance` | Character Rotation (rotation.nvgt)/Functions | references/include/Character Rotation (rotation.nvgt)/Functions/get_3d_distance.nvgt |
| `direction constants` | Character Rotation (rotation.nvgt)/Global Properties | references/include/Character Rotation (rotation.nvgt)/Global Properties/!direction constants.md |
| `pi` | Character Rotation (rotation.nvgt)/Global Properties | references/include/Character Rotation (rotation.nvgt)/Global Properties/pi.nvgt |
| `dictionary retrieval` | Dictionary Retrieval (dget.nvgt) | references/include/Dictionary Retrieval (dget.nvgt)/!dictionary retrieval.md |
| `dgetb` | Dictionary Retrieval (dget.nvgt)/functions | references/include/Dictionary Retrieval (dget.nvgt)/functions/dgetb.md |
| `dgetn` | Dictionary Retrieval (dget.nvgt)/functions | references/include/Dictionary Retrieval (dget.nvgt)/functions/dgetn.md |
| `dgets` | Dictionary Retrieval (dget.nvgt)/functions | references/include/Dictionary Retrieval (dget.nvgt)/functions/dgets.md |
| `dgetsl` | Dictionary Retrieval (dget.nvgt)/functions | references/include/Dictionary Retrieval (dget.nvgt)/functions/dgetsl.md |
| `INI Reader and Writer` | INI Reader and Writer (ini.nvgt) | references/include/INI Reader and Writer (ini.nvgt)/!INI Reader and Writer.md |
| `ini` | INI Reader and Writer (ini.nvgt)/classes/ini | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/!ini.md |
| `clear_section` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/clear_section.md |
| `create_section` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/create_section.md |
| `delete_key` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/delete_key.md |
| `delete_section` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/delete_section.md |
| `dump` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/dump.md |
| `get_bool` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/get_bool.md |
| `get_double` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/get_double.md |
| `get_error_line` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/get_error_line.md |
| `get_error_text` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/get_error_text.md |
| `get_string` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/get_string.md |
| `is_empty` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/is_empty.md |
| `key_exists` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/key_exists.md |
| `list_keys` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/list_keys.md |
| `list_sections` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/list_sections.md |
| `list_wildcard_sections` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/list_wildcard_sections.md |
| `load` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/load.md |
| `load_string` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/load_string.md |
| `reset` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/reset.md |
| `save` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/save.md |
| `save_robust` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/save_robust.md |
| `section_exists` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/section_exists.md |
| `set_bool` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/set_bool.md |
| `set_double` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/set_double.md |
| `set_string` | INI Reader and Writer (ini.nvgt)/classes/ini/methods | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/methods/set_string.md |
| `loaded_filename` | INI Reader and Writer (ini.nvgt)/classes/ini/properties | references/include/INI Reader and Writer (ini.nvgt)/classes/ini/properties/loaded_filename.md |
| `Instance Management` | Instance Management (instance.nvgt) | references/include/Instance Management (instance.nvgt)/!Instance Management.md |
| `wait_until_standalone` | Instance Management (instance.nvgt)/Classes/instance/Methods | references/include/Instance Management (instance.nvgt)/Classes/instance/Methods/wait_until_standalone.md |
| `is_already_running` | Instance Management (instance.nvgt)/Classes/instance/Properties | references/include/Instance Management (instance.nvgt)/Classes/instance/Properties/is_already_running.nvgt |
| `sound_pool` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/!sound_pool.md |
| `destroy_all` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/destroy_all.md |
| `destroy_sound` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/destroy_sound.md |
| `pause_all` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/pause_all.md |
| `pause_sound` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/pause_sound.md |
| `play_1d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/play_1d.md |
| `play_2d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/play_2d.md |
| `play_3d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/play_3d.md |
| `play_extended` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/play_extended.md |
| `play_stationary` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/play_stationary.md |
| `resume_all` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/resume_all.md |
| `resume_sound` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/resume_sound.md |
| `sound_is_active` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/sound_is_active.md |
| `sound_is_playing` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/sound_is_playing.md |
| `update_listener_1d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_listener_1d.md |
| `update_listener_2d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_listener_2d.md |
| `update_listener_3d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_listener_3d.md |
| `update_sound_1d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_1d.md |
| `update_sound_2d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_2d.md |
| `update_sound_3d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_3d.md |
| `update_sound_range_1d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_range_1d.md |
| `update_sound_range_2d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_range_2d.md |
| `update_sound_range_3d` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_range_3d.md |
| `update_sound_start_values` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Methods/update_sound_start_values.md |
| `behind_pitch_decrease` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties/behind_pitch_decrease.md |
| `max_distance` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties/max_distance.md |
| `pan_step` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties/pan_step.md |
| `volume_step` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties/volume_step.md |
| `y_is_elevation` | Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties | references/include/Legacy Sound Manager (sound_pool.nvgt)/Classes/sound_pool/Properties/y_is_elevation.md |
| `menu` | Menu Interface (menu.nvgt)/Classes/menu | references/include/Menu Interface (menu.nvgt)/Classes/menu/menu.md |
| `add_item` | Menu Interface (menu.nvgt)/Classes/menu/Methods | references/include/Menu Interface (menu.nvgt)/Classes/menu/Methods/add_item.md |
| `intro` | Menu Interface (menu.nvgt)/Classes/menu/Methods | references/include/Menu Interface (menu.nvgt)/Classes/menu/Methods/intro.md |
| `monitor` | Menu Interface (menu.nvgt)/Classes/menu/Methods | references/include/Menu Interface (menu.nvgt)/Classes/menu/Methods/monitor.md |
| `reset` | Menu Interface (menu.nvgt)/Classes/menu/Methods | references/include/Menu Interface (menu.nvgt)/Classes/menu/Methods/reset.md |
| `automatic_intro` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/automatic_intro.md |
| `click_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/click_sound.md |
| `close_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/close_sound.md |
| `edge_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/edge_sound.md |
| `focus_first_item` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/focus_first_item.md |
| `intro_text` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/intro_text.md |
| `open_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/open_sound.md |
| `pack_file` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/pack_file.md |
| `select_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/select_sound.md |
| `wrap` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/wrap.md |
| `wrap_delay` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/wrap_delay.md |
| `wrap_sound` | Menu Interface (menu.nvgt)/Classes/menu/Properties | references/include/Menu Interface (menu.nvgt)/Classes/menu/Properties/wrap_sound.md |
| `Music System` | Music System (music.nvgt) | references/include/Music System (music.nvgt)/!Music System.md |
| `loop` | Music System (music.nvgt)/classes/music_manager/methods | references/include/Music System (music.nvgt)/classes/music_manager/methods/loop.md |
| `play` | Music System (music.nvgt)/classes/music_manager/methods | references/include/Music System (music.nvgt)/classes/music_manager/methods/play.md |
| `set_load_callback` | Music System (music.nvgt)/classes/music_manager/methods | references/include/Music System (music.nvgt)/classes/music_manager/methods/set_load_callback.md |
| `stop` | Music System (music.nvgt)/classes/music_manager/methods | references/include/Music System (music.nvgt)/classes/music_manager/methods/stop.md |
| `playing` | Music System (music.nvgt)/classes/music_manager/properties | references/include/Music System (music.nvgt)/classes/music_manager/properties/playing.md |
| `volume` | Music System (music.nvgt)/classes/music_manager/properties | references/include/Music System (music.nvgt)/classes/music_manager/properties/volume.md |
| `number_speaker` | Number Speaking (number_speaker.nvgt)/classes/number_speaker | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/!number_speaker.nvgt |
| `set_sound_object` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods/set_sound_object.nvgt |
| `speak` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods/!speak.nvgt |
| `speak_next` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods/!speak_next.nvgt |
| `speak_wait` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods/!speak_wait.nvgt |
| `stop` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/methods/!stop.nvgt |
| `append` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties/!append.md |
| `include_and` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties/include_and.md |
| `pack_file` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties/pack_file.md |
| `prepend` | Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties | references/include/Number Speaking (number_speaker.nvgt)/classes/number_speaker/properties/!prepend.md |
| `size conversions` | Size Conversions (size.nvgt) | references/include/Size Conversions (size.nvgt)/!size conversions.md |
| `size_to_string` | Size Conversions (size.nvgt)/Functions | references/include/Size Conversions (size.nvgt)/Functions/size_to_string.nvgt |
| `GIGABYTES` | Size Conversions (size.nvgt)/Global Properties | references/include/Size Conversions (size.nvgt)/Global Properties/GIGABYTES.nvgt |
| `KILOBYTES` | Size Conversions (size.nvgt)/Global Properties | references/include/Size Conversions (size.nvgt)/Global Properties/KILOBYTES.nvgt |
| `MEGABYTES` | Size Conversions (size.nvgt)/Global Properties | references/include/Size Conversions (size.nvgt)/Global Properties/MEGABYTES.nvgt |
| `SIZE_TO_STRING_UNITS` | Size Conversions (size.nvgt)/Global Properties | references/include/Size Conversions (size.nvgt)/Global Properties/SIZE_TO_STRING_UNITS.nvgt |
| `TERABYTES` | Size Conversions (size.nvgt)/Global Properties | references/include/Size Conversions (size.nvgt)/Global Properties/TERABYTES.nvgt |
| `add` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods/add.md |
| `delete` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods/delete.md |
| `exists` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods/exists.md |
| `reset` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods/reset.md |
| `update` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Methods/update.md |
| `opIndex` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Operators | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Operators/opIndex.md |
| `size` | Statistic Management (stat_set.nvgt)/Classes/stat_set/Properties | references/include/Statistic Management (stat_set.nvgt)/Classes/stat_set/Properties/size.md |
| `token_gen` | Token Generation (token_gen.nvgt) | references/include/Token Generation (token_gen.nvgt)/!token_gen.md |
| `token_gen_flag` | Token Generation (token_gen.nvgt)/Enums | references/include/Token Generation (token_gen.nvgt)/Enums/token_gen_flag.md |
| `generate_custom_token` | Token Generation (token_gen.nvgt)/Functions | references/include/Token Generation (token_gen.nvgt)/Functions/generate_custom_token.nvgt |
| `generate_token` | Token Generation (token_gen.nvgt)/Functions | references/include/Token Generation (token_gen.nvgt)/Functions/!generate_token.nvgt |
| `touch_gesture_manager` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/!touch_gesture_manager.nvgt |
| `add_touch_interface` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods/add_touch_interface.md |
| `clear_touch_interfaces` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods/clear_touch_interfaces.md |
| `is_available` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods/is_available.nvgt |
| `monitor` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods/monitor.md |
| `set_touch_interfaces` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Methods/set_touch_interfaces.md |
| `flick_velocity_threshold` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/flick_velocity_threshold.md |
| `hold_jitter_threshold` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/hold_jitter_threshold.md |
| `long_press_time` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/long_press_time.md |
| `multi_tap_timeout` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/multi_tap_timeout.md |
| `swipe_min_dist` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/swipe_min_dist.md |
| `swipe_segment_threshold` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/swipe_segment_threshold.md |
| `tap_max_delay` | touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_gesture_manager/Properties/tap_max_delay.md |
| `touch_interface` | touch gesture management (touch.nvgt)/Classes/touch_interface | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/!touch_interface.md |
| `on_compound_swipe` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_compound_swipe.md |
| `on_double_tap` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_double_tap.md |
| `on_flick` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_flick.md |
| `on_hold` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_hold.md |
| `on_long_press` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_long_press.md |
| `on_released_finger` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_released_finger.md |
| `on_single_tap` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_single_tap.md |
| `on_swipe_down` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_down.md |
| `on_swipe_down_left` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_down_left.md |
| `on_swipe_down_right` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_down_right.md |
| `on_swipe_left` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_left.md |
| `on_swipe_right` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_right.md |
| `on_swipe_up` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_up.md |
| `on_swipe_up_left` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_up_left.md |
| `on_swipe_up_right` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_swipe_up_right.md |
| `on_triple_tap` | touch gesture management (touch.nvgt)/Classes/touch_interface/Methods | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Methods/on_triple_tap.md |
| `allow_passthrough` | touch gesture management (touch.nvgt)/Classes/touch_interface/Properties | references/include/touch gesture management (touch.nvgt)/Classes/touch_interface/Properties/allow_passthrough.md |
| `touch_keyboard_interface` | touch gesture management (touch.nvgt)/Classes/touch_keyboard_interface | references/include/touch gesture management (touch.nvgt)/Classes/touch_keyboard_interface/!touch_keyboard_interface.md |
| `TOUCH_UNCOORDINATED` | touch gesture management (touch.nvgt)/Constants | references/include/touch gesture management (touch.nvgt)/Constants/TOUCH_UNCOORDINATED.md |
| `swipe_touch_directions` | touch gesture management (touch.nvgt)/Enums | references/include/touch gesture management (touch.nvgt)/Enums/swipe_touch_directions.md |
| `touch_enable_8_way_swipes` | touch gesture management (touch.nvgt)/Global Properties | references/include/touch gesture management (touch.nvgt)/Global Properties/touch_enable_8_way_swipes.md |
| `settings` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/!settings.md |
| `close` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/close.md |
| `dump` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/dump.md |
| `exists` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/exists.md |
| `has_other_products` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/has_other_products.md |
| `read_number` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/read_number.md |
| `read_string` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/read_string.md |
| `remove_product` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/remove_product.md |
| `remove_value` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/remove_value.md |
| `setup` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/!setup.md |
| `write_number` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/write_number.md |
| `write_string` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Methods/write_string.md |
| `active` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/active.md |
| `company_name` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/company_name.md |
| `encryption_key` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/encryption_key.md |
| `instant_save` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/instant_save.md |
| `local_path` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/local_path.md |
| `product` | User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties | references/include/User Data Storage and Retrieval (settings.nvgt)/Classes/settings/Properties/product.md |

## Plugins (require `#pragma plugin`)

| Symbol | Area | File |
| --- | --- | --- |
| `bytes_downloaded` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/bytes_downloaded.md |
| `bytes_uploaded` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/bytes_uploaded.md |
| `complete` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/complete.md |
| `download_percent` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/download_percent.md |
| `download_size` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/download_size.md |
| `follow_redirects` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/follow_redirects.md |
| `in_progress` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/in_progress.md |
| `max_redirects` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/max_redirects.md |
| `no_curl` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/no_curl.md |
| `status_code` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/status_code.md |
| `upload_percent` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/upload_percent.md |
| `upload_size` | nvgt_curl/classes/internet_request/properties | references/plugin/nvgt_curl/classes/internet_request/properties/upload_size.md |
| `curl_url_decode` | nvgt_curl/functions | references/plugin/nvgt_curl/functions/curl_url_decode.md |
| `curl_url_encode` | nvgt_curl/functions | references/plugin/nvgt_curl/functions/curl_url_encode.md |
| `systemd_notify` | systemd_notify | references/plugin/systemd_notify/systemd_notify.nvgt |

