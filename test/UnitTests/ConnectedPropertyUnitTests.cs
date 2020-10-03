﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Nito.ConnectedProperties;
using Xunit;

namespace UnitTests
{
    public class ConnectedPropertyUnitTests
    {
        [Fact]
        public void TryConnect_WhenDisconnected_ReturnsTrueAndSetsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            Assert.True(ConnectedProperty.GetConnectedProperty(carrier, name).TryConnect(17));
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void TryConnect_WhenConnected_ReturnsFalseAndDoesNotSetValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryConnect(17));
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void Connect_WhenDisconnected_SetsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Connect(13);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void Connect_WhenConnected_ThrowsAndDoesNotModifyValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Connect(13);
            var property = ConnectedProperty.GetConnectedProperty(carrier, name);
            Assert.Throws<InvalidOperationException>(() => property.Connect(17));
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void TryDisconnect_WhenConnected_ReturnsTrueAndDisconnects()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            Assert.True(ConnectedProperty.GetConnectedProperty(carrier, name).TryDisconnect());
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryDisconnect());
        }

        [Fact]
        public void TryDisconnect_WhenDisconnected_ReturnsFalse()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryDisconnect());
        }

        [Fact]
        public void Disconnect_WhenConnected_Disconnects()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            ConnectedProperty.GetConnectedProperty(carrier, name).Disconnect();
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryDisconnect());
        }

        [Fact]
        public void Disconnect_WhenDisconnected_Throws()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var property = ConnectedProperty.GetConnectedProperty(carrier, name);
            Assert.Throws<InvalidOperationException>(() => property.Disconnect());
        }

        [Fact]
        public void TryGet_WhenConnected_ReturnsTrueAndValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            dynamic value;
            Assert.True(ConnectedProperty.GetConnectedProperty(carrier, name).TryGet(out value));
            Assert.Equal(13, value);
        }

        [Fact]
        public void Get_WhenConnected_ReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void TryGet_WhenDisconnected_ReturnsFalse()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            dynamic value;
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryGet(out value));
        }

        [Fact]
        public void Get_WhenDisconnected_Throws()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var property = ConnectedProperty.GetConnectedProperty(carrier, name);
            Assert.Throws<InvalidOperationException>(() => property.Get());
        }

        [Fact]
        public void GetOrConnect_WhenDisconnected_SetsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).GetOrConnect(13);
            Assert.Equal(13, result);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void GetOrConnect_WhenConnected_ReturnsExistingValueAndDoesNotModifyValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(17);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).GetOrConnect(13);
            Assert.Equal(17, result);
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void Set_WhenDisconnected_SetsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(17);
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void Set_WhenConnected_OverwritesExistingValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(17);
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void ConnectOrUpdate_WhenDisconnected_SetsAndReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).ConnectOrUpdate(13, x => x + 2);
            Assert.Equal(13, result);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void ConnectOrUpdate_WhenConnected_UpdatesAndReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).ConnectOrUpdate(13, x => x + 2);
            Assert.Equal(15, result);
            Assert.Equal(15, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void TryUpdate_WhenDisconnected_ReturnsFalseAndDoesNotSetValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).TryUpdate(19, 17);
            Assert.False(result);
            Assert.False(ConnectedProperty.GetConnectedProperty(carrier, name).TryDisconnect());
        }

        [Fact]
        public void TryUpdate_WhenConnectedWithNonmatchingValue_ReturnsFalseAndDoesNotSetValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).TryUpdate(19, 17);
            Assert.False(result);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void TryUpdate_WhenConnectedWithMatchingValue_ReturnsTrueAndSetsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).TryUpdate(19, 13);
            Assert.True(result);
            Assert.Equal(19, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void GetOrCreate_WhenConnected_GetsValueAndDoesNotSetValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(13);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).GetOrCreate(() => 17);
            Assert.Equal(13, result);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void GetOrCreate_WhenDisconnected_SetsAndReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).GetOrCreate(() => 17);
            Assert.Equal(17, result);
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void CreateOrUpdate_WhenDisconnected_SetsAndReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).CreateOrUpdate(() => 13, x => x + 2);
            Assert.Equal(13, result);
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void CreateOrUpdate_WhenConnected_UpdatesAndReturnsValue()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(17);
            var result = ConnectedProperty.GetConnectedProperty(carrier, name).CreateOrUpdate(() => 13, x => x + 2);
            Assert.Equal(19, result);
            Assert.Equal(19, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
        }

        [Fact]
        public void Properties_WithDifferentNames_HaveIndependentValues()
        {
            var carrier = new object();
            var name1 = Guid.NewGuid().ToString("N");
            var name2 = Guid.NewGuid().ToString("N");
            ConnectedProperty.GetConnectedProperty(carrier, name1).Set(17);
            ConnectedProperty.GetConnectedProperty(carrier, name2).Set(13);
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name1).Get());
            Assert.Equal(13, ConnectedProperty.GetConnectedProperty(carrier, name2).Get());
        }

        [Fact]
        public void Properties_WithSameNameButDifferentScopes_HaveIndependentValues()
        {
            var carrier = new object();
            var name = Guid.NewGuid().ToString("N");
            var other = new ConnectedPropertyScope();
            ConnectedProperty.GetConnectedProperty(carrier, name).Set(17);
            other.GetConnectedProperty(carrier, name).Set(13);
            Assert.Equal(17, ConnectedProperty.GetConnectedProperty(carrier, name).Get());
            Assert.Equal(13, other.GetConnectedProperty(carrier, name).Get());
        }

