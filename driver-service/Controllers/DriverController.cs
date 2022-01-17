using AutoMapper;
using driver_service.Abstraction;
using driver_service.Functions;
using driver_service.Models;
using driver_service.Models.Common;
using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UAParser;

namespace driver_service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    public class DriverController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IDriverRepository _driverRepository;
        private readonly Dependencies _dependencies;
        private readonly IDriverIncludedRepository _driverIncludedRepository;

        public DriverController(IOptions<AppSettings> appSettings, IMapper mapper, IDriverRepository repository, IOptions<Dependencies> dependencies, IDriverIncludedRepository driverIncludedRepository)
        {
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _driverRepository = repository;
            _dependencies = dependencies.Value;
            _driverIncludedRepository = driverIncludedRepository;
        }

        /// <summary>
        /// Get All Driver
        /// </summary>
        /// <remarks>
        /// Return All Drivers
        /// </remarks>
        /// <response code="200">Return list of Drivers</response>
        [HttpGet]
        [Route("drivers")]
        public IActionResult GetAll()
        {
            dynamic response;
            dynamic obj;
            try
            {
                response = _driverRepository.GetAll();
                obj = _mapper.Map<List<DriversReadDto>>(response);

                foreach (var item in obj)
                {
                    item.DriverId = Obfuscation.Encode(Convert.ToInt32(item.DriverId));
                    item.UserId = Obfuscation.Encode(Convert.ToInt32(item.UserId));
                    item.InstitutionId = Obfuscation.Encode(Convert.ToInt32(item.InstitutionId));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK, obj);
        }

        /// <summary>
        /// Get specific Driver
        /// </summary>
        /// <param name="Id"></param>
        /// <remarks>
        /// Returns Specific Driver on the base of driverid
        /// </remarks>
        /// <response code="200">Ok</response>
        /// <response code="404">Driver Not Found</response>

        [HttpGet]
        [Route("drivers/{id?}")]
        public IActionResult GetByID(string Id)
        {
            dynamic response;
            dynamic obj;
            try
            {
                response = _driverRepository.GetById(Obfuscation.Decode(Id));

                if (response == null)
                    throw new ArgumentNullException();

                obj = _mapper.Map<DriversReadDto>(response);
                obj.DriverId = Obfuscation.Encode(Convert.ToInt32(obj.DriverId));
                obj.UserId = Obfuscation.Encode(Convert.ToInt32(obj.UserId));
                obj.InstitutionId = Obfuscation.Encode(Convert.ToInt32(obj.InstitutionId));
            }
            catch(ArgumentNullException)
            {
                return StatusCode(StatusCodes.Status404NotFound, CommonMessage.DriverNotFound);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK, obj);
        }

        /// <summary>
        /// Post Driver
        /// </summary>
        /// <remarks>
        /// New Driver will be inserted 
        /// </remarks>
        /// <param name="driverdto"></param>
        /// <response code="201">Driver Register Successfuly</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [Route("drivers")]
        public IActionResult PostDriver(DriversDto driverdto)
        {
            PostDriverResponse response = new PostDriverResponse();
            try
            {
                if (driverdto == null)
                    throw new ArgumentNullException(CommonMessage.InvalidData);

                driverdto.UserId = Obfuscation.Decode(driverdto.User_Id);
                driverdto.InstitutionId = Obfuscation.Decode(driverdto.Institution_Id);

                Driver driver = _mapper.Map<Driver>(driverdto);

                _driverRepository.Insert(driver);
                _driverRepository.Save();

                response.Id = Obfuscation.Encode(driver.DriverId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            response.Message = CommonMessage.DriverInsert;
            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Delete Driver
        /// </summary>
        /// <remarks>
        /// Remove the driver on the base of driverid
        /// </remarks>
        /// <param name="driverId"></param>
        /// <response code="200">OK</response>
        /// <response code="404">Driver Not Found</response>
        [HttpDelete]
        [Route("drivers/{driverId}")]
        public IActionResult Delete(string driverId)
        {
            try
            {
                _driverRepository.Delete(Obfuscation.Decode(driverId));
                _driverRepository.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, CommonMessage.DriverNotFound + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Get Drivers with Include
        /// </summary>
        /// <remarks>
        /// Return All Driver with pagination and also list of identites related to the driver
        /// </remarks>
        /// <param name="Include">Identities {User , Institutes}</param>
        /// <response code="200">Return list of driver</response>
        [HttpGet]
        [Route("drivers/Include")]
        public IActionResult GetInclude(string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _driverIncludedRepository.GetDriver(pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }

        /// <summary>
        /// Check Existance With Phone Number
        /// </summary>
        /// <remarks>
        /// Check wheather phone is registered or not
        /// </remarks>
        /// <param name="number"></param>
        /// <response code="200">Empty Body</response>
        /// <response code="400">Not Found</response>
        [HttpGet]
        [Route("drivers/checknumber/{number}")]
        public IActionResult CheckNumber(string number)
        {
            Common comm = new Common();
            try
            {
                comm.GetAPI(_appSettings.Host + _dependencies.UsersUrl + "number/" + number);
                return Ok();
            }
            catch(Exception)
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Post Device
        /// </summary>
        /// <remarks>
        /// Insert a new Device
        /// </remarks>
        /// <param name="device"></param>
        /// <response code="201">Device Posted</response>
        [HttpPost]
        [Route("drivers/devices")]
        public IActionResult PostDevice(DeviceDto device)
        {
            Common _common = new Common();

            try
            {
                IRestResponse postedUserResponse = _common.PostAPI(_appSettings.Host + _dependencies.UsersUrl + "devices", device);
                DeviceResponse response = JsonConvert.DeserializeObject<DeviceResponse>(postedUserResponse.Content);
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }

            return StatusCode(StatusCodes.Status201Created, new SuccessResponse { message = CommonMessage.InsertDevice });
        }

        /// <summary>
        /// Update Device
        /// </summary>
        /// <remarks>
        /// Update a device by device ID
        /// </remarks>
        /// <param name="device"></param>
        /// <response code="200"></response>
        [HttpPut]
        [Route("drivers/UpdateToken")]
        public IActionResult UpdateDevice(DeviceDto device)
        {
            Common _common = new Common();
            DeviceResponse response = new DeviceResponse();

            try
            {
                IRestResponse postedUserResponse = _common.PutAPI(_appSettings.Host + _dependencies.UsersUrl + "updatedevices", device);
                response = JsonConvert.DeserializeObject<DeviceResponse>(postedUserResponse.Content);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }

            return StatusCode(StatusCodes.Status200OK, new SuccessResponse { message = response.Message });
        }

        /// <summary>
        /// Remove Device
        /// </summary>
        /// <remarks>
        /// Remove a device by device ID
        /// </remarks>
        /// <param name="DeviceId"></param>
        /// <response code="200"></response>
        [HttpDelete]
        [Route("drivers/devices/{DeviceId}")]
        public IActionResult DeleteDevice(string DeviceId)
        {
            Common _common = new Common();

            try
            {
                IRestResponse postedUserResponse = _common.DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + "devices/" + DeviceId);
                DeviceResponse response = JsonConvert.DeserializeObject<DeviceResponse>(postedUserResponse.Content);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }

            return StatusCode(StatusCodes.Status200OK, new SuccessResponse { message = CommonMessage.DeviceDeleted });
        }

        /// <summary>
        /// Verify Number
        /// </summary>
        /// <remarks>
        /// Verify a number by number and unique ID
        /// </remarks>
        /// <param name="number"></param>
        /// <param name="IdentifierId"></param>
        /// <response code="200"></response>
        /// <response code="404"></response>
        [HttpGet]
        [Route("drivers/{number}/{IdentifierId}")]
        public IActionResult VerifyNumber(string number , string IdentifierId)
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var verificationToken = HttpContext.Request.Headers["verificationToken"];

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(verificationToken);

            string sub = token.Claims.First(claim => claim.Type == "sub").Value;
            var exp = Convert.ToInt64(token.Claims.First(claim => claim.Type == "exp").Value);

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(exp);

            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);

            Common _common = new Common();

            try
            {
                if (sub == number && DateTime.Compare(dateTime, DateTime.UtcNow) > 0)
                {
                    _common.GetAPI(_appSettings.Host + _dependencies.UsersUrl + number + '/' + IdentifierId + '/' + c.OS.Family);
                    return Ok();
                }

                return NotFound();
            }
            catch(Exception e)
            {
                return NotFound();
            }
        }

    }
}