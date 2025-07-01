using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Meta
{
    public record RowKey(ImmutableArray<string> Dimensions)
    {
        public override string ToString() => string.Join("~", Dimensions);
        public string this[int index] => Dimensions[index];
        public int Count => Dimensions.Length;

        public static RowKey Of(params string[] parts) => new(ImmutableArray.Create(parts));

        public virtual bool Equals(RowKey? other) =>
            other is not null && Dimensions.SequenceEqual(other.Dimensions);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var dim in Dimensions)
                hash.Add(dim);
            return hash.ToHashCode();
        }
    }
}
