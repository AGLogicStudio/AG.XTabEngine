using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.XTabEngine.Meta;

namespace AG.XTabEngine
{
    public class XTabResult
    {
            public List<string> Columns { get; } = new();
            public Dictionary<RowKey, Dictionary<string, double>> Table { get; } = new();

            public Dictionary<RowKey, double> RowTotals { get; } = new();
            public Dictionary<string, double> ColumnTotals { get; } = new();

    }


}
