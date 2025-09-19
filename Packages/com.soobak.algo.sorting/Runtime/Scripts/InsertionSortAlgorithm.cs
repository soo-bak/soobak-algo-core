using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Soobak.Algo.Sorting {
  public sealed class InsertionSortAlgorithm : ISortingAlgorithm {
    public string Id => "insertion-sort";

    public async UniTask ExecuteAsync(List<SortItem> items, Func<SortOp, UniTask> publishEvent, CancellationToken cancellationToken) {
      if (items == null)
        throw new ArgumentNullException(nameof(items));

      if (publishEvent == null)
        throw new ArgumentNullException(nameof(publishEvent));

      for (var i = 1; i < items.Count; i++) {
        cancellationToken.ThrowIfCancellationRequested();

        var key = items[i];
        var j = i - 1;

        await publishEvent(SortOp.Highlight(i));

        while (j >= 0) {
          cancellationToken.ThrowIfCancellationRequested();

          await publishEvent(SortOp.Compare(j, j + 1));

          if (items[j].Value <= key.Value)
            break;

          var shifted = items[j];
          items[j + 1] = shifted;
          await publishEvent(SortOp.Shift(j, j + 1, shifted));
          j--;
        }

        var insertIndex = j + 1;
        items[insertIndex] = key;
        await publishEvent(SortOp.Insert(insertIndex, key));
      }

      Debug.Log($"InsertionSortAlgorithm: Finished sorting {items.Count} items.");
    }
  }
}
