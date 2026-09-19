# UGui Design System
This package contains code _and_ starting points for building a highly structured, customizable, prefab-based UI Design System.

## Idea
The goal is to mitigate some of UGUI's shortcomings in regards to robustness
by creating a library of prefabs that heavily use nesting and variants.

An example: All occurrences of text in the system are nested prefab instances
of either the base "Text" prefab or one of its variants.
This way, to change the primary font of the project, it just has to be changed
in that prefab.

## Installation
The package compiles right after installing. `UIContent` ships with it. To adjust it to a project's needs,
derive from it (its `components` field is protected), or copy the package into the project's `Packages` folder
and edit `Runtime/UIContent.cs`.

**Upgrading from 0.14 or older:** earlier versions asked you to import the sample `Initial UIContent`. If you did,
delete that imported copy of `UIContent.cs` and its `.asmref`. The class is part of the package now, and two
copies would clash.

## Concepts/Usage
### Controllers
Every root-level prefab has a `[...]Controller` component attached that is
meant to be the entry point for interacting with the UI element.

It references the `UIContent` of the element, if there is one.
It also contains specific methods for the element, like for managing click/toggle/slide/... event responses.

### UI Contents
UI elements come with different combinations of properties, like
- primary button with text
- primary button with icon
- secondary button with text
- ...

We don't want another prefab for each of these combinations.
So instead, we have a customizable set of "UIContent prefabs" that get nested
into content-less UI "element prefabs".

The element prefabs would be:
- primary button
- secondary button
- ...

And the UIContent prefabs are:
- UIContent (Text)
- UIContent (Icon)
- UIContent (Icon+Text, Horizontal)
- ...

They are meant to be combined ad-hoc:
1. Add "secondary button" to dialog.
2. Add nested "UIContent (Text)" to button.
3. Edit properties of each.

However, it is absolutely fine to have prefabs that combine these
for commonly occuring UI elements (like a "close window" button).

### Changing Elements
It is intended for _all_ elements that are provided through samples
to potentially be changed.
This includes the `UIContent` class and all prefabs.

For example, if a project's design system has a second text
as a recurring content element, the `UIContent` class should be extended
by a property that allows updating that text via code.

Similarly, if it's a common occurrence for a project to change the color
of a text, the `UIContent` class should get accessors to that property.

### Notification Cascade
With the package comes a system to logically connect 'red dot'-like markers
through a path of nested windows. 
Example: You want to notify the player that he/she can redeem some item 
in a seasonal event window. So, you can mark the button to open the event 
window with a red dot and the button inside the seasonal event window.
After clicking the redeem button you want both red dots to vanish, the redeem
and the window-open-button. This system can do this for you.

Internally the system uses a string-based path system. After removing a registered
notification (the 'red dot'), it will follow the path up and unregister it for all
elements along the path, up to the first element. (same for registering of course)

See example scene how the `NotificationDatabase` works.

### Tab System
The package includes a tab system for UI windows with multiple pages.
Example: You have a shop window with three tabs: "Items", "Resources", and "Special Offers".
When the player opens the shop, you want to show the "Special Offers" tab first.
The selected tab should have a different sprite than the others. When the player clicks
another tab, it should switch the content and update the visual state. 

Internally the system uses Unity's event interfaces (IPointerClickHandler, etc.).

See example scene how the Tab System works.

### Tooltips
Features:
- Tooltips with arrow pointers (up/down)
- Tooltips stay within given RectTransform
- Tooltips point towards given target RectTransform
- Tooltips animate in
- Tooltip nesting (a tooltip can invoke another tooltip)
- Screen spanning background blocker to close a tooltip
- Sync or async instantiation
- Smart free space selection: Tooltip will show below or above target, depending on where's more free space
- Example scene

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/ugui-design-system.git
```

Pin a release by appending a tag, for example `#v0.15.0`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.tea-spoons.package-core` 0.5.0
- `com.tea-spoons.runtime-toolbox` 0.11.0
- `com.cysharp.unitask` 2.5.0
- `com.tea-spoons.addressables-toolbox` 0.5.0

## Optional packages

This package works on its own. It uses the packages below when your project has them (Unity detects them automatically) and simply leaves the related code out when it does not.

| Package | Used for |
|---|---|
| uGUI (`com.unity.ugui`) | The package itself. Without it nothing is compiled. |
| TextMeshPro (part of uGUI 2.0+, `com.unity.textmeshpro` before Unity 6) | The label of `UIContent` and the counter label of `UINotificationHighlight`. Without TextMeshPro `UIContent` uses uGUI's built-in `Text`. |
| UniTask (`com.cysharp.unitask` 2.5.0+) | `TooltipContent<T>`. |
| Addressables (`com.unity.addressables` 2.2.2+) and UniTask | `AsyncLoadableTooltip` and `TooltipTReference`. |
| Unity Localization (`com.unity.localization` 1.5+) | Localized content in the `UIContent` editor. |

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
