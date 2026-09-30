# Managed/native interop smoke test

This is a minimal MSBuild-based C# executable that builds and consumes a trivial C++ shared library.

## Prerequisites

- .NET 10 SDK
- `clang++` on `PATH`

## Build and run

From the repository root:

```sh
dotnet build
dotnet run
```

The root project file is the default entry point, so the same commands can also be run with explicit project paths:

```sh
dotnet build managed/Interop.csproj
dotnet run --project managed/Interop.csproj
```

The MSBuild project invokes `clang++` before the managed build and places the native library beside the managed output. Expected output:

```text
Native log: step 1, item 19, message: left operand
Native log: step 2, item 23, message: right operand
Native log: step 3, item 42, message: sum
Native interop succeeded: 19 + 23 = 42
```

The exported API is the C-linkage function:

```cpp
int interop_add(int left, int right, void (*log)(int step, int item, const char* message));
```

Both the function and callback use the Cdecl calling convention. Native invokes the callback synchronously while `interop_add` is running and does not retain it afterward. Each message is a null-terminated UTF-8 string valid only during the callback; managed code must copy it if it needs to retain it. Managed exceptions must not cross the native boundary, so the sample captures callback failures and reports them after the native call returns.

The project currently names the Linux library `libinterop.so`. On Windows it emits `interop.dll`; the native compiler and flags may need adjustment for a Windows toolchain.
