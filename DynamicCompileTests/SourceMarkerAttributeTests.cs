using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCompile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reflection;

namespace DynamicCompile.Tests
{
    [TestClass()]
    public class SourceMarkerAttributeTests
    {
        [TestMethod()]
        public void BasicTest()
        {
            var sourcePath = @"c:\temp\a1.cs";
            var testGroup = "GroupA";
            var att = new SourceMarkerAttribute(testGroup:testGroup, sourcePath:sourcePath);
            att.TestGroup.Should().Be(testGroup);
            att.SourcePath.Should().Be(sourcePath);
        }

        [TestMethod()]
        [DoNotParallelize] // due to statics
        public void GroupSourcesTest()
        {
            var testData = new[]
            {
                (groupName: "GroupA", filename: @"c:\temp\a1.cs"),
                (groupName: "GroupA", filename: @"c:\temp\b1.cs"),
                (groupName: "GroupA", filename: @"c:\temp\c1.cs"),
                (groupName: "GroupB", filename: @"c:\temp\d1.cs"),
                (groupName: "GroupB", filename: @"c:\temp\e1.cs"),
                (groupName: "GroupC", filename: @"c:\temp\f1.cs"),
                (groupName: "GroupC", filename: @"c:\temp\g1.cs"),
                (groupName: "GroupD", filename: @"c:\temp\h1.cs"),
                (groupName: "GroupD", filename: @"c:\temp\i1.cs"),
                (groupName: "GroupD", filename: @"c:\temp\j1.cs"),
                (groupName: "GroupD", filename: @"c:\temp\k1.cs"),
            };

            foreach (var (groupName, filename) in testData)
            {
                var att = new SourceMarkerAttribute(groupName, filename);
                att.TestGroup.Should().Be(groupName);
                att.SourcePath.Should().Be(filename);
            }

            SourceMarkerAttribute.AllSources.Should().BeEquivalentTo(testData.Select(x => x.filename).ToArray());

            var grouped = testData.ToLookup(x => x.groupName);
            foreach (var g in grouped)
            {
                SourceMarkerAttribute.GroupSources(g.Key).Should().BeEquivalentTo(g.Select(x => x.filename));
            }

            var allSources = SourceMarkerAttribute.AllSourcesByGroup;

            foreach (var g in grouped)
            {
                allSources.Keys.Should().Contain(g.Key);
                allSources[g.Key].Should().BeEquivalentTo(g.Select(x=>x.filename));
            }
        }

        [TestMethod]
        [DoNotParallelize] // due to statics
        public void NotGrouped()
        {
            var testFilenames = new[]
            {
                @"c:\temp\a1.cs",
                @"c:\temp\b1.cs",
                @"c:\temp\c1.cs",
                @"c:\temp\d1.cs",
                @"c:\temp\e1.cs",
                @"c:\temp\f1.cs",
                @"c:\temp\g1.cs",
                @"c:\temp\h1.cs",
                @"c:\temp\i1.cs",
                @"c:\temp\j1.cs",
                @"c:\temp\k1.cs",
            };

            foreach (var filename in testFilenames)
            {
                var att = new SourceMarkerAttribute(sourcePath:filename);
                att.TestGroup.Should().Be(string.Empty);
                att.SourcePath.Should().Be(filename);
            }

            SourceMarkerAttribute.AllSources.Should().BeEquivalentTo(testFilenames);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var cleanupMethod = typeof(SourceMarkerAttribute).GetMethod("Cleanup", BindingFlags.NonPublic | BindingFlags.Static);
            cleanupMethod.Invoke(null, null);
        }
    }
}