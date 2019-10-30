using System;
using Xunit;
using YAHCMS.BlogService.Controllers;

namespace YAHCMS.BlogService.Tests
{
    public class UnitTest1
    {
        WeatherForecastController controller = new WeatherForecastController();

        [Fact]
        public void Test1()
        {
            Assert.NotEmpty(controller.Get());
        }
    }
}
