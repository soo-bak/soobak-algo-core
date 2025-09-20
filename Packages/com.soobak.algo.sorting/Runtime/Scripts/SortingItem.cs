using System;

namespace Soobak.Algo.Sorting {
  public sealed class SortingItem : IEquatable<SortingItem> {
    public SortingItem(int value, string stableId) {
      Value = value;
      StableId = stableId ?? string.Empty;
    }

    public int Value { get; }

    public string StableId { get; }

    public SortingItem Clone() {
      return new SortingItem(Value, StableId);
    }

    public bool Equals(SortingItem other) {
      if (other == null)
        return false;

      return Value == other.Value && string.Equals(StableId, other.StableId, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) {
      return obj is SortingItem other && Equals(other);
    }

    public override int GetHashCode() {
      return HashCode.Combine(Value, StableId);
    }

    public override string ToString() {
      return $"{Value} ({StableId})";
    }
  }
}
