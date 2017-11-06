﻿using NUnit.Framework;
using Playnite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayniteTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void NormalizeGameNameTest()
        {
            Assert.AreEqual("Command & Conquer Red Alert 3: Uprising",
                StringExtensions.NormalizeGameName("Command®   & Conquer™ Red Alert 3™: Uprising©"));
        }
    }
}