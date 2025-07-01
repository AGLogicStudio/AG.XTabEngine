using AG.XTabEngine.Aggregators;
using AG.XTabEngine.Meta;
namespace AG.XTabEngine
{

    public class XTabBuilder<T>
    {
        public required Func<T, RowKey> RowSelector { get; init; }
        public required Func<T, string> ColumnSelector { get; init; }
        public required IAggregatorFactory<T> AggregatorFactory { get; init; }

        // Optional now—can be passed through factory
        public Func<T, double>? ValueSelector { get; init; }

        public XTabResult Build(IEnumerable<T> items)
        {
            var columns = new HashSet<string>();
            var raw = new Dictionary<RowKey, Dictionary<string, IAggregator<T>>>();

            foreach (var item in items)
            {
                var rowKey = RowSelector(item);
                var colKey = ColumnSelector(item);
                var valueFactory = AggregatorFactory;

                columns.Add(colKey);

                if (!raw.TryGetValue(rowKey, out var row))
                    raw[rowKey] = row = new Dictionary<string, IAggregator<T>>();

                if (!row.TryGetValue(colKey, out var aggregator))
                    row[colKey] = aggregator = valueFactory.Create(ValueSelector ?? (_ => 1));

                aggregator.Add(item);
            }

            var result = new XTabResult();
            foreach (var col in columns)
                result.Columns.Add(col);

            foreach (var (rowKey, aggRow) in raw)
            {
                var finalized = new Dictionary<string, double>();
                foreach (var (colKey, aggregator) in aggRow)
                    finalized[colKey] = aggregator.Result;

                result.Table[rowKey] = finalized;
            }

            return result;
        }
    }
}
