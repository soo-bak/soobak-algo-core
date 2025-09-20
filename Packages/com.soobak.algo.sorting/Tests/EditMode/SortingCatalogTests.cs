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
    public void Catalog_TryGetDescriptor_FailsForUnknownId() {
      var catalog = new SortingAlgorithmCatalog();

      var result = catalog.TryGetDescriptor("merge-sort", out var descriptor);

      Assert.That(result, Is.False);
      Assert.That(descriptor, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_ByDescriptorId_RunsPipelineAndSorts() {
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
