using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Meta
{
    public sealed class RowKey
    {
        private readonly List<string> _components;

        private RowKey(IEnumerable<string> components)
        {
            _components = components.ToList();
        }

        public static RowKey Of(params string[] parts) => new(parts);

        public IReadOnlyList<string> Components => _components;
        public IReadOnlyList<string> Values => _components;  // used by renderers for HTML output


        public override string ToString() => string.Join("-", _components);

        public override bool Equals(object? obj) =>
            obj is RowKey other &&
            _components.SequenceEqual(other._components);

        public override int GetHashCode() =>
            HashCode.Combine(_components.Count, _components.Aggregate(0, (hash, val) => hash ^ val.GetHashCode()));
    }

}
