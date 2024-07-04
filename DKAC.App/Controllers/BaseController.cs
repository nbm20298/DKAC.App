using DKAC.App.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DKAC.App.Helpers;

namespace DKAC.App.Controllers
{
    [CustomAuthorize(Roles = "User")]
    [Area("User")]
    public class BaseController : Controller
    {
        IUserAccessor _userAccessor;
        public BaseController(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }
        public DKAC.Repositories.Users CurrentUser
        {
            get
            {
                if (User != null)
                    return _userAccessor.GetUser();
                else
                    return null;
            }
        }
    }
}
