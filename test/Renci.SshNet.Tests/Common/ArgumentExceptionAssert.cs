﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renci.SshNet.Tests.Common
{
    public static class ArgumentExceptionAssert
    {
        public static void MessageEquals(string expected, ArgumentException exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var newMessage = new ArgumentException(expected, exception.ParamName);

            Assert.AreEqual(newMessage.Message, exception.Message);
        }
    }
}
