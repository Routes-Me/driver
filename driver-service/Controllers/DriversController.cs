using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.DTO;
using driver_service.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using static driver_service.Models.Response;

namespace driver_service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    public class DriversController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public DriversController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Get

        [HttpGet]
        [Route("drivers")]
        public ActionResult Get([FromQuery] Pagination pagination)
        {
            try
            {
                GetResponse<DriversReadDTO> response = new GetResponse<DriversReadDTO>();
                List<DriversReadDTO> driversReadDTOs = new List<DriversReadDTO>();

                var drivers = _unitOfWork.DriverRepository.Get(pagination, null, x => x.OrderBy(x => x.DriverId), d => d.DriverVehicle);
                foreach (var driver in drivers)
                {
                    DriversReadDTO driversReadDTO = new DriversReadDTO
                    {
                        DriverId = Obfuscation.Encode(driver.DriverId),
                        InvitationId = Obfuscation.Encode(driver.InvitationId),
                        UserId = Obfuscation.Encode(driver.UserId),
                        VehicleId = driver.DriverVehicle.Select(x => Obfuscation.Encode(Convert.ToInt32(x.VehicleId))).ToList()
                    };
                    driversReadDTOs.Add(driversReadDTO);
                }
                response.Data = driversReadDTOs;
                response.Status = true;
                response.Code = StatusCodes.Status200OK;
                response.Message = CommonMessage.DriverRetrived;
                response.Pagination = pagination;
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }

        #endregion

        #region GetById
        [HttpGet]
        [Route("drivers/{id}")]
        public ActionResult GetById(string id)
        {
            try
            {
                var response = new GetResponseById<DriversReadDTO>();
                var driver = _unitOfWork.DriverRepository.GetById(a => a.DriverId == Obfuscation.Decode(id), x => x.OrderBy(x => x.DriverId), d => d.DriverVehicle);

                DriversReadDTO driversReadDTO = new DriversReadDTO
                {
                    DriverId = Obfuscation.Encode(driver.DriverId),
                    InvitationId = Obfuscation.Encode(driver.InvitationId),
                    UserId = Obfuscation.Encode(driver.UserId),
                    VehicleId = driver.DriverVehicle.Select(x => Obfuscation.Encode(Convert.ToInt32(x.VehicleId))).ToList()
                };

                response.Data = driversReadDTO;
                response.Status = true;
                response.Code = StatusCodes.Status200OK;
                response.Message = CommonMessage.DriverRetrived;
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }
        #endregion

        #region Post
        [HttpPost]
        [Route("drivers")]
        public ActionResult Post(DriversDTO driversDTO)
        {
            GetResponse<Driver> response = new GetResponse<Driver>();
            try
            {
                if (driversDTO == null)
                    throw new ArgumentNullException(CommonMessage.InvalidData);
                Driver driver = new Driver
                {
                    UserId = Obfuscation.Decode(driversDTO.UserId),
                    InvitationId = Obfuscation.Decode(driversDTO.InvitationId),
                    Name = driversDTO.Name,
                    AvatarUrl = driversDTO.AvatarUrl
                };
                if (CheckExistance(driversDTO))
                {
                    DriverVehicle driverVehicle = new DriverVehicle();
                    var list = _unitOfWork.DriverRepository.Get(null, x => x.UserId == Obfuscation.Decode(driversDTO.UserId), null, d => d.DriverVehicle);

                    if (list.FirstOrDefault().DriverVehicle.Any(x => x.VehicleId == Obfuscation.Decode(driversDTO.VehicleId)))
                    {
                        response.Message = CommonMessage.DriverExist;
                    }
                    else
                    {
                        driverVehicle.DriverId = list.FirstOrDefault().DriverId;
                        driverVehicle.VehicleId = Obfuscation.Decode(driversDTO.VehicleId);
                        _unitOfWork.DriverVehicleRepository.Post(driverVehicle);
                        _unitOfWork.Save();
                        response.Message = CommonMessage.VehicleInsert;

                    }
                }
                else
                {
                    _unitOfWork.DriverRepository.Post(driver);
                    _unitOfWork.Save();
                    DriverVehicle driverVehicle = new DriverVehicle
                    {
                        DriverId = driver.DriverId,
                        VehicleId = Obfuscation.Decode(driversDTO.VehicleId)
                    };
                    _unitOfWork.DriverVehicleRepository.Post(driverVehicle);
                    _unitOfWork.Save();
                    response.Message = CommonMessage.DriverInsert;
                }
                response.Status = true;
                response.Code = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status201Created, ReturnResponse.SuccessResponse(CommonMessage.DriverInsert, true));
            //return StatusCode(StatusCodes.Status201Created, response);
        }
        #endregion


        //Delete Functionality update pending
        #region Delete

        [HttpDelete]
        [Route("drivers/{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                _unitOfWork.DriverRepository.Delete(Obfuscation.Decode(id));
                _unitOfWork.Save();
                return StatusCode(StatusCodes.Status200OK, ReturnResponse.SuccessResponse(CommonMessage.DriverDelete, false));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ReturnResponse.ExceptionResponse(ex));
            }
        }

        #endregion


        private bool CheckExistance(DriversDTO driversDTO)
        {
            var list = _unitOfWork.DriverRepository.Get(null, x => x.UserId == Obfuscation.Decode(driversDTO.UserId), null, null);
            if (list.Count > 0)
                return true;
            else
                return false;
        }
    }
}
