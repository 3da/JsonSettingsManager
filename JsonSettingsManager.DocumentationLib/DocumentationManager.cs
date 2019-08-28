using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonSettingsManager.DocumentationLib
{
    public class DocumentationManager
    {
        private readonly IContractResolver _contractResolver = new DefaultContractResolver();

        public IList<MemberInfo> Generate(params object[] objects)
        {

            var context = new Context();
            var result = new List<MemberInfo>();
            foreach (var obj in objects)
            {
                result.Add(ProcessMember(obj.GetType().Name, obj, null, context));
            }
            return result;
        }

        private MemberInfo ProcessMember(string name, object obj, Type type, Context context)
        {
            var contract = _contractResolver.ResolveContract(type ?? obj?.GetType());

            type = type ?? obj?.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                return new MemberInfo()
                {
                    Type = GetTypeFriendlyName(type),
                    Name = name
                };
            }

            if (type.IsEnum)
                return ProcessEnum(name, type, context);

            if (contract is JsonArrayContract jsonArrayContract)
            {
                var elType = jsonArrayContract.CollectionItemType;


                return new MemberInfo()
                {
                    Type = GetTypeFriendlyName(type),
                    Name = name,
                    Items = new List<MemberInfo>()
                    {
                        ProcessMember("Item", null, elType, context)
                    }

                };
            }

            if (contract is JsonObjectContract jsonObjectContract)
                return ProcessClass(name, jsonObjectContract, obj, context);

            return new MemberInfo()
            {
                Type = GetTypeFriendlyName(type),
                Name = name,
            };
        }

        private string GetTypeFriendlyName(Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(long))
                return "long";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type == typeof(char))
                return "char";

            if (type.IsGenericType)
            {
                var gt = type.GetGenericTypeDefinition();

                if (gt == typeof(Nullable<>))
                    return $"{GetTypeFriendlyName(type.GenericTypeArguments[0])}?";

                if (gt == typeof(IList<>) || gt == typeof(List<>))
                    return $"{GetTypeFriendlyName(type.GenericTypeArguments[0])}[]";

                return $"{gt.Name}<{string.Join(", ", type.GetGenericArguments().Select(GetTypeFriendlyName))}>";
            }

            if (type.IsArray)
            {
                return $"{GetTypeFriendlyName(type.GetElementType())}[]";
            }

            return type.Name;
        }

        private EnumInfo ProcessEnum(string name, Type type, Context context)
        {
            return new EnumInfo()
            {
                Name = name,
                Type = type.Name,
                Items = Enum.GetValues(type).Cast<object>().Select(q => new MemberInfo()
                {
                    Name = Enum.GetName(type, q),
                    Type = GetTypeFriendlyName(type.GetEnumUnderlyingType()),
                    Value = Convert.ChangeType(q, type.GetEnumUnderlyingType()).ToString()
                }).ToArray()
            };
        }

        private ClassInfo ProcessClass(string name, JsonObjectContract contract, object obj, Context context)
        {

            return new ClassInfo()
            {
                Name = name,
                Type = contract.UnderlyingType.Name,
                Items = contract.Properties.Select(q => ProcessMember(q.PropertyName, obj == null ? null : q.ValueProvider.GetValue(obj), q.PropertyType, context)).Where(q => q != null).ToArray()
            };
        }

    }
}
