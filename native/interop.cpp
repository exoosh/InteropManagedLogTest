#if defined(_WIN32)
#define INTEROP_EXPORT __declspec(dllexport)
#else
#define INTEROP_EXPORT __attribute__((visibility("default")))
#endif

using interop_log_callback = void (*)(int step, int item, const char* message);

extern "C" INTEROP_EXPORT int interop_add(
    int left,
    int right,
    interop_log_callback log) {
    if (log != nullptr) {
        log(1, left, "left operand");
        log(2, right, "right operand");
    }

    const int result = left + right;

    if (log != nullptr) {
        log(3, result, "sum");
    }

    return result;
}
