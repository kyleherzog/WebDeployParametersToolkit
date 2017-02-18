using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests
{
    public static class WebConfigSettingsComparer
    {
        public static bool HasSameItems(this IEnumerable<WebConfigSetting> source, IEnumerable<WebConfigSetting> target)
        {
            if (source.Count() == target.Count())
            {
                foreach (var sourceItem in source)
                {
                    var targetItem = target.Where(t => t.NodePath == sourceItem.NodePath).FirstOrDefault();
                    if (targetItem == null 
                        || targetItem.Name != sourceItem.Name)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
