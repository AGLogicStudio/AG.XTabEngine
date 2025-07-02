using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering.Renderers
{
    public class MarkdownRenderer : IXTabRenderer
    {
        public string Render(XTabResult result, XTabRenderOptions options)
        {
            // Force delimiter and render as Markdown-style
            options = new XTabRenderOptions
            {
                Style = RenderStyle.Markdown,
                Delimiter = '|',
                IncludeTotals = options.IncludeTotals,
                SortBeforeRender = options.SortBeforeRender,
                AliasFor = options.AliasFor
            };

            return new DelimitedRenderer().Render(result, options);
        }
    }

}
