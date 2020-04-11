namespace Utf8Json.Internal.Formatters
{
    internal static class CollectionFormatterHelper
    {
        internal static readonly byte[][] GroupingName =
        {
            new byte[] { 0x4B, 0x65, 0x79, }, // Key
            new byte[] { 0x45, 0x6C, 0x65, 0x6D, 0x65, 0x6E, 0x74, 0x73, }, // Elements
        };

        //internal static readonly AutomataDictionary groupingAutomata;

        static CollectionFormatterHelper()
        {
            /*groupingAutomata = new AutomataDictionary
            {
                {JsonWriter.GetEncodedPropertyNameWithoutQuotation("Key"), 0 },
                {JsonWriter.GetEncodedPropertyNameWithoutQuotation("Elements"), 1 },
            };*/
        }
    }
}
