﻿using System.Collections.Generic;

namespace WebDeployParametersToolkit.Utilities
{
    public class WebDeployParameter
    {
        public string Name { get; set; }

        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public IEnumerable<WebDeployParameterEntry> Entries { get; set; }
    }
}