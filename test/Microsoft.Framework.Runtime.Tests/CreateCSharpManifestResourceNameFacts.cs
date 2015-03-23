﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.Framework.Runtime.Tests
{
    public class CreateCSharpManifestResourceNameFacts
    {
        [Theory]
        [InlineData("file", "namespace", "namespace.file")]
        [InlineData("file", null, "file")]
        [InlineData("folder/file", "namespace", "namespace.folder.file")]
        [InlineData("folder/file", null, "folder.file")]
        [InlineData("folder/_/file", null, "folder.__.file")]
        [InlineData("myResource.resx", "myNamespace", "myNamespace.myResource.resx")]
        [InlineData("myResource.txt", "myNamespace", "myNamespace.myResource.txt")]

        public void ValidateGeneratedCSharpManifestResourceNames(string fileName, string rootNamespace, string expectedName)
        {
            var generatedName = CreateCSharpManifestResourceName.CreateManifestName(fileName, rootNamespace);
            Assert.Equal(expectedName, generatedName); 
        }
    }
}