using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonSettingsManager.DocumentationLib
{
    internal class Context
    {
        public IList<Type> ProcessedTypes { get; set; }

        public Context Clone() => new Context() { ProcessedTypes = ProcessedTypes.ToList() };
    }
}
