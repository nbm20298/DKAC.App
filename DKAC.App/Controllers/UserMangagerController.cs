using DKAC.App.Models;
using DKAC.Repositories;
using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DKAC.App.Areas.Admin.Controllers
{
    public class UserMangagerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService authenticationService;
        private readonly IToastNotification toastNotification;
        public UserMangagerController(IUserService userService, IAuthenticationService authenticationService, IToastNotification toastNotification)
        {
            _userService = userService;
            this.authenticationService = authenticationService;
            this.toastNotification = toastNotification;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string roleName = authenticationService.getRoleNameUser(userID);

                if (roleName == "Admin")
                {
                    int pageSize = 13;
                    List<string> departmentNames;
                    List<string> roleNames;

                    var data = _userService.GetUsers(pageNumber, pageSize, out departmentNames, out roleNames);

                    int totalRecords = _userService.countUser();

                    ViewBag.HasNextPage = totalRecords > pageNumber * pageSize;
                    ViewBag.PageNumber = pageNumber;
                    ViewBag.DepartmentNames = departmentNames;
                    ViewBag.RoleNames = roleNames;

                    return View(data);
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Bạn không có quyền truy cập", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult Delete(int userID)
        {
            
            _userService.deleteUser(userID);
            return RedirectToAction("Index");
        }

        public IActionResult UpdatePassword(int userID)
        {
            var user = authenticationService.GetUserById(userID);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserModel
            {
                ID = user.UserID
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePassword(UserModel model)
        {
            bool result = authenticationService.UpdatePassword(model.ID, model.OldPassword, model.NewPassword);
            if (result)
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string roleName = authenticationService.getRoleNameUser(userID);

                if (roleName == "Admin")
                {
                    toastNotification.AddSuccessToastMessage("Thay đổi mật khẩu thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("Index");
                }
                toastNotification.AddSuccessToastMessage("Thay đổi mật khẩu thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("RegisterToOne", "Register");

            }
            else
            {
                toastNotification.AddErrorToastMessage("Thay đổi mật khẩu thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("UpdatePassword");
            }
        }
    }
}
