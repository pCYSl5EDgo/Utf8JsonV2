namespace System.Text.Json.Serialization
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonPropertyNameAttribute : JsonAttribute
    {
        public string Name { get; }

        public JsonPropertyNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
