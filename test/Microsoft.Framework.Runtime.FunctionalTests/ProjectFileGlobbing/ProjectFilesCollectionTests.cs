﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Framework.Runtime.FileGlobbing;
using Microsoft.Framework.Runtime.FunctionalTests.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Framework.Runtime.FunctionalTests.ProjectFileGlobbing
{
    public class ProjectFilesCollectionTests : FileGlobbingTestBase
    {
        public ProjectFilesCollectionTests()
            : base()
        {
        }

        [Fact]
        public void DefaultSearchPathForSources()
        {
            var testFilesCollection = CreateFilesCollection(@"{}", "src\\project");
            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\source1.cs",
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs",
                @"src\project\sub2\source4.cs",
                @"src\project\sub2\source5.cs");
        }

        [Fact]
        public void DefaultSearchPathForContents()
        {
            var testFilesCollection = CreateFilesCollection(@"{}", "src\\project");
            VerifyFilePathsCollection(testFilesCollection.ContentFiles,
                @"src\project\content1.txt",
                @"src\project\compiler\preprocess\sub\sub\preprocess-source3.txt",
                @"src\project\compiler\shared\shared1.txt",
                @"src\project\compiler\shared\sub\shared2.txt");
        }

        [Fact]
        public void DefaultSearchPathForResources()
        {
            var testFilesCollection = CreateFilesCollection(@"{}", "src\\project");
            VerifyFilePathsCollection(testFilesCollection.ResourceFiles,
                @"src\project\compiler\resources\resource.res",
                @"src\project\compiler\resources\sub\resource2.res",
                @"src\project\compiler\resources\sub\sub\resource3.res");
        }
        
        [Fact]
        public void DefaultSearchPathForShared()
        {
            var testFilesCollection = CreateFilesCollection(@"{}", "src\\project");
            VerifyFilePathsCollection(testFilesCollection.SharedFiles,
                @"src\project\compiler\shared\shared1.cs",
                @"src\project\compiler\shared\sub\shared2.cs",
                @"src\project\compiler\shared\sub\sub\sharedsub.cs");
        }

        [Fact]
        public void DefaultSearchPathForBundleExcludeFiles()
        {
            var testFilesCollection = CreateFilesCollection(@"{}", "src\\project");
            VerifyFilePathsCollection(testFilesCollection.BundleExcludeFiles,
                @"src\project\bin\object",
                @"src\project\obj\object.o",
                @"src\project\.hidden/file1.hid",
                @"src\project\.hidden/sub/file2.hid");
        }

        [Fact]
        public void IncludeCodesUpperLevelWithSlash()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""../../lib/**/*.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\source6.cs",
                @"src\project\..\..\lib\sub3\source7.cs",
                @"src\project\..\..\lib\sub4\source8.cs");
        }

        [Fact]
        public void IncludeCodesUpperLevel()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""..\\..\\lib\\**\\*.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\source6.cs",
                @"src\project\..\..\lib\sub3\source7.cs",
                @"src\project\..\..\lib\sub4\source8.cs");
        }

        [Fact]
        public void IncludeCodesUsingUpperLevelAndRecursive()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""**\\*.cs;..\\..\\lib\\**\\*.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\source6.cs",
                @"src\project\..\..\lib\sub3\source7.cs",
                @"src\project\..\..\lib\sub4\source8.cs",
                @"src\project\source1.cs",
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs",
                @"src\project\sub2\source4.cs",
                @"src\project\sub2\source5.cs");
        }

        [Fact]
        public void IncludeCodesUsingUpperLevelAndWildcard()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""**\\*.cs;..\\..\\lib\\*.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\source6.cs",
                @"src\project\source1.cs",
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs",
                @"src\project\sub2\source4.cs",
                @"src\project\sub2\source5.cs");
        }

        [Fact]
        public void IncludeCodesUpperLevelSingleFile()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""..\\..\\lib\\sub4\\source8.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\sub4\source8.cs");
        }

        [Fact]
        public void IncludeCodesUpperLevelSingleFileAndRecursive()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""**\\*.cs;..\\..\\lib\\sub4\\source8.cs"",
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\..\..\lib\sub4\source8.cs",
                @"src\project\source1.cs",
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs",
                @"src\project\sub2\source4.cs",
                @"src\project\sub2\source5.cs");
        }

        [Fact]
        public void IncludeCodeFolder()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""sub\\""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs");
        }

        [Fact]
        public void IncludeCodeUpperLevelFolder()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""..\\project2\\""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project2\content1.txt",
                @"src\project2\source1.cs",
                @"src\project2\bin\object",
                @"src\project2\compiler\preprocess\preprocess-source1.cs",
                @"src\project2\compiler\preprocess\sub\preprocess-source2.cs",
                @"src\project2\compiler\preprocess\sub\sub\preprocess-source3.cs",
                @"src\project2\compiler\preprocess\sub\sub\preprocess-source3.txt",
                @"src\project2\compiler\resources\resource.res",
                @"src\project2\compiler\resources\sub\resource2.res",
                @"src\project2\compiler\resources\sub\sub\resource3.res",
                @"src\project2\compiler\shared\shared1.cs",
                @"src\project2\compiler\shared\shared1.txt",
                @"src\project2\compiler\shared\sub\shared2.cs",
                @"src\project2\compiler\shared\sub\shared2.txt",
                @"src\project2\compiler\shared\sub\sub\sharedsub.cs",
                @"src\project2\obj\object.o",
                @"src\project2\sub\source2.cs",
                @"src\project2\sub\source3.cs",
                @"src\project2\sub2\source4.cs",
                @"src\project2\sub2\source5.cs");
        }

        [Fact]
        public void IncludeCodeFolderWithSlash()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""sub/""
}
", @"src\project");
            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs");
        }

        [Fact]
        public void IncludeCodeFolderWithoutSlash()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""sub""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs");
        }

        [Fact]
        public void ThrowForAbosolutePath()
        {
            var absolutePath = Path.Combine(_context.RootPath, @"src\project2\sub2\source5.cs");
            var projectJsonContent = @"{""code"": """ + absolutePath.Replace("\\", "\\\\") + @"""}";

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                CreateFilesCollection(projectJsonContent, @"src\project");
            });

            Assert.True(exception.Message.Contains(absolutePath));
        }

        [Fact]
        public void IncludeCodeCurrentDirectory()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": "".\\**\\*.cs""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project\sub\source2.cs",
                @"src\project\sub\source3.cs",
                @"src\project\source1.cs",
                @"src\project\sub2\source4.cs",
                @"src\project\sub2\source5.cs");
        }

        [Fact]
        public void IncludeCodeUpperLevelWithCurrent()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""..\\..\\lib\\.""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"lib\source6.cs",
                @"lib\sub3\source7.cs",
                @"lib\sub4\source8.cs");
        }

        [Fact]
        public void IncludeCodeUpperLevelFolderWithExcluding()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""code"": ""..\\project2\\"",
    ""exclude"": ""..\\project2\\compiler\\**\\*.txt"",
    ""resources"": ""..\\project2\\compiler\\resources\\**\\*.*""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.SourceFiles,
                @"src\project2\content1.txt",
                @"src\project2\source1.cs",
                @"src\project2\bin\object",
                @"src\project2\compiler\preprocess\preprocess-source1.cs",
                @"src\project2\compiler\preprocess\sub\preprocess-source2.cs",
                @"src\project2\compiler\preprocess\sub\sub\preprocess-source3.cs",
                @"src\project2\compiler\shared\shared1.cs",
                @"src\project2\compiler\shared\sub\shared2.cs",
                @"src\project2\compiler\shared\sub\sub\sharedsub.cs",
                @"src\project2\obj\object.o",
                @"src\project2\sub\source2.cs",
                @"src\project2\sub\source3.cs",
                @"src\project2\sub2\source4.cs",
                @"src\project2\sub2\source5.cs");
        }

        [Fact]
        public void IncludeRecursivelyWithExtension()
        {
            var testFilesCollection = CreateFilesCollection(@"
{
    ""bundleExclude"": ""**.txt""
}
", @"src\project");

            VerifyFilePathsCollection(testFilesCollection.BundleExcludeFiles,
                "src/project/compiler/preprocess/sub/sub/preprocess-source3.txt",
                "src/project/compiler/shared/shared1.txt",
                "src/project/compiler/shared/sub/shared2.txt",
                "src/project/content1.txt");
        }

        protected override DisposableProjectContext CreateContext()
        {
            var context = new DisposableProjectContext();
            context.AddFiles(
                    "src/project/source1.cs",
                    "src/project/sub/source2.cs",
                    "src/project/sub/source3.cs",
                    "src/project/sub2/source4.cs",
                    "src/project/sub2/source5.cs",
                    "src/project/compiler/preprocess/preprocess-source1.cs",
                    "src/project/compiler/preprocess/sub/preprocess-source2.cs",
                    "src/project/compiler/preprocess/sub/sub/preprocess-source3.cs",
                    "src/project/compiler/preprocess/sub/sub/preprocess-source3.txt",
                    "src/project/compiler/shared/shared1.cs",
                    "src/project/compiler/shared/shared1.txt",
                    "src/project/compiler/shared/sub/shared2.cs",
                    "src/project/compiler/shared/sub/shared2.txt",
                    "src/project/compiler/shared/sub/sub/sharedsub.cs",
                    "src/project/compiler/resources/resource.res",
                    "src/project/compiler/resources/sub/resource2.res",
                    "src/project/compiler/resources/sub/sub/resource3.res",
                    "src/project/content1.txt",
                    "src/project/obj/object.o",
                    "src/project/bin/object",
                    "src/project/.hidden/file1.hid",
                    "src/project/.hidden/sub/file2.hid",
                    "src/project2/source1.cs",
                    "src/project2/sub/source2.cs",
                    "src/project2/sub/source3.cs",
                    "src/project2/sub2/source4.cs",
                    "src/project2/sub2/source5.cs",
                    "src/project2/compiler/preprocess/preprocess-source1.cs",
                    "src/project2/compiler/preprocess/sub/preprocess-source2.cs",
                    "src/project2/compiler/preprocess/sub/sub/preprocess-source3.cs",
                    "src/project2/compiler/preprocess/sub/sub/preprocess-source3.txt",
                    "src/project2/compiler/shared/shared1.cs",
                    "src/project2/compiler/shared/shared1.txt",
                    "src/project2/compiler/shared/sub/shared2.cs",
                    "src/project2/compiler/shared/sub/shared2.txt",
                    "src/project2/compiler/shared/sub/sub/sharedsub.cs",
                    "src/project2/compiler/resources/resource.res",
                    "src/project2/compiler/resources/sub/resource2.res",
                    "src/project2/compiler/resources/sub/sub/resource3.res",
                    "src/project2/content1.txt",
                    "src/project2/obj/object.o",
                    "src/project2/bin/object",
                    "lib/source6.cs",
                    "lib/sub3/source7.cs",
                    "lib/sub4/source8.cs",
                    "res/resource1.text",
                    "res/resource2.text",
                    "res/resource3.text",
                    ".hidden/file1.hid",
                    ".hidden/sub/file2.hid");

            return context;
        }

        protected virtual ProjectFilesCollection CreateFilesCollection(string jsonContent, string projectDir)
        {
            var rawProject = JsonConvert.DeserializeObject<JObject>(jsonContent);
            var filesCollection = new ProjectFilesCollection(rawProject, Path.Combine(_context.RootPath, projectDir));

            return filesCollection;
        }
    }
}