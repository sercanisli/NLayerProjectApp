using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayerProject.Core.DTOs;

namespace NLayerProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        //Not a endpoint
        [NonAction]
        public IActionResult CreateActionResult<T>(CustomResponseDto<T> response)
        {   //Status Code 204
            if(response.StatusCode==204)
            {
                return new ObjectResult(null)
                {
                    StatusCode = response.StatusCode
                };
            }

            //Status Code 404
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
