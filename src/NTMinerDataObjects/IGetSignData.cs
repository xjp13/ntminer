﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public interface IGetSignData {
        StringBuilder GetSignData();
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ManualSignAttribute : Attribute {
        public ManualSignAttribute() { }
    }

    public static class GetSignDataExtension {
        private static Dictionary<Type, PropertyInfo[]> _propertyInfos = new Dictionary<Type, PropertyInfo[]>();
        private static readonly object _locker = new object();
        private static PropertyInfo[] GetPropertyInfos(Type type) {
            if (!_propertyInfos.TryGetValue(type, out PropertyInfo[] properties)) {
                lock (_locker) {
                    if (!_propertyInfos.TryGetValue(type, out properties)) {
                        properties = type.GetProperties().Where(a => a.CanRead && a.CanWrite && a.GetCustomAttributes(typeof(ManualSignAttribute), inherit: false).Length == 0).ToArray();
                        _propertyInfos.Add(type, properties);
                    }
                }
            }
            return properties;
        }

        public static StringBuilder BuildSign(this IGetSignData obj) {
            var propertyInfos = GetPropertyInfos(obj.GetType());
            StringBuilder sb = new StringBuilder();
            foreach (var propertyInfo in propertyInfos) {
                sb.Append(propertyInfo.Name).Append(propertyInfo.GetValue(obj, null));
            }
            return sb;
        }
    }
}
