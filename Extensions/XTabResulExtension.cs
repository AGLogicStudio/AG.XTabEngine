using AG.XTabEngine.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Extensions
{
    public static class XTabResultExtensions
    {
        public static IEnumerable<string> GetDataColumns(this XTabResult result)
        {
            // Assume any column that's part of the inner Table values is "data"
            var keysUsed = new HashSet<string>(
                result.Table.Values.SelectMany(row => row.Keys)
            );

            return result.Columns.Where(col => keysUsed.Contains(col));
        }

        public static XTabResult WithRowTotals(this XTabResult source)
        {
            var result = new XTabResult();

            // Copy columns
            foreach (var col in source.Columns)
                result.Columns.Add(col);

            double grandTotal = 0;

            // Copy table and compute row totals
            foreach (var (rowKey, row) in source.Table)
            {
                result.Table[rowKey] = new(row);

                double rowSum = row.Values.Sum();
                result.RowTotals[rowKey] = rowSum;

                grandTotal += rowSum;
            }

            result.RowsGrandTotal = grandTotal;

            return result;
        }

        public static XTabResult WithColumnTotals(this XTabResult source)
        {
            var result = new XTabResult();

            // Copy columns
            foreach (var col in source.GetDataColumns())
                result.Columns.Add(col);

            // Copy original table
            foreach (var (rowKey, row) in source.Table)
                result.Table[rowKey] = new(row);

            double grandTotal = 0;

            foreach (var col in source.GetDataColumns())
            {
                double colTotal = 0;

                foreach (var row in source.Table.Values)
                    if (row.TryGetValue(col, out var value))
                        colTotal += value;

                result.ColumnTotals[col] = colTotal;
                grandTotal += colTotal;
            }

            result.ColumnsGrandTotal = grandTotal;

            return result;
        }

        public static XTabResult WithTotals(this XTabResult source)
        {
            var result = new XTabResult();

            // Copy column headers
            foreach (var col in source.Columns)
                result.Columns.Add(col);

            // Initialize totals
            double rowsGrandTotal = 0;
            double columnsGrandTotal = 0;
            var columnTotals = new Dictionary<string, double>();

            // Copy table and compute row totals
            foreach (var (rowKey, row) in source.Table)
            {
                var copiedRow = new Dictionary<string, double>(row);
                result.Table[rowKey] = copiedRow;

                double rowSum = row.Values.Sum();
                result.RowTotals[rowKey] = rowSum;
                rowsGrandTotal += rowSum;

                foreach (var col in source.GetDataColumns())
                {
                    if (row.TryGetValue(col, out var val))
                    {
                        if (!columnTotals.ContainsKey(col))
                            columnTotals[col] = 0;

                        columnTotals[col] += val;
                    }
                }

            }

            // Assign column totals and compute grand total from column sums
            foreach (var (col, total) in columnTotals)
            {
                result.ColumnTotals[col] = total;
                columnsGrandTotal += total;
            }

            result.RowsGrandTotal = rowsGrandTotal;
            result.ColumnsGrandTotal = columnsGrandTotal;

            return result;
        }

        public static XTabResult SortRowsBy(this XTabResult source, Comparison<RowKey> comparison)
        {
            var result = new XTabResult();

            foreach (var col in source.Columns)
                result.Columns.Add(col);

            var sortedRows = source.Table.Keys.ToList();
            sortedRows.Sort(comparison);

            foreach (var key in sortedRows)
                result.Table[key] = new(source.Table[key]);

            // Copy totals if available
            foreach (var (key, total) in source.RowTotals)
                result.RowTotals[key] = total;

            result.RowsGrandTotal = source.RowsGrandTotal;

            foreach (var (col, total) in source.ColumnTotals)
                result.ColumnTotals[col] = total;

            result.ColumnsGrandTotal = source.ColumnsGrandTotal;

            return result;
        }

        public static XTabResult SortColumnsByData(
            this XTabResult source,
            Comparison<string>? comparison = null)
        {
            comparison ??= string.Compare;

            var dataColumns = source.GetDataColumns().ToList();
            dataColumns.Sort(comparison);

            // Keep structural columns at the front
            var structuralColumns = source.Columns.Except(dataColumns).ToList();
            var finalOrder = structuralColumns.Concat(dataColumns).ToList();

            var result = new XTabResult();

            foreach (var col in finalOrder)
                result.Columns.Add(col);

            // Copy data rows
            foreach (var (key, row) in source.Table)
            {
                result.Table[key] = new();
                foreach (var col in finalOrder)
                    if (row.TryGetValue(col, out var val))
                        result.Table[key][col] = val;
            }

            // Copy totals
            foreach (var (col, total) in source.ColumnTotals)
                result.ColumnTotals[col] = total;

            foreach (var (col, total) in source.ColumnTotals)
                result.ColumnTotals[col] = total;

            foreach (var (rowKey, total) in source.RowTotals)
                result.RowTotals[rowKey] = total;

            result.RowsGrandTotal = source.RowsGrandTotal;
            result.ColumnsGrandTotal = source.ColumnsGrandTotal;

            return result;
        }


        public static XTabResult SortForPresentation(
    this XTabResult result,
    Comparison<RowKey>? rowComparer = null,
    Comparison<string>? columnComparer = null)
        {
            var rowSort = rowComparer ?? ((a, b) =>
                result.RowTotals.TryGetValue(b, out var tb) && result.RowTotals.TryGetValue(a, out var ta)
                    ? tb.CompareTo(ta)
                    : string.Compare(a.ToString(), b.ToString()));

            var colSort = columnComparer ?? ((a, b) =>
                result.ColumnTotals.TryGetValue(b, out var tb) && result.ColumnTotals.TryGetValue(a, out var ta)
                    ? tb.CompareTo(ta)
                    : string.Compare(a, b, StringComparison.Ordinal));

            return result
                .SortRowsBy(rowSort)
                .SortColumnsByData(colSort);
        }
        public static string ToDelimited(
            this XTabResult result,
            char delimiter = '|', // "|",
            bool includeTotals = false,
            bool sortForPresentation = false)

        {
            var snapshot = sortForPresentation ? result.SortForPresentation() : result;
            var sb = new StringBuilder();

            var keyLabels = snapshot.RowKeyColumns;
            var dataCols = snapshot.Columns
                .Where(col => !keyLabels.Contains(col))
                .ToList();

            // HEADER
            sb.Append(delimiter);
            foreach (var label in keyLabels)
                sb.Append(label + delimiter);
            foreach (var col in dataCols)
                sb.Append(col + delimiter);
            if (includeTotals)
                sb.Append("Total").Append(delimiter);
            sb.AppendLine();

            // SEPARATOR
            sb.Append(delimiter);
            foreach (var _ in keyLabels)
                sb.Append("---").Append(delimiter);
            foreach (var _ in dataCols)
                sb.Append("---").Append(delimiter);
            if (includeTotals)
                sb.Append("---").Append(delimiter);
            sb.AppendLine();

            // DATA ROWS
            foreach (var (rowKey, row) in snapshot.Table)
            {
                sb.Append(delimiter);
                foreach (var part in rowKey.Components)
                    sb.Append(part + delimiter);
                foreach (var col in dataCols)
                    sb.Append(row.TryGetValue(col, out var val) ? $"{val}{delimiter}" : $" {delimiter}");
                if (includeTotals && snapshot.RowTotals.TryGetValue(rowKey, out var total))
                    sb.Append($"{total}{delimiter}");
                sb.AppendLine();
            }

            // TOTALS ROW
            if (includeTotals && snapshot.ColumnTotals.Any())
            {
                sb.Append($"{delimiter}Totals{delimiter}");
                for (int i = 0; i < keyLabels.Count + 1; i++)
                    sb.Append(delimiter);

                foreach (var col in dataCols)
                {
                    if (snapshot.GetDataColumns().Contains(col) &&
                        snapshot.ColumnTotals.TryGetValue(col, out var sum))
                        sb.Append($"{sum}{delimiter}");
                    else
                        sb.Append(" ").Append(delimiter);
                }

                if (snapshot.ColumnsGrandTotal.HasValue && snapshot.RowsGrandTotal.HasValue)
                {
                    var match = Math.Abs(snapshot.ColumnsGrandTotal.Value - snapshot.RowsGrandTotal.Value) < 1e-6;
                    var label = match
                        ? $"{snapshot.ColumnsGrandTotal.Value}"
                        : $"⚠️ {snapshot.ColumnsGrandTotal.Value} ≠ {snapshot.RowsGrandTotal.Value}";
                    sb.Append(label + delimiter);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }


    }


}
