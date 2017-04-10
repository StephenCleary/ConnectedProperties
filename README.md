# ConnectedProperties [![AppVeyor](https://img.shields.io/appveyor/ci/StephenCleary/ConnectedProperties.svg?style=plastic)](https://ci.appveyor.com/project/StephenCleary/ConnectedProperties) [![Coveralls](https://img.shields.io/coveralls/StephenCleary/ConnectedProperties.svg?style=plastic)](https://coveralls.io/r/StephenCleary/ConnectedProperties)

Dynamically attach properties to (almost) any object instance.

[API Docs](http://dotnetapis.com/pkg/Nito.ConnectedProperties)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Nito.ConnectedProperties.svg?style=plastic)](https://www.nuget.org/packages/Nito.ConnectedProperties/)

## The 2-Minute Intro: Connecting a "Name" to (almost) any Object

The following code shows how to connect a "Name" property to an object:

````C#
// Use the Connected Properties library.
using Nito.ConnectedProperties;

class Program
{
  // Display the Name of any object passed into this method.
  public static void DisplayName(object obj)
  {
    // Look up a connected property called "Name".
    var name = ConnectedProperty.Get(obj, "Name");
    Console.WriteLine("Name: " + name);
  }

  static void Main()
  {
    // Create an object to name.
    var obj = new object();

    // I dub this object "Bob".
    ConnectedProperty.Set(obj, "Name", "Bob");

    // Pass the object to the DisplayName method, which is able to retrieve the connected property.
    DisplayName(obj);
  }
}
````

Note that the lifetime of the connected property is exactly as if it was a real property of the object, and the lifetime of the object is not changed in any way (even if the property refers to the object). Connected properties are a true [ephemeron implementation](http://en.wikipedia.org/wiki/Ephemeron) for .NET.

See the [documentation](doc/README.md) for all kinds of fun details.
