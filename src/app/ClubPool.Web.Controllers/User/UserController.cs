﻿using System;
using System.Web.Mvc;
using System.Web.Security;

using MvcContrib;
using MvcContrib.Attributes;
using SharpArch.Web.NHibernate;
using xVal.ServerSide;
using Elmah;

using ClubPool.ApplicationServices.Contracts;
using ClubPool.Web.Controllers.User.ViewModels;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using Core = ClubPool.Core;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRoleService roleService;

    public UserController(IAuthenticationService authSvc, IMembershipService membershipSvc, IRoleService roleSvc) {
      authenticationService = authSvc;
      membershipService = membershipSvc;
      roleService = roleSvc;
    }

    public ActionResult Index() {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(x => x.Index());
      }

      return this.RedirectToAction(x => x.Login(new LoginViewModel()));
    }

    [AcceptGet]
    public ActionResult Login(string returnUrl) {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(c => c.Index());
      }
      else {
        return View(new LoginViewModel() { ReturnUrl = returnUrl });
      }
    }

    [AcceptPost]
    public ActionResult Login(LoginViewModel viewModel) {
      if (membershipService.ValidateUser(viewModel.Username, viewModel.Password)) {
        authenticationService.LogIn(viewModel.Username, viewModel.StayLoggedIn);
        if (!string.IsNullOrEmpty(viewModel.ReturnUrl)) {
          return this.Redirect(viewModel.ReturnUrl);
        }
        else {
          return this.RedirectToAction<HomeController>(x => x.Index());
        }
      }
      else {
        viewModel.Password = "";
        viewModel.Message = "Invalid username/password";
        return View(viewModel);
      }
    }

    public ActionResult LoginStatus() {
      var viewModel = new LoginStatusViewModel() {
        UserIsLoggedIn = authenticationService.IsLoggedIn(),
      };
      if (viewModel.UserIsLoggedIn) {
        viewModel.Username = authenticationService.GetCurrentIdentity().Username;
      }
      return PartialView(viewModel);
    }

    public ActionResult LoginGadget() {
      return PartialView(new LoginViewModel());
    }

    public ActionResult AccountHelp() {
      return View();
    }

    [AcceptGet]
    public ActionResult ResetPassword() {
      return View();
    }

    [AcceptPost]
    public ActionResult ResetPassword(ResetPasswordViewModel viewModel) {
      return View();
    }

    public ActionResult Logout() {
      authenticationService.LogOut();
      return this.RedirectToAction<HomeController>(x => x.Index());
    }

    [AcceptGet]
    public ActionResult SignUp() {
      return View(new SignUpViewModel());
    }

    [AcceptPost]
    [ValidateAntiForgeryToken]
    [Transaction]
    public ActionResult SignUp(SignUpViewModel viewModel) {
      try {
        viewModel.Validate();
        membershipService.CreateUser(viewModel.Username, viewModel.Password, viewModel.Email, false);
        return View("SignUpComplete");
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
      }
      catch (MembershipCreateUserException me) {
        var context = System.Web.HttpContext.Current;
        // log the exception
        var signal = ErrorSignal.FromContext(context);
        if (signal != null) {
          signal.Raise(me, context);
        }
        viewModel.ErrorMessage = me.Message;
      }
      return View(viewModel);
    }
  }
}
