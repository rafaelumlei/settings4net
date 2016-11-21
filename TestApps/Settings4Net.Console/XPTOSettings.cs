using settings4net.Core.Interfaces;
using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.TestConsoleApp
{
    class XPTOSettings : ISettingsClass
    {

        public enum MenuType
        {
            Internal = 1,
            External = 2
        }
        
        public class MenuOption
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Path { get; set; }

            public MenuType Type { get; set; } = MenuType.Internal;

            public IEnumerable<MenuOption> SubMenus { get; set; }
        }

        /// <b>
        /// Setting with the Remote API endpoint. Configure it to match the environment's instance.
        /// </b>
        /// <p>
        /// This API is used to get video content metadata from <i>freebase</i> ...
        /// </p>
        public static string ServiceUrl = "http://www.sapo.pt/";

        /// <b>
        /// Setting to configure the allowed video types.
        /// </b>
        /// <p>
        /// The following video formats are currently supported by the app: 
        /// <br/>
        /// <ul>
        ///     <li>mov</li>
        ///     <li>wmv</li>
        ///     <li>ogg</li>
        ///     <li>avi</li>
        ///     <li>flv</li>
        ///     <li>ogg</li>
        ///     <li>mp4</li>
        ///     <li>mpeg</li>
        /// </ul>
        /// However, this setting may be used to select the ones that are currently allowed.
        /// </p>
        public static string[] AllowedVideoFormats = new string[] 
        {
            "mp4",
            "mkv"
        };

        /// <b>
        /// Setting to configure the app's not found error message per lang/locale.
        /// </b>
        /// <p>
        /// Add more pairs locale, message as needed.
        /// </p>
        public static Dictionary<string, string> NotFoundErrorMenssages = new Dictionary<string, string>()
        {
            {
                "en-US", "Page not found."
            },
            {
                "pt-PT", "Página não encontrada."
            }
        };


        /// <b>
        /// Setting to configure the main menu options available in the app.
        /// </b>
        /// <p> 
        /// The menus may have SubMenus and each menu may have an executable path.
        /// The executable path may be a direct link to an external site or a internal 
        /// path, to execute within the app's context.
        /// <br/>
        /// The Menu Types supported
        /// <ol>
        ///     <li>Internal (default)</li>
        ///     <li>External</li>
        /// </ol>
        /// Config sample:
        /// <br/>
        /// <pre>
        /// 
        /// </pre>
        /// </p>
        public static List<MenuOption> MainMenuOptions = new List<MenuOption>()
        {
            new MenuOption()
            {
                Id = 1,
                Name = "Menu 1",
                Path = null,
                SubMenus = new List<MenuOption>()
                {
                    new MenuOption()
                    {
                        Id = 11,
                        Name = "Submenu 1",
                        Path = "app/select/11",
                        SubMenus = null
                    },
                    new MenuOption()
                    {
                        Id = 12,
                        Name = "Submenu 2",
                        Path = "app/select/12",
                        SubMenus = null
                    }
                }
            },
            new MenuOption()
            {
                Id = 2,
                Name = "Menu 2",
                SubMenus = new List<MenuOption>()
                {
                    new MenuOption()
                    {
                        Id = 21,
                        Name = "Submenu 1",
                        Path = "app/select/21",
                        SubMenus = null
                    },
                    new MenuOption()
                    {
                        Id = 22,
                        Name = "Submenu 2",
                        Type = MenuType.External,
                        Path = "http://www.google.com",
                        SubMenus = null
                    }
                }
            },
        };

    }
}
