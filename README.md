# Soobak Algorithm Core & Sorting

Core delivers shared primitives for algorithm runners and step sinks. Sorting extends them with stable algorithms and bar visualizers.

## Architecture
- `com.soobak.algo.core`: `IAlgorithm<TState, TEvent>` contracts plus `AlgorithmRunner<TState, TEvent>` for cloning snapshots and broadcasting steps.
- `com.soobak.algo.sorting`: `SortingState`/`SortingItem`, `SortOp`, `SortingRunner`, and a reference `InsertionSortAlgorithm` built on the core layer.

## Usage
1. Add Git UPM dependencies to the consumer project manifest.
   ```json
   {
     "dependencies": {
       "com.soobak.algo.core": "https://github.com/soobak-algo/soobak-algo-core.git",
       "com.soobak.algo.sorting": "https://github.com/soobak-algo/soobak-algo-core.git?path=Packages/com.soobak.algo.sorting"
     }
   }
   ```
2. Compose a `SortingRunner` with one or more `IBarVisualizer` instances and execute the selected algorithm.
3. Clone `SortingState` before publishing steps so visualizers receive immutable snapshots.

## Testing Philosophy
- EditMode tests cover deterministic logic; future PlayMode specs will validate visual timing.
- Async flows use UniTask and always log cancellations or errors through `Debug`.

## Continuous Integration
- `.github/workflows/unity-editmode.yml` runs EditMode suites via `game-ci/unity-test-runner@v4` and archives coverage.

## Roadmap
- Add Merge Sort and Quick Sort modules.
- Publish additional algorithm packages (e.g., graphs) that reuse the core runner.
- Document Git UPM consumption from the demo repository.
