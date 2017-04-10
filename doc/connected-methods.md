Connected properties only allow attaching _data_ to carrier objects, but you may attach a delegate as data. It can then be used as a connected method.

This is similar to setting delegates on an ExpandoObject.

````C#
private const string LogMethod = Guid.NewGuid().ToString("N");

public void Log(object obj)
{
  var logMethodProperty = ConnectedProperty.GetConnectedProperty(obj, LogMethod);
  Action logMethod;
  if (logMethodProperty.TryGet(out logMethod))
  {
    // This instance has a connected Log method.
    logMethod();
  }
  else
  {
    // This instance does not have a connected Log method.
    GenericLog();
  }
}
````

Note that connected methods are different than extension methods. Extension methods extend a _type_, and can be called for any instance of that type. Connected methods extend an _instance_, so they can be overridden on a per-instance basis.