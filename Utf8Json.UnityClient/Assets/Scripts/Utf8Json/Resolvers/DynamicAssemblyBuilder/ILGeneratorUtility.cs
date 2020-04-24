// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Emit;

namespace Utf8Json.Resolvers
{
    internal static class IntermediateLanguageGeneratorUtility
    {
        public static void LdcI4(this ILGenerator processor, int number)
        {
            switch (number)
            {
                case -1: processor.Emit(OpCodes.Ldc_I4_M1); return;
                case 1: processor.Emit(OpCodes.Ldc_I4_1); return;
                case 2: processor.Emit(OpCodes.Ldc_I4_2); return;
                case 3: processor.Emit(OpCodes.Ldc_I4_3); return;
                case 4: processor.Emit(OpCodes.Ldc_I4_4); return;
                case 5: processor.Emit(OpCodes.Ldc_I4_5); return;
                case 6: processor.Emit(OpCodes.Ldc_I4_6); return;
                case 7: processor.Emit(OpCodes.Ldc_I4_7); return;
                case 8: processor.Emit(OpCodes.Ldc_I4_8); return;
            }

            if (number <= 127 && number >= -128)
            {
                processor.Emit(OpCodes.Ldc_I4_S, (sbyte)number);
            }
            else
            {
                processor.Emit(OpCodes.Ldc_I4, number);
            }
        }
    }
}
