﻿using System;
using System.Runtime.Versioning;

namespace Microsoft.Framework.Runtime.Roslyn
{
    internal class ProjectContext : IProjectContext
    {
        public ProjectContext(Project project, FrameworkName targetFramework, string configuration)
        {
            Name = project.Name;
            ProjectDirectory = project.ProjectDirectory;
            ProjectFilePath = project.ProjectFilePath;
            TargetFramework = targetFramework;
            Version = project.Version?.ToString();
            Configuration = configuration;
        }

        public string Name { get; }
        public string Version { get; }
        public string ProjectDirectory { get; }
        public string ProjectFilePath { get; }
        public FrameworkName TargetFramework { get; }
        public string Configuration { get; }
    }
}