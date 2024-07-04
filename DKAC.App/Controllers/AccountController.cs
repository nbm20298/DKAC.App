using DKAC.App.Models;
using DKAC.Services.Implementations;
using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DKAC.App.Controllers
{
    public class AccountController : Controller
    {
        IAuthenticationService authenticationService;
        private readonly IToastNotification toastNotification;
        IDepartmentService _departmentService;
        public AccountController(IAuthenticationService authenticationService, IDepartmentService departmentService, IToastNotification toastNotification)
        {
            this.authenticationService = authenticationService;
            this.toastNotification = toastNotification;
            this._departmentService = departmentService;
        }

        //Login
        public IActionResult Login()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                toastNotification.AddErrorToastMessage("Bạn đã đăng nhập", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("RegisterToOne", "Register");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = authenticationService.AuthenticateUser(loginModel.UserName, loginModel.Password);
                if (user != null)
                {
                    int userID = user.UserID;
                    int departmentID = authenticationService.getDepartmentID(loginModel.UserName);

                    if (userID > 0 && departmentID > 0)
                    {
                        HttpContext.Session.SetInt32("UserID", userID);
                        int? sessionUserID = HttpContext.Session.GetInt32("UserID");

                        HttpContext.Session.SetInt32("DepartmentID", departmentID);
                        int? sessionDepartmentID = HttpContext.Session.GetInt32("DepartmentID");
                        toastNotification.AddSuccessToastMessage("Đăng nhập thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                        return RedirectToAction("RegisterToOne", "Register");
                    }
                }
            }

            toastNotification.AddErrorToastMessage("Sai tài khoản hoặc mật khẩu", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
            return RedirectToAction("Login");
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("DepartmentID");

            toastNotification.AddSuccessToastMessage("Đăng xuất thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
            return RedirectToAction("Index", "Home");

        }

        public IActionResult SignUp()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                toastNotification.AddErrorToastMessage("Bạn đã đăng nhập", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("RegisterToOne", "Register");
            }
            catch (Exception)
            {
                ViewBag.department = _departmentService.GetDepartments();
                return View();
            }
        }

        [HttpPost]
        public IActionResult SignUp(UserModel model)
        {
            ViewBag.department = _departmentService.GetDepartments();
            var result = authenticationService.CreateUser(model.FullName, model.UserName, model.Password, model.DepartmentId, model.ManagerID, model.RoleID);
            if (result)
            {
                toastNotification.AddSuccessToastMessage("Đăng ký thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("Login");
            }
            else
            {
                toastNotification.AddErrorToastMessage("Tạo tài khoản thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("SignUp");

            }
        }

    }
}
