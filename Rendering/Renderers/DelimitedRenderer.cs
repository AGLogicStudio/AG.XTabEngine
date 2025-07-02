using AG.XTabEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering.Renderers
{
    public class DelimitedRenderer : IXTabRenderer
    {
        public string Render(XTabResult result, XTabRenderOptions options)
        {
            // Reuse your existing ToDelimited logic here
            return result.ToDelimited(
                delimiter: options.Delimiter,
                includeTotals: options.IncludeTotals,
                sortForPresentation: options.SortBeforeRender,
                aliasFor: options.AliasFor
            );
        }
    }

}
