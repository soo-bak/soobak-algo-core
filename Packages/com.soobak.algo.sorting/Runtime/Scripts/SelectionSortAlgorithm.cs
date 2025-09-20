using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class SelectionSortAlgorithm : ISortingAlgorithm {
    public string Id => "selection-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        for (var i = 0; i < count - 1; i++) {
          cancellationToken.ThrowIfCancellationRequested();

          var minIndex = i;
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, "Select pivot")), cancellationToken);

          for (var j = i + 1; j < count; j++) {
            cancellationToken.ThrowIfCancellationRequested();
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(minIndex, j)), cancellationToken);

            if (state.Items[j].Value < state.Items[minIndex].Value)
              minIndex = j;
          }

          if (minIndex != i) {
            state.Swap(i, minIndex);
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(i, minIndex)), cancellationToken);
          }
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Selection sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"SelectionSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"SelectionSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
