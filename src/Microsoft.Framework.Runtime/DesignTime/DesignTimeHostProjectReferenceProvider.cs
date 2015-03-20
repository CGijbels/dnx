using System;
using Microsoft.Framework.Runtime.Compilation;

namespace Microsoft.Framework.Runtime
{
    public class DesignTimeHostProjectCompiler : IProjectCompiler
    {
        private readonly IDesignTimeHostCompiler _compiler;

        public DesignTimeHostProjectCompiler(IDesignTimeHostCompiler compiler)
        {
            _compiler = compiler;
        }

        public IMetadataProjectReference CompileProject(
            ICompilationProject project,
            ILibraryKey target,
            Func<ILibraryExport> referenceResolver)
        {
            // The target framework and configuration are assumed to be correct
            // in the design time process
            var task = _compiler.Compile(project.ProjectDirectory, target);

            return new DesignTimeProjectReference(project, task.Result);
        }
    }
}