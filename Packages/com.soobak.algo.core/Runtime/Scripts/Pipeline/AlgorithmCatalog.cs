using System;
using System.Collections.Generic;

namespace Soobak.Algo.Core.Pipeline {
  public sealed class AlgorithmCatalog<TState, TEvent> : IAlgorithmCatalog<TState, TEvent> {
    readonly IReadOnlyList<IAlgorithmDescriptor<TState, TEvent>> _descriptors;
    readonly Dictionary<string, IAlgorithmDescriptor<TState, TEvent>> _lookup;

    public AlgorithmCatalog(IEnumerable<IAlgorithmDescriptor<TState, TEvent>> descriptors) {
      if (descriptors == null)
        throw new ArgumentNullException(nameof(descriptors));

      var list = new List<IAlgorithmDescriptor<TState, TEvent>>();
      _lookup = new Dictionary<string, IAlgorithmDescriptor<TState, TEvent>>(StringComparer.Ordinal);

      foreach (var descriptor in descriptors) {
        if (descriptor == null)
          throw new ArgumentNullException(nameof(descriptors), "AlgorithmCatalog cannot include null descriptors.");

        if (_lookup.ContainsKey(descriptor.Id))
          throw new ArgumentException($"AlgorithmCatalog duplicate id detected: {descriptor.Id}", nameof(descriptors));

        list.Add(descriptor);
        _lookup.Add(descriptor.Id, descriptor);
      }

      _descriptors = list.AsReadOnly();
    }

    public IReadOnlyList<IAlgorithmDescriptor<TState, TEvent>> Descriptors => _descriptors;

    public bool TryGetDescriptor(string id, out IAlgorithmDescriptor<TState, TEvent> descriptor) {
      if (string.IsNullOrWhiteSpace(id)) {
        descriptor = default!;
        return false;
      }

      return _lookup.TryGetValue(id, out descriptor!);
    }
  }
}
