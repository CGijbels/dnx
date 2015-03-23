// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using NuGet;
using System;

namespace Microsoft.Framework.Runtime
{
    public class ResxResourceProvider : IResourceProvider
    {
        public IList<ResourceDescriptor> GetResources(Project project)
        {
            string root = PathUtility.EnsureTrailingSlash(project.ProjectDirectory);
            return project
                   .Files.ResourceFiles
                   .Where(res => IsResxResourceFile(res))
                   .Select(resxFilePath =>
                        new ResourceDescriptor()
                        {
                            Name = CreateCSharpManifestResourceName.CreateManifestName(
                                 ResourcePathUtility.GetResourceName(root, resxFilePath),
                                 project.Name),
                            Stream = () => GetResourceStream(resxFilePath),
                        })
                   .ToList();
        }

        public static bool IsResxResourceFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);

            return
                string.Compare(ext, ".resx", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(ext, ".restext", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(ext, ".resources", StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static Stream GetResourceStream(string resxFilePath)
        {
            using (var fs = File.OpenRead(resxFilePath))
            {
                var document = XDocument.Load(fs);

                var ms = new MemoryStream();
                var rw = new ResourceWriter(ms);

                foreach (var e in document.Root.Elements("data"))
                {
                    string name = e.Attribute("name").Value;
                    string value = e.Element("value").Value;

                    rw.AddResource(name, value);
                }

                rw.Generate();
                ms.Seek(0, SeekOrigin.Begin);

                return ms;
            }
        }
    }
}