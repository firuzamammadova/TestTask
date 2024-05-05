using Microsoft.AspNetCore.Mvc;
using Services.Models;
using Services.Services;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RectangleController : ControllerBase
    {


        private readonly IRectangleService _rectangleService;

        public RectangleController(IRectangleService rectangleService)
        {
            _rectangleService = rectangleService;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("intersecting_rectangles")]
        public async Task<IActionResult> GetIntersectingRectangles([FromQuery] Segment request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _rectangleService.GetAllAsync(request);

                if (response?.ToList() == null)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }


                return Ok(response);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);

            }


        }
    }
}
