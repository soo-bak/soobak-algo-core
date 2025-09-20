# Architecture Details

## Layer Responsibilities
- **Core Primitives**: Split algorithm execution (`IAlgorithm<TState, TEvent>`) from step sinks (`IAlgorithmStepSink<TState, TEvent>`) while `AlgorithmRunner<TState, TEvent>` clones state snapshots before broadcasting.
- **Core Pipeline**: Descriptors (`IAlgorithmDescriptor<TState, TEvent>`) and catalogs (`IAlgorithmCatalog<TState, TEvent>`) declare metadata plus factories; `AlgorithmPipeline<TState, TEvent>` receives a state cloner and sinks so it focuses solely on orchestrating execution.
- **Domain Packages**: Each domain (e.g., `com.soobak.algo.sorting`) implements the contracts, defines state/event models, and provides domain-specific visualizers.

## Execution Contracts
1. **Descriptor authoring**: Domain packages call `AlgorithmDescriptor.Create` to register IDs, display names, descriptions, factories, and metadata.
2. **Catalog aggregation**: Domain catalogs collect descriptors and delegate to `AlgorithmCatalog<TState, TEvent>` for validation and lookup.
3. **Pipeline composition**: At runtime compose `AlgorithmPipeline<TState, TEvent>` with a state cloner (`Func<TState, TState>`) plus visualizer sinks, then call `ExecuteAsync(id, state, token)`.
4. **Runner integration**: Domain runners (e.g., `SortingRunner`) continue to support direct algorithm invocation while exposing descriptor-driven execution when a catalog is provided.

## Extension Strategy
- Add new algorithms by introducing domain-specific state/event types and wiring their implementations into descriptor factories.
- Additional domains (graph traversal, pathfinding, etc.) can reuse the pipeline while shipping their own catalogs and runners.
- Centralized logging and cancellation handling in the core keeps domain implementations focused on correctness concerns such as stability or ordering.

## Testing Guidance
- Unit-test the core pipeline for duplicate IDs, null-guard behavior, and factory failures.
- Domain tests should cover algorithm outputs, descriptor metadata, and parity between direct and descriptor-based execution paths.
- Longer term, add PlayMode tests to confirm `IAlgorithmStepSink` implementations respect visual pacing.

## Demo Integration Checklist
- In the demo repo, bind `SortingAlgorithmCatalog().Descriptors` to UI for algorithm selection.
- Inject concrete `IBarVisualizer` implementations into `SortingRunner` and keep `AlgorithmStep` payloads serialization-friendly for WebGL.
- Reuse the existing EditMode CI job and add PlayMode coverage tailored to the demo as it matures.