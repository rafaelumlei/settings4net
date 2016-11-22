# Settings4net 

[![Build status](https://ci.appveyor.com/api/projects/status/iiysdoygrgt10ktd/branch/master?svg=true)](https://ci.appveyor.com/project/rafaelumlei/settings4net/branch/master)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/settings4net.svg)](https://www.nuget.org/packages/settings4net/)

This project provides a framework and tools that enable applications to be configured locally using a JSON file or in a remote server for centralized settings's management/browsing. The settings are implemented using normal code, no additional configuration is necessary.


How to use it
-------------

1. Install the nuget package [settings4net](https://www.nuget.org/packages/settings4net/) in your project and initialize the **settings4net** ensuring that it is initialized when the settings start to be used. 

    **In a console app**:

    ```csharp
        class Program
        {
            static Program() 
            {
                // the withremote parameter is only to use if you want send 
                // all the application settings to a remote API for centralized 
                // management. Without remote the settings are still overridable 
                // using a JSON file.
                SettingsManager.InitializeSettings4net(withRemote: true);
            }

            static void Main(string[] args)
            {
                // your code goes here
            }
        }
    ```

    **In a ASP.NET Web project**:

    ```csharp
        public class WebApiApplication : System.Web.HttpApplication
        {
            static WebApiApplication()
            {
                SettingsManager.InitializeSettings4net(withRemote: true);
            }

            protected void Application_Start()
            {
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
        }
    ```

2. Add to the App/Web.config the settings that specify what is the current environment of the app (typically DEV while you are developing it) and, optinonally, the remote settings repository API endpoint (this is necessary if you are initializing the settings4net with the remote option active):

   ```xml
     <appSettings>
       <add key="log4net.config" value="Log.config" />
       <add key="Settings4netCurrentEnvironment" value="dev" />
       <add key="Settings4netRemoteURI" value="http://HOST/settings4net.API/" />
     </appSettings>
     ```

3. Start coding your settings. For that aim, the unique requirement is to mark all your setting classes with the intercace **ISettingsClass**:


   ```csharp

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
   ```

   Note: all your application settings must be well documented so that the operations team may be autonomous managing the setting values. The comments were writtten using plain to html to improve readability in the web portal, because in this first version the setting documentation is presented as is. It is only possible for the **settings4net** to  extract the setttings' documentation if the MSBuild outputs the XML files as part of the build, for that go to Project->Settings->Build and check the **XML Documentation File**. 

How it works
-------------

Settings repositories chain:

![Settings Chain](/../readme-assets/settings4net_architecture_v1.png)


under construction...
