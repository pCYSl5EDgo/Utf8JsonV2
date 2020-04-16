﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using StaticFunctionPointerHelper;
using System;
using Utf8Json.Internal;

namespace Utf8Json.Formatters
{
    public sealed unsafe class Dimension2ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,]?>
#else
        : IJsonFormatter<T[,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);

            if (startIndexOf0 != 0 || startIndexOf1 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
	                if (first)
	                {
	                    first = false;
	                }
	                else
	                {
	                    var span = writer.Writer.GetSpan(1);
	                    span[0] = (byte)',';
	                    writer.Writer.Advance(1);
	                }

	                var element = value[
	                    iteratorOf0 + startIndexOf0
	                    , iteratorOf1 + startIndexOf1
	                ];
	                writer.Serialize(element, options, serializer);
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 2)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 2)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
                	answer[iterator0, iterator1] = elements[index++];
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
                	answer[iterator0, iterator1] = elements[index++];
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension3ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,]?>
#else
        : IJsonFormatter<T[,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
		                if (first)
		                {
		                    first = false;
		                }
		                else
		                {
		                    var span = writer.Writer.GetSpan(1);
		                    span[0] = (byte)',';
		                    writer.Writer.Advance(1);
		                }

		                var element = value[
		                    iteratorOf0 + startIndexOf0
		                    , iteratorOf1 + startIndexOf1
		                    , iteratorOf2 + startIndexOf2
		                ];
		                writer.Serialize(element, options, serializer);
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 3)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 3)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
                		answer[iterator0, iterator1, iterator2] = elements[index++];
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
                		answer[iterator0, iterator1, iterator2] = elements[index++];
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension4ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,]?>
#else
        : IJsonFormatter<T[,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
			                if (first)
			                {
			                    first = false;
			                }
			                else
			                {
			                    var span = writer.Writer.GetSpan(1);
			                    span[0] = (byte)',';
			                    writer.Writer.Advance(1);
			                }

			                var element = value[
			                    iteratorOf0 + startIndexOf0
			                    , iteratorOf1 + startIndexOf1
			                    , iteratorOf2 + startIndexOf2
			                    , iteratorOf3 + startIndexOf3
			                ];
			                writer.Serialize(element, options, serializer);
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 4)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 4)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
                			answer[iterator0, iterator1, iterator2, iterator3] = elements[index++];
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
                			answer[iterator0, iterator1, iterator2, iterator3] = elements[index++];
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension5ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,]?>
#else
        : IJsonFormatter<T[,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
				                if (first)
				                {
				                    first = false;
				                }
				                else
				                {
				                    var span = writer.Writer.GetSpan(1);
				                    span[0] = (byte)',';
				                    writer.Writer.Advance(1);
				                }

				                var element = value[
				                    iteratorOf0 + startIndexOf0
				                    , iteratorOf1 + startIndexOf1
				                    , iteratorOf2 + startIndexOf2
				                    , iteratorOf3 + startIndexOf3
				                    , iteratorOf4 + startIndexOf4
				                ];
				                writer.Serialize(element, options, serializer);
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 5)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 5)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
                				answer[iterator0, iterator1, iterator2, iterator3, iterator4] = elements[index++];
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
                				answer[iterator0, iterator1, iterator2, iterator3, iterator4] = elements[index++];
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension6ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
					                if (first)
					                {
					                    first = false;
					                }
					                else
					                {
					                    var span = writer.Writer.GetSpan(1);
					                    span[0] = (byte)',';
					                    writer.Writer.Advance(1);
					                }

					                var element = value[
					                    iteratorOf0 + startIndexOf0
					                    , iteratorOf1 + startIndexOf1
					                    , iteratorOf2 + startIndexOf2
					                    , iteratorOf3 + startIndexOf3
					                    , iteratorOf4 + startIndexOf4
					                    , iteratorOf5 + startIndexOf5
					                ];
					                writer.Serialize(element, options, serializer);
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 6)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 6)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
                					answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5] = elements[index++];
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
                					answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5] = elements[index++];
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension7ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
						                if (first)
						                {
						                    first = false;
						                }
						                else
						                {
						                    var span = writer.Writer.GetSpan(1);
						                    span[0] = (byte)',';
						                    writer.Writer.Advance(1);
						                }

						                var element = value[
						                    iteratorOf0 + startIndexOf0
						                    , iteratorOf1 + startIndexOf1
						                    , iteratorOf2 + startIndexOf2
						                    , iteratorOf3 + startIndexOf3
						                    , iteratorOf4 + startIndexOf4
						                    , iteratorOf5 + startIndexOf5
						                    , iteratorOf6 + startIndexOf6
						                ];
						                writer.Serialize(element, options, serializer);
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 7)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 7)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
                						answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6] = elements[index++];
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
                						answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6] = elements[index++];
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension8ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
							                if (first)
							                {
							                    first = false;
							                }
							                else
							                {
							                    var span = writer.Writer.GetSpan(1);
							                    span[0] = (byte)',';
							                    writer.Writer.Advance(1);
							                }

							                var element = value[
							                    iteratorOf0 + startIndexOf0
							                    , iteratorOf1 + startIndexOf1
							                    , iteratorOf2 + startIndexOf2
							                    , iteratorOf3 + startIndexOf3
							                    , iteratorOf4 + startIndexOf4
							                    , iteratorOf5 + startIndexOf5
							                    , iteratorOf6 + startIndexOf6
							                    , iteratorOf7 + startIndexOf7
							                ];
							                writer.Serialize(element, options, serializer);
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 8)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 8)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
                							answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7] = elements[index++];
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
                							answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7] = elements[index++];
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension9ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
								                if (first)
								                {
								                    first = false;
								                }
								                else
								                {
								                    var span = writer.Writer.GetSpan(1);
								                    span[0] = (byte)',';
								                    writer.Writer.Advance(1);
								                }

								                var element = value[
								                    iteratorOf0 + startIndexOf0
								                    , iteratorOf1 + startIndexOf1
								                    , iteratorOf2 + startIndexOf2
								                    , iteratorOf3 + startIndexOf3
								                    , iteratorOf4 + startIndexOf4
								                    , iteratorOf5 + startIndexOf5
								                    , iteratorOf6 + startIndexOf6
								                    , iteratorOf7 + startIndexOf7
								                    , iteratorOf8 + startIndexOf8
								                ];
								                writer.Serialize(element, options, serializer);
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 9)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 9)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
                								answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8] = elements[index++];
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
                								answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8] = elements[index++];
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension10ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
									                if (first)
									                {
									                    first = false;
									                }
									                else
									                {
									                    var span = writer.Writer.GetSpan(1);
									                    span[0] = (byte)',';
									                    writer.Writer.Advance(1);
									                }

									                var element = value[
									                    iteratorOf0 + startIndexOf0
									                    , iteratorOf1 + startIndexOf1
									                    , iteratorOf2 + startIndexOf2
									                    , iteratorOf3 + startIndexOf3
									                    , iteratorOf4 + startIndexOf4
									                    , iteratorOf5 + startIndexOf5
									                    , iteratorOf6 + startIndexOf6
									                    , iteratorOf7 + startIndexOf7
									                    , iteratorOf8 + startIndexOf8
									                    , iteratorOf9 + startIndexOf9
									                ];
									                writer.Serialize(element, options, serializer);
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 10)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 10)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
                									answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9] = elements[index++];
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
                									answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9] = elements[index++];
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension11ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
										                if (first)
										                {
										                    first = false;
										                }
										                else
										                {
										                    var span = writer.Writer.GetSpan(1);
										                    span[0] = (byte)',';
										                    writer.Writer.Advance(1);
										                }

										                var element = value[
										                    iteratorOf0 + startIndexOf0
										                    , iteratorOf1 + startIndexOf1
										                    , iteratorOf2 + startIndexOf2
										                    , iteratorOf3 + startIndexOf3
										                    , iteratorOf4 + startIndexOf4
										                    , iteratorOf5 + startIndexOf5
										                    , iteratorOf6 + startIndexOf6
										                    , iteratorOf7 + startIndexOf7
										                    , iteratorOf8 + startIndexOf8
										                    , iteratorOf9 + startIndexOf9
										                    , iteratorOf10 + startIndexOf10
										                ];
										                writer.Serialize(element, options, serializer);
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 11)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 11)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
                										answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10] = elements[index++];
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
                										answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10] = elements[index++];
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension12ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
											                if (first)
											                {
											                    first = false;
											                }
											                else
											                {
											                    var span = writer.Writer.GetSpan(1);
											                    span[0] = (byte)',';
											                    writer.Writer.Advance(1);
											                }

											                var element = value[
											                    iteratorOf0 + startIndexOf0
											                    , iteratorOf1 + startIndexOf1
											                    , iteratorOf2 + startIndexOf2
											                    , iteratorOf3 + startIndexOf3
											                    , iteratorOf4 + startIndexOf4
											                    , iteratorOf5 + startIndexOf5
											                    , iteratorOf6 + startIndexOf6
											                    , iteratorOf7 + startIndexOf7
											                    , iteratorOf8 + startIndexOf8
											                    , iteratorOf9 + startIndexOf9
											                    , iteratorOf10 + startIndexOf10
											                    , iteratorOf11 + startIndexOf11
											                ];
											                writer.Serialize(element, options, serializer);
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 12)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 12)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
                											answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11] = elements[index++];
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
                											answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11] = elements[index++];
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension13ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
												                if (first)
												                {
												                    first = false;
												                }
												                else
												                {
												                    var span = writer.Writer.GetSpan(1);
												                    span[0] = (byte)',';
												                    writer.Writer.Advance(1);
												                }

												                var element = value[
												                    iteratorOf0 + startIndexOf0
												                    , iteratorOf1 + startIndexOf1
												                    , iteratorOf2 + startIndexOf2
												                    , iteratorOf3 + startIndexOf3
												                    , iteratorOf4 + startIndexOf4
												                    , iteratorOf5 + startIndexOf5
												                    , iteratorOf6 + startIndexOf6
												                    , iteratorOf7 + startIndexOf7
												                    , iteratorOf8 + startIndexOf8
												                    , iteratorOf9 + startIndexOf9
												                    , iteratorOf10 + startIndexOf10
												                    , iteratorOf11 + startIndexOf11
												                    , iteratorOf12 + startIndexOf12
												                ];
												                writer.Serialize(element, options, serializer);
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 13)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 13)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
                												answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12] = elements[index++];
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
                												answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12] = elements[index++];
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension14ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
													                if (first)
													                {
													                    first = false;
													                }
													                else
													                {
													                    var span = writer.Writer.GetSpan(1);
													                    span[0] = (byte)',';
													                    writer.Writer.Advance(1);
													                }

													                var element = value[
													                    iteratorOf0 + startIndexOf0
													                    , iteratorOf1 + startIndexOf1
													                    , iteratorOf2 + startIndexOf2
													                    , iteratorOf3 + startIndexOf3
													                    , iteratorOf4 + startIndexOf4
													                    , iteratorOf5 + startIndexOf5
													                    , iteratorOf6 + startIndexOf6
													                    , iteratorOf7 + startIndexOf7
													                    , iteratorOf8 + startIndexOf8
													                    , iteratorOf9 + startIndexOf9
													                    , iteratorOf10 + startIndexOf10
													                    , iteratorOf11 + startIndexOf11
													                    , iteratorOf12 + startIndexOf12
													                    , iteratorOf13 + startIndexOf13
													                ];
													                writer.Serialize(element, options, serializer);
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 14)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 14)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
                													answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13] = elements[index++];
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
                													answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13] = elements[index++];
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension15ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
														                if (first)
														                {
														                    first = false;
														                }
														                else
														                {
														                    var span = writer.Writer.GetSpan(1);
														                    span[0] = (byte)',';
														                    writer.Writer.Advance(1);
														                }

														                var element = value[
														                    iteratorOf0 + startIndexOf0
														                    , iteratorOf1 + startIndexOf1
														                    , iteratorOf2 + startIndexOf2
														                    , iteratorOf3 + startIndexOf3
														                    , iteratorOf4 + startIndexOf4
														                    , iteratorOf5 + startIndexOf5
														                    , iteratorOf6 + startIndexOf6
														                    , iteratorOf7 + startIndexOf7
														                    , iteratorOf8 + startIndexOf8
														                    , iteratorOf9 + startIndexOf9
														                    , iteratorOf10 + startIndexOf10
														                    , iteratorOf11 + startIndexOf11
														                    , iteratorOf12 + startIndexOf12
														                    , iteratorOf13 + startIndexOf13
														                    , iteratorOf14 + startIndexOf14
														                ];
														                writer.Serialize(element, options, serializer);
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 15)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 15)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
                														answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14] = elements[index++];
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
                														answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14] = elements[index++];
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension16ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
															                if (first)
															                {
															                    first = false;
															                }
															                else
															                {
															                    var span = writer.Writer.GetSpan(1);
															                    span[0] = (byte)',';
															                    writer.Writer.Advance(1);
															                }

															                var element = value[
															                    iteratorOf0 + startIndexOf0
															                    , iteratorOf1 + startIndexOf1
															                    , iteratorOf2 + startIndexOf2
															                    , iteratorOf3 + startIndexOf3
															                    , iteratorOf4 + startIndexOf4
															                    , iteratorOf5 + startIndexOf5
															                    , iteratorOf6 + startIndexOf6
															                    , iteratorOf7 + startIndexOf7
															                    , iteratorOf8 + startIndexOf8
															                    , iteratorOf9 + startIndexOf9
															                    , iteratorOf10 + startIndexOf10
															                    , iteratorOf11 + startIndexOf11
															                    , iteratorOf12 + startIndexOf12
															                    , iteratorOf13 + startIndexOf13
															                    , iteratorOf14 + startIndexOf14
															                    , iteratorOf15 + startIndexOf15
															                ];
															                writer.Serialize(element, options, serializer);
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 16)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 16)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
                															answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15] = elements[index++];
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
                															answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15] = elements[index++];
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension17ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																                if (first)
																                {
																                    first = false;
																                }
																                else
																                {
																                    var span = writer.Writer.GetSpan(1);
																                    span[0] = (byte)',';
																                    writer.Writer.Advance(1);
																                }

																                var element = value[
																                    iteratorOf0 + startIndexOf0
																                    , iteratorOf1 + startIndexOf1
																                    , iteratorOf2 + startIndexOf2
																                    , iteratorOf3 + startIndexOf3
																                    , iteratorOf4 + startIndexOf4
																                    , iteratorOf5 + startIndexOf5
																                    , iteratorOf6 + startIndexOf6
																                    , iteratorOf7 + startIndexOf7
																                    , iteratorOf8 + startIndexOf8
																                    , iteratorOf9 + startIndexOf9
																                    , iteratorOf10 + startIndexOf10
																                    , iteratorOf11 + startIndexOf11
																                    , iteratorOf12 + startIndexOf12
																                    , iteratorOf13 + startIndexOf13
																                    , iteratorOf14 + startIndexOf14
																                    , iteratorOf15 + startIndexOf15
																                    , iteratorOf16 + startIndexOf16
																                ];
																                writer.Serialize(element, options, serializer);
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 17)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 17)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
                																answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16] = elements[index++];
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
                																answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16] = elements[index++];
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension18ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																	                if (first)
																	                {
																	                    first = false;
																	                }
																	                else
																	                {
																	                    var span = writer.Writer.GetSpan(1);
																	                    span[0] = (byte)',';
																	                    writer.Writer.Advance(1);
																	                }

																	                var element = value[
																	                    iteratorOf0 + startIndexOf0
																	                    , iteratorOf1 + startIndexOf1
																	                    , iteratorOf2 + startIndexOf2
																	                    , iteratorOf3 + startIndexOf3
																	                    , iteratorOf4 + startIndexOf4
																	                    , iteratorOf5 + startIndexOf5
																	                    , iteratorOf6 + startIndexOf6
																	                    , iteratorOf7 + startIndexOf7
																	                    , iteratorOf8 + startIndexOf8
																	                    , iteratorOf9 + startIndexOf9
																	                    , iteratorOf10 + startIndexOf10
																	                    , iteratorOf11 + startIndexOf11
																	                    , iteratorOf12 + startIndexOf12
																	                    , iteratorOf13 + startIndexOf13
																	                    , iteratorOf14 + startIndexOf14
																	                    , iteratorOf15 + startIndexOf15
																	                    , iteratorOf16 + startIndexOf16
																	                    , iteratorOf17 + startIndexOf17
																	                ];
																	                writer.Serialize(element, options, serializer);
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 18)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 18)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
                																	answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17] = elements[index++];
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
                																	answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17] = elements[index++];
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension19ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																		                if (first)
																		                {
																		                    first = false;
																		                }
																		                else
																		                {
																		                    var span = writer.Writer.GetSpan(1);
																		                    span[0] = (byte)',';
																		                    writer.Writer.Advance(1);
																		                }

																		                var element = value[
																		                    iteratorOf0 + startIndexOf0
																		                    , iteratorOf1 + startIndexOf1
																		                    , iteratorOf2 + startIndexOf2
																		                    , iteratorOf3 + startIndexOf3
																		                    , iteratorOf4 + startIndexOf4
																		                    , iteratorOf5 + startIndexOf5
																		                    , iteratorOf6 + startIndexOf6
																		                    , iteratorOf7 + startIndexOf7
																		                    , iteratorOf8 + startIndexOf8
																		                    , iteratorOf9 + startIndexOf9
																		                    , iteratorOf10 + startIndexOf10
																		                    , iteratorOf11 + startIndexOf11
																		                    , iteratorOf12 + startIndexOf12
																		                    , iteratorOf13 + startIndexOf13
																		                    , iteratorOf14 + startIndexOf14
																		                    , iteratorOf15 + startIndexOf15
																		                    , iteratorOf16 + startIndexOf16
																		                    , iteratorOf17 + startIndexOf17
																		                    , iteratorOf18 + startIndexOf18
																		                ];
																		                writer.Serialize(element, options, serializer);
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 19)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 19)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
                																		answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18] = elements[index++];
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
                																		answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18] = elements[index++];
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension20ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																			                if (first)
																			                {
																			                    first = false;
																			                }
																			                else
																			                {
																			                    var span = writer.Writer.GetSpan(1);
																			                    span[0] = (byte)',';
																			                    writer.Writer.Advance(1);
																			                }

																			                var element = value[
																			                    iteratorOf0 + startIndexOf0
																			                    , iteratorOf1 + startIndexOf1
																			                    , iteratorOf2 + startIndexOf2
																			                    , iteratorOf3 + startIndexOf3
																			                    , iteratorOf4 + startIndexOf4
																			                    , iteratorOf5 + startIndexOf5
																			                    , iteratorOf6 + startIndexOf6
																			                    , iteratorOf7 + startIndexOf7
																			                    , iteratorOf8 + startIndexOf8
																			                    , iteratorOf9 + startIndexOf9
																			                    , iteratorOf10 + startIndexOf10
																			                    , iteratorOf11 + startIndexOf11
																			                    , iteratorOf12 + startIndexOf12
																			                    , iteratorOf13 + startIndexOf13
																			                    , iteratorOf14 + startIndexOf14
																			                    , iteratorOf15 + startIndexOf15
																			                    , iteratorOf16 + startIndexOf16
																			                    , iteratorOf17 + startIndexOf17
																			                    , iteratorOf18 + startIndexOf18
																			                    , iteratorOf19 + startIndexOf19
																			                ];
																			                writer.Serialize(element, options, serializer);
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 20)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 20)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
                																			answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19] = elements[index++];
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
                																			answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19] = elements[index++];
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension21ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																				                if (first)
																				                {
																				                    first = false;
																				                }
																				                else
																				                {
																				                    var span = writer.Writer.GetSpan(1);
																				                    span[0] = (byte)',';
																				                    writer.Writer.Advance(1);
																				                }

																				                var element = value[
																				                    iteratorOf0 + startIndexOf0
																				                    , iteratorOf1 + startIndexOf1
																				                    , iteratorOf2 + startIndexOf2
																				                    , iteratorOf3 + startIndexOf3
																				                    , iteratorOf4 + startIndexOf4
																				                    , iteratorOf5 + startIndexOf5
																				                    , iteratorOf6 + startIndexOf6
																				                    , iteratorOf7 + startIndexOf7
																				                    , iteratorOf8 + startIndexOf8
																				                    , iteratorOf9 + startIndexOf9
																				                    , iteratorOf10 + startIndexOf10
																				                    , iteratorOf11 + startIndexOf11
																				                    , iteratorOf12 + startIndexOf12
																				                    , iteratorOf13 + startIndexOf13
																				                    , iteratorOf14 + startIndexOf14
																				                    , iteratorOf15 + startIndexOf15
																				                    , iteratorOf16 + startIndexOf16
																				                    , iteratorOf17 + startIndexOf17
																				                    , iteratorOf18 + startIndexOf18
																				                    , iteratorOf19 + startIndexOf19
																				                    , iteratorOf20 + startIndexOf20
																				                ];
																				                writer.Serialize(element, options, serializer);
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 21)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 21)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
                																				answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20] = elements[index++];
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
                																				answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20] = elements[index++];
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension22ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																					                if (first)
																					                {
																					                    first = false;
																					                }
																					                else
																					                {
																					                    var span = writer.Writer.GetSpan(1);
																					                    span[0] = (byte)',';
																					                    writer.Writer.Advance(1);
																					                }

																					                var element = value[
																					                    iteratorOf0 + startIndexOf0
																					                    , iteratorOf1 + startIndexOf1
																					                    , iteratorOf2 + startIndexOf2
																					                    , iteratorOf3 + startIndexOf3
																					                    , iteratorOf4 + startIndexOf4
																					                    , iteratorOf5 + startIndexOf5
																					                    , iteratorOf6 + startIndexOf6
																					                    , iteratorOf7 + startIndexOf7
																					                    , iteratorOf8 + startIndexOf8
																					                    , iteratorOf9 + startIndexOf9
																					                    , iteratorOf10 + startIndexOf10
																					                    , iteratorOf11 + startIndexOf11
																					                    , iteratorOf12 + startIndexOf12
																					                    , iteratorOf13 + startIndexOf13
																					                    , iteratorOf14 + startIndexOf14
																					                    , iteratorOf15 + startIndexOf15
																					                    , iteratorOf16 + startIndexOf16
																					                    , iteratorOf17 + startIndexOf17
																					                    , iteratorOf18 + startIndexOf18
																					                    , iteratorOf19 + startIndexOf19
																					                    , iteratorOf20 + startIndexOf20
																					                    , iteratorOf21 + startIndexOf21
																					                ];
																					                writer.Serialize(element, options, serializer);
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 22)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 22)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
                																					answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21] = elements[index++];
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
                																					answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21] = elements[index++];
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension23ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																						                if (first)
																						                {
																						                    first = false;
																						                }
																						                else
																						                {
																						                    var span = writer.Writer.GetSpan(1);
																						                    span[0] = (byte)',';
																						                    writer.Writer.Advance(1);
																						                }

																						                var element = value[
																						                    iteratorOf0 + startIndexOf0
																						                    , iteratorOf1 + startIndexOf1
																						                    , iteratorOf2 + startIndexOf2
																						                    , iteratorOf3 + startIndexOf3
																						                    , iteratorOf4 + startIndexOf4
																						                    , iteratorOf5 + startIndexOf5
																						                    , iteratorOf6 + startIndexOf6
																						                    , iteratorOf7 + startIndexOf7
																						                    , iteratorOf8 + startIndexOf8
																						                    , iteratorOf9 + startIndexOf9
																						                    , iteratorOf10 + startIndexOf10
																						                    , iteratorOf11 + startIndexOf11
																						                    , iteratorOf12 + startIndexOf12
																						                    , iteratorOf13 + startIndexOf13
																						                    , iteratorOf14 + startIndexOf14
																						                    , iteratorOf15 + startIndexOf15
																						                    , iteratorOf16 + startIndexOf16
																						                    , iteratorOf17 + startIndexOf17
																						                    , iteratorOf18 + startIndexOf18
																						                    , iteratorOf19 + startIndexOf19
																						                    , iteratorOf20 + startIndexOf20
																						                    , iteratorOf21 + startIndexOf21
																						                    , iteratorOf22 + startIndexOf22
																						                ];
																						                writer.Serialize(element, options, serializer);
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 23)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 23)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
                																						answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22] = elements[index++];
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
                																						answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22] = elements[index++];
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension24ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																							                if (first)
																							                {
																							                    first = false;
																							                }
																							                else
																							                {
																							                    var span = writer.Writer.GetSpan(1);
																							                    span[0] = (byte)',';
																							                    writer.Writer.Advance(1);
																							                }

																							                var element = value[
																							                    iteratorOf0 + startIndexOf0
																							                    , iteratorOf1 + startIndexOf1
																							                    , iteratorOf2 + startIndexOf2
																							                    , iteratorOf3 + startIndexOf3
																							                    , iteratorOf4 + startIndexOf4
																							                    , iteratorOf5 + startIndexOf5
																							                    , iteratorOf6 + startIndexOf6
																							                    , iteratorOf7 + startIndexOf7
																							                    , iteratorOf8 + startIndexOf8
																							                    , iteratorOf9 + startIndexOf9
																							                    , iteratorOf10 + startIndexOf10
																							                    , iteratorOf11 + startIndexOf11
																							                    , iteratorOf12 + startIndexOf12
																							                    , iteratorOf13 + startIndexOf13
																							                    , iteratorOf14 + startIndexOf14
																							                    , iteratorOf15 + startIndexOf15
																							                    , iteratorOf16 + startIndexOf16
																							                    , iteratorOf17 + startIndexOf17
																							                    , iteratorOf18 + startIndexOf18
																							                    , iteratorOf19 + startIndexOf19
																							                    , iteratorOf20 + startIndexOf20
																							                    , iteratorOf21 + startIndexOf21
																							                    , iteratorOf22 + startIndexOf22
																							                    , iteratorOf23 + startIndexOf23
																							                ];
																							                writer.Serialize(element, options, serializer);
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 24)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 24)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
                																							answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23] = elements[index++];
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
                																							answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23] = elements[index++];
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension25ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																								                if (first)
																								                {
																								                    first = false;
																								                }
																								                else
																								                {
																								                    var span = writer.Writer.GetSpan(1);
																								                    span[0] = (byte)',';
																								                    writer.Writer.Advance(1);
																								                }

																								                var element = value[
																								                    iteratorOf0 + startIndexOf0
																								                    , iteratorOf1 + startIndexOf1
																								                    , iteratorOf2 + startIndexOf2
																								                    , iteratorOf3 + startIndexOf3
																								                    , iteratorOf4 + startIndexOf4
																								                    , iteratorOf5 + startIndexOf5
																								                    , iteratorOf6 + startIndexOf6
																								                    , iteratorOf7 + startIndexOf7
																								                    , iteratorOf8 + startIndexOf8
																								                    , iteratorOf9 + startIndexOf9
																								                    , iteratorOf10 + startIndexOf10
																								                    , iteratorOf11 + startIndexOf11
																								                    , iteratorOf12 + startIndexOf12
																								                    , iteratorOf13 + startIndexOf13
																								                    , iteratorOf14 + startIndexOf14
																								                    , iteratorOf15 + startIndexOf15
																								                    , iteratorOf16 + startIndexOf16
																								                    , iteratorOf17 + startIndexOf17
																								                    , iteratorOf18 + startIndexOf18
																								                    , iteratorOf19 + startIndexOf19
																								                    , iteratorOf20 + startIndexOf20
																								                    , iteratorOf21 + startIndexOf21
																								                    , iteratorOf22 + startIndexOf22
																								                    , iteratorOf23 + startIndexOf23
																								                    , iteratorOf24 + startIndexOf24
																								                ];
																								                writer.Serialize(element, options, serializer);
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 25)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 25)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
                																								answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24] = elements[index++];
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
                																								answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24] = elements[index++];
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension26ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																									                if (first)
																									                {
																									                    first = false;
																									                }
																									                else
																									                {
																									                    var span = writer.Writer.GetSpan(1);
																									                    span[0] = (byte)',';
																									                    writer.Writer.Advance(1);
																									                }

																									                var element = value[
																									                    iteratorOf0 + startIndexOf0
																									                    , iteratorOf1 + startIndexOf1
																									                    , iteratorOf2 + startIndexOf2
																									                    , iteratorOf3 + startIndexOf3
																									                    , iteratorOf4 + startIndexOf4
																									                    , iteratorOf5 + startIndexOf5
																									                    , iteratorOf6 + startIndexOf6
																									                    , iteratorOf7 + startIndexOf7
																									                    , iteratorOf8 + startIndexOf8
																									                    , iteratorOf9 + startIndexOf9
																									                    , iteratorOf10 + startIndexOf10
																									                    , iteratorOf11 + startIndexOf11
																									                    , iteratorOf12 + startIndexOf12
																									                    , iteratorOf13 + startIndexOf13
																									                    , iteratorOf14 + startIndexOf14
																									                    , iteratorOf15 + startIndexOf15
																									                    , iteratorOf16 + startIndexOf16
																									                    , iteratorOf17 + startIndexOf17
																									                    , iteratorOf18 + startIndexOf18
																									                    , iteratorOf19 + startIndexOf19
																									                    , iteratorOf20 + startIndexOf20
																									                    , iteratorOf21 + startIndexOf21
																									                    , iteratorOf22 + startIndexOf22
																									                    , iteratorOf23 + startIndexOf23
																									                    , iteratorOf24 + startIndexOf24
																									                    , iteratorOf25 + startIndexOf25
																									                ];
																									                writer.Serialize(element, options, serializer);
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 26)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 26)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
                																									answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25] = elements[index++];
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
                																									answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25] = elements[index++];
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension27ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																										                if (first)
																										                {
																										                    first = false;
																										                }
																										                else
																										                {
																										                    var span = writer.Writer.GetSpan(1);
																										                    span[0] = (byte)',';
																										                    writer.Writer.Advance(1);
																										                }

																										                var element = value[
																										                    iteratorOf0 + startIndexOf0
																										                    , iteratorOf1 + startIndexOf1
																										                    , iteratorOf2 + startIndexOf2
																										                    , iteratorOf3 + startIndexOf3
																										                    , iteratorOf4 + startIndexOf4
																										                    , iteratorOf5 + startIndexOf5
																										                    , iteratorOf6 + startIndexOf6
																										                    , iteratorOf7 + startIndexOf7
																										                    , iteratorOf8 + startIndexOf8
																										                    , iteratorOf9 + startIndexOf9
																										                    , iteratorOf10 + startIndexOf10
																										                    , iteratorOf11 + startIndexOf11
																										                    , iteratorOf12 + startIndexOf12
																										                    , iteratorOf13 + startIndexOf13
																										                    , iteratorOf14 + startIndexOf14
																										                    , iteratorOf15 + startIndexOf15
																										                    , iteratorOf16 + startIndexOf16
																										                    , iteratorOf17 + startIndexOf17
																										                    , iteratorOf18 + startIndexOf18
																										                    , iteratorOf19 + startIndexOf19
																										                    , iteratorOf20 + startIndexOf20
																										                    , iteratorOf21 + startIndexOf21
																										                    , iteratorOf22 + startIndexOf22
																										                    , iteratorOf23 + startIndexOf23
																										                    , iteratorOf24 + startIndexOf24
																										                    , iteratorOf25 + startIndexOf25
																										                    , iteratorOf26 + startIndexOf26
																										                ];
																										                writer.Serialize(element, options, serializer);
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 27)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 27)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
                																										answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26] = elements[index++];
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
                																										answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26] = elements[index++];
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension28ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            var startIndexOf27 = value.GetLowerBound(27);
            var lengthOf27 = value.GetLength(27);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf27);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0 || startIndexOf27 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf27);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																											            for (var iteratorOf27 = 0; iteratorOf27 < lengthOf27; iteratorOf27++)
																											            {
																											                if (first)
																											                {
																											                    first = false;
																											                }
																											                else
																											                {
																											                    var span = writer.Writer.GetSpan(1);
																											                    span[0] = (byte)',';
																											                    writer.Writer.Advance(1);
																											                }

																											                var element = value[
																											                    iteratorOf0 + startIndexOf0
																											                    , iteratorOf1 + startIndexOf1
																											                    , iteratorOf2 + startIndexOf2
																											                    , iteratorOf3 + startIndexOf3
																											                    , iteratorOf4 + startIndexOf4
																											                    , iteratorOf5 + startIndexOf5
																											                    , iteratorOf6 + startIndexOf6
																											                    , iteratorOf7 + startIndexOf7
																											                    , iteratorOf8 + startIndexOf8
																											                    , iteratorOf9 + startIndexOf9
																											                    , iteratorOf10 + startIndexOf10
																											                    , iteratorOf11 + startIndexOf11
																											                    , iteratorOf12 + startIndexOf12
																											                    , iteratorOf13 + startIndexOf13
																											                    , iteratorOf14 + startIndexOf14
																											                    , iteratorOf15 + startIndexOf15
																											                    , iteratorOf16 + startIndexOf16
																											                    , iteratorOf17 + startIndexOf17
																											                    , iteratorOf18 + startIndexOf18
																											                    , iteratorOf19 + startIndexOf19
																											                    , iteratorOf20 + startIndexOf20
																											                    , iteratorOf21 + startIndexOf21
																											                    , iteratorOf22 + startIndexOf22
																											                    , iteratorOf23 + startIndexOf23
																											                    , iteratorOf24 + startIndexOf24
																											                    , iteratorOf25 + startIndexOf25
																											                    , iteratorOf26 + startIndexOf26
																											                    , iteratorOf27 + startIndexOf27
																											                ];
																											                writer.Serialize(element, options, serializer);
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 28)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 28)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26] * lengths[27];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26], lengths[27]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = 0, end27 = lengths[27]; iterator27 < end27; iterator27++)
            																											{
                																											answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27] = elements[index++];
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = starts[27], end27 = starts[27] + lengths[27]; iterator27 < end27; iterator27++)
            																											{
                																											answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27] = elements[index++];
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension29ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            var startIndexOf27 = value.GetLowerBound(27);
            var lengthOf27 = value.GetLength(27);
            var startIndexOf28 = value.GetLowerBound(28);
            var lengthOf28 = value.GetLength(28);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf27);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf28);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0 || startIndexOf27 != 0 || startIndexOf28 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf27);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf28);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																											            for (var iteratorOf27 = 0; iteratorOf27 < lengthOf27; iteratorOf27++)
																											            {
																												            for (var iteratorOf28 = 0; iteratorOf28 < lengthOf28; iteratorOf28++)
																												            {
																												                if (first)
																												                {
																												                    first = false;
																												                }
																												                else
																												                {
																												                    var span = writer.Writer.GetSpan(1);
																												                    span[0] = (byte)',';
																												                    writer.Writer.Advance(1);
																												                }

																												                var element = value[
																												                    iteratorOf0 + startIndexOf0
																												                    , iteratorOf1 + startIndexOf1
																												                    , iteratorOf2 + startIndexOf2
																												                    , iteratorOf3 + startIndexOf3
																												                    , iteratorOf4 + startIndexOf4
																												                    , iteratorOf5 + startIndexOf5
																												                    , iteratorOf6 + startIndexOf6
																												                    , iteratorOf7 + startIndexOf7
																												                    , iteratorOf8 + startIndexOf8
																												                    , iteratorOf9 + startIndexOf9
																												                    , iteratorOf10 + startIndexOf10
																												                    , iteratorOf11 + startIndexOf11
																												                    , iteratorOf12 + startIndexOf12
																												                    , iteratorOf13 + startIndexOf13
																												                    , iteratorOf14 + startIndexOf14
																												                    , iteratorOf15 + startIndexOf15
																												                    , iteratorOf16 + startIndexOf16
																												                    , iteratorOf17 + startIndexOf17
																												                    , iteratorOf18 + startIndexOf18
																												                    , iteratorOf19 + startIndexOf19
																												                    , iteratorOf20 + startIndexOf20
																												                    , iteratorOf21 + startIndexOf21
																												                    , iteratorOf22 + startIndexOf22
																												                    , iteratorOf23 + startIndexOf23
																												                    , iteratorOf24 + startIndexOf24
																												                    , iteratorOf25 + startIndexOf25
																												                    , iteratorOf26 + startIndexOf26
																												                    , iteratorOf27 + startIndexOf27
																												                    , iteratorOf28 + startIndexOf28
																												                ];
																												                writer.Serialize(element, options, serializer);
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 29)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 29)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26] * lengths[27] * lengths[28];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26], lengths[27], lengths[28]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = 0, end27 = lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = 0, end28 = lengths[28]; iterator28 < end28; iterator28++)
            																												{
                																												answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28] = elements[index++];
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = starts[27], end27 = starts[27] + lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = starts[28], end28 = starts[28] + lengths[28]; iterator28 < end28; iterator28++)
            																												{
                																												answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28] = elements[index++];
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension30ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            var startIndexOf27 = value.GetLowerBound(27);
            var lengthOf27 = value.GetLength(27);
            var startIndexOf28 = value.GetLowerBound(28);
            var lengthOf28 = value.GetLength(28);
            var startIndexOf29 = value.GetLowerBound(29);
            var lengthOf29 = value.GetLength(29);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf27);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf28);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf29);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0 || startIndexOf27 != 0 || startIndexOf28 != 0 || startIndexOf29 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf27);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf28);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf29);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																											            for (var iteratorOf27 = 0; iteratorOf27 < lengthOf27; iteratorOf27++)
																											            {
																												            for (var iteratorOf28 = 0; iteratorOf28 < lengthOf28; iteratorOf28++)
																												            {
																													            for (var iteratorOf29 = 0; iteratorOf29 < lengthOf29; iteratorOf29++)
																													            {
																													                if (first)
																													                {
																													                    first = false;
																													                }
																													                else
																													                {
																													                    var span = writer.Writer.GetSpan(1);
																													                    span[0] = (byte)',';
																													                    writer.Writer.Advance(1);
																													                }

																													                var element = value[
																													                    iteratorOf0 + startIndexOf0
																													                    , iteratorOf1 + startIndexOf1
																													                    , iteratorOf2 + startIndexOf2
																													                    , iteratorOf3 + startIndexOf3
																													                    , iteratorOf4 + startIndexOf4
																													                    , iteratorOf5 + startIndexOf5
																													                    , iteratorOf6 + startIndexOf6
																													                    , iteratorOf7 + startIndexOf7
																													                    , iteratorOf8 + startIndexOf8
																													                    , iteratorOf9 + startIndexOf9
																													                    , iteratorOf10 + startIndexOf10
																													                    , iteratorOf11 + startIndexOf11
																													                    , iteratorOf12 + startIndexOf12
																													                    , iteratorOf13 + startIndexOf13
																													                    , iteratorOf14 + startIndexOf14
																													                    , iteratorOf15 + startIndexOf15
																													                    , iteratorOf16 + startIndexOf16
																													                    , iteratorOf17 + startIndexOf17
																													                    , iteratorOf18 + startIndexOf18
																													                    , iteratorOf19 + startIndexOf19
																													                    , iteratorOf20 + startIndexOf20
																													                    , iteratorOf21 + startIndexOf21
																													                    , iteratorOf22 + startIndexOf22
																													                    , iteratorOf23 + startIndexOf23
																													                    , iteratorOf24 + startIndexOf24
																													                    , iteratorOf25 + startIndexOf25
																													                    , iteratorOf26 + startIndexOf26
																													                    , iteratorOf27 + startIndexOf27
																													                    , iteratorOf28 + startIndexOf28
																													                    , iteratorOf29 + startIndexOf29
																													                ];
																													                writer.Serialize(element, options, serializer);
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 30)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 30)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26] * lengths[27] * lengths[28] * lengths[29];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26], lengths[27], lengths[28], lengths[29]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = 0, end27 = lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = 0, end28 = lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = 0, end29 = lengths[29]; iterator29 < end29; iterator29++)
            																													{
                																													answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29] = elements[index++];
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = starts[27], end27 = starts[27] + lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = starts[28], end28 = starts[28] + lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = starts[29], end29 = starts[29] + lengths[29]; iterator29 < end29; iterator29++)
            																													{
                																													answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29] = elements[index++];
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension31ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            var startIndexOf27 = value.GetLowerBound(27);
            var lengthOf27 = value.GetLength(27);
            var startIndexOf28 = value.GetLowerBound(28);
            var lengthOf28 = value.GetLength(28);
            var startIndexOf29 = value.GetLowerBound(29);
            var lengthOf29 = value.GetLength(29);
            var startIndexOf30 = value.GetLowerBound(30);
            var lengthOf30 = value.GetLength(30);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf27);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf28);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf29);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf30);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0 || startIndexOf27 != 0 || startIndexOf28 != 0 || startIndexOf29 != 0 || startIndexOf30 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf27);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf28);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf29);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf30);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																											            for (var iteratorOf27 = 0; iteratorOf27 < lengthOf27; iteratorOf27++)
																											            {
																												            for (var iteratorOf28 = 0; iteratorOf28 < lengthOf28; iteratorOf28++)
																												            {
																													            for (var iteratorOf29 = 0; iteratorOf29 < lengthOf29; iteratorOf29++)
																													            {
																														            for (var iteratorOf30 = 0; iteratorOf30 < lengthOf30; iteratorOf30++)
																														            {
																														                if (first)
																														                {
																														                    first = false;
																														                }
																														                else
																														                {
																														                    var span = writer.Writer.GetSpan(1);
																														                    span[0] = (byte)',';
																														                    writer.Writer.Advance(1);
																														                }

																														                var element = value[
																														                    iteratorOf0 + startIndexOf0
																														                    , iteratorOf1 + startIndexOf1
																														                    , iteratorOf2 + startIndexOf2
																														                    , iteratorOf3 + startIndexOf3
																														                    , iteratorOf4 + startIndexOf4
																														                    , iteratorOf5 + startIndexOf5
																														                    , iteratorOf6 + startIndexOf6
																														                    , iteratorOf7 + startIndexOf7
																														                    , iteratorOf8 + startIndexOf8
																														                    , iteratorOf9 + startIndexOf9
																														                    , iteratorOf10 + startIndexOf10
																														                    , iteratorOf11 + startIndexOf11
																														                    , iteratorOf12 + startIndexOf12
																														                    , iteratorOf13 + startIndexOf13
																														                    , iteratorOf14 + startIndexOf14
																														                    , iteratorOf15 + startIndexOf15
																														                    , iteratorOf16 + startIndexOf16
																														                    , iteratorOf17 + startIndexOf17
																														                    , iteratorOf18 + startIndexOf18
																														                    , iteratorOf19 + startIndexOf19
																														                    , iteratorOf20 + startIndexOf20
																														                    , iteratorOf21 + startIndexOf21
																														                    , iteratorOf22 + startIndexOf22
																														                    , iteratorOf23 + startIndexOf23
																														                    , iteratorOf24 + startIndexOf24
																														                    , iteratorOf25 + startIndexOf25
																														                    , iteratorOf26 + startIndexOf26
																														                    , iteratorOf27 + startIndexOf27
																														                    , iteratorOf28 + startIndexOf28
																														                    , iteratorOf29 + startIndexOf29
																														                    , iteratorOf30 + startIndexOf30
																														                ];
																														                writer.Serialize(element, options, serializer);
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 31)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 31)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26] * lengths[27] * lengths[28] * lengths[29] * lengths[30];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26], lengths[27], lengths[28], lengths[29], lengths[30]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = 0, end27 = lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = 0, end28 = lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = 0, end29 = lengths[29]; iterator29 < end29; iterator29++)
            																													{
            																														for (int iterator30 = 0, end30 = lengths[30]; iterator30 < end30; iterator30++)
            																														{
                																														answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29, iterator30] = elements[index++];
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = starts[27], end27 = starts[27] + lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = starts[28], end28 = starts[28] + lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = starts[29], end29 = starts[29] + lengths[29]; iterator29 < end29; iterator29++)
            																													{
            																														for (int iterator30 = starts[30], end30 = starts[30] + lengths[30]; iterator30 < end30; iterator30++)
            																														{
                																														answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29, iterator30] = elements[index++];
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

    public sealed unsafe class Dimension32ArrayFormatter<T>
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]?>
#else
        : IJsonFormatter<T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]>
