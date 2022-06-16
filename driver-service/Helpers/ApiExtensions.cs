using driver_service.Functions;
using driver_service.Models;
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
    public static class ApiExtensions
    {
        internal static dynamic GetUsers(List<DriversReadDTO> driversReadDTOs, string url)
        {
            try
            {
                List<UsersDTO> usersDTOs = new List<UsersDTO>();
                foreach (var item in driversReadDTOs)
                {
                    var client = new RestClient(url + item.UserId);
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content;
                        var userData = JsonConvert.DeserializeObject<GetResponseApi<UsersDTO>>(result);
                        usersDTOs.AddRange(userData.Data);
                    }
                }
                var usersList = usersDTOs.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
                return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
            }
            catch (Exception ex)
            {

                throw new Exception(ReturnResponse.ErrorResponse(ex.Message, 500));
            }
        }
        internal static dynamic GetVehicles(List<int?> VehicleIds, string url)
        {
            try
            {
                List<VehiclesDTO> vehiclesDTO = new List<VehiclesDTO>();
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(VehicleIds), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var vehiclesData = JsonConvert.DeserializeObject<GetResponseApi<VehiclesDTO>>(result);
                    vehiclesDTO.AddRange(vehiclesData.Data);
                }

                var vehiclesList = vehiclesDTO.GroupBy(x => x.VehicleId).Select(a => a.First()).ToList();
                return Common.SerializeJsonForIncludedRepo(vehiclesList.Cast<dynamic>().ToList());

            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
