using AutoMapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartWeight.Panel.Server.Controllers;

namespace SmartWeight.Panel.Tests
{
    public class FactoryControllerTest
    {
        [Fact]
        public async void CreateFactoryPreview()
        {
            var mapper = new Mock<IMapper>();
            var factory = new Mock<FactoryManager>();
            var controller = new FactoryController(mapper.Object, factory.Object);
            var result = await controller.CreateFactoryAsync("220330 Автовесы тест");
            var expected = OperationResult.Ok();

            Assert.Equal(expected, result);
        }
    }
}