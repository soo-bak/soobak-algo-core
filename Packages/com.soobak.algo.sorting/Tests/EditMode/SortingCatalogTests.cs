using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;
using Soobak.Algo.Core.Pipeline;

namespace Soobak.Algo.Sorting.Tests {
  public class SortingCatalogTests {
    [Test]
    public void Catalog_ProvidesInsertionSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "insertion-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "insertion-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Insertion Sort (Stable)"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Stable"));
      Assert.That(descriptor.Metadata["complexity-average"], Is.EqualTo("O(n^2)"));
    }

    [Test]
    public void Catalog_ProvidesBubbleSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "bubble-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "bubble-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Bubble Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Stable"));
      Assert.That(descriptor.Metadata["complexity-best"], Is.EqualTo("O(n)"));
    }

    [Test]
    public void Catalog_ProvidesSelectionSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "selection-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "selection-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Selection Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Unstable"));
      Assert.That(descriptor.Metadata["complexity-worst"], Is.EqualTo("O(n^2)"));
    }

    [Test]
    public void Catalog_ProvidesMergeSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "merge-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "merge-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Merge Sort (Stable)"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Stable"));
      Assert.That(descriptor.Metadata["complexity-average"], Is.EqualTo("O(n log n)"));
    }

    [Test]
    public void Catalog_ProvidesQuickSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "quick-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "quick-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Quick Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Unstable"));
      Assert.That(descriptor.Metadata["complexity-worst"], Is.EqualTo("O(n^2)"));
    }

    [Test]
    public void Catalog_ProvidesHeapSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "heap-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "heap-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Heap Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Unstable"));
      Assert.That(descriptor.Metadata["complexity-average"], Is.EqualTo("O(n log n)"));
    }

    [Test]
    public void Catalog_ProvidesShellSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "shell-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "shell-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Shell Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Unstable"));
      Assert.That(descriptor.Metadata["complexity-average"], Is.EqualTo("O(n^(3/2))"));
    }

    [Test]
    public void Catalog_ProvidesCountingSortDescriptor() {
      var catalog = new SortingAlgorithmCatalog();

      Assert.That(catalog.Descriptors.Any(d => d.Id == "counting-sort"), Is.True);
      var descriptor = catalog.Descriptors.Single(d => d.Id == "counting-sort");
      Assert.That(descriptor.DisplayName, Is.EqualTo("Counting Sort"));
      Assert.That(descriptor.Metadata["stability"], Is.EqualTo("Stable"));
      Assert.That(descriptor.Metadata["complexity-average"], Is.EqualTo("O(n + k)"));
    }

    [Test]
    public void Catalog_TryGetDescriptor_FailsForUnknownId() {
      var catalog = new SortingAlgorithmCatalog();

      var result = catalog.TryGetDescriptor("radix-sort", out var descriptor);

      Assert.That(result, Is.False);
      Assert.That(descriptor, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_InsertionSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 4, 2, 3, 1 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("insertion-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 4, 2, 3, 1 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 4 }));
    }

    [Test]
    public async Task ExecuteAsync_BubbleSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 5, 1, 4, 2 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("bubble-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 5, 1, 4, 2 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 4, 5 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 4, 5 }));
    }

    [Test]
    public async Task ExecuteAsync_MergeSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 7, 3, 6, 2 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("merge-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 7, 3, 6, 2 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 3, 6, 7 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 3, 6, 7 }));
    }

    [Test]
    public async Task ExecuteAsync_QuickSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 9, 4, 6, 2 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("quick-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 9, 4, 6, 2 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 4, 6, 9 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 4, 6, 9 }));
    }

    [Test]
    public async Task ExecuteAsync_HeapSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 7, 1, 5, 3 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("heap-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 7, 1, 5, 3 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 3, 5, 7 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 3, 5, 7 }));
    }

    [Test]
    public async Task ExecuteAsync_ShellSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 10, 6, 8, 2, 4 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("shell-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 10, 6, 8, 2, 4 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 4, 6, 8, 10 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 2, 4, 6, 8, 10 }));
    }

    [Test]
    public async Task ExecuteAsync_CountingSortDescriptor_RunsPipelineAndSorts() {
      var catalog = new SortingAlgorithmCatalog();
      var visualizer = new RecordingVisualizer();
      var runner = new SortingRunner(visualizer, catalog);
      var initial = SortingState.FromValues(new[] { 5, 3, 5, 2, 1 });
      var original = initial.Clone();

      var result = await runner.ExecuteAsync("counting-sort", initial, CancellationToken.None);

      Assert.That(original.Items.Select(item => item.Value), Is.EqualTo(new[] { 5, 3, 5, 2, 1 }));
      Assert.That(result.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 5, 5 }));
      Assert.That(visualizer.Events.Count, Is.GreaterThan(0));
      Assert.That(visualizer.CompletedSnapshot.Items.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3, 5, 5 }));
    }

    sealed class RecordingVisualizer : IBarVisualizer {
      public SortingState InitialSnapshot { get; private set; } = SortingState.FromValues(Array.Empty<int>());
      public SortingState CompletedSnapshot { get; private set; } = SortingState.FromValues(Array.Empty<int>());
      public System.Collections.Generic.List<AlgorithmStep<SortingState, SortOp>> Events { get; } = new();

      public UniTask InitializeAsync(SortingState initialState, CancellationToken cancellationToken) {
        InitialSnapshot = initialState.Clone();
        return UniTask.CompletedTask;
      }

      public UniTask PublishAsync(AlgorithmStep<SortingState, SortOp> step, CancellationToken cancellationToken) {
        Events.Add(step);
        return UniTask.CompletedTask;
      }

      public UniTask CompleteAsync(SortingState finalState, CancellationToken cancellationToken) {
        CompletedSnapshot = finalState.Clone();
        return UniTask.CompletedTask;
      }
    }
  }
}
