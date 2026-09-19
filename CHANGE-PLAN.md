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

- [x] Tag and publish `v0.15.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
<!-- review-items:start -->
- [ ] **P0** `package.json` has no `unity` field. Set it to the lowest Unity version that is actually tested (only 6000.3.8f1 was tested in this review).
- [ ] **P1** Add PlayMode tests for the controllers and formatters (a button and toggle click, tab switching, progress bar formatting).
- [ ] **P1** Make the three own-package dependencies optional, in the same way as in the other packages.
- [ ] **P2** Collect content with the `GetComponentsInChildren(list)` overload and a pooled list to avoid the allocation.
- [ ] **P2** Document `UIContent` and `UIRepresentation` and how a prefab is meant to be wired up.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Unity UI Extensions (uGUI)](https://github.com/Unity-UI-Extensions/com.unity.uiextensions) | BSD-3-Clause (uGUI package) | A catalogue of ready-made uGUI controls. This package is closer to a design system (controllers, content, formatters, tooltips), so no direct comparable turned up in this pass. |

### Findings from reading the code

- **[Metadata]** `package.json` has no `unity` field.
- **[Tests]** One test file (56 lines) for about 2,450 lines of code. The controllers (button, toggle, tab, progress bar, counter) and the formatters are untested.
- **[Coupling]** It declares three own packages (package-core, runtime-toolbox, addressables-toolbox). The Addressables and UniTask parts already compile out when those are missing.
- **[Perf]** `UIController` collects its content with `GetComponentsInChildren<UIContent>()` and then checks `GetComponentInParent<UIController>()` for each one (`UIController.cs`, lines 49-51). That allocates an array on every collection.
- **[Logging]** Six direct `Debug.Log*` calls.
<!-- review:end -->

## Notes and ideas

_Add your own here._
