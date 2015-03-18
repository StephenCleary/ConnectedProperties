// Copyright (c) 2013 Nito Programs.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    public static class AssertEx
    {
        public static void Throws<TException>(Action action, bool allowDerivedTypes = true)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (allowDerivedTypes && !(ex is TException))
                    Assert.Fail("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof(TException).Name + " or a derived type was expected.");
                if (!allowDerivedTypes && ex.GetType() != typeof(TException))
                    Assert.Fail("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof(TException).Name + " was expected.");
                return;
            }
            Assert.Fail("Delegate did not throw expected exception " + typeof(TException).Name + ".");
        }
    }
}
