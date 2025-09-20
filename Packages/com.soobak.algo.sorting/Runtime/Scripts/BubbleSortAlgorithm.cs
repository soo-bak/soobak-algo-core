using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class BubbleSortAlgorithm : ISortingAlgorithm {
    public string Id => "bubble-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var length = state.Items.Count;
        if (length <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Bubble sort complete")), cancellationToken);
          return;
        }

        for (var passEnd = length - 1; passEnd > 0; passEnd--) {
          cancellationToken.ThrowIfCancellationRequested();
          var swapped = false;

          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(passEnd, "Start bubble pass")), cancellationToken);

          for (var index = 0; index < passEnd; index++) {
            cancellationToken.ThrowIfCancellationRequested();

            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(index, index + 1)), cancellationToken);

            var left = state.Items[index];
            var right = state.Items[index + 1];
            if (left.Value > right.Value) {
              state.Swap(index, index + 1);
              swapped = true;
              await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(index, index + 1)), cancellationToken);
            }
          }

          if (!swapped)
            break;
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Bubble sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"BubbleSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"BubbleSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
