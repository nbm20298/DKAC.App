using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DKAC.App.Areas.Admin.Controllers
{
    public class HomeManagerController : Controller
    {
        private readonly IReportService reportService;
        private readonly IAuthenticationService authenticationService;
        private readonly IToastNotification toastNotification;
        public HomeManagerController(IReportService reportService, IAuthenticationService authenticationService, IToastNotification toastNotification)
        {
            this.reportService = reportService;
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
                    List<string> fullName;
                    List<string> shiftNames;

                    var data = reportService.GetDepartments(pageNumber, pageSize, out departmentNames, out fullName, out shiftNames);
                    int totalRecords = reportService.countUserOrders();

                    ViewBag.HasNextPage = totalRecords > pageNumber * pageSize;
                    ViewBag.PageNumber = pageNumber;
                    ViewBag.DepartmentNames = departmentNames;
                    ViewBag.FullName = fullName;
                    ViewBag.ShiftNames = shiftNames;

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
    }
}
