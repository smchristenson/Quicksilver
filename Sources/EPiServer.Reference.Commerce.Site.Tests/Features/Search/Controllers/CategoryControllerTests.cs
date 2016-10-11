﻿using EPiServer.Reference.Commerce.Site.Features.Search.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using Moq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;


namespace EPiServer.Reference.Commerce.Site.Tests.Features.Search.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public void Index_WhenNotAjaxRequest_ShouldReturnViewResult()
        {
            // Act
            var result = _subject.Index(null, null);

            // Assert
            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public void Index_WhenAjaxRequest_ShouldReturnPartialView()
        {
            // Arrange
            _httpRequestMock.SetupGet(x => x.Headers)
                .Returns(new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } }); // This will make Request.IsAjaxRequest return true.

            // Act
            var result = _subject.Index(null, null);

            // Assert
            Assert.IsType(typeof(PartialViewResult), result);
        }

        [Fact]
        public void Index_WhenPassingFashionNode_ShouldPassItOnToFactory()
        {
            // Arrange
            var fashionNode = new FashionNode();

            // Act
            _subject.Index(fashionNode, null);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(fashionNode, It.IsAny<FilterOptionFormModel>()));
        }

        [Fact]
        public void Index_WhenPassingFormModel_ShouldPassItOnToFactory()
        {
            // Arrange
            var formModel = new FilterOptionFormModel();

            // Act
            _subject.Index(null, formModel);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(It.IsAny<FashionNode>(), formModel));
        }

        CategoryController _subject;
        Mock<SearchViewModelFactory> _viewModelFactoryMock;
        Mock<HttpRequestBase> _httpRequestMock;


        public CategoryControllerTests()
        {
            _viewModelFactoryMock = new Mock<SearchViewModelFactory>(null, null);
            _httpRequestMock = new Mock<HttpRequestBase>();

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(_httpRequestMock.Object);
            
            _subject = new CategoryController(_viewModelFactoryMock.Object);
            _subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), _subject);
        }
    }
}
