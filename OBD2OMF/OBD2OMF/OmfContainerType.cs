using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using OSIsoft.Models.OMF;

namespace OBD2OMF
{
    public class OmfContainerType
    {
        [JsonIgnore]
        public OmfTypeProperty.AttributeType TypeOfAttribute { get; set; }
        [JsonIgnore]
        public int NumberOfAttributes { get { return Properties.Count; } set {; } }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; } //The version of OMF being used
        [JsonProperty("type")]
        public string Type { get; }
        [JsonProperty("classification")]
        public string Classification { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("tags")]
        public string[] Tags { get; set; }
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
        [JsonProperty("properties")]
        public Dictionary<string, OmfTypeProperty> Properties { get; set; }

        public OmfContainerType(string id, string classification, string version = "1.0.0.0")
        {
            Id = id;
            Properties = new Dictionary<string, OmfTypeProperty>();
            Version = version;
            Type = "object";
            Classification = classification;
            NumberOfAttributes = Properties.Count;
        }


        /// <summary>
        /// Gets the Json formatted string of this Type object
        /// </summary>
        /// <returns name="content"></returns>
        public string GetJsonContent(out string errMsg)
        {
            errMsg = "";

            if (!Validate(out errMsg))
                return errMsg;

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(this, settings);
            return "[" + content + "]";
        }

        /// <summary>
        /// Attempts to add a new property to the Type message
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="label">The name of the new property</param>
        /// <param name="property">The property to add</param>
        /// <returns name="successful">bool indicating success</returns>
        public bool AddProperty(out string errMsg, string label, OmfTypeProperty property)
        {
            bool successful = true;
            errMsg = "";

            if (Properties.ContainsKey(label))
            {
                errMsg = $"The key {label} has already been added to properties";
                return false;
            }

            if (property.IsName)
            {
                foreach (OmfTypeProperty prop in Properties.Values)
                {
                    if (prop.IsName)
                    {
                        errMsg = "Only one property can have isname as true";
                        return false;
                    }
                }
            }

            if (property.Uom != null)
            {
                if (Version != "1.1.0.0")
                {
                    errMsg = "The property uom can only be added to Type messages of version 1.1 or later.";
                    return false;
                }
            }

            if (Classification == "dynamic" && !(property.Format == "date-time") && property.IsIndex)
            {
                errMsg = "The index for a dynamic Type must be in date-time format";
                return false;
            }

            Properties.Add(label, property);
            return successful;
        }

        /// <summary>
        /// Validates the Type message
        /// </summary>
        /// <returns>bool indicating if it is valid</returns>
        private bool Validate(out string errMsg)
        {
            errMsg = "";
            bool valid = true;

            if (!(Classification == "dynamic" || Classification == "static"))
            {
                errMsg = "The classification must be either dynamic or static";
                return false;
            }
            if (Properties.Count == 0)
            {
                errMsg = "There must be at least one Property";
                return false;
            }

            bool foundIsIndex = false;
            foreach (OmfTypeProperty prop in Properties.Values)
            {
                if (prop.IsIndex)
                    foundIsIndex = true;
                break;
            }
            if (!foundIsIndex)
            {
                errMsg = "At least one Property must have isindex marked as true";
                return false;
            }

            return valid;
        }
    }
}
