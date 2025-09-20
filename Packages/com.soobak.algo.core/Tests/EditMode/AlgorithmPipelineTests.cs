using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Soobak.Algo.Core;
using Soobak.Algo.Core.Pipeline;
using UnityEngine;
using UnityEngine.TestTools;

namespace Soobak.Algo.Core.Tests {
  public class AlgorithmPipelineTests {
    [Test]
    public void CatalogConstructor_DuplicateIds_ThrowsArgumentException() {
      var first = AlgorithmDescriptor.Create<List<int>, string>("duplicate", "First", string.Empty, () => new StubAlgorithm("first"));
      var second = AlgorithmDescriptor.Create<List<int>, string>("duplicate", "Second", string.Empty, () => new StubAlgorithm("second"));

      Assert.Throws<ArgumentException>(() => new AlgorithmCatalog<List<int>, string>(new[] { first, second }));
    }

    [Test]
    public void CatalogConstructor_NullDescriptor_ThrowsArgumentNullException() {
      Assert.Throws<ArgumentNullException>(() => new AlgorithmCatalog<List<int>, string>(new IAlgorithmDescriptor<List<int>, string>[] { null! }));
    }

    [Test]
    public void TryGetDescriptor_WithEmptyId_ReturnsFalse() {
      var descriptor = AlgorithmDescriptor.Create<List<int>, string>("valid", "Valid", string.Empty, () => new StubAlgorithm("valid"));
      var catalog = new AlgorithmCatalog<List<int>, string>(new[] { descriptor });

      Assert.That(catalog.TryGetDescriptor(" ", out _), Is.False);
    }

    [Test]
    public void PipelineConstructor_DuplicateIds_ThrowsArgumentException() {
      var first = AlgorithmDescriptor.Create<List<int>, string>("duplicate", "First", string.Empty, () => new StubAlgorithm("first"));
      var second = AlgorithmDescriptor.Create<List<int>, string>("duplicate", "Second", string.Empty, () => new StubAlgorithm("second"));
      var runner = new AlgorithmRunner<List<int>, string>(CloneState, Array.Empty<IAlgorithmStepSink<List<int>, string>>());

      Assert.Throws<ArgumentException>(() => new AlgorithmPipeline<List<int>, string>(runner, new[] { first, second }));
    }

    [Test]
    public void PipelineConstructor_NullDescriptor_ThrowsArgumentNullException() {
      var runner = new AlgorithmRunner<List<int>, string>(CloneState, Array.Empty<IAlgorithmStepSink<List<int>, string>>());

      Assert.Throws<ArgumentNullException>(() => new AlgorithmPipeline<List<int>, string>(runner, new IAlgorithmDescriptor<List<int>, string>[] { null! }));
    }

    [Test]
    public void ExecuteAsync_UnknownId_ThrowsKeyNotFoundException() {
      var descriptor = AlgorithmDescriptor.Create<List<int>, string>("known", "Known", string.Empty, () => new StubAlgorithm("known"));
      var pipeline = CreatePipeline(descriptor);

      Assert.ThrowsAsync<KeyNotFoundException>(
        async () => await pipeline.ExecuteAsync("missing", new List<int> { 1 }, CancellationToken.None));
    }

    [Test]
    public void ExecuteAsync_FactoryThrows_PropagatesException() {
      var descriptor = AlgorithmDescriptor.Create<List<int>, string>("faulty", "Faulty", string.Empty, () => throw new InvalidOperationException("factory failure"));
      var pipeline = CreatePipeline(descriptor);

      LogAssert.Expect(LogType.Error, new Regex("^AlgorithmPipeline: Factory failed for algorithm faulty\\.", RegexOptions.Singleline));
      Assert.ThrowsAsync<InvalidOperationException>(
        async () => await pipeline.ExecuteAsync("faulty", new List<int> { 1 }, CancellationToken.None));
    }

    [Test]
    public void ExecuteAsync_FactoryReturnsNull_ThrowsInvalidOperationException() {
      var descriptor = AlgorithmDescriptor.Create<List<int>, string>("null-algorithm", "Null Algorithm", string.Empty, () => null!);
      var pipeline = CreatePipeline(descriptor);

      Assert.ThrowsAsync<InvalidOperationException>(
        async () => await pipeline.ExecuteAsync("null-algorithm", new List<int> { 1 }, CancellationToken.None));
    }

    static AlgorithmPipeline<List<int>, string> CreatePipeline(IAlgorithmDescriptor<List<int>, string> descriptor) {
      var runner = new AlgorithmRunner<List<int>, string>(CloneState, Array.Empty<IAlgorithmStepSink<List<int>, string>>());
      return new AlgorithmPipeline<List<int>, string>(runner, new[] { descriptor });
    }

    static List<int> CloneState(List<int> source) {
      return source?.ToList() ?? throw new ArgumentNullException(nameof(source));
    }

    sealed class StubAlgorithm : IAlgorithm<List<int>, string> {
      public StubAlgorithm(string id) {
        Id = id;
      }

      public string Id { get; }

      public UniTask ExecuteAsync(List<int> state, IAlgorithmStepSink<List<int>, string> sink, CancellationToken cancellationToken) {
        return UniTask.CompletedTask;
      }
    }
  }
}
