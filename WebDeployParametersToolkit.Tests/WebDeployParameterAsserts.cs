using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    public static class WebDeployParameterAsserts
    {
        public static void AssertHasSameItems(this IEnumerable<WebDeployParameter> source, IEnumerable<WebDeployParameter> target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source.Count() == target.Count())
            {
                foreach (var sourceItem in source)
                {
                    var targetItem = target.FirstOrDefault(t => t.Name == sourceItem.Name);
                    if (targetItem == null)
                    {
                        throw new AssertFailedException($"A target item with a {nameof(WebDeployParameter.Name)} of {sourceItem.Name} could not be found.");
                    }
                    else if (targetItem.DefaultValue != sourceItem.DefaultValue)
                    {
                        throw new AssertFailedException($"Non-matching properties({nameof(WebDeployParameter.DefaultValue)}) on parameter named '{sourceItem.Name}' source({sourceItem.DefaultValue}) vs. target({targetItem.DefaultValue}).");
                    }
                    else if (targetItem.Description != sourceItem.Description)
                    {
                        throw new AssertFailedException($"Non-matching properties({nameof(WebDeployParameter.Description)}) on parameter named '{sourceItem.Name}' source({sourceItem.Description}) vs. target({targetItem.Description}).");
                    }
                    else if ((sourceItem.Entries != null && (targetItem.Entries == null || sourceItem.Entries.Count() != targetItem.Entries.Count()))
                        || (sourceItem.Entries == null && targetItem.Entries != null))
                    {
                        throw new AssertFailedException($"Non-matching {nameof(WebDeployParameter.Entries)} count on parameter named '{sourceItem.Name}' source({sourceItem.Entries?.Count()}) vs. target({targetItem.Entries?.Count()}).");
                    }
                    else if (sourceItem.Entries != null)
                    {
                        foreach (var sourceEntry in sourceItem.Entries)
                        {
                            if (!targetItem.Entries.Any(e => e.Kind == sourceEntry.Kind && e.Match == sourceEntry.Match && e.Scope == sourceEntry.Scope))
                            {
                                throw new AssertFailedException($"Unable to find matching entry on parameter named '{sourceItem.Name}' (Match = '{sourceEntry.Match}', Kind = '{sourceEntry.Kind}', Scope = '{sourceEntry.Scope}').");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new AssertFailedException($"Number of source items({source.Count()}) does not match number of target items({target.Count()}).");
            }
        }
    }
}
