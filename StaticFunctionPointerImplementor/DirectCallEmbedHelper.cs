using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StaticFunctionPointerImplementor
{
    public static class DirectCallEmbedHelper
    {
        public static void Visit(ModuleDefinition module)
        {
            var @void = module.TypeSystem.Void;
            foreach (var type in module.Types)
            {
                Visit(type, @void);
            }
        }

        private static void Visit(TypeDefinition type, TypeReference @void)
        {
            foreach (var method in type.Methods)
            {
                if (ValidGetSerializeOrDeSerialize(method))
                {
                    EmbedStaticFunctionPointerHelperCall(method, @void);
                }

                if (ValidLoadFunctionCctor(method))
                {
                    //EmbedLoadFunctionPointerDirectly(method);
                }
            }

            foreach (var nestedType in type.NestedTypes)
            {
                Visit(nestedType, @void);
            }
        }

        private static void EmbedLoadFunctionPointerDirectly(MethodDefinition method)
        {
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code != Code.Call)
                {
                    continue;
                }

                if (!(instruction.Operand is GenericInstanceMethod getStaticMethod))
                {
                    continue;
                }

                if (!getStaticMethod.ReturnType.IsPrimitive || getStaticMethod.ReturnType.FullName != "System.IntPtr")
                {
                    continue;
                }

                if (getStaticMethod.DeclaringType.FullName != "Utf8Json.Internal.StaticHelper")
                {
                    continue;
                }

                var type = getStaticMethod.GenericArguments[1];
                if (type.IsGenericParameter)
                {
                    continue;
                }

                switch (getStaticMethod.Name)
                {
                    case "GetSerializeStatic":
                        if (!type.IsGenericInstance)
                        {
                            EmbedGetSerializeStatic(instruction, type);
                        }
                        break;
                    case "GetDeserializeStatic":
                        if (!type.IsGenericInstance)
                        {
                            EmbedGetDeserializeStatic(instruction, type);
                        }
                        break;
                }
            }
        }

        private static void EmbedGetDeserializeStatic(Instruction instruction, TypeReference type)
        {
            var definition = type.Resolve();
            var method = definition.Methods.FirstOrDefault(x => x.IsPublic && x.IsStatic && x.Name == "SerializeStatic" && x.Parameters.Count == 3);
            if (method is null)
            {
                return;
            }

            instruction.OpCode = OpCodes.Ldftn;
            instruction.Operand = method;
        }

        private static void EmbedGetSerializeStatic(Instruction instruction, TypeReference type)
        {
            var definition = type.Resolve();
            var method = definition.Methods.FirstOrDefault(x => x.IsPublic && x.IsStatic && x.Name == "DeserializeStatic" && x.Parameters.Count == 2);
            if (method is null)
            {
                return;
            }

            instruction.OpCode = OpCodes.Ldftn;
            instruction.Operand = method;
        }

        private static void EmbedStaticFunctionPointerHelperCall(MethodDefinition method, TypeReference @void)
        {
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code != Code.Call)
                {
                    continue;
                }

                if (!(instruction.Operand is GenericInstanceMethod calledMethod))
                {
                    continue;
                }

                if (calledMethod.HasThis)
                {
                    continue;
                }

                if (calledMethod.DeclaringType.FullName != "StaticFunctionPointerHelper.CallHelper")
                {
                    continue;
                }

                switch (calledMethod.Name)
                {
                    case "Serialize":
                        ReplaceWithSerialize(instruction, calledMethod, @void);
                        break;
                    case "Deserialize":
                        ReplaceWithDeserialize(instruction, calledMethod);
                        break;
                    default:
                        continue;
                }
            }
        }

        private static void ReplaceWithDeserialize(Instruction instruction, GenericInstanceMethod calledMethod)
        {
            var site = new CallSite(calledMethod.GenericArguments[0])
            {
                HasThis = false,
                Parameters =
                {
                    new ParameterDefinition(calledMethod.Parameters[0].ParameterType),
                    new ParameterDefinition(calledMethod.Parameters[1].ParameterType),
                },
            };

            instruction.OpCode = OpCodes.Calli;
            instruction.Operand = site;
        }

        private static void ReplaceWithSerialize(Instruction instruction, GenericInstanceMethod calledMethod, TypeReference @void)
        {
            var site = new CallSite(@void)
            {
                HasThis = false,
                Parameters =
                {
                    new ParameterDefinition(calledMethod.Parameters[0].ParameterType),
                    new ParameterDefinition(calledMethod.GenericArguments[0]),
                    new ParameterDefinition(calledMethod.Parameters[2].ParameterType),
                },
            };

            instruction.OpCode = OpCodes.Calli;
            instruction.Operand = site;
        }

        private static bool ValidLoadFunctionCctor(MethodDefinition method)
        {
            return method.HasBody && method.IsStatic && method.IsRuntimeSpecialName && method.IsSpecialName && method.Name == ".cctor";
        }

        private static bool ValidGetSerializeOrDeSerialize(MethodDefinition method)
        {
            if (!method.HasBody)
            {
                return false;
            }

            foreach (var variable in method.Body.Variables)
            {
                var variableType = variable.VariableType;
                if (variableType.IsPrimitive && variableType.FullName == "System.IntPtr")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
