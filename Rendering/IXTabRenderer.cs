using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Rendering
{
    public interface IXTabRenderer
    {
        string Render(XTabResult snapshot, XTabRenderOptions options);
    }

}