#if DEBUG
#warning Skipping some unit tests due to DEBUG; build in Release to run all unit tests.
#else
        [Fact]
        public void Property_WhenCarrierIsAlive_IsNotCollected()
        {
            // A "dictionary mapping" with weak value references will not pass this test.
            var (carrier, valueRef) = Create();
            GC.Collect();
            Assert.True(valueRef.IsAlive);
            GC.KeepAlive(carrier);

            (object, WeakReference) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var carrier = new object();
                var value = new object();
                var valueRef = new WeakReference(value);
                ConnectedProperty.GetConnectedProperty(carrier, name).Set(value);
                return (carrier, valueRef);
            }
        }

        [Fact]
        public void Property_WhenCarrierIsCollected_IsCollected()
        {
            var (carrierRef, valueRef) = Create();
            GC.Collect();
            Assert.False(carrierRef.IsAlive);
            Assert.False(valueRef.IsAlive);

            (WeakReference, WeakReference) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var carrier = new object();
                var carrierRef = new WeakReference(carrier);
                var value = new object();
                var valueRef = new WeakReference(value);
                ConnectedProperty.GetConnectedProperty(carrier, name).Set(value);
                return (carrierRef, valueRef);
            }
        }

        [Fact]
        public void Property_ReferencingCarrier_WhenCarrierIsCollected_IsCollected()
        {
            // A "dictionary mapping" with strong value references will not pass this test.
            var (carrierRef, valueRef) = Create();
            GC.Collect();
            Assert.False(carrierRef.IsAlive);
            Assert.False(valueRef.IsAlive);

            (WeakReference, WeakReference) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var carrier = new object();
                var carrierRef = new WeakReference(carrier);
                Func<int> value = carrier.GetHashCode;
                var valueRef = new WeakReference(value);
                ConnectedProperty.GetConnectedProperty(carrier, name).Set(value);
                return (carrierRef, valueRef);
            }
        }

        [Fact]
        public void Properties_CrossReferencingCarriers_WhenCarriersAreCollected_AreCollected()
        {
            var (carrierRef1, valueRef1, carrierRef2, valueRef2) = Create();
            GC.Collect();
            Assert.False(carrierRef1.IsAlive);
            Assert.False(valueRef1.IsAlive);
            Assert.False(carrierRef2.IsAlive);
            Assert.False(valueRef2.IsAlive);

            (WeakReference, WeakReference, WeakReference, WeakReference) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var carrier1 = new object();
                var carrierRef1 = new WeakReference(carrier1);
                var carrier2 = new object();
                var carrierRef2 = new WeakReference(carrier1);
                Func<int> value1 = carrier2.GetHashCode;
                var valueRef1 = new WeakReference(value1);
                Func<int> value2 = carrier1.GetHashCode;
                var valueRef2 = new WeakReference(value2);
                ConnectedProperty.GetConnectedProperty(carrier1, name).Set(value1);
                ConnectedProperty.GetConnectedProperty(carrier2, name).Set(value2);
                return (carrierRef1, valueRef1, carrierRef2, valueRef2);
            }
        }

        [Fact]
        public void Properties_CrossReferencingCarriers_WhenOnePropertyRemainsAlive_AreNotCollected()
        {
            var (carrierRef1, valueRef1, carrierRef2, valueRef2, value1) = Create();
            GC.Collect();
            Assert.True(carrierRef1.IsAlive);
            Assert.True(valueRef1.IsAlive);
            Assert.True(carrierRef2.IsAlive);
            Assert.True(valueRef2.IsAlive);
            GC.KeepAlive(value1);

            (WeakReference, WeakReference, WeakReference, WeakReference, Func<int>) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var carrier1 = new object();
                var carrierRef1 = new WeakReference(carrier1);
                var carrier2 = new object();
                var carrierRef2 = new WeakReference(carrier1);
                Func<int> value1 = carrier2.GetHashCode;
                var valueRef1 = new WeakReference(value1);
                Func<int> value2 = carrier1.GetHashCode;
                var valueRef2 = new WeakReference(value2);
                ConnectedProperty.GetConnectedProperty(carrier1, name).Set(value1);
                ConnectedProperty.GetConnectedProperty(carrier2, name).Set(value2);
                return (carrierRef1, valueRef1, carrierRef2, valueRef2, value1);
            }
        }

        [Fact]
        public void Properties_CrossReferencingCarriersAcrossScopes_WhenCarriersAreCollected_AreCollected()
        {
            var (carrierRef1, valueRef1, carrierRef2, valueRef2) = Create();
            GC.Collect();
            Assert.False(carrierRef1.IsAlive);
            Assert.False(valueRef1.IsAlive);
            Assert.False(carrierRef2.IsAlive);
            Assert.False(valueRef2.IsAlive);

            (WeakReference, WeakReference, WeakReference, WeakReference) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var other = new ConnectedPropertyScope();
                var carrier1 = new object();
                var carrierRef1 = new WeakReference(carrier1);
                var carrier2 = new object();
                var carrierRef2 = new WeakReference(carrier1);
                Func<int> value1 = carrier2.GetHashCode;
                var valueRef1 = new WeakReference(value1);
                Func<int> value2 = carrier1.GetHashCode;
                var valueRef2 = new WeakReference(value2);
                ConnectedProperty.GetConnectedProperty(carrier1, name).Set(value1);
                other.GetConnectedProperty(carrier2, name).Set(value2);
                return (carrierRef1, valueRef1, carrierRef2, valueRef2);
            }
        }

        [Fact]
        public void Properties_CrossReferencingCarriersAcrossScopes_WhenOnePropertyRemainsAlive_AreNotCollected()
        {
            var (carrierRef1, valueRef1, carrierRef2, valueRef2, value1) = Create();
            GC.Collect();
            Assert.True(carrierRef1.IsAlive);
            Assert.True(valueRef1.IsAlive);
            Assert.True(carrierRef2.IsAlive);
            Assert.True(valueRef2.IsAlive);
            GC.KeepAlive(value1);

            (WeakReference, WeakReference, WeakReference, WeakReference, Func<int>) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var other = new ConnectedPropertyScope();
                var carrier1 = new object();
                var carrierRef1 = new WeakReference(carrier1);
                var carrier2 = new object();
                var carrierRef2 = new WeakReference(carrier1);
                Func<int> value1 = carrier2.GetHashCode;
                var valueRef1 = new WeakReference(value1);
                Func<int> value2 = carrier1.GetHashCode;
                var valueRef2 = new WeakReference(value2);
                ConnectedProperty.GetConnectedProperty(carrier1, name).Set(value1);
                other.GetConnectedProperty(carrier2, name).Set(value2);
                return (carrierRef1, valueRef1, carrierRef2, valueRef2, value1);
            }
        }

        [Fact]
        public void Property_WhenScopeIsCollected_IsCollected()
        {
            var (valueRef, carrier) = Create();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.False(valueRef.IsAlive);
            GC.KeepAlive(carrier);

            (WeakReference, object) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var scope = new ConnectedPropertyScope();
                var carrier = new object();
                var value = new object();
                var valueRef = new WeakReference(value);
                scope.GetConnectedProperty(carrier, name).Set(value);
                return (valueRef, carrier);
            }
        }

        [Fact]
        public void Property_ReferencingScope_ActsAsStrongReference()
        {
            var (valueRef, scopeRef) = Part1();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.False(valueRef.IsAlive);
            Assert.False(scopeRef.IsAlive);

            (WeakReference, WeakReference, object) Create()
            {
                var name = Guid.NewGuid().ToString("N");
                var scope = new ConnectedPropertyScope();
                var scopeRef = new WeakReference(scope);
                var carrier = new object();
                Func<int> value = scope.GetHashCode;
                var valueRef = new WeakReference(value);
                scope.GetConnectedProperty(carrier, name).Set(value);
                return (valueRef, scopeRef, carrier);
            }

            (WeakReference, WeakReference) Part1()
            {
                var (valueRef, scopeRef, carrier) = Create();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.True(valueRef.IsAlive);
                Assert.True(scopeRef.IsAlive);
                GC.KeepAlive(carrier);
                return (valueRef, scopeRef);
            }
        }
#endif
    }
}
