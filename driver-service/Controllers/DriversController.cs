using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.Common;
using driver_service.Models.DTO;
using driver_service.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using driver_service.Helpers;
using Microsoft.AspNetCore.JsonPatch.Helpers;
using Newtonsoft.Json;
using RestSharp;
using static driver_service.Helpers.ApiExtensions;
using static driver_service.Models.Response;

namespace driver_service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Dependencies _dependencies;
        private readonly AppSettings _appSettings;

        public DriversController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings, IOptions<Dependencies> dependencies)
        {
            _unitOfWork = unitOfWork;
            _dependencies = dependencies.Value;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public ActionResult Get([FromQuery] Pagination pagination, string include)
        {
            try
            {
                var response = new GetResponse<DriversReadDto>();
                var driversReadDtOs = new List<DriversReadDto>();

                var drivers = _unitOfWork.DriverRepository.Get(pagination, null, x => x.OrderBy(x => x.DriverId), d => d.DriverVehicle);
                if (drivers.Count > 0)
                {

                    foreach (var driver in drivers)
                    {
                        var driversReadDto = new DriversReadDto
                        {
                            DriverId = Obfuscation.Encode(driver.DriverId),
                            InvitationId = Obfuscation.Encode(driver.InvitationId),
                            UserId = Obfuscation.Encode(driver.UserId),
                            VehicleId = driver.DriverVehicle.Select(x => Obfuscation.Encode(Convert.ToInt32(x.VehicleId))).ToList()
                        };
                        driversReadDtOs.Add(driversReadDto);
                    }

                    dynamic includeData = new JObject();

                    if (!string.IsNullOrEmpty(include))
                    {
                        var includeArr = include.Split(',');
                        if (includeArr.Length > 0)
                        {
                            foreach (var item in includeArr)
                            {
                                switch (item.ToLower())
                                {
                                    case "user":
                                    case "users":
                                        includeData.users = GetUsers(driversReadDtOs, _appSettings.Host + _dependencies.UsersUrl);
                                        break;
                                    case "vehicle":
                                    case "vehicles":
                                        {
                                            var driversVehicleDto = new DriversVehicleDto { VehicleId = new List<int?>() };
                                            foreach (var id in drivers.SelectMany(driver => driver.DriverVehicle))
                                            {
                                                driversVehicleDto.VehicleId.Add(id.VehicleId);
                                            }
                                            includeData.vehicle = GetVehicles(driversVehicleDto.VehicleId, _appSettings.Host + _dependencies.VehiclesUrl);
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    if (((JContainer)includeData).Count == 0)
                        includeData = null;

                    response.Data = driversReadDtOs;
                    response.Status = true;
                    response.Code = StatusCodes.Status200OK;
                    response.Message = CommonMessage.DriverRetrieved;
                    response.Pagination = pagination;
                    response.Included = includeData;
                    return StatusCode(response.Code, response);
                }
                else
                    return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ErrorResponse(CommonMessage.DriverNotFound, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult GetById(string id)
        {
            try
            {
                var response = new GetResponseById<DriversReadDto>();
                var driver = _unitOfWork.DriverRepository.GetById(a => a.DriverId == Obfuscation.Decode(id), x => x.OrderBy(x => x.DriverId), d => d.DriverVehicle);
                if (driver != null)
                {
                    var driversReadDto = new DriversReadDto
                    {
                        DriverId = Obfuscation.Encode(driver.DriverId),
                        InvitationId = Obfuscation.Encode(driver.InvitationId),
                        UserId = Obfuscation.Encode(driver.UserId),
                        VehicleId = driver.DriverVehicle.Select(x => Obfuscation.Encode(Convert.ToInt32(x.VehicleId))).ToList()
                    };
                    response.Data = driversReadDto;
                    response.Status = true;
                    response.Code = StatusCodes.Status200OK;
                    response.Message = CommonMessage.DriverRetrieved;
                    return StatusCode(response.Code, response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ErrorResponse(CommonMessage.DriverNotFound, 404));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }

        [HttpPost]
        public ActionResult Post(DriversDto driversDto)
        {
            var response = new GetResponse<Driver>();
            //var tokenResponse = new TokenResponse();
            try
            {
                if (driversDto == null)
                    throw new ArgumentNullException(CommonMessage.InvalidData);
                if (VerifyToken(driversDto))
                {
                    var driver = new Driver
                    {
                        UserId = Obfuscation.Decode(driversDto.UserId),
                        InvitationId = Obfuscation.Decode(driversDto.InvitationId),
                        Name = driversDto.Name,
                        AvatarUrl = driversDto.AvatarUrl,
                        VerificationToken = driversDto.VerificationToken
                    };
                    //dynamic includeData = new JObject();
                    if (CheckExistence(driversDto))
                    {
                        var driverVehicle = new DriverVehicle();
                        driverVehicle.VehicleId = Obfuscation.Decode(driversDto.VehicleId);
                        var list = _unitOfWork.DriverRepository.Get(null, x => x.UserId == Obfuscation.Decode(driversDto.UserId), null, d => d.DriverVehicle);

                        if (list.FirstOrDefault()!.DriverVehicle.Any(x => x.VehicleId == Obfuscation.Decode(driversDto.VehicleId)))
                        {
                            response.Message = CommonMessage.DriverExist;
                        }
                        else
                        {
                            driverVehicle.DriverId = list.FirstOrDefault()!.DriverId;
                            driverVehicle.VehicleId = Obfuscation.Decode(driversDto.VehicleId);
                            _unitOfWork.DriverVehicleRepository.Post(driverVehicle);
                            _unitOfWork.Save();
                            response.Message = CommonMessage.VehicleInsert;

                        }
                    }
                    else
                    {
                        _unitOfWork.DriverRepository.Post(driver);
                        _unitOfWork.Save();
                        var driverVehicle = new DriverVehicle
                        {
                            DriverId = driver.DriverId,
                            VehicleId = Obfuscation.Decode(driversDto.VehicleId)
                        };
                        _unitOfWork.DriverVehicleRepository.Post(driverVehicle);
                        _unitOfWork.Save();
                        response.Message = CommonMessage.DriverInsert;
                    }
                    response.Status = true;
                    response.Code = StatusCodes.Status201Created;
                    return StatusCode(response.Code,response);
                }
                else
                {
                    throw new SecurityTokenExpiredException(CommonMessage.TokenExpired);
                }
            }
            catch (Exception)
            {
                throw;
            }
            //return StatusCode(StatusCodes.Status201Created, ReturnResponse.SuccessResponse(CommonMessage.DriverInsert, true));
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                var driver = _unitOfWork.DriverRepository.GetById(x => x.DriverId == Obfuscation.Decode(id), null, x => x.DriverVehicle);
                if (driver != null)
                {
                    _unitOfWork.DriverRepository.Remove(driver);
                    _unitOfWork.Save();
                    return StatusCode(StatusCodes.Status200OK, ReturnResponse.SuccessResponse(CommonMessage.DriverDelete, false));
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ErrorResponse(CommonMessage.DriverNotFound, 404));

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }


        private bool CheckExistence(DriversDto driversDto)
        {
            var list = _unitOfWork.DriverRepository.Get(null, x => x.UserId == Obfuscation.Decode(driversDto.UserId), null, null);
            if (list.Count == 0)
                return false;
            else
                return true;
        }

        private bool VerifyToken(DriversDto driversDto)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (string.IsNullOrEmpty(driversDto.VerificationToken) || !tokenHandler.CanReadToken(driversDto.VerificationToken))
                    throw new ArgumentNullException(CommonMessage.InvalidData);

                var tokenData = (JwtSecurityToken)tokenHandler.ReadToken(driversDto.VerificationToken);


                if (tokenData.ValidTo > DateTime.UtcNow &&
                       tokenData.Subject == driversDto.PhoneNumber &&
                       tokenData.Issuer == _appSettings.ValidIssuer &&
                       tokenData.Audiences.FirstOrDefault() == _appSettings.ValidAudience)
                    return true;
                else
                    return false;
                //throw new SecurityTokenExpiredException(CommonMessage.TokenExpired);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
