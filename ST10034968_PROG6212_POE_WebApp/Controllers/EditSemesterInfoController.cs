using Microsoft.AspNetCore.Mvc;
using POEClassLibrary;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection.Emit;

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
            return View(allMmodules());
        }
        public IActionResult AddStudyTime()
        {
            return View(CurrentSemester.modules);
        }

        //methods for edit semester info view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSemesterInfo(IFormCollection collection)
        {
            try
            {
                //checking if correct values have been entered
                DateTime? startDate = Convert.ToDateTime(collection["txtStartDate"]);
                int? numOfWeeks = Convert.ToInt32(collection["txtNumOfWeeks"]);
                if (startDate == null)
                {
                    throw new Exception("Please select a start date.");
                }
                else if (numOfWeeks == null)
                {
                    throw new Exception("Please enter the number of weeks.");
                }
                else
                {
                    //entering info into the database
                    CurrentSemester.updateDB(((DateTime)startDate).ToString("yyyy-MM-dd"), (int)numOfWeeks);
                    //going back to home window
                    return RedirectToAction("HomePage", "Home");
                }
            }
            catch (FormatException fe)
            {
                ViewBag.ErrorMessage = "Please ensure values are entered correctly.";
                return View("EditSemesterInfo");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("EditSemesterInfo");
            }
        }

        //methods for add module view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddModule(IFormCollection collection)
        {
            //adding modules to database
            try
            {
                //declaring temporary variables to store the values inputted
                string? modName = collection["txtName"];
                string? modCode = collection["txtCode"];
                int? numOfCredits = Convert.ToInt32(collection["txtNumOfCredits"]);
                double? classHrsPerWeek = Convert.ToInt32(collection["txtClassHoursPerWeek"]);
                //checking if all values are inputted correctly
                if (modName == null)
                {
                    throw new Exception("Please enter the module name\n");
                }
                else if (modCode == null)
                {
                    throw new Exception("Please enter the module code\n");
                }
                else if (numOfCredits == null)
                {
                    throw new Exception("Please enter the number of credits\n");
                }
                else if (classHrsPerWeek == null)
                {
                    throw new Exception("Please enter class hours per week\n");
                }
                else
                {
                    //adding module to database
                    addModuleToDB(modCode, modName, (int)numOfCredits, (double)classHrsPerWeek);
                    return RedirectToAction("HomePage", "Home");
                }
            }
            catch (FormatException fe)
            {
                ViewBag.ErrorMessage = "Please ensure values are entered correctly.";
                return View("AddModule", allMmodules());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("AddModule");
            }
        }

        public List<Module> allMmodules()
        {
            //populating table with all modules in database
            //making list that will store all modules in the database 
            List<Module> allMmodulesInDB = new List<Module>();
            using (SqlConnection con = Connections.GetConnection())
            {
                Module tempMod = null;
                string strSelect = $"SELECT * FROM Module;";
                con.Open();
                SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    while (r.Read())
                    {
                        tempMod = new Module(r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3));
                        allMmodulesInDB.Add(tempMod);
                    }
                }
            }
            return allMmodulesInDB;
        }

        /// <summary>
        /// Method that handles adding the modules to the database
        /// </summary>
        /// <param name="modCode">Module Code</param>
        /// <param name="modName">Module Name</param>
        /// <param name="modCredits">Credits for Module</param>
        /// <param name="modClassHours">Amount of class hours for the module</param>
        public async void addModuleToDB(string modCode, string modName, int modCredits, double modClassHours)
        {

            //checking if the module already exists in the database 
            Module mod = null;
            using (SqlConnection con = Connections.GetConnection())
            {
                string strSelect = $"SELECT * FROM Module WHERE ModCode = '{modCode}'";
                await con.OpenAsync();
                SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    //checking if module exists (using module code) to register student for
                    if (await r.ReadAsync())
                    {
                        mod = new Module(r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3));
                    }

                }
                if (mod != null)
                {
                    //inserting module into databse if the module already exists
                    string strInsert = $"INSERT INTO RegisterModule VALUES('{CurrentSemester.ID}', '{mod.Code}')";
                    SqlCommand cmdInsert = new SqlCommand(strInsert, con);
                    await cmdInsert.ExecuteNonQueryAsync();
                    return; 
                }
                //inserting data into both Module table and RegisterModule table
                //inserting new module into Module table
                string strInsert2 = $"INSERT INTO Module VALUES('{modCode}', '{modName}', {modCredits}, {modClassHours})";
                SqlCommand cmdInsert2 = new SqlCommand(strInsert2, con);
                await cmdInsert2.ExecuteNonQueryAsync();
                //inserting into RegisterModule table
                strInsert2 = $"INSERT INTO RegisterModule VALUES('{CurrentSemester.ID}', '{modCode}')";
                cmdInsert2 = new SqlCommand(strInsert2, con);
                await cmdInsert2.ExecuteNonQueryAsync();
            }
        }

        //methods for add study time view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudyTime(IFormCollection collection)
        {
            try
            {
                //making temporary variables to assign inputted values to
                DateTime? dateCom = Convert.ToDateTime(collection["txtDateCompleted"]);
                double? hrsStudied = Convert.ToDouble(collection["txtNumOfHours"]);
                string modName = collection["cmbModCode"];
                //checking data entered is valid
                if (dateCom == null)
                {
                    throw new Exception("Please select the date the studying was completed.");
                }
                else if (hrsStudied == null)
                {
                    throw new Exception("Please enter the amount of hours studied.");
                }
                else
                {
                    //check if date studied is in this semester
                    if (dateCom < CurrentSemester.StartDate)
                    {
                        throw new Exception("The date must be in this current semester.");
                    }
                    else
                    {
                        //retrieving module code based on module name from modules stored in memory
                        string modCode = "";
                        foreach (Module m in CurrentSemester.modules)
                        {
                            if (modName.Equals(m.Name))
                            {
                                modCode = m.Code;
                            }
                        }
                        //adding values to database
                        addStudyTimeToDB((DateTime)dateCom, (double)hrsStudied, modCode);
                        return RedirectToAction("HomePage", "Home");
                    }
                }
            }
            catch (FormatException fe)
            {
                ViewBag.ErrorMessage = "Please ensure values are entered correctly.";
                return View("AddStudyTime");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("AddStudyTime");
            }
        }


        /// <summary>
        /// Adds StudyTime to the database
        /// </summary>
        /// <param name="DateCompleted"> Date studying was completed</param>
        /// <param name="numOfHours">Number of hours studied</param>
        /// <param name="modCode">Module code</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async void addStudyTimeToDB(DateTime DateCompleted, double numOfHours, string modCode)
        {
            using (SqlConnection con = Connections.GetConnection())
            {
                string strInsert = $"INSERT INTO StudyTime VALUES('{DateCompleted.ToString("yyyy-MM-dd")}', {numOfHours.ToString("F2", CultureInfo.GetCultureInfo("en-US"))}, '{modCode}', {CurrentSemester.ID})";
                await con.OpenAsync();
                SqlCommand cmdInsert = new SqlCommand(strInsert, con);
                await cmdInsert.ExecuteNonQueryAsync();
            }
        }
    }
}
