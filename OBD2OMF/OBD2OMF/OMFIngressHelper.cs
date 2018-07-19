using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngressServiceAPI.API;

namespace OBD2OMF
{
    public static class OMFIngressHelper
    {
        private static Dictionary<IngressClient, string> ingressClients = new Dictionary<IngressClient, string>();

        public static void CreateClients(IEnumerable<OMFEndPoint> endPoints, bool useCompression = true)
        {
            if (!(endPoints?.Count() > 0))
            {
                string msg = "Must include at least one omf endpoint configuration";
                Logger.Write(msg);
                throw new ArgumentException(msg, nameof(endPoints));
            }

            //create client for each endpoint configuration
            foreach (OMFEndPoint endPoint in endPoints)
            {
                IngressClient ingressClient = new IngressClient(endPoint.URL, endPoint.ProducerToken);
                ingressClient.UseCompression = useCompression;
                OMFIngressHelper.ingressClients.Add(ingressClient, endPoint.Name);
            }



        }

        /// <summary>
        /// Sends values to each endpoint specified in <see cref="CreateClients(OMFEndPoint[], bool)"/>
        /// </summary>
        /// <param name="values">Values to be passed to <see cref="IngressClient.SendValuesAsync(IEnumerable{object})"/></param>
        /// <example>
        /// <code>
        ///     OMFIngressHelper.CreateClients(endPoints, true);
        ///     AssetLinkValues values = new AssetLinkValues();
        ///     values.Add(new T());
        ///     OMFIngressHelper.SendValuesToAllEndPointsAsync(values);
        /// </code>
        /// </example>
        /// <returns></returns>
        public async static Task SendValuesToAllEndPointsAsync(IEnumerable<object> values)
        {
            Dictionary<Task, string> tasks = new Dictionary<Task, string>();
            foreach (IngressClient client in ingressClients.Keys)
            {
                try
                {
                    tasks.Add(client.SendValuesAsync(values),ingressClients[client]);
                }
                catch (Exception e)
                {
                    Logger.Write("Exception thrown from client for endpoint: " + ingressClients[client] +
                        "\n" + e.Message +
                        "\n" + e.StackTrace);
                }
            }

            foreach (Task task in tasks.Keys)
            {

                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    Logger.Write("Exception thrown from client for endpoint: " + tasks[task]+
                        "\n" + e.Message +
                        "\n" + e.InnerException +
                        "\n" + e.StackTrace);
                }
            }

        }

        public static void CreateTypesForAllEndPoints(IEnumerable<string> types)
        {
            foreach (IngressClient client in ingressClients.Keys)
            {
                try
                {
                    client.CreateTypes(types);
                }
                catch (Exception e)
                {
                    Logger.Write("Exception thrown from client for endpoint: " + ingressClients[client] +
                        "\n" + e.Message +
                        "\n" + e.StackTrace);
                }
            }
        }
        public static void CreateContainersForAllEndPoints(IEnumerable<Container> streams)
        {
            foreach (IngressClient client in ingressClients.Keys)
            {
                try
                {
                    client.CreateContainers(streams);
                }
                catch (Exception e)
                {
                    Logger.Write("Exception thrown from client for endpoint: " + ingressClients[client] +
                        "\n" + e.Message +
                        "\n" + e.StackTrace);
                }
            }
        }
    }
}
