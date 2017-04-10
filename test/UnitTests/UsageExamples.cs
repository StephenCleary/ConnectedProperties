using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Nito.ConnectedProperties.ConnectedProperty;

namespace UnitTests
{
    class UsageExamples
    {
        [Fact]
        public void SimpleUsage()
        {
            var obj = new object();

            var nameProperty = GetConnectedProperty(obj, "Name");
            nameProperty.Set("Bob");

            Assert.Equal("Bob", ReadName(obj));
        }

        private static string ReadName(object obj)
        {
            var nameProperty = GetConnectedProperty(obj, "Name");
            return nameProperty.Get();
        }
    }
}
