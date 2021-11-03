using _Core.Interfaces;
using _Core.Models;
using Bank_Management_System.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTesting.Validation.Services;
using Xunit;

namespace UnitTesting.Validation.Controller
{
    public class TransactionControllerTest
    {
        private readonly TransactionTestController _controller;
        private readonly ITransactionTest _service;

        public TransactionControllerTest()
        {
            _service = new ITestFakeService();
            _controller = new TransactionTestController(_service);
        }
        
        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            //Act
            var okResult = _controller.Get();

            //Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            //Act
            var okResult = _controller.Get() as OkObjectResult;

            //Assert
            var items = Assert.IsType<List<Bill_Types>>(okResult.Value);
            Assert.Equal(4, items.Count);
        }


        [Fact]
        public void GetById_UnknownIntPassed_ReturnsNotFoundResult()
        {
            //Act
            var notFoundResult = _controller.Get(6);

            //Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public void GetById_ExistingIntPassed_ReturnsOkResult()
        {
            //Arrange
            var testInt = 1;

            //Act
            var okResult = _controller.Get(testInt);

            //Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }
      
        [Fact]
        public void GetById_ExistingIntPassed_ReturnsRightItem()
        {
            //Arrange
            var testInt = 2;

            //Act
            var okResult = _controller.Get(testInt) as OkObjectResult;

            //Assert
            Assert.IsType<Bill_Types>(okResult.Value);
            Assert.Equal(testInt, (okResult.Value as Bill_Types).Id);
        }
        
    }
}
