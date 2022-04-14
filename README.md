# SpaceWizards.Sodium ![Nuget](https://img.shields.io/nuget/v/SpaceWizards.Sodium)

A .NET [libsodium](https://libsodium.gitbook.io/doc/) binding that doesn't suck. Actually exposes the native API so you don't have to cry yourself to sleep in unnecessary allocations or OOP nonsense.

Currently depends on the ["libsodium" NuGet package](https://www.nuget.org/packages/libsodium/) provided by [Sodium.Core](https://github.com/ektrah/libsodium-core) for the native library. They say you shouldn't depend on it directly but I am too lazy to compile them myself so deal with it.

# API Shape

There is a low-level API in `SpaceWizards.Sodium.Interop`. This is a raw P/Invoke binding, hope you like pointers.

The high-level API in `SpaceWizards.Sodium` generally has two variants: span with return code, or `byte[]` with exceptions. Note that the former still throws exceptions if you pass spans that are too small, but otherwise failing return codes from libsodium are passed through so you can handle them. This is just what I decided to settle on to keep API size down.


Also, if it wasn't obvious, the only API wrapped is the ones I needed at the moment. PRs welcome I guess.
