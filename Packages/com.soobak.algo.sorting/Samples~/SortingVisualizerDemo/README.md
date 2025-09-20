# Sorting Visualizer Demo

This sample scene demonstrates how to pair `SortingRunner` with a simple
`IBarVisualizer` implementation to preview algorithms from the catalog at runtime.

## Contents
- **SortingBarVisualizer** – Instantiates primitive cubes to represent each
  element and updates their height/colour as `SortOp` events arrive.
- **SortingDemoController** – Seeds a random array, picks an algorithm ID, and
  kicks off execution via `SortingRunner` on play.

## Usage
1. In a Unity project that references `com.soobak.algo.sorting`, open the Package
   Manager and import the "Sorting Visualizer Demo" sample.
2. Drop `SortingDemoController` onto an empty GameObject (it requires
   `SortingBarVisualizer` and will add it automatically).
3. Adjust the array length, value range, and `algorithmId` in the inspector.
4. Enter Play mode to watch the selected sorting algorithm animate.

You can re-run the demo at any time via the component's context menu or by
calling `RunFromInspector()`.
