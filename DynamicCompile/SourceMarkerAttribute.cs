using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DynamicCompile
{
    /// <summary>
    /// Simple attribute class for marking a class so that the name of the source containing the class can
    /// be obtained (for partial classes, the name obtained will be that of the file where the attribute is used).
    /// Static methods are provided to access information about all instances of the class.
    /// Classes are grouped using the testGroup attribute parameter so that you can get all classes marked with a particular
    /// group using the GroupSources property. You can also call AllSources to get all source files defined by the attribute
    /// or AllSourcesByGroup to get 
    /// </summary>
    public class SourceMarkerAttribute : Attribute
    {
        private static Dictionary<string, List<string>> _groupToSources = new Dictionary<string, List<string>>();

        public SourceMarkerAttribute(string testGroup = "", [CallerFilePath] string sourcePath = "")
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

        public static string[] GroupSources(string name) => _groupToSources[name].ToArray();

        public static string[] AllSources => _groupToSources.Values.SelectMany(x => x).ToArray();
        public static Dictionary<string, List<string>> AllSourcesByGroup => _groupToSources;

        // used in testing only:
        private static void Cleanup()
        {
            _groupToSources.Clear();
        }
    }
}