#endif
    {
#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] value, JsonSerializerOptions options)
#endif
        {
            if (value == null)
            {
                var span = writer.Writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Writer.Advance(4);
                return;
            }

            var startIndexOf0 = value.GetLowerBound(0);
            var lengthOf0 = value.GetLength(0);
            var startIndexOf1 = value.GetLowerBound(1);
            var lengthOf1 = value.GetLength(1);
            var startIndexOf2 = value.GetLowerBound(2);
            var lengthOf2 = value.GetLength(2);
            var startIndexOf3 = value.GetLowerBound(3);
            var lengthOf3 = value.GetLength(3);
            var startIndexOf4 = value.GetLowerBound(4);
            var lengthOf4 = value.GetLength(4);
            var startIndexOf5 = value.GetLowerBound(5);
            var lengthOf5 = value.GetLength(5);
            var startIndexOf6 = value.GetLowerBound(6);
            var lengthOf6 = value.GetLength(6);
            var startIndexOf7 = value.GetLowerBound(7);
            var lengthOf7 = value.GetLength(7);
            var startIndexOf8 = value.GetLowerBound(8);
            var lengthOf8 = value.GetLength(8);
            var startIndexOf9 = value.GetLowerBound(9);
            var lengthOf9 = value.GetLength(9);
            var startIndexOf10 = value.GetLowerBound(10);
            var lengthOf10 = value.GetLength(10);
            var startIndexOf11 = value.GetLowerBound(11);
            var lengthOf11 = value.GetLength(11);
            var startIndexOf12 = value.GetLowerBound(12);
            var lengthOf12 = value.GetLength(12);
            var startIndexOf13 = value.GetLowerBound(13);
            var lengthOf13 = value.GetLength(13);
            var startIndexOf14 = value.GetLowerBound(14);
            var lengthOf14 = value.GetLength(14);
            var startIndexOf15 = value.GetLowerBound(15);
            var lengthOf15 = value.GetLength(15);
            var startIndexOf16 = value.GetLowerBound(16);
            var lengthOf16 = value.GetLength(16);
            var startIndexOf17 = value.GetLowerBound(17);
            var lengthOf17 = value.GetLength(17);
            var startIndexOf18 = value.GetLowerBound(18);
            var lengthOf18 = value.GetLength(18);
            var startIndexOf19 = value.GetLowerBound(19);
            var lengthOf19 = value.GetLength(19);
            var startIndexOf20 = value.GetLowerBound(20);
            var lengthOf20 = value.GetLength(20);
            var startIndexOf21 = value.GetLowerBound(21);
            var lengthOf21 = value.GetLength(21);
            var startIndexOf22 = value.GetLowerBound(22);
            var lengthOf22 = value.GetLength(22);
            var startIndexOf23 = value.GetLowerBound(23);
            var lengthOf23 = value.GetLength(23);
            var startIndexOf24 = value.GetLowerBound(24);
            var lengthOf24 = value.GetLength(24);
            var startIndexOf25 = value.GetLowerBound(25);
            var lengthOf25 = value.GetLength(25);
            var startIndexOf26 = value.GetLowerBound(26);
            var lengthOf26 = value.GetLength(26);
            var startIndexOf27 = value.GetLowerBound(27);
            var lengthOf27 = value.GetLength(27);
            var startIndexOf28 = value.GetLowerBound(28);
            var lengthOf28 = value.GetLength(28);
            var startIndexOf29 = value.GetLowerBound(29);
            var lengthOf29 = value.GetLength(29);
            var startIndexOf30 = value.GetLowerBound(30);
            var lengthOf30 = value.GetLength(30);
            var startIndexOf31 = value.GetLowerBound(31);
            var lengthOf31 = value.GetLength(31);
            {
                const int sizeHint = 12;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)'{';
                span[ 1] = (byte)'"';
                span[ 2] = (byte)'l';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'n';
                span[ 5] = (byte)'g';
                span[ 6] = (byte)'t';
                span[ 7] = (byte)'h';
                span[ 8] = (byte)'s';
                span[ 9] = (byte)'"';
                span[10] = (byte)':';
                span[11] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            writer.Write(lengthOf0);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf1);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf2);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf3);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf4);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf5);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf6);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf7);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf8);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf9);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf10);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf11);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf12);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf13);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf14);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf15);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf16);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf17);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf18);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf19);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf20);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf21);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf22);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf23);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf24);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf25);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf26);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf27);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf28);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf29);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf30);
            {
                var span = writer.Writer.GetSpan(1);
                span[0] = (byte)',';
                writer.Writer.Advance(1);
            }
            writer.Write(lengthOf31);

            if (startIndexOf0 != 0 || startIndexOf1 != 0 || startIndexOf2 != 0 || startIndexOf3 != 0 || startIndexOf4 != 0 || startIndexOf5 != 0 || startIndexOf6 != 0 || startIndexOf7 != 0 || startIndexOf8 != 0 || startIndexOf9 != 0 || startIndexOf10 != 0 || startIndexOf11 != 0 || startIndexOf12 != 0 || startIndexOf13 != 0 || startIndexOf14 != 0 || startIndexOf15 != 0 || startIndexOf16 != 0 || startIndexOf17 != 0 || startIndexOf18 != 0 || startIndexOf19 != 0 || startIndexOf20 != 0 || startIndexOf21 != 0 || startIndexOf22 != 0 || startIndexOf23 != 0 || startIndexOf24 != 0 || startIndexOf25 != 0 || startIndexOf26 != 0 || startIndexOf27 != 0 || startIndexOf28 != 0 || startIndexOf29 != 0 || startIndexOf30 != 0 || startIndexOf31 != 0)
            {
                {
                    const int sizeHint = 12;
                    var span = writer.Writer.GetSpan(sizeHint);
                    span[ 0] = (byte)']';
                    span[ 1] = (byte)',';
                    span[ 2] = (byte)'"';
                    span[ 3] = (byte)'s';
                    span[ 4] = (byte)'t';
                    span[ 5] = (byte)'a';
                    span[ 6] = (byte)'r';
                    span[ 7] = (byte)'t';
                    span[ 8] = (byte)'s';
                    span[ 9] = (byte)'"';
                    span[10] = (byte)':';
                    span[11] = (byte)'[';
                    writer.Writer.Advance(sizeHint);
                }

                writer.Write(startIndexOf0);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf1);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf2);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf3);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf4);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf5);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf6);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf7);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf8);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf9);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf10);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf11);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf12);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf13);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf14);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf15);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf16);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf17);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf18);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf19);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf20);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf21);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf22);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf23);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf24);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf25);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf26);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf27);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf28);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf29);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf30);
                {
                    var span = writer.Writer.GetSpan(1);
                    span[0] = (byte)',';
                    writer.Writer.Advance(1);
                }
                writer.Write(startIndexOf31);
            }

            {
                const int sizeHint = 14;
                var span = writer.Writer.GetSpan(sizeHint);
                span[ 0] = (byte)']';
                span[ 1] = (byte)',';
                span[ 2] = (byte)'"';
                span[ 3] = (byte)'e';
                span[ 4] = (byte)'l';
                span[ 5] = (byte)'e';
                span[ 6] = (byte)'m';
                span[ 7] = (byte)'e';
                span[ 8] = (byte)'n';
                span[ 9] = (byte)'t';
                span[10] = (byte)'s';
                span[11] = (byte)'"';
                span[12] = (byte)':';
                span[13] = (byte)'[';
                writer.Writer.Advance(sizeHint);
            }

            var serializer = options.Resolver.GetSerializeStatic<T>();
            if (serializer.ToPointer() == null)
            {
                throw new NullReferenceException();
            }

            var first = true;
            for (var iteratorOf0 = 0; iteratorOf0 < lengthOf0; iteratorOf0++)
            {
	            for (var iteratorOf1 = 0; iteratorOf1 < lengthOf1; iteratorOf1++)
	            {
		            for (var iteratorOf2 = 0; iteratorOf2 < lengthOf2; iteratorOf2++)
		            {
			            for (var iteratorOf3 = 0; iteratorOf3 < lengthOf3; iteratorOf3++)
			            {
				            for (var iteratorOf4 = 0; iteratorOf4 < lengthOf4; iteratorOf4++)
				            {
					            for (var iteratorOf5 = 0; iteratorOf5 < lengthOf5; iteratorOf5++)
					            {
						            for (var iteratorOf6 = 0; iteratorOf6 < lengthOf6; iteratorOf6++)
						            {
							            for (var iteratorOf7 = 0; iteratorOf7 < lengthOf7; iteratorOf7++)
							            {
								            for (var iteratorOf8 = 0; iteratorOf8 < lengthOf8; iteratorOf8++)
								            {
									            for (var iteratorOf9 = 0; iteratorOf9 < lengthOf9; iteratorOf9++)
									            {
										            for (var iteratorOf10 = 0; iteratorOf10 < lengthOf10; iteratorOf10++)
										            {
											            for (var iteratorOf11 = 0; iteratorOf11 < lengthOf11; iteratorOf11++)
											            {
												            for (var iteratorOf12 = 0; iteratorOf12 < lengthOf12; iteratorOf12++)
												            {
													            for (var iteratorOf13 = 0; iteratorOf13 < lengthOf13; iteratorOf13++)
													            {
														            for (var iteratorOf14 = 0; iteratorOf14 < lengthOf14; iteratorOf14++)
														            {
															            for (var iteratorOf15 = 0; iteratorOf15 < lengthOf15; iteratorOf15++)
															            {
																            for (var iteratorOf16 = 0; iteratorOf16 < lengthOf16; iteratorOf16++)
																            {
																	            for (var iteratorOf17 = 0; iteratorOf17 < lengthOf17; iteratorOf17++)
																	            {
																		            for (var iteratorOf18 = 0; iteratorOf18 < lengthOf18; iteratorOf18++)
																		            {
																			            for (var iteratorOf19 = 0; iteratorOf19 < lengthOf19; iteratorOf19++)
																			            {
																				            for (var iteratorOf20 = 0; iteratorOf20 < lengthOf20; iteratorOf20++)
																				            {
																					            for (var iteratorOf21 = 0; iteratorOf21 < lengthOf21; iteratorOf21++)
																					            {
																						            for (var iteratorOf22 = 0; iteratorOf22 < lengthOf22; iteratorOf22++)
																						            {
																							            for (var iteratorOf23 = 0; iteratorOf23 < lengthOf23; iteratorOf23++)
																							            {
																								            for (var iteratorOf24 = 0; iteratorOf24 < lengthOf24; iteratorOf24++)
																								            {
																									            for (var iteratorOf25 = 0; iteratorOf25 < lengthOf25; iteratorOf25++)
																									            {
																										            for (var iteratorOf26 = 0; iteratorOf26 < lengthOf26; iteratorOf26++)
																										            {
																											            for (var iteratorOf27 = 0; iteratorOf27 < lengthOf27; iteratorOf27++)
																											            {
																												            for (var iteratorOf28 = 0; iteratorOf28 < lengthOf28; iteratorOf28++)
																												            {
																													            for (var iteratorOf29 = 0; iteratorOf29 < lengthOf29; iteratorOf29++)
																													            {
																														            for (var iteratorOf30 = 0; iteratorOf30 < lengthOf30; iteratorOf30++)
																														            {
																															            for (var iteratorOf31 = 0; iteratorOf31 < lengthOf31; iteratorOf31++)
																															            {
																															                if (first)
																															                {
																															                    first = false;
																															                }
																															                else
																															                {
																															                    var span = writer.Writer.GetSpan(1);
																															                    span[0] = (byte)',';
																															                    writer.Writer.Advance(1);
																															                }

																															                var element = value[
																															                    iteratorOf0 + startIndexOf0
																															                    , iteratorOf1 + startIndexOf1
																															                    , iteratorOf2 + startIndexOf2
																															                    , iteratorOf3 + startIndexOf3
																															                    , iteratorOf4 + startIndexOf4
																															                    , iteratorOf5 + startIndexOf5
																															                    , iteratorOf6 + startIndexOf6
																															                    , iteratorOf7 + startIndexOf7
																															                    , iteratorOf8 + startIndexOf8
																															                    , iteratorOf9 + startIndexOf9
																															                    , iteratorOf10 + startIndexOf10
																															                    , iteratorOf11 + startIndexOf11
																															                    , iteratorOf12 + startIndexOf12
																															                    , iteratorOf13 + startIndexOf13
																															                    , iteratorOf14 + startIndexOf14
																															                    , iteratorOf15 + startIndexOf15
																															                    , iteratorOf16 + startIndexOf16
																															                    , iteratorOf17 + startIndexOf17
																															                    , iteratorOf18 + startIndexOf18
																															                    , iteratorOf19 + startIndexOf19
																															                    , iteratorOf20 + startIndexOf20
																															                    , iteratorOf21 + startIndexOf21
																															                    , iteratorOf22 + startIndexOf22
																															                    , iteratorOf23 + startIndexOf23
																															                    , iteratorOf24 + startIndexOf24
																															                    , iteratorOf25 + startIndexOf25
																															                    , iteratorOf26 + startIndexOf26
																															                    , iteratorOf27 + startIndexOf27
																															                    , iteratorOf28 + startIndexOf28
																															                    , iteratorOf29 + startIndexOf29
																															                    , iteratorOf30 + startIndexOf30
																															                    , iteratorOf31 + startIndexOf31
																															                ];
																															                writer.Serialize(element, options, serializer);
            																															}
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            {
                const int sizeHint = 2;
                var span = writer.Writer.GetSpan(sizeHint);
                span[0] = (byte)']';
                span[1] = (byte)'}';
                writer.Writer.Advance(sizeHint);
            }
        }

#if CSHARP_8_OR_NEWER
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,]? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            reader.ReadIsBeginObjectWithVerify();

            var lengths = default(int[]);
            var starts = default(int[]);
            var elements = default(T[]);
            var count = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keySpan = reader.ReadPropertyNameSegmentRaw();
                switch (keySpan.Length)
                {
                    case 6:
                        if (keySpan[0] != 's' || keySpan[1] != 't' || keySpan[2] != 'a' || keySpan[3] != 'r' || keySpan[4] != 't' || keySpan[5] != 's')
                        {
                            goto default;
                        }

                        starts = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (starts != null && starts.Length != 32)
                        {
                            throw new JsonParsingException("Lower bounds' length does not equal to two times of rank.");
                        }

                        continue;
                    case 7:
                        if (keySpan[0] != 'l' || keySpan[1] != 'e' || keySpan[2] != 'n' || keySpan[3] != 'g' || keySpan[4] != 't' || keySpan[5] != 'h' || keySpan[6] != 's')
                        {
                            goto default;
                        }
                            
                        lengths = Int32ArrayFormatter.DeserializeStatic(ref reader, options);
                        if (lengths != null && lengths.Length != 32)
                        {
                            throw new JsonParsingException("Lengths' length does not equal to two times of rank.");
                        }

                        continue;
                    case 8:
                        if (keySpan[0] != 'e' || keySpan[1] != 'l' || keySpan[2] != 'e' || keySpan[3] != 'm' || keySpan[4] != 'e' || keySpan[5] != 'n' || keySpan[6] != 't' || keySpan[7] != 's')
                        {
                            goto default;
                        }

                        elements = AddArrayFormatter<T>.DeserializeStatic(ref reader, options);
                        continue;
                    default:
                        reader.ReadNextBlock();
                        continue;
                }
            }

            if (lengths == null)
            {
                return default;
            }

            var totalLength = lengths[0] * lengths[1] * lengths[2] * lengths[3] * lengths[4] * lengths[5] * lengths[6] * lengths[7] * lengths[8] * lengths[9] * lengths[10] * lengths[11] * lengths[12] * lengths[13] * lengths[14] * lengths[15] * lengths[16] * lengths[17] * lengths[18] * lengths[19] * lengths[20] * lengths[21] * lengths[22] * lengths[23] * lengths[24] * lengths[25] * lengths[26] * lengths[27] * lengths[28] * lengths[29] * lengths[30] * lengths[31];

            if (starts == null)
            {
                goto NO_STARTS;
            }

            foreach (var startIndex in starts)
            {
                if (startIndex != 0)
                {
                    goto STARTS_EXIST;
                }
            }

            T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,] answer;

        NO_STARTS:
            answer = new T[lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5], lengths[6], lengths[7], lengths[8], lengths[9], lengths[10], lengths[11], lengths[12], lengths[13], lengths[14], lengths[15], lengths[16], lengths[17], lengths[18], lengths[19], lengths[20], lengths[21], lengths[22], lengths[23], lengths[24], lengths[25], lengths[26], lengths[27], lengths[28], lengths[29], lengths[30], lengths[31]];
            if (elements == null)
            {
                return answer;
            }
            
            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = 0, end0 = lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = 0, end1 = lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = 0, end2 = lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = 0, end3 = lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = 0, end4 = lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = 0, end5 = lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = 0, end6 = lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = 0, end7 = lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = 0, end8 = lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = 0, end9 = lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = 0, end10 = lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = 0, end11 = lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = 0, end12 = lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = 0, end13 = lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = 0, end14 = lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = 0, end15 = lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = 0, end16 = lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = 0, end17 = lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = 0, end18 = lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = 0, end19 = lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = 0, end20 = lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = 0, end21 = lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = 0, end22 = lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = 0, end23 = lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = 0, end24 = lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = 0, end25 = lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = 0, end26 = lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = 0, end27 = lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = 0, end28 = lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = 0, end29 = lengths[29]; iterator29 < end29; iterator29++)
            																													{
            																														for (int iterator30 = 0, end30 = lengths[30]; iterator30 < end30; iterator30++)
            																														{
            																															for (int iterator31 = 0, end31 = lengths[31]; iterator31 < end31; iterator31++)
            																															{
                																															answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29, iterator30, iterator31] = elements[index++];
            																															}
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;

        STARTS_EXIST:
#if CSHARP_8_OR_NEWER
            answer = (Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,])!;
#else
            answer = Array.CreateInstance(typeof(T), lengths, starts) as T[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
#endif

            if (elements == null)
            {
                return answer;
            }

            if (elements.Length != totalLength)
            {
                throw new JsonParsingException("Does not match the length.");
            }

            for (int iterator0 = starts[0], end0 = starts[0] + lengths[0], index = 0; iterator0 < end0; iterator0++)
            {
            	for (int iterator1 = starts[1], end1 = starts[1] + lengths[1]; iterator1 < end1; iterator1++)
            	{
            		for (int iterator2 = starts[2], end2 = starts[2] + lengths[2]; iterator2 < end2; iterator2++)
            		{
            			for (int iterator3 = starts[3], end3 = starts[3] + lengths[3]; iterator3 < end3; iterator3++)
            			{
            				for (int iterator4 = starts[4], end4 = starts[4] + lengths[4]; iterator4 < end4; iterator4++)
            				{
            					for (int iterator5 = starts[5], end5 = starts[5] + lengths[5]; iterator5 < end5; iterator5++)
            					{
            						for (int iterator6 = starts[6], end6 = starts[6] + lengths[6]; iterator6 < end6; iterator6++)
            						{
            							for (int iterator7 = starts[7], end7 = starts[7] + lengths[7]; iterator7 < end7; iterator7++)
            							{
            								for (int iterator8 = starts[8], end8 = starts[8] + lengths[8]; iterator8 < end8; iterator8++)
            								{
            									for (int iterator9 = starts[9], end9 = starts[9] + lengths[9]; iterator9 < end9; iterator9++)
            									{
            										for (int iterator10 = starts[10], end10 = starts[10] + lengths[10]; iterator10 < end10; iterator10++)
            										{
            											for (int iterator11 = starts[11], end11 = starts[11] + lengths[11]; iterator11 < end11; iterator11++)
            											{
            												for (int iterator12 = starts[12], end12 = starts[12] + lengths[12]; iterator12 < end12; iterator12++)
            												{
            													for (int iterator13 = starts[13], end13 = starts[13] + lengths[13]; iterator13 < end13; iterator13++)
            													{
            														for (int iterator14 = starts[14], end14 = starts[14] + lengths[14]; iterator14 < end14; iterator14++)
            														{
            															for (int iterator15 = starts[15], end15 = starts[15] + lengths[15]; iterator15 < end15; iterator15++)
            															{
            																for (int iterator16 = starts[16], end16 = starts[16] + lengths[16]; iterator16 < end16; iterator16++)
            																{
            																	for (int iterator17 = starts[17], end17 = starts[17] + lengths[17]; iterator17 < end17; iterator17++)
            																	{
            																		for (int iterator18 = starts[18], end18 = starts[18] + lengths[18]; iterator18 < end18; iterator18++)
            																		{
            																			for (int iterator19 = starts[19], end19 = starts[19] + lengths[19]; iterator19 < end19; iterator19++)
            																			{
            																				for (int iterator20 = starts[20], end20 = starts[20] + lengths[20]; iterator20 < end20; iterator20++)
            																				{
            																					for (int iterator21 = starts[21], end21 = starts[21] + lengths[21]; iterator21 < end21; iterator21++)
            																					{
            																						for (int iterator22 = starts[22], end22 = starts[22] + lengths[22]; iterator22 < end22; iterator22++)
            																						{
            																							for (int iterator23 = starts[23], end23 = starts[23] + lengths[23]; iterator23 < end23; iterator23++)
            																							{
            																								for (int iterator24 = starts[24], end24 = starts[24] + lengths[24]; iterator24 < end24; iterator24++)
            																								{
            																									for (int iterator25 = starts[25], end25 = starts[25] + lengths[25]; iterator25 < end25; iterator25++)
            																									{
            																										for (int iterator26 = starts[26], end26 = starts[26] + lengths[26]; iterator26 < end26; iterator26++)
            																										{
            																											for (int iterator27 = starts[27], end27 = starts[27] + lengths[27]; iterator27 < end27; iterator27++)
            																											{
            																												for (int iterator28 = starts[28], end28 = starts[28] + lengths[28]; iterator28 < end28; iterator28++)
            																												{
            																													for (int iterator29 = starts[29], end29 = starts[29] + lengths[29]; iterator29 < end29; iterator29++)
            																													{
            																														for (int iterator30 = starts[30], end30 = starts[30] + lengths[30]; iterator30 < end30; iterator30++)
            																														{
            																															for (int iterator31 = starts[31], end31 = starts[31] + lengths[31]; iterator31 < end31; iterator31++)
            																															{
                																															answer[iterator0, iterator1, iterator2, iterator3, iterator4, iterator5, iterator6, iterator7, iterator8, iterator9, iterator10, iterator11, iterator12, iterator13, iterator14, iterator15, iterator16, iterator17, iterator18, iterator19, iterator20, iterator21, iterator22, iterator23, iterator24, iterator25, iterator26, iterator27, iterator28, iterator29, iterator30, iterator31] = elements[index++];
            																															}
            																														}
            																													}
            																												}
            																											}
            																										}
            																									}
            																								}
            																							}
            																						}
            																					}
            																				}
            																			}
            																		}
            																	}
            																}
            															}
            														}
            													}
            												}
            											}
            										}
            									}
            								}
            							}
            						}
            					}
            				}
            			}
            		}
            	}
            }

            return answer;
        }
    }

}