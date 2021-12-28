using AutoMapper;
using driver_service.Models.ResponseModel;
using DriverService.Abstraction;
using DriverService.Models;
using DriverService.Models.Common;
using DriverService.Models.DBModels;
using DriverService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoutesSecurity;
using System;
using System.Collections.Generic;

namespace DriverService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    public class DriverController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private IDriverRepository _repo;
        public DriverController(IOptions<AppSettings> appSettings, IMapper mapper, IDriverRepository repository)
        {
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _repo = repository;
        }

        [HttpGet]
        [Route("drivers")]
        public IActionResult GetAll()
        {
            dynamic response;
            dynamic obj;
            try
            {
                response = _repo.GetAll();
                obj = (_mapper.Map<List<DriverReadDto>>(response));

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
                response = _repo.GetById(Obfuscation.Decode(Id));

                obj = _mapper.Map<DriverReadDto>(response);
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

                _repo.Insert(driver);
                _repo.Save();

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
                _repo.Delete(Obfuscation.Decode(driverId));
                _repo.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

    }
}