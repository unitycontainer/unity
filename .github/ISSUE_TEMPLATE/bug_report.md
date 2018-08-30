---
name: Bug report
about: Create a report to help us improve

---

### Description

A clear and concise description of what is wrong.

### To Reproduce

Please provide UnitTest in the form of:

```C#
[TestMethod]
public void SomeDescriptiveName()
{
    var container = new UnityContainer()
                .RegisterType<ISomeType, SomeType>();
    ...

    var res = container.Resolve<Func<ISomeType>>();
    
    AssertExtensions.IsInstanceOfType(res, typeof(SomeType));
}
```

**Additional context**

Add any other context about the problem here.
