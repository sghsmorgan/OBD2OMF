using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBD2OMF
{
    /// <summary>
    /// Represents 
    /// </summary>
    public class OmfLinkType
    {
        [JsonProperty("typeid")]
        public string TypeId { get; }
        [JsonProperty("typeversion")]
        public string TypeVersion { get; set; }
        [JsonIgnore]
        public List<OmfSource> Source { get; set; }
        [JsonIgnore]
        public List<OmfTarget> Target { get; set; }
        [JsonProperty("values")]
        public List<Dictionary<string, object>> Values { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The source Type or Container</param>
        /// <param name="target">The destination Type or Container</param>
        public OmfLinkType(OmfSource source, OmfTarget target)
        {
            Source = new List<OmfSource>();
            Target = new List<OmfTarget>();
            Values = new List<Dictionary<string, object>>();
            Source.Add(source);
            Target.Add(target);
            TypeId = "__Link";
        }

        public void AddSourceTarget(OmfSource source, OmfTarget target)
        {
            Source.Add(source);
            Target.Add(target);
        }

        /// <summary>
        /// Gets a Json formatted string to use for requests
        /// </summary>
        /// <returns name="content">The formatted string</returns>
        public string GetJsonString()
        {
            for (int i = 0; i < Source.Count; i++)
            {
                Dictionary<string, object> value = new Dictionary<string, object>();
                value.Add("source", Source[i]);
                value.Add("target", Target[i]);
                Values.Add(value);

            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(this, settings);
            return "[" + content + "]";
        }
    }

    /// <summary>
    /// The source Type or Container for the Link
    /// </summary>
    public class OmfSource
    {
        [JsonProperty("typeid")]
        public string TypeId { get; set; } //Need to have either this or ContainerId specified 
        [JsonProperty("containerid")]
        public string ContainerId { get; set; } //Need to have either this or TypeId specified
        [JsonProperty("typeversion")]
        public string TypeVersion { get; set; } //Optional. Defaults to 1.0.0.0
        [JsonProperty("index")]
        public string Index { get; set; } //Required if TypeId is specified. Optional if ContainerId is specified

        /// <summary>
        /// Constructor for an OMF Type Source
        /// </summary>
        /// <param name="type">The OMF Type to use</param>
        /// <param name="index">Required index of the value to map</param>
        public OmfSource(OmfContainerType type, string index)
        {
            TypeId = type.Id;
            Index = index;
        }

        /// <summary>
        /// Constructor for an OMF Container Source
        /// </summary>
        /// <param name="container">The OMF Container to use</param>
        /// <param name="index">Optional index of the value to map</param>
        public OmfSource(OmfContainer container, string index = "")
        {
            ContainerId = container.Id;
            if (index != "")
                Index = index;
        }
    }

    /// <summary>
    /// The target Type or Container of the Link Type 
    /// </summary>
    public class OmfTarget
    {
        [JsonProperty("typeid")]
        public string TypeId { get; set; } //Need to have either this or ContainerId specified 
        [JsonProperty("containerid")]
        public string ContainerId { get; set; } //Need to have either this or TypeId specified
        [JsonProperty("typeversion")]
        public string TypeVersion { get; set; } //Optional. Defaults to 1.0.0.0
        [JsonProperty("index")]
        public string Index { get; set; } //Required if TypeId is specified. Optional if ContainerId is specified

        /// <summary>
        /// Constructor for an OMF Type Target
        /// </summary>
        /// <param name="type">The OMF Type to use</param>
        /// <param name="index">Required index of the value to map</param>
        public OmfTarget(OmfContainerType type, string index)
        {
            TypeId = type.Id;
            Index = index;
        }

        /// <summary>
        /// Constructor for an OMF Container Target
        /// </summary>
        /// <param name="container">The OMF Container to use</param>
        /// <param name="index">Optional index of the value to map</param>
        public OmfTarget(OmfContainer container, string index = "")
        {
            ContainerId = container.Id;
            if (index != "")
                Index = index;
        }
    }
}
