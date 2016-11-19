using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core
{
    class XPTOSettings : ISettingsClass
    {

        public class EntryConfig
        {
            public int Number { get; set; }

            public string Description { get; set; }
        }

        public class BrowseSetting
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Env { get; set; }

            public IEnumerable<EntryConfig> Entries { get; set; }
        }

        /// <summary>
        ///   Initializes a new instance of a
        ///   <c>DocumentationSample</c> type.
        /// </summary>
        /// <example>The following is an example of initializing a
        /// <c>DocumentationSample</c> type:
        ///   <code>
        ///     // Create the type.
        ///     DocumentationSample ds = new DocumentationSample();
        ///
        ///     if ( null == ds )
        ///       return;
        ///
        ///     return ds.MyMethod( “someString” );
        ///   </code>
        /// </example>
        public static string ServiceUrl = "http://www.sapo.pt/";

        /// <summary>The <c>DocumentationSample</c> type
        /// demonstrates code comments.</summary>
        /// <remarks>
        ///     <para>
        ///         The <c>DocumentationSample</c> type
        ///         provides no real functionality;
        ///         however, it does provide examples of
        ///         using the most common, built in
        ///         <c>C#</c> code comment xml tags.
        ///     </para>
        ///     <para><c>DocumentationSample</c> types are not
        ///           safe for access by concurrent threads.</para>
        /// </remarks>
        public static string[] ContentTypes = new string[] { "a", "b", "c" };

        /// <h4>
        ///     Initializes a new instance of a
        ///     <c>DocumentationSample</c> type.
        /// </h4>
        /// <p>The following is an example of initializing
        ///          an <b>DocumentationSample</b> type:
        ///   <b>
        ///       // Create the type.
        ///       DocumentationSample ds = new DocumentationSample;
        ///
        ///       if ( null == ds )
        ///           return;
        ///
        ///       return ds.DoSomething( 5 );
        ///   </b>
        /// </p>
        public static List<EntryConfig> MyCutomObject = new List<EntryConfig>()
        {
            new EntryConfig()
            {
                Number = 1,
                Description = "Hello 1"
            },
            new EntryConfig()
            {
                Number = 2,
                Description = "Hello 2"
            }
        };

        /// <h4>
        ///     Initializes a new instance of a
        ///     <c>DocumentationSample</c> type.
        /// </h4>
        /// <p>The following is an example of initializing
        ///          an <b>DocumentationSample</b> type:
        ///   <b>
        ///       // Create the type.
        ///       DocumentationSample ds = new DocumentationSample;
        ///
        ///       if ( null == ds )
        ///           return;
        ///
        ///       return ds.DoSomething( 5 );
        ///   </b>
        /// </p>
        public static List<BrowseSetting> BrowseOptions = new List<BrowseSetting>()
        {
            new BrowseSetting()
            {
                Id = 1,
                Name = "Menu 1",
                Entries = new List<EntryConfig>()
                {
                    new EntryConfig()
                    {
                        Number = 1,
                        Description = "Hello 1"
                    },
                    new EntryConfig()
                    {
                        Number = 2,
                        Description = "Hello 2"
                    }
                }
            },
            new BrowseSetting()
            {
                Id = 1,
                Name = "Menu 1",
                Entries = new List<EntryConfig>()
                {
                    new EntryConfig()
                    {
                        Number = 1,
                        Description = "Hello 1"
                    },
                    new EntryConfig()
                    {
                        Number = 2,
                        Description = "Hello 2"
                    }
                }
            },
        };

    }
}
