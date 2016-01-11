using Newtonsoft.Json;

namespace ECS.Utilities
{
    static class ObjectExtensions
    {
        // Initialize inner objects individually.
        // For example in default constructor some list property initialized with some values,
        // but in 'source' these items are cleaned -
        // without ObjectCreationHandling.Replace default constructor values will be added to result.
        static readonly JsonSerializerSettings _jsonSerializationSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        // Ref: http://stackoverflow.com/a/78612/1466456
        internal static object CloneJson(this object source)
        {
            // Don't serialize a null object, simply return the default for that object.
            if (ReferenceEquals(source, null))
                return null;

            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source), source.GetType(), _jsonSerializationSettings);            
        }
    }
}
