using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager.DocumentationLib
{
    public class MemberInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public IList<MemberInfo> Children { get; set; }

        public IList<MemberInfo> Implementations { get; set; }

        public MemberType MemberType { get; set; }
    }
}
