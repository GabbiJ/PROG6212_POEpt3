using Microsoft.AspNetCore.Mvc;
using POEClassLibrary;

namespace ST10034968_PROG6212_POE_WebApp.Controllers
{
    public class EditSemesterInfoController : Controller
    {
        public IActionResult EditSemesterInfo()
        {
            return View();
        }
        public IActionResult AddModule()
        {
            return View();
        }
        public IActionResult AddStudyTime()
        {
            return View(CurrentSemester.modules);
        }

        //My methods
    }
}
