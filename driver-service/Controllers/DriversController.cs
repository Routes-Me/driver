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

        [HttpGet]
        [Route("drivers")]
        public ActionResult Get([FromQuery] Pagination pagination)
        {
            try
            {
                GetResponse<DriversDTO> response = new GetResponse<DriversDTO>();
                List<DriversDTO> driversDTOs = new List<DriversDTO>();

                var drivers = _unitOfWork.DriverRepository.Get(pagination, null, x => x.OrderBy(x => x.DriverId), d => d.DriverVehicles);
                foreach (var driver in drivers)
                {
                    DriversDTO driversDTO = new DriversDTO();

                    driversDTO.DriverId = Obfuscation.Encode(driver.DriverId);
                    driversDTO.InvitationId = Obfuscation.Encode(driver.InvitationId);
                    driversDTO.UserId = Obfuscation.Encode(driver.UserId);

                    if (driver.DriverVehicles.Count > 0)
                    {
                        foreach (var driverVehicle in driver.DriverVehicles)
                        {
                            driversDTO.VehicleId.Add(Obfuscation.Encode(Convert.ToInt32(driverVehicle.VehicleId)));

                        }
                    }
                    driversDTOs.Add(driversDTO);
                }
                response.Data = driversDTOs;
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
    }
}
