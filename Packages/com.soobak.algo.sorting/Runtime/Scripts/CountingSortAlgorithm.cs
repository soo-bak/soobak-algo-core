using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class CountingSortAlgorithm : ISortingAlgorithm {
    public string Id => "counting-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        if (count <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Counting sort complete")), cancellationToken);
          return;
        }

        var min = state.Items[0].Value;
        var max = min;
        for (var i = 1; i < count; i++) {
          cancellationToken.ThrowIfCancellationRequested();
          var value = state.Items[i].Value;
          if (value < min)
            min = value;
          if (value > max)
            max = value;
        }

        var range = max - min + 1;
        if (range <= 0)
          throw new InvalidOperationException("CountingSortAlgorithm: Computed invalid value range.");

        var counts = new int[range];
        for (var i = 0; i < count; i++) {
          cancellationToken.ThrowIfCancellationRequested();
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, "Count value")), cancellationToken);
          counts[state.Items[i].Value - min]++;
        }

        for (var i = 1; i < range; i++) {
          cancellationToken.ThrowIfCancellationRequested();
          counts[i] += counts[i - 1];
        }

        var output = new SortingItem[count];
        for (var i = count - 1; i >= 0; i--) {
          cancellationToken.ThrowIfCancellationRequested();
          var item = state.Items[i];
          var bucket = item.Value - min;
          counts[bucket]--;
          var targetIndex = counts[bucket];
          output[targetIndex] = item.Clone();
        }

        for (var i = 0; i < count; i++) {
          cancellationToken.ThrowIfCancellationRequested();
          state.Replace(i, output[i].Clone());
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, "Write sorted value")), cancellationToken);
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Counting sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"CountingSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"CountingSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
