
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace driver_service.Functions
{
    public class Common
    {
        public static JArray SerializeJsonForIncludedRepo(List<dynamic> objList)
        {
            var modelsJson = JsonConvert.SerializeObject(objList,
                                 new JsonSerializerSettings
                                 {
                                     NullValueHandling = NullValueHandling.Ignore,
                                 });

            return JArray.Parse(modelsJson);
        }

        public IRestResponse PostAPI(string url, dynamic objectToSend)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception(response.Content);
            return response;
        }

        public IRestResponse PutAPI(string url, dynamic objectToSend)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.Content);
            return response;
        }

        public IRestResponse DeleteAPI(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.DELETE);
            return client.Execute(request);
        }

        public dynamic GetAPI(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpListenerException(400);

            return response;
        }
        public UriBuilder AppendQueryToUrl(UriBuilder baseUri, string queryToAppend)
        {
            if (baseUri.Query != null && baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;
            return baseUri;
        }
    }
}
