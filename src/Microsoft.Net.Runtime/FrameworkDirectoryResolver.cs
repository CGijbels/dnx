﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Net.Runtime
{
    public static class FrameworkDirectoryResolver
    {
        public static IEnumerable<string> GetFrameworkDirectories()
        {
            string klrPath = Environment.GetEnvironmentVariable("KLR_PATH");

            if (!String.IsNullOrEmpty(klrPath))
            {
                klrPath = Path.GetDirectoryName(klrPath);

                return new[] {
                    Path.GetFullPath(Path.Combine(klrPath, @"..\..\..\Framework")),
#if DEBUG
                    Path.GetFullPath(Path.Combine(klrPath, @"..\..\..\artifacts\build\ProjectK\Framework"))
#endif
                };
            }

            return new string[0];
        }

    }
}