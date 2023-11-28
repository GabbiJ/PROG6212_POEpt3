using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POEClassLibrary;

namespace ST10034968_PROG6212_POE_WebApp.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// loads home page
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePage()
        {
            loadCurrentSemester();

            //initialising list and passing it as a model to be used in the view
            //using a LINQ query to get modules from list in CurrentSemester class with an additional list
            //with calculated remaining study hours for the current week
            var moduleInfo = (from m in CurrentSemester.modules
                              select (object)new
                              {
                                  Name = m.Name,
                                  Code = m.Code,
                                  NumOfCredits = m.NumOfCredits,
                                  ClassHoursPerWeek = m.ClassHoursPerWeek.ToString("F2"),
                                  TotalStudyHrsPerWeek = m.selfStudyPerWeek().ToString("F2"),
                                  SelfStudyPerWeek = m.remainingHrsThisWeek().ToString("F2")
                              }).ToList();
            return View(moduleInfo);
        }

        //my methods
        /// <summary>
        /// Method that fetches data from the database and stores it in memory
        /// </summary>
        private async void loadCurrentSemester()
        {
            //fetching CurrentSemester data from database
            CurrentSemester.assignFromDB();
            //assign username value to title
            @ViewBag.WelcomeUser = $"{CurrentSemester.user.Username}'s Current Semester";
            //only load semester info if values are not null
            if (CurrentSemester.StartDate != null && CurrentSemester.NumOfWeeks != null)
            {
                //assign values for duration and start date of semester to relevant labels
                @ViewBag.StartDate = $"Start Date: {((DateTime)(CurrentSemester.StartDate)).Day.ToString()} {((DateTime)(CurrentSemester.StartDate)).ToString("MMMM")} {((DateTime)(CurrentSemester.StartDate)).Year.ToString()}";
                @ViewBag.Dur = $"Duration: {CurrentSemester.NumOfWeeks} weeks";
            }
            else
            {
                @ViewBag.StartDate = "Click the Edit button to enter your current semester information";
                return;
            }
            //loading what the user plans to study that day
            DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek;
            string? plannedModule = null;
            for (int i = 0; i < CurrentSemester.plannedModules.Length; i++)
            {
                if ((int)currentDayOfWeek == i)
                {
                    plannedModule = CurrentSemester.plannedModules[i];
                }
            }
            if (plannedModule != null) 
            {
                plannedModule = "Your planned module to study today: " + plannedModule;
            }
            else
            {
                plannedModule = "You have not planned to study any modules today.";
            }
            @ViewBag.PlannedModule = plannedModule;
           
        }

    }
}
