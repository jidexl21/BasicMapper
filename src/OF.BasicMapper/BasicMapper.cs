using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OF.BasicMapper
{
    /// <summary>
    /// Minimal reflection-based object-to-object mapper. A property is copied when the source and destination
    /// both have a public instance property with the same name (case-sensitive), the source property is readable,
    /// the destination property is writable, and the source type can be assigned to the destination type
    /// (including <c>T</c> to/from <c>Nullable&lt;T&gt;</c>). Everything else is skipped silently.
    /// </summary>
    /// <remarks>
    /// The copy is shallow: reference-type values (lists, nested objects) are shared, not cloned.
    /// Property pairs are cached per (source type, destination type), so instances are cheap and thread-safe.
    /// </remarks>
    public class BasicMapper : IBasicMapper
    {
        private static readonly ConcurrentDictionary<(Type From, Type To), PropertyPair[]> PlanCache =
            new ConcurrentDictionary<(Type From, Type To), PropertyPair[]>();

        /// <inheritdoc />
        public T MapFields<T>(object from) => MapNew<T>(from, null);

        /// <inheritdoc />
        public T MapFields<T>(object from, List<string> skip) => MapNew<T>(from, skip);

        /// <inheritdoc />
        public T MapFields<T>(object from, object to) => MapExisting<T>(from, to, null);

        /// <inheritdoc />
        public T MapFields<T>(object from, object to, List<string> skip) => MapExisting<T>(from, to, skip);

        private static T MapNew<T>(object from, IEnumerable<string> skip)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));
            object to;
            try
            {
                to = Activator.CreateInstance(typeof(T));
            }
            catch (MissingMethodException ex)
            {
                throw new InvalidOperationException(
                    $"Cannot create {typeof(T).FullName}: it needs a public parameterless constructor. " +
                    "Use the MapFields<T>(from, to) overload with an existing instance instead.", ex);
            }
            return MapExisting<T>(from, to, skip);
        }

        private static T MapExisting<T>(object from, object to, IEnumerable<string> skip)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (!(to is T))
                throw new ArgumentException(
                    $"Destination object of type {to.GetType().FullName} is not a {typeof(T).FullName}.", nameof(to));

            var skipSet = skip == null ? null : new HashSet<string>(skip, StringComparer.Ordinal);

            foreach (var pair in GetPlan(from.GetType(), to.GetType()))
            {
                if (skipSet != null && skipSet.Contains(pair.Name)) continue;
                pair.Target.SetValue(to, pair.Source.GetValue(from));
            }
            // Cast after mapping so value-type destinations return the updated (boxed) copy.
            return (T)to;
        }

        private static PropertyPair[] GetPlan(Type fromType, Type toType) =>
            PlanCache.GetOrAdd((fromType, toType), key => BuildPlan(key.From, key.To));

        private static PropertyPair[] BuildPlan(Type fromType, Type toType)
        {
            var sources = MostDerivedProperties(fromType, p => p.GetGetMethod() != null);
            var targets = MostDerivedProperties(toType, p => p.GetSetMethod() != null);

            var plan = new List<PropertyPair>();
            foreach (var target in targets.Values)
            {
                if (sources.TryGetValue(target.Name, out var source) &&
                    IsCompatible(source.PropertyType, target.PropertyType))
                {
                    plan.Add(new PropertyPair(target.Name, source, target));
                }
            }
            return plan.ToArray();
        }

        // Public instance, non-indexed properties. When a derived class hides a base property with `new`,
        // GetProperties returns both; keep the one declared on the most-derived type.
        private static Dictionary<string, PropertyInfo> MostDerivedProperties(Type type, Func<PropertyInfo, bool> accessible)
        {
            var result = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.GetIndexParameters().Length > 0 || !accessible(p)) continue;
                if (!result.TryGetValue(p.Name, out var existing) ||
                    existing.DeclaringType.IsAssignableFrom(p.DeclaringType))
                {
                    result[p.Name] = p;
                }
            }
            return result;
        }

        private static bool IsCompatible(Type source, Type target)
        {
            if (target.IsAssignableFrom(source)) return true;
            // int? -> int : a null value is written as default(int), matching 2.x behaviour.
            var underlying = Nullable.GetUnderlyingType(source);
            return underlying != null && target == underlying;
        }

        private sealed class PropertyPair
        {
            public PropertyPair(string name, PropertyInfo source, PropertyInfo target)
            {
                Name = name;
                Source = source;
                Target = target;
            }

            public string Name { get; }
            public PropertyInfo Source { get; }
            public PropertyInfo Target { get; }
        }
    }
}
