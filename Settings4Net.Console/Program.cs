using settings4net.Core;
using settings4net.Core.Repositories;
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
            SettingsManager.InitializeSettings4net(withRemote: true);
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine(XPTOSettings.ServiceUrl);
            System.Console.ReadLine();

        }
    }
}
