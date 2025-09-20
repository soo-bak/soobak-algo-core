using System;
using System.Collections.Generic;
using System.Linq;

namespace Soobak.Algo.Sorting {
  public sealed class SortingState {
    readonly List<SortingItem> _items;

    public SortingState(IEnumerable<SortingItem> items) {
      if (items == null)
        throw new ArgumentNullException(nameof(items));

      _items = new List<SortingItem>();
      foreach (var item in items) {
        if (item == null)
          throw new ArgumentNullException(nameof(items), "SortingState cannot include null items.");
        _items.Add(item.Clone());
      }
    }

    public IReadOnlyList<SortingItem> Items => _items;

    public static SortingState FromValues(IEnumerable<int> values) {
      if (values == null)
        throw new ArgumentNullException(nameof(values));

      var items = values.Select((value, index) => new SortingItem(value, $"item-{index:D4}"));
      return new SortingState(items);
    }

    public static SortingState FromLabeledValues(IEnumerable<(int value, string stableId)> items) {
      if (items == null)
        throw new ArgumentNullException(nameof(items));

      return new SortingState(items.Select(tuple => new SortingItem(tuple.value, tuple.stableId)));
    }

    public SortingState Clone() {
      return new SortingState(_items);
    }

    public void Replace(int index, SortingItem item) {
      if (index < 0 || index >= _items.Count)
        throw new ArgumentOutOfRangeException(nameof(index));

      if (item == null)
        throw new ArgumentNullException(nameof(item));

      _items[index] = item;
    }

    public void Move(int sourceIndex, int targetIndex) {
      if (sourceIndex < 0 || sourceIndex >= _items.Count)
        throw new ArgumentOutOfRangeException(nameof(sourceIndex));

      if (targetIndex < 0 || targetIndex > _items.Count)
        throw new ArgumentOutOfRangeException(nameof(targetIndex));

      var item = _items[sourceIndex];
      _items.RemoveAt(sourceIndex);
      _items.Insert(targetIndex, item);
    }

    public void Swap(int leftIndex, int rightIndex) {
      if (leftIndex < 0 || leftIndex >= _items.Count)
        throw new ArgumentOutOfRangeException(nameof(leftIndex));

      if (rightIndex < 0 || rightIndex >= _items.Count)
        throw new ArgumentOutOfRangeException(nameof(rightIndex));

      (_items[leftIndex], _items[rightIndex]) = (_items[rightIndex], _items[leftIndex]);
    }
  }
}
