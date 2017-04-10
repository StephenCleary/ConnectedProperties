A carrier object is an object that may have properties connected to it.

Carrier objects must be reference types; value types cannot have properties connected to them.

Furthermore, carrier objects must use reference equality. Types such as `String` do not qualify as carrier objects. This restriction is checked at runtime, and the Connected Properties library will throw an `InvalidOperationException` if you attempt to connect a property to an invalid carrier object.

As a result of these restrictions, two different carrier object variables have the same connected property instances iff the two objects are equal (i.e., both objects are the same instance).

In advanced scenarios, you may need to connect properties to a carrier object that does not use reference equality. You may bypass carrier object validation by passing a value of `true` for the `bypassValidation` argument.
