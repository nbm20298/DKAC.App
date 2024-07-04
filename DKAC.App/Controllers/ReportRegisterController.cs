using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DKAC.App.Controllers
{
    public class ReportRegisterController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDepartmentService _departmentService;
        private readonly IToastNotification toastNotification;
        public ReportRegisterController(IReportService reportService, IAuthenticationService authenticationService, IDepartmentService departmentService, IToastNotification toastNotification)
        {
            _reportService = reportService;
            _authenticationService = authenticationService;
            _departmentService = departmentService;
            this.toastNotification = toastNotification;
        }

        //thống kê user theo tháng
        public ActionResult GetOrdersByUserIDAndMonth()
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string fullName = _authenticationService.getFullName(userID);
                ViewBag.FullName = fullName;
                int departmentID = (int)HttpContext.Session.GetInt32("DepartmentID");

                string departmentName = _departmentService.getDepartmentName(departmentID);
                ViewBag.DepartmentName = departmentName;
                var data = _reportService.GetDayUserOrders(userID);

                return View(data);
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //thống kê ca trong ngày các phòng và danh sách user trong phòng
        public ActionResult UserOrderShiftInDay(DateTime? dateInsert, int? shiftID)
        {
            try
            {
                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string roleName = _authenticationService.getRoleNameUser(userID);

                if (roleName == "Admin")
                {
                    DateTime date = dateInsert ?? DateTime.Now.Date;
                    int shift = shiftID ?? 1;
                    ViewBag.date = date;
                    var data = _reportService.GetOrderUserInShiftDay(date, shift);
                    return View(data);
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

        //thống kê ca ăn tháng của năm
        public ActionResult UserOrderShiftInMonth()
        {
            try
            {

                int userID = (int)HttpContext.Session.GetInt32("UserID");
                string roleName = _authenticationService.getRoleNameUser(userID);

                if (roleName == "Admin")
                {
                    var data = _reportService.GetOrderUserInShiftMonth();
                    return View(data);
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
    }
}
