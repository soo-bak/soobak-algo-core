using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soobak.Algo.Core;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class HeapSortAlgorithm : ISortingAlgorithm {
    public string Id => "heap-sort";

    public async UniTask ExecuteAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      if (state == null)
        throw new ArgumentNullException(nameof(state));

      if (sink == null)
        throw new ArgumentNullException(nameof(sink));

      try {
        var count = state.Items.Count;
        if (count <= 1) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Heap sort complete")), cancellationToken);
          return;
        }

        await BuildMaxHeapAsync(state, sink, cancellationToken);

        for (var end = count - 1; end > 0; end--) {
          cancellationToken.ThrowIfCancellationRequested();

          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(0, "Swap heap root")), cancellationToken);

          state.Swap(0, end);
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(0, end)), cancellationToken);

          await SiftDownAsync(state, sink, 0, end - 1, cancellationToken);
        }

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Finalize("Heap sort complete")), cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"HeapSortAlgorithm: Execution cancelled. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"HeapSortAlgorithm: Execution failed. {ex}");
        throw;
      }
    }

    static async UniTask BuildMaxHeapAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, CancellationToken cancellationToken) {
      var count = state.Items.Count;
      for (var start = (count - 2) / 2; start >= 0; start--) {
        cancellationToken.ThrowIfCancellationRequested();
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(start, "Sift down")), cancellationToken);
        await SiftDownAsync(state, sink, start, count - 1, cancellationToken);
      }
    }

    static async UniTask SiftDownAsync(SortingState state, IAlgorithmStepSink<SortingState, SortOp> sink, int start, int end, CancellationToken cancellationToken) {
      var root = start;
      while (true) {
        cancellationToken.ThrowIfCancellationRequested();
        var leftChild = root * 2 + 1;
        if (leftChild > end)
          break;

        var swapIndex = root;
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Highlight(root, "Compare children")), cancellationToken);

        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(root, leftChild)), cancellationToken);
        if (state.Items[swapIndex].Value < state.Items[leftChild].Value)
          swapIndex = leftChild;

        var rightChild = leftChild + 1;
        if (rightChild <= end) {
          await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Compare(root, rightChild)), cancellationToken);
          if (state.Items[swapIndex].Value < state.Items[rightChild].Value)
            swapIndex = rightChild;
        }

        if (swapIndex == root)
          break;

        state.Swap(root, swapIndex);
        await sink.PublishAsync(new AlgorithmStep<SortingState, SortOp>(state.Clone(), SortOp.Swap(root, swapIndex)), cancellationToken);

        root = swapIndex;
      }
    }
  }
}
