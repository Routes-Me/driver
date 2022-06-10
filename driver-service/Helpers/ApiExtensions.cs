using driver_service.Functions;
using driver_service.Models.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static driver_service.Models.Response;

namespace driver_service.Helpers
{
    public static class ApiExtensions<T> where T : class
    {
        public static dynamic GetUsers(List<DriversDTO> driversDTOs, string url)
        {
            List<UsersDTO> lstUsers = new List<UsersDTO>();
            foreach (var item in driversDTOs)
            {
                var client = new RestClient(url + item.UserId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var userData = JsonConvert.DeserializeObject<GetResponse<UsersDTO>>(result);
                    lstUsers.AddRange(userData.Data);
                }
            }
            var usersList = lstUsers.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
        }

        public static dynamic GetVehicles(List<DriversDTO> driversDTOs, string url)
        {
            List<VehiclesDTO> vehiclesDTO = new List<VehiclesDTO>();
            List<int> id = new List<int>(driversDTOs.Select(x => Obfuscation.Decode(x.VehicleId)).ToList().Distinct());
            List<string> attr = new List<string> { "ModelYear", "PlateNumber", "CreatedAt" };

            var client = new RestClient(url + "?attr=" + attr);
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(id), ParameterType.RequestBody);
            request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(attr), ParameterType.QueryString);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content;
                var vehiclesData = JsonConvert.DeserializeObject<GetResponseApi<VehiclesDTO>>(result);
                vehiclesDTO.AddRange(vehiclesData.Data);
                var vehiclesList = vehiclesDTO.GroupBy(x => x.VehicleId).Select(a => a.First()).ToList();
                return Common.SerializeJsonForIncludedRepo(vehiclesList.Cast<dynamic>().ToList());
            }
            else
            {
                throw new Exception(response.ErrorException.ToString());
            }
            //return JArray.Parse(JsonConvert.SerializeObject(vehiclesDTO.GroupBy(x => x.VehicleId).Select(x => x.First()).ToList()));
        }
    }
}
