﻿using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renci.SshNet.Sftp;
using Renci.SshNet.Tests.Common;

namespace Renci.SshNet.Tests.Classes.Sftp
{
    [TestClass]
    public class SftpFileStreamTest_Ctor_FileModeAppend_FileAccessRead : SftpFileStreamTestBase
    {
        private Random _random;
        private string _path;
        private FileMode _fileMode;
        private FileAccess _fileAccess;
        private int _bufferSize;
        private ArgumentException _actualException;

        protected override void SetupData()
        {
            base.SetupData();

            _random = new Random();
            _path = _random.Next().ToString();
            _fileMode = FileMode.Append;
            _fileAccess = FileAccess.Read;
            _bufferSize = _random.Next(5, 1000);
        }

        protected override void SetupMocks()
        {
        }

        protected override void Act()
        {
            SftpFileStream fs = null;

            try
            {
                fs = new SftpFileStream(SftpSessionMock.Object, _path, _fileMode, _fileAccess, _bufferSize);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                _actualException = ex;
            }
            finally
            {
                fs?.Dispose();
            }
        }

        [TestMethod]
        public void CtorShouldHaveThrownArgumentException()
        {
            Assert.IsNotNull(_actualException);
            Assert.IsNull(_actualException.InnerException);
            ArgumentExceptionAssert.MessageEquals(string.Format(CultureInfo.InvariantCulture,
                                                                "{0} mode can be requested only when combined with write-only access.",
                                                                _fileMode),
                                                  _actualException);
            Assert.AreEqual("mode", _actualException.ParamName);
        }
    }
}
