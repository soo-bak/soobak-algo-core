using System;
using System.Collections.Generic;

namespace Soobak.Algo.Core.Pipeline {
  public interface IAlgorithmDescriptor<TState, TEvent> {
    string Id { get; }

    string DisplayName { get; }

    string Description { get; }

    Func<IAlgorithm<TState, TEvent>> Factory { get; }

    IReadOnlyDictionary<string, string> Metadata { get; }
  }

  public interface IAlgorithmCatalog<TState, TEvent> {
    IReadOnlyList<IAlgorithmDescriptor<TState, TEvent>> Descriptors { get; }

    bool TryGetDescriptor(string id, out IAlgorithmDescriptor<TState, TEvent> descriptor);
  }

  public static class AlgorithmDescriptor {
    public static IAlgorithmDescriptor<TState, TEvent> Create<TState, TEvent>(
      string id,
      string displayName,
      string description,
      Func<IAlgorithm<TState, TEvent>> factory,
      IReadOnlyDictionary<string, string>? metadata = null) {
      if (string.IsNullOrWhiteSpace(id))
        throw new ArgumentException("AlgorithmDescriptor id cannot be null or whitespace.", nameof(id));

      if (string.IsNullOrWhiteSpace(displayName))
        throw new ArgumentException("AlgorithmDescriptor display name cannot be null or whitespace.", nameof(displayName));

      if (factory == null)
        throw new ArgumentNullException(nameof(factory));

      return new DelegateAlgorithmDescriptor<TState, TEvent>(id, displayName, description ?? string.Empty, factory, metadata);
    }

    sealed class DelegateAlgorithmDescriptor<TState, TEvent> : IAlgorithmDescriptor<TState, TEvent> {
      readonly IReadOnlyDictionary<string, string> _metadata;

      public DelegateAlgorithmDescriptor(string id, string displayName, string description, Func<IAlgorithm<TState, TEvent>> factory, IReadOnlyDictionary<string, string>? metadata) {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Factory = factory;
        _metadata = metadata != null ? new Dictionary<string, string>(metadata) : new Dictionary<string, string>();
      }

      public string Id { get; }

      public string DisplayName { get; }

      public string Description { get; }

      public Func<IAlgorithm<TState, TEvent>> Factory { get; }

      public IReadOnlyDictionary<string, string> Metadata => _metadata;
    }
  }
}
