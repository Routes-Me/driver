using driver_service.Functions;
using driver_service.Models;
using driver_service.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using driver_service.Models.Common;
using driver_service.Models.Entities;
using static driver_service.Models.Response;

namespace driver_service.Helpers
{
    public class ApiExtensions
    {
        internal static dynamic GetUsers(List<DriversReadDto> driversReadDtOs, string url)
        {
            try
            {
                var usersDtOs = new List<UsersDto>();
                foreach (var item in driversReadDtOs)
                {
                    var client = new RestClient(url + item.UserId);
                    var request = new RestRequest(Method.GET);
                    var response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content;
                        var userData = JsonConvert.DeserializeObject<GetResponseApi<UsersDto>>(result);
                        usersDtOs.AddRange(userData.Data);
                    }
                }

                var usersList = usersDtOs.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
                return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
            }
            catch (Exception ex)
            {

                throw new Exception(ReturnResponse.ErrorResponse(ex.Message, 500));
            }
        }
        internal static dynamic GetVehicles(List<int?> vehicleIds, string url)
        {
            try
            {
                var vehiclesDto = new List<VehiclesDto>();
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(vehicleIds), ParameterType.RequestBody);
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var vehiclesData = JsonConvert.DeserializeObject<GetResponseApi<VehiclesDto>>(result);
                    vehiclesDto.AddRange(vehiclesData.Data);
                }

                var vehiclesList = vehiclesDto.GroupBy(x => x.VehicleId).Select(a => a.First()).ToList();
                return Common.SerializeJsonForIncludedRepo(vehiclesList.Cast<dynamic>().ToList());

            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
