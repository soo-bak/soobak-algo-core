using System.Collections.Generic;
using Soobak.Algo.Core.Pipeline;

namespace Soobak.Algo.Sorting {
  public sealed class SortingAlgorithmCatalog : IAlgorithmCatalog<SortingState, SortOp> {
    readonly AlgorithmCatalog<SortingState, SortOp> _catalog;

    public SortingAlgorithmCatalog()
      : this(CreateDefaultDescriptors()) {
    }

    public SortingAlgorithmCatalog(IEnumerable<IAlgorithmDescriptor<SortingState, SortOp>> descriptors) {
      _catalog = new AlgorithmCatalog<SortingState, SortOp>(descriptors);
    }

    public IReadOnlyList<IAlgorithmDescriptor<SortingState, SortOp>> Descriptors => _catalog.Descriptors;

    public bool TryGetDescriptor(string id, out IAlgorithmDescriptor<SortingState, SortOp> descriptor) {
      return _catalog.TryGetDescriptor(id, out descriptor);
    }

    static IEnumerable<IAlgorithmDescriptor<SortingState, SortOp>> CreateDefaultDescriptors() {
      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "bubble-sort",
        displayName: "Bubble Sort",
        description: "Stable bubble sort with early exit when no swaps occur.",
        factory: () => new BubbleSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n^2)" },
          { "complexity-best", "O(n)" },
          { "complexity-worst", "O(n^2)" },
          { "stability", "Stable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "insertion-sort",
        displayName: "Insertion Sort (Stable)",
        description: "Stable insertion sort that tracks comparisons and insert operations.",
        factory: () => new InsertionSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n^2)" },
          { "complexity-best", "O(n)" },
          { "complexity-worst", "O(n^2)" },
          { "stability", "Stable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "selection-sort",
        displayName: "Selection Sort",
        description: "Classic selection sort that highlights minimum selection before swapping.",
        factory: () => new SelectionSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n^2)" },
          { "complexity-best", "O(n^2)" },
          { "complexity-worst", "O(n^2)" },
          { "stability", "Unstable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "merge-sort",
        displayName: "Merge Sort (Stable)",
        description: "Stable merge sort that merges sorted partitions while preserving order of equals.",
        factory: () => new MergeSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n log n)" },
          { "complexity-best", "O(n log n)" },
          { "complexity-worst", "O(n log n)" },
          { "stability", "Stable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "heap-sort",
        displayName: "Heap Sort",
        description: "In-place heap sort using a max-heap built from the input sequence.",
        factory: () => new HeapSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n log n)" },
          { "complexity-best", "O(n log n)" },
          { "complexity-worst", "O(n log n)" },
          { "stability", "Unstable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "shell-sort",
        displayName: "Shell Sort",
        description: "Shell sort using halved gap sequence and in-place element shifting.",
        factory: () => new ShellSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n^(3/2))" },
          { "complexity-best", "O(n log n)" },
          { "complexity-worst", "O(n^(3/2))" },
          { "stability", "Unstable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "counting-sort",
        displayName: "Counting Sort",
        description: "Stable counting sort for integer keys using prefix sums.",
        factory: () => new CountingSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n + k)" },
          { "complexity-best", "O(n + k)" },
          { "complexity-worst", "O(n + k)" },
          { "stability", "Stable" }
        });

      yield return AlgorithmDescriptor.Create<SortingState, SortOp>(
        id: "quick-sort",
        displayName: "Quick Sort",
        description: "Quick sort using Lomuto partitioning with pivot highlighting.",
        factory: () => new QuickSortAlgorithm(),
        metadata: new Dictionary<string, string> {
          { "complexity-average", "O(n log n)" },
          { "complexity-best", "O(n log n)" },
          { "complexity-worst", "O(n^2)" },
          { "stability", "Unstable" }
        });
    }
  }
}
