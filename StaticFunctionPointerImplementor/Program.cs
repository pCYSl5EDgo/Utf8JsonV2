using System.IO;
using System.Threading.Tasks;
using Mono.Cecil;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Mono.Cecil.Cil;

namespace StaticFunctionPointerImplementor
{
    internal sealed class Program : ConsoleAppBase
    {
        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        public void Run(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(directory);
            }

            foreach (var dllFile in directoryInfo.EnumerateFiles("StaticFunctionPointerHelper.dll", SearchOption.AllDirectories))
            {
                ProcessModule(dllFile.FullName);
            }
        }

        private static void ProcessModule(string filePath)
        {
            var module = ModuleDefinition.ReadModule(filePath, new ReaderParameters()
            {
                ReadWrite = true,
            });

            try
            {
                var callHelper = module.GetType("StaticFunctionPointerHelper", "CallHelper");
                var serialize = callHelper.Methods[0];
                ProcessSerialize(serialize);
                var deserialize = callHelper.Methods[1];
                ProcessDeserialize(deserialize);
                module.Write();
            }
            finally
            {
                module.Dispose();
            }
        }

        private static void ProcessDeserialize(MethodDefinition method)
        {
            var body = method.Body;
            body.Variables.Clear();
            body.Instructions.Clear();

            var processor = body.GetILProcessor();
            var callSite = new CallSite(method.GenericParameters[0])
            {
                HasThis = false,
                Parameters =
                {
                    new ParameterDefinition("reader", method.Parameters[0].Attributes, method.Parameters[0].ParameterType),
                    new ParameterDefinition("options", method.Parameters[1].Attributes, method.Parameters[1].ParameterType),
                },
            };

            processor.Append(Instruction.Create(OpCodes.Ldarg_0));
            processor.Append(Instruction.Create(OpCodes.Ldarg_1));
            processor.Append(Instruction.Create(OpCodes.Ldarg_2));
            processor.Append(Instruction.Create(OpCodes.Calli, callSite));
            processor.Append(Instruction.Create(OpCodes.Ret));
        }

        private static void ProcessSerialize(MethodDefinition method)
        {
            var body = method.Body;
            body.Variables.Clear();
            body.Instructions.Clear();
            var processor = body.GetILProcessor();
            var callSite = new CallSite(method.Module.TypeSystem.Void)
            {
                HasThis = false,
                Parameters =
                {
                    new ParameterDefinition("writer", method.Parameters[0].Attributes, method.Parameters[0].ParameterType),
                    new ParameterDefinition("value", method.Parameters[1].Attributes, method.Parameters[1].ParameterType),
                    new ParameterDefinition("options", method.Parameters[2].Attributes, method.Parameters[2].ParameterType),
                },
            };

            processor.Append(Instruction.Create(OpCodes.Ldarg_0));
            processor.Append(Instruction.Create(OpCodes.Ldarg_1));
            processor.Append(Instruction.Create(OpCodes.Ldarg_2));
            processor.Append(Instruction.Create(OpCodes.Ldarg_3));
            processor.Append(Instruction.Create(OpCodes.Calli, callSite));
            processor.Append(Instruction.Create(OpCodes.Ret));
        }
    }
}
