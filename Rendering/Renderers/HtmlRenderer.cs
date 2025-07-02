using AG.XTabEngine.Extensions;
using AG.XTabEngine.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering.Renderers
{
    public class HtmlRenderer : IXTabRenderer
    {
        public string Render(XTabResult result, XTabRenderOptions options)
        {
            var sb = new StringBuilder();

            if (options.SortBeforeRender)
                result = result.SortForPresentation();

            var keys = result.RowKeyColumns;
            var data = result.GetDataColumns();
            var alias = options.AliasFor ?? (s => s);

            sb.AppendLine("<table>");

            // THEAD
            sb.AppendLine("  <thead><tr>");
            foreach (var key in keys)
                sb.Append($"<th>{HtmlEncode(alias(key))}</th>");
            foreach (var col in data)
                sb.Append($"<th>{HtmlEncode(alias(col))}</th>");
            if (options.IncludeTotals)
                sb.Append("<th>Total</th>");
            sb.AppendLine("</tr></thead>");

            // TBODY
            sb.AppendLine("  <tbody>");
            foreach (var (rk, row) in result.Table)
            {
                sb.Append("    <tr>");
                foreach (var key in rk.Values)
                    sb.Append($"<td>{HtmlEncode(key)}</td>");
                foreach (var col in data)
                {
                    var val = row.TryGetValue(col, out var num) ? num : (double?)null;
                    sb.Append(Td(val, rightAlign: true));
                }
                if (options.IncludeTotals)
                    sb.Append($"<td>{result.RowTotals[rk]}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("  </tbody>");

            // TFOOT
            if (options.IncludeTotals)
            {
                sb.AppendLine("  <tfoot><tr>");

                // Alias "Totals" and left-align
                var totalLabel = alias("Totals") ?? "Totals";
                sb.Append(Td(totalLabel)); // left-aligned by default

                // Fill empty key columns
                for (int i = 1; i < keys.Count; i++)
                    sb.Append(Td("")); // left-align blank cells

                // Add column totals (right-aligned)
                foreach (var col in data)
                    sb.Append(Td(result.ColumnTotals.GetValueOrDefault(col), rightAlign: true));

                // Add grand total if available (right-aligned)
                var grand = result.ColumnsGrandTotal;
                if (grand.HasValue)
                    sb.Append(Td(grand.Value, rightAlign: true));

                sb.AppendLine("</tr></tfoot>");
            }


            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private static string HtmlEncode(string input) =>
            System.Net.WebUtility.HtmlEncode(input);

        private static string Td(object? value, bool rightAlign = false)
        {
            var encoded = HtmlEncode(value?.ToString() ?? "");
            var align = rightAlign ? " style=\"text-align:right\"" : "";
            return $"<td{align}>{encoded}</td>";
        }

    }

}
