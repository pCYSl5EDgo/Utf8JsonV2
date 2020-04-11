// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json.Internal;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        public static byte[] Serialize(Byte value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Byte value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Byte value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Byte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(SByte value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(SByte value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, SByte value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.SByte value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Int16 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int16 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int16 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Int32 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int32 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int32 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Int64 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Int64 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Int64 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Int64 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(UInt16 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt16 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt16 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt16 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(UInt32 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt32 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt32 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt32 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(UInt64 value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(UInt64 value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, UInt64 value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.UInt64 value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Char value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Char value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Char value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Char value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(String value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(String value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.String value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, String value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.String value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Boolean value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Boolean value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Boolean value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Boolean value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Single value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Single value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Single value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Single value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

        public static byte[] Serialize(Double value, JsonSerializerOptions options)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

        public static byte[] Serialize(Double value)
        {
            var array = ScratchArray;
            if (array == null)
            {
                ScratchArray = array = new byte[65536];
            }

            var jsonWriter = new JsonWriter(SequencePool.Shared, array)
            {
                CancellationToken = CancellationToken.None,
            };
            try
            {
                jsonWriter.Write(value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
            }
            return jsonWriter.FlushAndGetArray();
        }

#if SPAN_BUILTIN
        public static void Serialize(Stream stream, Double value)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var fastWriter = new JsonWriter(sequenceRental.Value)
                {
                    CancellationToken = CancellationToken.None,
                };
                try
                {
                    fastWriter.Write(value);
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Failed to serialize System.Double value.", ex);
                }
                fastWriter.Flush();

                try
                {
                    foreach (var segment in (ReadOnlySequence<byte>)sequenceRental.Value)
                    {
                        stream.Write(segment.Span);
                    }
                }
                catch (Exception ex)
                {
                    throw new JsonSerializationException("Error occurred while writing the serialized data to the stream.", ex);
                }
            }
        }
#endif

    }
}
