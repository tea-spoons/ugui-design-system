# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.ugui-design-system` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- The samples folder now uses Unity's hidden `Samples~` layout and is registered in `package.json`.
- Declared the missing dependency on `addressables-toolbox`.
- Made standalone: no longer declares UniTask as a dependency. The parts that need TextMeshPro, UniTask or Addressables are compiled only when those are installed; the rest of the package needs only uGUI.
- Ships a default `UIContent` in the package (with the original script GUID, so the sample prefabs keep working), so the package compiles without importing a sample. The `Initial UIContent` sample was removed. Without TextMeshPro its label uses uGUI's `Text`.
- Restored the `.meta` files inside `Samples~`: the first migration dropped them, which breaks the links between sample assets when a sample is imported.

## Planned changes

- [ ] Tag and publish `v0.15.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.

## Notes and ideas

_Add your own here._
