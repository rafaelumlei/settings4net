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
            try
            {
                SettingsManager.InitializeSettings4net(withRemote: true);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
            }
        }

        static void Main(string[] args)
        {

            System.Console.WriteLine(XPTOSettings.MainMenuOptions.First().Name);
            System.Console.ReadLine();

        }
    }
}
