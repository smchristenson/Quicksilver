﻿using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Shared.Models.Identity;
using EPiServer.Reference.Commerce.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Login.Services;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.Controllers;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.Pages;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;


namespace EPiServer.Reference.Commerce.Site.Tests.Features.ResetPassword.Controllers
{
    public class ResetPasswordControllerTests
    {
        [Fact]
        public void Index_ShouldReturnForgotPasswordView()
        {
            ViewResult result = _subject.Index(_resetPasswordPageMock.Object) as ViewResult;
            Assert.Equal("ForgotPassword", result.ViewName);
        }

        [Fact]
        public void Index_ShouldUseForgotPasswordViewModel()
        {
            ViewResult result = _subject.Index(_resetPasswordPageMock.Object) as ViewResult;
            ForgotPasswordViewModel viewModel = result.Model as ForgotPasswordViewModel;
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void ForgotPasswordConfirmation_ShouldReturnForgotPasswordConfirmationView()
        {
            ViewResult result = _subject.ForgotPasswordConfirmation() as ViewResult;
            Assert.Equal("ForgotPasswordConfirmation", result.ViewName);
        }

        [Fact]
        public void ResetPassword_WhenCodeIsNotNull_ShouldReturnResetPasswordView()
        {
            string code = "tHis_IS_aN_awFully_tR1cky_C0dE";
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel();
            ViewResult result = _subject.ResetPassword(code) as ViewResult;
            Assert.Equal("ResetPassword", result.ViewName);
        }

        [Fact]
        public void ResetPassword_WhenCodeIsNull_ShouldReturnErrorView()
        {
            string code = null;
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel();
            ViewResult result = _subject.ResetPassword(code) as ViewResult;
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void ResetPassword_WhenValidInput_ShouldRedirectToResetPasswordConfirmationView()
        {
            string code = "A1B2C3";
            string newPassword = "myNewPassword";
            string email = "john.doe@company.com";
            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                Id = Guid.NewGuid().ToString()
            };
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel
            {
                Code = code,
                Email = email,
                Password = newPassword,
                Password2 = newPassword
            };
            _userManagerMock.Setup(x => x.FindByNameAsync(email)).Returns(Task.FromResult<ApplicationUser>(user));
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user.Id, code, It.IsAny<string>())).Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));

            RedirectToRouteResult result = _subject.ResetPassword(viewModel).Result as RedirectToRouteResult;
            Assert.Equal("ResetPasswordConfirmation", result.RouteValues["action"]);
        }

        [Fact]
        public void ResetPasswordConfirmation_ShouldReturnResetPasswordConfirmationView()
        {
            ViewResult result = _subject.ResetPasswordConfirmation() as ViewResult;
            Assert.Equal("ResetPasswordConfirmation", result.ViewName);
        }

        ResetPasswordController _subject;
        private Mock<ApplicationUserManager> _userManagerMock;
        private Mock<UserService> _userServiceMock;
        private Mock<ResetPasswordPage> _resetPasswordPageMock;


        public ResetPasswordControllerTests()
        {
            Mock<ApplicationSignInManager> signinManagerMock = null;
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var authenticationManagerMock = new Mock<IAuthenticationManager>();
            var contentLoaderMock = new Mock<IContentLoader>();
            var mailServiceMock = new Mock<IMailService>();
            var localizationService = new MemoryLocalizationService();
            var customerContextFacade = new Mock<CustomerContextFacade>();

            _resetPasswordPageMock = new Mock<ResetPasswordPage>();
            _userManagerMock = new Mock<ApplicationUserManager>(userStoreMock.Object);
            signinManagerMock = new Mock<ApplicationSignInManager>(_userManagerMock.Object, authenticationManagerMock.Object);
            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, signinManagerMock.Object, authenticationManagerMock.Object, localizationService, customerContextFacade.Object);
            _subject = new ResetPasswordController(signinManagerMock.Object, _userManagerMock.Object, _userServiceMock.Object, contentLoaderMock.Object, mailServiceMock.Object, new MemoryLocalizationService());
        }
    }
}
