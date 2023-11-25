using Microsoft.AspNetCore.Mvc;
using POEClassLibrary;
using System.Data.SqlClient;
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
            //populating table with all modules in database
            //making list that will store all modules in the database 
            List<Module> allMmodules = new List<Module>();

            using (SqlConnection con = Connections.GetConnection())
            {
                Module tempMod = null;
                string strSelect = $"SELECT * FROM Module";
                con.Open();
                SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    tempMod = new Module(r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3));
                    allMmodules.Add(tempMod);
                }
            }

                return View(allMmodules);
        }
        public IActionResult AddStudyTime()
        {
            return View(CurrentSemester.modules);
        }

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
                return View("AddModule");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("AddModule");
            }
        }

        /// <summary>
        /// Method that handles adding the modules to the database
        /// </summary>
        /// <param name="modCode"></param>
        /// <param name="modName"></param>
        /// <param name="modCredits"></param>
        /// <param name="modClassHours"></param>
        public async void addModuleToDB(string modCode, string modName, int modCredits, double modClassHours)
        {
            
            //checking if the module already exists in the database 
            Module mod = null;
            MessageBoxResult? confirmModuleInsertion = null;
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
                        //asking student if they are ok with letting the module theyve inputted be overwritten by already stored module 
                        mod = new Module(r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3));
                        confirmModuleInsertion = MessageBox.Show("Another module with this code already exists (information below). By clicking yes, you agree to the information already stored under this module code to be used rather than the information you have entered." +
                            $"\nModule Code: {mod.Code}" +
                            $"\nModule Name: {mod.Name}" +
                            $"\nNumber of Credits: {mod.NumOfCredits}" +
                            $"\nClass hours per week: {mod.ClassHoursPerWeek}", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (confirmModuleInsertion == MessageBoxResult.No)
                        {
                            txbModName.Clear();
                            txbModCode.Clear();
                            txbNumOfCredits.Clear();
                            txbClassHoursPerWeek.Clear();
                            return;
                        }
                    }
                }
                //inserting data into RegisterModule when module already exists
                if (confirmModuleInsertion == MessageBoxResult.Yes)
                {
                    string strInsert = $"INSERT INTO RegisterModule VALUES('{CurrentSemester.ID}', '{mod.Code}')";
                    SqlCommand cmdInsert = new SqlCommand(strInsert, con);
                    await cmdInsert.ExecuteNonQueryAsync();
                    this.Close();
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
            this.Close();
        }



    }
}
