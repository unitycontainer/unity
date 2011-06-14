using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    public partial class VirtualMethodOverrideFixture
    {
        [TestMethod]
        public void MemberNameMatchingRuleMatchesNameInDoubleDerivedClass()
        {
            var rule = new MemberNameMatchingRule("DoSomething");
            MethodInfo memberInfo = typeof (DoubleDerivedClass).GetMethod("DoSomething");

            Assert.IsTrue(rule.Matches(memberInfo));
        }
    }
}
