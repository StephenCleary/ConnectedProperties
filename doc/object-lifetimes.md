Connected properties do not extend the lifetime of their carrier objects in any way. This is true even if the property value references its carrier object.

A property that is connected to a carrier object will become eligible for garbage collection when _any_ of the following conditions take place:
- The carrier object becomes eligible for garbage collection. When a carrier object is garbage collected, all of its connected property values become eligible for garbage collection.
- The property is disconnected. If the property is explicitly disconnected, then its former value will become eligible for garbage collection immediately.
- The connected property scope becomes eligible for garbage collection. When a scope is garbage collected, all of its property values that are connected to any carrier objects become eligible for garbage collection (however, see the note below).
**Note:** This last point only applies to connected property scopes that you explicitly create. The default connected property scope is only garbage collected when the AppDomain shuts down.
(This is assuming that there are no other references to the property value; if there are, then of course it does not become eligible for garbage collection.)

Note that if you have a property value that references a scope (even its own scope), then the lifetime of the scope is extended to that of the property value.

Connected property values are never disposed, but they will be finalized.
