using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSettingsManager.TypeResolving
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class ResolveTypeAttribute : Attribute
	{
	}
}
