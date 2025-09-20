using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class SortingRunner {
    readonly IReadOnlyList<IBarVisualizer> _visualizers;
    readonly AlgorithmRunner<SortingState, SortOp> _runner;

    public SortingRunner(IEnumerable<IBarVisualizer> visualizers) {
      if (visualizers == null)
        throw new ArgumentNullException(nameof(visualizers));

      var list = visualizers.ToList();
      if (list.Any(v => v == null))
        throw new ArgumentNullException(nameof(visualizers), "SortingRunner cannot include null visualizers.");

      _visualizers = new ReadOnlyCollection<IBarVisualizer>(list);
      _runner = new AlgorithmRunner<SortingState, SortOp>(CloneState, _visualizers);
    }

    public SortingRunner(params IBarVisualizer[] visualizers)
      : this((IEnumerable<IBarVisualizer>)(visualizers ?? Array.Empty<IBarVisualizer>())) {
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

    static SortingState CloneState(SortingState source) {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      return source.Clone();
    }
  }
}
