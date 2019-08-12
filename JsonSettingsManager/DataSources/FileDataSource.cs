using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
	public class FileDataSource : IDataSource
	{
		public string Path { get; set; }

		public string WorkDir { get; set; }

		public (JToken, IDataSource) Load(IDataSource lastDataSource)
		{
			if (WorkDir == null && lastDataSource is FileDataSource f)
			{
				WorkDir = f.WorkDir;
			}

			var searchPaths = new List<string>
			{
				Path, Path + ".json",
			};

			if (WorkDir != null)
			{
				searchPaths.Insert(0, System.IO.Path.Combine(WorkDir, Path));
				searchPaths.Insert(1, System.IO.Path.Combine(WorkDir, Path + ".json"));
			}

			FileInfo bestSearchPath = null;

			foreach (var searchPath in searchPaths)
			{
				var sp = new FileInfo(searchPath);
				if (sp.Exists)
				{
					bestSearchPath = sp;
					break;
				}
			}
			if (bestSearchPath == null)
				throw new Exception();

			WorkDir = bestSearchPath.DirectoryName;

			return (JToken.Parse(File.ReadAllText(bestSearchPath.FullName)), this);
		}
	}
}
