# SpaceWizards.Sodium

A .NET [libsodium](https://libsodium.gitbook.io/doc/) binding that doesn't suck. Actually exposes the native API so you don't have to cry yourself to sleep in unnecessary allocations or OOP nonsense.

Currently depends on the ["libsodium" NuGet package](https://www.nuget.org/packages/libsodium/) provided by [Sodium.Core](https://github.com/ektrah/libsodium-core) for the native library. They say you shouldn't depend on it directly but I am too lazy to compile them myself so deal with it.
