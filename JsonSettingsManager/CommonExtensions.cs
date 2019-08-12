using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager
{
	public static class CommonExtensions
	{
		public static T[] WrapToArray<T>(this T obj)
		{
			return new T[] { obj };
		}
	}
}
