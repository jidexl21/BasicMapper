# OF.BasicMapper

[![CI](https://github.com/jidexl21/BasicMapper/actions/workflows/ci.yml/badge.svg)](https://github.com/jidexl21/BasicMapper/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/OF.BasicMapper.svg)](https://www.nuget.org/packages/OF.BasicMapper)

A tiny, dependency-free object-to-object mapper for .NET. It copies values between public properties that have
the **same name** and a **compatible type** — handy for entity ↔ DTO conversion when a full mapping library is overkill.

Targets **.NET Standard 2.0**, so it runs on .NET Framework 4.6.1+, .NET Core 2.0+ and .NET 5–10.

## Install

```bash
dotnet add package OF.BasicMapper
```

## Quick start

```csharp
using OF.BasicMapper;

public class Customer    { public int Id { get; set; } public string Name { get; set; } public string PasswordHash { get; set; } }
public class CustomerDto { public int Id { get; set; } public string Name { get; set; } }

IBasicMapper mapper = new BasicMapper();

// 1. Create a new object
CustomerDto dto = mapper.MapFields<CustomerDto>(customer);

// 2. Update an existing object (returns the same instance)
mapper.MapFields<Customer>(dto, existingCustomer);

// 3. Skip properties you don't want copied
var safe = mapper.MapFields<Customer>(input, new List<string> { "Id", "PasswordHash" });
mapper.MapFields<Customer>(input, existingCustomer, new List<string> { "Id" });
```

Anonymous objects work as a source too:

```csharp
var dto = mapper.MapFields<CustomerDto>(new { Id = 1, Name = "Ada" });
```

### Dependency injection

`BasicMapper` is stateless and thread-safe, so register it as a singleton:

```csharp
services.AddSingleton<IBasicMapper, BasicMapper>();
```

## API

| Method | What it does |
|---|---|
| `T MapFields<T>(object from)` | Creates a new `T` and copies matching properties into it. |
| `T MapFields<T>(object from, List<string> skip)` | Same, but leaves the named properties alone. |
| `T MapFields<T>(object from, object to)` | Copies matching properties onto `to` and returns it as `T`. |
| `T MapFields<T>(object from, object to, List<string> skip)` | Same, but leaves the named properties alone. |

## Mapping rules

A property is copied only when **all** of these hold:

1. Source and destination both have a **public instance** property with the **same name** (case-sensitive).
2. The source property has a public getter and the destination property has a public setter.
3. The source type can be assigned to the destination type: same type, base/interface type, `T → T?`, or `T? → T`.
4. The name is not in the `skip` list (also case-sensitive).

Everything else is **skipped silently**: read-only or private-setter properties, mismatched types (e.g. `int` → `string`),
indexers, static properties and properties that exist on only one side.

Other things to know:

- **Shallow copy.** Lists and nested objects are shared by reference, not cloned or mapped recursively.
- **No type conversion.** `"42"` is not converted to `42`. Map those by hand after calling `MapFields`.
- **`null` from `int?` into `int`** writes `0` (the default), the same as earlier versions.
- **Public fields are not mapped** — only properties, despite the method name.
- `MapFields<T>(from)` needs `T` to have a public parameterless constructor; otherwise pass an existing instance.
- Hidden properties (`public new int Id`) use the most-derived declaration.
- `null` for `from` or `to` throws `ArgumentNullException`; a `to` that isn't a `T` throws `ArgumentException` before anything is changed.

## Performance

Matching property pairs are worked out once per (source type, destination type) and cached, so repeated mappings
only pay for the reflection get/set calls. If you're mapping millions of objects in a hot path, a compile-time
mapper (e.g. a source generator) will still be faster.

## Building and testing

```bash
dotnet build
dotnet test
dotnet pack src/OF.BasicMapper -c Release -o artifacts
```

## Releasing

Releases are published to nuget.org by GitHub Actions when a version tag is pushed:

```bash
git tag v2.1.0
git push origin v2.1.0
```

The tag (without the `v`) becomes the package version. See [`.github/workflows/publish.yml`](https://github.com/jidexl21/BasicMapper/blob/master/.github/workflows/publish.yml)
and [CHANGELOG.md](https://github.com/jidexl21/BasicMapper/blob/master/CHANGELOG.md).

## Licence

MIT

Copyright (c) 2025 BasicMapper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.