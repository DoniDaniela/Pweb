using GameStore.Domain;
using GameStore.Domain.Identity;
using GameStore.Domain.Infrastructure;
using GameStore.Domain.Model;
using GameStore.WebUI.Areas.Admin.Models.DTO;
using GameStore.WebUI.Helper;
using GameStore.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameStore.WebUI.Controllers
{
    public class AccountController : BaseController
    {

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = UserManager.FindByEmail(model.Email);
                if (user != null)
                {
                    ModelState.AddModelError("", "User with this email address has already existed! Please try another email address!");
                    return View(model);
                }
                user = UserManager.FindByName(model.UserName);
                if (user != null)
                {
                    ModelState.AddModelError("", "The User Name you specified is already existing! Please try with another user name!");
                    return View(model);
                }
                Session["Register"] = model;
                String AppId = ConfigurationHelper.GetAppId();
                String SharedKey = ConfigurationHelper.GetSharedKey();
                String AppTransId = "20";
                String hash = HttpUtility.UrlEncode(CreditAuthorizationClient.GenerateClientRequestHash(SharedKey, AppId, AppTransId));

                String url = "http://ectweb2.cs.depaul.edu/ECTCreditGateway/Authorize.aspx";
                url = url + "?AppId=" + AppId;
                url = url + "&TransId=" + AppTransId;
                url = url + "&AppHash=" + hash;

                return Redirect(url);
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ProcessCreditResponse(String TransId, String TransAmount, String StatusCode, String AppHash)
        {
            String AppId = ConfigurationHelper.GetAppId();
            String SharedKey = ConfigurationHelper.GetSharedKey();

            if (CreditAuthorizationClient.VerifyServerResponseHash(AppHash, SharedKey, AppId, TransId, TransAmount, StatusCode))
            {
                switch (StatusCode)
                {
                    case ("A"): ViewBag.TransactionStatus = "Transaction Approved!"; break;
                    case ("D"): ViewBag.TransactionStatus = "Transaction Denied!"; break;
                    case ("C"): ViewBag.TransactionStatus = "Transaction Cancelled!"; break;
                }
            }
            else
            {
                ViewBag.TransactionStatus = "Hash Verification failed... something went wrong.";
            }


            if (StatusCode.Equals("A"))
            {
                RegisterViewModel model = (RegisterViewModel)Session["Register"];
                if (model != null)
                {
                    var user = new AppUser { Email = model.Email, UserName = model.UserName, Membership = model.Membership };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var newUser = UserManager.FindByEmail(model.Email);
                        var identity = await UserManager.CreateIdentityAsync(newUser, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

                        System.Web.HttpContext.Current.Cache.Remove("UserList");
                        Session["Register"] = null;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var user = UserManager.FindByEmail(model.Email);
                        var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
                        if (!String.IsNullOrEmpty(returnUrl))
                            return RedirectToLocal(returnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Log in failed, please check you email and password!");
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["OrderCount"] = 0;
            Session["CartCount"] = 0;
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult MemberProfile()
        {
            UserDTO user = new UserDTO();
            using (GameStoreDBContext context = new GameStoreDBContext())
            {
                AppUser u = context.Users.Find(User.Identity.GetUserId());
                user = new UserDTO { Id = u.Id, Email = u.Email, UserName = u.UserName, Membership = u.Membership };
            }
            return View(user);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }

    }
}