using Microsoft.AspNetCore.Mvc;
using PawCareApi.Domain.Common;

namespace PawCareApi.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleResult(OperationResult result)
        {
            if (result.IsSuccess)
            {
                if (result.StatusCode == StatusCodes.Status204NoContent)
                {
                    return NoContent();
                }

                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message
                });
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        protected IActionResult HandleResult<T>(OperationResult<T> result, string? routeName = null, object? routeValues = null)
        {
            if (result.IsSuccess)
            {
                if (result.StatusCode == StatusCodes.Status201Created && !string.IsNullOrWhiteSpace(routeName))
                {
                    return CreatedAtRoute(routeName, routeValues, new
                    {
                        success = true,
                        message = result.Message,
                        data = result.Data
                    });
                }

                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        protected string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }
    }
}

