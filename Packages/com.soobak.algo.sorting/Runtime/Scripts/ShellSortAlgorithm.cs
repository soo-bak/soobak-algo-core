using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class ShellSortAlgorithm : ISortingAlgorithm {
    public string Id => "shell-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        if (count <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Shell sort complete")), cancellationToken);
          return;
        }

        for (var gap = count / 2; gap > 0; gap /= 2) {
          for (var i = gap; i < count; i++) {
            cancellationToken.ThrowIfCancellationRequested();

            await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(i, $"Gap {gap} insertion")), cancellationToken);

            var current = state.Items[i].Clone();
            var position = i;

            while (position >= gap && state.Items[position - gap].Value > current.Value) {
              cancellationToken.ThrowIfCancellationRequested();

              await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(position - gap, position)), cancellationToken);

              var shifted = state.Items[position - gap].Clone();
              state.Replace(position, shifted);
              await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Insert(position - gap, position, shifted)), cancellationToken);

              position -= gap;
            }

            if (position != i) {
              state.Replace(position, current);
              await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Insert(i, position, current)), cancellationToken);
            }
          }
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Shell sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"ShellSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"ShellSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }
  }
}
