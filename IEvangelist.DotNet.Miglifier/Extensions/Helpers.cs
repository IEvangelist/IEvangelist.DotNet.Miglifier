using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IEvangelist.DotNet.Miglifier.Extensions
{
    static class JsonSettings
    {
        static readonly DefaultContractResolver _contractResolver =
            new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

        internal static JsonSerializerSettings SerializerDefaults { get; } =
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ContractResolver = _contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
    }

    public static class StringExtensions
    {
        public static T To<T>(this string json) 
            => JsonConvert.DeserializeObject<T>(json, JsonSettings.SerializerDefaults);

        public static string ToFullPath(this string path)
            => Path.IsPathFullyQualified(path) ? path : Path.GetFullPath(path);
    }

    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T instance) 
            => JsonConvert.SerializeObject(instance, JsonSettings.SerializerDefaults);

        public static bool PublicInstancePropertiesEqual<T>(
            this T self, 
            T to, 
            params string[] ignore) where T : class
        {
            if (self is null || to is null)
            {
                return self == to;
            }

            var type = typeof(T);
            var ignoreList = new List<string>(ignore);

            // ReSharper disable ImplicitlyCapturedClosure
            var unequalProperties =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => !ignoreList.Contains(pi.Name)
                              && pi.GetUnderlyingType().IsSimpleType()
                              && pi.GetIndexParameters().Length == 0)
                    .Select(pi => (pi, selfValue: type.GetProperty(pi.Name).GetValue(self, null)))
                    .Select(_ => (_.selfValue, toValue: type.GetProperty(_.pi.Name).GetValue(to, null)))
                    .Where(_ => _.selfValue != _.toValue
                             && (_.selfValue is null || !_.selfValue.Equals(_.toValue)))
                    .Select(_ => _.selfValue);

            return !unequalProperties.Any();
        }

    }

    public static class TypeExtensions
    {
        public static bool IsSimpleType(this Type type)
            => type.IsValueType
            || type.IsPrimitive
            || new[]
               {
                   typeof(string),
                   typeof(decimal),
                   typeof(DateTime),
                   typeof(DateTimeOffset),
                   typeof(TimeSpan),
                   typeof(Guid)
               }.Contains(type)
            || Convert.GetTypeCode(type) != TypeCode.Object;

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }

    public static class GlobExtensions
    {
        public static IEnumerable<string> Glob(
            this string root,
            string globPattern,
            bool ignoreMinifiedFiles = true)
            => string.IsNullOrWhiteSpace(root)
                ? new string[0]
                : GlobExpressions.Glob
                                 .Files(root, globPattern)
                                 .Where(file => 
                                            !ignoreMinifiedFiles
                                         || !file.Contains(".min.", StringComparison.OrdinalIgnoreCase))
                                 .Select(file => Path.Combine(root, file));
    }
}