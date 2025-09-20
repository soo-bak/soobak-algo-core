using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using Soobak.Algo.Core.Pipeline;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class SortingRunner {
    readonly IReadOnlyList<IBarVisualizer> _visualizers;
    readonly AlgorithmRunner<SortingState, SortOp> _runner;
    readonly AlgorithmPipeline<SortingState, SortOp>? _pipeline;

    public SortingRunner(IEnumerable<IBarVisualizer> visualizers, IAlgorithmCatalog<SortingState, SortOp>? catalog = null) {
      if (visualizers == null)
        throw new ArgumentNullException(nameof(visualizers));

      var list = visualizers.ToList();
      if (list.Any(v => v == null))
        throw new ArgumentNullException(nameof(visualizers), "SortingRunner cannot include null visualizers.");

      _visualizers = new ReadOnlyCollection<IBarVisualizer>(list);
      _runner = new AlgorithmRunner<SortingState, SortOp>(CloneState, _visualizers);
      _pipeline = catalog != null ? new AlgorithmPipeline<SortingState, SortOp>(CloneState, _visualizers, catalog.Descriptors) : null;
    }

    public SortingRunner(IBarVisualizer visualizer, IAlgorithmCatalog<SortingState, SortOp>? catalog = null)
      : this(new[] { visualizer ?? throw new ArgumentNullException(nameof(visualizer)) }, catalog) {
    }

    public SortingRunner(params IBarVisualizer[] visualizers)
      : this((IEnumerable<IBarVisualizer>)(visualizers ?? Array.Empty<IBarVisualizer>())) {
    }

    public IReadOnlyList<IAlgorithmDescriptor<SortingState, SortOp>> Descriptors => _pipeline?.Descriptors ?? Array.Empty<IAlgorithmDescriptor<SortingState, SortOp>>();

    public bool TryGetDescriptor(string id, out IAlgorithmDescriptor<SortingState, SortOp> descriptor) {
      if (_pipeline == null) {
        descriptor = default!;
        return false;
      }

      return _pipeline.TryGetDescriptor(id, out descriptor);
    }

    public async UniTask<SortingState> ExecuteAsync(ISortingAlgorithm algorithm, SortingState initialState, CancellationToken cancellationToken) {
      if (algorithm == null)
        throw new ArgumentNullException(nameof(algorithm));

      if (initialState == null)
        throw new ArgumentNullException(nameof(initialState));

      try {
        return await _runner.ExecuteAsync(algorithm, initialState, cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"SortingRunner: Execution cancelled for algorithm {algorithm.Id}. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"SortingRunner: Execution failed for algorithm {algorithm.Id}. {ex}");
        throw;
      }
    }

    public async UniTask<SortingState> ExecuteAsync(string algorithmId, SortingState initialState, CancellationToken cancellationToken) {
      if (_pipeline == null)
        throw new InvalidOperationException("SortingRunner: Catalog not configured. Provide an IAlgorithmCatalog to enable descriptor-based execution.");

      return await _pipeline.ExecuteAsync(algorithmId, initialState, cancellationToken);
    }

    static SortingState CloneState(SortingState source) {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      return source.Clone();
    }
  }
}
