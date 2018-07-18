using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using System.Threading;
//using InterfaceTestFramework;

namespace OBD2OMF
{
    /// <summary>
    /// Class that is responsible for creating, and sneding OMF Values, Types and Containers to an OMF endpoint
    /// </summary>
    public class OMFWriter
    {
        private string _omfEndpoint;
        private string _producerToken;
        private bool _useCompression;
        //private static Logger _logger;
        private static OMFClient _client;


        public OMFWriter(string endpoint, string producerToken, bool useCompression = true)
        {
            _omfEndpoint = endpoint;
            _producerToken = producerToken;
            _useCompression = useCompression;
            //_logger = new Logger("OMF Application Logger");
            _client = new OMFClient(_omfEndpoint, _producerToken, useCompression);

        }

        private void LogOMFMessage(string omfMessage)
        {
            //_logger.Log($"Sending OMF Message: \n {omfMessage}", LogLevel.Information);
        }

        
        /// </summary>
        /// <param name="id">The unique id for the new Type</param>
        /// <returns name="successful">successful or not</returns>
        public OmfContainerType SendTypeMessage(out string errMsg, string id, Dictionary<string, OmfTypeProperty> properties, string classification)
        {
            errMsg = "";

            OmfContainerType omfType = new OmfContainerType(id, classification);

            foreach (string property in properties.Keys)
            {
                omfType.AddProperty(out errMsg, property, properties[property]);
            }

            string content = omfType.GetJsonContent(out errMsg);
            HttpResponseMessage response = _client.SendOmfRequest(OMFClient.OmfMessageType.type, content);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                errMsg = response.ToString();
            return omfType;
        }

        public OmfLinkType SendLinkTypeMessage(out string errMsg, List<OmfSource> source, List<OmfTarget> target)
        {
            errMsg = "";
            OmfLinkType link = new OmfLinkType(source.ElementAt(0), target.ElementAt(0));
            for (int i = 1; i < source.Count; i++)
            {
                link.AddSourceTarget(source.ElementAt(i), target.ElementAt(i));
            }
            string content = link.GetJsonString();
            HttpResponseMessage response = _client.SendOmfRequest(OMFClient.OmfMessageType.data, content);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                errMsg = response.ToString();

            return link;
        }

        /// <summary>
        /// Sends a Container message to the endpoint specified in the OmfClient
        /// </summary>
        /// <param name="containerType">The Type that the Container will hold</param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public OmfContainer SendContainerMessage(out string errMsg, OmfContainerType containerType, string containerID)
        {
            errMsg = "";
            OmfContainer container = new OmfContainer(out errMsg, containerType, containerID);

            string content = container.GetContentString();
            HttpResponseMessage response = _client.SendOmfRequest(OMFClient.OmfMessageType.container, content);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                errMsg = response.ToString();

            return container;

        }

        /// <summary>
        /// Formats and sends an OMF Data message
        /// </summary>
        /// <param name="typeId">The typeId of the Data message</param>
        /// <param name="values">The values of this Data message</param>
        /// <returns>A new OmfData object</returns>
        public OmfData SendDataMessage(out string errMsg, string typeId, List<Dictionary<string, object>> values, OmfContainerType type, string containerId, string typeVersion = "1.0.0.0")
        {
            errMsg = "";
            OmfData data = new OmfData(typeId, values, type, containerId);

            string content = data.GetDataContentString();

            HttpResponseMessage response = _client.SendOmfRequest(OMFClient.OmfMessageType.data, content);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                errMsg = response.ToString();

            return data;
        }

        /// <summary>
        /// Formats and sends an OMF Data message
        /// </summary>
        /// <param name="typeId">The typeId of the Data message</param>
        /// <param name="values">The values of this Data message</param>
        /// <returns>A new OmfData object</returns>
        public OmfData UpdateDataMessage(out string errMsg, string typeId, List<Dictionary<string, object>> values, OmfContainerType type, string containerId, string typeVersion = "1.0.0.0")
        {
            errMsg = "";
            OmfData data = new OmfData(typeId, values, type, containerId);

            string content = data.GetDataContentString();

            HttpResponseMessage response = _client.SendOmfRequest(OMFClient.OmfMessageType.data, content, action:OMFClient.OmfAction.update);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                errMsg = response.ToString();

            return data;
        }
        

