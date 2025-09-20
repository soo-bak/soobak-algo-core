using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Soobak.Algo.Core.Pipeline {
  public sealed class AlgorithmPipeline<TState, TEvent> {
    readonly AlgorithmRunner<TState, TEvent> _runner;
    readonly Dictionary<string, IAlgorithmDescriptor<TState, TEvent>> _descriptors;

    public AlgorithmPipeline(
      Func<TState, TState> stateCloner,
      IEnumerable<IAlgorithmStepSink<TState, TEvent>> sinks,
      IEnumerable<IAlgorithmDescriptor<TState, TEvent>> descriptors)
      : this(new AlgorithmRunner<TState, TEvent>(stateCloner, ToSinkList(sinks)), descriptors) {
    }

    public AlgorithmPipeline(
      AlgorithmRunner<TState, TEvent> runner,
      IEnumerable<IAlgorithmDescriptor<TState, TEvent>> descriptors) {
      _runner = runner ?? throw new ArgumentNullException(nameof(runner));

      if (descriptors == null)
        throw new ArgumentNullException(nameof(descriptors));

      _descriptors = new Dictionary<string, IAlgorithmDescriptor<TState, TEvent>>(StringComparer.Ordinal);
      var descriptorList = new List<IAlgorithmDescriptor<TState, TEvent>>();

      foreach (var descriptor in descriptors) {
        if (descriptor == null)
          throw new ArgumentNullException(nameof(descriptors), "AlgorithmPipeline cannot include null descriptors.");

        if (_descriptors.ContainsKey(descriptor.Id))
          throw new ArgumentException($"AlgorithmPipeline duplicate descriptor id detected: {descriptor.Id}", nameof(descriptors));

        _descriptors.Add(descriptor.Id, descriptor);
        descriptorList.Add(descriptor);
      }

      Descriptors = descriptorList.AsReadOnly();
    }

    public IReadOnlyList<IAlgorithmDescriptor<TState, TEvent>> Descriptors { get; }

    public bool TryGetDescriptor(string id, out IAlgorithmDescriptor<TState, TEvent> descriptor) {
      if (string.IsNullOrWhiteSpace(id)) {
        descriptor = default!;
        return false;
      }

      return _descriptors.TryGetValue(id, out descriptor!);
    }

    public async UniTask<TState> ExecuteAsync(string algorithmId, TState initialState, CancellationToken cancellationToken) {
      if (string.IsNullOrWhiteSpace(algorithmId))
        throw new ArgumentException("AlgorithmPipeline requires a non-empty algorithm id.", nameof(algorithmId));

      if (initialState == null)
        throw new ArgumentNullException(nameof(initialState));

      if (!_descriptors.TryGetValue(algorithmId, out var descriptor))
        throw new KeyNotFoundException($"AlgorithmPipeline cannot locate algorithm id {algorithmId}.");

      IAlgorithm<TState, TEvent> algorithm;
      try {
        algorithm = descriptor.Factory();
      }
      catch (Exception ex) {
        Debug.LogError($"AlgorithmPipeline: Factory failed for algorithm {descriptor.Id}. {ex}");
        throw;
      }

      if (algorithm == null)
        throw new InvalidOperationException($"AlgorithmPipeline descriptor {descriptor.Id} produced a null algorithm instance.");

      try {
        return await _runner.ExecuteAsync(algorithm, initialState, cancellationToken);
      }
      catch (OperationCanceledException ex) {
        Debug.LogWarning($"AlgorithmPipeline: Execution cancelled for algorithm {descriptor.Id}. {ex.Message}");
        throw;
      }
      catch (Exception ex) {
        Debug.LogError($"AlgorithmPipeline: Execution failed for algorithm {descriptor.Id}. {ex}");
        throw;
      }
    }

    static IReadOnlyList<IAlgorithmStepSink<TState, TEvent>> ToSinkList(IEnumerable<IAlgorithmStepSink<TState, TEvent>> sinks) {
      if (sinks == null)
        throw new ArgumentNullException(nameof(sinks));

      var list = new List<IAlgorithmStepSink<TState, TEvent>>();
      foreach (var sink in sinks) {
        if (sink == null)
          throw new ArgumentNullException(nameof(sinks), "AlgorithmPipeline cannot include null sinks.");
        list.Add(sink);
      }

      return list.AsReadOnly();
    }
  }
}
