using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;

namespace Soobak.Algo.Sorting {
  public sealed class SortingRunner {
    readonly AlgorithmRunner<List<SortItem>, SortOp> _runner;

    public SortingRunner(ISortingAlgorithm algorithm, IBarVisualizer visualizer) {
      if (algorithm == null)
        throw new ArgumentNullException(nameof(algorithm));

      if (visualizer == null)
        throw new ArgumentNullException(nameof(visualizer));

      _runner = new AlgorithmRunner<List<SortItem>, SortOp>(
        algorithm,
        new VisualizerAdapter(visualizer),
        Clone);
    }

    public async UniTask<IReadOnlyList<SortItem>> RunAsync(IReadOnlyList<SortItem> input, CancellationToken cancellationToken = default) {
      if (input == null)
        throw new ArgumentNullException(nameof(input));

      var initial = input.Select(item => new SortItem(item.Value, item.Label)).ToList();
      var result = await _runner.RunAsync(initial, cancellationToken);
      return new ReadOnlyCollection<SortItem>(result);
    }

    static List<SortItem> Clone(List<SortItem> source) {
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      return source.Select(item => new SortItem(item.Value, item.Label)).ToList();
    }

    sealed class VisualizerAdapter : IAlgorithmVisualizer<List<SortItem>, SortOp> {
      readonly IBarVisualizer _inner;

      public VisualizerAdapter(IBarVisualizer inner) {
        _inner = inner;
      }

      public UniTask InitializeAsync(List<SortItem> state, CancellationToken cancellationToken) {
        return _inner.InitializeAsync(new ReadOnlyCollection<SortItem>(state), cancellationToken);
      }

      public UniTask ApplyAsync(SortOp op, List<SortItem> state, CancellationToken cancellationToken) {
        return _inner.ApplyAsync(op, new ReadOnlyCollection<SortItem>(state), cancellationToken);
      }

      public UniTask CompleteAsync(List<SortItem> state, CancellationToken cancellationToken) {
        return _inner.CompleteAsync(new ReadOnlyCollection<SortItem>(state), cancellationToken);
      }
    }
  }
}
