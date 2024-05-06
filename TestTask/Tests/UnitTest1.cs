using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Infrastructure;
using Services.Models;
using Services.Services;
using System.Data.SqlClient;
using TestTask.Controllers;

namespace Tests
{
    public class Tests
    {
        private Mock<IRectangleService> _rectangleService;
        private RectangleController _controller;

        private IEnumerable<Rectangle> _rectangleList = new List<Rectangle>
        {
            new Rectangle{X1 = 1, X2 = 1, Y1 = 2,Y2=2},
            new Rectangle{X1 = 2, X2 = 1, Y1 = 4,Y2=3},
            new Rectangle{X1 = 1, X2 = 2, Y1 = 3,Y2=4},

        };
        [SetUp]
        public void Setup()
        {
            _rectangleService = new Mock<IRectangleService>();
            _controller = new RectangleController(_rectangleService.Object);
        }

        [Test]
        public async Task get_intersecting_rectangles_returns_ok()
        {
            //ARRANGE
            var segment = new Segment { X1 = 0, Y1 = 0, X2 = 2, Y2 = 2 };
            _rectangleService.Setup(c => c.GetIntersectingRectangles(It.IsAny<Segment>())).ReturnsAsync(_rectangleList);

            //ACT
            var controllerResponse = await _controller.GetIntersectingRectangles(segment);

            // ASSERT
            Assert.IsInstanceOf(typeof(OkObjectResult), controllerResponse);

            List<Rectangle> response = (controllerResponse as OkObjectResult).Value as List<Rectangle>;
            Assert.That(response, Is.EqualTo(_rectangleList));

            Assert.That(response.Count(), Is.EqualTo(_rectangleList.Count()));

        }
        [Test]
        public async Task get_intersecting_rectangles_on_request_null_returns_badrequest()
        {
            //ARRANGE

            //ACT
            var controllerResponse = await _controller.GetIntersectingRectangles(null);

            // ASSERT
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), controllerResponse);


        }
        [Test]
        public async Task get_intersecting_rectangles_on_model_error_returns_badrequest()
        {
            //ARRANGE
            _controller.ModelState.AddModelError("Something", "Error");
            var segment = new Segment { X1 = 0, Y1 = 0, X2 = 2, Y2 = 2 };

            //ACT
            var controllerResponse = await _controller.GetIntersectingRectangles(segment);

            // ASSERT
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), controllerResponse);


        }
        [Test]
        public async Task get_intersecting_rectangles_on_server_error_returns_servererror()
        {
            //ARRANGE
            _rectangleService
                    .Setup(u => u.GetIntersectingRectangles(It.IsAny<Segment>()))
                    .ThrowsAsync(new Exception());
            var segment = new Segment { X1 = 0, Y1 = 0, X2 = 2, Y2 = 2 };

            //ACT
            var controllerResponse = await _controller.GetIntersectingRectangles(segment);

            // ASSERT
            Assert.IsInstanceOf(typeof(StatusCodeResult), controllerResponse);
            Assert.That(((StatusCodeResult)controllerResponse).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));



        }
        [Test]
        public async Task get_intersecting_rectangles_response_null_returns_error()
        {
            //ARRANGE
            _rectangleService
                    .Setup(u => u.GetIntersectingRectangles(It.IsAny<Segment>()))
                    .ReturnsAsync(() => null);
            var segment = new Segment { X1 = 0, Y1 = 0, X2 = 2, Y2 = 2 };

            //ACT
            var controllerResponse = await _controller.GetIntersectingRectangles(segment);

            // ASSERT
            Assert.IsInstanceOf(typeof(StatusCodeResult), controllerResponse);
            Assert.That(((StatusCodeResult)controllerResponse).StatusCode, Is.EqualTo(StatusCodes.Status200OK));



        }


    }
}