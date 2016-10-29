using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Interfaces
{
    internal interface IDocumentationLoader
    {

        string GetDocumentation(FieldInfo field);

    }
}
