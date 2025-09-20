using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class RadixSortAlgorithm : ISortingAlgorithm {
    public string Id => "radix-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        if (count <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Radix sort complete")), cancellationToken);
          return;
        }

        var min = int.MaxValue;
        var max = int.MinValue;
        foreach (var item in state.Items) {
          if (item.Value < 0)
            throw new NotSupportedException("RadixSortAlgorithm currently supports non-negative integers only.");
          if (item.Value < min)
            min = item.Value;
          if (item.Value > max)
            max = item.Value;
        }

        var output = new SortingItem[count];
        var counts = new int[10];
        var exp = 1;

        while (max / exp > 0) {
          cancellationToken.ThrowIfCancellationRequested();
          Array.Clear(counts, 0, counts.Length);

          for (var i = 0; i < count; i++) {
            cancellationToken.ThrowIfCancellationRequested();
            var value = state.Items[i].Value;
            var digit = (value / exp) % 10;
            counts[digit]++;
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, $"Count digit {digit} (exp {exp})")), cancellationToken);
          }

          for (var i = 1; i < counts.Length; i++) {
            cancellationToken.ThrowIfCancellationRequested();
            counts[i] += counts[i - 1];
          }

          for (var i = count - 1; i >= 0; i--) {
            cancellationToken.ThrowIfCancellationRequested();
            var item = state.Items[i];
            var digit = (item.Value / exp) % 10;
            counts[digit]--;
            var targetIndex = counts[digit];
            output[targetIndex] = item.Clone();
          }

          for (var i = 0; i < count; i++) {
            cancellationToken.ThrowIfCancellationRequested();
            state.Replace(i, output[i].Clone());
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, $"Write digit pass (exp {exp})")), cancellationToken);
          }

          exp *= 10;
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Radix sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"RadixSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"RadixSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
