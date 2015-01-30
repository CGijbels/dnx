// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Framework.Runtime
{
    public class GlobalSettings
    {
        public const string GlobalFileName = "global.json";

        public IList<string> ProjectSearchPaths { get; private set; }
        public IDictionary<Library, string> PackageHashes { get; private set; }
        public string PackagesPath { get; private set; }
        public string FilePath { get; private set; }

        public static bool TryGetGlobalSettings(string path, out GlobalSettings globalSettings)
        {
            globalSettings = null;

            string globalJsonPath = null;

            if (Path.GetFileName(path) == GlobalFileName)
            {
                globalJsonPath = path;
                path = Path.GetDirectoryName(path);
            }
            else if (!HasGlobalFile(path))
            {
                return false;
            }
            else
            {
                globalJsonPath = Path.Combine(path, GlobalFileName);
            }

            globalSettings = new GlobalSettings();

            JObject settings = null;

            try
            {
                using (var stream = File.OpenRead(globalJsonPath))
                {
                    settings = JObject.Load(new JsonTextReader(new StreamReader(stream)));
                }
            }
            catch (JsonReaderException ex)
            {
                throw FileFormatException.Create(ex, globalJsonPath);
            }

            // TODO: Remove sources
            var projectSearchPaths = settings["projects"] ?? settings["sources"];
            var dependencies = settings["dependencies"] as JObject;

            globalSettings.ProjectSearchPaths = projectSearchPaths == null ? new string[] { } : (projectSearchPaths as JArray).Select(t => t.Value<string>()).ToArray();
            globalSettings.PackagesPath = settings.Value<string>("packages");
            globalSettings.PackageHashes = new Dictionary<Library, string>();
            globalSettings.FilePath = globalJsonPath;

            return true;
        }

        public static bool HasGlobalFile(string path)
        {
            string projectPath = Path.Combine(path, GlobalFileName);

            return File.Exists(projectPath);
        }

    }
}
