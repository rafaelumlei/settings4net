using settings4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings4Net.Console
{
    class Program
    {
        static Program() 
        {
            SettingsManager.InitializeSettings4net("dev", new CodeSettingsRepository(), new JSONSettingsRepository());
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine(XPTOSettings.ServiceUrl);
            System.Console.ReadLine();

        }
    }
}
