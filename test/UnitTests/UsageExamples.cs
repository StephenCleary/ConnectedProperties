using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Nito.ConnectedProperties;

namespace UnitTests
{
    public class UsageExamples
    {
        [Fact]
        public void SimpleUsage()
        {
            var obj = new object();

            ConnectedProperty.Set(obj, "Name", "Bob");

            Assert.Equal("Bob", ReadName(obj));
        }

        private static string ReadName(object obj) => ConnectedProperty.Get(obj, "Name");
    }
}
