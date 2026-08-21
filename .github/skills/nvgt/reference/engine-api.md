# NVGT engine API (complete, generated)

Generated from NVGT 0.90.0-dev by asking the engine to serialize its own
registered API. Every signature here is exact. This file is the authority
when the prose documentation is silent, incomplete or out of date.

Search it with Grep rather than reading it start to finish.

---

## Global functions

- `ARCHITECTURE get_PROCESSOR_ARCHITECTURE() property;`
- `OPERATING_SYSTEM get_OS() property;`
- `aabb aabb_create_from_triangle(const array<vector>@ points);`
- `array<int>@ keys_down();`
- `array<int>@ keys_pressed();`
- `array<int>@ keys_released();`
- `array<string>@ find_directories(const string&in pattern);`
- `array<string>@ find_files(const string&in pattern);`
- `array<string>@ get_preferred_locales();`
- `array<string>@ glob(const string&in pattern, glob_options options = GLOB_DEFAULT);`
- `array<touch_finger>@ query_touch_device(uint64 device_id = 0);`
- `array<uint64>@ get_touch_devices();`
- `array<uint>@ get_keyboards();`
- `array<uint>@ get_mice();`
- `audio_engine@ get_sound_default_engine() property;`
- `audio_error_state get_SOUNDSYSTEM_LAST_ERROR() property;`
- `bool android_request_permission(const string&in permission, android_permission_request_callback@ callback = null, const string&in callback_data = "");`
- `bool android_show_toast(const string&in message, int duration, int gravity = -1, int x_offset = 0, int y_offset = 0);`
- `bool boxes_intersect(float, float, float, float, float, float, float, float, float, float);`
- `bool chdir(const string&in directory);`
- `bool clipboard_set_raw_text(const string&in text);`
- `bool clipboard_set_text(const string&in text);`
- `bool close_to(double, double, double = 0.0000000001);`
- `bool close_to(float, float, float = 0.00001f);`
- `bool datetime_is_leap_year(int year);`
- `bool datetime_is_valid(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0, int microsecond = 0);`
- `bool datetime_is_valid_format(const string&in datetime);`
- `bool datetime_is_valid_format_string(const string&in fmt);`
- `bool destroy_window();`
- `bool directory_create(const string&in path);`
- `bool directory_delete(const string&in path, bool recursive = true);`
- `bool directory_exists(const string&in path);`
- `bool environment_variable_exists(const string&in variable);`
- `bool file_copy(const string&in source, const string&in destination, bool);`
- `bool file_delete(const string&in path);`
- `bool file_exists(const string&in path);`
- `bool file_hard_link(const string&in source, const string&in destination);`
- `bool file_move(const string&in source, const string&in destination);`
- `bool file_put_contents(const string&in filename, const string&in contents, bool append = false);`
- `bool file_touch(const string&in path, const timestamp&in new_time = timestamp());`
- `bool fnmatch(const string&in text, const string&in pattern);`
- `bool focus_window();`
- `bool get_KEYBOARD_AVAILABLE() property;`
- `bool get_MOUSE_AVAILABLE() property;`
- `bool get_SCREEN_KEYBOARD_SUPPORTED() property;`
- `bool get_SCREEN_READER_AVAILABLE() property;`
- `bool get_SCRIPT_COMPILED() property;`
- `bool get_cursor_visible() property;`
- `bool get_mouse_grab() property;`
- `bool get_sound_3d_attenuator_enabled(int attenuator_id);`
- `bool get_sound_3d_panner_enabled(int panner_id);`
- `bool get_sound_global_hrtf() property;`
- `bool get_system_is_DeX_mode() property;`
- `bool get_system_is_chromebook() property;`
- `bool get_system_is_mobile() property;`
- `bool get_system_is_tablet() property;`
- `bool get_system_is_unix() property;`
- `bool get_system_is_windows() property;`
- `bool get_thread_is_main() property;`
- `bool hide_window();`
- `bool http_credentials_extract(const spec::uri&in uri, string&out username, string&out password);`
- `bool http_credentials_extract(const string&in user_info, string&out username, string&out password);`
- `bool http_credentials_is_basic(const http_request&in request);`
- `bool http_credentials_is_basic(const string&in header);`
- `bool http_credentials_is_digest(const http_request&in request);`
- `bool http_credentials_is_digest(const string&in header);`
- `bool http_credentials_is_ntlm(const http_request&in request);`
- `bool http_credentials_is_ntlm(const string&in header);`
- `bool http_credentials_is_proxy_basic(const http_request&in request);`
- `bool http_credentials_is_proxy_digest(const http_request&in request);`
- `bool http_credentials_is_proxy_ntlm(const http_request&in request);`
- `bool info_box(const string&in title, const string&in caption, const string&in text, uint64 flags = 0);`
- `bool install_keyhook();`
- `bool insure_key_up(int key);`
- `bool is_console_available();`
- `bool is_debugger_present();`
- `bool is_finite(double x);`
- `bool is_finite(float x);`
- `bool is_greater(double x, double y);`
- `bool is_greater(float x, float y);`
- `bool is_greater_equal(double x, double y);`
- `bool is_greater_equal(float x, float y);`
- `bool is_inf(double x);`
- `bool is_inf(float x);`
- `bool is_less(double x, double y);`
- `bool is_less(float x, float y);`
- `bool is_less_equal(double x, double y);`
- `bool is_less_equal(float x, float y);`
- `bool is_less_greater(double x, double y);`
- `bool is_less_greater(float x, float y);`
- `bool is_nan(double x);`
- `bool is_nan(float x);`
- `bool is_negative(double x);`
- `bool is_negative(float x);`
- `bool is_normal(double x);`
- `bool is_normal(float x);`
- `bool is_screen_keyboard_shown();`
- `bool is_unordered(double x, double y);`
- `bool is_unordered(float x, float y);`
- `bool is_window_active();`
- `bool is_window_hidden();`
- `bool key_down(int key);`
- `bool key_pressed(int key);`
- `bool key_released(int key);`
- `bool key_repeating(int key);`
- `bool key_up(int key);`
- `bool mouse_down(uint8 button);`
- `bool mouse_pressed(uint8 button);`
- `bool mouse_released(uint8 button);`
- `bool mouse_up(uint8 button);`
- `bool natural_number_sort(const string&in string1, const string&in string2);`
- `bool random_bool(int = 50);`
- `bool random_set_state(const string&in);`
- `bool regexp_match(const string&in, const string&in, int = RE_UTF8);`
- `bool regexp_search(const string&in, const string&in, int = RE_UTF8);`
- `bool run(const string&in filename, const string&in arguments, bool wait_for_completion, bool background);`
- `bool screen_reader_braille(const string&in text);`
- `bool screen_reader_has_braille();`
- `bool screen_reader_has_speech();`
- `bool screen_reader_is_speaking();`
- `bool screen_reader_output(const string&in text, bool interrupt = true);`
- `bool screen_reader_silence();`
- `bool screen_reader_speak(const string&in text, bool interrupt = true);`
- `bool sdl_set_hint(const string&in hint, const string&in value, int priority = SDL_HINT_NORMAL);`
- `bool set_application_name(const string&in name);`
- `bool set_key_name(int key, const string&in name);`
- `bool set_sound_global_hrtf(bool enabled);`
- `bool set_window_fullscreen(bool fullscreen);`
- `bool show_window(const string&in title);`
- `bool simulate_key_down(int key);`
- `bool simulate_key_up(int key);`
- `bool sound_set_listener_position(const vector&in position, uint listener_index = 0);`
- `bool sound_set_listener_position(float x, float y, float z, uint listener_index = 0);`
- `bool sound_set_spatialization(int panner, int attenuator, bool disable_previous = false, bool set_default = true);`
- `bool start_text_input();`
- `bool stop_text_input();`
- `bool text_input_active();`
- `bool thread_sleep(uint ms);`
- `bool urlopen(const string&in url);`
- `bool utf8valid(const string&in text, bool ban_ascii_special = true);`
- `bool validate_email_address(const string&in);`
- `const array<string>@ get_sound_input_devices() property;`
- `const array<string>@ get_sound_output_devices() property;`
- `datastream@ get_cerr() property;`
- `datastream@ get_cin() property;`
- `datastream@ get_cout() property;`
- `datetime@ parse_datetime(const string&in fmt, const string&in str, int&inout tzd);`
- `datetime@ parse_datetime(const string&in str, int&inout tzd);`
- `dictionary@ deserialize(const string&in);`
- `dns_host_entry dns_resolve(const string&in address);`
- `dns_host_entry system_dns_host_entry();`
- `double abs(double v);`
- `double acos(double x);`
- `double acosh(double x);`
- `double asin(double x);`
- `double asinh(double x);`
- `double atan(double x);`
- `double atan2(double y, double x);`
- `double atanh(double x);`
- `double bytes_to_double(const string&in data);`
- `double calculate_gamma(double x);`
- `double calculate_lgamma(double x);`
- `double cbrt(double a);`
- `double ceil(double x);`
- `double copysign(double mag, double sgn);`
- `double cos(double x);`
- `double cosh(double x);`
- `double dmax(double a, double b);`
- `double dmin(double a, double b);`
- `double erf(double x);`
- `double erfc(double x);`
- `double exp(double a);`
- `double exp2(double a);`
- `double expm1(double a);`
- `double fdim(double a, double b);`
- `double floor(double x);`
- `double fma(double a, double b, double c);`
- `double fmod(double a, double b);`
- `double fpFromIEEE(uint64);`
- `double frexp(double x, int&out exp);`
- `double hypot(double a, double b);`
- `double hypot(double a, double b, double c);`
- `double ldexp(double x, int exp);`
- `double lerp(double a, double b, double c);`
- `double log(double a);`
- `double log10(double a);`
- `double log1p(double a);`
- `double log2(double a);`
- `double logb(double x);`
- `double modf(double num, double&out iptr);`
- `double nearbyint(double x);`
- `double nextafter(double from, double to);`
- `double nexttoward(double from, double to);`
- `double parse_double(const string&in number);`
- `double pow(double a, double b);`
- `double remainder(double a, double b);`
- `double remquo(double a, double b, int&out quo);`
- `double rint(double x);`
- `double round(double number, int place);`
- `double scalbn(double x, int exp);`
- `double scalbn(double x, int64 exp);`
- `double sin(double x);`
- `double sinh(double x);`
- `double sqrt(double a);`
- `double tan(double x);`
- `double tanh(double x);`
- `double tinyexpr(const string&in expression);`
- `double trunc(double x);`
- `float absf(float v);`
- `float acosf(float x);`
- `float acosh(float x);`
- `float asinf(float x);`
- `float asinh(float x);`
- `float atan2f(float y, float x);`
- `float atanf(float x);`
- `float atanh(float x);`
- `float bytes_to_float(const string&in data);`
- `float calculate_gamma(float x);`
- `float calculate_lgamma(float x);`
- `float cbrt(float a);`
- `float ceilf(float x);`
- `float clamp(float value, float min, float max);`
- `float copysign(float mag, float sgn);`
- `float cosf(float x);`
- `float cosh(float x);`
- `float erf(float x);`
- `float erfc(float x);`
- `float exp(float a);`
- `float exp2(float a);`
- `float expm1(float a);`
- `float fdim(float a, float b);`
- `float floorf(float x);`
- `float fma(float a, float b, float c);`
- `float fmax(float a, float b);`
- `float fmin(float a, float b);`
- `float fmod(float a, float b);`
- `float fp_from_IEEE(uint);`
- `float frexp(float x, int&out exp);`
- `float get_sound_master_volume() property;`
- `float hypot(float a, float b);`
- `float hypot(float a, float b, float c);`
- `float ldexp(float x, int exp);`
- `float lerp(float a, float b, float c);`
- `float log10(float a);`
- `float log1p(float a);`
- `float log2(float a);`
- `float logb(float x);`
- `float logf(float a);`
- `float modf(float num, float&out iptr);`
- `float nearbyint(float x);`
- `float nextafter(float from, float to);`
- `float nexttoward(float from, double to);`
- `float parse_float(const string&in number);`
- `float powf(float a, float b);`
- `float random_float();`
- `float remainder(float a, float b);`
- `float remquo(float a, float b, int&out quo);`
- `float rint(float x);`
- `float scalbn(float x, int exp);`
- `float scalbn(float x, int64 exp);`
- `float sinf(float x);`
- `float sinh(float x);`
- `float sqrtf(float a);`
- `float tanf(float x);`
- `float tanh(float x);`
- `float trunc(float x);`
- `int alert(const string&in title, const string&in text, bool can_cancel = false, uint flags = 0);`
- `int bit_width(const uint x);`
- `int bit_width(const uint16 x);`
- `int bit_width(const uint64 x);`
- `int bit_width(const uint8 x);`
- `int clamp(int value, int min, int max);`
- `int count_leading_ones(uint x);`
- `int count_leading_ones(uint16 x);`
- `int count_leading_ones(uint64 x);`
- `int count_leading_ones(uint8 x);`
- `int count_leading_zeroes(uint x);`
- `int count_leading_zeroes(uint16 x);`
- `int count_leading_zeroes(uint64 x);`
- `int count_leading_zeroes(uint8 x);`
- `int count_trailing_ones(uint x);`
- `int count_trailing_ones(uint16 x);`
- `int count_trailing_ones(uint64 x);`
- `int count_trailing_ones(uint8 x);`
- `int count_trailing_zeroes(uint x);`
- `int count_trailing_zeroes(uint16 x);`
- `int count_trailing_zeroes(uint64 x);`
- `int count_trailing_zeroes(uint8 x);`
- `int datetime_days_of_month(int year, int month);`
- `int fpclassify(double x);`
- `int fpclassify(float x);`
- `int get_ANDROID_SDK_VERSION() property;`
- `int get_DATE_DAY() property;`
- `int get_DATE_MONTH() property;`
- `int get_DATE_WEEKDAY() property;`
- `int get_DATE_YEAR() property;`
- `int get_SCRIPT_CURRENT_LINE() property;`
- `int get_TIMEZONE_BASE_OFFSET() property;`
- `int get_TIMEZONE_DST_OFFSET() property;`
- `int get_TIMEZONE_OFFSET() property;`
- `int get_TIME_HOUR() property;`
- `int get_TIME_MINUTE() property;`
- `int get_TIME_SECOND() property;`
- `int get_call_stack_size() property;`
- `int get_exception_line();`
- `int get_garbage_collect_auto_frequency() property;`
- `int get_garbage_collect_mode() property;`
- `int get_key_code(const string&in name);`
- `int get_last_error();`
- `int get_sound_default_3d_attenuator() property;`
- `int get_sound_default_3d_panner() property;`
- `int get_sound_output_device() property;`
- `int ilogb(double x);`
- `int ilogb(float x);`
- `int joystick_count(bool = true);`
- `int message_box(const string&in title, const string&in message, array<string>@ buttons, uint flags = 0);`
- `int popcount(uint x);`
- `int popcount(uint16 x);`
- `int popcount(uint64 x);`
- `int popcount(uint8 x);`
- `int question(const string&in title, const string&in text, bool can_cancel = false, uint flags = 0);`
- `int random(int, int);`
- `int total_keys_down();`
- `int utf8next(const string&in text, int cursor);`
- `int utf8prev(const string&in text, int cursor);`
- `int utf8size(const string&in character);`
- `int64 file_get_size(const string&in path);`
- `int64 parse_int(const string&in, uint base = 10, uint&out byteCount = 0);`
- `key_modifier get_keyboard_modifiers() property;`
- `mail_message@ parse_email_message(const string&in);`
- `matrix3x3 get_IDENTITY_MATRIX() property;`
- `pack_interface@ get_sound_default_pack() property;`
- `physics_convex_mesh@ physics_convex_mesh_create(physics_vertex_data@ vertex_data);`
- `physics_convex_mesh@ physics_convex_mesh_create_from_polygon(physics_polygon_data@ polygon_data);`
- `physics_default_logger@ physics_default_logger_create();`
- `physics_height_field@ physics_height_field_create(int nb_columns, int nb_rows, array<double>@ height_data, float integer_height_scale = 1.0f);`
- `physics_height_field@ physics_height_field_create(int nb_columns, int nb_rows, array<float>@ height_data, float integer_height_scale = 1.0f);`
- `physics_height_field@ physics_height_field_create(int nb_columns, int nb_rows, array<int>@ height_data, float integer_height_scale = 1.0f);`
- `physics_logger@ physics_logger_get_current();`
- `physics_transform get_IDENTITY_TRANSFORM() property;`
- `physics_transform transforms_interpolate();`
- `physics_triangle_mesh@ physics_triangle_mesh_create(physics_triangle_data@ triangle_data);`
- `quaternion get_IDENTITY_QUATERNION() property;`
- `quaternion quaternion_from_euler_angles(const vector&inout angles);`
- `quaternion quaternion_from_euler_angles(float angle_x, float angle_y, float angle_z);`
- `quaternion quaternion_slerp(const quaternion&inout q1, const quaternion&inout q2, float t);`
- `random_interface@ get_default_random();`
- `script_module@ script_get_module(const string&in, int = 1);`
- `sound@ sound_play(const string&in path, const vector&in position = vector(FLOAT_MAX, FLOAT_MAX, FLOAT_MAX), float volume = 0.0, float pan = 0.0, float pitch = 100.0, mixer@ mix = null, const pack_interface@ pack_file = sound_default_pack, bool autoplay = true);`
- `spec::ip_address dns_resolve_single(const string&in address);`
- `string DIRECTORY_PREFERENCES(const string&in company_name, const string&in application_name);`
- `string ascii_to_character(uint8 character_code);`
- `string clipboard_get_text();`
- `string cwdir();`
- `string double_to_bytes(double number);`
- `string expand_environment_variables(const string&in text);`
- `string file_get_contents(const string&in filename);`
- `string float_to_bytes(float number);`
- `string format_float(double val, const string&in options = "", uint width = 0, uint precision = 0);`
- `string format_int(int64 val, const string&in options = "", uint width = 0);`
- `string format_uint(uint64 val, const string&in options = "", uint width = 0);`
- `string generate_profile(bool = true);`
- `string generate_system_fingerprint(const string&in application_id = );`
- `string generate_system_fingerprint_legacy1(const string&in application_id = );`
- `string get_COMMAND_LINE() property;`
- `string get_DATE_MONTH_NAME() property;`
- `string get_DATE_WEEKDAY_NAME() property;`
- `string get_DIRECTORY_APPDATA() property;`
- `string get_DIRECTORY_COMMON_APPDATA() property;`
- `string get_DIRECTORY_HOME() property;`
- `string get_DIRECTORY_LOCAL_APPDATA() property;`
- `string get_DIRECTORY_TEMP() property;`
- `string get_SCRIPT_CURRENT_FILE() property;`
- `string get_SCRIPT_CURRENT_FUNCTION() property;`
- `string get_SCRIPT_EXECUTABLE() property;`
- `string get_SCRIPT_MAIN_PATH() property;`
- `string get_SOUNDSYSTEM_LAST_ERROR_TEXT() property;`
- `string get_TIMEZONE_DST_NAME() property;`
- `string get_TIMEZONE_NAME() property;`
- `string get_TIMEZONE_STANDARD_NAME() property;`
- `string get_call_stack() property;`
- `string get_characters();`
- `string get_exception_file();`
- `string get_exception_function();`
- `string get_exception_info();`
- `string get_function_signature(?&in);`
- `string get_key_name(int key);`
- `string get_keyboard_name(uint id);`
- `string get_mouse_name(uint id);`
- `string get_preferences_path(const string&in company_name, const string&in application_name);`
- `string get_system_node_id() property;`
- `string get_system_node_name() property;`
- `string get_touch_device_name(uint64 device_id);`
- `string get_window_text();`
- `string hex_to_string(const string&in hex);`
- `string html_entities_decode(const string&in input);`
- `string http_status_reason(http_status);`
- `string input_box(const string&in title, const string&in caption, const string&in default_value = '', uint64 flags = 0);`
- `string join(const array<string>&in, const string&in, int = 0, int = -1);`
- `string number_to_words(int64 number, bool include_and = true);`
- `string open_file_dialog(const string&in filters = "", const string&in default_location = "");`
- `string packet(const ?&in);`
- `string packet(const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string packet(const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `string physics_logger_get_category_name(physics_logger_category category);`
- `string physics_logger_get_level_name(physics_logger_level level);`
- `string random_bytes(uint count);`
- `string random_character(const string&in, const string&in);`
- `string random_get_state();`
- `string read_environment_variable(const string&in variable, const string&in default_value = "");`
- `string regexp_replace(const string&in, const string&in, const string&in, int = RE_UTF8);`
- `string save_file_dialog(const string&in filters = "", const string&in default_location = "");`
- `string screen_reader_detect();`
- `string sdl_get_hint(const string&in hint);`
- `string select_folder_dialog(const string&in default_location = "");`
- `string set_linux_thread_priority(int64 thread_id, int priority);`
- `string set_linux_thread_priority_and_policy(int64 thread_id, int priority, int policy);`
- `string string_aes_decrypt(const string&in ciphertext, string);`
- `string string_aes_encrypt(const string&in plaintext, string key);`
- `string string_base32_decode(const string&in encoded);`
- `string string_base32_encode(const string&in binary);`
- `string string_base32_normalize(const string&in base32encoded);`
- `string string_base64_decode(const string&in encoded, string_base64_options options = STRING_BASE64_PADLESS);`
- `string string_base64_encode(const string&in binary, string_base64_options options = STRING_BASE64_DEFAULT);`
- `string string_create_from_pointer(uint64 ptr, uint64 length);`
- `string string_deflate(const string&in data, int compression_level = 9);`
- `string string_hash_md5(const string&in data, bool binary = false);`
- `string string_hash_sha1(const string&in data, bool binary = false);`
- `string string_hash_sha224(const string&in data, bool binary = false);`
- `string string_hash_sha256(const string&in data, bool binary = false);`
- `string string_hash_sha384(const string&in data, bool binary = false);`
- `string string_hash_sha512(const string&in data, bool binary = false);`
- `string string_inflate(const string&in deflated);`
- `string string_recode(const string&in text, const string&in in_encoding, const string&in out_encoding, int&out error_count = void);`
- `string string_to_hex(const string&in binary);`
- `string url_decode(const string&in url, bool plus_as_space = true);`
- `string url_encode(const string&in url, const string&in reserved = "");`
- `string url_get(const string&in url, http_response&out response = void);`
- `string url_post(const string&in url, const string&in data, http_response&out response = void);`
- `string url_request(const string&in method, const string&in url, const string&in data = "", http_response&out response = void);`
- `system_power_state system_power_info(int&out seconds = void, int&out percent = void);`
- `thread@ get_thread_current() property;`
- `thread_pool& get_thread_pool_default() property;`
- `timestamp file_get_date_created(const string&in path);`
- `timestamp file_get_date_modified(const string&in path);`
- `timestamp timestamp_from_UTC_time(int64 UTC);`
- `touch_device_type get_touch_device_type(uint64 device_id);`
- `uint HOTP(const string&in key, uint64 counter, uint digits = 6);`
- `uint adler32(const string&in data);`
- `uint crc32(const string&in data);`
- `uint fp_to_IEEE(float);`
- `uint get_PROCESSOR_COUNT() property;`
- `uint random_seed();`
- `uint string_distance(const string&in string1, const string&in string2, uint insert_cost = 1, uint delete_cost = 1, uint replace_cost = 1);`
- `uint thread_current_id();`
- `uint64 fpToIEEE(double);`
- `uint64 get_SYSTEM_PERFORMANCE_COUNTER() property;`
- `uint64 get_SYSTEM_PERFORMANCE_FREQUENCY() property;`
- `uint64 get_TIME_STAMP() property;`
- `uint64 get_TIME_SYSTEM_RUNNING_MILLISECONDS() property;`
- `uint64 get_window_os_handle();`
- `uint64 idle_ticks();`
- `uint64 memory_allocate(uint64 size);`
- `uint64 memory_allocate_units(uint64 unit_size, uint64 unit_count);`
- `uint64 memory_reallocate(uint64 ptr, uint64 size);`
- `uint64 microticks(bool secure = false);`
- `uint64 nanoticks();`
- `uint64 parseUInt(const string&in, uint base = 10, uint&out byteCount = 0);`
- `uint64 random_seed64();`
- `uint64 secure_ticks();`
- `uint64 ticks(bool secure = false);`
- `uint8 character_to_ascii(const string&in character);`
- `uuid uuid_create_from_name(const uuid&in, const string&in);`
- `uuid uuid_generate();`
- `uuid uuid_generate_random();`
- `uuid uuid_generate_time();`
- `uuid uuid_namespace_dns();`
- `uuid uuid_namespace_oid();`
- `uuid uuid_namespace_url();`
- `uuid uuid_namespace_x500();`
- `var@ parse_json(const string&in payload);`
- `var@ parse_json(datastream@ stream);`
- `vector rotate(const vector&in point, const vector&in origin, double theta, bool maintain_z = true);`
- `vector sound_get_listener_position(uint listener_index = 0);`
- `void acquire_exclusive_lock();`
- `void acquire_shared_lock();`
- `void android_send_back_button();`
- `void assert(bool, const string&in = "");`
- `void c_debug_break();`
- `void c_debug_break(const string&in message);`
- `void c_debug_message(const string&in message);`
- `void create_coroutine(coroutine@, dictionary@);`
- `void debug_add_file_breakpoint(const string&in, int);`
- `void debug_add_func_breakpoint(const string&in);`
- `void debug_break();`
- `void exit(int = 0);`
- `void garbage_collect(bool = true);`
- `void memory_free(uint64 ptr);`
- `void mouse_update();`
- `void nanosleep(uint64 ns);`
- `void next_keyboard_layout();`
- `void physics_box_shape_destroy(physics_box_shape@ shape);`
- `void physics_capsule_shape_destroy(physics_capsule_shape@ shape);`
- `void physics_concave_mesh_shape_destroy(physics_concave_mesh_shape@ shape);`
- `void physics_convex_mesh_destroy(physics_convex_mesh@ mesh);`
- `void physics_convex_mesh_shape_destroy(physics_convex_mesh_shape@ shape);`
- `void physics_default_logger_destroy(physics_default_logger@ logger);`
- `void physics_height_field_destroy(physics_height_field@ height_field);`
- `void physics_height_field_shape_destroy(physics_height_field_shape@ shape);`
- `void physics_logger_set_current(physics_logger@ logger);`
- `void physics_shape_destroy(physics_collision_shape@ shape);`
- `void physics_sphere_shape_destroy(physics_sphere_shape@ shape);`
- `void physics_triangle_mesh_destroy(physics_triangle_mesh@ mesh);`
- `void physics_triangle_shape_compute_smooth_triangle_mesh_contact(const physics_collision_shape&in shape1, const physics_collision_shape&in shape2, vector&inout local_contact_point_shape1, vector&inout local_contact_point_shape2, const physics_transform&in shape1_to_world, const physics_transform&in shape2_to_world, float penitration_depth, vector&inout out_smooth_vertex_normal);`
- `void physics_world_destroy(physics_world&inout world);`
- `void print(const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null);`
- `void printf(const string&in format, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null);`
- `void println(const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null);`
- `void refresh_window();`
- `void release_exclusive_lock();`
- `void release_shared_lock();`
- `void reset_keyboard();`
- `void reset_profiler();`
- `void script_dump_engine_configuration(datastream@);`
- `void set_cursor_visible(bool state) property;`
- `void set_default_random(random_generator@);`
- `void set_default_random(random_interface@);`
- `void set_garbage_collect_auto_frequency(int) property;`
- `void set_garbage_collect_mode(int) property;`
- `void set_keyboard_modifiers(key_modifier modifier) property;`
- `void set_mouse_grab(bool grabbed) property;`
- `void set_sound_3d_attenuator_enabled(int attenuator_id, bool enabled);`
- `void set_sound_3d_panner_enabled(int panner_id, bool enabled);`
- `void set_sound_default_3d_attenuator(int attenuator_id);`
- `void set_sound_default_3d_panner(int panner_id);`
- `void set_sound_default_decryption_key(const string&in key) property;`
- `void set_sound_default_engine(audio_engine@ engine) property;`
- `void set_sound_default_pack(pack_interface@ storage) property;`
- `void set_sound_master_volume(float db) property;`
- `void set_sound_output_device(int device) property;`
- `void sleep(uint);`
- `void start_profiling();`
- `void stop_profiling();`
- `void thread_yield();`
- `void throw(const string&in);`
- `void uninstall_keyhook();`
- `void wait(int ms);`
- `void write_environment_variable(const string&in variable, const string&in value);`
- `void yield();`
- `spec::bool parse_ip_address(const string&in addr_in, ip_address&out addr_out);`
- `spec::ip_address broadcast_ip_address();`
- `spec::ip_address wildcard_ip_address(ip_address_family);`

## Global properties

- `bool speedhack_protection;`
- `const array<string>@ ARGS;`
- `const atomic_flag memory_scan_detected;`
- `const atomic_flag speed_hack_detected;`
- `const bool profiler_is_running;`
- `const double DOUBLE_EPSILON;`
- `const double DOUBLE_MAX;`
- `const double DOUBLE_MIN_NORMALIZED;`
- `const double DOUBLE_NEG_EPSILON;`
- `const float EPSILON;`
- `const float FLOAT_EPSILON;`
- `const float FLOAT_MAX;`
- `const float FLOAT_MIN_NORMALIZED;`
- `const float FLOAT_NEG_EPSILON;`
- `const float MOUSE_ABSOLUTE_X;`
- `const float MOUSE_ABSOLUTE_Y;`
- `const float MOUSE_ABSOLUTE_Z;`
- `const float MOUSE_X;`
- `const float MOUSE_Y;`
- `const float MOUSE_Z;`
- `const int DOUBLE_EPSILON_EXPONENT;`
- `const int DOUBLE_EXPONENT_BITS;`
- `const int DOUBLE_GUARD_DIGITS;`
- `const int DOUBLE_MANTISSA_DIGITS;`
- `const int DOUBLE_MAX_EXPONENT;`
- `const int DOUBLE_MIN_EXPONENT;`
- `const int DOUBLE_NEG_EPSILON_EXPONENT;`
- `const int DOUBLE_RADIX;`
- `const int DOUBLE_ROUNDING_MODE;`
- `const int FLOAT_EPSILON_EXPONENT;`
- `const int FLOAT_EXPONENT_BITS;`
- `const int FLOAT_GUARD_DIGITS;`
- `const int FLOAT_MANTISSA_DIGITS;`
- `const int FLOAT_MAX_EXPONENT;`
- `const int FLOAT_MIN_EXPONENT;`
- `const int FLOAT_NEG_EPSILON_EXPONENT;`
- `const int FLOAT_RADIX;`
- `const int FLOAT_ROUNDING_MODE;`
- `const int HTTP_UNKNOWN_CONTENT_LENGTH;`
- `const int NVGT_VERSION_MAJOR;`
- `const int NVGT_VERSION_MINOR;`
- `const int NVGT_VERSION_PATCH;`
- `const int64 DAYS;`
- `const int64 HOURS;`
- `const int64 MICROSECONDS;`
- `const int64 MILLISECONDS;`
- `const int64 MINUTES;`
- `const int64 SECONDS;`
- `const string DATE_TIME_FORMAT_ASCTIME;`
- `const string DATE_TIME_FORMAT_HTTP;`
- `const string DATE_TIME_FORMAT_ISO8601;`
- `const string DATE_TIME_FORMAT_ISO8601_FRAC;`
- `const string DATE_TIME_FORMAT_RFC1036;`
- `const string DATE_TIME_FORMAT_RFC1123;`
- `const string DATE_TIME_FORMAT_RFC822;`
- `const string DATE_TIME_FORMAT_RFC850;`
- `const string DATE_TIME_FORMAT_SORTABLE;`
- `const string DATE_TIME_REGEX_ASCTIME;`
- `const string DATE_TIME_REGEX_HTTP;`
- `const string DATE_TIME_REGEX_ISO8601;`
- `const string DATE_TIME_REGEX_RFC1036;`
- `const string DATE_TIME_REGEX_RFC1123;`
- `const string DATE_TIME_REGEX_RFC822;`
- `const string DATE_TIME_REGEX_RFC850;`
- `const string DATE_TIME_REGEX_SORTABLE;`
- `const string HTTP_1_0;`
- `const string HTTP_1_1;`
- `const string HTTP_CHUNKED_TRANSFER_ENCODING;`
- `const string HTTP_DELETE;`
- `const string HTTP_GET;`
- `const string HTTP_HEAD;`
- `const string HTTP_IDENTITY_TRANSFER_ENCODING;`
- `const string HTTP_OPTIONS;`
- `const string HTTP_PATCH;`
- `const string HTTP_POST;`
- `const string HTTP_PUT;`
- `const string HTTP_UNKNOWN_CONTENT_TYPE;`
- `const string NVGT_VERSION;`
- `const string NVGT_VERSION_BUILD_TIME;`
- `const string NVGT_VERSION_COMMIT_HASH;`
- `const string NVGT_VERSION_TYPE;`
- `const string PLATFORM;`
- `const string PLATFORM_ARCHITECTURE;`
- `const string PLATFORM_DISPLAY_NAME;`
- `const string PLATFORM_VERSION;`
- `const string last_exception_call_stack;`
- `const timestamp SCRIPT_BUILD_TIME;`
- `const uint NVGT_VERSION_BUILD_TIMESTAMP;`
- `engine_character_event on_characters;`
- `engine_key_event on_key_press;`
- `engine_key_event on_key_release;`
- `engine_key_event on_key_repeat;`
- `engine_touch_event on_touch_finger_cancel;`
- `engine_touch_event on_touch_finger_down;`
- `engine_touch_event on_touch_finger_up;`
- `engine_touch_motion_event on_touch_finger_move;`
- `mixer@ sound_default_mixer;`
- `uint64 timer_default_accuracy;`
- `spec::const string NEWLINE_CR;`
- `spec::const string NEWLINE_CRLF;`
- `spec::const string NEWLINE_DEFAULT;`
- `spec::const string NEWLINE_LF;`

## Funcdefs (callback signatures)

- `funcdef bool array<T>::less(const T&in a, const T&in b);`
- `funcdef bool coordinate_map_filter_callback(coordinate_map_area@);`
- `funcdef bool engine_character_event_callback(string character);`
- `funcdef bool engine_key_event_callback(int key);`
- `funcdef bool engine_touch_event_callback(uint64 device, const touch_finger&inout finger);`
- `funcdef bool engine_touch_motion_event_callback(uint64 device, const touch_finger&inout finger, float relative_x, float relative_y);`
- `funcdef float physics_raycast_callback(const raycast_info&in info);`
- `funcdef int pathfinder_callback(int, int, int, any@ = null);`
- `funcdef int pathfinder_callback_ex(int, int, int, int, int, int, any@ = null);`
- `funcdef int pathfinder_callback_legacy(int, int, int, int, string);`
- `funcdef uint timer_callback(string timer_id, string user_data);`
- `funcdef void android_permission_request_callback(string permission, bool granted, string user_data);`
- `funcdef void audio_engine_processing_callback(audio_engine@ engine, memory_buffer<float>&inout data, uint64 frames);`
- `funcdef void coroutine(dictionary@);`
- `funcdef void engine_character_event_passthrough_callback(string character);`
- `funcdef void engine_key_event_passthrough_callback(int key);`
- `funcdef void engine_touch_event_passthrough_callback(uint64 device, const touch_finger&inout finger);`
- `funcdef void engine_touch_motion_event_passthrough_callback(uint64 device, const touch_finger&inout finger, float relative_x, float relative_y);`
- `funcdef void physics_collision_callback(const physics_collision_callback_data&inout data);`
- `funcdef void physics_overlap_callback(const physics_overlap_callback_data&inout data);`
- `funcdef void thread_callback(dictionary@ args);`

## Enums

### ARCHITECTURE

- `ARCH_ALPHA = 1`
- `ARCH_IA32 = 2`
- `ARCH_IA64 = 3`
- `ARCH_MIPS = 4`
- `ARCH_HPPA = 5`
- `ARCH_PPC = 6`
- `ARCH_POWER = 7`
- `ARCH_SPARC = 8`
- `ARCH_AMD64 = 9`
- `ARCH_ARM = 10`
- `ARCH_M68K = 11`
- `ARCH_S390 = 12`
- `ARCH_SH = 13`
- `ARCH_NIOS2 = 14`
- `ARCH_AARCH64 = 15`
- `ARCH_ARM64 = 15`
- `ARCH_RISCV64 = 16`
- `ARCH_RISCV32 = 17`
- `ARCH_LOONGARCH64 = 18`

### OPERATING_SYSTEM

- `OS_FREE_BSD = 1`
- `OS_AIX = 2`
- `OS_HPUX = 3`
- `OS_TRU64 = 4`
- `OS_LINUX = 5`
- `OS_DARWIN = 6`
- `OS_NET_BSD = 7`
- `OS_OPEN_BSD = 8`
- `OS_IRIX = 9`
- `OS_SOLARIS = 10`
- `OS_QNX = 11`
- `OS_VXWORKS = 12`
- `OS_CYGWIN = 13`
- `OS_NACL = 14`
- `OS_ANDROID = 15`
- `OS_UNKNOWN_UNIX = 255`
- `OS_WINDOWS_NT = 4097`
- `OS_VMS = 8193`

### audio_attenuator

- `audio_attenuator_basic = 0`
- `audio_attenuator_phonon = 1`

### audio_encoder_flags

- `AUDIO_ENCODER_OVERWRITE = 1`
- `AUDIO_ENCODER_DEFAULTS = -2147483648`

### audio_engine_flags

- `AUDIO_ENGINE_DURATIONS_IN_FRAMES = 1`
- `AUDIO_ENGINE_NO_AUTO_START = 2`
- `AUDIO_ENGINE_NO_DEVICE = 4`
- `AUDIO_ENGINE_NO_CLIP = 8`
- `AUDIO_ENGINE_PERCENTAGE_ATTRIBUTES = 16`

### audio_error_state

- `AUDIO_ERROR_STATE_SUCCESS = 0`
- `AUDIO_ERROR_STATE_ERROR = -1`
- `AUDIO_ERROR_STATE_INVALID_ARGS = -2`
- `AUDIO_ERROR_STATE_INVALID_OPERATION = -3`
- `AUDIO_ERROR_STATE_OUT_OF_MEMORY = -4`
- `AUDIO_ERROR_STATE_OUT_OF_RANGE = -5`
- `AUDIO_ERROR_STATE_ACCESS_DENIED = -6`
- `AUDIO_ERROR_STATE_DOES_NOT_EXIST = -7`
- `AUDIO_ERROR_STATE_ALREADY_EXISTS = -8`
- `AUDIO_ERROR_STATE_TOO_MANY_OPEN_FILES = -9`
- `AUDIO_ERROR_STATE_INVALID_FILE = -10`
- `AUDIO_ERROR_STATE_TOO_BIG = -11`
- `AUDIO_ERROR_STATE_PATH_TOO_LONG = -12`
- `AUDIO_ERROR_STATE_NAME_TOO_LONG = -13`
- `AUDIO_ERROR_STATE_NOT_DIRECTORY = -14`
- `AUDIO_ERROR_STATE_IS_DIRECTORY = -15`
- `AUDIO_ERROR_STATE_DIRECTORY_NOT_EMPTY = -16`
- `AUDIO_ERROR_STATE_AT_END = -17`
- `AUDIO_ERROR_STATE_NO_SPACE = -18`
- `AUDIO_ERROR_STATE_BUSY = -19`
- `AUDIO_ERROR_STATE_IO_ERROR = -20`
- `AUDIO_ERROR_STATE_INTERRUPT = -21`
- `AUDIO_ERROR_STATE_UNAVAILABLE = -22`
- `AUDIO_ERROR_STATE_ALREADY_IN_USE = -23`
- `AUDIO_ERROR_STATE_BAD_ADDRESS = -24`
- `AUDIO_ERROR_STATE_BAD_SEEK = -25`
- `AUDIO_ERROR_STATE_BAD_PIPE = -26`
- `AUDIO_ERROR_STATE_DEADLOCK = -27`
- `AUDIO_ERROR_STATE_TOO_MANY_LINKS = -28`
- `AUDIO_ERROR_STATE_NOT_IMPLEMENTED = -29`
- `AUDIO_ERROR_STATE_NO_MESSAGE = -30`
- `AUDIO_ERROR_STATE_BAD_MESSAGE = -31`
- `AUDIO_ERROR_STATE_NO_DATA_AVAILABLE = -32`
- `AUDIO_ERROR_STATE_INVALID_DATA = -33`
- `AUDIO_ERROR_STATE_TIMEOUT = -34`
- `AUDIO_ERROR_STATE_NO_NETWORK = -35`
- `AUDIO_ERROR_STATE_NOT_UNIQUE = -36`
- `AUDIO_ERROR_STATE_NOT_SOCKET = -37`
- `AUDIO_ERROR_STATE_NO_ADDRESS = -38`
- `AUDIO_ERROR_STATE_BAD_PROTOCOL = -39`
- `AUDIO_ERROR_STATE_PROTOCOL_UNAVAILABLE = -40`
- `AUDIO_ERROR_STATE_PROTOCOL_NOT_SUPPORTED = -41`
- `AUDIO_ERROR_STATE_PROTOCOL_FAMILY_NOT_SUPPORTED = -42`
- `AUDIO_ERROR_STATE_ADDRESS_FAMILY_NOT_SUPPORTED = -43`
- `AUDIO_ERROR_STATE_SOCKET_NOT_SUPPORTED = -44`
- `AUDIO_ERROR_STATE_CONNECTION_RESET = -45`
- `AUDIO_ERROR_STATE_ALREADY_CONNECTED = -46`
- `AUDIO_ERROR_STATE_NOT_CONNECTED = -47`
- `AUDIO_ERROR_STATE_CONNECTION_REFUSED = -48`
- `AUDIO_ERROR_STATE_NO_HOST = -49`
- `AUDIO_ERROR_STATE_IN_PROGRESS = -50`
- `AUDIO_ERROR_STATE_CANCELLED = -51`
- `AUDIO_ERROR_STATE_MEMORY_ALREADY_MAPPED = -52`
- `AUDIO_ERROR_STATE_CRC_MISMATCH = -100`
- `AUDIO_ERROR_STATE_FORMAT_NOT_SUPPORTED = -200`
- `AUDIO_ERROR_STATE_DEVICE_TYPE_NOT_SUPPORTED = -201`
- `AUDIO_ERROR_STATE_SHARE_MODE_NOT_SUPPORTED = -202`
- `AUDIO_ERROR_STATE_NO_BACKEND = -203`
- `AUDIO_ERROR_STATE_NO_DEVICE = -204`
- `AUDIO_ERROR_STATE_API_NOT_FOUND = -205`
- `AUDIO_ERROR_STATE_INVALID_DEVICE_CONFIG = -206`
- `AUDIO_ERROR_STATE_LOOP = -207`
- `AUDIO_ERROR_STATE_BACKEND_NOT_ENABLED = -208`
- `AUDIO_ERROR_STATE_DEVICE_NOT_INITIALIZED = -300`
- `AUDIO_ERROR_STATE_DEVICE_ALREADY_INITIALIZED = -301`
- `AUDIO_ERROR_STATE_DEVICE_NOT_STARTED = -302`
- `AUDIO_ERROR_STATE_DEVICE_NOT_STOPPED = -303`
- `AUDIO_ERROR_STATE_FAILED_TO_INIT_BACKEND = -400`
- `AUDIO_ERROR_STATE_FAILED_TO_OPEN_BACKEND_DEVICE = -401`
- `AUDIO_ERROR_STATE_FAILED_TO_START_BACKEND_DEVICE = -402`
- `AUDIO_ERROR_STATE_FAILED_TO_STOP_BACKEND_DEVICE = -403`

### audio_format

- `AUDIO_FORMAT_UNKNOWN = 0`
- `AUDIO_FORMAT_U8 = 1`
- `AUDIO_FORMAT_S16 = 2`
- `AUDIO_FORMAT_S24 = 3`
- `AUDIO_FORMAT_S32 = 4`
- `AUDIO_FORMAT_F32 = 5`

### audio_node_state

- `AUDIO_NODE_STATE_STARTED = 0`
- `AUDIO_NODE_STATE_STOPPED = 1`

### audio_pan_mode

- `AUDIO_PAN_MODE_BALANCE = 0`
- `AUDIO_PAN_MODE_PAN = 1`

### audio_panner

- `audio_panner_basic = 0`
- `audio_panner_phonon_hrtf = 1`

### audio_positioning_mode

- `AUDIO_POSITIONING_ABSOLUTE = 0`
- `AUDIO_POSITIONING_RELATIVE = 1`

### audio_wav_encoder_flags

- `AUDIO_ENCODER_WAV_U8 = 2`
- `AUDIO_ENCODER_WAV_S16 = 4`
- `AUDIO_ENCODER_WAV_S24 = 8`
- `AUDIO_ENCODER_WAV_S32 = 16`
- `AUDIO_ENCODER_WAV_F32 = 32`

### compression_method

- `COMPRESSION_METHOD_ZLIB = 0`
- `COMPRESSION_METHOD_GZIP = 1`

### datastream_byte_order

- `STREAM_BYTE_ORDER_NATIVE = 1`
- `STREAM_BYTE_ORDER_BIG_ENDIAN = 2`
- `STREAM_BYTE_ORDER_NETWORK = 2`
- `STREAM_BYTE_ORDER_LITTLE_ENDIAN = 3`

### floating_point_classification

- `FP_NORMAL = -1`
- `FP_SUBNORMAL = -2`
- `FP_ZERO = 0`
- `FP_INFINITE = 1`
- `FP_NAN = 2`

### ftp_file_type

- `FTP_FILE_TYPE_TEXT = 0`
- `FTP_FILE_TYPE_BINARY = 1`

### glob_options

- `GLOB_DEFAULT = 0`
- `GLOB_IGNORE_HIDDEN = 1`
- `GLOB_FOLLOW_SYMLINKS = 2`
- `GLOB_CASELESS = 4`

### http_status

- `HTTP_ACCEPTED = 202`
- `HTTP_ALREADY_REPORTED = 208`
- `HTTP_BAD_GATEWAY = 502`
- `HTTP_BAD_REQUEST = 400`
- `HTTP_CONFLICT = 409`
- `HTTP_CONTINUE = 100`
- `HTTP_CREATED = 201`
- `HTTP_ENCHANCE_YOUR_CALM = 420`
- `HTTP_EXPECTATION_FAILED = 417`
- `HTTP_FAILED_DEPENDENCY = 424`
- `HTTP_FORBIDDEN = 403`
- `HTTP_FOUND = 302`
- `HTTP_GATEWAY_TIMEOUT = 504`
- `HTTP_GONE = 410`
- `HTTP_IM_A_TEAPOT = 418`
- `HTTP_IM_USED = 226`
- `HTTP_INSUFFICIENT_STORAGE = 507`
- `HTTP_INTERNAL_SERVER_ERROR = 500`
- `HTTP_LENGTH_REQUIRED = 411`
- `HTTP_LOCKED = 423`
- `HTTP_LOOP_DETECTED = 508`
- `HTTP_METHOD_NOT_ALLOWED = 405`
- `HTTP_MISDIRECTED_REQUEST = 421`
- `HTTP_MOVED_PERMANENTLY = 301`
- `HTTP_MULTIPLE_CHOICES = 300`
- `HTTP_MULTI_STATUS = 207`
- `HTTP_NETWORK_AUTHENTICATION_REQUIRED = 511`
- `HTTP_NONAUTHORITATIVE = 203`
- `HTTP_NOT_ACCEPTABLE = 406`
- `HTTP_NOT_EXTENDED = 510`
- `HTTP_NOT_FOUND = 404`
- `HTTP_NOT_IMPLEMENTED = 501`
- `HTTP_NOT_MODIFIED = 304`
- `HTTP_NO_CONTENT = 204`
- `HTTP_OK = 200`
- `HTTP_PARTIAL_CONTENT = 206`
- `HTTP_PAYMENT_REQUIRED = 402`
- `HTTP_PERMANENT_REDIRECT = 308`
- `HTTP_PRECONDITION_FAILED = 412`
- `HTTP_PRECONDITION_REQUIRED = 428`
- `HTTP_PROCESSING = 102`
- `HTTP_PROXY_AUTHENTICATION_REQUIRED = 407`
- `HTTP_REQUESTED_RANGE_NOT_SATISFIABLE = 416`
- `HTTP_REQUEST_ENTITY_TOO_LARGE = 413`
- `HTTP_REQUEST_HEADER_FIELDS_TOO_LARGE = 431`
- `HTTP_REQUEST_TIMEOUT = 408`
- `HTTP_REQUEST_URI_TOO_LONG = 414`
- `HTTP_RESET_CONTENT = 205`
- `HTTP_SEE_OTHER = 303`
- `HTTP_SERVICE_UNAVAILABLE = 503`
- `HTTP_SWITCHING_PROTOCOLS = 101`
- `HTTP_TEMPORARY_REDIRECT = 307`
- `HTTP_TOO_EARLY = 425`
- `HTTP_TOO_MANY_REQUESTS = 429`
- `HTTP_UNAUTHORIZED = 401`
- `HTTP_UNAVAILABLE_FOR_LEGAL_REASONS = 451`
- `HTTP_UNPROCESSABLE_ENTITY = 422`
- `HTTP_UNSUPPORTED_MEDIA_TYPE = 415`
- `HTTP_UPGRADE_REQUIRED = 426`
- `HTTP_USE_PROXY = 305`
- `HTTP_VARIANT_ALSO_NEGOTIATES = 506`
- `HTTP_VERSION_NOT_SUPPORTED = 505`

### ip_address_family

- `IP_FAMILY_UNKNOWN = 0`
- `IP_FAMILY_unix_local = 0`
- `IP_FAMILY_IPV4 = 2`
- `IP_FAMILY_IPV6 = 23`

### joystick_bind_type

- `JOYSTICK_BIND_TYPE_NONE = 0`
- `JOYSTICK_BIND_TYPE_BUTTON = 1`
- `JOYSTICK_BIND_TYPE_AXIS = 2`
- `JOYSTICK_BIND_TYPE_HAT = 3`

### joystick_control_type

- `JOYSTICK_BUTTON_INVALID = -1`
- `JOYSTICK_BUTTON_A = 0`
- `JOYSTICK_BUTTON_B = 1`
- `JOYSTICK_BUTTON_X = 2`
- `JOYSTICK_BUTTON_Y = 3`
- `JOYSTICK_BUTTON_BACK = 4`
- `JOYSTICK_BUTTON_GUIDE = 5`
- `JOYSTICK_BUTTON_START = 6`
- `JOYSTICK_CONTROL_LEFT_STICK = 7`
- `JOYSTICK_CONTROL_RIGHT_STICK = 8`
- `JOYSTICK_CONTROL_LEFT_SHOULDER = 9`
- `JOYSTICK_CONTROL_RIGHT_SHOULDER = 10`
- `JOYSTICK_CONTROL_DPAD_UP = 11`
- `JOYSTICK_CONTROL_DPAD_DOWN = 12`
- `JOYSTICK_CONTROL_DPAD_LEFT = 13`
- `JOYSTICK_CONTROL_DPAD_RIGHT = 14`
- `JOYSTICK_BUTTON_MISC = 15`
- `JOYSTICK_CONTROL_PADDLE1 = 16`
- `JOYSTICK_CONTROL_PADDLE2 = 17`
- `JOYSTICK_CONTROL_PADDLE3 = 18`
- `JOYSTICK_CONTROL_PADDLE4 = 19`
- `JOYSTICK_CONTROL_TOUCHPAD = 20`

### joystick_power_state

- `JOYSTICK_POWER_ERROR = -1`
- `JOYSTICK_POWER_UNKNOWN = 0`
- `JOYSTICK_POWER_ON_BATTERY = 1`
- `JOYSTICK_POWER_NO_BATTERY = 2`
- `JOYSTICK_POWER_CHARGING = 3`
- `JOYSTICK_POWER_CHARGED = 4`

### joystick_type

- `JOYSTICK_TYPE_UNKNOWN = 0`
- `JOYSTICK_TYPE_STANDARD = 1`
- `JOYSTICK_TYPE_XBOX360 = 2`
- `JOYSTICK_TYPE_XBOX1 = 3`
- `JOYSTICK_TYPE_PS3 = 4`
- `JOYSTICK_TYPE_PS4 = 5`
- `JOYSTICK_TYPE_NINTENDO_SWITCH_PRO = 7`
- `JOYSTICK_TYPE_PS5 = 6`
- `JOYSTICK_TYPE_NINTENDO_SWITCH_JOYCON_LEFT = 8`
- `JOYSTICK_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT = 9`
- `JOYSTICK_TYPE_NINTENDO_SWITCH_JOYCON_PAIR = 10`

### key_code

- `KEY_UNKNOWN = 0`
- `KEY_A = 4`
- `KEY_B = 5`
- `KEY_C = 6`
- `KEY_D = 7`
- `KEY_E = 8`
- `KEY_F = 9`
- `KEY_G = 10`
- `KEY_H = 11`
- `KEY_I = 12`
- `KEY_J = 13`
- `KEY_K = 14`
- `KEY_L = 15`
- `KEY_M = 16`
- `KEY_N = 17`
- `KEY_O = 18`
- `KEY_P = 19`
- `KEY_Q = 20`
- `KEY_R = 21`
- `KEY_S = 22`
- `KEY_T = 23`
- `KEY_U = 24`
- `KEY_V = 25`
- `KEY_W = 26`
- `KEY_X = 27`
- `KEY_Y = 28`
- `KEY_Z = 29`
- `KEY_1 = 30`
- `KEY_2 = 31`
- `KEY_3 = 32`
- `KEY_4 = 33`
- `KEY_5 = 34`
- `KEY_6 = 35`
- `KEY_7 = 36`
- `KEY_8 = 37`
- `KEY_9 = 38`
- `KEY_0 = 39`
- `KEY_RETURN = 40`
- `KEY_ESCAPE = 41`
- `KEY_BACK = 42`
- `KEY_TAB = 43`
- `KEY_SPACE = 44`
- `KEY_MINUS = 45`
- `KEY_EQUALS = 46`
- `KEY_LEFTBRACKET = 47`
- `KEY_RIGHTBRACKET = 48`
- `KEY_BACKSLASH = 49`
- `KEY_NONUSHASH = 50`
- `KEY_SEMICOLON = 51`
- `KEY_APOSTROPHE = 52`
- `KEY_GRAVE = 53`
- `KEY_COMMA = 54`
- `KEY_PERIOD = 55`
- `KEY_SLASH = 56`
- `KEY_CAPSLOCK = 57`
- `KEY_F1 = 58`
- `KEY_F2 = 59`
- `KEY_F3 = 60`
- `KEY_F4 = 61`
- `KEY_F5 = 62`
- `KEY_F6 = 63`
- `KEY_F7 = 64`
- `KEY_F8 = 65`
- `KEY_F9 = 66`
- `KEY_F10 = 67`
- `KEY_F11 = 68`
- `KEY_F12 = 69`
- `KEY_PRINTSCREEN = 70`
- `KEY_SCROLLLOCK = 71`
- `KEY_PAUSE = 72`
- `KEY_INSERT = 73`
- `KEY_HOME = 74`
- `KEY_PAGEUP = 75`
- `KEY_DELETE = 76`
- `KEY_END = 77`
- `KEY_PAGEDOWN = 78`
- `KEY_RIGHT = 79`
- `KEY_LEFT = 80`
- `KEY_DOWN = 81`
- `KEY_UP = 82`
- `KEY_NUMLOCKCLEAR = 83`
- `KEY_NUMPAD_DIVIDE = 84`
- `KEY_NUMPAD_MULTIPLY = 85`
- `KEY_NUMPAD_MINUS = 86`
- `KEY_NUMPAD_PLUS = 87`
- `KEY_NUMPAD_ENTER = 88`
- `KEY_NUMPAD_1 = 89`
- `KEY_NUMPAD_2 = 90`
- `KEY_NUMPAD_3 = 91`
- `KEY_NUMPAD_4 = 92`
- `KEY_NUMPAD_5 = 93`
- `KEY_NUMPAD_6 = 94`
- `KEY_NUMPAD_7 = 95`
- `KEY_NUMPAD_8 = 96`
- `KEY_NUMPAD_9 = 97`
- `KEY_NUMPAD_0 = 98`
- `KEY_NUMPAD_PERIOD = 99`
- `KEY_NONUSBACKSLASH = 100`
- `KEY_APPLICATION = 101`
- `KEY_POWER = 102`
- `KEY_NUMPAD_EQUALS = 103`
- `KEY_F13 = 104`
- `KEY_F14 = 105`
- `KEY_F15 = 106`
- `KEY_F16 = 107`
- `KEY_F17 = 108`
- `KEY_F18 = 109`
- `KEY_F19 = 110`
- `KEY_F20 = 111`
- `KEY_F21 = 112`
- `KEY_F22 = 113`
- `KEY_F23 = 114`
- `KEY_F24 = 115`
- `KEY_EXECUTE = 116`
- `KEY_HELP = 117`
- `KEY_MENU = 118`
- `KEY_SELECT = 119`
- `KEY_STOP = 120`
- `KEY_AGAIN = 121`
- `KEY_UNDO = 122`
- `KEY_CUT = 123`
- `KEY_COPY = 124`
- `KEY_PASTE = 125`
- `KEY_FIND = 126`
- `KEY_MUTE = 127`
- `KEY_VOLUMEUP = 128`
- `KEY_VOLUMEDOWN = 129`
- `KEY_NUMPAD_COMMA = 133`
- `KEY_NUMPAD_EQUALSAS400 = 134`
- `KEY_INTERNATIONAL1 = 135`
- `KEY_INTERNATIONAL2 = 136`
- `KEY_INTERNATIONAL3 = 137`
- `KEY_INTERNATIONAL4 = 138`
- `KEY_INTERNATIONAL5 = 139`
- `KEY_INTERNATIONAL6 = 140`
- `KEY_INTERNATIONAL7 = 141`
- `KEY_INTERNATIONAL8 = 142`
- `KEY_INTERNATIONAL9 = 143`
- `KEY_LANG1 = 144`
- `KEY_LANG2 = 145`
- `KEY_LANG3 = 146`
- `KEY_LANG4 = 147`
- `KEY_LANG5 = 148`
- `KEY_LANG6 = 149`
- `KEY_LANG7 = 150`
- `KEY_LANG8 = 151`
- `KEY_LANG9 = 152`
- `KEY_ALTERASE = 153`
- `KEY_SYSREQ = 154`
- `KEY_CANCEL = 155`
- `KEY_CLEAR = 156`
- `KEY_SDL_PRIOR = 157`
- `KEY_RETURN2 = 158`
- `KEY_SEPARATOR = 159`
- `KEY_OUT = 160`
- `KEY_OPER = 161`
- `KEY_CLEARAGAIN = 162`
- `KEY_CRSEL = 163`
- `KEY_EXSEL = 164`
- `KEY_NUMPAD_00 = 176`
- `KEY_NUMPAD_000 = 177`
- `KEY_THOUSANDSSEPARATOR = 178`
- `KEY_DECIMALSEPARATOR = 179`
- `KEY_CURRENCYUNIT = 180`
- `KEY_CURRENCYSUBUNIT = 181`
- `KEY_NUMPAD_LEFTPAREN = 182`
- `KEY_NUMPAD_RIGHTPAREN = 183`
- `KEY_NUMPAD_LEFTBRACE = 184`
- `KEY_NUMPAD_RIGHTBRACE = 185`
- `KEY_NUMPAD_TAB = 186`
- `KEY_NUMPAD_BACKSPACE = 187`
- `KEY_NUMPAD_A = 188`
- `KEY_NUMPAD_B = 189`
- `KEY_NUMPAD_C = 190`
- `KEY_NUMPAD_D = 191`
- `KEY_NUMPAD_E = 192`
- `KEY_NUMPAD_F = 193`
- `KEY_NUMPAD_XOR = 194`
- `KEY_NUMPAD_POWER = 195`
- `KEY_NUMPAD_PERCENT = 196`
- `KEY_NUMPAD_LESS = 197`
- `KEY_NUMPAD_GREATER = 198`
- `KEY_NUMPAD_AMPERSAND = 199`
- `KEY_NUMPAD_DBLAMPERSAND = 200`
- `KEY_NUMPAD_VERTICALBAR = 201`
- `KEY_NUMPAD_DBLVERTICALBAR = 202`
- `KEY_NUMPAD_COLON = 203`
- `KEY_NUMPAD_HASH = 204`
- `KEY_NUMPAD_SPACE = 205`
- `KEY_NUMPAD_AT = 206`
- `KEY_NUMPAD_EXCLAM = 207`
- `KEY_NUMPAD_MEMSTORE = 208`
- `KEY_NUMPAD_MEMRECALL = 209`
- `KEY_NUMPAD_MEMCLEAR = 210`
- `KEY_NUMPAD_MEMADD = 211`
- `KEY_NUMPAD_MEMSUBTRACT = 212`
- `KEY_NUMPAD_MEMMULTIPLY = 213`
- `KEY_NUMPAD_MEMDIVIDE = 214`
- `KEY_NUMPAD_PLUSMINUS = 215`
- `KEY_NUMPAD_CLEAR = 216`
- `KEY_NUMPAD_CLEARENTRY = 217`
- `KEY_NUMPAD_BINARY = 218`
- `KEY_NUMPAD_OCTAL = 219`
- `KEY_NUMPAD_DECIMAL = 220`
- `KEY_NUMPAD_HEXADECIMAL = 221`
- `KEY_LCTRL = 224`
- `KEY_LSHIFT = 225`
- `KEY_LALT = 226`
- `KEY_LGUI = 227`
- `KEY_RCTRL = 228`
- `KEY_RSHIFT = 229`
- `KEY_RALT = 230`
- `KEY_RGUI = 231`
- `KEY_MODE = 257`
- `KEY_MEDIA_NEXT_TRACK = 267`
- `KEY_MEDIA_PREVIOUS_TRACK = 268`
- `KEY_MEDIA_STOP = 269`
- `KEY_MEDIA_PLAY = 262`
- `KEY_MEDIA_SELECT = 272`
- `KEY_AC_SEARCH = 280`
- `KEY_AC_HOME = 281`
- `KEY_AC_BACK = 282`
- `KEY_AC_FORWARD = 283`
- `KEY_AC_STOP = 284`
- `KEY_AC_REFRESH = 285`
- `KEY_AC_BOOKMARKS = 286`
- `KEY_MEDIA_EJECT = 270`
- `KEY_SLEEP = 258`
- `KEY_MEDIA_REWIND = 266`
- `KEY_MEDIA_FAST_FORWARD = 265`
- `KEY_SOFTLEFT = 287`
- `KEY_SOFTRIGHT = 288`
- `KEY_CALL = 289`
- `KEY_ENDCALL = 290`

### key_modifier

- `KEYMOD_NONE = 0`
- `KEYMOD_LSHIFT = 1`
- `KEYMOD_RSHIFT = 2`
- `KEYMOD_LCTRL = 64`
- `KEYMOD_RCTRL = 128`
- `KEYMOD_LALT = 256`
- `KEYMOD_RALT = 512`
- `KEYMOD_LGUI = 1024`
- `KEYMOD_RGUI = 2048`
- `KEYMOD_NUM = 4096`
- `KEYMOD_CAPS = 8192`
- `KEYMOD_MODE = 16384`
- `KEYMOD_SCROLL = 32768`
- `KEYMOD_CTRL = 192`
- `KEYMOD_SHIFT = 3`
- `KEYMOD_ALT = 768`
- `KEYMOD_GUI = 3072`

### mail_priority

- `MAIL_PRIORITY_HIGHEST = 1`
- `MAIL_PRIORITY_HIGH = 2`
- `MAIL_PRIORITY_NORMAL = 3`
- `MAIL_PRIORITY_LOW = 4`
- `MAIL_PRIORITY_LOWEST = 5`

### mail_recipient_type

- `RECIPIENT_TO = 0`
- `RECIPIENT_CC = 1`
- `RECIPIENT_BCC = 2`

### memory_order

- `MEMORY_ORDER_RELAXED = 0`
- `MEMORY_ORDER_ACQUIRE = 2`
- `MEMORY_ORDER_RELEASE = 3`
- `MEMORY_ORDER_ACQ_REL = 4`
- `MEMORY_ORDER_SEQ_CST = 5`

### message_box_flags

- `MESSAGE_BOX_ERROR = 16`
- `MESSAGE_BOX_WARNING = 32`
- `MESSAGE_BOX_INFORMATION = 64`
- `MESSAGE_BOX_BUTTONS_LEFT_TO_RIGHT = 128`
- `MESSAGE_BOX_BUTTONS_RIGHT_TO_LEFT = 256`

### network_event_type

- `event_none = 0`
- `event_connect = 1`
- `event_disconnect = 2`
- `event_disconnect_timeout = 4`
- `event_receive = 3`

### opus_application_type

- `OPUS_APPLICATION_VOIP = 2048`
- `OPUS_APPLICATION_AUDIO = 2049`
- `OPUS_APPLICATION_RESTRICTED_LOWDELAY = 2051`

### opus_signal_type

- `OPUS_AUTO = -1000`
- `OPUS_SIGNAL_VOICE = 3001`
- `OPUS_SIGNAL_MUSIC = 3002`

### path_style

- `PATH_STYLE_UNIX = 0`
- `PATH_STYLE_URI = 0`
- `PATH_STYLE_WINDOWS = 1`
- `PATH_STYLE_VMS = 2`
- `PATH_STYLE_NATIVE = 3`
- `PATH_STYLE_AUTO = 4`

### physics_body_type

- `PHYSICS_BODY_STATIC = 0`
- `PHYSICS_BODY_KINEMATIC = 1`
- `PHYSICS_BODY_DYNAMIC = 2`

### physics_contact_event_type

- `PHYSICS_CONTACT_START = 0`
- `PHYSICS_CONTACT_STAY = 1`
- `PHYSICS_CONTACT_EXIT = 2`

### physics_contact_position_correction_technique

- `POSITION_CORRECTION_TECHNIQUE_BAUMGARTE_CONTACTS = 0`
- `POSITION_CORRECTION_TECHNIQUE_SPLIT_IMPULSES = 1`

### physics_height_data_type

- `PHYSICS_HEIGHT_FLOAT_TYPE = 0`
- `PHYSICS_HEIGHT_DOUBLE_TYPE = 1`
- `PHYSICS_HEIGHT_INT_TYPE = 2`

### physics_joint_type

- `BALL_SOCKET_JOINT = 0`
- `SLIDER_JOINT = 1`
- `HINGE_JOINT = 2`
- `FIXED_JOINT = 3`

### physics_joints_position_correction_technique

- `JOINTS_CORRECTION_TECHNIQUE_BAUMGARTE_JOINTS = 0`
- `JOINTS_CORRECTION_TECHNIQUE_NON_LINEAR_GAUSS_SEIDEL = 1`

### physics_logger_category

- `LOGGER_CATEGORY_PHYSICS_COMMON = 0`
- `LOGGER_CATEGORY_WORLD = 1`
- `LOGGER_CATEGORY_BODY = 2`
- `LOGGER_CATEGORY_JOINT = 3`
- `LOGGER_CATEGORY_COLLIDER = 4`

### physics_logger_format

- `LOGGER_FORMAT_TEXT = 0`
- `LOGGER_FORMAT_HTML = 1`

### physics_logger_level

- `LOGGER_LEVEL_ERROR = 1`
- `LOGGER_LEVEL_WARNING = 2`
- `LOGGER_LEVEL_INFORMATION = 4`

### physics_message_type

- `PHYSICS_MESSAGE_ERROR = 1`
- `PHYSICS_MESSAGE_WARNING = 2`
- `PHYSICS_MESSAGE_INFORMATION = 4`

### physics_overlap_event_type

- `PHYSICS_OVERLAP_START = 0`
- `PHYSICS_OVERLAP_STAY = 1`
- `PHYSICS_OVERLAP_EXIT = 2`

### physics_polygon_index_data_type

- `POLYGON_INDEX_INTEGER_TYPE = 0`
- `POLYGON_INDEX_SHORT_TYPE = 1`

### physics_polygon_vertex_data_type

- `POLYGON_VERTEX_FLOAT_TYPE = 0`
- `POLYGON_VERTEX_DOUBLE_TYPE = 1`

### physics_shape_name

- `SHAPE_TRIANGLE = 0`
- `SHAPE_SPHERE = 1`
- `SHAPE_CAPSULE = 2`
- `SHAPE_BOX = 3`
- `SHAPE_CONVEX_MESH = 4`
- `SHAPE_TRIANGLE_MESH = 5`
- `SHAPE_HEIGHTFIELD = 6`

### physics_shape_type

- `SHAPE_TYPE_SPHERE = 0`
- `SHAPE_TYPE_CAPSULE = 1`
- `SHAPE_TYPE_CONVEX_POLYHEDRON = 2`
- `SHAPE_TYPE_CONCAVE = 3`

### physics_triangle_index_data_type

- `TRIANGLE_INDEX_INTEGER_TYPE = 0`
- `TRIANGLE_INDEX_SHORT_TYPE = 1`

### physics_triangle_normal_data_type

- `TRIANGLE_NORMAL_FLOAT_TYPE = 0`
- `TRIANGLE_NORMAL_DOUBLE_TYPE = 1`

### physics_triangle_raycast_side

- `TRIANGLE_RAYCAST_SIDE_FRONT = 0`
- `TRIANGLE_RAYCAST_SIDE_BACK = 1`
- `TRIANGLE_RAYCAST_SIDE_FRONT_AND_BACK = 2`

### physics_triangle_vertex_data_type

- `TRIANGLE_VERTEX_FLOAT_TYPE = 0`
- `TRIANGLE_VERTEX_DOUBLE_TYPE = 1`

### physics_vertex_data_type

- `VERTEX_FLOAT_TYPE = 0`
- `VERTEX_DOUBLE_TYPE = 1`

### regexp_options

- `RE_CASELESS = 1`
- `RE_MULTILINE = 2`
- `RE_DOTALL = 4`
- `RE_EXTENDED = 8`
- `RE_ANCHORED = 16`
- `RE_DOLLAR_END_ONLY = 32`
- `RE_EXTRA = 64`
- `RE_NOT_BOL = 128`
- `RE_NOT_EOL = 256`
- `RE_UNGREEDY = 512`
- `RE_NOT_EMPTY = 1024`
- `RE_UTF8 = 2048`
- `RE_NO_AUTO_CAPTURE = 4096`
- `RE_NO_UTF8_CHECK = 8192`
- `RE_FIRSTLINE = 262144`
- `RE_DUPNAMES = 524288`
- `RE_NEWLINE_CR = 1048576`
- `RE_NEWLINE_LF = 2097152`
- `RE_NEWLINE_CRLF = 3145728`
- `RE_NEWLINE_ANY = 4194304`
- `RE_NEWLINE_ANY_CRLF = 5242880`
- `RE_GLOBAL = 268435456`
- `RE_NO_VARS = 536870912`

### reverb3d_placement

- `reverb3d_prepan = 0`
- `reverb3d_postpan = 1`
- `reverb3d_postattenuate = 2`

### sdl_hint_priority

- `SDL_HINT_DEFAULT = 0`
- `SDL_HINT_NORMAL = 1`
- `SDL_HINT_OVERRIDE = 2`

### smtp_auth_method

- `SMTP_AUTH_NONE = 0`
- `SMTP_AUTH_LOGIN = 3`
- `SMTP_AUTH_PLAIN = 4`
- `SMTP_AUTH_CRAM_MD5 = 1`
- `SMTP_AUTH_CRAM_SHA1 = 2`
- `SMTP_AUTH_XOAUTH2 = 5`
- `SMTP_AUTH_NTLM = 6`

### socket_select_mode

- `SOCKET_SELECT_READ = 1`
- `SOCKET_SELECT_WRITE = 2`
- `SOCKET_SELECT_ERROR = 4`

### socket_type

- `SOCKET_TYPE_STREAM = 1`
- `SOCKET_TYPE_DATAGRAM = 2`
- `SOCKET_TYPE_RAW = 3`

### string_base64_options

- `STRING_BASE64_DEFAULT = 0`
- `STRING_BASE64_URL = 1`
- `STRING_BASE64_PADLESS = 2`
- `STRING_BASE64_URL_PADLESS = 3`

### system_power_state

- `POWER_STATE_ERROR = -1`
- `POWER_STATE_UNKNOWN = 0`
- `POWER_STATE_ON_BATTERY = 1`
- `POWER_STATE_NO_BATTERY = 2`
- `POWER_STATE_CHARGING = 3`
- `POWER_STATE_CHARGED = 4`

### thread_event_type

- `THREAD_EVENT_MANUAL_RESET = 0`
- `THREAD_EVENT_AUTO_RESET = 1`

### thread_priority

- `THREAD_PRIORITY_LOWEST = -2`
- `THREAD_PRIORITY_LOW = -1`
- `THREAD_PRIORITY_NORMAL = 0`
- `THREAD_PRIORITY_HIGH = 1`
- `THREAD_PRIORITY_HIGHEST = 2`

### touch_device_type

- `TOUCH_DEVICE_TYPE_INVALID = -1`
- `TOUCH_DEVICE_DIRECT = 0`
- `TOUCH_DEVICE_INDIRECT_ABSOLUTE = 1`
- `TOUCH_DEVICE_INDIRECT_RELATIVE = 2`

### web_socket_error_codes

- `WS_ERR_NO_HANDSHAKE = 1`
- `WS_ERR_HANDSHAKE_NO_VERSION = 2`
- `WS_ERR_HANDSHAKE_UNSUPPORTED_VERSION = 3`
- `WS_ERR_HANDSHAKE_NO_KEY = 4`
- `WS_ERR_HANDSHAKE_ACCEPT = 5`
- `WS_ERR_UNAUTHORIZED = 6`
- `WS_ERR_PAYLOAD_TOO_BIG = 10`
- `WS_ERR_INCOMPLETE_FRAME = 11`

### web_socket_frame_flags

- `WS_FRAME_FLAG_FIN = 128`

### web_socket_frame_opcodes

- `WS_FRAME_OP_CONT = 0`
- `WS_FRAME_OP_TEXT = 1`
- `WS_FRAME_OP_BINARY = 2`
- `WS_FRAME_OP_CLOSE = 8`
- `WS_FRAME_OP_PING = 9`
- `WS_FRAME_OP_PONG = 10`
- `WS_FRAME_OP_BITMASK = 15`
- `WS_FRAME_OP_SETRAW = 256`

### web_socket_mode

- `WS_SERVER = 0`
- `WS_CLIENT = 1`

### web_socket_send_flags

- `WS_FRAME_TEXT = 129`
- `WS_FRAME_BINARY = 130`

### web_socket_status_codes

- `WS_NORMAL_CLOSE = 1000`
- `WS_ENDPOINT_GOING_AWAY = 1001`
- `WS_PROTOCOL_ERROR = 1002`
- `WS_PAYLOAD_NOT_ACCEPTABLE = 1003`
- `WS_RESERVED = 1004`
- `WS_RESERVED_NO_STATUS_CODE = 1005`
- `WS_RESERVED_ABNORMAL_CLOSE = 1006`
- `WS_MALFORMED_PAYLOAD = 1007`
- `WS_POLICY_VIOLATION = 1008`
- `WS_PAYLOAD_TOO_BIG = 1009`
- `WS_EXTENSION_REQUIRED = 1010`
- `WS_UNEXPECTED_CONDITION = 1011`
- `WS_RESERVED_TLS_FAILURE = 1015`

## Types

### aabb

Methods:

- `vector get_center() const property;`
- `const vector& get_min() const property;`
- `const vector& get_max() const property;`
- `void set_min(const vector&in min) property;`
- `void set_max(const vector&in max) property;`
- `vector get_extent() const property;`
- `void inflate(float x, float y, float z);`
- `void inflate_with_point(const vector&in point);`
- `bool test_collision(const aabb&in aabb) const;`
- `void merge_with(const aabb&in aabb);`
- `void merge(const aabb&in aabb1, const aabb&in aabb2);`
- `bool contains(const aabb&in aabb) const;`
- `bool contains(const vector&in point, float epsilon = EPSILON) const;`
- `bool test_collision_triangle_aabb(const array<vector>@ points) const;`
- `float get_volume() const property;`
- `void apply_scale(const vector&in scale);`
- `bool test_ray_intersect(const vector&in ray_origin, const vector&in ray_direction, float ray_max_fraction);`
- `bool raycast(const ray&in ray, vector&out hit_point);`

### any

Construction:

- `any@ any();`
- `any@ any(?&in);`
- `any@ any(const int64&in);`
- `any@ any(const double&in);`

Methods:

- `any& opAssign(any&in);`
- `void store(?&in);`
- `void store(const int64&in);`
- `void store(const double&in);`
- `bool retrieve(?&out);`
- `bool retrieve(int64&out);`
- `bool retrieve(double&out);`

### array<T>

Construction:

- `array<T>@ array(int&in);`
- `array<T>@ array(int&in, uint length);`
- `array<T>@ array(int&in, uint length, const T&in value);`

Methods:

- `T& opIndex(int64 index);`
- `const T& opIndex(int64 index) const;`
- `int64 opForBegin() const;`
- `bool opForEnd(int64) const;`
- `int64 opForNext(int64) const;`
- `const T& opForValue0(int64 index) const;`
- `int64 opForValue1(int64 index) const;`
- `array<T>& opAssign(const array<T>&in);`
- `void insert_at(uint index, const T&in value);`
- `void insert_at(uint index, const array<T>&inout arr);`
- `void insert_last(const T&in value);`
- `void extend(const array<T>&inout arr);`
- `void remove_at(uint index);`
- `void remove_last();`
- `void remove_range(uint start, uint count);`
- `uint length() const;`
- `void reserve(uint length);`
- `void resize(uint length);`
- `void sort_ascending();`
- `void sort_ascending(uint startAt, uint count);`
- `void sort_descending();`
- `void sort_descending(uint startAt, uint count);`
- `T& front();`
- `T& back();`
- `void reverse();`
- `int find(const T&in value) const;`
- `int find(uint startAt, const T&in value) const;`
- `int find_by_ref(const T&in value) const;`
- `int find_by_ref(uint startAt, const T&in value) const;`
- `bool opEquals(const array<T>&in) const;`
- `bool is_empty() const;`
- `void sort(array<T>::less&in, uint startAt = 0, uint count = uint(-1));`
- `uint size() const;`
- `bool empty() const;`
- `void push_back(const T&in);`
- `void pop_back();`
- `void insert(uint index, const T&in value);`
- `void insert(uint index, const array<T>&inout arr);`
- `void erase(uint);`
- `const T& random() const;`
- `const T& random(random_interface@ rng) const;`
- `const T& random(random_generator@ rng) const;`
- `void shuffle();`
- `void shuffle(random_interface@ rng);`
- `void shuffle(random_generator@ rng);`
- `const T& random(random_pcg@ generator) const;`
- `const T& random(random_well@ generator) const;`
- `const T& random(random_gamerand@ generator) const;`
- `const T& random(random_xorshift@ generator) const;`
- `void shuffle(random_pcg@ generator);`
- `void shuffle(random_well@ generator);`
- `void shuffle(random_gamerand@ generator);`
- `void shuffle(random_xorshift@ generator);`
- `const T& random(const random_pcg&in generator) const;`
- `const T& random(const random_well&in generator) const;`
- `const T& random(const random_gamerand&in generator) const;`
- `const T& random(const random_xorshift&in generator) const;`

### asset_decryptor

Construction:

- `asset_decryptor@ asset_decryptor();`
- `asset_decryptor@ asset_decryptor(datastream@, const string&in key, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `asset_decryptor& opShr(int8&out);`
- `int8 read_int8();`
- `asset_decryptor& opShl(int8);`
- `asset_decryptor& write_int8(int8);`
- `asset_decryptor& opShr(uint8&out);`
- `uint8 read_uint8();`
- `asset_decryptor& opShl(uint8);`
- `asset_decryptor& write_uint8(uint8);`
- `asset_decryptor& opShr(int16&out);`
- `int16 read_int16();`
- `asset_decryptor& opShl(int16);`
- `asset_decryptor& write_int16(int16);`
- `asset_decryptor& opShr(uint16&out);`
- `uint16 read_uint16();`
- `asset_decryptor& opShl(uint16);`
- `asset_decryptor& write_uint16(uint16);`
- `asset_decryptor& opShr(int&out);`
- `int read_int();`
- `asset_decryptor& opShl(int);`
- `asset_decryptor& write_int(int);`
- `asset_decryptor& opShr(uint&out);`
- `uint read_uint();`
- `asset_decryptor& opShl(uint);`
- `asset_decryptor& write_uint(uint);`
- `asset_decryptor& opShr(int64&out);`
- `int64 read_int64();`
- `asset_decryptor& opShl(int64);`
- `asset_decryptor& write_int64(int64);`
- `asset_decryptor& opShr(uint64&out);`
- `uint64 read_uint64();`
- `asset_decryptor& opShl(uint64);`
- `asset_decryptor& write_uint64(uint64);`
- `asset_decryptor& opShr(float&out);`
- `float read_float();`
- `asset_decryptor& opShl(float);`
- `asset_decryptor& write_float(float);`
- `asset_decryptor& opShr(double&out);`
- `double read_double();`
- `asset_decryptor& opShl(double);`
- `asset_decryptor& write_double(double);`
- `asset_decryptor& opShr(string&out);`
- `string read_string();`
- `asset_decryptor& opShl(string);`
- `asset_decryptor& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in key, const string&in = "", int byteorder = 1);`

### asset_encryptor

Construction:

- `asset_encryptor@ asset_encryptor();`
- `asset_encryptor@ asset_encryptor(datastream@, string&in key, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `asset_encryptor& opShr(int8&out);`
- `int8 read_int8();`
- `asset_encryptor& opShl(int8);`
- `asset_encryptor& write_int8(int8);`
- `asset_encryptor& opShr(uint8&out);`
- `uint8 read_uint8();`
- `asset_encryptor& opShl(uint8);`
- `asset_encryptor& write_uint8(uint8);`
- `asset_encryptor& opShr(int16&out);`
- `int16 read_int16();`
- `asset_encryptor& opShl(int16);`
- `asset_encryptor& write_int16(int16);`
- `asset_encryptor& opShr(uint16&out);`
- `uint16 read_uint16();`
- `asset_encryptor& opShl(uint16);`
- `asset_encryptor& write_uint16(uint16);`
- `asset_encryptor& opShr(int&out);`
- `int read_int();`
- `asset_encryptor& opShl(int);`
- `asset_encryptor& write_int(int);`
- `asset_encryptor& opShr(uint&out);`
- `uint read_uint();`
- `asset_encryptor& opShl(uint);`
- `asset_encryptor& write_uint(uint);`
- `asset_encryptor& opShr(int64&out);`
- `int64 read_int64();`
- `asset_encryptor& opShl(int64);`
- `asset_encryptor& write_int64(int64);`
- `asset_encryptor& opShr(uint64&out);`
- `uint64 read_uint64();`
- `asset_encryptor& opShl(uint64);`
- `asset_encryptor& write_uint64(uint64);`
- `asset_encryptor& opShr(float&out);`
- `float read_float();`
- `asset_encryptor& opShl(float);`
- `asset_encryptor& write_float(float);`
- `asset_encryptor& opShr(double&out);`
- `double read_double();`
- `asset_encryptor& opShl(double);`
- `asset_encryptor& write_double(double);`
- `asset_encryptor& opShr(string&out);`
- `string read_string();`
- `asset_encryptor& opShl(string);`
- `asset_encryptor& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, string&in key, const string&in = "", int byteorder = 1);`

### async<T>

Construction:

- `async<T>@ async(int&in);`
- `async<T>@ async(int&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`
- `async<T>@ async(int&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in, const ?&in);`

Methods:

- `const T& get_value() property;`
- `bool get_complete() const property;`
- `bool get_failed() const property;`
- `string get_exception() const property;`
- `void wait();`
- `bool try_wait(uint ms);`

### atomic_bool

Methods:

- `bool is_lock_free();`
- `void store(bool val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool opAssign(bool val);`
- `bool load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool opImplConv();`
- `bool exchange(bool desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(bool&inout expected, bool desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(bool&inout expected, bool desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(bool&inout expected, bool desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(bool&inout expected, bool desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(bool old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `bool get_is_always_lock_free() property;`

### atomic_flag

Methods:

- `bool test(memory_order order = MEMORY_ORDER_SEQ_CST) const;`
- `void clear(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool test_and_set(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(bool old, memory_order order = MEMORY_ORDER_SEQ_CST) const;`
- `void notify_one();`
- `void notify_all();`

### atomic_int

Methods:

- `bool is_lock_free();`
- `void store(int val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAssign(int val);`
- `int load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opImplConv();`
- `int exchange(int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(int&inout expected, int desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(int&inout expected, int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(int&inout expected, int desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(int&inout expected, int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(int old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `int fetch_add(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_sub(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAddAssign(int arg);`
- `int opSubAssign(int arg);`
- `int opPreInc();`
- `int opPostInc(int arg);`
- `int opPreDec();`
- `int opPostDec(int arg);`
- `int fetch_and(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_or(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_xor(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAndAssign(int arg);`
- `int opOrAssign(int arg);`
- `int opXorAssign(int arg);`
- `bool get_is_always_lock_free() property;`

### atomic_int16

Methods:

- `bool is_lock_free();`
- `void store(int16 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 opAssign(int16 val);`
- `int16 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 opImplConv();`
- `int16 exchange(int16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(int16&inout expected, int16 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(int16&inout expected, int16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(int16&inout expected, int16 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(int16&inout expected, int16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(int16 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `int16 fetch_add(int16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 fetch_sub(int16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 opAddAssign(int16 arg);`
- `int16 opSubAssign(int16 arg);`
- `int16 opPreInc();`
- `int16 opPostInc(int16 arg);`
- `int16 opPreDec();`
- `int16 opPostDec(int16 arg);`
- `int16 fetch_and(int16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 fetch_or(int16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 fetch_xor(int16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int16 opAndAssign(int16 arg);`
- `int16 opOrAssign(int16 arg);`
- `int16 opXorAssign(int16 arg);`
- `bool get_is_always_lock_free() property;`

### atomic_int32

Methods:

- `bool is_lock_free();`
- `void store(int val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAssign(int val);`
- `int load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opImplConv();`
- `int exchange(int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(int&inout expected, int desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(int&inout expected, int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(int&inout expected, int desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(int&inout expected, int desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(int old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `int fetch_add(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_sub(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAddAssign(int arg);`
- `int opSubAssign(int arg);`
- `int opPreInc();`
- `int opPostInc(int arg);`
- `int opPreDec();`
- `int opPostDec(int arg);`
- `int fetch_and(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_or(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int fetch_xor(int arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int opAndAssign(int arg);`
- `int opOrAssign(int arg);`
- `int opXorAssign(int arg);`
- `bool get_is_always_lock_free() property;`

### atomic_int64

Methods:

- `bool is_lock_free();`
- `void store(int64 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 opAssign(int64 val);`
- `int64 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 opImplConv();`
- `int64 exchange(int64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(int64&inout expected, int64 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(int64&inout expected, int64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(int64&inout expected, int64 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(int64&inout expected, int64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(int64 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `int64 fetch_add(int64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 fetch_sub(int64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 opAddAssign(int64 arg);`
- `int64 opSubAssign(int64 arg);`
- `int64 opPreInc();`
- `int64 opPostInc(int64 arg);`
- `int64 opPreDec();`
- `int64 opPostDec(int64 arg);`
- `int64 fetch_and(int64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 fetch_or(int64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 fetch_xor(int64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int64 opAndAssign(int64 arg);`
- `int64 opOrAssign(int64 arg);`
- `int64 opXorAssign(int64 arg);`
- `bool get_is_always_lock_free() property;`

### atomic_int8

Methods:

- `bool is_lock_free();`
- `void store(int8 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 opAssign(int8 val);`
- `int8 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 opImplConv();`
- `int8 exchange(int8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(int8&inout expected, int8 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(int8&inout expected, int8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(int8&inout expected, int8 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(int8&inout expected, int8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(int8 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `int8 fetch_add(int8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 fetch_sub(int8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 opAddAssign(int8 arg);`
- `int8 opSubAssign(int8 arg);`
- `int8 opPreInc();`
- `int8 opPostInc(int8 arg);`
- `int8 opPreDec();`
- `int8 opPostDec(int8 arg);`
- `int8 fetch_and(int8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 fetch_or(int8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 fetch_xor(int8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `int8 opAndAssign(int8 arg);`
- `int8 opOrAssign(int8 arg);`
- `int8 opXorAssign(int8 arg);`
- `bool get_is_always_lock_free() property;`

### atomic_uint

Methods:

- `bool is_lock_free();`
- `void store(uint val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAssign(uint val);`
- `uint load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opImplConv();`
- `uint exchange(uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(uint&inout expected, uint desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(uint&inout expected, uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(uint&inout expected, uint desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(uint&inout expected, uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(uint old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `uint fetch_add(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_sub(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAddAssign(uint arg);`
- `uint opSubAssign(uint arg);`
- `uint opPreInc();`
- `uint opPostInc(uint arg);`
- `uint opPreDec();`
- `uint opPostDec(uint arg);`
- `uint fetch_and(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_or(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_xor(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAndAssign(uint arg);`
- `uint opOrAssign(uint arg);`
- `uint opXorAssign(uint arg);`
- `bool get_is_always_lock_free() property;`

### atomic_uint16

Methods:

- `bool is_lock_free();`
- `void store(uint16 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 opAssign(uint16 val);`
- `uint16 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 opImplConv();`
- `uint16 exchange(uint16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(uint16&inout expected, uint16 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(uint16&inout expected, uint16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(uint16&inout expected, uint16 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(uint16&inout expected, uint16 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(uint16 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `uint16 fetch_add(uint16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 fetch_sub(uint16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 opAddAssign(uint16 arg);`
- `uint16 opSubAssign(uint16 arg);`
- `uint16 opPreInc();`
- `uint16 opPostInc(uint16 arg);`
- `uint16 opPreDec();`
- `uint16 opPostDec(uint16 arg);`
- `uint16 fetch_and(uint16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 fetch_or(uint16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 fetch_xor(uint16 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint16 opAndAssign(uint16 arg);`
- `uint16 opOrAssign(uint16 arg);`
- `uint16 opXorAssign(uint16 arg);`
- `bool get_is_always_lock_free() property;`

### atomic_uint32

Methods:

- `bool is_lock_free();`
- `void store(uint val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAssign(uint val);`
- `uint load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opImplConv();`
- `uint exchange(uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(uint&inout expected, uint desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(uint&inout expected, uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(uint&inout expected, uint desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(uint&inout expected, uint desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(uint old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `uint fetch_add(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_sub(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAddAssign(uint arg);`
- `uint opSubAssign(uint arg);`
- `uint opPreInc();`
- `uint opPostInc(uint arg);`
- `uint opPreDec();`
- `uint opPostDec(uint arg);`
- `uint fetch_and(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_or(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint fetch_xor(uint arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint opAndAssign(uint arg);`
- `uint opOrAssign(uint arg);`
- `uint opXorAssign(uint arg);`
- `bool get_is_always_lock_free() property;`

### atomic_uint64

Methods:

- `bool is_lock_free();`
- `void store(uint64 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 opAssign(uint64 val);`
- `uint64 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 opImplConv();`
- `uint64 exchange(uint64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(uint64&inout expected, uint64 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(uint64&inout expected, uint64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(uint64&inout expected, uint64 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(uint64&inout expected, uint64 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(uint64 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `uint64 fetch_add(uint64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 fetch_sub(uint64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 opAddAssign(uint64 arg);`
- `uint64 opSubAssign(uint64 arg);`
- `uint64 opPreInc();`
- `uint64 opPostInc(uint64 arg);`
- `uint64 opPreDec();`
- `uint64 opPostDec(uint64 arg);`
- `uint64 fetch_and(uint64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 fetch_or(uint64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 fetch_xor(uint64 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint64 opAndAssign(uint64 arg);`
- `uint64 opOrAssign(uint64 arg);`
- `uint64 opXorAssign(uint64 arg);`
- `bool get_is_always_lock_free() property;`

### atomic_uint8

Methods:

- `bool is_lock_free();`
- `void store(uint8 val, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 opAssign(uint8 val);`
- `uint8 load(memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 opImplConv();`
- `uint8 exchange(uint8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_weak(uint8&inout expected, uint8 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_weak(uint8&inout expected, uint8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `bool compare_exchange_strong(uint8&inout expected, uint8 desired, memory_order success, memory_order failure);`
- `bool compare_exchange_strong(uint8&inout expected, uint8 desired, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void wait(uint8 old, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `void notify_one();`
- `void notify_all();`
- `uint8 fetch_add(uint8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 fetch_sub(uint8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 opAddAssign(uint8 arg);`
- `uint8 opSubAssign(uint8 arg);`
- `uint8 opPreInc();`
- `uint8 opPostInc(uint8 arg);`
- `uint8 opPreDec();`
- `uint8 opPostDec(uint8 arg);`
- `uint8 fetch_and(uint8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 fetch_or(uint8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 fetch_xor(uint8 arg, memory_order order = MEMORY_ORDER_SEQ_CST);`
- `uint8 opAndAssign(uint8 arg);`
- `uint8 opOrAssign(uint8 arg);`
- `uint8 opXorAssign(uint8 arg);`
- `bool get_is_always_lock_free() property;`

### audio_band_pass_filter

Construction:

- `audio_band_pass_filter@ audio_band_pass_filter(double cutoff_frequency, uint order, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_cutoff_frequency(double frequency) property;`
- `double get_cutoff_frequency() const property;`
- `void set_order(uint order) property;`
- `uint get_order() const property;`

### audio_data_source

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_advised_read_frame_count() const property;`
- `array<float>@ read(uint64 frame_count = 0);`
- `uint64 skip_frames(uint64 frame_count);`
- `float skip_milliseconds(float ms);`
- `bool seek_frames(uint64 frame_index);`
- `uint64 get_cursor_frames() const property;`
- `bool seek_milliseconds(float ms);`
- `float get_cursor_milliseconds() const property;`
- `uint64 get_length_frames() const property;`
- `float get_length_milliseconds() const property;`
- `bool set_looping(bool looping);`
- `bool get_looping() const property;`
- `bool set_range(uint64 start_frame, uint64 end_frame);`
- `void get_range(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_loop_point(uint64 start_frame, uint64 end_frame);`
- `void get_loop_point(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_current(audio_data_source@ new_current);`
- `audio_data_source@ get_current() const property;`
- `bool set_next(audio_data_source@ new_next);`
- `audio_data_source@ get_next() const property;`
- `uint get_channels() const property;`
- `uint get_sample_rate() const property;`
- `bool get_active() const property;`
- `audio_decoder@ opCast();`
- `audio_ring_buffer@ opCast();`
- `microphone@ opCast();`

### audio_decoder

Construction:

- `audio_decoder@ audio_decoder(audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_advised_read_frame_count() const property;`
- `array<float>@ read(uint64 frame_count = 0);`
- `uint64 skip_frames(uint64 frame_count);`
- `float skip_milliseconds(float ms);`
- `bool seek_frames(uint64 frame_index);`
- `uint64 get_cursor_frames() const property;`
- `bool seek_milliseconds(float ms);`
- `float get_cursor_milliseconds() const property;`
- `uint64 get_length_frames() const property;`
- `float get_length_milliseconds() const property;`
- `bool set_looping(bool looping);`
- `bool get_looping() const property;`
- `bool set_range(uint64 start_frame, uint64 end_frame);`
- `void get_range(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_loop_point(uint64 start_frame, uint64 end_frame);`
- `void get_loop_point(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_current(audio_data_source@ new_current);`
- `audio_data_source@ get_current() const property;`
- `bool set_next(audio_data_source@ new_next);`
- `audio_data_source@ get_next() const property;`
- `uint get_channels() const property;`
- `uint get_sample_rate() const property;`
- `bool get_active() const property;`
- `audio_data_source@ opImplCast();`
- `bool open(const string&in filename, const pack_interface@ pack_file = sound_default_pack, uint sample_rate = 0, uint channels = 0);`
- `bool open(datastream@ stream, uint sample_rate = 0, uint channels = 0);`
- `bool close();`

### audio_delay_node

Construction:

- `audio_delay_node@ audio_delay_node(uint delay_in_frames, float decay, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_wet(float wet) property;`
- `float get_wet() const property;`
- `void set_dry(float dry) property;`
- `float get_dry() const property;`
- `void set_decay(float decay) property;`
- `float get_decay() const property;`

### audio_encoder

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_default_open_flags() const property;`
- `bool open(uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(const string&in filename, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(datastream@ stream, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool close();`
- `bool get_active() const property;`
- `uint64 get_frames_written() const property;`
- `uint write(const array<float>@ frames);`
- `uint write(const memory_buffer<float>&inout frames);`
- `string read();`
- `string get_format() const property;`
- `uint get_sample_rate() const property;`
- `uint get_channels() const property;`
- `audio_wav_encoder@ opCast();`
- `audio_opus_encoder@ opCast();`

### audio_engine

Construction:

- `audio_engine@ audio_engine(int flags, int sample_rate = 0, int channels = 0);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `int get_flags() const property;`
- `int get_device() const;`
- `bool set_device(int device);`
- `audio_node@ get_endpoint() const property;`
- `array<float>@ read(uint64 frame_count);`
- `void set_processing_callback(audio_engine_processing_callback@ cb) property;`
- `audio_engine_processing_callback@ get_processing_callback() const property;`
- `uint64 get_time_in_frames() const property;`
- `bool set_time_in_frames(uint64 time_frames);`
- `uint64 get_time_in_milliseconds() const property;`
- `bool set_time_in_milliseconds(uint64 time_ms);`
- `int get_channels() const property;`
- `int get_sample_rate() const property;`
- `bool start();`
- `bool stop();`
- `bool set_volume(float volume);`
- `float get_volume() const property;`
- `bool set_gain(float gain);`
- `float get_gain() const property;`
- `uint get_listener_count() const property;`
- `int find_closest_listener(float x, float y, float z) const;`
- `int find_closest_listener(const vector&in position) const;`
- `void set_listener_position(int index, float x, float y, float z);`
- `void set_listener_position(int index, const vector&in position);`
- `vector get_listener_position(int index) const;`
- `void set_listener_direction(int index, float x, float y, float z);`
- `void set_listener_direction(int index, const vector&in direction);`
- `vector get_listener_direction(int index) const;`
- `void set_listener_velocity(int index, float x, float y, float z);`
- `void set_listener_velocity(int index, const vector&in velocity);`
- `vector get_listener_velocity(int index) const;`
- `void set_listener_cone(int index, float inner_radians, float outer_radians, float outer_gain);`
- `void get_listener_cone(int index, float&out inner_radians, float&out outer_radians, float&out outer_gain) const;`
- `void set_listener_world_up(int index, float x, float y, float z);`
- `void set_listener_world_up(int index, const vector&in world_up);`
- `vector get_listener_world_up(int index) const;`
- `void set_listener_enabled(int index, bool enabled);`
- `bool get_listener_enabled(int index) const;`
- `sound@ play(const string&in path, const vector&in position = vector(FLOAT_MAX, FLOAT_MAX, FLOAT_MAX), float volume = 0.0, float pan = 0.0, float pitch = 100.0, mixer@ mix = null, const pack_interface@ pack_file = sound_default_pack, bool autoplay = true);`
- `mixer@ mixer();`
- `sound@ sound();`

### audio_freeverb_node

Construction:

- `audio_freeverb_node@ audio_freeverb_node(audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_room_size(float size) property;`
- `float get_room_size() const property;`
- `void set_damping(float damping) property;`
- `float get_damping() const property;`
- `void set_width(float width) property;`
- `float get_width() const property;`
- `void set_wet(float wet) property;`
- `float get_wet() const property;`
- `void set_dry(float dry) property;`
- `float get_dry() const property;`
- `void set_input_width(float width) property;`
- `float get_input_width() const property;`
- `void set_frozen(bool frozen) property;`
- `bool get_frozen() const property;`

### audio_high_pass_filter

Construction:

- `audio_high_pass_filter@ audio_high_pass_filter(double cutoff_frequency, uint order, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_cutoff_frequency(double frequency) property;`
- `double get_cutoff_frequency() const property;`
- `void set_order(uint order) property;`
- `uint get_order() const property;`

### audio_high_shelf_filter

Construction:

- `audio_high_shelf_filter@ audio_high_shelf_filter(double gain_db, double q, double frequency, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_gain(double gain) property;`
- `double get_gain() const property;`
- `void set_q(double q) property;`
- `double get_q() const property;`
- `void set_frequency(double frequency) property;`
- `double get_frequency() const property;`

### audio_low_pass_filter

Construction:

- `audio_low_pass_filter@ audio_low_pass_filter(double cutoff_frequency, uint order, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_cutoff_frequency(double frequency) property;`
- `double get_cutoff_frequency() const property;`
- `void set_order(uint order) property;`
- `uint get_order() const property;`

### audio_low_shelf_filter

Construction:

- `audio_low_shelf_filter@ audio_low_shelf_filter(double gain_db, double q, double frequency, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_gain(double gain) property;`
- `double get_gain() const property;`
- `void set_q(double q) property;`
- `double get_q() const property;`
- `void set_frequency(double frequency) property;`
- `double get_frequency() const property;`

### audio_node

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_engine@ opCast();`
- `audio_node_chain@ opCast();`
- `audio_splitter_node@ opCast();`
- `reverb3d@ opCast();`
- `mixer@ opCast();`
- `sound@ opCast();`
- `audio_data_source@ opCast();`
- `audio_decoder@ opCast();`
- `audio_ring_buffer@ opCast();`
- `microphone@ opCast();`
- `phonon_binaural_node@ opCast();`
- `audio_low_pass_filter@ opCast();`
- `audio_high_pass_filter@ opCast();`
- `audio_band_pass_filter@ opCast();`
- `audio_notch_filter@ opCast();`
- `audio_peak_filter@ opCast();`
- `audio_low_shelf_filter@ opCast();`
- `audio_high_shelf_filter@ opCast();`
- `audio_delay_node@ opCast();`
- `audio_freeverb_node@ opCast();`
- `audio_encoder@ opCast();`
- `audio_wav_encoder@ opCast();`
- `audio_opus_encoder@ opCast();`

### audio_node_chain

Construction:

- `audio_node_chain@ audio_node_chain(audio_node@ source = null, audio_node@ endpoint = null, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `bool add_node(audio_node@ node, audio_node@ after = null, uint input_bus_index = 0);`
- `bool add_node(audio_node@ node, int after, uint input_bus_index = 0);`
- `bool remove_node(audio_node@ node);`
- `bool remove_node(uint index);`
- `bool clear(bool detach_nodes = true);`
- `void set_endpoint(audio_node@ endpoint, uint input_bus_index = 0);`
- `audio_node@ get_endpoint() const property;`
- `audio_node@ get_first() const property;`
- `audio_node@ get_last() const property;`
- `audio_node@ opIndex(uint index) const;`
- `int find(audio_node@ node) const;`
- `uint get_node_count() const property;`

### audio_notch_filter

Construction:

- `audio_notch_filter@ audio_notch_filter(double q, double frequency, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_q(double q) property;`
- `double get_q() const property;`
- `void set_frequency(double frequency) property;`
- `double get_frequency() const property;`

### audio_opus_encoder

Construction:

- `audio_opus_encoder@ audio_opus_encoder(audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_default_open_flags() const property;`
- `bool open(uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(const string&in filename, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(datastream@ stream, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool close();`
- `bool get_active() const property;`
- `uint64 get_frames_written() const property;`
- `uint write(const array<float>@ frames);`
- `uint write(const memory_buffer<float>&inout frames);`
- `string read();`
- `string get_format() const property;`
- `uint get_sample_rate() const property;`
- `uint get_channels() const property;`
- `audio_encoder@ opImplCast();`
- `int get_bitrate() const property;`
- `void set_bitrate(int bitrate) property;`
- `int get_complexity() const property;`
- `void set_complexity(int complexity) property;`
- `int get_signal_type() const property;`
- `void set_signal_type(int signal_type) property;`
- `int get_application() const property;`
- `void set_application(int application) property;`
- `int get_packet_loss_percent() const property;`
- `void set_packet_loss_percent(int percent) property;`
- `bool get_vbr() const property;`
- `void set_vbr(bool enabled) property;`
- `bool get_cvbr() const property;`
- `void set_cvbr(bool enabled) property;`
- `bool get_dtx() const property;`
- `void set_dtx(bool enabled) property;`

### audio_peak_filter

Construction:

- `audio_peak_filter@ audio_peak_filter(double gain_db, double q, double frequency, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_gain(double gain) property;`
- `double get_gain() const property;`
- `void set_q(double q) property;`
- `double get_q() const property;`
- `void set_frequency(double frequency) property;`
- `double get_frequency() const property;`

### audio_ring_buffer

Construction:

- `audio_ring_buffer@ audio_ring_buffer(uint channels, uint size, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_advised_read_frame_count() const property;`
- `array<float>@ read(uint64 frame_count = 0);`
- `uint64 skip_frames(uint64 frame_count);`
- `float skip_milliseconds(float ms);`
- `bool seek_frames(uint64 frame_index);`
- `uint64 get_cursor_frames() const property;`
- `bool seek_milliseconds(float ms);`
- `float get_cursor_milliseconds() const property;`
- `uint64 get_length_frames() const property;`
- `float get_length_milliseconds() const property;`
- `bool set_looping(bool looping);`
- `bool get_looping() const property;`
- `bool set_range(uint64 start_frame, uint64 end_frame);`
- `void get_range(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_loop_point(uint64 start_frame, uint64 end_frame);`
- `void get_loop_point(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_current(audio_data_source@ new_current);`
- `audio_data_source@ get_current() const property;`
- `bool set_next(audio_data_source@ new_next);`
- `audio_data_source@ get_next() const property;`
- `uint get_channels() const property;`
- `uint get_sample_rate() const property;`
- `bool get_active() const property;`
- `audio_data_source@ opImplCast();`
- `void reset();`
- `uint write(const array<float>@ frames);`
- `uint write(const memory_buffer<float>&inout frames);`
- `uint get_available_read() const property;`
- `uint get_available_write() const property;`
- `microphone@ opCast();`

### audio_splitter_node

Construction:

- `audio_splitter_node@ audio_splitter_node(audio_engine@ engine, int channels);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`

### audio_wav_encoder

Construction:

- `audio_wav_encoder@ audio_wav_encoder(audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_default_open_flags() const property;`
- `bool open(uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(const string&in filename, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool open(datastream@ stream, uint sample_rate, uint channels, uint flags = AUDIO_ENCODER_DEFAULTS);`
- `bool close();`
- `bool get_active() const property;`
- `uint64 get_frames_written() const property;`
- `uint write(const array<float>@ frames);`
- `uint write(const memory_buffer<float>&inout frames);`
- `string read();`
- `string get_format() const property;`
- `uint get_sample_rate() const property;`
- `uint get_channels() const property;`
- `audio_encoder@ opImplCast();`
- `audio_format get_wav_format() const property;`

### base32_decoder

Construction:

- `base32_decoder@ base32_decoder();`
- `base32_decoder@ base32_decoder(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `base32_decoder& opShr(int8&out);`
- `int8 read_int8();`
- `base32_decoder& opShl(int8);`
- `base32_decoder& write_int8(int8);`
- `base32_decoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `base32_decoder& opShl(uint8);`
- `base32_decoder& write_uint8(uint8);`
- `base32_decoder& opShr(int16&out);`
- `int16 read_int16();`
- `base32_decoder& opShl(int16);`
- `base32_decoder& write_int16(int16);`
- `base32_decoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `base32_decoder& opShl(uint16);`
- `base32_decoder& write_uint16(uint16);`
- `base32_decoder& opShr(int&out);`
- `int read_int();`
- `base32_decoder& opShl(int);`
- `base32_decoder& write_int(int);`
- `base32_decoder& opShr(uint&out);`
- `uint read_uint();`
- `base32_decoder& opShl(uint);`
- `base32_decoder& write_uint(uint);`
- `base32_decoder& opShr(int64&out);`
- `int64 read_int64();`
- `base32_decoder& opShl(int64);`
- `base32_decoder& write_int64(int64);`
- `base32_decoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `base32_decoder& opShl(uint64);`
- `base32_decoder& write_uint64(uint64);`
- `base32_decoder& opShr(float&out);`
- `float read_float();`
- `base32_decoder& opShl(float);`
- `base32_decoder& write_float(float);`
- `base32_decoder& opShr(double&out);`
- `double read_double();`
- `base32_decoder& opShl(double);`
- `base32_decoder& write_double(double);`
- `base32_decoder& opShr(string&out);`
- `string read_string();`
- `base32_decoder& opShl(string);`
- `base32_decoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`

### base32_encoder

Construction:

- `base32_encoder@ base32_encoder();`
- `base32_encoder@ base32_encoder(datastream@, bool padding = true, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `base32_encoder& opShr(int8&out);`
- `int8 read_int8();`
- `base32_encoder& opShl(int8);`
- `base32_encoder& write_int8(int8);`
- `base32_encoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `base32_encoder& opShl(uint8);`
- `base32_encoder& write_uint8(uint8);`
- `base32_encoder& opShr(int16&out);`
- `int16 read_int16();`
- `base32_encoder& opShl(int16);`
- `base32_encoder& write_int16(int16);`
- `base32_encoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `base32_encoder& opShl(uint16);`
- `base32_encoder& write_uint16(uint16);`
- `base32_encoder& opShr(int&out);`
- `int read_int();`
- `base32_encoder& opShl(int);`
- `base32_encoder& write_int(int);`
- `base32_encoder& opShr(uint&out);`
- `uint read_uint();`
- `base32_encoder& opShl(uint);`
- `base32_encoder& write_uint(uint);`
- `base32_encoder& opShr(int64&out);`
- `int64 read_int64();`
- `base32_encoder& opShl(int64);`
- `base32_encoder& write_int64(int64);`
- `base32_encoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `base32_encoder& opShl(uint64);`
- `base32_encoder& write_uint64(uint64);`
- `base32_encoder& opShr(float&out);`
- `float read_float();`
- `base32_encoder& opShl(float);`
- `base32_encoder& write_float(float);`
- `base32_encoder& opShr(double&out);`
- `double read_double();`
- `base32_encoder& opShl(double);`
- `base32_encoder& write_double(double);`
- `base32_encoder& opShr(string&out);`
- `string read_string();`
- `base32_encoder& opShl(string);`
- `base32_encoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, bool padding = true, const string&in = "", int byteorder = 1);`

### base64_decoder

Construction:

- `base64_decoder@ base64_decoder();`
- `base64_decoder@ base64_decoder(datastream@, int options = 0, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `base64_decoder& opShr(int8&out);`
- `int8 read_int8();`
- `base64_decoder& opShl(int8);`
- `base64_decoder& write_int8(int8);`
- `base64_decoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `base64_decoder& opShl(uint8);`
- `base64_decoder& write_uint8(uint8);`
- `base64_decoder& opShr(int16&out);`
- `int16 read_int16();`
- `base64_decoder& opShl(int16);`
- `base64_decoder& write_int16(int16);`
- `base64_decoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `base64_decoder& opShl(uint16);`
- `base64_decoder& write_uint16(uint16);`
- `base64_decoder& opShr(int&out);`
- `int read_int();`
- `base64_decoder& opShl(int);`
- `base64_decoder& write_int(int);`
- `base64_decoder& opShr(uint&out);`
- `uint read_uint();`
- `base64_decoder& opShl(uint);`
- `base64_decoder& write_uint(uint);`
- `base64_decoder& opShr(int64&out);`
- `int64 read_int64();`
- `base64_decoder& opShl(int64);`
- `base64_decoder& write_int64(int64);`
- `base64_decoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `base64_decoder& opShl(uint64);`
- `base64_decoder& write_uint64(uint64);`
- `base64_decoder& opShr(float&out);`
- `float read_float();`
- `base64_decoder& opShl(float);`
- `base64_decoder& write_float(float);`
- `base64_decoder& opShr(double&out);`
- `double read_double();`
- `base64_decoder& opShl(double);`
- `base64_decoder& write_double(double);`
- `base64_decoder& opShr(string&out);`
- `string read_string();`
- `base64_decoder& opShl(string);`
- `base64_decoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, int options = 0, const string&in = "", int byteorder = 1);`

### base64_encoder

Construction:

- `base64_encoder@ base64_encoder();`
- `base64_encoder@ base64_encoder(datastream@, int options = 0, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `base64_encoder& opShr(int8&out);`
- `int8 read_int8();`
- `base64_encoder& opShl(int8);`
- `base64_encoder& write_int8(int8);`
- `base64_encoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `base64_encoder& opShl(uint8);`
- `base64_encoder& write_uint8(uint8);`
- `base64_encoder& opShr(int16&out);`
- `int16 read_int16();`
- `base64_encoder& opShl(int16);`
- `base64_encoder& write_int16(int16);`
- `base64_encoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `base64_encoder& opShl(uint16);`
- `base64_encoder& write_uint16(uint16);`
- `base64_encoder& opShr(int&out);`
- `int read_int();`
- `base64_encoder& opShl(int);`
- `base64_encoder& write_int(int);`
- `base64_encoder& opShr(uint&out);`
- `uint read_uint();`
- `base64_encoder& opShl(uint);`
- `base64_encoder& write_uint(uint);`
- `base64_encoder& opShr(int64&out);`
- `int64 read_int64();`
- `base64_encoder& opShl(int64);`
- `base64_encoder& write_int64(int64);`
- `base64_encoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `base64_encoder& opShl(uint64);`
- `base64_encoder& write_uint64(uint64);`
- `base64_encoder& opShr(float&out);`
- `float read_float();`
- `base64_encoder& opShl(float);`
- `base64_encoder& write_float(float);`
- `base64_encoder& opShr(double&out);`
- `double read_double();`
- `base64_encoder& opShl(double);`
- `base64_encoder& write_double(double);`
- `base64_encoder& opShr(string&out);`
- `string read_string();`
- `base64_encoder& opShl(string);`
- `base64_encoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, int options = 0, const string&in = "", int byteorder = 1);`

### calendar

Construction:

- `calendar@ calendar();`
- `calendar@ calendar(double julian_day);`
- `calendar@ calendar(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0, int microsecond = 0);`
- `calendar@ calendar(const datetime&in);`
- `calendar@ calendar(const calendar&in);`

Methods:

- `calendar& opAssign(const calendar&in);`
- `calendar& opAssign(const timestamp&in);`
- `calendar& opAssign(double julian_day);`
- `calendar& set(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0, int microsecond = 0);`
- `int get_year() const property;`
- `int get_yearday() const property;`
- `int get_month() const property;`
- `int week(int first_day_of_week = 1) const;`
- `int get_weekday() const property;`
- `int get_day() const property;`
- `int get_hour() const property;`
- `int get_hour12() const property;`
- `bool get_AM() const property;`
- `bool get_PM() const property;`
- `int get_minute() const property;`
- `int get_second() const property;`
- `int get_millisecond() const property;`
- `int get_microsecond() const property;`
- `double get_julian_day() const property;`
- `int get_tzd() const property;`
- `datetime@ get_UTC() const property;`
- `timestamp get_timestamp() const property;`
- `int64 get_UTC_time() const property;`
- `bool opEquals(const calendar&in) const;`
- `int opCmp(const calendar&in) const;`
- `calendar@ opAdd(const timespan&in) const;`
- `calendar@ opSub(const timespan&in) const;`
- `timespan opSub(const calendar&in) const;`
- `calendar& opAddAssign(const timespan&in);`
- `calendar& opSubAssign(const timespan&in);`
- `void reset();`
- `string get_month_name() const property;`
- `string get_weekday_name() const property;`
- `bool add_days(int amount);`
- `bool add_hours(int amount);`
- `bool add_minutes(int amount);`
- `bool add_seconds(int amount);`
- `bool add_months(int amount);`
- `bool add_years(int amount);`
- `int64 diff_days(calendar@ other);`
- `int64 diff_hours(calendar@ other);`
- `int64 diff_minutes(calendar@ other);`
- `int64 diff_seconds(calendar@ other);`
- `double diff_years(calendar@ other);`
- `int64 diff_months(calendar@ other);`
- `bool get_valid() const property;`
- `bool get_leap_year();`
- `string format(const string&in fmt);`

### combination

Construction:

- `combination@ combination();`

Methods:

- `void reset();`
- `bool generate_all_combinations(int items, int size);`
- `bool generate_all_combinations(int items, int min_size, int max_size);`
- `bool generate_unique_combinations(int items, int size);`
- `bool generate_unique_combinations(int items, int min_size, int max_size);`
- `bool generate_permutations(int items);`
- `bool next(array<int>@ list);`
- `bool get_active() property;`

### complex

Properties:

- `float r;`
- `float i;`

Methods:

- `complex& opAddAssign(const complex&in);`
- `complex& opSubAssign(const complex&in);`
- `complex& opMulAssign(const complex&in);`
- `complex& opDivAssign(const complex&in);`
- `bool opEquals(const complex&in) const;`
- `complex opAdd(const complex&in) const;`
- `complex opSub(const complex&in) const;`
- `complex opMul(const complex&in) const;`
- `complex opDiv(const complex&in) const;`
- `float abs() const;`
- `complex get_ri() const property;`
- `complex get_ir() const property;`
- `void set_ri(const complex&in) property;`
- `void set_ir(const complex&in) property;`

### const_weakref<T>

Methods:

- `const T@ opImplCast() const;`
- `const T@ get() const;`
- `const_weakref<T>& opHndlAssign(const const_weakref<T>&in);`
- `const_weakref<T>& opAssign(const const_weakref<T>&in);`
- `bool opEquals(const const_weakref<T>&in) const;`
- `const_weakref<T>& opHndlAssign(const T@);`
- `bool opEquals(const T@) const;`
- `const_weakref<T>& opHndlAssign(const weakref<T>&in);`
- `bool opEquals(const weakref<T>&in) const;`

### coordinate_map

Construction:

- `coordinate_map@ coordinate_map();`

Methods:

- `coordinate_map_area@ add_area(float minx, float maxx, float miny, float maxy, float minz, float maxz, float rotation, any@ primary_data, const string&in data1, const string&in data2, const string&in data3, int priority, int64 flags = 0);`
- `array<coordinate_map_area@>@ get_areas(float x, float y, float z, float d = 0.0, coordinate_map_filter_callback@ = null, int64 required_flags = 0, int64 excluded_flags = 0) const;`
- `array<coordinate_map_area@>@ get_areas(float minx, float maxx, float miny, float maxy, float minz, float maxz, float d = 0.0, coordinate_map_filter_callback@ = null, int64 required_flags = 0, int64 excluded_flags = 0) const;`
- `coordinate_map_area@ get_area(float x, float y, float z, int priority = -1, float d = 0.0, coordinate_map_filter_callback@ = null, int64 required_flags = 0, int64 excluded_flags = 0) const;`
- `void reset();`

### coordinate_map_area

Properties:

- `const coordinate_map@ map;`
- `const float minx;`
- `const float maxx;`
- `const float miny;`
- `const float maxy;`
- `const float minz;`
- `const float maxz;`
- `const float rotation;`
- `any@ primary_data;`
- `const string data1;`
- `const string data2;`
- `const string data3;`
- `const int priority;`
- `const bool framed;`
- `int64 flags;`

Methods:

- `void unframe();`
- `void reframe();`
- `void set(float minx, float maxx, float miny, float maxy, float minz, float maxz, float theta);`
- `void set_area(float minx, float maxx, float miny, float maxy, float minz, float maxz);`
- `void set_rotation(float theta);`
- `bool is_in_area(float x, float y, float z, float d = 0.0, coordinate_map_filter_callback@ = null, int64 required_flags = 0, int64 excluded_flags = 0) const;`

### counting_reader

Construction:

- `counting_reader@ counting_reader();`
- `counting_reader@ counting_reader(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `counting_reader& opShr(int8&out);`
- `int8 read_int8();`
- `counting_reader& opShl(int8);`
- `counting_reader& write_int8(int8);`
- `counting_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `counting_reader& opShl(uint8);`
- `counting_reader& write_uint8(uint8);`
- `counting_reader& opShr(int16&out);`
- `int16 read_int16();`
- `counting_reader& opShl(int16);`
- `counting_reader& write_int16(int16);`
- `counting_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `counting_reader& opShl(uint16);`
- `counting_reader& write_uint16(uint16);`
- `counting_reader& opShr(int&out);`
- `int read_int();`
- `counting_reader& opShl(int);`
- `counting_reader& write_int(int);`
- `counting_reader& opShr(uint&out);`
- `uint read_uint();`
- `counting_reader& opShl(uint);`
- `counting_reader& write_uint(uint);`
- `counting_reader& opShr(int64&out);`
- `int64 read_int64();`
- `counting_reader& opShl(int64);`
- `counting_reader& write_int64(int64);`
- `counting_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `counting_reader& opShl(uint64);`
- `counting_reader& write_uint64(uint64);`
- `counting_reader& opShr(float&out);`
- `float read_float();`
- `counting_reader& opShl(float);`
- `counting_reader& write_float(float);`
- `counting_reader& opShr(double&out);`
- `double read_double();`
- `counting_reader& opShl(double);`
- `counting_reader& write_double(double);`
- `counting_reader& opShr(string&out);`
- `string read_string();`
- `counting_reader& opShl(string);`
- `counting_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`
- `int64 get_chars() property;`
- `int64 get_lines() property;`
- `int64 get_current_line() property;`
- `void reset();`
- `void set_current_line(int64);`
- `void add_chars(int64);`
- `void add_lines(int64);`
- `void add_pos(int64);`

### counting_writer

Construction:

- `counting_writer@ counting_writer();`
- `counting_writer@ counting_writer(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `counting_writer& opShr(int8&out);`
- `int8 read_int8();`
- `counting_writer& opShl(int8);`
- `counting_writer& write_int8(int8);`
- `counting_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `counting_writer& opShl(uint8);`
- `counting_writer& write_uint8(uint8);`
- `counting_writer& opShr(int16&out);`
- `int16 read_int16();`
- `counting_writer& opShl(int16);`
- `counting_writer& write_int16(int16);`
- `counting_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `counting_writer& opShl(uint16);`
- `counting_writer& write_uint16(uint16);`
- `counting_writer& opShr(int&out);`
- `int read_int();`
- `counting_writer& opShl(int);`
- `counting_writer& write_int(int);`
- `counting_writer& opShr(uint&out);`
- `uint read_uint();`
- `counting_writer& opShl(uint);`
- `counting_writer& write_uint(uint);`
- `counting_writer& opShr(int64&out);`
- `int64 read_int64();`
- `counting_writer& opShl(int64);`
- `counting_writer& write_int64(int64);`
- `counting_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `counting_writer& opShl(uint64);`
- `counting_writer& write_uint64(uint64);`
- `counting_writer& opShr(float&out);`
- `float read_float();`
- `counting_writer& opShl(float);`
- `counting_writer& write_float(float);`
- `counting_writer& opShr(double&out);`
- `double read_double();`
- `counting_writer& opShl(double);`
- `counting_writer& write_double(double);`
- `counting_writer& opShr(string&out);`
- `string read_string();`
- `counting_writer& opShl(string);`
- `counting_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`
- `int64 get_chars() property;`
- `int64 get_lines() property;`
- `int64 get_current_line() property;`
- `void reset();`
- `void set_current_line(int64);`
- `void add_chars(int64);`
- `void add_lines(int64);`
- `void add_pos(int64);`

### datastream

Construction:

- `datastream@ datastream(const string&in = "");`
- `datastream@ datastream(const string&in initial_data, const string&in encoding, int byteorder = STREAM_BYTE_ORDER_NATIVE);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `datastream& opShr(int8&out);`
- `int8 read_int8();`
- `datastream& opShl(int8);`
- `datastream& write_int8(int8);`
- `datastream& opShr(uint8&out);`
- `uint8 read_uint8();`
- `datastream& opShl(uint8);`
- `datastream& write_uint8(uint8);`
- `datastream& opShr(int16&out);`
- `int16 read_int16();`
- `datastream& opShl(int16);`
- `datastream& write_int16(int16);`
- `datastream& opShr(uint16&out);`
- `uint16 read_uint16();`
- `datastream& opShl(uint16);`
- `datastream& write_uint16(uint16);`
- `datastream& opShr(int&out);`
- `int read_int();`
- `datastream& opShl(int);`
- `datastream& write_int(int);`
- `datastream& opShr(uint&out);`
- `uint read_uint();`
- `datastream& opShl(uint);`
- `datastream& write_uint(uint);`
- `datastream& opShr(int64&out);`
- `int64 read_int64();`
- `datastream& opShl(int64);`
- `datastream& write_int64(int64);`
- `datastream& opShr(uint64&out);`
- `uint64 read_uint64();`
- `datastream& opShl(uint64);`
- `datastream& write_uint64(uint64);`
- `datastream& opShr(float&out);`
- `float read_float();`
- `datastream& opShl(float);`
- `datastream& write_float(float);`
- `datastream& opShr(double&out);`
- `double read_double();`
- `datastream& opShl(double);`
- `datastream& write_double(double);`
- `datastream& opShr(string&out);`
- `string read_string();`
- `datastream& opShl(string);`
- `datastream& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(const string&in initial_data = "", const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `string str();`
- `void str(const string&in new_data);`
- `file@ opCast();`
- `hex_decoder@ opCast();`
- `hex_encoder@ opCast();`
- `base32_decoder@ opCast();`
- `base32_encoder@ opCast();`
- `base64_decoder@ opCast();`
- `base64_encoder@ opCast();`
- `random_reader@ opCast();`
- `discarding_writer@ opCast();`
- `duplicating_reader@ opCast();`
- `duplicating_writer@ opCast();`
- `deflating_reader@ opCast();`
- `deflating_writer@ opCast();`
- `inflating_reader@ opCast();`
- `inflating_writer@ opCast();`
- `counting_reader@ opCast();`
- `counting_writer@ opCast();`
- `line_converting_reader@ opCast();`
- `line_converting_writer@ opCast();`
- `asset_encryptor@ opCast();`
- `asset_decryptor@ opCast();`
- `memory_reader@ opCast();`
- `memory_writer@ opCast();`

### datetime

Construction:

- `datetime@ datetime();`
- `datetime@ datetime(const timestamp&in timestamp);`
- `datetime@ datetime(double julian_day);`
- `datetime@ datetime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0, int microsecond = 0);`
- `datetime@ datetime(const datetime&in);`

Methods:

- `datetime& opAssign(const datetime&in);`
- `datetime& opAssign(const timestamp&in);`
- `datetime& opAssign(double julian_day);`
- `datetime& set(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0, int microsecond = 0);`
- `int get_year() const property;`
- `int get_yearday() const property;`
- `int get_month() const property;`
- `int week(int first_day_of_week = 1) const;`
- `int get_weekday() const property;`
- `int get_day() const property;`
- `int get_hour() const property;`
- `int get_hour12() const property;`
- `bool get_AM() const property;`
- `bool get_PM() const property;`
- `int get_minute() const property;`
- `int get_second() const property;`
- `int get_millisecond() const property;`
- `int get_microsecond() const property;`
- `double get_julian_day() const property;`
- `timestamp get_timestamp() const property;`
- `int64 get_UTC_time() const property;`
- `bool opEquals(const datetime&in) const;`
- `int opCmp(const datetime&in) const;`
- `datetime@ opAdd(const timespan&in) const;`
- `datetime@ opSub(const timespan&in) const;`
- `timespan opSub(const datetime&in) const;`
- `datetime& opAddAssign(const timespan&in);`
- `datetime& opSubAssign(const timespan&in);`
- `void make_UTC(int timezone_offset);`
- `void make_local(int timezone_offset);`
- `void reset();`
- `string get_month_name() const property;`
- `string get_weekday_name() const property;`
- `bool add_days(int amount);`
- `bool add_hours(int amount);`
- `bool add_minutes(int amount);`
- `bool add_seconds(int amount);`
- `bool add_months(int amount);`
- `bool add_years(int amount);`
- `int64 diff_days(datetime@ other);`
- `int64 diff_hours(datetime@ other);`
- `int64 diff_minutes(datetime@ other);`
- `int64 diff_seconds(datetime@ other);`
- `double diff_years(datetime@ other);`
- `int64 diff_months(datetime@ other);`
- `bool get_valid() const property;`
- `bool get_leap_year();`
- `string format(const string&in fmt, int tzd = 0xffff);`

### deflating_reader

Construction:

- `deflating_reader@ deflating_reader();`
- `deflating_reader@ deflating_reader(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, int level = 9, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `deflating_reader& opShr(int8&out);`
- `int8 read_int8();`
- `deflating_reader& opShl(int8);`
- `deflating_reader& write_int8(int8);`
- `deflating_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `deflating_reader& opShl(uint8);`
- `deflating_reader& write_uint8(uint8);`
- `deflating_reader& opShr(int16&out);`
- `int16 read_int16();`
- `deflating_reader& opShl(int16);`
- `deflating_reader& write_int16(int16);`
- `deflating_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `deflating_reader& opShl(uint16);`
- `deflating_reader& write_uint16(uint16);`
- `deflating_reader& opShr(int&out);`
- `int read_int();`
- `deflating_reader& opShl(int);`
- `deflating_reader& write_int(int);`
- `deflating_reader& opShr(uint&out);`
- `uint read_uint();`
- `deflating_reader& opShl(uint);`
- `deflating_reader& write_uint(uint);`
- `deflating_reader& opShr(int64&out);`
- `int64 read_int64();`
- `deflating_reader& opShl(int64);`
- `deflating_reader& write_int64(int64);`
- `deflating_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `deflating_reader& opShl(uint64);`
- `deflating_reader& write_uint64(uint64);`
- `deflating_reader& opShr(float&out);`
- `float read_float();`
- `deflating_reader& opShl(float);`
- `deflating_reader& write_float(float);`
- `deflating_reader& opShr(double&out);`
- `double read_double();`
- `deflating_reader& opShl(double);`
- `deflating_reader& write_double(double);`
- `deflating_reader& opShr(string&out);`
- `string read_string();`
- `deflating_reader& opShl(string);`
- `deflating_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, int level = 9, const string&in = "", int byteorder = 1);`

### deflating_writer

Construction:

- `deflating_writer@ deflating_writer();`
- `deflating_writer@ deflating_writer(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, int level = 9, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `deflating_writer& opShr(int8&out);`
- `int8 read_int8();`
- `deflating_writer& opShl(int8);`
- `deflating_writer& write_int8(int8);`
- `deflating_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `deflating_writer& opShl(uint8);`
- `deflating_writer& write_uint8(uint8);`
- `deflating_writer& opShr(int16&out);`
- `int16 read_int16();`
- `deflating_writer& opShl(int16);`
- `deflating_writer& write_int16(int16);`
- `deflating_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `deflating_writer& opShl(uint16);`
- `deflating_writer& write_uint16(uint16);`
- `deflating_writer& opShr(int&out);`
- `int read_int();`
- `deflating_writer& opShl(int);`
- `deflating_writer& write_int(int);`
- `deflating_writer& opShr(uint&out);`
- `uint read_uint();`
- `deflating_writer& opShl(uint);`
- `deflating_writer& write_uint(uint);`
- `deflating_writer& opShr(int64&out);`
- `int64 read_int64();`
- `deflating_writer& opShl(int64);`
- `deflating_writer& write_int64(int64);`
- `deflating_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `deflating_writer& opShl(uint64);`
- `deflating_writer& write_uint64(uint64);`
- `deflating_writer& opShr(float&out);`
- `float read_float();`
- `deflating_writer& opShl(float);`
- `deflating_writer& write_float(float);`
- `deflating_writer& opShr(double&out);`
- `double read_double();`
- `deflating_writer& opShl(double);`
- `deflating_writer& write_double(double);`
- `deflating_writer& opShr(string&out);`
- `string read_string();`
- `deflating_writer& opShl(string);`
- `deflating_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, int level = 9, const string&in = "", int byteorder = 1);`

### dictionary

Construction:

- `dictionary@ dictionary();`

Methods:

- `dictionary& opAssign(const dictionary&in);`
- `void set(const string&in, const ?&in);`
- `bool get(const string&in, ?&out) const;`
- `void set(const string&in, const int64&in);`
- `bool get(const string&in, int64&out) const;`
- `void set(const string&in, const double&in);`
- `bool get(const string&in, double&out) const;`
- `bool exists(const string&in) const;`
- `bool is_empty() const;`
- `uint get_size() const;`
- `bool delete(const string&in);`
- `void delete_all();`
- `array<string>@ get_keys() const;`
- `dictionaryValue& opIndex(const string&in);`
- `const dictionaryValue& opIndex(const string&in) const;`
- `dictionaryIter@ opForBegin() const;`
- `bool opForEnd(dictionaryIter@) const;`
- `dictionaryIter@ opForNext(dictionaryIter@) const;`
- `const dictionaryValue& opForValue0(dictionaryIter@) const;`
- `const string& opForValue1(dictionaryIter@) const;`
- `bool empty() const;`
- `uint size() const;`
- `void erase(const string&in);`
- `void clear();`
- `string serialize();`

### dictionaryIter

### dictionaryValue

Methods:

- `dictionaryValue& opAssign(const dictionaryValue&in);`
- `dictionaryValue& opHndlAssign(const ?&in);`
- `dictionaryValue& opHndlAssign(const dictionaryValue&in);`
- `dictionaryValue& opAssign(const ?&in);`
- `dictionaryValue& opAssign(double);`
- `dictionaryValue& opAssign(int64);`
- `void opCast(?&out);`
- `void opConv(?&out);`
- `int64 opConv();`
- `double opConv();`

### discarding_writer

Construction:

- `discarding_writer@ discarding_writer(const string&in encoding = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `bool open(const string&in encoding = "", int byteorder = 1);`
- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `discarding_writer& opShr(int8&out);`
- `int8 read_int8();`
- `discarding_writer& opShl(int8);`
- `discarding_writer& write_int8(int8);`
- `discarding_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `discarding_writer& opShl(uint8);`
- `discarding_writer& write_uint8(uint8);`
- `discarding_writer& opShr(int16&out);`
- `int16 read_int16();`
- `discarding_writer& opShl(int16);`
- `discarding_writer& write_int16(int16);`
- `discarding_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `discarding_writer& opShl(uint16);`
- `discarding_writer& write_uint16(uint16);`
- `discarding_writer& opShr(int&out);`
- `int read_int();`
- `discarding_writer& opShl(int);`
- `discarding_writer& write_int(int);`
- `discarding_writer& opShr(uint&out);`
- `uint read_uint();`
- `discarding_writer& opShl(uint);`
- `discarding_writer& write_uint(uint);`
- `discarding_writer& opShr(int64&out);`
- `int64 read_int64();`
- `discarding_writer& opShl(int64);`
- `discarding_writer& write_int64(int64);`
- `discarding_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `discarding_writer& opShl(uint64);`
- `discarding_writer& write_uint64(uint64);`
- `discarding_writer& opShr(float&out);`
- `float read_float();`
- `discarding_writer& opShl(float);`
- `discarding_writer& write_float(float);`
- `discarding_writer& opShr(double&out);`
- `double read_double();`
- `discarding_writer& opShl(double);`
- `discarding_writer& write_double(double);`
- `discarding_writer& opShr(string&out);`
- `string read_string();`
- `discarding_writer& opShl(string);`
- `discarding_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`

### dns_host_entry

Methods:

- `dns_host_entry& opAssign(const dns_host_entry&in e);`
- `const string& get_name() const property;`
- `array<string>@ get_aliases() const;`
- `array<spec::ip_address>@ get_addresses() const;`

### duplicating_reader

Construction:

- `duplicating_reader@ duplicating_reader();`
- `duplicating_reader@ duplicating_reader(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `duplicating_reader& opShr(int8&out);`
- `int8 read_int8();`
- `duplicating_reader& opShl(int8);`
- `duplicating_reader& write_int8(int8);`
- `duplicating_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `duplicating_reader& opShl(uint8);`
- `duplicating_reader& write_uint8(uint8);`
- `duplicating_reader& opShr(int16&out);`
- `int16 read_int16();`
- `duplicating_reader& opShl(int16);`
- `duplicating_reader& write_int16(int16);`
- `duplicating_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `duplicating_reader& opShl(uint16);`
- `duplicating_reader& write_uint16(uint16);`
- `duplicating_reader& opShr(int&out);`
- `int read_int();`
- `duplicating_reader& opShl(int);`
- `duplicating_reader& write_int(int);`
- `duplicating_reader& opShr(uint&out);`
- `uint read_uint();`
- `duplicating_reader& opShl(uint);`
- `duplicating_reader& write_uint(uint);`
- `duplicating_reader& opShr(int64&out);`
- `int64 read_int64();`
- `duplicating_reader& opShl(int64);`
- `duplicating_reader& write_int64(int64);`
- `duplicating_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `duplicating_reader& opShl(uint64);`
- `duplicating_reader& write_uint64(uint64);`
- `duplicating_reader& opShr(float&out);`
- `float read_float();`
- `duplicating_reader& opShl(float);`
- `duplicating_reader& write_float(float);`
- `duplicating_reader& opShr(double&out);`
- `double read_double();`
- `duplicating_reader& opShl(double);`
- `duplicating_reader& write_double(double);`
- `duplicating_reader& opShr(string&out);`
- `string read_string();`
- `duplicating_reader& opShl(string);`
- `duplicating_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`
- `duplicating_reader@ opAdd(datastream@);`
- `duplicating_reader@ opAddAssign(datastream@);`
- `duplicating_reader@ add(datastream@);`

### duplicating_writer

Construction:

- `duplicating_writer@ duplicating_writer(const string&in encoding = "", int byteorder = 1);`
- `duplicating_writer@ duplicating_writer(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `bool open(const string&in encoding = "", int byteorder = 1);`
- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `duplicating_writer& opShr(int8&out);`
- `int8 read_int8();`
- `duplicating_writer& opShl(int8);`
- `duplicating_writer& write_int8(int8);`
- `duplicating_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `duplicating_writer& opShl(uint8);`
- `duplicating_writer& write_uint8(uint8);`
- `duplicating_writer& opShr(int16&out);`
- `int16 read_int16();`
- `duplicating_writer& opShl(int16);`
- `duplicating_writer& write_int16(int16);`
- `duplicating_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `duplicating_writer& opShl(uint16);`
- `duplicating_writer& write_uint16(uint16);`
- `duplicating_writer& opShr(int&out);`
- `int read_int();`
- `duplicating_writer& opShl(int);`
- `duplicating_writer& write_int(int);`
- `duplicating_writer& opShr(uint&out);`
- `uint read_uint();`
- `duplicating_writer& opShl(uint);`
- `duplicating_writer& write_uint(uint);`
- `duplicating_writer& opShr(int64&out);`
- `int64 read_int64();`
- `duplicating_writer& opShl(int64);`
- `duplicating_writer& write_int64(int64);`
- `duplicating_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `duplicating_writer& opShl(uint64);`
- `duplicating_writer& write_uint64(uint64);`
- `duplicating_writer& opShr(float&out);`
- `float read_float();`
- `duplicating_writer& opShl(float);`
- `duplicating_writer& write_float(float);`
- `duplicating_writer& opShr(double&out);`
- `double read_double();`
- `duplicating_writer& opShl(double);`
- `duplicating_writer& write_double(double);`
- `duplicating_writer& opShr(string&out);`
- `string read_string();`
- `duplicating_writer& opShl(string);`
- `duplicating_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`
- `duplicating_writer@ opAdd(datastream@);`
- `duplicating_writer@ opAddAssign(datastream@);`
- `duplicating_writer@ add(datastream@);`

### engine_character_event

Methods:

- `int find(engine_character_event_listener@ listener) const;`
- `bool insert(engine_character_event_listener@ listener, int index = -1);`
- `bool opAddAssign(engine_character_event_listener@ listener);`
- `bool remove(engine_character_event_listener@ listener);`
- `bool opSubAssign(engine_character_event_listener@ listener);`
- `int find(engine_character_event_callback@ listener) const;`
- `bool insert(engine_character_event_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_character_event_callback@ listener);`
- `bool remove(engine_character_event_callback@ listener);`
- `bool opSubAssign(engine_character_event_callback@ listener);`
- `int find(engine_character_event_passthrough_callback@ listener) const;`
- `bool insert(engine_character_event_passthrough_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_character_event_passthrough_callback@ listener);`
- `bool remove(engine_character_event_passthrough_callback@ listener);`
- `bool opSubAssign(engine_character_event_passthrough_callback@ listener);`
- `bool remove(uint index);`
- `void opCall(string character);`
- `void clear();`
- `uint get_count() const property;`

### engine_key_event

Methods:

- `int find(engine_key_event_listener@ listener) const;`
- `bool insert(engine_key_event_listener@ listener, int index = -1);`
- `bool opAddAssign(engine_key_event_listener@ listener);`
- `bool remove(engine_key_event_listener@ listener);`
- `bool opSubAssign(engine_key_event_listener@ listener);`
- `int find(engine_key_event_callback@ listener) const;`
- `bool insert(engine_key_event_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_key_event_callback@ listener);`
- `bool remove(engine_key_event_callback@ listener);`
- `bool opSubAssign(engine_key_event_callback@ listener);`
- `int find(engine_key_event_passthrough_callback@ listener) const;`
- `bool insert(engine_key_event_passthrough_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_key_event_passthrough_callback@ listener);`
- `bool remove(engine_key_event_passthrough_callback@ listener);`
- `bool opSubAssign(engine_key_event_passthrough_callback@ listener);`
- `bool remove(uint index);`
- `void opCall(int key);`
- `void clear();`
- `uint get_count() const property;`

### engine_touch_event

Methods:

- `int find(engine_touch_event_listener@ listener) const;`
- `bool insert(engine_touch_event_listener@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_event_listener@ listener);`
- `bool remove(engine_touch_event_listener@ listener);`
- `bool opSubAssign(engine_touch_event_listener@ listener);`
- `int find(engine_touch_event_callback@ listener) const;`
- `bool insert(engine_touch_event_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_event_callback@ listener);`
- `bool remove(engine_touch_event_callback@ listener);`
- `bool opSubAssign(engine_touch_event_callback@ listener);`
- `int find(engine_touch_event_passthrough_callback@ listener) const;`
- `bool insert(engine_touch_event_passthrough_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_event_passthrough_callback@ listener);`
- `bool remove(engine_touch_event_passthrough_callback@ listener);`
- `bool opSubAssign(engine_touch_event_passthrough_callback@ listener);`
- `bool remove(uint index);`
- `void opCall(uint64 device, const touch_finger&inout finger);`
- `void clear();`
- `uint get_count() const property;`

### engine_touch_motion_event

Methods:

- `int find(engine_touch_motion_event_listener@ listener) const;`
- `bool insert(engine_touch_motion_event_listener@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_motion_event_listener@ listener);`
- `bool remove(engine_touch_motion_event_listener@ listener);`
- `bool opSubAssign(engine_touch_motion_event_listener@ listener);`
- `int find(engine_touch_motion_event_callback@ listener) const;`
- `bool insert(engine_touch_motion_event_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_motion_event_callback@ listener);`
- `bool remove(engine_touch_motion_event_callback@ listener);`
- `bool opSubAssign(engine_touch_motion_event_callback@ listener);`
- `int find(engine_touch_motion_event_passthrough_callback@ listener) const;`
- `bool insert(engine_touch_motion_event_passthrough_callback@ listener, int index = -1);`
- `bool opAddAssign(engine_touch_motion_event_passthrough_callback@ listener);`
- `bool remove(engine_touch_motion_event_passthrough_callback@ listener);`
- `bool opSubAssign(engine_touch_motion_event_passthrough_callback@ listener);`
- `bool remove(uint index);`
- `void opCall(uint64 device, const touch_finger&inout finger, float relative_x, float relative_y);`
- `void clear();`
- `uint get_count() const property;`

### fast_mutex

Construction:

- `fast_mutex@ fast_mutex();`

Methods:

- `void lock(uint);`
- `bool try_lock(uint);`
- `void lock();`
- `bool try_lock();`
- `void unlock();`

### fast_mutex_lock

Methods:

- `void unlock();`

### file

Construction:

- `file@ file();`
- `file@ file(const string&in, const string&in, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `file& opShr(int8&out);`
- `int8 read_int8();`
- `file& opShl(int8);`
- `file& write_int8(int8);`
- `file& opShr(uint8&out);`
- `uint8 read_uint8();`
- `file& opShl(uint8);`
- `file& write_uint8(uint8);`
- `file& opShr(int16&out);`
- `int16 read_int16();`
- `file& opShl(int16);`
- `file& write_int16(int16);`
- `file& opShr(uint16&out);`
- `uint16 read_uint16();`
- `file& opShl(uint16);`
- `file& write_uint16(uint16);`
- `file& opShr(int&out);`
- `int read_int();`
- `file& opShl(int);`
- `file& write_int(int);`
- `file& opShr(uint&out);`
- `uint read_uint();`
- `file& opShl(uint);`
- `file& write_uint(uint);`
- `file& opShr(int64&out);`
- `int64 read_int64();`
- `file& opShl(int64);`
- `file& write_int64(int64);`
- `file& opShr(uint64&out);`
- `uint64 read_uint64();`
- `file& opShl(uint64);`
- `file& write_uint64(uint64);`
- `file& opShr(float&out);`
- `float read_float();`
- `file& opShl(float);`
- `file& write_float(float);`
- `file& opShr(double&out);`
- `double read_double();`
- `file& opShl(double);`
- `file& write_double(double);`
- `file& opShr(string&out);`
- `string read_string();`
- `file& opShl(string);`
- `file& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(const string&in, const string&in, const string&in = "", int byteorder = 1);`
- `uint64 get_size() const property;`

### ftp_client

Construction:

- `ftp_client@ ftp_client(uint16 active_data_port = 0);`
- `ftp_client@ ftp_client(const string&in host, uint16 port = 21, const string&in username = "", const string&in password = "", uint16 active_data_port = 0);`

Methods:

- `void set_passive(bool passive, bool use_rfc1738 = true);`
- `bool get_passive() const property;`
- `void open(const string&in host, uint16 port, const string&in username = "", const string&in password = "");`
- `void login(const string&in username, const string&in password);`
- `void logout();`
- `void close();`
- `string system_type();`
- `void set_file_type(ftp_file_type type);`
- `ftp_file_type get_file_type() const property;`
- `void set_working_directory(const string&in path);`
- `string get_working_directory();`
- `void cdup();`
- `void rename(const string&in source, const string&in destination);`
- `void remove(const string&in path);`
- `void create_directory(const string&in path);`
- `void remove_directory(const string&in path);`
- `datastream@ begin_download(const string&in path, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `void end_download();`
- `datastream@ begin_upload(const string&in path, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `void end_upload();`
- `datastream@ begin_list(const string&in path = "", bool extended = false, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `void end_list();`
- `void abort();`
- `int send_command(const string&in command, string&inout response);`
- `int send_command(const string&in command, const string&in argument, string&inout response);`
- `bool get_is_open() const property;`
- `bool get_is_logged_in() const property;`
- `bool get_is_secure() const property;`
- `const string& get_welcome_message() const property;`

### grid<T>

Construction:

- `grid<T>@ grid(int&in);`
- `grid<T>@ grid(int&in, uint, uint);`
- `grid<T>@ grid(int&in, uint, uint, const T&in);`

Methods:

- `T& opIndex(uint, uint);`
- `const T& opIndex(uint, uint) const;`
- `void resize(uint width, uint height);`
- `uint width() const;`
- `uint height() const;`

### hex_decoder

Construction:

- `hex_decoder@ hex_decoder();`
- `hex_decoder@ hex_decoder(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `hex_decoder& opShr(int8&out);`
- `int8 read_int8();`
- `hex_decoder& opShl(int8);`
- `hex_decoder& write_int8(int8);`
- `hex_decoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `hex_decoder& opShl(uint8);`
- `hex_decoder& write_uint8(uint8);`
- `hex_decoder& opShr(int16&out);`
- `int16 read_int16();`
- `hex_decoder& opShl(int16);`
- `hex_decoder& write_int16(int16);`
- `hex_decoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `hex_decoder& opShl(uint16);`
- `hex_decoder& write_uint16(uint16);`
- `hex_decoder& opShr(int&out);`
- `int read_int();`
- `hex_decoder& opShl(int);`
- `hex_decoder& write_int(int);`
- `hex_decoder& opShr(uint&out);`
- `uint read_uint();`
- `hex_decoder& opShl(uint);`
- `hex_decoder& write_uint(uint);`
- `hex_decoder& opShr(int64&out);`
- `int64 read_int64();`
- `hex_decoder& opShl(int64);`
- `hex_decoder& write_int64(int64);`
- `hex_decoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `hex_decoder& opShl(uint64);`
- `hex_decoder& write_uint64(uint64);`
- `hex_decoder& opShr(float&out);`
- `float read_float();`
- `hex_decoder& opShl(float);`
- `hex_decoder& write_float(float);`
- `hex_decoder& opShr(double&out);`
- `double read_double();`
- `hex_decoder& opShl(double);`
- `hex_decoder& write_double(double);`
- `hex_decoder& opShr(string&out);`
- `string read_string();`
- `hex_decoder& opShl(string);`
- `hex_decoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`

### hex_encoder

Construction:

- `hex_encoder@ hex_encoder();`
- `hex_encoder@ hex_encoder(datastream@, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `hex_encoder& opShr(int8&out);`
- `int8 read_int8();`
- `hex_encoder& opShl(int8);`
- `hex_encoder& write_int8(int8);`
- `hex_encoder& opShr(uint8&out);`
- `uint8 read_uint8();`
- `hex_encoder& opShl(uint8);`
- `hex_encoder& write_uint8(uint8);`
- `hex_encoder& opShr(int16&out);`
- `int16 read_int16();`
- `hex_encoder& opShl(int16);`
- `hex_encoder& write_int16(int16);`
- `hex_encoder& opShr(uint16&out);`
- `uint16 read_uint16();`
- `hex_encoder& opShl(uint16);`
- `hex_encoder& write_uint16(uint16);`
- `hex_encoder& opShr(int&out);`
- `int read_int();`
- `hex_encoder& opShl(int);`
- `hex_encoder& write_int(int);`
- `hex_encoder& opShr(uint&out);`
- `uint read_uint();`
- `hex_encoder& opShl(uint);`
- `hex_encoder& write_uint(uint);`
- `hex_encoder& opShr(int64&out);`
- `int64 read_int64();`
- `hex_encoder& opShl(int64);`
- `hex_encoder& write_int64(int64);`
- `hex_encoder& opShr(uint64&out);`
- `uint64 read_uint64();`
- `hex_encoder& opShl(uint64);`
- `hex_encoder& write_uint64(uint64);`
- `hex_encoder& opShr(float&out);`
- `float read_float();`
- `hex_encoder& opShl(float);`
- `hex_encoder& write_float(float);`
- `hex_encoder& opShr(double&out);`
- `double read_double();`
- `hex_encoder& opShl(double);`
- `hex_encoder& write_double(double);`
- `hex_encoder& opShr(string&out);`
- `string read_string();`
- `hex_encoder& opShl(string);`
- `hex_encoder& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in = "", int byteorder = 1);`

### http

Construction:

- `http@ http();`

Methods:

- `bool get(const spec::uri&in url, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool head(const spec::uri&in url, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool post(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool put(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool options(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool delete(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool trace(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool connect(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `bool patch(const spec::uri&in url, const string&in body, const name_value_collection@ headers = null, const http_credentials@ creds = null);`
- `http_response@ get_response_headers() property;`
- `string get_response_body() property;`
- `string request();`
- `string opIndex(const string&in key);`
- `spec::uri get_url() property;`
- `float get_progress() property;`
- `int get_status_code() property;`
- `string get_user_agent() const property;`
- `void set_user_agent(const string&in agent = "") property;`
- `int get_max_retries() const property;`
- `void set_max_retries(int retries) property;`
- `int get_retry_delay() const property;`
- `void set_retry_delay(int delay = 0) property;`
- `bool get_complete() property;`
- `bool get_running() property;`
- `void wait();`
- `void reset();`

### http_client

Construction:

- `http_client@ http_client(const string&in, uint16 = 80);`

Methods:

- `void set_keep_alive(bool) property;`
- `bool get_keep_alive() const property;`
- `bool get_connected() const property;`
- `void abort();`
- `void set_keep_alive_timeout(const timespan&in timeout) property;`
- `timespan get_keep_alive_timeout() const property;`
- `void set_send_timeout(const timespan&in timeout) property;`
- `timespan get_send_timeout() const property;`
- `void set_receive_timeout(const timespan&in timeout) property;`
- `timespan get_receive_timeout() const property;`
- `void set_host(const string&in) property;`
- `const string& get_host() const property;`
- `void set_port(uint16) property;`
- `uint16 get_port() const property;`
- `datastream@ send_request(http_request&inout, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `datastream@ receive_response(http_response&inout, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `bool peek_response(http_response&inout);`
- `void flush_request();`
- `void reset();`
- `bool get_secure() const property;`
- `https_client@ opCast();`

### http_credentials

Construction:

- `http_credentials@ http_credentials();`
- `http_credentials@ http_credentials(const string&in username, const string&in password);`

Methods:

- `void from_user_info(const string&in user_info);`
- `void from_uri(const spec::uri&in uri);`
- `void clear();`
- `void set_username(const string&in username) property;`
- `string get_username() const property;`
- `void set_password(const string&in password) property;`
- `string get_password() const property;`
- `void set_host(const string&in host) property;`
- `string get_host() const property;`
- `bool get_empty() const property;`
- `void authenticate(http_request&inout request, const http_response&in response);`
- `void update_auth_info(http_request&inout request);`
- `void proxy_authenticate(http_request&inout request, const http_response&in response);`
- `void update_proxy_auth_info(http_request&inout request);`

### http_request

Construction:

- `http_request@ http_request();`
- `http_request@ http_request(const http_request&in);`
- `http_request@ http_request(const string&in, const string&in, const string&in = HTTP_1_1);`

Methods:

- `http_request& opAssign(const http_request&in);`
- `const string& get_opIndex(const string&in) const property;`
- `void set_opIndex(const string&in, const string&in) property;`
- `void set(const string&in, const string&in);`
- `void add(const string&in, const string&in);`
- `const string& get(const string&in, const string&in = "") const;`
- `bool exists(const string&in) const;`
- `bool empty() const;`
- `uint64 size() const;`
- `void erase(const string&in);`
- `void secure_erase(const string&in);`
- `void clear();`
- `const string& name_at(uint) const;`
- `const string& value_at(uint) const;`
- `internet_message_header@ opImplCast();`
- `bool write(datastream@) const;`
- `bool read(datastream@);`
- `bool get_auto_decode() const property;`
- `void set_auto_decode(bool) property;`
- `string get_decoded(const string&in, const string&in = "");`
- `int get_field_limit() const property;`
- `void set_field_limit(int) property;`
- `int get_name_length_limit() const property;`
- `void set_name_length_limit(int) property;`
- `int get_value_length_limit() const property;`
- `void set_value_length_limit(int) property;`
- `bool has_token(const string&in, const string&in);`
- `void set_version(const string&in) property;`
- `const string& get_version() const property;`
- `void set_content_length(int64) property;`
- `int64 get_content_length() const property;`
- `bool get_has_content_length() const property;`
- `void set_transfer_encoding(const string&in) property;`
- `string get_transfer_encoding() const property;`
- `void set_chunked_transfer_encoding(bool) property;`
- `bool get_chunked_transfer_encoding() const property;`
- `void set_content_type(const string&in) property;`
- `string get_content_type() const property;`
- `void set_keep_alive(bool) property;`
- `bool get_keep_alive() const property;`
- `void set_method(const string&in) property;`
- `const string& get_method() const property;`
- `void set_uri(const string&in) property;`
- `const string& get_uri() const property;`
- `void set_host(const string&in) property;`
- `void set_host(const string&in, uint16) property;`
- `const string& get_host() const property;`
- `void set_cookies(const name_value_collection&inout);`
- `void get_cookies(name_value_collection&inout) const;`
- `bool get_has_credentials() const property;`
- `void get_credentials(string&inout, string&inout) const;`
- `void set_credentials(const string&in, const string&in);`
- `void remove_credentials();`
- `bool get_expect_continue() const property;`
- `void set_expect_continue(bool) property;`
- `bool get_has_proxy_credentials() const property;`
- `void get_proxy_credentials(string&inout, string&inout) const;`
- `void set_proxy_credentials(const string&in, const string&in);`
- `void remove_proxy_credentials();`

### http_response

Construction:

- `http_response@ http_response();`
- `http_response@ http_response(const http_response&in);`
- `http_response@ http_response(http_status);`
- `http_response@ http_response(http_status, const string&in);`
- `http_response@ http_response(const string&in, http_status, const string&in);`
- `http_response@ http_response(const string&in, http_status);`

Methods:

- `http_response& opAssign(const http_response&in);`
- `const string& get_opIndex(const string&in) const property;`
- `void set_opIndex(const string&in, const string&in) property;`
- `void set(const string&in, const string&in);`
- `void add(const string&in, const string&in);`
- `const string& get(const string&in, const string&in = "") const;`
- `bool exists(const string&in) const;`
- `bool empty() const;`
- `uint64 size() const;`
- `void erase(const string&in);`
- `void secure_erase(const string&in);`
- `void clear();`
- `const string& name_at(uint) const;`
- `const string& value_at(uint) const;`
- `internet_message_header@ opImplCast();`
- `bool write(datastream@) const;`
- `bool read(datastream@);`
- `bool get_auto_decode() const property;`
- `void set_auto_decode(bool) property;`
- `string get_decoded(const string&in, const string&in = "");`
- `int get_field_limit() const property;`
- `void set_field_limit(int) property;`
- `int get_name_length_limit() const property;`
- `void set_name_length_limit(int) property;`
- `int get_value_length_limit() const property;`
- `void set_value_length_limit(int) property;`
- `bool has_token(const string&in, const string&in);`
- `void set_version(const string&in) property;`
- `const string& get_version() const property;`
- `void set_content_length(int64) property;`
- `int64 get_content_length() const property;`
- `bool get_has_content_length() const property;`
- `void set_transfer_encoding(const string&in) property;`
- `string get_transfer_encoding() const property;`
- `void set_chunked_transfer_encoding(bool) property;`
- `bool get_chunked_transfer_encoding() const property;`
- `void set_content_type(const string&in) property;`
- `string get_content_type() const property;`
- `void set_keep_alive(bool) property;`
- `bool get_keep_alive() const property;`
- `void set_status(http_status) property;`
- `http_status get_status() const property;`
- `void set_status(const string&in);`
- `void set_reason(const string&in) property;`
- `const string& get_reason() const property;`
- `void set_status_and_reason(http_status, const string&in);`
- `void set_status_and_reason(http_status);`

### https_client

Construction:

- `https_client@ https_client(const string&in, uint16 = 443);`

Methods:

- `void set_keep_alive(bool) property;`
- `bool get_keep_alive() const property;`
- `bool get_connected() const property;`
- `void abort();`
- `void set_keep_alive_timeout(const timespan&in timeout) property;`
- `timespan get_keep_alive_timeout() const property;`
- `void set_send_timeout(const timespan&in timeout) property;`
- `timespan get_send_timeout() const property;`
- `void set_receive_timeout(const timespan&in timeout) property;`
- `timespan get_receive_timeout() const property;`
- `void set_host(const string&in) property;`
- `const string& get_host() const property;`
- `void set_port(uint16) property;`
- `uint16 get_port() const property;`
- `datastream@ send_request(http_request&inout, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `datastream@ receive_response(http_response&inout, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `bool peek_response(http_response&inout);`
- `void flush_request();`
- `void reset();`
- `bool get_secure() const property;`
- `http_client@ opImplCast();`

### inflating_reader

Construction:

- `inflating_reader@ inflating_reader();`
- `inflating_reader@ inflating_reader(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `inflating_reader& opShr(int8&out);`
- `int8 read_int8();`
- `inflating_reader& opShl(int8);`
- `inflating_reader& write_int8(int8);`
- `inflating_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `inflating_reader& opShl(uint8);`
- `inflating_reader& write_uint8(uint8);`
- `inflating_reader& opShr(int16&out);`
- `int16 read_int16();`
- `inflating_reader& opShl(int16);`
- `inflating_reader& write_int16(int16);`
- `inflating_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `inflating_reader& opShl(uint16);`
- `inflating_reader& write_uint16(uint16);`
- `inflating_reader& opShr(int&out);`
- `int read_int();`
- `inflating_reader& opShl(int);`
- `inflating_reader& write_int(int);`
- `inflating_reader& opShr(uint&out);`
- `uint read_uint();`
- `inflating_reader& opShl(uint);`
- `inflating_reader& write_uint(uint);`
- `inflating_reader& opShr(int64&out);`
- `int64 read_int64();`
- `inflating_reader& opShl(int64);`
- `inflating_reader& write_int64(int64);`
- `inflating_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `inflating_reader& opShl(uint64);`
- `inflating_reader& write_uint64(uint64);`
- `inflating_reader& opShr(float&out);`
- `float read_float();`
- `inflating_reader& opShl(float);`
- `inflating_reader& write_float(float);`
- `inflating_reader& opShr(double&out);`
- `double read_double();`
- `inflating_reader& opShl(double);`
- `inflating_reader& write_double(double);`
- `inflating_reader& opShr(string&out);`
- `string read_string();`
- `inflating_reader& opShl(string);`
- `inflating_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, const string&in = "", int byteorder = 1);`

### inflating_writer

Construction:

- `inflating_writer@ inflating_writer();`
- `inflating_writer@ inflating_writer(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `inflating_writer& opShr(int8&out);`
- `int8 read_int8();`
- `inflating_writer& opShl(int8);`
- `inflating_writer& write_int8(int8);`
- `inflating_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `inflating_writer& opShl(uint8);`
- `inflating_writer& write_uint8(uint8);`
- `inflating_writer& opShr(int16&out);`
- `int16 read_int16();`
- `inflating_writer& opShl(int16);`
- `inflating_writer& write_int16(int16);`
- `inflating_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `inflating_writer& opShl(uint16);`
- `inflating_writer& write_uint16(uint16);`
- `inflating_writer& opShr(int&out);`
- `int read_int();`
- `inflating_writer& opShl(int);`
- `inflating_writer& write_int(int);`
- `inflating_writer& opShr(uint&out);`
- `uint read_uint();`
- `inflating_writer& opShl(uint);`
- `inflating_writer& write_uint(uint);`
- `inflating_writer& opShr(int64&out);`
- `int64 read_int64();`
- `inflating_writer& opShl(int64);`
- `inflating_writer& write_int64(int64);`
- `inflating_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `inflating_writer& opShl(uint64);`
- `inflating_writer& write_uint64(uint64);`
- `inflating_writer& opShr(float&out);`
- `float read_float();`
- `inflating_writer& opShl(float);`
- `inflating_writer& write_float(float);`
- `inflating_writer& opShr(double&out);`
- `double read_double();`
- `inflating_writer& opShl(double);`
- `inflating_writer& write_double(double);`
- `inflating_writer& opShr(string&out);`
- `string read_string();`
- `inflating_writer& opShl(string);`
- `inflating_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, compression_method compression = COMPRESSION_METHOD_ZLIB, const string&in = "", int byteorder = 1);`

### internet_message_header

Construction:

- `internet_message_header@ internet_message_header();`
- `internet_message_header@ internet_message_header(const internet_message_header&in);`

Methods:

- `internet_message_header& opAssign(const internet_message_header&in);`
- `const string& get_opIndex(const string&in) const property;`
- `void set_opIndex(const string&in, const string&in) property;`
- `void set(const string&in, const string&in);`
- `void add(const string&in, const string&in);`
- `const string& get(const string&in, const string&in = "") const;`
- `bool exists(const string&in) const;`
- `bool empty() const;`
- `uint64 size() const;`
- `void erase(const string&in);`
- `void secure_erase(const string&in);`
- `void clear();`
- `const string& name_at(uint) const;`
- `const string& value_at(uint) const;`
- `name_value_collection@ opImplCast();`
- `bool write(datastream@) const;`
- `bool read(datastream@);`
- `bool get_auto_decode() const property;`
- `void set_auto_decode(bool) property;`
- `string get_decoded(const string&in, const string&in = "");`
- `int get_field_limit() const property;`
- `void set_field_limit(int) property;`
- `int get_name_length_limit() const property;`
- `void set_name_length_limit(int) property;`
- `int get_value_length_limit() const property;`
- `void set_value_length_limit(int) property;`
- `bool has_token(const string&in, const string&in);`
- `http_request@ opCast();`
- `http_response@ opCast();`

### spec::ip_address

Methods:

- `spec::ip_address& opAssign(const spec::ip_address&in addr);`
- `bool get_is_v4() const property;`
- `bool get_is_v6() const property;`
- `spec::ip_address_family get_family() const property;`
- `uint get_scope() const property;`
- `string opImplConv() const;`
- `bool get_is_wildcard() const property;`
- `bool get_is_broadcast() const property;`
- `bool get_is_loopback() const property;`
- `bool get_is_multicast() const property;`
- `bool get_is_unicast() const property;`
- `bool get_is_link_local() const property;`
- `bool get_is_site_local() const property;`
- `bool get_is_IPV4_compatible() const property;`
- `bool get_is_IPV4_mapped() const property;`
- `bool get_is_well_known_multicast() const property;`
- `bool get_is_node_local_multicast() const property;`
- `bool get_is_link_local_multicast() const property;`
- `bool get_is_site_local_multicast() const property;`
- `bool get_is_org_local_multicast() const property;`
- `bool get_is_global_multicast() const property;`
- `bool opEquals(const spec::ip_address&in addr) const;`
- `int opCmp(const spec::ip_address&in);`
- `spec::ip_address opAnd(const spec::ip_address&in addr) const;`
- `spec::ip_address opOr(const spec::ip_address&in addr) const;`
- `spec::ip_address opXor(const spec::ip_address&in addr) const;`
- `spec::ip_address opCom() const;`
- `uint get_prefix_length() const property;`
- `void mask(const spec::ip_address&in mask);`
- `void mask(const spec::ip_address&in mask, const spec::ip_address&in set);`

### joystick

Construction:

- `joystick@ joystick();`

Methods:

- `uint get_joysticks() const property;`
- `bool get_has_x() const property;`
- `bool get_has_y() const property;`
- `bool get_has_z() const property;`
- `bool get_has_r_x() const property;`
- `bool get_has_r_y() const property;`
- `bool get_has_r_z() const property;`
- `uint get_buttons() const property;`
- `uint get_sliders() const property;`
- `uint get_povs() const property;`
- `string get_name() const property;`
- `bool get_active() const property;`
- `int get_preferred_joystick() const property;`
- `void set_preferred_joystick(int index) property;`
- `int get_x() const property;`
- `int get_y() const property;`
- `int get_z() const property;`
- `int get_r_x() const property;`
- `int get_r_y() const property;`
- `int get_r_z() const property;`
- `int get_slider_1() const property;`
- `int get_slider_2() const property;`
- `int get_pov_1() const property;`
- `int get_pov_2() const property;`
- `int get_pov_3() const property;`
- `int get_pov_4() const property;`
- `int get_v_x() const property;`
- `int get_v_y() const property;`
- `int get_v_z() const property;`
- `int get_vr_x() const property;`
- `int get_vr_y() const property;`
- `int get_vr_z() const property;`
- `int get_v_slider_1() const property;`
- `int get_v_slider_2() const property;`
- `int get_a_x() const property;`
- `int get_a_y() const property;`
- `int get_a_z() const property;`
- `int get_ar_x() const property;`
- `int get_ar_y() const property;`
- `int get_ar_z() const property;`
- `int get_a_slider_1() const property;`
- `int get_a_slider_2() const property;`
- `int get_f_x() const property;`
- `int get_f_y() const property;`
- `int get_f_z() const property;`
- `int get_fr_x() const property;`
- `int get_fr_y() const property;`
- `int get_fr_z() const property;`
- `int get_f_slider_1() const property;`
- `int get_f_slider_2() const property;`
- `bool button_down(int button);`
- `bool button_pressed(int button);`
- `bool button_released(int button);`
- `bool button_up(int button);`
- `array<int>@ buttons_down();`
- `array<int>@ buttons_pressed();`
- `array<int>@ buttons_released();`
- `array<int>@ buttons_up();`
- `array<string>@ list_joysticks();`
- `bool pov_centered(int pov);`
- `bool refresh_joystick_list();`
- `bool set(int index);`
- `uint get_type() const property;`
- `joystick_power_info get_power_info() const property;`
- `bool get_has_led() const property;`
- `bool get_can_vibrate() const property;`
- `bool get_can_vibrate_triggers() const property;`
- `int get_touchpads() const property;`
- `string get_serial() const property;`
- `bool set_led(uint8 red, uint8 green, uint8 blue);`
- `bool vibrate(uint16 low_frequency, uint16 high_frequency, int duration);`
- `bool vibrate_triggers(uint16 left, uint16 right, int duration);`

### joystick_power_info

Properties:

- `int state;`
- `int percentage;`

Methods:

- `string get_state_name() const property;`
- `string to_string() const;`
- `string opConv() const;`
- `string opImplConv() const;`

### json_array

Construction:

- `json_array@ json_array();`
- `json_array@ json_array(json_array@ other);`

Methods:

- `json_array& opAssign(json_array@ other);`
- `var@ get_opIndex(uint index) property;`
- `void set_opIndex(uint index, const var&in value) property;`
- `void add(const var&in value);`
- `var@ opCall(const string&in path) const;`
- `json_array& extend(const json_array@ array);`
- `json_array@ get_array(uint index) const;`
- `json_object@ get_object(uint index) const;`
- `string stringify(uint indent = 0, int step = -1);`
- `void stringify(datastream@ stream, uint indent = 0, int step = -1);`
- `uint length();`
- `uint size();`
- `bool get_escape_unicode() property;`
- `void set_escape_unicode(bool value) property;`
- `bool get_empty() property;`
- `void clear();`
- `void remove(uint index);`
- `bool is_array(uint index);`
- `bool is_null(uint index);`
- `bool is_object(uint index);`

### json_object

Construction:

- `json_object@ json_object();`
- `json_object@ json_object(json_object@ other);`

Methods:

- `json_object& opAssign(json_object@ other);`
- `var@ get_opIndex(const string&in key) const property;`
- `void set_opIndex(const string&in key, const var&in value) property;`
- `void set(const string&in key, const var&in value);`
- `var@ get(const string&in key, var@ default_value = null) const;`
- `var@ opCall(const string&in path, var@ default_value = null) const;`
- `json_array@ get_array(const string&in key) const;`
- `json_object@ get_object(const string&in key) const;`
- `string stringify(uint indent = 0, int step = -1) const;`
- `void stringify(datastream@ stream, uint indent = 0, int step = -1) const;`
- `uint size() const;`
- `bool get_escape_unicode() const property;`
- `void set_escape_unicode(bool value) property;`
- `void clear();`
- `void remove(const string&in key);`
- `bool exists(const string&in key) const;`
- `bool is_array(const string&in key) const;`
- `bool is_null(const string&in key) const;`
- `bool is_object(const string&in key) const;`
- `array<string>@ get_keys() const;`

### library

Construction:

- `library@ library();`

Methods:

- `bool load(const string&in filename);`
- `bool unload();`
- `bool get_active() const property;`
- `dictionary@ call(const string&in signature, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null, ?&in = null);`

### line_converting_reader

Construction:

- `line_converting_reader@ line_converting_reader();`
- `line_converting_reader@ line_converting_reader(datastream@, const string&in line_ending = spec::NEWLINE_DEFAULT, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `line_converting_reader& opShr(int8&out);`
- `int8 read_int8();`
- `line_converting_reader& opShl(int8);`
- `line_converting_reader& write_int8(int8);`
- `line_converting_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `line_converting_reader& opShl(uint8);`
- `line_converting_reader& write_uint8(uint8);`
- `line_converting_reader& opShr(int16&out);`
- `int16 read_int16();`
- `line_converting_reader& opShl(int16);`
- `line_converting_reader& write_int16(int16);`
- `line_converting_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `line_converting_reader& opShl(uint16);`
- `line_converting_reader& write_uint16(uint16);`
- `line_converting_reader& opShr(int&out);`
- `int read_int();`
- `line_converting_reader& opShl(int);`
- `line_converting_reader& write_int(int);`
- `line_converting_reader& opShr(uint&out);`
- `uint read_uint();`
- `line_converting_reader& opShl(uint);`
- `line_converting_reader& write_uint(uint);`
- `line_converting_reader& opShr(int64&out);`
- `int64 read_int64();`
- `line_converting_reader& opShl(int64);`
- `line_converting_reader& write_int64(int64);`
- `line_converting_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `line_converting_reader& opShl(uint64);`
- `line_converting_reader& write_uint64(uint64);`
- `line_converting_reader& opShr(float&out);`
- `float read_float();`
- `line_converting_reader& opShl(float);`
- `line_converting_reader& write_float(float);`
- `line_converting_reader& opShr(double&out);`
- `double read_double();`
- `line_converting_reader& opShl(double);`
- `line_converting_reader& write_double(double);`
- `line_converting_reader& opShr(string&out);`
- `string read_string();`
- `line_converting_reader& opShl(string);`
- `line_converting_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in line_ending = spec::NEWLINE_DEFAULT, const string&in = "", int byteorder = 1);`

### line_converting_writer

Construction:

- `line_converting_writer@ line_converting_writer();`
- `line_converting_writer@ line_converting_writer(datastream@, const string&in line_ending = spec::NEWLINE_DEFAULT, const string&in = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `line_converting_writer& opShr(int8&out);`
- `int8 read_int8();`
- `line_converting_writer& opShl(int8);`
- `line_converting_writer& write_int8(int8);`
- `line_converting_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `line_converting_writer& opShl(uint8);`
- `line_converting_writer& write_uint8(uint8);`
- `line_converting_writer& opShr(int16&out);`
- `int16 read_int16();`
- `line_converting_writer& opShl(int16);`
- `line_converting_writer& write_int16(int16);`
- `line_converting_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `line_converting_writer& opShl(uint16);`
- `line_converting_writer& write_uint16(uint16);`
- `line_converting_writer& opShr(int&out);`
- `int read_int();`
- `line_converting_writer& opShl(int);`
- `line_converting_writer& write_int(int);`
- `line_converting_writer& opShr(uint&out);`
- `uint read_uint();`
- `line_converting_writer& opShl(uint);`
- `line_converting_writer& write_uint(uint);`
- `line_converting_writer& opShr(int64&out);`
- `int64 read_int64();`
- `line_converting_writer& opShl(int64);`
- `line_converting_writer& write_int64(int64);`
- `line_converting_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `line_converting_writer& opShl(uint64);`
- `line_converting_writer& write_uint64(uint64);`
- `line_converting_writer& opShr(float&out);`
- `float read_float();`
- `line_converting_writer& opShl(float);`
- `line_converting_writer& write_float(float);`
- `line_converting_writer& opShr(double&out);`
- `double read_double();`
- `line_converting_writer& opShl(double);`
- `line_converting_writer& write_double(double);`
- `line_converting_writer& opShr(string&out);`
- `string read_string();`
- `line_converting_writer& opShl(string);`
- `line_converting_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(datastream@, const string&in line_ending = spec::NEWLINE_DEFAULT, const string&in = "", int byteorder = 1);`

### mail_message

Construction:

- `mail_message@ mail_message();`

Methods:

- `void set_sender(const string&in);`
- `string get_sender() const property;`
- `void add_recipient(const mail_recipient&in);`
- `void add_recipient(const string&in, mail_recipient_type = RECIPIENT_TO);`
- `array<mail_recipient@>@ get_recipients() const;`
- `void set_subject(const string&in);`
- `string get_subject() const property;`
- `void set_content(const string&in, const string&in = "text/plain");`
- `string get_content() const property;`
- `bool add_attachment_file(const string&in, const string&in);`
- `bool add_attachment_string(const string&in, const string&in, const string&in = "application/octet-stream");`
- `void set_html_content(const string&in, const string&in = "");`
- `void set_priority(mail_priority);`
- `int get_priority() const property;`
- `void add_header(const string&in, const string&in);`
- `string get_header(const string&in) const;`
- `void set_reply_to(const string&in) property;`
- `string get_reply_to() const property;`
- `void set_read_receipt(const string&in);`
- `string get_message_id() const property;`
- `void set_message_id(const string&in) property;`
- `string add_inline_attachment_file(const string&in, const string&in = "");`
- `string add_inline_attachment_string(const string&in, const string&in, const string&in);`
- `string get_last_error() const property;`
- `void set_in_reply_to(const string&in) property;`
- `string get_in_reply_to() const property;`
- `void set_references(const string&in) property;`
- `string get_references() const property;`
- `void set_return_receipt_to(const string&in) property;`
- `string get_return_receipt_to() const property;`
- `void set_disposition_notification_to(const string&in) property;`
- `string get_disposition_notification_to() const property;`

### mail_recipient

Construction:

- `mail_recipient@ mail_recipient();`
- `mail_recipient@ mail_recipient(mail_recipient_type, const string&in, const string&in = "");`

Properties:

- `mail_recipient_type type;`
- `string address;`
- `string real_name;`

### matrix3x3

Methods:

- `void set(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2, float c3);`
- `void set_to_zero();`
- `void set_to_identity();`
- `vector get_column(int i) const;`
- `vector get_row(int i) const;`
- `matrix3x3 get_transpose() const property;`
- `float get_determinant() const property;`
- `float get_trace() const property;`
- `matrix3x3 get_inverse() const property;`
- `matrix3x3 get_inverse(float determinant) const;`
- `matrix3x3 get_absolute() const property;`
- `matrix3x3 opAdd(const matrix3x3&in matrix) const;`
- `matrix3x3& opAddAssign(const matrix3x3&in matrix);`
- `matrix3x3 opSub(const matrix3x3&in matrix) const;`
- `matrix3x3& opSubAssign(const matrix3x3&in matrix);`
- `matrix3x3 opNeg() const;`
- `matrix3x3 opMul(const matrix3x3&in matrix) const;`
- `matrix3x3 opMul(float value) const;`
- `matrix3x3 opMulR(float value) const;`
- `matrix3x3& opMulAssign(float value);`
- `vector opMul(const vector&in value) const;`
- `bool opEquals(const matrix3x3&in);`
- `vector& opIndex(int row);`
- `const vector& opIndex(int row) const;`
- `string opImplConv();`

### memory_buffer<T>

Properties:

- `uint64 address;`
- `uint64 size;`

Methods:

- `T& opIndex(uint64 index);`
- `const T& opIndex(uint64 index) const;`
- `array<T>@ opImplConv() const;`
- `memory_buffer<T>& opAssign(array<T>@ array);`
- `int get_element_size() const property;`

### memory_reader

Construction:

- `memory_reader@ memory_reader();`
- `memory_reader@ memory_reader(uint64, uint64, const string&in encoding = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `memory_reader& opShr(int8&out);`
- `int8 read_int8();`
- `memory_reader& opShl(int8);`
- `memory_reader& write_int8(int8);`
- `memory_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `memory_reader& opShl(uint8);`
- `memory_reader& write_uint8(uint8);`
- `memory_reader& opShr(int16&out);`
- `int16 read_int16();`
- `memory_reader& opShl(int16);`
- `memory_reader& write_int16(int16);`
- `memory_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `memory_reader& opShl(uint16);`
- `memory_reader& write_uint16(uint16);`
- `memory_reader& opShr(int&out);`
- `int read_int();`
- `memory_reader& opShl(int);`
- `memory_reader& write_int(int);`
- `memory_reader& opShr(uint&out);`
- `uint read_uint();`
- `memory_reader& opShl(uint);`
- `memory_reader& write_uint(uint);`
- `memory_reader& opShr(int64&out);`
- `int64 read_int64();`
- `memory_reader& opShl(int64);`
- `memory_reader& write_int64(int64);`
- `memory_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `memory_reader& opShl(uint64);`
- `memory_reader& write_uint64(uint64);`
- `memory_reader& opShr(float&out);`
- `float read_float();`
- `memory_reader& opShl(float);`
- `memory_reader& write_float(float);`
- `memory_reader& opShr(double&out);`
- `double read_double();`
- `memory_reader& opShl(double);`
- `memory_reader& write_double(double);`
- `memory_reader& opShr(string&out);`
- `string read_string();`
- `memory_reader& opShl(string);`
- `memory_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(uint64, uint64, const string&in encoding = "", int byteorder = 1);`

### memory_writer

Construction:

- `memory_writer@ memory_writer();`
- `memory_writer@ memory_writer(uint64, uint64, const string&in encoding = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `memory_writer& opShr(int8&out);`
- `int8 read_int8();`
- `memory_writer& opShl(int8);`
- `memory_writer& write_int8(int8);`
- `memory_writer& opShr(uint8&out);`
- `uint8 read_uint8();`
- `memory_writer& opShl(uint8);`
- `memory_writer& write_uint8(uint8);`
- `memory_writer& opShr(int16&out);`
- `int16 read_int16();`
- `memory_writer& opShl(int16);`
- `memory_writer& write_int16(int16);`
- `memory_writer& opShr(uint16&out);`
- `uint16 read_uint16();`
- `memory_writer& opShl(uint16);`
- `memory_writer& write_uint16(uint16);`
- `memory_writer& opShr(int&out);`
- `int read_int();`
- `memory_writer& opShl(int);`
- `memory_writer& write_int(int);`
- `memory_writer& opShr(uint&out);`
- `uint read_uint();`
- `memory_writer& opShl(uint);`
- `memory_writer& write_uint(uint);`
- `memory_writer& opShr(int64&out);`
- `int64 read_int64();`
- `memory_writer& opShl(int64);`
- `memory_writer& write_int64(int64);`
- `memory_writer& opShr(uint64&out);`
- `uint64 read_uint64();`
- `memory_writer& opShl(uint64);`
- `memory_writer& write_uint64(uint64);`
- `memory_writer& opShr(float&out);`
- `float read_float();`
- `memory_writer& opShl(float);`
- `memory_writer& write_float(float);`
- `memory_writer& opShr(double&out);`
- `double read_double();`
- `memory_writer& opShl(double);`
- `memory_writer& write_double(double);`
- `memory_writer& opShr(string&out);`
- `string read_string();`
- `memory_writer& opShl(string);`
- `memory_writer& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`
- `bool open(uint64, uint64, const string&in encoding = "", int byteorder = 1);`

### microphone

Construction:

- `microphone@ microphone(int device = -1, audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `uint get_advised_read_frame_count() const property;`
- `array<float>@ read(uint64 frame_count = 0);`
- `uint64 skip_frames(uint64 frame_count);`
- `float skip_milliseconds(float ms);`
- `bool seek_frames(uint64 frame_index);`
- `uint64 get_cursor_frames() const property;`
- `bool seek_milliseconds(float ms);`
- `float get_cursor_milliseconds() const property;`
- `uint64 get_length_frames() const property;`
- `float get_length_milliseconds() const property;`
- `bool set_looping(bool looping);`
- `bool get_looping() const property;`
- `bool set_range(uint64 start_frame, uint64 end_frame);`
- `void get_range(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_loop_point(uint64 start_frame, uint64 end_frame);`
- `void get_loop_point(uint64&out start_frame, uint64&out end_frame) const;`
- `bool set_current(audio_data_source@ new_current);`
- `audio_data_source@ get_current() const property;`
- `bool set_next(audio_data_source@ new_next);`
- `audio_data_source@ get_next() const property;`
- `uint get_channels() const property;`
- `uint get_sample_rate() const property;`
- `bool get_active() const property;`
- `audio_data_source@ opImplCast();`
- `void reset();`
- `uint write(const array<float>@ frames);`
- `uint write(const memory_buffer<float>&inout frames);`
- `uint get_available_read() const property;`
- `uint get_available_write() const property;`
- `audio_ring_buffer@ opImplCast();`
- `bool set_device(int device);`
- `int get_device() const property;`
- `void set_volume(float volume);`
- `float get_volume() const property;`

### mixer

Construction:

- `mixer@ mixer();`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `audio_engine@ get_engine() const property;`
- `bool set_mixer(mixer@ parent_mixer);`
- `mixer@ get_mixer() const;`
- `void set_3d_panner(int panner_id);`
- `int get_3d_panner() const property;`
- `void set_3d_attenuator(int attenuator_id);`
- `int get_3d_attenuator() const property;`
- `int get_preferred_3d_panner() const property;`
- `int get_preferred_3d_attenuator() const property;`
- `void set_hrtf(bool enabled) property;`
- `bool get_hrtf() const property;`
- `bool set_shape(ref shape);`
- `ref get_shape() const property;`
- `void set_reverb3d(reverb3d@ reverb) property;`
- `void set_reverb3d_at(reverb3d@ reverb, reverb3d_placement placement);`
- `reverb3d@ get_reverb3d() const property;`
- `audio_splitter_node@ get_reverb3d_attachment() const property;`
- `reverb3d_placement get_reverb3d_placement() const property;`
- `audio_node_chain@ get_effects_chain() property;`
- `audio_node_chain@ get_internal_node_chain() property;`
- `bool play(bool reset_loop_state = true);`
- `bool play_looped();`
- `bool stop();`
- `void set_volume(float volume) property;`
- `float get_volume() const property;`
- `void set_pan(float pan) property;`
- `float get_pan() const property;`
- `void set_pan_mode(audio_pan_mode mode) property;`
- `audio_pan_mode get_pan_mode() const property;`
- `void set_pitch(float pitch) property;`
- `float get_pitch() const property;`
- `void set_spatialization_enabled(bool enabled) property;`
- `bool get_spatialization_enabled() const property;`
- `void set_pinned_listener(uint index) property;`
- `uint get_pinned_listener() const property;`
- `uint get_listener() const property;`
- `vector get_direction_to_listener() const;`
- `float get_distance_to_listener() const;`
- `void set_position_3d(float x, float y, float z);`
- `void set_position_3d(const vector&in position);`
- `vector get_position_3d() const;`
- `void set_direction(float x, float y, float z);`
- `void set_direction(const vector&in direction);`
- `vector get_direction() const;`
- `void set_velocity(float x, float y, float z);`
- `void set_velocity(const vector&in velocity);`
- `vector get_velocity() const;`
- `void set_positioning(audio_positioning_mode mode) property;`
- `audio_positioning_mode get_positioning() const property;`
- `void set_rolloff(float rolloff) property;`
- `float get_rolloff() const property;`
- `void set_min_gain(float gain) property;`
- `float get_min_gain() const property;`
- `void set_max_gain(float gain) property;`
- `float get_max_gain() const property;`
- `void set_min_distance(float distance) property;`
- `float get_min_distance() const property;`
- `void set_max_distance(float distance) property;`
- `float get_max_distance() const property;`
- `void set_cone(float inner_radians, float outer_radians, float outer_gain);`
- `void get_cone(float&out inner_radians, float&out outer_radians, float&out outer_gain);`
- `void set_doppler_factor(float factor) property;`
- `float get_doppler_factor() const property;`
- `void set_directional_attenuation_factor(float factor) property;`
- `float get_directional_attenuation_factor() const property;`
- `void set_fade(float start_volume, float end_volume, uint64 length);`
- `void set_fade_in_frames(float start_volume, float end_volume, uint64 length_frames);`
- `void set_fade_in_milliseconds(float start_volume, float end_volume, uint64 length_ms);`
- `float get_current_fade_volume() const property;`
- `void set_start_time(uint64 absolute_time) property;`
- `void set_stop_time(uint64 absolute_time);`
- `bool get_playing() const property;`

### mutex

Construction:

- `mutex@ mutex();`

Methods:

- `void lock(uint);`
- `bool try_lock(uint);`
- `void lock();`
- `bool try_lock();`
- `void unlock();`

### mutex_lock

Methods:

- `void unlock();`

### name_value_collection

Construction:

- `name_value_collection@ name_value_collection();`
- `name_value_collection@ name_value_collection(const name_value_collection&in);`

Methods:

- `name_value_collection& opAssign(const name_value_collection&in);`
- `const string& get_opIndex(const string&in) const property;`
- `void set_opIndex(const string&in, const string&in) property;`
- `void set(const string&in, const string&in);`
- `void add(const string&in, const string&in);`
- `const string& get(const string&in, const string&in = "") const;`
- `bool exists(const string&in) const;`
- `bool empty() const;`
- `uint64 size() const;`
- `void erase(const string&in);`
- `void secure_erase(const string&in);`
- `void clear();`
- `const string& name_at(uint) const;`
- `const string& value_at(uint) const;`
- `internet_message_header@ opCast();`

### named_mutex

Construction:

- `named_mutex@ named_mutex(const string&in);`

Methods:

- `void lock();`
- `bool try_lock();`
- `void unlock();`

### named_mutex_lock

Methods:

- `void unlock();`

### network

Construction:

- `network@ network();`

Properties:

- `bool IPV6enabled;`
- `bool receive_timeout_event;`
- `bool send_immediately;`

Methods:

- `void destroy(bool flush = true);`
- `bool setup_client(uint8 max_channels, uint16 max_peers);`
- `bool setup_server(uint16 port, uint8 max_channels, uint16 max_peers);`
- `bool setup_local_server(uint16 port, uint8 max_channels, uint16 max_peers);`
- `uint64 connect(const string&in host, uint16 port);`
- `const network_event@ request(uint timeout = 0);`
- `string get_peer_address(uint64 peer_id) const;`
- `uint get_peer_average_round_trip_time(uint64 peer_id) const;`
- `bool send(uint64 peer_id, const string&in message, uint8 channel, bool reliable = true);`
- `bool send_reliable(uint64 peer_id, const string&in message, uint8 channel);`
- `bool send_unreliable(uint64 peer_id, const string&in message, uint8 channel);`
- `bool send_peer(uint64 peer_pointer, const string&in message, uint8 channel, bool reliable = true);`
- `bool send_reliable_peer(uint64 peer_pointer, const string&in message, uint8 channel);`
- `bool send_unreliable_peer(uint64 peer_pointer, const string&in message, uint8 channel);`
- `bool flush();`
- `bool disconnect_peer_softly(uint64 peer_id);`
- `bool disconnect_peer(uint64 peer_id);`
- `bool disconnect_peer_forcefully(uint64 peer_id);`
- `array<uint64>@ get_peer_list() const;`
- `uint64 get_connected_peers() const property;`
- `bool get_packet_compression() const property;`
- `void set_packet_compression(bool compressed) property;`
- `uint get_duplicate_peers() const property;`
- `void set_duplicate_peers(uint max_duplicates) property;`
- `uint get_bytes_received() const property;`
- `uint get_bytes_sent() const property;`
- `uint get_packets_received() const property;`
- `uint get_packets_sent() const property;`
- `void set_bandwidth_limits(uint max_incoming_bytes_per_second, uint max_outgoing_bytes_per_second);`
- `bool get_active() const property;`

### network_event

Construction:

- `network_event@ network_event();`

Properties:

- `const network_event_type type;`
- `const uint64 peer_id;`
- `const uint channel;`
- `const string message;`

Methods:

- `network_event& opAssign(const network_event&in);`

### pack_file

Construction:

- `pack_file@ pack_file();`

Methods:

- `pack_interface@ opImplCast();`
- `bool create(const string&in filename, const string&in key = "");`
- `bool open(const string&in filename, const string&in key = "", uint64 pack_offset = 0, uint64 pack_size = 0);`
- `bool close();`
- `bool add_file(const string&in filename, const string&in internal_name);`
- `bool add_stream(const string&in internal_name, datastream@ ds);`
- `bool add_memory(const string&in internal_name, const string&in data);`
- `bool file_exists(const string&in filename);`
- `int64 get_file_size(const string&in filename);`
- `datastream@ get_file(const string&in filename, const string&in encoding = "", int byteorder = STREAM_BYTE_ORDER_NATIVE);`
- `string get_pack_name() const property;`
- `bool get_active() const property;`
- `int64 get_file_count() const property;`
- `array<string>@ list_files() const;`
- `bool extract_file(const string&in internal_name, const string&in file_on_disk);`

### pack_interface

Methods:

- `pack_file@ opCast();`

### spec::path

Methods:

- `spec::path& opAssign(const spec::path&in);`
- `spec::path& opAssign(const string&in);`
- `spec::path& assign(const string&in);`
- `spec::path& assign(const string&in, spec::path_style);`
- `spec::path& assign(const spec::path&in);`
- `spec::path& assign_directory(const string&in);`
- `spec::path& assign_directory(const string&in, spec::path_style);`
- `bool parse(const string&in);`
- `bool parse(const string&in, spec::path_style);`
- `string opImplConv() const;`
- `string to_string(spec::path_style = spec::PATH_STYLE_NATIVE) const;`
- `spec::path& make_directory();`
- `spec::path& make_file();`
- `spec::path& make_parent();`
- `spec::path& make_absolute();`
- `spec::path& make_absolute(const spec::path&in);`
- `spec::path& append(const spec::path&in);`
- `spec::path& resolve(const spec::path&in);`
- `bool get_is_absolute() const property;`
- `bool get_is_relative() const property;`
- `bool get_is_directory() const property;`
- `bool get_is_file() const property;`
- `spec::path& set_node(const string&in);`
- `const string& get_node() const property;`
- `spec::path& set_device(const string&in);`
- `const string& get_device() const property;`
- `int get_depth() const property;`
- `const string& get_opIndex(int) const property;`
- `spec::path& push_directory(const string&in);`
- `spec::path& pop_directory();`
- `spec::path& pop_front_directory();`
- `spec::path& set_filename(const string&in);`
- `const string& get_filename() const property;`
- `spec::path& set_basename(const string&in);`
- `string get_basename() const property;`
- `spec::path& set_extension(const string&in);`
- `string get_extension() const property;`
- `const string& get_vms_version() const property;`
- `spec::path& clear();`
- `spec::path get_parent() const property;`
- `spec::path absolute() const;`
- `spec::path absolute(const spec::path&in) const;`

### pathfinder

Construction:

- `pathfinder@ pathfinder(int = 1024, bool = true);`

Properties:

- `const bool solving;`
- `const float total_cost;`
- `int desperation_factor;`
- `bool allow_diagonals;`
- `bool automatic_reset;`
- `int search_range;`

Methods:

- `void set_callback_function(pathfinder_callback@);`
- `void set_callback_function(pathfinder_callback_ex@);`
- `void cancel();`
- `void set_callback_function(pathfinder_callback_legacy@);`
- `void reset();`
- `array<vector>@ find(int, int, int, int, int, int, any@ = null);`
- `array<vector>@ find(int, int, int, int, string = "");`

### phonon_binaural_node

Construction:

- `phonon_binaural_node@ phonon_binaural_node(audio_engine@ engine, int channels, int sample_rate, int frame_size = 0);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_direction(float x, float y, float z, float distance);`
- `void set_direction(const vector&in direction, float distance);`
- `void set_spatial_blend_max_distance(float max_distance);`

### physics_body

Methods:

- `physics_entity get_entity() const property;`
- `bool get_is_active() const property;`
- `void set_is_active(bool is_active) property;`
- `const physics_transform& get_transform() const property;`
- `void set_transform(const physics_transform&in transform) property;`
- `physics_collider@ add_collider(physics_collision_shape@ shape, const physics_transform&in transform);`
- `void remove_collider(physics_collider&in collider);`
- `bool test_point_inside(const vector&in point) const;`
- `bool raycast(const ray&inout point, raycast_info&inout raycast_info) const;`
- `bool test_aabb_overlap(const aabb&in world_aabb) const;`
- `aabb get_aabb() const property;`
- `const physics_collider& get_collider(uint index) const;`
- `physics_collider& get_collider(uint index);`
- `uint get_nb_colliders() const property;`
- `vector get_world_point(const vector&in local_point) const;`
- `vector get_world_vector(const vector&in local_vector) const;`
- `vector get_local_point(const vector&in world_point) const;`
- `vector get_local_vector(const vector&in world_vector) const;`
- `bool get_is_debug_enabled() const property;`
- `void set_debug_enabled(bool enabled) property;`
- `void set_user_data(any@ userData);`
- `any@ get_user_data() const;`

### physics_box_shape

Construction:

- `physics_box_shape@ physics_box_shape(const vector&in half_extents);`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `float get_margin() const property;`
- `uint get_nb_faces() const property;`
- `const physics_half_edge_structure_face& get_face(uint face_index);`
- `uint get_nb_vertices() const property;`
- `const physics_half_edge_structure_vertex& get_vertex(uint vertex_index);`
- `const vector get_vertex_position(uint vertex_index);`
- `const vector get_face_normal(uint vertex_index);`
- `uint get_nb_half_edges() const property;`
- `const physics_half_edge_structure_edge& get_half_edge(uint edge_index) const;`
- `vector get_centroid() const property;`
- `uint find_most_anti_parallel_face(const vector&in direction) const;`
- `vector& get_half_extents() const property;`
- `void set_half_extents(const vector&in half_extents) property;`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_capsule_shape

Construction:

- `physics_capsule_shape@ physics_capsule_shape(float radius, float height);`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `float get_margin() const property;`
- `float get_radius() const property;`
- `void set_radius(float radius) property;`
- `float get_height() const property;`
- `void set_height(float height) property;`
- `string opImplConv();`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_collider

Methods:

- `physics_entity get_entity() const property;`
- `physics_collision_shape@ get_collision_shape() property;`
- `const physics_collision_shape@ get_collision_shape() const property;`
- `physics_body@ get_body() const property;`
- `const physics_transform& get_local_to_body_transform() const property;`
- `void set_local_to_body_transform(const physics_transform&in transform) property;`
- `const physics_transform get_local_to_world_transform() const;`
- `const aabb get_world_aabb() const property;`
- `bool test_aabb_overlap(const aabb&in world_aabb) const;`
- `bool test_point_inside(const vector&in world_point);`
- `bool raycast(const ray&in ray, raycast_info&inout raycast_info);`
- `uint16 get_collide_with_mask() const property;`
- `void set_collide_with_mask(uint16 bits) property;`
- `uint16 get_collision_category() const property;`
- `void set_collision_category(uint16 bits) property;`
- `uint16 get_broad_phase_id() const property;`
- `physics_material& get_material() property;`
- `void set_material(const physics_material&in material) property;`
- `bool get_is_trigger() const property;`
- `void set_is_trigger(bool is_trigger) property;`
- `bool get_is_simulation_collider() const property;`
- `void set_is_simulation_collider(bool is_simulation_collider) property;`
- `bool get_is_world_query_collider() const property;`
- `void set_is_world_query_collider(bool is_world_query_collider) property;`

### physics_collision_callback_data

Methods:

- `uint get_nb_contact_pairs() const property;`
- `physics_contact_pair get_contact_pair(uint64 index) const;`

### physics_collision_shape

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`

### physics_concave_mesh_shape

Construction:

- `physics_concave_mesh_shape@ physics_concave_mesh_shape(physics_triangle_mesh@ triangle_mesh, const vector&in scaling = vector(1,1,1));`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `physics_triangle_raycast_side get_raycast_test_type() const property;`
- `void set_raycast_test_type(physics_triangle_raycast_side side) property;`
- `vector get_scale() const property;`
- `void set_scale(const vector&in scale) property;`
- `uint get_nb_vertices() const property;`
- `uint get_nb_triangles() const property;`
- `void get_triangle_vertices_indices(uint triangle_index, uint&out v1_index, uint&out v2_index, uint&out v3_index) const;`
- `void get_triangle_vertices(uint triangle_index, vector&out v1, vector&out v2, vector&out v3) const;`
- `void get_triangle_vertices_normals(uint triangle_index, vector&out n1, vector&out n2, vector&out n3) const;`
- `vector get_vertex(uint vertex_index) const;`
- `const vector& get_vertex_normal(uint vertex_index) const;`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_contact_pair

Methods:

- `uint get_nb_contact_points() const property;`
- `physics_contact_point@ get_contact_point(uint index) const;`
- `physics_body@ get_body1() const property;`
- `physics_body@ get_body2() const property;`
- `physics_collider@ get_collider1() const property;`
- `physics_collider@ get_collider2() const property;`
- `physics_contact_event_type get_event_type() const property;`

### physics_contact_point

Methods:

- `const vector& get_world_normal() const property;`
- `const vector& get_local_point_on_collider1() const property;`
- `const vector& get_local_point_on_collider2() const property;`

### physics_convex_mesh

Methods:

- `uint get_nb_vertices() const property;`
- `const vector& get_vertex(uint index) const;`
- `uint get_nb_faces() const property;`
- `const vector& get_face_normal(uint index) const;`
- `const physics_half_edge_structure& get_half_edge_structure() const property;`
- `const vector& get_centroid() const property;`
- `const aabb& get_bounds() const property;`
- `float get_volume() const property;`
- `vector get_local_inertia_tensor(float mass, vector scale) const;`

### physics_convex_mesh_shape

Construction:

- `physics_convex_mesh_shape@ physics_convex_mesh_shape(physics_convex_mesh@ convex_mesh, const vector&in scaling = vector(1,1,1));`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `float get_margin() const property;`
- `uint get_nb_faces() const property;`
- `const physics_half_edge_structure_face& get_face(uint face_index);`
- `uint get_nb_vertices() const property;`
- `const physics_half_edge_structure_vertex& get_vertex(uint vertex_index);`
- `const vector get_vertex_position(uint vertex_index);`
- `const vector get_face_normal(uint vertex_index);`
- `uint get_nb_half_edges() const property;`
- `const physics_half_edge_structure_edge& get_half_edge(uint edge_index) const;`
- `vector get_centroid() const property;`
- `uint find_most_anti_parallel_face(const vector&in direction) const;`
- `vector& get_scale() const property;`
- `void set_scale(vector&inout scale) const property;`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_default_logger

Methods:

- `void add_file_destination(const string&in filePath, uint logLevelFlag, physics_logger_format format);`
- `void remove_all_destinations();`
- `physics_logger@ opImplCast();`
- `const physics_logger@ opImplCast() const;`

### physics_entity

Properties:

- `uint id;`

Methods:

- `uint get_index() const property;`
- `uint get_generation() const property;`
- `bool opEquals(const physics_entity&in entity) const;`

### physics_half_edge_structure

Methods:

- `void compute_half_edges();`
- `uint add_vertex(uint vertex_point_index);`
- `uint get_nb_faces() const property;`
- `uint get_nb_half_edges() const property;`
- `uint get_nb_vertices() const property;`
- `const physics_half_edge_structure_face& get_face(uint index) const property;`

### physics_half_edge_structure_edge

Properties:

- `uint vertex_index;`
- `uint twin_edge_index;`
- `uint face_index;`
- `uint next_edge_index;`

### physics_half_edge_structure_face

Methods:

- `void set_face_vertices(array<uint>@ face_vertices);`
- `array<uint>@ get_face_vertices() const;`

### physics_half_edge_structure_vertex

Properties:

- `uint vertex_point_index;`
- `uint vertex_edge_index;`

### physics_height_field

Methods:

- `uint get_nb_rows() const property;`
- `uint get_nb_columns() const property;`
- `float get_min_height() const property;`
- `float get_max_height() const property;`
- `float get_integer_height_scale() const property;`
- `vector get_vertex_at(uint x, uint y) const;`
- `float get_height_at(uint x, uint y) const;`
- `physics_height_data_type get_height_data_type() const property;`
- `aabb& get_bounds() const property;`
- `string opImplConv() const;`

### physics_height_field_shape

Construction:

- `physics_height_field_shape@ physics_height_field_shape(physics_height_field@ height_field, const vector&in scaling = vector(1,1,1));`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `physics_triangle_raycast_side get_raycast_test_type() const property;`
- `void set_raycast_test_type(physics_triangle_raycast_side side) property;`
- `vector get_scale() const property;`
- `void set_scale(const vector&in scale) property;`
- `physics_height_field@ get_height_field() const property;`
- `vector get_vertex_at(uint x, uint y) const;`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_joint

Methods:

- `physics_rigid_body@ get_body1() const property;`
- `physics_rigid_body@ get_body2() const property;`
- `physics_joint_type get_type() const property;`
- `vector get_reaction_force(float time_step) const;`
- `vector get_reaction_torque(float time_step) const;`
- `bool get_is_collision_enabled() const property;`
- `physics_entity get_entity() const property;`
- `string opImplConv();`

### physics_joint_info

Properties:

- `physics_rigid_body@ body1;`
- `physics_rigid_body@ body2;`
- `physics_joint_type type;`
- `physics_joints_position_correction_technique position_correction_technique;`
- `bool isCollisionEnabled;`

### physics_logger

Methods:

- `void log(physics_logger_level level, const string&in worldName, physics_logger_category category, const string&in message);`

### physics_material

Methods:

- `float get_bounciness() const property;`
- `void set_bounciness(float bounciness) property;`
- `float get_friction_coefficient() const property;`
- `void set_friction_coefficient(float friction_coefficient) property;`
- `float get_friction_coefficient_sqrt() const property;`
- `float get_mass_density() const property;`
- `void set_mass_density(float mass_density) property;`
- `string opImplConv();`

### physics_message

Properties:

- `string text;`
- `physics_message_type type;`

### physics_overlap_callback_data

Methods:

- `uint get_nb_overlap_pairs() const property;`
- `physics_overlap_pair get_overlapping_pair(uint index) const;`

### physics_overlap_pair

Methods:

- `physics_body& get_body1() const property;`
- `physics_body& get_body2() const property;`
- `physics_collider& get_collider1() const property;`
- `physics_collider& get_collider2() const property;`
- `physics_overlap_event_type get_event_type() const property;`

### physics_polygon_data

Construction:

- `physics_polygon_data@ physics_polygon_data(array<float>@ vertices, array<array<uint>>@ faces);`

### physics_polygon_face

Properties:

- `uint nb_vertices;`
- `uint index_base;`

### physics_polygon_vertex_array

Methods:

- `physics_polygon_vertex_data_type get_vertex_data_type() const property;`
- `physics_polygon_index_data_type get_index_data_type() const property;`
- `uint get_nb_vertices() const property;`
- `uint get_nb_faces() const property;`
- `uint get_vertices_stride() const property;`
- `uint get_indices_stride() const property;`
- `uint get_vertex_index_in_face(uint face_index, uint vertex_in_face) const;`
- `vector get_vertex(uint vertex_index) const;`
- `physics_polygon_face@ get_polygon_face(uint face_index) const;`

### physics_rigid_body

Methods:

- `physics_entity get_entity() const property;`
- `bool get_is_active() const property;`
- `void set_is_active(bool is_active) property;`
- `const physics_transform& get_transform() const property;`
- `void set_transform(const physics_transform&in transform) property;`
- `physics_collider@ add_collider(physics_collision_shape@ shape, const physics_transform&in transform);`
- `void remove_collider(physics_collider&in collider);`
- `bool test_point_inside(const vector&in point) const;`
- `bool raycast(const ray&inout point, raycast_info&inout raycast_info) const;`
- `bool test_aabb_overlap(const aabb&in world_aabb) const;`
- `aabb get_aabb() const property;`
- `const physics_collider& get_collider(uint index) const;`
- `physics_collider& get_collider(uint index);`
- `uint get_nb_colliders() const property;`
- `vector get_world_point(const vector&in local_point) const;`
- `vector get_world_vector(const vector&in local_vector) const;`
- `vector get_local_point(const vector&in world_point) const;`
- `vector get_local_vector(const vector&in world_vector) const;`
- `bool get_is_debug_enabled() const property;`
- `void set_debug_enabled(bool enabled) property;`
- `void set_user_data(any@ userData);`
- `any@ get_user_data() const;`
- `float get_mass() const property;`
- `void set_mass(float mass) property;`
- `vector get_linear_velocity() const property;`
- `void set_linear_velocity(const vector&in linear_velocity) property;`
- `vector get_angular_velocity() const property;`
- `void set_angular_velocity(const vector&in angular_velocity) property;`
- `const vector& get_local_inertia_tensor() const property;`
- `void set_local_inertia_tensor(const vector&in local_inertia_tensor) property;`
- `const vector& get_local_center_of_mass() const property;`
- `void set_local_center_of_mass(const vector&in local_center_of_mass) property;`
- `void update_local_center_of_mass_from_colliders();`
- `void update_local_inertia_tensor_from_colliders();`
- `void update_mass_from_colliders();`
- `void update_mass_properties_from_colliders();`
- `physics_body_type get_type() const property;`
- `void set_type(physics_body_type type) property;`
- `bool get_is_gravity_enabled() const property;`
- `void set_is_gravity_enabled(bool enabled) property;`
- `void set_is_sleeping(bool enabled);`
- `float get_linear_damping() const property;`
- `void set_linear_damping(float linear_damping) property;`
- `float get_angular_damping() const property;`
- `void set_angular_damping(float angular_damping) property;`
- `const vector& get_linear_lock_axis_factor() const property;`
- `void set_linear_lock_axis_factor(const vector&in linear_lock_axis_factor) property;`
- `const vector& get_angular_lock_axis_factor() const property;`
- `void set_angular_lock_axis_factor(const vector&in angular_lock_axis_factor) property;`
- `physics_body@ opImplCast();`
- `const physics_body@ opImplCast() const;`

### physics_sphere_shape

Construction:

- `physics_sphere_shape@ physics_sphere_shape(float radius);`

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `float get_margin() const property;`
- `float get_radius() const property;`
- `void set_radius(float radius) property;`
- `string opImplConv();`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_transform

Methods:

- `const vector& get_position() const property;`
- `const quaternion& get_orientation() const property;`
- `void set_position(const vector&in position) property;`
- `void set_orientation(const quaternion&in orientation) property;`
- `void set_to_identity();`
- `physics_transform get_inverse() const property;`
- `bool get_is_valid() const property;`
- `void set_from_opengl_matrix(array<float>@ matrix);`
- `array<float>@ get_opengl_matrix() const;`
- `bool opEquals(const physics_transform&in) const;`
- `physics_transform opMul(const physics_transform&in) const;`
- `vector opMul(const vector&in) const;`
- `string opImplConv();`

### physics_triangle_data

Construction:

- `physics_triangle_data@ physics_triangle_data(array<float>@ vertices, array<uint>@ indices);`
- `physics_triangle_data@ physics_triangle_data(array<float>@ vertices, array<float>@ normals, array<uint>@ indices);`

### physics_triangle_mesh

Methods:

- `uint get_nb_vertices() const property;`
- `uint get_nb_triangles() const property;`
- `const aabb& get_bounds() const property;`
- `void get_triangle_vertices_indices(uint triangle_index, uint&out v1_index, uint&out v2_index, uint&out v3_index) const;`
- `void get_triangle_vertices(uint triangle_index, vector&out v1, vector&out v2, vector&out v3) const;`
- `void get_triangle_vertices_normals(uint triangle_index, vector&out n1, vector&out n2, vector&out n3) const;`
- `const vector& get_vertex(uint vertex_index) const;`
- `const vector& get_vertex_normal(uint vertex_index) const;`

### physics_triangle_shape

Methods:

- `physics_shape_name get_name() const property;`
- `physics_shape_type get_type() const property;`
- `bool get_is_convex() const property;`
- `bool get_is_polyhedron() const property;`
- `aabb get_local_bounds() const;`
- `int get_id() const property;`
- `vector get_local_inertia_tensor(float mass) const;`
- `float get_volume() const property;`
- `aabb compute_transformed_aabb(const physics_transform&in transform) const;`
- `string opImplConv() const;`
- `float get_margin() const property;`
- `uint get_nb_faces() const property;`
- `const physics_half_edge_structure_face& get_face(uint face_index);`
- `uint get_nb_vertices() const property;`
- `const physics_half_edge_structure_vertex& get_vertex(uint vertex_index);`
- `const vector get_vertex_position(uint vertex_index);`
- `const vector get_face_normal(uint vertex_index);`
- `uint get_nb_half_edges() const property;`
- `const physics_half_edge_structure_edge& get_half_edge(uint edge_index) const;`
- `vector get_centroid() const property;`
- `uint find_most_anti_parallel_face(const vector&in direction) const;`
- `physics_triangle_raycast_side get_raycast_test_type() const property;`
- `void set_raycast_test_type(physics_triangle_raycast_side test_type) property;`
- `physics_collision_shape@ opImplCast();`
- `const physics_collision_shape@ opImplCast() const;`

### physics_triangle_vertex_array

Methods:

- `physics_triangle_vertex_data_type get_vertex_data_type() const property;`
- `physics_triangle_normal_data_type get_vertex_normal_data_type() const property;`
- `bool get_has_normals() const property;`
- `physics_triangle_index_data_type get_index_data_type() const property;`
- `uint get_nb_vertices() const property;`
- `uint get_nb_triangles() const property;`
- `uint get_vertices_stride() const property;`
- `uint get_vertices_normals_stride() const property;`
- `uint get_indices_stride() const property;`
- `void get_triangle_vertices_indices(uint triangle_index, uint&out v1_index, uint&out v2_index, uint&out v3_index) const;`
- `vector get_vertex(uint vertex_index) const;`
- `vector get_vertex_normal(uint vertex_index) const;`

### physics_vertex_array

Methods:

- `physics_vertex_data_type get_data_type() const property;`
- `uint get_nb_vertices() const property;`
- `uint get_stride() const property;`
- `vector get_vertex(uint index) const;`

### physics_vertex_data

Construction:

- `physics_vertex_data@ physics_vertex_data(array<float>@ vertices);`

### physics_world

Construction:

- `physics_world@ physics_world(const physics_world_settings&in world_settings);`

Methods:

- `bool test_overlap(physics_body@ body1, physics_body@ body2);`
- `void raycast(const ray&in ray, physics_raycast_callback@ callback, uint16 category_mask = 0xffff);`
- `void test_overlap(physics_body@ body, physics_overlap_callback@ callback);`
- `void test_overlap(physics_overlap_callback@ callback);`
- `void test_collision(physics_body@ body1, physics_body@ body2, physics_collision_callback@ callback);`
- `void test_collision(physics_body@ body, physics_collision_callback@ callback);`
- `void test_collision(physics_collision_callback@ callback);`
- `aabb get_world_aabb(const physics_collider@ collider) const;`
- `const string& get_name() const property;`
- `void update(float time_step);`
- `uint16 get_nb_iterations_velocity_solver() const property;`
- `void set_nb_iterations_velocity_solver(uint16 iterations) property;`
- `uint16 get_nb_iterations_position_solver() const property;`
- `void set_nb_iterations_position_solver(uint16 iterations) property;`
- `void set_contacts_position_correction_technique(physics_contact_position_correction_technique technique) property;`
- `physics_rigid_body@ create_rigid_body(const physics_transform&in transform);`
- `void destroy_rigid_body(physics_rigid_body&inout body);`
- `physics_joint@ create_joint(const physics_joint_info&in joint_info);`
- `void destroy_joint(physics_joint&inout joint);`
- `vector get_gravity() const property;`
- `void set_gravity(const vector&in gravity) property;`
- `bool get_is_gravity_enabled() const property;`
- `void set_is_gravity_enabled(bool enabled) property;`
- `bool get_is_sleeping_enabled() const property;`
- `void set_is_sleeping_enabled(bool enabled) property;`
- `float get_sleep_linear_velocity() const property;`
- `void set_sleep_linear_velocity(float sleep_linear_velocity) property;`
- `float get_sleep_angular_velocity() const property;`
- `void set_sleep_angular_velocity(float sleep_angular_velocity) property;`
- `float get_time_before_sleep() const property;`
- `void set_time_before_sleep(float time_before_sleep) property;`
- `void set_callbacks(physics_collision_callback@ collision_callback, physics_overlap_callback@ trigger_callback);`
- `uint get_nb_rigid_bodies() const property;`
- `const physics_rigid_body& get_rigid_body(uint index) const;`
- `physics_rigid_body& get_rigid_body(uint index);`

### physics_world_settings

Properties:

- `string world_name;`
- `vector gravity;`
- `float persistent_contact_distance_threshold;`
- `float default_friction_coefficient;`
- `float default_bounciness;`
- `float restitution_velocity_threshold;`
- `bool is_sleeping_enabled;`
- `uint16 default_velocity_solver_iterations_count;`
- `uint16 default_position_solver_iterations_count;`
- `float default_time_before_sleep;`
- `float default_sleep_linear_velocity;`
- `float default_sleep_angular_velocity;`
- `float cos_angle_similar_contact_manifold;`

Methods:

- `physics_world_settings& opAssign(const physics_world_settings&in);`
- `string opImplConv();`

### quaternion

Properties:

- `float x;`
- `float y;`
- `float z;`
- `float w;`

Methods:

- `quaternion opAdd(const quaternion&in);`
- `quaternion& opAddAssign(const quaternion&in);`
- `quaternion opSub(const quaternion&in);`
- `quaternion& opSubAssign(const quaternion&in);`
- `quaternion opMul(const quaternion&in);`
- `quaternion opMul(float) const;`
- `bool opEquals(const quaternion&in) const;`
- `void set(float x, float y, float z, float w);`
- `void set_to_zero();`
- `void set_to_identity();`
- `float length() const;`
- `float length_square() const;`
- `bool get_is_unit() const property;`
- `bool get_is_valid() const property;`
- `bool get_is_finite() const property;`
- `float dot(const quaternion&in) const;`
- `void normalize();`
- `void inverse();`
- `vector get_v() const property;`
- `quaternion get_unit() const property;`
- `quaternion get_conjugate() const property;`
- `quaternion get_inversed() const property;`
- `void get_rotation_angle_axis(float&out angle, vector&out axis) const;`
- `matrix3x3 get_matrix() const property;`
- `string opImplConv() const;`

### random_gamerand

Construction:

- `random_gamerand@ random_gamerand();`
- `random_gamerand@ random_gamerand(uint seed);`

Methods:

- `uint next();`
- `float nextf();`
- `int range(int min, int max);`
- `void seed(uint s);`
- `string get_state() const;`
- `bool set_state(const string&in state);`
- `bool next_bool(int percent = 50);`
- `string next_character(const string&in min, const string&in max);`
- `random_interface@ opImplCast();`

### random_interface

Methods:

- `uint next();`
- `float nextf();`
- `int range(int min, int max);`
- `void seed(uint s);`
- `void seed64(uint64 s);`
- `string get_state() const;`
- `bool set_state(const string&in state);`
- `bool next_bool(int percent = 50);`
- `string next_character(const string&in min, const string&in max);`

### random_pcg

Construction:

- `random_pcg@ random_pcg();`
- `random_pcg@ random_pcg(uint seed);`

Methods:

- `uint next();`
- `float nextf();`
- `int range(int min, int max);`
- `void seed(uint s);`
- `string get_state() const;`
- `bool set_state(const string&in state);`
- `bool next_bool(int percent = 50);`
- `string next_character(const string&in min, const string&in max);`
- `random_interface@ opImplCast();`

### random_reader

Construction:

- `random_reader@ random_reader(const string&in encoding = "", int byteorder = 1);`

Properties:

- `bool binary;`
- `bool sync_rw_cursors;`

Methods:

- `bool open(const string&in encoding = "", int byteorder = 1);`
- `datastream@ opImplCast();`
- `bool close(bool = false);`
- `bool close_all();`
- `bool get_active() const property;`
- `uint64 get_available() const property;`
- `bool seek(uint64);`
- `bool seek_end(uint64 = 0);`
- `bool seek_relative(int64);`
- `int64 get_pos() const property;`
- `bool rseek(uint64);`
- `bool rseek_end(uint64 = 0);`
- `bool rseek_relative(int64);`
- `int64 get_rpos() const property;`
- `bool wseek(uint64);`
- `bool wseek_end(uint64 = 0);`
- `bool wseek_relative(int64);`
- `int64 get_wpos() const property;`
- `string read(uint = 0);`
- `string read_line();`
- `string read_until(const string&in text, bool require_full);`
- `uint64 read_7bit_encoded();`
- `void read_7bit_encoded(uint64&out integer);`
- `void write_7bit_encoded(uint64 integer);`
- `uint write(const string&in);`
- `random_reader& opShr(int8&out);`
- `int8 read_int8();`
- `random_reader& opShl(int8);`
- `random_reader& write_int8(int8);`
- `random_reader& opShr(uint8&out);`
- `uint8 read_uint8();`
- `random_reader& opShl(uint8);`
- `random_reader& write_uint8(uint8);`
- `random_reader& opShr(int16&out);`
- `int16 read_int16();`
- `random_reader& opShl(int16);`
- `random_reader& write_int16(int16);`
- `random_reader& opShr(uint16&out);`
- `uint16 read_uint16();`
- `random_reader& opShl(uint16);`
- `random_reader& write_uint16(uint16);`
- `random_reader& opShr(int&out);`
- `int read_int();`
- `random_reader& opShl(int);`
- `random_reader& write_int(int);`
- `random_reader& opShr(uint&out);`
- `uint read_uint();`
- `random_reader& opShl(uint);`
- `random_reader& write_uint(uint);`
- `random_reader& opShr(int64&out);`
- `int64 read_int64();`
- `random_reader& opShl(int64);`
- `random_reader& write_int64(int64);`
- `random_reader& opShr(uint64&out);`
- `uint64 read_uint64();`
- `random_reader& opShl(uint64);`
- `random_reader& write_uint64(uint64);`
- `random_reader& opShr(float&out);`
- `float read_float();`
- `random_reader& opShl(float);`
- `random_reader& write_float(float);`
- `random_reader& opShr(double&out);`
- `double read_double();`
- `random_reader& opShl(double);`
- `random_reader& write_double(double);`
- `random_reader& opShr(string&out);`
- `string read_string();`
- `random_reader& opShl(string);`
- `random_reader& write_string(string);`
- `bool get_good() const property;`
- `bool get_bad() const property;`
- `bool get_fail() const property;`
- `bool get_eof() const property;`

### random_well

Construction:

- `random_well@ random_well();`
- `random_well@ random_well(uint seed);`

Methods:

- `uint next();`
- `float nextf();`
- `int range(int min, int max);`
- `void seed(uint s);`
- `string get_state() const;`
- `bool set_state(const string&in state);`
- `bool next_bool(int percent = 50);`
- `string next_character(const string&in min, const string&in max);`
- `random_interface@ opImplCast();`

### random_xorshift

Construction:

- `random_xorshift@ random_xorshift();`
- `random_xorshift@ random_xorshift(uint seed);`
- `random_xorshift@ random_xorshift(uint64 seed);`

Methods:

- `uint next();`
- `float nextf();`
- `int range(int min, int max);`
- `void seed(uint s);`
- `void seed64(uint64 s);`
- `string get_state() const;`
- `bool set_state(const string&in state);`
- `bool next_bool(int percent = 50);`
- `string next_character(const string&in min, const string&in max);`
- `random_interface@ opImplCast();`

### ray

Properties:

- `vector point1;`
- `vector point2;`
- `float max_fraction;`

### raycast_info

Properties:

- `vector world_point;`
- `vector world_normal;`
- `float hit_fraction;`
- `int triangle_index;`
- `physics_body@ body;`
- `physics_collider@ collider;`

### ref

Methods:

- `void opCast(?&out);`
- `ref& opHndlAssign(const ref&in);`
- `ref& opHndlAssign(const ?&in);`
- `bool opEquals(const ref&in) const;`
- `bool opEquals(const ?&in) const;`

### refstring

Construction:

- `refstring@ refstring();`

Properties:

- `string str;`

### regexp

Methods:

- `bool match(const string&in, uint64 = 0) const;`
- `bool match(const string&in, uint64, int) const;`
- `bool opEquals(const string&in) const;`
- `string extract(const string&in, uint64 = 0) const;`
- `string extract(const string&in, uint64, int) const;`
- `int subst(string&inout, uint64, const string&in, int = RE_UTF8) const;`
- `int subst(string&inout, const string&in, int = RE_UTF8) const;`
- `array<string>@ split(const string&in, uint64 = 0) const;`
- `array<string>@ split(const string&in, uint64, int) const;`

### reverb3d

Construction:

- `reverb3d@ reverb3d(audio_node@ reverb, mixer@ destination = mixer(), audio_engine@ engine = sound_default_engine);`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `void set_reverb(audio_node@ reverb) property;`
- `audio_node@ get_reverb() const property;`
- `void set_mixer(mixer@ mix) property;`
- `mixer@ get_mixer() const property;`
- `void set_min_volume(float min_volume) property;`
- `float get_min_volume() const property;`
- `void set_max_volume(float max_volume) property;`
- `float get_max_volume() const property;`
- `void set_max_volume_distance(float distance) property;`
- `float get_max_volume_distance() const property;`
- `void set_max_audible_distance(float distance) property;`
- `float get_max_audible_distance() const property;`
- `void set_volume_curve(float volume_curve) property;`
- `float get_volume_curve() const property;`
- `float get_volume_at(float distance) const;`
- `audio_splitter_node@ create_attachment(audio_node@ dry_input = null, audio_node@ dry_output = null);`

### rw_lock

Construction:

- `rw_lock@ rw_lock();`

Methods:

- `void read_lock();`
- `bool try_read_lock();`
- `void write_lock();`
- `bool try_write_lock();`
- `void unlock();`

### rw_read_lock

### rw_scoped_lock

### rw_write_lock

### script_function

Methods:

- `dictionary@ call(dictionary@ args, array<string>@ errors = null, int max_statement_count = 0);`
- `dictionary@ opCall(dictionary@ args, array<string>@ errors = null, int max_statement_count = 0);`
- `bool retrieve(?&out);`
- `string get_declared_at(int&inout row, int&inout column);`
- `string get_decl(bool include_object_name, bool include_namespace = true, bool include_param_names = true);`
- `string get_decl() property;`
- `string get_name() property;`
- `string get_namespace() property;`
- `string get_script() property;`
- `int get_line() property;`
- `bool get_is_explicit() property;`
- `bool get_is_final() property;`
- `bool get_is_override() property;`
- `bool get_is_private() property;`
- `bool get_is_property() property;`
- `bool get_is_protected() property;`
- `bool get_is_read_only() property;`
- `bool get_is_shared() property;`

### script_module

Properties:

- `uint max_statement_count;`

Methods:

- `int add_section(const string&in, const string&in, uint = 0);`
- `int build(array<string>@ = null);`
- `string get_bytecode(bool);`
- `int set_bytecode(const string&in, bool&out, array<string>@ = null);`
- `int reset_globals(array<string>@ = null);`
- `int bind_all_imported_functions();`
- `int bind_imported_function(uint, script_function@);`
- `int compile_global(const string&in, const string&in, uint = 0);`
- `script_function@ compile_function(const string&in, const string&in, array<string>@ = null, bool = false, uint = 0);`
- `void discard();`
- `script_function@ get_function_by_decl(const string&in);`
- `script_function@ get_function_by_index(uint);`
- `script_function@ get_function_by_name(const string&in);`
- `any@ get_global(uint);`
- `const string get_global_decl(uint);`
- `int get_global_index_by_decl(const string&in);`
- `int get_global_index_by_name(const string&in);`
- `const string get_global_name(uint);`
- `uint get_function_count();`
- `uint get_global_count();`
- `uint get_imported_function_count();`
- `uint set_access_mask(uint);`
- `const string get_imported_function_decl(uint);`
- `int get_imported_function_index(const string&in);`
- `const string get_imported_function_module(uint);`
- `string get_name() property;`
- `void set_name(const string&in) property;`

### smtp_client

Construction:

- `smtp_client@ smtp_client();`

Methods:

- `void set_host(const string&in) property;`
- `string get_host() const property;`
- `void set_port(int) property;`
- `int get_port() const property;`
- `void set_use_ssl(bool) property;`
- `bool get_use_ssl() const property;`
- `bool connect();`
- `bool login(const string&in, const string&in, smtp_auth_method = SMTP_AUTH_LOGIN);`
- `bool login_oauth2(const string&in, const string&in);`
- `bool send_message(mail_message@);`
- `void close();`
- `bool get_is_connected() const property;`
- `bool get_is_authenticated() const property;`
- `string get_last_error() const property;`
- `void set_timeout(int) property;`
- `int get_timeout() const property;`
- `string query_server_capabilities();`
- `string get_server_capabilities() const property;`
- `bool send_messages(array<mail_message@>@);`

### socket

Construction:

- `socket@ socket();`
- `socket@ socket(const socket&in sock);`

Methods:

- `socket& opAssign(const socket&in socket);`
- `int opCmp(const socket&in);`
- `socket_type get_type() const property;`
- `bool get_is_null() const property;`
- `bool get_is_stream() const property;`
- `bool get_is_datagram() const property;`
- `bool get_is_raw() const property;`
- `void close();`
- `bool poll(const timespan&inout timeout, int mode) const;`
- `int get_available() const property;`
- `int get_error() const property;`
- `void set_send_buffer_size(int size) property;`
- `int get_send_buffer_size() const property;`
- `void set_receive_buffer_size(int size) property;`
- `int get_receive_buffer_size() const property;`
- `void set_send_timeout(const timespan&in timeout) property;`
- `timespan get_send_timeout() const property;`
- `void set_receive_timeout(const timespan&in timeout) property;`
- `timespan get_receive_timeout() const property;`
- `void set_option(int level, int option, int value);`
- `void set_option(int level, int option, uint value);`
- `void set_option(int level, int option, uint8 value);`
- `void set_option(int level, int option, const timespan&in value);`
- `void set_option(int level, int option, const spec::ip_address&in value);`
- `void get_option(int level, int option, int&out value) const;`
- `void get_option(int level, int option, uint&out value) const;`
- `void get_option(int level, int option, uint8&out value) const;`
- `void get_option(int level, int option, timespan&out value) const;`
- `void get_option(int level, int option, spec::ip_address&out value);`
- `void set_linger(bool on, int seconds);`
- `void get_linger(bool&out on, int&out seconds);`
- `void set_no_delay(bool flag) property;`
- `bool get_no_delay() const property;`
- `void set_keep_alive(bool flag) property;`
- `bool get_keep_alive() const property;`
- `void set_reuse_address(bool flag) property;`
- `bool get_reuse_address() const property;`
- `void set_reuse_port(bool flag) property;`
- `bool get_reuse_port() const property;`
- `void set_oob_inline(bool flag) property;`
- `bool get_oob_inline() const property;`
- `void set_blocking(bool flag) property;`
- `bool get_blocking() const property;`
- `socket_address get_address() const property;`
- `socket_address get_peer_address() const property;`
- `bool get_secure() const property;`
- `void init(int af);`

### socket_address

Methods:

- `socket_address& opAssign(const socket_address&in addr);`
- `spec::ip_address get_host() const property;`
- `uint16 get_port() const property;`
- `string opImplConv() const;`
- `spec::ip_address_family get_family() const property;`
- `int opCmp(const socket_address&in);`

### sound

Construction:

- `sound@ sound();`

Methods:

- `uint get_input_bus_count() const property;`
- `uint get_output_bus_count() const property;`
- `uint get_input_channels(uint bus) const;`
- `uint get_output_channels(uint bus) const;`
- `bool attach_output_bus(uint output_bus, audio_node@ destination, uint destination_input_bus);`
- `bool detach_output_bus(uint bus);`
- `bool detach_all_output_buses();`
- `bool set_output_bus_volume(uint bus, float volume);`
- `float get_output_bus_volume(uint bus);`
- `bool set_state(audio_node_state state);`
- `audio_node_state get_state();`
- `bool set_state_time(audio_node_state state, uint64 time);`
- `uint64 get_state_time(uint64 global_time);`
- `audio_node_state get_state_by_time(uint64 global_time);`
- `audio_node_state get_state_by_time_range(uint64 global_time_begin, uint64 global_time_end);`
- `uint64 get_time() const;`
- `bool set_time(uint64 local_time);`
- `audio_node@ opImplCast();`
- `audio_engine@ get_engine() const property;`
- `bool set_mixer(mixer@ parent_mixer);`
- `mixer@ get_mixer() const;`
- `void set_3d_panner(int panner_id);`
- `int get_3d_panner() const property;`
- `void set_3d_attenuator(int attenuator_id);`
- `int get_3d_attenuator() const property;`
- `int get_preferred_3d_panner() const property;`
- `int get_preferred_3d_attenuator() const property;`
- `void set_hrtf(bool enabled) property;`
- `bool get_hrtf() const property;`
- `bool set_shape(ref shape);`
- `ref get_shape() const property;`
- `void set_reverb3d(reverb3d@ reverb) property;`
- `void set_reverb3d_at(reverb3d@ reverb, reverb3d_placement placement);`
- `reverb3d@ get_reverb3d() const property;`
- `audio_splitter_node@ get_reverb3d_attachment() const property;`
- `reverb3d_placement get_reverb3d_placement() const property;`
- `audio_node_chain@ get_effects_chain() property;`
- `audio_node_chain@ get_internal_node_chain() property;`
- `bool play(bool reset_loop_state = true);`
- `bool play_looped();`
- `bool stop();`
- `void set_volume(float volume) property;`
- `float get_volume() const property;`
- `void set_pan(float pan) property;`
- `float get_pan() const property;`
- `void set_pan_mode(audio_pan_mode mode) property;`
- `audio_pan_mode get_pan_mode() const property;`
- `void set_pitch(float pitch) property;`
- `float get_pitch() const property;`
- `void set_spatialization_enabled(bool enabled) property;`
- `bool get_spatialization_enabled() const property;`
- `void set_pinned_listener(uint index) property;`
- `uint get_pinned_listener() const property;`
- `uint get_listener() const property;`
- `vector get_direction_to_listener() const;`
- `float get_distance_to_listener() const;`
- `void set_position_3d(float x, float y, float z);`
- `void set_position_3d(const vector&in position);`
- `vector get_position_3d() const;`
- `void set_direction(float x, float y, float z);`
- `void set_direction(const vector&in direction);`
- `vector get_direction() const;`
- `void set_velocity(float x, float y, float z);`
- `void set_velocity(const vector&in velocity);`
- `vector get_velocity() const;`
- `void set_positioning(audio_positioning_mode mode) property;`
- `audio_positioning_mode get_positioning() const property;`
- `void set_rolloff(float rolloff) property;`
- `float get_rolloff() const property;`
- `void set_min_gain(float gain) property;`
- `float get_min_gain() const property;`
- `void set_max_gain(float gain) property;`
- `float get_max_gain() const property;`
- `void set_min_distance(float distance) property;`
- `float get_min_distance() const property;`
- `void set_max_distance(float distance) property;`
- `float get_max_distance() const property;`
- `void set_cone(float inner_radians, float outer_radians, float outer_gain);`
- `void get_cone(float&out inner_radians, float&out outer_radians, float&out outer_gain);`
- `void set_doppler_factor(float factor) property;`
- `float get_doppler_factor() const property;`
- `void set_directional_attenuation_factor(float factor) property;`
- `float get_directional_attenuation_factor() const property;`
- `void set_fade(float start_volume, float end_volume, uint64 length);`
- `void set_fade_in_frames(float start_volume, float end_volume, uint64 length_frames);`
- `void set_fade_in_milliseconds(float start_volume, float end_volume, uint64 length_ms);`
- `float get_current_fade_volume() const property;`
- `void set_start_time(uint64 absolute_time) property;`
- `void set_stop_time(uint64 absolute_time);`
- `bool get_playing() const property;`
- `bool load(const string&in filename, const pack_interface@ pack = sound_default_pack);`
- `bool stream(const string&in filename, const pack_interface@ pack = sound_default_pack);`
- `bool stream_url(const string&in url);`
- `bool load_memory(const string&in data);`
- `bool load_pcm(const array<float>@ data, int samplerate, int channels);`
- `bool load_pcm(const array<int>@ data, int samplerate, int channels);`
- `bool load_pcm(const array<int16>@ data, int samplerate, int channels);`
- `bool load_pcm(const array<uint8>@ data, int samplerate, int channels);`
- `bool load_pcm(const memory_buffer<float>&in data, int samplerate, int channels);`
- `bool load_pcm(const memory_buffer<int>&in data, int samplerate, int channels);`
- `bool load_pcm(const memory_buffer<int16>&in data, int samplerate, int channels);`
- `bool load_pcm(const memory_buffer<uint8>&in data, int samplerate, int channels);`
- `bool stream_pcm(const array<float>@ data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const array<int>@ data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const array<int16>@ data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const array<uint8>@ data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const memory_buffer<float>&in data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const memory_buffer<int>&in data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const memory_buffer<int16>&in data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool stream_pcm(const memory_buffer<uint8>&in data, uint sample_rate = 0, uint channels = 0, uint buffer_size = 0);`
- `bool open(audio_data_source@ datasource);`
- `bool close();`
- `void set_autoclose(bool enabled = true) property;`
- `bool get_autoclose() const property;`
- `const string& get_loaded_filename() const property;`
- `audio_data_source@ get_datasource() const property;`
- `bool get_load_complete() const property;`
- `bool get_active() const property;`
- `bool get_paused() const property;`
- `bool play_wait();`
- `bool pause();`
- `bool pause_fade(const uint64 length);`
- `bool pause_fade_in_frames(const uint64 length_in_frames);`
- `bool pause_fade_in_milliseconds(const uint64 length_in_milliseconds);`
- `void set_timed_fade(float start_volume, float end_volume, uint64 length, uint64 absolute_time);`
- `void set_timed_fade_in_frames(float start_volume, float end_volume, uint64 length, uint64 absolute_time);`
- `void set_timed_fade_in_milliseconds(float start_volume, float end_volume, uint64 length, uint64 absolute_time);`
- `void set_stop_time_with_fade(uint64 absolute_time, uint64 fade_length);`
- `void set_stop_time_with_fade_in_frames(uint64 absolute_time, uint64 fade_length);`
- `void set_stop_time_with_fade_in_milliseconds(uint64 absolute_time, uint64 fade_length);`
- `void set_looping(bool looping) property;`
- `bool get_looping() const property;`
- `bool get_at_end() const property;`
- `bool seek(const uint64 position);`
- `bool seek_in_frames(const uint64 position);`
- `bool seek_in_milliseconds(const uint64 position);`
- `uint64 get_position() property;`
- `uint64 get_position_in_frames() const property;`
- `uint64 get_position_in_milliseconnds() const property;`
- `uint64 get_length() property;`
- `uint64 get_length_in_frames() const property;`
- `uint64 get_length_in_ms() const property;`
- `bool get_data_format(audio_format&out format, uint&out channels, uint&out sample_rate);`
- `double get_pitch_lower_limit() const property;`

### sound_aabb_shape

Construction:

- `sound_aabb_shape@ sound_aabb_shape(int left_range, int right_range, int backward_range, int forward_range, int lower_range, int upper_range);`

Properties:

- `int left_range;`
- `int right_range;`
- `int backward_range;`
- `int forward_range;`
- `int lower_range;`
- `int upper_range;`

### spinlock_mutex

Construction:

- `spinlock_mutex@ spinlock_mutex(const string&in);`

Methods:

- `void lock();`
- `bool try_lock();`
- `void unlock();`

### spinlock_mutex_lock

Methods:

- `void unlock();`

### stream_socket

Construction:

- `stream_socket@ stream_socket();`
- `stream_socket@ stream_socket(const socket&in sock);`
- `stream_socket@ stream_socket(const stream_socket&in sock);`
- `stream_socket@ stream_socket(const socket_address&in address);`
- `stream_socket@ stream_socket(const spec::ip_address_family);`

Methods:

- `stream_socket& opAssign(const socket&in sock);`
- `stream_socket& opAssign(const stream_socket&in socket);`
- `int opCmp(const stream_socket&in);`
- `socket_type get_type() const property;`
- `bool get_is_null() const property;`
- `bool get_is_stream() const property;`
- `bool get_is_datagram() const property;`
- `bool get_is_raw() const property;`
- `void close();`
- `bool poll(const timespan&inout timeout, int mode) const;`
- `int get_available() const property;`
- `int get_error() const property;`
- `void set_send_buffer_size(int size) property;`
- `int get_send_buffer_size() const property;`
- `void set_receive_buffer_size(int size) property;`
- `int get_receive_buffer_size() const property;`
- `void set_send_timeout(const timespan&in timeout) property;`
- `timespan get_send_timeout() const property;`
- `void set_receive_timeout(const timespan&in timeout) property;`
- `timespan get_receive_timeout() const property;`
- `void set_option(int level, int option, int value);`
- `void set_option(int level, int option, uint value);`
- `void set_option(int level, int option, uint8 value);`
- `void set_option(int level, int option, const timespan&in value);`
- `void set_option(int level, int option, const spec::ip_address&in value);`
- `void get_option(int level, int option, int&out value) const;`
- `void get_option(int level, int option, uint&out value) const;`
- `void get_option(int level, int option, uint8&out value) const;`
- `void get_option(int level, int option, timespan&out value) const;`
- `void get_option(int level, int option, spec::ip_address&out value);`
- `void set_linger(bool on, int seconds);`
- `void get_linger(bool&out on, int&out seconds);`
- `void set_no_delay(bool flag) property;`
- `bool get_no_delay() const property;`
- `void set_keep_alive(bool flag) property;`
- `bool get_keep_alive() const property;`
- `void set_reuse_address(bool flag) property;`
- `bool get_reuse_address() const property;`
- `void set_reuse_port(bool flag) property;`
- `bool get_reuse_port() const property;`
- `void set_oob_inline(bool flag) property;`
- `bool get_oob_inline() const property;`
- `void set_blocking(bool flag) property;`
- `bool get_blocking() const property;`
- `socket_address get_address() const property;`
- `socket_address get_peer_address() const property;`
- `bool get_secure() const property;`
- `void init(int af);`
- `void connect(const socket_address&in address);`
- `void connect(const socket_address&in address, const timespan&in timeout);`
- `void connect_nonblocking(const socket_address&in address);`
- `bool bind(const socket_address&in address, bool reuse_address = false, bool IPv6_only = false);`
- `void shutdown_receive();`
- `int shutdown_send();`
- `int shutdown();`
- `int send_bytes(const string&in data, int flags = 0);`
- `string receive_bytes(int length, int flags = 0);`
- `string receive_bytes(int flags = 0, const timespan&in timeout = 100000);`

### string

Methods:

- `string& opAssign(const string&in);`
- `string& opAddAssign(const string&in);`
- `bool opEquals(const string&in) const;`
- `int opCmp(const string&in) const;`
- `string opAdd(const string&in) const;`
- `uint length() const;`
- `void resize(uint);`
- `void reserve(uint);`
- `bool is_empty() const;`
- `const string get_opIndex(uint) const property;`
- `void set_opIndex(uint, const string&in) property;`
- `string& opAssign(double);`
- `string& opAddAssign(double);`
- `string opAdd(double) const;`
- `string opAdd_r(double) const;`
- `string& opAssign(float);`
- `string& opAddAssign(float);`
- `string opAdd(float) const;`
- `string opAdd_r(float) const;`
- `string& opAssign(int64);`
- `string& opAddAssign(int64);`
- `string opAdd(int64) const;`
- `string opAdd_r(int64) const;`
- `string& opAssign(uint64);`
- `string& opAddAssign(uint64);`
- `string opAdd(uint64) const;`
- `string opAdd_r(uint64) const;`
- `string& opAssign(bool);`
- `string& opAddAssign(bool);`
- `string opAdd(bool) const;`
- `string opAdd_r(bool) const;`
- `string substr(uint start = 0, int count = -1) const;`
- `string substr(int start, int count = -1) const;`
- `int find_first(const string&in, uint start = 0) const;`
- `int find_first_of(const string&in, uint start = 0) const;`
- `int find_first_not_of(const string&in, uint start = 0) const;`
- `int find_last(const string&in, int start = -1) const;`
- `int find_last_of(const string&in, int start = -1) const;`
- `int find_last_not_of(const string&in, int start = -1) const;`
- `void insert(uint pos, const string&in other);`
- `void erase(uint pos, int count = -1);`
- `uint size() const;`
- `bool empty() const;`
- `int find(const string&in, uint start = 0) const;`
- `int rfind(const string&in, int start = -1) const;`
- `array<string>@ split(const string&in, bool = true, bool = false) const;`
- `string slice(int start = 0, int end = 0) const;`
- `string replace_range(uint start, int count, const string&in) const;`
- `string replace(const string&in, const string&in, bool = true, uint = 0) const;`
- `string& replace_this(const string&in, const string&in, bool = true, uint = 0) const;`
- `string reverse_bytes() const;`
- `string opMul(uint) const;`
- `string& opMulAssign(uint);`
- `uint64 count(const string&in search, uint64 start = 0) const;`
- `string format(array<string>@ elements) const;`
- `string format(const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null, const ?&in = null) const;`
- `uint64 get_address() const property;`
- `string opAdd(const var&in) const;`
- `string& opAssign(const var&in);`
- `string& opAddAssign(const var&in);`
- `bool is_upper(const string&in = "") const;`
- `bool is_lower(const string&in = "") const;`
- `bool is_whitespace(const string&in = "") const;`
- `bool is_punctuation(const string&in = "") const;`
- `bool is_alphabetic(const string&in = "") const;`
- `bool is_digits(const string&in = "") const;`
- `bool is_alphanumeric(const string&in = "") const;`
- `string upper() const;`
- `string& upper_this();`
- `string lower() const;`
- `string& lower_this();`
- `string trim_whitespace_left() const;`
- `string& trim_whitespace_left_this();`
- `string trim_whitespace_right() const;`
- `string& trim_whitespace_right_this();`
- `string trim_whitespace() const;`
- `string& trim_whitespace_this();`
- `string reverse(const string&in = "") const;`
- `string escape(bool = false) const;`
- `string unescape() const;`
- `bool starts_with(const string&in) const;`
- `bool ends_with(const string&in) const;`
- `string replace_characters(const string&in, const string&in) const;`
- `string& replace_characters_this(const string&in, const string&in);`
- `void remove_UTF8_BOM();`
- `uint unpacket(uint, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`
- `uint unpacket(uint, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out, const ?&out);`

### thread

Construction:

- `thread@ thread();`
- `thread@ thread(const string&in name);`

Methods:

- `int get_id() const property;`
- `void set_priority(thread_priority priority) property;`
- `thread_priority get_priority() const property;`
- `void set_name(const string&in name) property;`
- `string get_name() const property;`
- `void join();`
- `bool join(uint ms);`
- `bool get_running() const property;`
- `void start(thread_callback@ routine, dictionary@ args = null);`
- `void wake_up();`

### thread_event

Construction:

- `thread_event@ thread_event(thread_event_type type = THREAD_EVENT_AUTO_RESET);`

Methods:

- `void set();`
- `void wait();`
- `void wait(uint ms);`
- `bool try_wait(uint ms);`
- `void reset();`

### thread_pool

Construction:

- `thread_pool@ thread_pool(int min_capacity = 2, int max_capacity = 16, int idle_time = 60, int stack_size = 0);`

Methods:

- `void add_capacity(int modifier);`
- `int get_capacity() const property;`
- `void set_stack_size(int size) property;`
- `int get_stack_size() const property;`
- `int get_used() const property;`
- `int get_allocated() const property;`
- `int get_available() const property;`
- `void start(thread_callback@ routine, dictionary@ args = null);`
- `void start(thread_callback@ routine, dictionary@ args, thread_priority priority);`
- `void start(thread_callback@ routine, dictionary@ args, const string&in name);`
- `void start(thread_callback@ routine, dictionary@ args, const string&in name, thread_priority priority);`
- `void stop_all();`
- `void join_all();`
- `void collect();`
- `const string& get_name() const property;`

### timer

Construction:

- `timer@ timer();`
- `timer@ timer(bool speedhack_protection);`
- `timer@ timer(int64 initial_elapsed, bool speedhack_protection = speedhack_protection);`
- `timer@ timer(int64 initial_elapsed, uint64 accuracy, bool speedhack_protection = speedhack_protection);`

Properties:

- `uint64 accuracy;`

Methods:

- `int64 get_elapsed() const property;`
- `void set_elapsed(int64 time_units) property;`
- `bool has_elapsed(int64 time_units) const;`
- `bool tick(int64 time_units);`
- `void force(int64 elapsed);`
- `void adjust(int64 mod_elapsed);`
- `void restart();`
- `bool get_secure() const property;`
- `void set_secure(bool secure) property;`
- `bool get_paused() const property;`
- `bool get_running() const property;`
- `void toggle_pause();`
- `bool pause();`
- `bool resume();`
- `bool set_paused(bool paused);`

### timer_queue

Construction:

- `timer_queue@ timer_queue();`

Properties:

- `const string failures;`

Methods:

- `void set(const string&in timer_id, timer_callback@ callback, const string&in callback_data, uint64 milliseconds, bool repeating = false);`
- `void set(const string&in timer_id, timer_callback@ callback, uint64 milliseconds, bool repeating = false);`
- `uint64 elapsed(const string&in timer_id) const;`
- `uint64 timeout(const string&in timer_id) const;`
- `bool exists(const string&in timer_id) const;`
- `bool restart(const string&in timer_id);`
- `bool is_repeating(const string&in timer_id) const;`
- `bool set_timeout(const string&in timer_id, uint64 milliseconds, bool repeating = false);`
- `bool delete(const string&in timer_id);`
- `void flush();`
- `void reset();`
- `array<string>@ list_timers();`
- `uint size() const;`
- `bool loop(int max_timers = 0, int max_catchup_milliseconds = 100);`

### timespan

Methods:

- `timespan& opAssign(const timespan&in);`
- `timespan& opAssign(int64 microseconds);`
- `bool opEquals(const timespan&in) const;`
- `bool opEquals(int64 microseconds) const;`
- `int opCmp(const timespan&in) const;`
- `int opCmp(int64 microseconds) const;`
- `timespan opAdd(int64 microseconds) const;`
- `timespan opAdd(const timespan&in) const;`
- `timespan opSub(int64 microseconds) const;`
- `timespan opSub(const timespan&in) const;`
- `timespan& opAddAssign(int64 milliseconds);`
- `timespan& opAddAssign(const timespan&in);`
- `timespan& opSubAssign(int64 milliseconds);`
- `timespan& opSubAssign(const timespan&in);`
- `int get_days() const property;`
- `int get_hours() const property;`
- `int get_total_hours() const property;`
- `int get_minutes() const property;`
- `int get_total_minutes() const property;`
- `int get_seconds() const property;`
- `int get_total_seconds() const property;`
- `int get_milliseconds() const property;`
- `int get_total_milliseconds() const property;`
- `int get_microseconds() const property;`
- `int get_useconds() const property;`
- `int get_total_microseconds() const property;`
- `string format(const string&in fmt = "%dd %H:%M:%S.%i");`

### timestamp

Methods:

- `timestamp& opAssign(const timestamp&in);`
- `timestamp& opAssign(int64);`
- `void update();`
- `bool opEquals(const timestamp&in) const;`
- `int opCmp(const timestamp&in) const;`
- `timestamp opAdd(int64) const;`
- `timestamp opAdd(const timespan&in) const;`
- `timestamp opSub(int64) const;`
- `timestamp opSub(const timespan&in) const;`
- `int64 opSub(const timestamp&in) const;`
- `timestamp& opAddAssign(int64);`
- `timestamp& opAddAssign(const timespan&in);`
- `timestamp& opSubAssign(int64);`
- `timestamp& opSubAssign(const timespan&in);`
- `int64 get_UTC_time() const property;`
- `int64 get_elapsed() const property;`
- `bool has_elapsed(int64) const;`
- `int64 opImplConv() const;`
- `string format(const string&in fmt, int tzd = 0xffff);`

### tone_synth

Construction:

- `tone_synth@ tone_synth();`

Methods:

- `void reset();`
- `void set_waveform_type(int type) property;`
- `int get_waveform_type() const property;`
- `void set_allow_silent_output(bool silence) property;`
- `bool get_allow_silent_output() const property;`
- `void set_volume(double value) property;`
- `double get_volume() const property;`
- `void set_pan(double value) property;`
- `double get_pan() const property;`
- `void set_tempo(double value) property;`
- `double get_tempo() const property;`
- `void set_note_transpose(double value) property;`
- `double get_note_transpose() const property;`
- `void set_freq_transpose(double value) property;`
- `double get_freq_transpose() const property;`
- `double get_position() const property;`
- `int get_position_ms() const property;`
- `double get_length() const property;`
- `int get_length_ms() const property;`
- `bool seek(double position);`
- `bool seek_ms(int position);`
- `bool rewind(double amount);`
- `bool rewind_ms(int amount);`
- `bool set_edge_fades(int start, int end);`
- `bool note(string note, double length);`
- `bool note_ms(string note, int ms);`
- `bool note_bend(string note, int bend_amount, double length, double bend_start, double bend_length);`
- `bool note_bend_ms(string note, int bend_amount, int length, int bend_start, int bend_length);`
- `bool freq(double freq, double length);`
- `bool freq_ms(double freq, int ms);`
- `bool freq_bend(double freq, int bend_amount, double length, double bend_start, double bend_length);`
- `bool freq_bend_ms(double freq, int bend_amount, int length, int bend_start, int bend_length);`
- `bool rest(double length);`
- `bool rest_ms(int ms);`
- `int get_sample_rate() property;`
- `int get_channels() property;`
- `sound@ write_wave_sound();`
- `array<int16>@ write_samples();`
- `bool write_wave_file(const string&in filename);`

### touch_finger

Properties:

- `const uint64 id;`
- `const float x;`
- `const float y;`
- `const float pressure;`

### tts_voice

Construction:

- `tts_voice@ tts_voice(const string&in engines = "");`

Methods:

- `bool speak(const string&in text, bool interrupt = false);`
- `bool speak_interrupt(const string&in text);`
- `bool speak_to_file(const string&in filename, const string&in text);`
- `bool speak_wait(const string&in text, bool interrupt = false);`
- `string speak_to_memory(const string&in text);`
- `sound@ speak_to_sound(const string&in text);`
- `bool speak_interrupt_wait(const string&in text);`
- `bool refresh();`
- `bool stop();`
- `array<string>@ list_voices() const;`
- `array<string>@ get_voice_names() const;`
- `bool set_voice(int index);`
- `bool set_current_voice(int index);`
- `float get_rate() const property;`
- `void set_rate(float rate) property;`
- `float get_pitch() const property;`
- `void set_pitch(float pitch) property;`
- `float get_volume() const property;`
- `void set_volume(float volume) property;`
- `int get_voice_count() const property;`
- `string get_voice_name(int index) const;`
- `string get_voice_language(int index) const;`
- `bool set_language(const string&in language);`
- `string get_language() const property;`
- `bool get_speaking() const property;`
- `int get_voice() const property;`

### spec::uri

Methods:

- `spec::uri& opAssign(const spec::uri&in);`
- `spec::uri& opAssign(const string&in uri);`
- `bool opEquals(const spec::uri&in);`
- `bool opEquals(const string&in uri);`
- `void clear();`
- `string opImplConv() const;`
- `const string& get_scheme() const property;`
- `void set_scheme(const string&in scheme) property;`
- `const string& get_user_info() const property;`
- `void set_user_info(const string&in user_info) property;`
- `const string& get_host() const property;`
- `void set_host(const string&in host) property;`
- `uint16 get_port() const property;`
- `void set_port(uint16 port) property;`
- `uint16 get_specified_port() const property;`
- `string get_authority() const property;`
- `void set_authority(const string&in authority) property;`
- `const string& get_path() const property;`
- `void set_path(const string&in path) property;`
- `string get_query() const property;`
- `void set_query(const string&in query) property;`
- `void add_query_parameter(const string&in param, const string&in value = "");`
- `const string& get_raw_query() const property;`
- `void set_raw_query(const string&in query) property;`
- `string get_fragment() const property;`
- `void set_fragment(const string&in fragment) property;`
- `string get_raw_fragment() const property;`
- `void set_raw_fragment(const string&in fragment) property;`
- `string get_path_etc() const property;`
- `void set_path_etc(const string&in path_etc) property;`
- `string get_path_and_query() const property;`
- `void resolve(const string&in relative_uri);`
- `void resolve(const spec::uri&in relative_uri);`
- `bool get_is_relative() const property;`
- `bool get_is_empty() const property;`
- `bool normalize();`
- `array<array<string>>@ get_query_parameters(bool plus_as_space = true) const;`
- `array<string>@ get_path_segments() const;`

### uuid

Methods:

- `uuid& opAssign(const uuid&in);`
- `string to_string() const;`
- `string get_str() const property;`
- `string opConv() const;`
- `string opImplConv() const;`
- `void parse(const string&in);`
- `bool try_parse(const string&in);`
- `int get_version() const property;`
- `int get_variant() const property;`
- `bool get_is_null() const property;`
- `bool opEquals(const uuid&in) const;`
- `int opCmp(const uuid&in);`
- `string get_bytes() const;`
- `void set_bytes(const string&in);`

### var

Construction:

- `var@ var();`
- `var@ var(const int&in);`
- `var@ var(const uint&in);`
- `var@ var(const int16&in);`
- `var@ var(const uint16&in);`
- `var@ var(const int64&in);`
- `var@ var(const uint64&in);`
- `var@ var(const float&in);`
- `var@ var(const double&in);`
- `var@ var(const bool&in);`
- `var@ var(const string&in);`
- `var@ var(json_object@);`
- `var@ var(json_array@);`

Methods:

- `var& opAssign(const var&in);`
- `var& opPostInc();`
- `var& opPostDec();`
- `int opCmp(const var&in) const;`
- `var& opAssign(const int&in);`
- `int opAddAssign(const int&in);`
- `int opAdd(const int&in) const;`
- `int opSubAssign(const int&in);`
- `int opSub(const int&in) const;`
- `int opMulAssign(const int&in);`
- `int opMul(const int&in) const;`
- `int opDivAssign(const int&in);`
- `int opDiv(const int&in) const;`
- `int opModAssign(const int&in);`
- `int opMod(const int&in) const;`
- `int opImplConv() const;`
- `var& opAssign(const uint&in);`
- `uint opAddAssign(const uint&in);`
- `uint opAdd(const uint&in) const;`
- `uint opSubAssign(const uint&in);`
- `uint opSub(const uint&in) const;`
- `uint opMulAssign(const uint&in);`
- `uint opMul(const uint&in) const;`
- `uint opDivAssign(const uint&in);`
- `uint opDiv(const uint&in) const;`
- `uint opModAssign(const uint&in);`
- `uint opMod(const uint&in) const;`
- `uint opImplConv() const;`
- `var& opAssign(const int16&in);`
- `int16 opAddAssign(const int16&in);`
- `int16 opAdd(const int16&in) const;`
- `int16 opSubAssign(const int16&in);`
- `int16 opSub(const int16&in) const;`
- `int16 opMulAssign(const int16&in);`
- `int16 opMul(const int16&in) const;`
- `int16 opDivAssign(const int16&in);`
- `int16 opDiv(const int16&in) const;`
- `int16 opModAssign(const int16&in);`
- `int16 opMod(const int16&in) const;`
- `int16 opImplConv() const;`
- `var& opAssign(const uint16&in);`
- `uint16 opAddAssign(const uint16&in);`
- `uint16 opAdd(const uint16&in) const;`
- `uint16 opSubAssign(const uint16&in);`
- `uint16 opSub(const uint16&in) const;`
- `uint16 opMulAssign(const uint16&in);`
- `uint16 opMul(const uint16&in) const;`
- `uint16 opDivAssign(const uint16&in);`
- `uint16 opDiv(const uint16&in) const;`
- `uint16 opModAssign(const uint16&in);`
- `uint16 opMod(const uint16&in) const;`
- `uint16 opImplConv() const;`
- `var& opAssign(const int64&in);`
- `int64 opAddAssign(const int64&in);`
- `int64 opAdd(const int64&in) const;`
- `int64 opSubAssign(const int64&in);`
- `int64 opSub(const int64&in) const;`
- `int64 opMulAssign(const int64&in);`
- `int64 opMul(const int64&in) const;`
- `int64 opDivAssign(const int64&in);`
- `int64 opDiv(const int64&in) const;`
- `int64 opModAssign(const int64&in);`
- `int64 opMod(const int64&in) const;`
- `int64 opImplConv() const;`
- `var& opAssign(const uint64&in);`
- `uint64 opAddAssign(const uint64&in);`
- `uint64 opAdd(const uint64&in) const;`
- `uint64 opSubAssign(const uint64&in);`
- `uint64 opSub(const uint64&in) const;`
- `uint64 opMulAssign(const uint64&in);`
- `uint64 opMul(const uint64&in) const;`
- `uint64 opDivAssign(const uint64&in);`
- `uint64 opDiv(const uint64&in) const;`
- `uint64 opModAssign(const uint64&in);`
- `uint64 opMod(const uint64&in) const;`
- `uint64 opImplConv() const;`
- `var& opAssign(const float&in);`
- `float opAddAssign(const float&in);`
- `float opAdd(const float&in) const;`
- `float opSubAssign(const float&in);`
- `float opSub(const float&in) const;`
- `float opMulAssign(const float&in);`
- `float opMul(const float&in) const;`
- `float opDivAssign(const float&in);`
- `float opDiv(const float&in) const;`
- `float opImplConv() const;`
- `var& opAssign(const double&in);`
- `double opAddAssign(const double&in);`
- `double opAdd(const double&in) const;`
- `double opSubAssign(const double&in);`
- `double opSub(const double&in) const;`
- `double opMulAssign(const double&in);`
- `double opMul(const double&in) const;`
- `double opDivAssign(const double&in);`
- `double opDiv(const double&in) const;`
- `double opImplConv() const;`
- `var& opAssign(const bool&in);`
- `bool opAddAssign(const bool&in);`
- `bool opAdd(const bool&in) const;`
- `bool opImplConv() const;`
- `var& opAssign(const string&in);`
- `string opAddAssign(const string&in);`
- `string opAdd(const string&in) const;`
- `string opImplConv() const;`
- `void clear();`
- `bool get_empty() const property;`
- `bool get_is_integer() const property;`
- `bool get_is_signed() const property;`
- `bool get_is_numeric() const property;`
- `bool get_is_boolean() const property;`
- `bool get_is_string() const property;`
- `var& opAssign(const json_object&in) const;`
- `json_object@ opImplCast() const;`
- `var& opAssign(const json_array&in) const;`
- `json_array@ opImplCast() const;`

### vector

Properties:

- `float x;`
- `float y;`
- `float z;`

Methods:

- `vector& opAddAssign(const vector&in);`
- `vector& opSubAssign(const vector&in);`
- `vector& opMulAssign(float);`
- `vector& opDivAssign(float);`
- `bool opEquals(const vector&in) const;`
- `vector opAdd(const vector&in) const;`
- `vector opSub(const vector&in) const;`
- `vector opMul(const vector&in) const;`
- `vector opDiv(const vector&in) const;`
- `vector opMul(float) const;`
- `vector opDiv(float) const;`
- `void set(float x, float y, float z);`
- `void setToZero();`
- `float length() const;`
- `float length_square() const;`
- `bool get_is_zero() const property;`
- `bool get_is_unit() const property;`
- `bool get_is_finite() const property;`
- `float dot(const vector&in) const;`
- `vector cross(const vector&in) const;`
- `void normalize();`
- `vector get_absolute() const property;`
- `int get_min_axis() const property;`
- `int get_max_axis() const property;`
- `float get_min_value() const property;`
- `float get_max_value() const property;`
- `float& opIndex(int index);`
- `const float& opIndex(int index) const;`
- `string opImplConv() const;`

### weakref<T>

Methods:

- `T@ opImplCast();`
- `T@ get() const;`
- `weakref<T>& opHndlAssign(const weakref<T>&in);`
- `weakref<T>& opAssign(const weakref<T>&in);`
- `bool opEquals(const weakref<T>&in) const;`
- `weakref<T>& opHndlAssign(T@);`
- `bool opEquals(const T@) const;`

### web_socket

Construction:

- `web_socket@ web_socket(const socket&in sock);`
- `web_socket@ web_socket(const web_socket&in sock);`
- `web_socket@ web_socket(http_client&inout cs, http_request&inout request, http_response&inout response);`
- `web_socket@ web_socket(http_client&inout cs, http_request&inout request, http_response&inout response, http_credentials&inout credentials);`

Methods:

- `web_socket& opAssign(const socket&in sock);`
- `web_socket& opAssign(const web_socket&in socket);`
- `int opCmp(const web_socket&in);`
- `socket_type get_type() const property;`
- `bool get_is_null() const property;`
- `bool get_is_stream() const property;`
- `bool get_is_datagram() const property;`
- `bool get_is_raw() const property;`
- `void close();`
- `bool poll(const timespan&inout timeout, int mode) const;`
- `int get_available() const property;`
- `int get_error() const property;`
- `void set_send_buffer_size(int size) property;`
- `int get_send_buffer_size() const property;`
- `void set_receive_buffer_size(int size) property;`
- `int get_receive_buffer_size() const property;`
- `void set_send_timeout(const timespan&in timeout) property;`
- `timespan get_send_timeout() const property;`
- `void set_receive_timeout(const timespan&in timeout) property;`
- `timespan get_receive_timeout() const property;`
- `void set_option(int level, int option, int value);`
- `void set_option(int level, int option, uint value);`
- `void set_option(int level, int option, uint8 value);`
- `void set_option(int level, int option, const timespan&in value);`
- `void set_option(int level, int option, const spec::ip_address&in value);`
- `void get_option(int level, int option, int&out value) const;`
- `void get_option(int level, int option, uint&out value) const;`
- `void get_option(int level, int option, uint8&out value) const;`
- `void get_option(int level, int option, timespan&out value) const;`
- `void get_option(int level, int option, spec::ip_address&out value);`
- `void set_linger(bool on, int seconds);`
- `void get_linger(bool&out on, int&out seconds);`
- `void set_no_delay(bool flag) property;`
- `bool get_no_delay() const property;`
- `void set_keep_alive(bool flag) property;`
- `bool get_keep_alive() const property;`
- `void set_reuse_address(bool flag) property;`
- `bool get_reuse_address() const property;`
- `void set_reuse_port(bool flag) property;`
- `bool get_reuse_port() const property;`
- `void set_oob_inline(bool flag) property;`
- `bool get_oob_inline() const property;`
- `void set_blocking(bool flag) property;`
- `bool get_blocking() const property;`
- `socket_address get_address() const property;`
- `socket_address get_peer_address() const property;`
- `bool get_secure() const property;`
- `void init(int af);`
- `void shutdown_receive();`
- `int shutdown_send();`
- `int shutdown();`
- `int send_bytes(const string&in data, int flags = 0);`
- `string receive_bytes(int length, int flags = 0);`
- `string receive_bytes(int flags = 0, const timespan&in timeout = 100000);`
- `int shutdown(uint16 status_code, const string&in status_message = "");`
- `int send_frame(const string&in data, int flags = WS_FRAME_TEXT);`
- `string receive_frame(int&out flags);`
- `web_socket_mode get_mode() const property;`
- `void set_max_payload_size(int size) property;`
- `int get_max_payload_size() const property;`

