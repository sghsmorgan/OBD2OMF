using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OBD2OMF
{
    public class OmfTypeProperty
    {
        [JsonProperty("type")]
        public string Type { get; set; } //Required
        [JsonProperty("format")]
        public string Format { get; set; } //Optional
        [JsonProperty("isindex")]
        public bool IsIndex { get; set; } //Required for at least one property of a Type
        [JsonProperty("isname")]
        public bool IsName { get; set; }//Oprional for one property of a Type. True of False
        [JsonProperty("name")]
        public string Name { get; set; } //Optional
        [JsonProperty("description")]
        public string Description { get; set; } //Optional
        [JsonProperty("uom")]
        public string Uom { get; set; } //Optional unit of measure. Version 1.1 only
        [JsonProperty("additionalproperties")]
        public Dictionary<string, string> AdditionalProperties { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propType">The type of the property</param>
        /// <param name="format">Optional property format</param>
        /// <param name="additionalValues">Only use for dictionary formatted properties</param>
        public OmfTypeProperty(AttributeType propType, AttributeFormat format = AttributeFormat.Default, Dictionary<string, string> additionalValues = null)
        {
            Type = propType.ToString().ToLower();

            switch (propType)
            {
                case AttributeType.Array:
                    break;

                case AttributeType.Boolean:
                    break;

                case AttributeType.Integer:
                    if (format == AttributeFormat.Int64 || format == AttributeFormat.Int32 || format == AttributeFormat.Int16
                        || format == AttributeFormat.Uint64 || format == AttributeFormat.Uint32 || format == AttributeFormat.Uint16)
                        Format = format.ToString().ToLower();
                    break;

                case AttributeType.Number:
                    if (format == AttributeFormat.Float64 || format == AttributeFormat.Float32 || format == AttributeFormat.Float16)
                        Format = format.ToString().ToLower();
                    break;

                case AttributeType.Object:
                    if (format == AttributeFormat.Dictionary)
                    {
                        Format = format.ToString().ToLower();
                        AdditionalProperties = additionalValues;
                    }
                    break;


                case AttributeType.String:
                    if (format == AttributeFormat.DateTime)
                        Format = "date-time";
                    break;
            }
        }

        //TO DO: Add verification to set uom. This will need to be done once I have an object set up

        /// <summary>
        /// Converts OmfTypeProperty to Json
        /// </summary>
        /// <returns></returns>
        public string GetJsonString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(this, settings);
            return content;
        }

        /// <summary>
        /// The types of Properties
        /// </summary>
        public enum AttributeType
        {
            Integer = 0,
            Number,
            String,
            Array,
            Boolean,
            Object
        }

        /// <summary>
        /// The possible formats for a Property
        /// </summary>
        public enum AttributeFormat
        {
            Int64,
            Int32,
            Int16,
            Uint64,
            Uint32,
            Uint16,
            Float64,
            Float32,
            Float16,
            Dictionary,
            DateTime,
            Default
        }
    }
}
