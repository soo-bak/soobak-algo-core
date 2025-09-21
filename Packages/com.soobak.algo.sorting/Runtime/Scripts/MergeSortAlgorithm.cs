using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class MergeSortAlgorithm : ISortingAlgorithm {
    public string Id => "merge-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        if (count <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Merge sort complete")), cancellationToken);
          return;
        }

        var buffer = new SortingItem[count];
        var sourceIndices = new int[count];
        await MergeSortAsync(state, sink, buffer, sourceIndices, 0, count - 1, cancellationToken);
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Merge sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"MergeSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"MergeSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }

    async UniTask MergeSortAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, SortingItem[] buffer, int[] sourceIndices, int left, int right, CancellationToken cancellationToken) {
      if (left >= right)
        return;

      cancellationToken.ThrowIfCancellationRequested();
      var mid = left + (right - left) / 2;

      await MergeSortAsync(state, sink, buffer, sourceIndices, left, mid, cancellationToken);
      await MergeSortAsync(state, sink, buffer, sourceIndices, mid + 1, right, cancellationToken);
      await MergeAsync(state, sink, buffer, sourceIndices, left, mid, right, cancellationToken);
    }

    async UniTask MergeAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, SortingItem[] buffer, int[] sourceIndices, int left, int mid, int right, CancellationToken cancellationToken) {
      var leftIndex = left;
      var rightIndex = mid + 1;
      var bufferIndex = 0;

      await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(left, $"Merge range [{left}, {right}]")), cancellationToken);

      while (leftIndex <= mid && rightIndex <= right) {
        cancellationToken.ThrowIfCancellationRequested();
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(leftIndex, rightIndex)), cancellationToken);

        if (state.Items[leftIndex].Value <= state.Items[rightIndex].Value) {
          var sourceIndex = leftIndex;
          buffer[bufferIndex] = state.Items[leftIndex].Clone();
          sourceIndices[bufferIndex++] = sourceIndex;
          leftIndex++;
        }
        else {
          var sourceIndex = rightIndex;
          buffer[bufferIndex] = state.Items[rightIndex].Clone();
          sourceIndices[bufferIndex++] = sourceIndex;
          rightIndex++;
        }
      }

      while (leftIndex <= mid) {
        cancellationToken.ThrowIfCancellationRequested();
        var sourceIndex = leftIndex;
        buffer[bufferIndex] = state.Items[leftIndex].Clone();
        sourceIndices[bufferIndex++] = sourceIndex;
        leftIndex++;
      }

      while (rightIndex <= right) {
        cancellationToken.ThrowIfCancellationRequested();
        var sourceIndex = rightIndex;
        buffer[bufferIndex] = state.Items[rightIndex].Clone();
        sourceIndices[bufferIndex++] = sourceIndex;
        rightIndex++;
      }

      for (var i = 0; i < bufferIndex; i++) {
        cancellationToken.ThrowIfCancellationRequested();
        var targetIndex = left + i;
        state.Replace(targetIndex, buffer[i].Clone());
        var sourceIndex = sourceIndices[i];
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Insert(sourceIndex, targetIndex, buffer[i])), cancellationToken);
      }
    }
  }
}
