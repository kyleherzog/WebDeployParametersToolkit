using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDeployParametersToolkit.Utilities
{
    public class WebDeployParameterEntry
    {
        public string Kind { get; set; }

        public string Match { get; set; }

        public string Scope { get; set; }
    }
}
