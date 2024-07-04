using DKAC.App.Models;
using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DKAC.App.Areas.Admin.Controllers
{
    public class DepartmentManagerController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IToastNotification toastNotification;
        private readonly IAuthenticationService authenticationService;


        public DepartmentManagerController(IDepartmentService departmentService, IToastNotification toastNotification, IAuthenticationService authenticationService)
        {
            _departmentService = departmentService;
            this.toastNotification = toastNotification;
            this.authenticationService = authenticationService;
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
                    List<string> managerName;

                    int totalRecords = _departmentService.countDepartments();
                    ViewBag.HasNextPage = totalRecords > pageNumber * pageSize;
                    ViewBag.PageNumber = pageNumber;

                    var data = _departmentService.GetDepartments(pageNumber, pageSize, out managerName);
                    ViewBag.ManagerName = managerName;

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

        public IActionResult Delete(int DepartmentID)
        {
            _departmentService.deleteDepartment(DepartmentID);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteManager(int DepartmentID)
        {
            _departmentService.deleteManagerID(DepartmentID);
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(DepartmentModel model)
        {
            bool data = _departmentService.createDepartment(model.DepartmentName);
            if (data)
            {
                toastNotification.AddSuccessToastMessage("Tạo phòng ban thành công", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("Index");
            }
            else
            {
                toastNotification.AddErrorToastMessage("Tạo phòng ban thất bại", new ToastrOptions { PositionClass = ToastPositions.BottomRight, TimeOut = 2000 });
                return RedirectToAction("Create");
            }

        }
    }
}
