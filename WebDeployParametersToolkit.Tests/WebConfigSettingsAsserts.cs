using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    public static class WebConfigSettingsAsserts
    {
        public static void AssertHasSameItems(this IEnumerable<WebConfigSetting> source, WebConfigSetting target)
        {
            AssertHasSameItems(source, new List<WebConfigSetting>() { target });
        }

        public static void AssertHasSameItems(this IEnumerable<WebConfigSetting> source, IEnumerable<WebConfigSetting> target)
        {
            if (source.Count() == target.Count())
            {
                foreach (var sourceItem in source)
                {
                    var targetItem = target.FirstOrDefault(t => t.NodePath == sourceItem.NodePath);
                    if (targetItem == null)
                    {
                        throw new AssertFailedException($"A target item with a NodePath of {sourceItem.NodePath} could not be found.");
                    }
                    else if (targetItem.Name != sourceItem.Name)
                    {
                        throw new AssertFailedException($"The {sourceItem.NodePath} item source name ({sourceItem.Name}) does not match target name ({targetItem.Name}).");
                    }
                    else if (targetItem.Value != sourceItem.Value)
                    {
                        throw new AssertFailedException($"The {sourceItem.NodePath} item source name ({sourceItem.Value}) does not match target name ({targetItem.Value}).");
                    }
                }
                return;
            }
            else
            {
                throw new AssertFailedException($"Number of source items({source.Count()}) does not match number of target items({target.Count()}).");
            }
        }
    }
}