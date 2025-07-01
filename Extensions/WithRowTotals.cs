using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Extensions
{
    public static class XTabResultExtensions
    {
        public static XTabResult WithRowTotals(this XTabResult source)
        {
            var result = new XTabResult();

            // Copy columns
            foreach (var col in source.Columns)
                result.Columns.Add(col);

            foreach (var (rowKey, row) in source.Table)
            {
                result.Table[rowKey] = new(row);
                result.RowTotals[rowKey] = row.Values.Sum();
            }

            return result;
        }
    }

}
