using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary
{
    /// <summary>
    /// Static class that stores all data for the current semester in memory
    /// </summary>
    public static class CurrentSemester
    {
        //Declarations
        public static int ID { get; set; }
        public static List<Module> modules = new List<Module>();
        public static List<StudyTime> selfStudyCompleted = new List<StudyTime>();
        public static int? NumOfWeeks { get; set; }
        public static DateTime? StartDate { get; set; }
        public static Student user = new Student();
        public static String[] plannedModules = new String[7];

        /// <summary>
        /// This method updates the CurrentSemester table in the database
        /// </summary>
        /// <param name="sDate">Semester Start Date</param>
        /// <param name="numOfWeeks">Number of Weeks</param>
        public static async void updateDB(string sDate, int numOfWeeks, string[] plannedDays)
        {
            //Updating database with new info
            using (SqlConnection con = Connections.GetConnection())
            {
                //converting all null values to "NULL" string so that they can be insterted into the database
                for (int i = 0; i < plannedDays.Length; i++)
                {
                    if (plannedDays[i] == null)
                    {
                        plannedDays[i] = "NULL";
                    }
                }
                string strInsert = $"UPDATE CurrentSemester " +
                            $"SET StartDate = '{sDate}'," +
                            $" NumOfWeeks = {numOfWeeks}," +
                            $" Monday = '{plannedDays[0]}', " +
                            $" Tuesday = '{plannedDays[1]}', " +
                            $" Wednesday = '{plannedDays[2]}', " +
                            $" Thursday = '{plannedDays[3]}', " +
                            $" Friday = '{plannedDays[4]}', " +
                            $" Saturday = '{plannedDays[5]}', " +
                            $" Sunday = '{plannedDays[6]}' " +
                            $"WHERE Username = '{CurrentSemester.user.Username}';";
                await con.OpenAsync();
                SqlCommand cmdInsert = new SqlCommand(strInsert, con);
                await cmdInsert.ExecuteNonQueryAsync();
            }
        }
        /// <summary>
        /// This method takes info from the database and assigns it to values in this class
        /// </summary>
        public static async void assignFromDB()
        {
            using (SqlConnection con = Connections.GetConnection())
            {
                //loading current semester information from database
                string strSelect = $"SELECT * FROM CurrentSemester WHERE Username = '{CurrentSemester.user.Username}';";
                await con.OpenAsync();
                SqlCommand cmdSelect = new SqlCommand(strSelect, con);
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    if (await r.ReadAsync())
                    {
                        CurrentSemester.ID = r.GetInt32(0);
                        //only assigning following values if they are present in the database
                        if (r.IsDBNull(1) == false && r.IsDBNull(2) == false)
                        {
                            CurrentSemester.StartDate = r.GetDateTime(1);
                            CurrentSemester.NumOfWeeks = r.GetInt32(2);                           
                        }
                        //assigning values for plannedModules array
                        for (int i = 3; i < 10; i++)
                        {
                            if (r.IsDBNull(i) == false)
                            {
                                CurrentSemester.plannedModules[i - 3] = r.GetString(i);
                            }                            
                        }
                    }
                    
                }
                //loading all modules from the database
                strSelect = $"SELECT * FROM Module " +
                    $"JOIN RegisterModule ON Module.ModCode = RegisterModule.ModCode " +
                    $"WHERE CurrentSemesterID = '{CurrentSemester.ID}';";
                cmdSelect = new SqlCommand(strSelect, con);
                //list to temporarily store modules
                List<Module> tempModList = new List<Module>();
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    while (await r.ReadAsync())
                    {
                        Module tempMod = new Module(r.GetString(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3));
                        tempModList.Add(tempMod);
                    }
                }
                //assigning temp list to list stored in current semester
                CurrentSemester.modules = tempModList;
                //loading all study time from the database
                strSelect = $"SELECT * FROM StudyTime " +
                    $"WHERE CurrentSemesterID = '{CurrentSemester.ID}'";
                cmdSelect = new SqlCommand(strSelect, con);
                //making temporary list to store study time  
                List<StudyTime> tempStList = new List<StudyTime>();
                using (SqlDataReader r = cmdSelect.ExecuteReader())
                {
                    while (await r.ReadAsync())
                    {
                        StudyTime tempSt = new StudyTime(r.GetDateTime(1), r.GetDouble(2), r.GetString(3));
                        tempStList.Add(tempSt);
                    }
                }
                //assigning temp study time list to list stored in current semester
                CurrentSemester.selfStudyCompleted = tempStList; 
            }
        }
    }
}
