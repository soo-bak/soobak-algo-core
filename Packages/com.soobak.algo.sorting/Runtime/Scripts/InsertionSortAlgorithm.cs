using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class InsertionSortAlgorithm : ISortingAlgorithm {
    public string Id => "insertion-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        for (var i = 1; i < state.Items.Count; i++) {
          cancellationToken.ThrowIfCancellationRequested();

          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, "Start insertion")), cancellationToken);

          var current = state.Items[i];
          var targetIndex = i;

          while (targetIndex > 0 && state.Items[targetIndex - 1].Value > current.Value) {
            cancellationToken.ThrowIfCancellationRequested();

            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(targetIndex - 1, targetIndex)), cancellationToken);
            targetIndex--;
          }

          if (targetIndex != i) {
            state.Move(i, targetIndex);
            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Insert(i, targetIndex, current)), cancellationToken);
          }
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Insertion sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"InsertionSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"InsertionSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
