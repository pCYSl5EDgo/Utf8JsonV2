// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Mono.Cecil;

namespace Utf8Json.Generator
{
    public sealed class GeneratorExecutor : IDisposable
    {
        private readonly Action<string> logger;

        private readonly ReaderParameters readerParam;
        private readonly ReaderParameters doNotWriteReaderParam;

        private ModuleDefinition[] moduleDefinitions;
        private ModuleDefinition[] definitionModuleDefinitions;
        
        private bool disposed;

        public GeneratorExecutor(Action<string> logger)
        {
            this.logger = logger;
            var assemblyResolver = new DefaultAssemblyResolver();
            this.readerParam = new ReaderParameters
            {
                AssemblyResolver = assemblyResolver,
                ReadWrite = true,
            };
            this.doNotWriteReaderParam = new ReaderParameters()
            {
                AssemblyResolver = assemblyResolver,
                ReadWrite = false,
            };
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (!(moduleDefinitions is null))
            {
                foreach (var module in moduleDefinitions)
                {
                    module.Dispose();
                }

                moduleDefinitions = default;
            }

            // ReSharper disable once InvertIf
            if (!(definitionModuleDefinitions is null))
            {
                foreach (var module in definitionModuleDefinitions)
                {
                    module.Dispose();
                }

                definitionModuleDefinitions = default;
            }
        }

        public void Generate(
            string inputPath,
            string resolverName,
            string memoryDllPath,
            string[] libraryPaths,
            bool useMapMode,
            double loadFactor)
        {
        }
    }
}
