﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    public class StringDataSource : IDataSource
    {
        public string Text { get; set; }
        public (JToken, IDataSource) Load(IDataSource lastDataSource, LoadMode mode, ParseContext context)
        {
            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(Text), this);
                case LoadMode.Text:
                    return (new JValue(Text), this);
                case LoadMode.Bin:
                    throw new InvalidOperationException();
                case LoadMode.Lines:
                    var lines = Text.Split('\n').Select(q => q.Trim('\r')).ToArray();
                    return (new JArray(lines), this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
