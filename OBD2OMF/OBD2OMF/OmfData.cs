using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OBD2OMF

{
    public class OmfData
    {
        [JsonProperty("typeid")]
        public string TypeId { get; set; }
        [JsonProperty("containerid")]
        public string ContainerId { get; set; }
        [JsonProperty("typeversion")]
        public string TypeVersion { get; set; }
        [JsonProperty("values")]
        public List<Dictionary<string, object>> Values { get; set; }
        [JsonIgnore]
        public OmfContainerType Type { get; set; }
        [JsonIgnore]
        private readonly Random _rand = new Random();
        [JsonIgnore]
        Random rnum = new Random();

        /// <summary>
        /// Constructor for predefined values
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="values"></param>
        /// <param name="type"></param>
        /// <param name="containerId"></param>
        public OmfData(string typeId, List<Dictionary<string, object>> values, OmfContainerType type, string containerId = "")
        {
            if (containerId == "")
                TypeId = typeId;
            else
                ContainerId = containerId;
            Values = values;
            Type = type;
        }

        /// <summary>
        /// Constructor for predefined values
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="values"></param>
        /// <param name="type"></param>
        /// <param name="containerId"></param>
        public OmfData(string typeId, OmfContainerType type, string containerId = "")
        {
            if (containerId == "")
                TypeId = typeId;
            else
                ContainerId = containerId;
            Values = new List<Dictionary<string, object>>();
            Type = type;
        }

        /// <summary>
        /// Converts the Data object to a Json formatted string
        /// </summary>
        /// <returns></returns>
        public string GetDataContentString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(this, settings);
            return "[" + content + "]";
        }

        /// <summary>
        /// Generates random values for each property of the given type and puts them in the data message
        /// </summary>
        /// <param name="numberOfIterations">The number of times to generate a random value for each property</param>
        /// <returns></returns>
        public void GenerateRandomValues(int numberOfIterations)
        {
            for (int k = 0; k < numberOfIterations; k++)
            {
                var valueArrayElement = new JObject { };
                Values.Add(new Dictionary<string, object>());
                int valueIndex = Values.Count - 1;

                for (var i = 0; i < Type.NumberOfAttributes; i++)
                {
                    var attributeTypeStringPrefix = Type.Properties.ElementAt(i).Key;
                    switch (Type.Properties.ElementAt(i).Value.Type)
                    {
                        case "integer":
                            Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, GetNextInteger());
                            break;
                        case "number":
                            Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, GetNextDouble());
                            break;
                        case "string":
                            if (Type.Properties.ElementAt(i).Value.Format == null || Type.Properties.ElementAt(i).Value.Format == "")
                                Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, GetNextString(10));
                            else if (Type.Properties.ElementAt(i).Value.Format == "date-time")
                            {
                                Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, DateTime.UtcNow);
                            }
                            break;
                        case "array":
                            Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, new string[3] { GetNextString(5), GetNextString(5), GetNextString(5) });
                            break;
                        case "boolean":
                            if (i % 2 == 0)
                                Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, true);
                            else
                                Values.ElementAt(valueIndex).Add(attributeTypeStringPrefix, false);
                            break;
                            /*case "object": These don't appear to be supported yet
                                if(Type.Properties.ElementAt(i).Value.Format == "dictionary")
                                {

                                }
                                else
                                {

                                }
                                break;*/
                    }
                }
            }
        }

        private int GetNextInteger()
        {
            return rnum.Next();
        }

        private double GetNextDouble()
        {
            return rnum.NextDouble();
        }

        private string GetNextString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }
    }
}
