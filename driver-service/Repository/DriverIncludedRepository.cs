using driver_service.Abstraction;
using driver_service.Functions;
using driver_service.Models;
using driver_service.Models.Common;
using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace driver_service.Repository
{
    public class DriverIncludedRepository : IDriverIncludedRepository
    {
        private readonly Dependencies _dependencies;
        private readonly AppSettings _appSettings;
        private readonly DriversServiceContext _context;
        public DriverIncludedRepository(IOptions<Dependencies> dependencies, IOptions<AppSettings> appSettings, DriversServiceContext context)
        {
            _dependencies = dependencies.Value;
            _appSettings = appSettings.Value;
            _context = context;
        }
        public dynamic GetDriver(Pagination pageInfo, string includeType)
        {
            int totalCount = 0;
            DriversGetResponse response = new DriversGetResponse();

            try
            {
                List<DriversModel> objDriversModelList = new List<DriversModel>();

                objDriversModelList = (from driver in _context.Drivers
                                       select new DriversModel()
                                       {
                                           DriverId = Obfuscation.Encode(driver.DriverId),
                                           UserId = Obfuscation.Encode(Convert.ToInt32(driver.UserId)),
                                           InstitutionId = Obfuscation.Encode(Convert.ToInt32(driver.InstitutionId))
                                       }).AsEnumerable().OrderBy(a => a.DriverId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                totalCount = _context.Drivers.ToList().Count;

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount,
                };
                dynamic includeData = new JObject();

                if (!string.IsNullOrEmpty(includeType))
                {
                    string[] includeArr = includeType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "users" || item.ToLower() == "user")
                            {
                                includeData.users = GetUsersIncludedData(objDriversModelList);
                            }
                            else if (item.ToLower() == "institutions" || item.ToLower() == "institution")
                            {
                                includeData.institutions = GetInstitutionsIncludedData(objDriversModelList);
                            }
                        }
                    }
                }
                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                response.status = true;
                response.message = CommonMessage.DriverRetrived;
                response.pagination = page;
                response.data = objDriversModelList;
                response.included = includeData;
                response.statusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
        public dynamic GetInstitutionsIncludedData(List<DriversModel> objDriversModelList)
        {
            List<InstitutionsModel> lstInstitutions = new List<InstitutionsModel>();
            foreach (var item in objDriversModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.InstitutionsUrl + item.InstitutionId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var institutionsData = JsonConvert.DeserializeObject<InstitutionsData>(result);
                    lstInstitutions.AddRange(institutionsData.Data);
                }
            }
            var institutionsList = lstInstitutions.GroupBy(x => x.InstitutionId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(institutionsList.Cast<dynamic>().ToList());
        }

        public dynamic GetUsersIncludedData(List<DriversModel> objDriversModelList)
        {
            List<UserModel> lstUsers = new List<UserModel>();
            foreach (var item in objDriversModelList)
            {
                var client = new RestClient(_appSettings.Host + _dependencies.UsersUrl + item.UserId);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content;
                    var userData = JsonConvert.DeserializeObject<UserData>(result);
                    lstUsers.AddRange(userData.Data);
                }
            }
            var usersList = lstUsers.GroupBy(x => x.UserId).Select(a => a.First()).ToList();
            return Common.SerializeJsonForIncludedRepo(usersList.Cast<dynamic>().ToList());
        }
    }
}
