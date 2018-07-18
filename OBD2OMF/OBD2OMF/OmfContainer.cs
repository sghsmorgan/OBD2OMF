using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBD2OMF
{
    public class OmfContainer
    {
        [JsonIgnore]
        public OmfContainerType _containerType; //The Type associated with this Container
        [JsonProperty("typeid")]
        public string TypeId { get; set; } //Required
        [JsonProperty("id")]
        public string Id { get; set; } //Required
        [JsonProperty("typeversion")]
        public string TypeVersion { get; set; } //Optional: defaults to 1.0.0.0
        [JsonProperty("name")]
        public string Name { get; set; } //Optional
        [JsonProperty("description")]
        public string Description { get; set; } //Optional
        [JsonProperty("tags")]
        public string[] Tags { get; set; } //Optional
        [JsonProperty("metadata")]
        public Dictionary<string, string> MetaData { get; set; } //Optional
        [JsonProperty("indexes")]
        public string[] Indexes { get; set; } //Optional array o Type Property ids to be used as secondary indexes for the Container


        public OmfContainer(out string errMsg, OmfContainerType containerType, string containerID)
        {
            errMsg = "";
            _containerType = containerType;
            TypeId = containerType.Id;
            Id = containerID;

            if (containerType.Classification == "static")
            {
                errMsg = "Containers may only be used for dynamic Types.";
            }
        }

        /// <summary>
        /// Converts the Container object to a Json formatted string
        /// </summary>
        /// <returns></returns>
        public string GetContentString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(this, settings);
            return "[" + content + "]";
        }
    }
}
