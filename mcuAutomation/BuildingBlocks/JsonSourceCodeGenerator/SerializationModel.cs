namespace McAfeeLabs.Engineering.Automation.Base.Json
{
    /// <summary>
    /// The serialization model for the generated class. Currently supported the DataContractJsonSerializer's
    /// [DataContract] / [DataMember] model, and JSON.NET's [JsonProperty] model.
    /// </summary>
    public enum SerializationModel
    {
        /// <summary>
        /// Programming model used by the DataContractJsonSerializer: types are decorated with
        /// [DataContract], members are decorated with [DataMember].
        /// </summary>
        DataContractJsonSerializer,

        /// <summary>
        /// Programming model used by the Json.NET serializer: types are decorated with [JsonObject] (usually not needed),
        /// members are decorated with [JsonProperty].
        /// </summary>
        JsonNet,
    }
}
