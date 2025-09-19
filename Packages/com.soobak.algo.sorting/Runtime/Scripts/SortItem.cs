using System;

namespace Soobak.Algo.Sorting {
  public readonly struct SortItem : IEquatable<SortItem> {
    public SortItem(int value, string label) {
      Value = value;
      Label = label ?? string.Empty;
    }

    public int Value { get; }

    public string Label { get; }

    public bool Equals(SortItem other) {
      return Value == other.Value && string.Equals(Label, other.Label, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) {
      return obj is SortItem other && Equals(other);
    }

    public override int GetHashCode() {
      return HashCode.Combine(Value, Label);
    }

    public override string ToString() {
      return $"{Value} ({Label})";
    }
  }
}
