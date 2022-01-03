using AutoMapper;
using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.Common;
using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoutesSecurity;
using System;
using System.Collections.Generic;

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

        [HttpGet]
        [Route("drivers/{id?}")]
        public IActionResult GetByID(string Id)
        {
            dynamic response;
            dynamic obj;
            try
            {
                response = _driverRepository.GetById(Obfuscation.Decode(Id));

                obj = _mapper.Map<DriversReadDto>(response);
                obj.DriverId = Obfuscation.Encode(Convert.ToInt32(obj.DriverId));
                obj.UserId = Obfuscation.Encode(Convert.ToInt32(obj.UserId));
                obj.InstitutionId = Obfuscation.Encode(Convert.ToInt32(obj.InstitutionId));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK, obj);
        }

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

        [HttpDelete]
        [Route("drivers/{driverId}")]
        public IActionResult delete(string driverId)
        {
            try
            {
                _driverRepository.Delete(Obfuscation.Decode(driverId));
                _driverRepository.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        [Route("drivers/Include")]
        public IActionResult GetInclude(string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _driverIncludedRepository.GetDriver(pageInfo, Include);
            return StatusCode((int)response.statusCode, response);
        }



    }
}