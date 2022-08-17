﻿using AutoMapper;
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
        public IActionResult GetByID(string id)
        {
            dynamic response;
            dynamic obj;
            try
            {
                response = _driverRepository.GetById(Obfuscation.Decode(id));

                if (response == null)
                    throw new ArgumentNullException();

                obj = _mapper.Map<DriversReadDto>(response);
                obj.DriverId = Obfuscation.Encode(Convert.ToInt32(obj.DriverId));
                obj.UserId = Obfuscation.Encode(Convert.ToInt32(obj.UserId));
                obj.InstitutionId = Obfuscation.Encode(Convert.ToInt32(obj.InstitutionId));
            }
            catch (ArgumentNullException)
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
        public IActionResult PostDriver(DriversDto driverDto)
        {
            PostDriverResponse response = new PostDriverResponse();
            try
            {
                if (driverDto == null)
                    throw new ArgumentNullException(CommonMessage.InvalidData);

                driverDto.UserId = Obfuscation.Decode(driverDto.User_Id);
                driverDto.InstitutionId = Obfuscation.Decode(driverDto.Institution_Id);

                Driver driver = _mapper.Map<Driver>(driverDto);

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

    }
}