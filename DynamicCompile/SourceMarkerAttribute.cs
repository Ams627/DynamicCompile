using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DynamicCompile
{
    public class SourceMarkerAttribute : Attribute
    {
        private static Dictionary<string, List<string>> _groupToSources = new Dictionary<string, List<string>>();
        public SourceMarkerAttribute(string testGroup, [CallerFilePath] string sourcePath = "")
        {
            SourcePath = sourcePath;
            TestGroup = testGroup;

            if (!_groupToSources.TryGetValue(testGroup, out var sourceFileList))
            {
                sourceFileList = new List<string>();
                _groupToSources.Add(testGroup, sourceFileList);
            }
            sourceFileList.Add(sourcePath);
        }

        public string SourcePath { get; private set; }
        public string TestGroup { get; private set; }

        public static string[] GetGroupSources(string name) => _groupToSources[name].ToArray();
        public static Dictionary<string, List<string>> GetAllSources => _groupToSources;
    }
}