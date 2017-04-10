The Connected Properties library is just a thin wrapper around [`ConditionalWeakTable<TKey, TValue>`](http://msdn.microsoft.com/en-us/library/dd287757.aspx).

Each "connected property scope" is actually an instance of `ConditionalWeakTable`.

The wrapper provides the following advantages over using `ConditionalWeakTable` directly:

* You may attach value types to carrier objects (`ConditionalWeakTable` values must be reference types).
* Carrier objects are checked at runtime to prevent accidentally using an improper carrier object.
* The API for accessing property values is more complete (e.g., `Set`) and does not have [vexing exceptions](https://blogs.msdn.microsoft.com/ericlippert/2008/09/10/vexing-exceptions/) (`ConditionalWeakTable` has a vexing exception for its `Add` method).
