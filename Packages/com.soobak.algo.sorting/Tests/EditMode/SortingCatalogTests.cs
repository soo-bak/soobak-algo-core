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
    public void Catalog_TryGetDescriptor_FailsForUnknownId() {
      var catalog = new SortingAlgorithmCatalog();

      var result = catalog.TryGetDescriptor("heap-sort", out var descriptor);

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
