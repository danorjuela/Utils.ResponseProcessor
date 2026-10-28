using Danorjuela.Utils.ResponseProcessor.DTOs;
using Danorjuela.Utils.ResponseProcessor.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Security.Claims;

namespace Danorjuela.Utils.ResponseProcessor.Services
{
    public class ResponseService : IResponseService
    {

        public IActionResult ProcessException(Exception ex)
        {
            var g = Guid.NewGuid().ToString();

            switch (ex)
            {
                // Excepciones propias del paquete
                case INotFoundException:
                    return GenerateReponse(ex, StatusCodes.Status404NotFound, g);
                case IBadRequestException:
                    return GenerateReponse(ex, StatusCodes.Status400BadRequest, g);
                case INoDataException:
                    return GenerateReponse(ex, StatusCodes.Status204NoContent, g);
                case INotAutorizationException:
                    return GenerateReponse(ex, StatusCodes.Status401Unauthorized, g);
                case IForbiddenException:
                    return GenerateReponse(ex, StatusCodes.Status403Forbidden, g);
                case IConflicException:
                    return GenerateReponse(ex, StatusCodes.Status409Conflict, g);
                case IGoneException:
                    return GenerateReponse(ex, StatusCodes.Status410Gone, g);
                case IPreconditionFailedException:
                    return GenerateReponse(ex, StatusCodes.Status412PreconditionFailed, g);
                case IBusinessException:
                    return GenerateReponse(ex, StatusCodes.Status418ImATeapot, g);
                case IUnprocessableEntityException:
                    return GenerateReponse(ex, StatusCodes.Status422UnprocessableEntity, g);
                case ITooManyRequestsException:
                    return GenerateReponse(ex, StatusCodes.Status429TooManyRequests, g);
                case IServiceUnavailableException:
                    return GenerateReponse(ex, StatusCodes.Status503ServiceUnavailable, g);

                // Excepciones estandar de .NET
                case InvalidOperationException:
                case ArgumentException:
                    return GenerateReponse(ex, StatusCodes.Status400BadRequest, g);
                case UnauthorizedAccessException:
                    return GenerateReponse(ex, StatusCodes.Status403Forbidden, g);
                case KeyNotFoundException:
                    return GenerateReponse(ex, StatusCodes.Status404NotFound, g);
                case NotImplementedException:
                    return GenerateReponse(ex, StatusCodes.Status501NotImplemented, g);
                case TimeoutException:
                    return GenerateReponse(ex, StatusCodes.Status504GatewayTimeout, g);
                default:
                    return GenerateReponse(ex, StatusCodes.Status500InternalServerError, g);
            }
        }

        private ObjectResult GenerateReponse(Exception ex, int statusCode, string guid = "")
        {
            var tipoEx = ex.GetType();
            var isGeneric = tipoEx.IsGenericType;
            if (isGeneric)
            {
                var typeGeneric = tipoEx.GetGenericArguments().First();
                var typeDataProperty = tipoEx.GetProperty("ObjectData");
                var typeDataPropertyValue = typeDataProperty.GetValue(ex);
                var typeResponseDTO = typeof(ResponseDTO<>).MakeGenericType(typeGeneric);
                var reponse = Activator.CreateInstance(typeResponseDTO);
                PropertyInfo MessageProperty = reponse.GetType().GetProperty("Message");
                MessageProperty.SetValue(reponse, ex.Message);
                PropertyInfo DataProperty = reponse.GetType().GetProperty("Data");
                DataProperty.SetValue(reponse, typeDataPropertyValue);
                PropertyInfo guidProperty = reponse.GetType().GetProperty("Guid");
                guidProperty.SetValue(reponse, guid);
                return new ObjectResult(reponse)
                {
                    StatusCode = statusCode
                };
            }
            else
            {
                return new ObjectResult(new ResponseDTO() { Message = ex.Message, IsSuccess = false, Guid = guid })
                {
                    StatusCode = statusCode

                };
            }
        }

        public IActionResult ProcessException(Exception ex, int? code = null)
        {
            if (code != null)
            {
                return GenerateReponse(ex, code.Value, Guid.NewGuid().ToString());
            }
            else
            {
                return ProcessException(ex);
            }
        }

        public IActionResult ProcessResponse<T>(T data, int? code = 200)
        {
            var guid = Guid.NewGuid().ToString();
            return new ObjectResult(new ResponseDTO<T>() { Message = "", Data = data, Guid = guid })
            {
                StatusCode = code
            };
        }

    }
}
