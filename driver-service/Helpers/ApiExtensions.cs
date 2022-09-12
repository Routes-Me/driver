using driver_service.Functions;
using driver_service.Models.DTO;
using driver_service.Models.Entities;
using Newtonsoft.Json;
using RestSharp;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        internal static dynamic GetVehicleDevice(DriverVehicle driverVehicle, string url, ref List<DriverDeviceDto> driverDeviceDto)
        {
            //List<DriverDeviceDto> getDeviceResponse = new List<DriverDeviceDto>();
            //List<DeviceVehicleDto> getVehicleResponse = new List<DeviceVehicleDto>();

            var client = new RestClient(url + Obfuscation.Encode(Convert.ToInt32(driverVehicle.VehicleId)) + "/devices?offset=1&limit=10&include=vehicles");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content;
                var deviceResponse = JsonConvert.DeserializeObject<GetResponse<DriverDeviceDto>>(result);
                driverDeviceDto = deviceResponse.Data.Select(d => new DriverDeviceDto
                {
                    DeviceId = d.DeviceId,
                    SerialNumber = d.SerialNumber,
                    SimSerialNumber = d.SimSerialNumber,
                    VehicleId = d.VehicleId
                }).ToList();
                var getVehicleResponse = deviceResponse.Included["vehicles"].FirstOrDefault();
                return getVehicleResponse;

            }
            else
            {
                throw new Exception(response.ErrorException.ToString());
            }

        }
    }
}