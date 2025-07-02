using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering
{
    public class XTabRenderOptions
    {
        public RenderStyle Style { get; set; } = RenderStyle.Delimited;
        public char Delimiter { get; set; } = ',';
        public bool IncludeTotals { get; set; } = true;
        public bool SortBeforeRender { get; set; } = true;
        public Func<string, string>? AliasFor { get; set; }

        public static Func<string, string> FromAliasMap(Dictionary<string, string> map) =>
            label => map.TryGetValue(label, out var alias) ? alias : label;
    }

    public enum RenderStyle
    {
        Delimited,
        Html,
        Markdown
    }

}
