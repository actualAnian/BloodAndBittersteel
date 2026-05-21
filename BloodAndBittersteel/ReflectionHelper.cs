using System;
using System.Reflection;

namespace BloodAndBittersteel
{
    public static class ReflectionHelper
    {
        public static TReturn GetFieldValue<TOwner, TReturn>(TOwner owner, string name) where TOwner : class
        {
            return (TReturn)typeof(TOwner).GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).GetValue(owner);
        }

        public static void SetFieldValue<TOwner>(TOwner owner, string name, dynamic value) where TOwner : class
        {
            //_logger?.LogError($"ReflectionService: Field '{name}' not found on type '{typeof(TOwner).Name}'");
            var field = typeof(TOwner).GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static) ?? throw new InvalidOperationException($"Field '{name}' not found on type '{typeof(TOwner).Name}'");
            field.SetValue(owner, value);
        }

        public static TReturn GetPropertyValue<TOwner, TReturn>(TOwner owner, string name) where TOwner : class
        {
            return (TReturn)GetPropertyValue(owner, name);
        }

        public static dynamic GetPropertyValue<TOwner>(TOwner owner, string name) where TOwner : class
        {
            return typeof(TOwner).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(owner);
        }

        public static void SetPropertyValue<TOwner>(TOwner owner, string name, dynamic value)
            where TOwner : class
        {
            var property = typeof(TOwner).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                //_logger?.LogError($"ReflectionService: Property '{name}' not found on type '{typeof(TOwner).Name}'");
                throw new InvalidOperationException($"Property '{name}' not found on type '{typeof(TOwner).Name}'");
            }
            if (!property.CanWrite)
            {
                //_logger?.LogError($"ReflectionService: Property '{name}' on type '{typeof(TOwner).Name}' is read-only");
                throw new InvalidOperationException($"Property '{name}' on type '{typeof(TOwner).Name}' is read-only");
            }
            property.SetValue(owner, value);
        }

        public static dynamic CallPrivateMethod<TType>(TType owner, string name, object[] values) where TType : class
        {
            try
            {
                var typeToSearch = typeof(TType) == typeof(object) ? owner.GetType() : typeof(TType);

                var method = typeToSearch.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                if (method == null)
                {
                    //_logger?.LogError($"ReflectionService: Method '{name}' not found on type '{typeToSearch.Name}'");
                    throw new InvalidOperationException($"Method '{name}' not found on type '{typeToSearch.Name}'");
                }

                return method.Invoke(owner, values);
            }
            catch (Exception ex)
            {
                //if (_logger != null)
                //{
                //    _logger.LogError($"Error in CallPrivateMethod: {ex.Message}");
                //    _logger.LogError($"Stack trace: {ex.StackTrace}");
                //}
                //else
                //{
                //    Debug.WriteLine($"Error in CallPrivateMethod: {ex.Message}");
                //    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                //}

                return null;
            }
        }
    }
}