using DKAC.App.Models;
using DKAC.Repositories;
using DKAC.Services.Implementations;
using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.Security.Claims;

namespace DKAC.App.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IRegisterService registerService;
        private readonly IToastNotification toastNotification;
        private readonly IDepartmentService departmentService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IReportService reportService;

        public RegisterController(IRegisterService registerService, IToastNotification toastNotification, IDepartmentService departmentService, IAuthenticationService authenticationService, IReportService reportService)
        {
            this.registerService = registerService;
            this.toastNotification = toastNotification;
            this.departmentService = departmentService;
            this._authenticationService = authenticationService;
            this.reportService = reportService;
        }
        //Đăng kí 1 user
        public IActionResult RegisterToOne()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(userID);
                ViewBag.FullName = fullName;
                ViewBag.UserID = userID;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                int checkOrder = reportService.checkUserOrderInDay(userID);
                if (checkOrder > 0)
                {
                    return RedirectToAction("detailsRegisterUsers");
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        public IActionResult RegisterToOne(OrderModel order)
        {
            string fullName = _authenticationService.getFullName(order.UserID);
            ViewBag.FullName = fullName;
            string departmentName = departmentService.getDepartmentName(order.DepartmentID);
            ViewBag.DepartmentName = departmentName;

            bool result = registerService.RegisterMeal(order.UserID, order.ShiftIDs, order.Quantities, order.ShiftDate, order.DepartmentID);
            if (result)
            {
                toastNotification.AddSuccessToastMessage("Đăng ký thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("detailsRegisterUsers");
            }

            toastNotification.AddErrorToastMessage("Đăng ký thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
            return RedirectToAction("RegisterToOne");
        }

        public IActionResult detailsRegisterUsers()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(userID);
                ViewBag.FullName = fullName;
                ViewBag.UserID = userID;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                int checkOrder = reportService.checkUserOrderInDay(userID);
                if (checkOrder < 0)
                {
                    return RedirectToAction("RegisterToOne");
                }

                var data = reportService.getOrderInDayUser(userID).ToList();
                var shiftIDs = new List<int> { 1, 2, 3 };
                foreach (var shiftID in shiftIDs)
                {
                    if (!data.Any(o => o.ShiftID == shiftID))
                    {
                        data.Add(new Orders { ShiftID = shiftID, Quantity = 0 });
                    }
                }

                data = data.OrderBy(o => o.ShiftID).ToList();

                var model = new OrderModel
                {
                    UserID = userID,
                    DepartmentID = departmentID,
                    ShiftOrders = data.Select(o => new ShiftOrder
                    {
                        ShiftID = o.ShiftID,
                        Quantity = o.Quantity
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }

        }
        public IActionResult UpdateDetailsRegisterUsers(OrderModel model)
        {
            int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
            bool result = registerService.EditRegisterUser(model.UserID, model.ShiftIDs, model.Quantities, departmentID);
            if (result)
            {
                toastNotification.AddSuccessToastMessage("Cập nhật thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("RegisterToOne");
            }
            else
            {
                toastNotification.AddErrorToastMessage("Cập nhật thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("detailsRegisterUsers");
            }
        }


        //manager đăng kí cho các user cùng phòng
        public IActionResult RegisterMealsForUsers()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(userID);
                ViewBag.FullName = fullName;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                var managerID = departmentService.getManagerIDDepartment(departmentID);
                if (managerID == userID)
                {
                    int checkOrder = reportService.CheckUserInDepartmentOrderInDay(departmentID);
                    if (checkOrder > 0)
                    {
                        return RedirectToAction("DetailsRegisterAllUsers");
                    }

                    var users = _authenticationService.getAllUserInDepartment(departmentID);

                    OrderModel orderModel = new OrderModel
                    {
                        Users = users.ToList(),
                    };
                    return View(orderModel);
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Bạn không có quyền truy cập", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("RegisterToOne");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult RegisterMealsForUsers(OrderModel orderModel)
        {
            try
            {
                int UserID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(UserID);
                ViewBag.FullName = fullName;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                bool result = registerService.RegisterMeals(orderModel.UserIDs, orderModel.ShiftIDs, orderModel.Quantities, orderModel.ShiftDate, departmentID);
                if (result)
                {
                    toastNotification.AddSuccessToastMessage("Đăng ký thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("DetailsRegisterAllUsers");
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Đăng ký thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("RegisterMealsForUsers");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult DetailsRegisterAllUsers()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(userID);
                ViewBag.FullName = fullName;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                int checkOrder = reportService.CheckUserInDepartmentOrderInDay(departmentID);
                if (checkOrder < 0)
                {
                    return RedirectToAction("RegisterMealsForUsers");
                }

                var users = _authenticationService.getAllUserInDepartment(departmentID);

                var data = reportService.getOrderUserInDepartment(departmentID).ToList();
                var shiftIDs = new List<int> { 1, 2, 3 };

                var model = new OrderModel
                {
                    Users = users.ToList(),
                    UserID = userID,
                    DepartmentID = departmentID,
                    UserOrders = users.Select(user => new UserOrderModel
                    {
                        UserID = user.UserID,
                        FullName = user.FullName,
                        ShiftOrders = shiftIDs.Select(shiftID =>
                        {
                            var order = data.FirstOrDefault(o => o.UserID == user.UserID && o.ShiftID == shiftID);
                            return new ShiftOrder
                            {
                                ShiftID = shiftID,
                                Quantity = order != null ? order.Quantity : 0
                            };
                        }).ToList()
                    }).ToList()
                };

                var managerID = departmentService.getManagerIDDepartment(departmentID);
                if (managerID == userID)
                {
                    return View(model);
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Bạn không có quyền truy cập", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("RegisterToOne");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        public IActionResult UpdateDetailsRegisterAllUsers(OrderModel orderModel)
        {
            try
            {
                int UserID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(UserID);
                ViewBag.FullName = fullName;

                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");
                string departmentName = departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                ViewBag.DepartmentID = departmentID;

                bool result = registerService.EditRegisterUsers(orderModel.UserIDs, orderModel.ShiftIDs, orderModel.Quantities, departmentID);
                if (result)
                {
                    toastNotification.AddSuccessToastMessage("Cập nhật thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("RegisterMealsForUsers");
                }
                else
                {
                    toastNotification.AddErrorToastMessage("Cập nhật thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                    return RedirectToAction("RegisterMealsForUsers");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