        public void SendOmfData()
        {
            string errMsg;
            //QAConnector.AdminUri adminUri = new QAConnector.AdminUri(true, "localhost", 8412);

            //List<QAConnector.AdminOmfappNode> omfNodes = OmfManager.GenerateOmfNodes(1, adminUri);
            string producerToken = "";

            //QAConnector.AdminNode omfNode = QAConnector.GetAdminNode(out errMsg, omfNodes.ElementAt(0).Name, QAConnector.AdminNodeType.Omfapp, adminUri);
            //producerToken = (QAConnector.GetProducerToken(out errMsg, (QAConnector.AdminOmfappNode)omfNode, adminUri, "Relay1"));
            //OmfManager manager = new OmfManager("https://localhost:8118/ingress/messages", producerToken, true);

            Dictionary<string, OmfTypeProperty> properties = new Dictionary<string, OmfTypeProperty>();

            OmfTypeProperty timestamp = new OmfTypeProperty(OmfTypeProperty.AttributeType.String, OmfTypeProperty.AttributeFormat.DateTime)
            {
                IsIndex = true
            };

            OmfTypeProperty integer0 = new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer);
            OmfTypeProperty integer1 = new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer);
            OmfTypeProperty integer2 = new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer);
            OmfTypeProperty integer3 = new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer);
            OmfTypeProperty integer4 = new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer);

            properties.Add("Timestamp", timestamp);
            properties.Add("Integer0", integer0);
            properties.Add("Integer1", integer1);
            properties.Add("Integer2", integer2);
            properties.Add("Integer3", integer3);
            properties.Add("Integer4", integer4);


            //Not sure what the types are for
            OmfContainerType type = SendTypeMessage(out errMsg, "IntegerDynamicType5", properties, "dynamic");

            //For PI points
            OmfContainer container = SendContainerMessage(out errMsg, type, "container_IntegerDynamicType5_Remote_1__0_Integer_0");

            List<Dictionary<string, object>> values = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> values2 = new List<Dictionary<string, object>>();

            Dictionary<string, object> value1 = new Dictionary<string, object>();
            //value1.Add("Timestamp", "2018-06-20T18:17:40.2501439Z");
            value1.Add("Integer0", 0);
            value1.Add("Integer1", 1);
            value1.Add("Integer2", 2);
            value1.Add("Integer3", 3);
            value1.Add("Integer4", 4);

            Dictionary<string, object> value2 = new Dictionary<string, object>();
            //value2.Add("Timestamp", "2018-06-20T18:17:40.251147Z");
            value2.Add("Integer0", 10);
            value2.Add("Integer1", 11);
            value2.Add("Integer2", 12);
            value2.Add("Integer3", 13);
            value2.Add("Integer4", 14);

            Dictionary<string, object> value3 = new Dictionary<string, object>();
            //value2.Add("Timestamp", "2018-06-20T18:17:40.251147Z");
            value3.Add("Integer0", 15);
            value3.Add("Integer1", 16);
            value3.Add("Integer2", 17);
            value3.Add("Integer3", 23);
            value3.Add("Integer4", 24);

            values.Add(value1);
            values.Add(value2);
            values2.Add(value3);

            OmfData data1 = SendDataMessage(out errMsg, null, values, type, container.Id);
            //Allow time for replication
            Thread.Sleep(10000);

            var updatedData = UpdateDataMessage(out errMsg, null, values2, type, container.Id);
            

            Dictionary<string, OmfTypeProperty> properties2 = new Dictionary<string, OmfTypeProperty>();

            properties2.Add("Index", new OmfTypeProperty(OmfTypeProperty.AttributeType.Integer) { IsIndex = true });
            properties2.Add("Property1", new OmfTypeProperty(OmfTypeProperty.AttributeType.Number));

            OmfContainerType type2 = SendTypeMessage(out errMsg, "LinkTest", properties2, "static");

            //TODO: Fix the Link messages. Need to figure out what's going wrong
            //Theory! They need to go in a data message

            OmfSource source1 = new OmfSource(type2, "_ROOT");
            OmfTarget target1 = new OmfTarget(type2, "Property1");

            OmfSource source2 = new OmfSource(type2, "Property1");
            OmfTarget target2 = new OmfTarget(container);

            List<OmfSource> sources = new List<OmfSource>();
            sources.Add(source1);
            sources.Add(source2);

            List<OmfTarget> targets = new List<OmfTarget>();
            targets.Add(target1);
            targets.Add(target2);

            OmfLinkType typeMsg = SendLinkTypeMessage(out errMsg, sources, targets);
        }

    }
}
