using System;
using System.Collections.Concurrent;

namespace Nito.ConnectedProperties
{
    /// <summary>
    /// A property that may be connected to a carrier object at runtime.
    /// The property is either connected or disconnected. A disconnected property is different than a connected property value of <c>null</c>.
    /// All members are threadsafe.
    /// </summary>
    public sealed class ConnectedProperty
    {
        /// <summary>
        /// The dictionary representing the properties for our carrier object.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _dictionary;

        /// <summary>
        /// The key for this particular property.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// Creates a connected property with the specified properties dictionary and property key.
        /// </summary>
        /// <param name="dictionary">The dictionary representing the properties for our carrier object.</param>
        /// <param name="key">The key for this particular property.</param>
        internal ConnectedProperty(ConcurrentDictionary<string, object> dictionary, string key)
        {
            _dictionary = dictionary;
            _key = key;
        }

        /// <summary>
        /// Gets a connected property with the specified name. Throws <see cref="InvalidOperationException"/> if <paramref name="carrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="carrier">The carrier object for this property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="bypassValidation">An optional value indicating whether to bypass carrier object validation. The default is <c>false</c>.</param>
        public static ConnectedProperty GetConnectedProperty(object carrier, string name, bool bypassValidation = false)
        {
            return ConnectedPropertyScope.Default.GetConnectedProperty(carrier, name, bypassValidation);
        }

        /// <summary>
        /// Copies all connected properties in the default scope from one carrier object to another. Throws <see cref="InvalidOperationException"/> if either <paramref name="fromCarrier"/> or <paramref name="toCarrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="fromCarrier">The carrier object acting as the source of connected properties.</param>
        /// <param name="toCarrier">The carrier object acting as the destination of connected properties.</param>
        public static void CopyAll(object fromCarrier, object toCarrier)
        {
            ConnectedPropertyScope.Default.CopyAll(fromCarrier, toCarrier);
        }

        /// <summary>
        /// Attempts to disconnect the property. Returns <c>true</c> if the property was disconnected by this method; <c>false</c> if the property was already disconnected.
        /// </summary>
        public bool TryDisconnect()
        {
            object junk;
            return _dictionary.TryRemove(_key, out junk);
        }

        /// <summary>
        /// Gets the value of the property, if it is connected. Returns <c>true</c> if the property was returned in <paramref name="value"/>; <c>false</c> if the property was disconnected.
        /// </summary>
        /// <param name="value">Receives the value of the property, if this method returns <c>true</c>.</param>
        public bool TryGet(out object value)
        {
            return _dictionary.TryGetValue(_key, out value);
        }

        /// <summary>
        /// Sets the value of the property, if it is disconnected. Otherwise, does nothing. Returns <c>true</c> if the property value was set; <c>false</c> if the property was already connected.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public bool TryConnect(object value)
        {
            return _dictionary.TryAdd(_key, value);
        }

        /// <summary>
        /// Updates the value of the property, if the existing value matches a comparision value. Otherwise, does nothing. Returns <c>true</c> if the property value was updated; <c>false</c> if the comparision failed. The comparision is done using the default object equality comparer.
        /// </summary>
        /// <param name="newValue">The value to set.</param>
        /// <param name="comparisonValue">The value to compare to the existing value.</param>
        public bool TryUpdate(object newValue, object comparisonValue)
        {
            return _dictionary.TryUpdate(_key, newValue, comparisonValue);
        }

        /// <summary>
        /// Gets the value of the property, if it is connected; otherwise, sets the value of the property and returns the new value. <paramref name="createCallback"/> may be invoked multiple times.
        /// </summary>
        /// <param name="createCallback">The delegate invoked to create the value of the property, if it is disconnected. May not be <c>null</c>. If there is a multithreaded race condition, each thread's delegate may be invoked, but all values except one will be discarded.</param>
        /// <returns>The value of the property.</returns>
        public dynamic GetOrCreate(Func<object> createCallback)
        {
            return _dictionary.GetOrAdd(_key, _ => createCallback());
        }

        /// <summary>
        /// Sets the value of a property, connecting it if necessary. <paramref name="createCallback"/> and <paramref name="updateCallback"/> may be invoked multiple times. Returns the new value of the property.
        /// </summary>
        /// <param name="createCallback">The delegate invoked to create the value if the property is not connected.</param>
        /// <param name="updateCallback">The delegate invoked to update the value if the property is connected.</param>
        public dynamic CreateOrUpdate(Func<object> createCallback, Func<dynamic, object> updateCallback)
        {
            return _dictionary.AddOrUpdate(_key, _ => createCallback(), (_, oldValue) => updateCallback(oldValue));
        }

        /// <summary>
        /// Disconnects the property, throwing an exception if the property was already disconnected.
        /// </summary>
        public void Disconnect()
        {
            if (!TryDisconnect())
                throw new InvalidOperationException("Connected property was already disconnected.");
        }

        /// <summary>
        /// Gets the value of the property, throwing an exception if the property was disconnected.
        /// </summary>
        /// <returns>The value of the property.</returns>
        public dynamic Get()
        {
            object ret;
            if (!TryGet(out ret))
                throw new InvalidOperationException("Connected property is disconnected.");
            return ret;
        }

        /// <summary>
        /// Gets the value of the property. Throws <see cref="InvalidOperationException"/> if <paramref name="carrier"/> is not a valid carrier object or if the property specified by <paramref name="name"/> was disconnected.
        /// </summary>
        /// <param name="carrier">The carrier object for this property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public static dynamic Get(object carrier, string name) => GetConnectedProperty(carrier, name).Get();

        /// <summary>
        /// Sets the value of the property, throwing an exception if the property was already connected.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Connect(object value)
        {
            if (!TryConnect(value))
                throw new InvalidOperationException("Connected property was already connected.");
        }

        /// <summary>
        /// Gets the value of the property, if it is connected; otherwise, sets the value of the property and returns the new value.
        /// </summary>
        /// <param name="connectValue">The new value of the property, if it is disconnected.</param>
        /// <returns>The value of the property.</returns>
        public dynamic GetOrConnect(object connectValue)
        {
            return GetOrCreate(() => connectValue);
        }

        /// <summary>
        /// Connects a new value or updates the existing value of the property. <paramref name="updateCallback"/> may be invoked multiple times. Returns the new value of the property.
        /// </summary>
        /// <param name="connectValue">The value to set if the property is not connected.</param>
        /// <param name="updateCallback">The delegate invoked to update the value if the property is connected.</param>
        public dynamic ConnectOrUpdate(object connectValue, Func<dynamic, object> updateCallback)
        {
            return CreateOrUpdate(() => connectValue, updateCallback);
        }

        /// <summary>
        /// Sets the value of the property, overwriting any existing value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Set(object value)
        {
            CreateOrUpdate(() => value, _ => value);
        }

        /// <summary>
        /// Sets the value of the property, overwriting any existing value. Throws <see cref="InvalidOperationException"/> if <paramref name="carrier"/> is not a valid carrier object.
        /// </summary>
        /// <param name="carrier">The carrier object for this property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        public static void Set(object carrier, string name, object value) => GetConnectedProperty(carrier, name).Set(value);
    }
}
