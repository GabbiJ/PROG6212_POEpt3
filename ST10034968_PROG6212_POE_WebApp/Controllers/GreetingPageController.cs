using Microsoft.AspNetCore.Mvc;
using POEClassLibrary;
using System.Data.SqlClient;


namespace ST10034968_PROG6212_POE_WebApp.Controllers
{
    public class GreetingPageController : Controller
    {
        /// <summary>
        /// loads greeting page
        /// </summary>
        /// <returns></returns>
        public IActionResult Greeting()
        {
            return View();
        }

        /// <summary>
        /// loads login page
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// loads register page
        /// </summary>
        /// <returns></returns>
        public IActionResult Register() 
        {
            return View();
        }


        /// <summary>
        /// Post method for login page, activated by clicking login button
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(IFormCollection collection)
        {
            try
            {
                //checking if all fields have been filled correctly
                if (collection["txtUsername"] == "" || collection["txtPassword"] == "")
                {
                    throw new Exception("Please enter a username and password.");
                }
                //checking if login credentials are correct then if correct the user is stored in memeory
                if ( ValidateUser(collection["txtUsername"], collection["txtPassword"]))
                {
                    using (SqlConnection con = Connections.GetConnection())
                    {
                        //fetching user from the database that matches the inputted username
                        string strSelect = $"SELECT * FROM Student WHERE Username = '{collection["txtUsername"]}';";
                        con.Open();
                        SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                        using (SqlDataReader r = cmdSelect.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                //storing user data in memory
                                CurrentSemester.user = new Student(r.GetString(0), r.GetString(1));
                            }
                        }
                    }
                    //closing this window and going to homepage
                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    throw new Exception("Username or password is incorrect.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Login");
            }
        }

        /// <summary>
        /// Checks the validity of the username and password inputted
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="pass">Student's Password</param>
        /// <returns></returns>
        public bool ValidateUser(string username, string pass)
        {
            //hashing inputted password 
            string hashedPass = hashString(pass);
            //seeing if any users match the credentials
            Student? fetchedStudent = null;
            using (SqlConnection con2 = Connections.GetConnection())
            {
                //fetching user from the database that matches the inputted username
                string strSelect = $"SELECT * FROM Student WHERE Username = '{username}';";
                con2.Open();
                SqlCommand cmdSelect = new SqlCommand(strSelect, con2);
                //assigning student object out of fetched data
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    while (r.Read())
                    {
                        fetchedStudent = new Student(r.GetString(0), r.GetString(1));
                    }
                }
            }
            //checking if username and password match
            if (fetchedStudent == null)
            {
                return false;
            }
            if (fetchedStudent.Username.Equals(username) && fetchedStudent.Password.Equals(hashedPass))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes a string and outputs the hash string of the input string
        /// </summary>
        /// <param name="rawText">Input string</param>
        /// <returns>Hashed string</returns>
        public string hashString(string rawText)
        {
            string salt = "HU958lew8439i";
            //the methods used to hash the password were done using methods found on https://www.sean-lloyd.com/post/hash-a-string/
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                //converting string to byte array
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(rawText + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                //converting from byte to string
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hash;
            }
        }

        /// <summary>
        /// post method for reigstration page, activated by clicking register button
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(IFormCollection collection)
        {
            try
            {
                //checking all fields have valid values
                if (collection["txtUsername"] == "" || collection["txtPassword"] == "")
                {
                    throw new Exception("Please enter a username and password.");
                }
                //checking if username is taken
                Student s = null;
                using (SqlConnection con = Connections.GetConnection())
                {
                    //fetching user from the database that matches the inputted username
                    string strSelect = $"SELECT * FROM Student WHERE Username = '{collection["txtUsername"]}';";
                    con.Open();
                    SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                    using (SqlDataReader r = cmdSelect.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            s = new Student(r.GetString(0), r.GetString(1));
                        }
                    }
                }
                //registers user if username is not taken
                if (s == null)
                {
                    registerStudent(collection["txtUsername"], collection["txtPassword"]);
                }
                else
                {
                    throw new Exception("This username is taken.");
                }
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Register");
            }
        }

        /// <summary>
        /// Registers a student in the database
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="pass">Password</param>
        public async void registerStudent(string username, string pass)
        {
            try
            {
                //hashing password
                string hashedPass = hashString(pass);
                //inserting student data into database
                using (SqlConnection con2 = Connections.GetConnection())
                {
                    //entering student into the database
                    string strInsert = $"INSERT INTO Student VALUES('{username}', '{hashedPass}');";
                    await con2.OpenAsync();
                    SqlCommand cmdInsert = new SqlCommand(strInsert, con2);
                    await cmdInsert.ExecuteNonQueryAsync();
                    //creating a row in current semester table for student
                    strInsert = $"INSERT INTO CurrentSemester VALUES(NULL, NULL, '{username}');";
                    cmdInsert = new SqlCommand(strInsert, con2);
                    await cmdInsert.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
        }
    }
}
