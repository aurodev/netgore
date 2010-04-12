﻿using System;
using System.Linq;
using NetGore.Content;
using NetGore.IO;
using NUnit.Framework;

namespace NetGore.Tests.NetGore.IO
{
    [TestFixture]
    public class ContentAssetNameTests
    {
        #region Unit tests

        [Test]
        public void FromAbsoluteFilePathAlternateSeparatorDeepTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:/whatever/path/to/mycontent/is/super/awesome",
                                                                       @"C:/whatever/path/to");
            Assert.AreEqual(@"mycontent/is/super/awesome".Replace("/", ContentAssetName.PathSeparator), n.Value);
        }

        [Test]
        public void FromAbsoluteFilePathAlternateSeparatorTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:/whatever/path/to/mycontent", @"C:/whatever/path/to");
            Assert.AreEqual("mycontent", n.Value);
        }

        [Test]
        public void FromAbsoluteFilePathCapsTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:\whatever\path\to\mycontent".ToUpper(),
                                                                       @"C:\whatever\path\to");
            Assert.AreEqual("mycontent", n.Value.ToLower());
        }

        [Test]
        public void FromAbsoluteFilePathDeepTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:\whatever\path\to\mycontent\is\super\awesome",
                                                                       @"C:\whatever\path\to");
            Assert.AreEqual(@"mycontent\is\super\awesome".Replace("\\", ContentAssetName.PathSeparator), n.Value);
        }

        [Test]
        public void FromAbsoluteFilePathTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:\whatever\path\to\mycontent", @"C:\whatever\path\to");
            Assert.AreEqual("mycontent", n.Value);
        }

        [Test]
        public void FromAbsoluteFilePathTrailingSlashTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:\whatever\path\to\mycontent", @"C:\whatever\path\to\");
            Assert.AreEqual("mycontent", n.Value);
        }

        [Test]
        public void FromAbsoluteFilePathWithSuffixCapsTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath((@"C:\whatever\path\to\mycontent" + ContentPaths.ContentFileSuffix).ToUpper(),
                                                                       @"C:\whatever\path\to");
            Assert.AreEqual("mycontent", n.Value.ToLower());
        }

        [Test]
        public void FromAbsoluteFilePathWithSuffixTest()
        {
            ContentAssetName n = ContentAssetName.FromAbsoluteFilePath(@"C:\whatever\path\to\mycontent" + ContentPaths.ContentFileSuffix,
                                                                       @"C:\whatever\path\to");
            Assert.AreEqual("mycontent", n.Value);
        }

        [Test]
        public void GetAbsoluteFilePathAlternateSeparatorPrefixTest()
        {
            string s = new ContentAssetName("/myasset").GetAbsoluteFilePath(ContentPaths.Build);
            Assert.IsTrue(s.EndsWith("Content\\myasset" + ContentPaths.ContentFileSuffix, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void GetAbsoluteFilePathSeparatorPrefixTest()
        {
            string s = new ContentAssetName("\\myasset").GetAbsoluteFilePath(ContentPaths.Build);
            Assert.IsTrue(s.EndsWith("Content\\myasset" + ContentPaths.ContentFileSuffix, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void GetAbsoluteFilePathTest()
        {
            string s = new ContentAssetName("myasset").GetAbsoluteFilePath(ContentPaths.Build);
            Assert.IsTrue(s.EndsWith("Content\\myasset" + ContentPaths.ContentFileSuffix, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void SanitizeAlternateSeparatorTest()
        {
            const string s = @"asdf/basdf/wer/asdf";
            Assert.AreEqual(s.Replace("/", ContentAssetName.PathSeparator), ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizePrefixedAlternateSeparatorTest()
        {
            const string s = @"/asdf";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizePrefixedAndSuffixedAlternateSeparatorTest()
        {
            const string s = @"/asdf/";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizePrefixedAndSuffixedSeparatorTest()
        {
            const string s = @"\asdf\";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizePrefixedSeparatorTest()
        {
            const string s = @"\asdf";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizeSeparatorTest()
        {
            const string s = @"asdf\basdf\wer\asdf";
            Assert.AreEqual(s.Replace("\\", ContentAssetName.PathSeparator), ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizeSuffixedAlternateSeparatorTest()
        {
            const string s = @"asdf/";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        [Test]
        public void SanitizeSuffixedSeparatorTest()
        {
            const string s = @"asdf\";
            Assert.AreEqual("asdf", ContentAssetName.Sanitize(s));
        }

        #endregion
    }
}