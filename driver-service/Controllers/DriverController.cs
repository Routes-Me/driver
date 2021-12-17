using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RoutesSecurity;
using DriverService.Abstraction;
using DriverService.Models;
using DriverService.Models.DBModels;
using DriverService.Models.Common;
using DriverService.Models.ResponseModel;
using System.Linq;
using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;
using AutoMapper;

namespace DriverService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository _driversRepository;
        private readonly DriversServiceContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        public DriverController(IDriverRepository driversRepository, DriversServiceContext context, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _driversRepository = driversRepository;
            _context = context;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("drivers/{id?}")]
        public IActionResult Get(string id, string Include, [FromQuery] Pagination pageInfo)
        {
            dynamic response = _driversRepository.GetDriver(id, pageInfo, Include);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut]
        [Route("drivers")]
        public IActionResult Put(DriversDto driversDto)
        {
            dynamic response = _driversRepository.UpdateDriver(driversDto);
            return StatusCode(response.statusCode, response);
        }

        [HttpPost]
        [Route("drivers")]
        public async Task<IActionResult> PostDriver(DriversDto driversDto)
        {
            PostDriverResponse response = new PostDriverResponse();
            try
            {
                Driver driver = _driversRepository.PostDriver(driversDto);
                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();
                response.Id = Obfuscation.Encode(driver.DriverId);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
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
                Driver driver = _driversRepository.DeleteDriver(driverId);
                _context.Drivers.Remove(driver);
                _context.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("driver/device")]
        public IActionResult PostDevice(Device Device)
        {
            PostDriverResponse response = new PostDriverResponse();
            try
            {
                _driversRepository.PostDevicesAsync(Device);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            response.Message = CommonMessage.InsertDevice;
            return StatusCode(StatusCodes.Status201Created, response);

        }

        [HttpPut]
        [Route("driver/fcmToken")]
        public IActionResult UpdateToken(DeviceDto dt)
        {
            try
            {
                Device dev = _driversRepository.GetDeviceByToken(dt.oldToken);
                if (dev == null)
                {
                    return NotFound();
                }
                _mapper.Map(dt, dev);
                _context.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK, CommonMessage.FCMTokenUpdated);
        }

        [HttpDelete]
        [Route("driver/fcmToken")]
        public IActionResult DeleteDevice(DeviceDto dt)
        {
            PostDriverResponse response = new PostDriverResponse();
            try
            {
                _driversRepository.DeleteDevice(dt.fcmToken);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            response.Message = CommonMessage.DeviceDeleted;
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        [Route("driver/number")]
        public IActionResult CheckNumber(Phone phone)
        {
            if (_context.Drivers.Where(p => p.Phone.Number == phone.Number && p.Phone.IsActive == true).FirstOrDefault() != null)
                return StatusCode(StatusCodes.Status200OK);

            return StatusCode(StatusCodes.Status404NotFound);
        }

    }
}