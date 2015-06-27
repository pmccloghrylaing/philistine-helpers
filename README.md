# Philistine.Helpers

## Switch
This is a C# type "switch" inspired by Scala's. Each case consists of a predicate and a result selector, or for `default` just a result selector.

### Constructor Case Statements
Constructor case statements accept an expression that represents the construction of the control object, either through the constructor or through a factory method. This doesn't need to represent how the object was actually created, but the supplied values need to match, as do parameter and property name e.g.

```c#
class Person
{
  public string Name { get; set; }
  public int Age { get; set; }
  
  public static Person Create(string name, int age)
  {
    return new Person { Name = "Bob", Age = 30 };
  }
}

var bob = new Person { Name = "Bob", Age = 30 };

bob.Switch()
  .CaseConstructed(() => Person.Create("Bob", 30), p => true)
  .Default(p => false); // returns true

bob.Switch()
  .CaseConstructed(() => new Person { Name = "Bob" }, p => true)
  .Default(p => false); // returns true
```

Additionally, parameters can be added to the expression which will then be supplied to the result selector, e.g.

```c#
bob.Switch()
  .CaseConstructed((string n, int i) => new Person { Name = n, Age = i }, (p, n, i) => n)
  .Value; // return true
```
