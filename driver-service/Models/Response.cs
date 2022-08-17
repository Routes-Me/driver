﻿using driver_service.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace driver_service.Models
{
    public class Response
    {

        public int Code { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public string Id { get; set; }
        public static class ReturnResponse
        {
            public static dynamic ErrorResponse(string message, int statusCode)
            {
                var response = new Response
                {
                    Status = false,
                    Message = message,
                    Code = statusCode
                };
                return response;
            }
            public static dynamic ExceptionResponse(Exception ex)
            {
                var response = new Response
                {
                    Status = false,
                    Message = CommonMessage.ExceptionMessage + ex.Message,
                    Code = StatusCodes.Status500InternalServerError
                };
                return response;
            }
            public static dynamic SuccessResponse(string message, bool isCreated, int id = 0)
            {
                var response = new Response
                {
                    Status = true,
                    Message = message
                };
                if (isCreated)
                {
                    response.Code = StatusCodes.Status201Created;
                    response.Id = Obfuscation.Encode(id);
                }
                else
                    response.Code = StatusCodes.Status200OK;
                return response;
            }
        }
        public class GetResponse<T> : Response where T : class
        {
            public Pagination Pagination { get; set; }
            public List<T> Data { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public JObject Included { get; set; }
        }
        public class GetResponseById<T> : Response where T : class
        {
            public T Data { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public JObject Included { get; set; }
        }
        public class GetResponseApi<T> : Response where T : class
        {
            public List<T> Data { get; set; }
        }
    }
}
