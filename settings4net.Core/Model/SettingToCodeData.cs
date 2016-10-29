using settings4net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Model
{
    internal class SettingToCodeData
    {

        public FieldInfo SettingField { get; set; }

        public Setting SettingValue { get; set; } 

    }
}
