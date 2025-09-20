using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class QuickSortAlgorithm : ISortingAlgorithm {
    public string Id => "quick-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        await QuickSortAsync(state, sink, 0, state.Items.Count - 1, cancellationToken);
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Quick sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"QuickSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"QuickSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }

    async UniTask QuickSortAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, int left, int right, CancellationToken cancellationToken) {
      if (left >= right)
        return;

      cancellationToken.ThrowIfCancellationRequested();
      var pivotIndex = await PartitionAsync(state, sink, left, right, cancellationToken);
      await QuickSortAsync(state, sink, left, pivotIndex - 1, cancellationToken);
      await QuickSortAsync(state, sink, pivotIndex + 1, right, cancellationToken);
    }

    async UniTask<int> PartitionAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, int left, int right, CancellationToken cancellationToken) {
      var pivotItem = state.Items[right];
      var storeIndex = left;

      await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Pivot(right)), cancellationToken);

      for (var i = left; i < right; i++) {
        cancellationToken.ThrowIfCancellationRequested();
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(i, right)), cancellationToken);

        if (state.Items[i].Value <= pivotItem.Value) {
          if (i != storeIndex) {
            state.Swap(i, storeIndex);
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(i, storeIndex)), cancellationToken);
          }

          storeIndex++;
        }
      }

      if (storeIndex != right) {
        state.Swap(storeIndex, right);
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(storeIndex, right)), cancellationToken);
      }

      await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Partition(left, right)), cancellationToken);
      return storeIndex;
    }
  }
}
