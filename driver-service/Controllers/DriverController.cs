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
        public DriverController(IDriverRepository driversRepository, DriversServiceContext context, IOptions<AppSettings> appSettings)
        {
            _driversRepository = driversRepository;
            _context = context;
            _appSettings = appSettings.Value;
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
    }
}