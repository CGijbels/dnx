// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Runtime.Compilation;

namespace Microsoft.Framework.Runtime.Roslyn
{
    public class CompositeResourceProvider : IResourceProvider
    {
        private readonly IEnumerable<IResourceProvider> _providers;

        public CompositeResourceProvider(IEnumerable<IResourceProvider> providers)
        {
            _providers = providers;
        }

        public IList<ResourceDescription> GetResources(ICompilationProject project)
        {
            return _providers.SelectMany(provider => provider.GetResources(project)).ToList();
        }
    }
}
