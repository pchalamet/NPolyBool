# NPolyBool
[![Build status](https://github.com/pchalamet/NPolyBool/workflows/build/badge.svg)](https://github.com/pchalamet/NPolyBool/actions?query=workflow%3Abuild) [![Nuget](https://img.shields.io/nuget/v/NPolyBool?logo=nuget)](https://nuget.org/packages/NPolyBool)

This is a port of [polybooljs](https://github.com/voidqk/polybooljs) for .net standard 2.0 (written in C#).
Some code reused from [polybool.net](https://github.com/idormenco/PolyBool.Net). That was easier to port again instead of debugging PolyBool.Net.

Expose same interface as polybooljs. All operations have been validated with side by side runs of polybooljs. WARNING: not all tests are provided, just trust me ;-)

# How to
* Build: make build
* Test (on Windows x64 only sorry): make test

You can also use the sln directly with your favorite editor.

# License
Released under MIT (as original source):

* Original source code Copyright (c) 2016 Sean Connelly(@voidqk, web: syntheti.cc)
* Ported source code Copyright (c) 2018 - 2022 Pierre Chalamet
