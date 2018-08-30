---
name: Bug report
about: Create a report to help us improve

---

### Describe the bug

A clear and concise description of what the bug is.

### To Reproduce

Please provide UnitTest in the form of:

```C#
[TestMethod]
public void SomeDescriptiveName()
{
    var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

    var resolver = container.Resolve<Func<ILogger>>();
    ...
    AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
}
```

### Expected behavior and what is wrong

A clear and concise description.

**Additional context**

Add any other context about the problem here.
