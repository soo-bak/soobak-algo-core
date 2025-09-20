# Changelog

All notable changes to this project will be documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Algorithm pipeline contracts (`IAlgorithmDescriptor`, `IAlgorithmCatalog`, `AlgorithmPipeline`) for reusable execution flows.
- Sorting algorithm catalog with descriptor metadata and EditMode coverage for pipeline execution.
- Architecture documentation and README updates describing layering and execution flow.
- Selection sort implementation with catalog registration and EditMode regression tests.

### Changed
- SortingRunner now supports descriptor-driven execution via `ExecuteAsync(string, ...)` when a catalog is provided.

