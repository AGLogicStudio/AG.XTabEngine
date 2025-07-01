using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.XTabEngine.Meta;

namespace AG.XTabEngine
{
    public sealed class XTabResult
    {
        public List<string> Columns { get; } = new(); // Metrics only
        public Dictionary<RowKey, Dictionary<string, double>> Table { get; } = new();

        public Dictionary<string, double> ColumnTotals { get; } = new();
        public Dictionary<RowKey, double> RowTotals { get; } = new();
        public double? RowsGrandTotal { get; set; }
        public double? ColumnsGrandTotal { get; set; }

        public List<string> RowKeyColumns { get; } = new(); // e.g. "Region", "Product"

        // Optional: add method to inject labels
        public XTabResult WithRowKeyColumns(params string[] labels)
        {
            RowKeyColumns.Clear();
            RowKeyColumns.AddRange(labels);
            return this;
        }
    }


}
