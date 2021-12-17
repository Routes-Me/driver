using driver_service.Models.DbModels;
using DriverService.Abstraction;
using DriverService.Models;
using DriverService.Models.Common;
using DriverService.Models.DBModels;
using DriverService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriverService.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly DriversServiceContext _context;
        private readonly AppSettings _appSettings;
        public DriverRepository(IOptions<AppSettings> appSettings, DriversServiceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public dynamic DeleteDriver(string DriverId)
        {
            if (string.IsNullOrEmpty(DriverId))
                throw new ArgumentNullException(CommonMessage.DriverIdRequired);

            int DriverIdDecrypted = Obfuscation.Decode(DriverId);
            Driver Driver = _context.Drivers.Include(x => x.Phone).Where(x => x.DriverId == DriverIdDecrypted).FirstOrDefault();
            if (Driver == null)
                throw new ArgumentException(CommonMessage.DriverNotFound);

            return Driver;
        }

        public dynamic UpdateDriver(DriversDto DriversDto)
        {
            try
            {
                var DriverIdDecrypted = Obfuscation.Decode(DriversDto.DriverId);
                if (DriversDto == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.InvalidData, StatusCodes.Status400BadRequest);

                var Driver = _context.Drivers.Include(x => x.Phone).Where(x => x.DriverId == DriverIdDecrypted).FirstOrDefault();
                if (Driver == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.DriverNotFound, StatusCodes.Status404NotFound);

                if (!string.IsNullOrEmpty(DriversDto.phone.Number))
                {
                    var DriverPhone = Driver.Phone;
                    if (DriverPhone == null)
                    {
                        var newPhone = new Phone()
                        {
                            Number = DriversDto.phone.Number,
                            DriverId = DriverIdDecrypted
                        };
                        _context.Phone.Add(newPhone);
                    }
                    else
                    {
                        DriverPhone.Number = DriversDto.phone.Number;
                        _context.Phone.Update(DriverPhone);
                    }
                }

                Driver.Name = DriversDto.Name;
                Driver.Email = DriversDto.Email;
                _context.Drivers.Update(Driver);
                _context.SaveChanges();

                return ReturnResponse.SuccessResponse(CommonMessage.DriverUpdate, false);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetDriver(string DriverId, Pagination pageInfo, string includeType)
        {
            try
            {
                int totalCount = 0;
                DriversGetResponse response = new DriversGetResponse();
                List<DriversDto> DriversModelList = new List<DriversDto>();
                if (string.IsNullOrEmpty(DriverId))
                {
                    var DriversData = _context.Drivers.Include(x => x.Phone).AsEnumerable().ToList()
                                                      .GroupBy(p => p.DriverId).Select(g => g.First()).ToList()
                                                      .OrderBy(a => a.DriverId)
                                                      .Skip((pageInfo.offset - 1) * pageInfo.limit)
                                                      .Take(pageInfo.limit).ToList();
                    foreach (var _driver in DriversData)
                    {
                        DriversDto DriversModel = new DriversDto();
                        DriversModel.DriverId = Obfuscation.Encode(_driver.DriverId);
                        DriversModel.Name = _driver.Name;
                        DriversModel.Email = _driver.Email;
                        DriversModel.AvatarUrl = _driver.AvatarUrl;
                        DriversModel.phone = new DriversDto.Phones { Number = _driver.Phone.Number, VerificationToken = _driver.Phone.VerificationToken, IsActive = _driver.Phone.IsActive };
                        DriversModel.InvitationToken = _driver.InvitationToken;
                        DriversModelList.Add(DriversModel);
                    }
                    totalCount = _context.Drivers.ToList().Count();
                }
                else
                {
                    var DriverIdDecrypted = Obfuscation.Decode(DriverId);
                    var DriversData = _context.Drivers.Include("Phone").Where(x => x.DriverId == DriverIdDecrypted)
                        .AsEnumerable().ToList().GroupBy(p => p.DriverId).Select(g => g.First()).ToList().OrderBy(a => a.DriverId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                    foreach (var _driver in DriversData)
                    {
                        DriversDto DriversModel = new DriversDto();
                        DriversModel.DriverId = Obfuscation.Encode(_driver.DriverId);
                        DriversModel.Name = _driver.Name;
                        DriversModel.Email = _driver.Email;
                        DriversModel.AvatarUrl = _driver.AvatarUrl;
                        DriversModel.phone = new DriversDto.Phones { Number = _driver.Phone.Number, VerificationToken = _driver.Phone.VerificationToken, IsActive = _driver.Phone.IsActive };
                        DriversModel.InvitationToken = _driver.InvitationToken;
                        DriversModelList.Add(DriversModel);
                    }
                    totalCount = _context.Drivers.Where(x => x.DriverId == DriverIdDecrypted).ToList().Count();
                }

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                dynamic includeData = new JObject();

                if (((JContainer)includeData).Count == 0)
                    includeData = null;

                response.message = CommonMessage.DriverRetrived;
                response.statusCode = StatusCodes.Status200OK;
                response.status = true;
                response.pagination = page;
                response.data = DriversModelList;
                response.included = includeData;

                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic PostDriver(DriversDto DriversDto)
        {
            if (DriversDto == null)
                throw new ArgumentNullException(CommonMessage.InvalidData);

            if (!string.IsNullOrEmpty(DriversDto.phone.Number) && _context.Phone.Where(p => p.Number == DriversDto.phone.Number && p.IsActive == true).FirstOrDefault() != null)
                throw new ArgumentException(CommonMessage.PhoneAlreadyExists);

            return new Driver
            {
                Name = DriversDto.Name,
                Email = DriversDto.Email,
                AvatarUrl = DriversDto.AvatarUrl,
                Phone = new Phone()
                {
                    Number = DriversDto.phone.Number,
                    VerificationToken = DriversDto.phone.VerificationToken,
                    IsActive = true
                },
                InvitationToken = DriversDto.InvitationToken
            };
        }

        public void DeleteDevice(string fcmToken)
        {
            if (string.IsNullOrEmpty(fcmToken))
                throw new ArgumentException(CommonMessage.InvalidData);

            Device dev = _context.Devices.Where(x => x.fcmToken == fcmToken).FirstOrDefault();
            _context.Remove(dev);
            _context.SaveChanges();
        }

        public async Task PostDevicesAsync(Device Device)
        {
            if (Device == null)
                throw new ArgumentNullException(CommonMessage.InvalidData);

            _context.Devices.Add(Device);
            _context.SaveChanges();
        }

        public Device GetDeviceByToken(string token)
        {
            return _context.Devices.Where(x => x.fcmToken == token).ToList().FirstOrDefault();
        }

    }
}