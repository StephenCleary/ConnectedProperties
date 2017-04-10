The default connected property [scope](scope.md) instance is _per-AppDomain_. This means that the same names used in different AppDomains refer to different properties.

When an AppDomain is unloaded, all property values connected by the default connected property scope in that AppDomain become eligible for garbage collection.