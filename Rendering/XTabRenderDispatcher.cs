using AG.XTabEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering
{
    public static class XTabRenderDispatcher
    {
        public static string ToRender(this XTabResult result, XTabRenderOptions options)
        {
            if (options.SortBeforeRender)
                result = result.SortForPresentation();

            return options.Style switch
            {
                RenderStyle.Delimited => new Renderers.DelimitedRenderer().Render(result, options),
                RenderStyle.Html => new Renderers.HtmlRenderer().Render(result, options),
               // RenderStyle.Markdown => new MarkdownRenderer().Render(result, options),
                _ => throw new NotSupportedException("Unknown render style.")
            };
        }
    }

}
