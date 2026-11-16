# Changelog

## 2.1.0 — unreleased

The public API is unchanged; existing code compiles as-is.

### Fixed — these all threw exceptions in 2.0.1
- Read-only and private-setter destination properties are now skipped (was `ArgumentException`).
- Properties with the same name but incompatible types are now skipped (was `ArgumentException`).
- Write-only source properties are now skipped (was `ArgumentException`).
- Indexers on both types are now ignored (was `TargetParameterCountException`).
- Properties hidden with `new` in a derived class now use the most-derived one (was `AmbiguousMatchException`).
- Value-type (struct) destinations now return the mapped values instead of an unmapped copy.

### Changed
- `null` source or destination throws `ArgumentNullException` instead of `NullReferenceException`.
- A destination that is not a `T` throws `ArgumentException` **before** any property is changed
  (2.0.1 mapped first and then threw `InvalidCastException`).
- A type without a parameterless constructor throws `InvalidOperationException` with a clearer message.
- A `null` skip list is treated as empty.
- Static properties are no longer copied.
- Property pairs are cached per type pair — roughly 6× faster on repeated mappings.

### Packaging
- XML documentation, README, Source Link and a symbol package (`.snupkg`) are now included.
- Unit tests and GitHub Actions CI/publish workflows added.
- Removed the legacy `netcoreapp2.1` `BasicMapper` project; source moved to `src/`.

## 2.0.1 — 2020-05-26
- Compatibility fix; retargeted to .NET Standard 2.0.

## 1.1.0 / 1.0.0 — 2019-09-09
- Initial releases (`netcoreapp2.1`).
