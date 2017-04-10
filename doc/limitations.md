# Limited Enumeration

It is _not_ possible to enumerate _all_ properties connected to a given carrier object by all [connected property scopes](scope.md).

It is _not_ possible to enumerate all carrier objects for a given [connected property scope](scope.md).

# Not a GC Callback

Since connected property values are not disposed, they _cannot_ be used as an "object has been garbage collected" type of callback.
