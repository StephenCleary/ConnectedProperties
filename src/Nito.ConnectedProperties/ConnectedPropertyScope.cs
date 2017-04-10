using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nito.ConnectedProperties
{
    /// <summary>
    /// A collection of properties that can be connected to carrier objects.
    /// </summary>
    public sealed class ConnectedPropertyScope
    {
        /// <summary>
        /// The underlying property store.
        /// </summary>
        private readonly ConditionalWeakTable<object, ConcurrentDictionary<string, object>> _table = new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

        /// <summary>
        /// Gets the concurrent dictionary for the specified carrier object, creating it if necessary. No validation is done.
        /// </summary>
        /// <param name="carrier">The carrier object.</param>
        private ConcurrentDictionary<string, object> GetPropertyStore(object carrier) => _table.GetValue(carrier, _ => new ConcurrentDictionary<string, object>());

        /// <summary>
        /// Gets the default collection of connected properties.
        /// </summary>
        public static ConnectedPropertyScope Default { get; } = new ConnectedPropertyScope();

        /// <summary>
        /// Gets a connected property with the specified name. Throws <see cref="InvalidOperationException"/> if <paramref name="carrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="carrier">The carrier object for this property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="bypassValidation">An optional value indicating whether to bypass carrier object validation. The default is <c>false</c>.</param>
        public ConnectedProperty GetConnectedProperty(object carrier, string name, bool bypassValidation = false)
        {
            if (!bypassValidation && !TryVerify(carrier))
                throw ValidationException(carrier);
            return TryGetConnectedProperty(carrier, name, bypassValidation: true);
        }

        /// <summary>
        /// Gets a connected property with the specified name. Returns <c>null</c> if <paramref name="carrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="carrier">The carrier object for this property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="bypassValidation">An optional value indicating whether to bypass carrier object validation. The default is <c>false</c>.</param>
        public ConnectedProperty TryGetConnectedProperty(object carrier, string name, bool bypassValidation = false)
        {
            if (!bypassValidation && !TryVerify(carrier))
                return null;
            var dictionary = GetPropertyStore(carrier);
            return new ConnectedProperty(dictionary, name);
        }

        /// <summary>
        /// Copies all connected properties in this scope from one carrier object to another. Throws <see cref="InvalidOperationException"/> if either <paramref name="fromCarrier"/> or <paramref name="toCarrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="fromCarrier">The carrier object acting as the source of connected properties.</param>
        /// <param name="toCarrier">The carrier object acting as the destination of connected properties.</param>
        /// <param name="bypassValidation">An optional value indicating whether to bypass carrier object validation. The default is <c>false</c>.</param>
        public void CopyAll(object fromCarrier, object toCarrier, bool bypassValidation = false)
        {
            if (!bypassValidation)
            {
                if (!TryVerify(fromCarrier))
                    throw ValidationException(fromCarrier);
                if (!TryVerify(toCarrier))
                    throw ValidationException(toCarrier);
            }
            TryCopyAll(fromCarrier, toCarrier, bypassValidation: true);
        }

        /// <summary>
        /// Copies all connected properties in this scope from one carrier object to another. Returns <c>false</c> if either <paramref name="fromCarrier"/> or <paramref name="toCarrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="fromCarrier">The carrier object acting as the source of connected properties.</param>
        /// <param name="toCarrier">The carrier object acting as the destination of connected properties.</param>
        /// <param name="bypassValidation">An optional value indicating whether to bypass carrier object validation. The default is <c>false</c>.</param>
        public bool TryCopyAll(object fromCarrier, object toCarrier, bool bypassValidation = false)
        {
            if (!bypassValidation && (!TryVerify(fromCarrier) || !TryVerify(toCarrier)))
                return false;
            var properties = GetPropertyStore(fromCarrier);
            var destination = GetPropertyStore(toCarrier);
            foreach (var property in properties)
            {
                destination.AddOrUpdate(property.Key, _ => property.Value, (_, __) => property.Value);
            }
            return true;
        }

        /// <summary>
        /// Returns an <see cref="InvalidOperationException"/> indicating that <paramref name="carrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="carrier">The carrier object.</param>
        private static Exception ValidationException(object carrier)
        {
            return new InvalidOperationException("Object of type \"" + carrier.GetType() + "\" may not have connected properties. Only reference types that use reference equality may have connected properties.");
        }

        /// <summary>
        /// A cache of valid carrier types.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> ValidCarrierTypes = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Verifies the carrier object: it must be a reference type that uses reference equality. Returns <c>true</c> if the carrier object may have connected properties; <c>false</c> if the carrier object may not have connected properties.
        /// </summary>
        /// <param name="carrier">The carrier object to verify.</param>
        /// <returns><c>true</c> if the carrier object may have connected properties; <c>false</c> if the carrier object may not have connected properties.</returns>
        private static bool TryVerify(object carrier)
        {
            var type = carrier.GetType();
            return ValidCarrierTypes.GetOrAdd(type, IsReferenceEquatable);
        }

        /// <summary>
        /// Returns <c>true</c> if this type uses reference equality (i.e., does not override <see cref="object.Equals(object)"/>); returns <c>false</c> if this type or any of its base types override <see cref="object.Equals(object)"/>. This method returns <c>false</c> for any interface type, and returns <c>true</c> for any reference-equatable base class even if a derived class is not reference-equatable; the best way to determine if an object uses reference equality is to pass the exact type of that object.
        /// </summary>
        /// <param name="type">The type to test for reference equality. May not be <c>null</c>.</param>
        /// <returns>Returns <c>true</c> if this type uses reference equality (i.e., does not override <see cref="object.Equals(object)"/>); returns <c>false</c> if this type or any of its base types override <see cref="object.Equals(object)"/>.</returns>
        private static bool IsReferenceEquatable(Type type)
        {
            if (type.IsPointer)
                return false;

            var typeInfo = type.GetTypeInfo();

            // Only reference types can use reference equality.
            if (!typeInfo.IsClass)
                return false;

            return !OverridesEquals(new FullTypeInfo { Type = type, TypeInfo = typeInfo });
        }

        private static bool OverridesEquals(FullTypeInfo specificType)
        {
            foreach (var type in TypeAndBaseTypesExceptObject(specificType))
            {
                foreach (var method in type.TypeInfo.DeclaredMethods)
                {
                    if (!method.IsPublic || method.IsStatic || !method.IsVirtual || !method.IsHideBySig || method.Name != "Equals")
                        continue;
                    var baseDefinition = method.GetRuntimeBaseDefinition();
                    if (baseDefinition.Equals(method))
                        continue;
                    if (baseDefinition.DeclaringType == typeof(object))
                        return true;
                }
            }
            return false;
        }

        private static IEnumerable<FullTypeInfo> TypeAndBaseTypesExceptObject(FullTypeInfo type)
        {
            if (type.Type == null || type.Type == typeof(object))
                yield break;

            while (true)
            {
                yield return type;
                type.Type = type.TypeInfo.BaseType;
                if (type.Type == null || type.Type == typeof(object))
                    yield break;
                type.TypeInfo = type.Type.GetTypeInfo();
            }
        }

        private struct FullTypeInfo
        {
            public Type Type { get; set; }
            public TypeInfo TypeInfo { get; set; }
        }
    }
}
