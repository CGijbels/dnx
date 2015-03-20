// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Runtime.Compilation;

namespace Microsoft.Framework.Runtime.Roslyn
{
    public class EmbeddedResourceProvider : IResourceProvider
    {
        public IList<ResourceDescription> GetResources(ICompilationProject project)
        {
            string root = project.ProjectDirectory;
            if (root.Length > 0 && root[root.Length - 1] != '/')
            {
                root = root + "/";
            }

            // Resources have the relative path from the project root
            // and are separated by /. It's always / regardless of the
            // platform.

            return project.Files.ResourceFiles.Select(resourceFile => new ResourceDescription(
                GetRelativePath(root, resourceFile)
                           .Replace(Path.DirectorySeparatorChar, '/'),
                () => new FileStream(resourceFile, FileMode.Open, FileAccess.Read, FileShare.Read),
                true)).ToList();
        }

        private string GetRelativePath(string root, string resourceFile)
        {
            // This basically inlines PathHelper.GetRelativePath from Microsoft.Framework.Runtime
            //  (since we don't want to take a dependency on that)

            Uri source = new Uri(root);
            Uri target = new Uri(resourceFile);

            string path = source.MakeRelativeUri(target).OriginalString;
            if (path.StartsWith("/", StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }

            // Bug 483: We need the unescaped uri string to ensure that all characters are valid for a path.
            // Change the direction of the slashes to match the given separator.
            return Uri.UnescapeDataString(path);
        }
    }
}
