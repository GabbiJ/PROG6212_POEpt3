using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary
{
    /// <summary>
    /// class that stores information about a given module
    /// </summary>
    public class Module
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int NumOfCredits { get; set; }
        public double ClassHoursPerWeek { get; set; }

        /// <summary>
        /// Constructor for the module class that takes input for all available properties
        /// </summary>
        /// <param name="code">Module Code</param>
        /// <param name="name">Module Name</param>
        /// <param name="numOfCredits">Number of credits the module is worth</param>
        /// <param name="classHoursPerWeek">The amount of class hours this module has per academic week</param>
        public Module(string code, string name, int numOfCredits, double classHoursPerWeek)
        {
            Code = code;
            Name = name;
            NumOfCredits = numOfCredits;
            ClassHoursPerWeek = classHoursPerWeek;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public Module() { }
        /// <summary>
        /// Method that calculates the remaining hours of self study this module has left this week 
        /// </summary>
        /// <returns>The remaining hours left of self study this week for the module</returns>
        public double remainingHrsThisWeek()
        {
            //calculating how many self study hours there are for the module per week
            double result = this.selfStudyPerWeek();
            //calculating amount of self study hours since the last monday (taking monday as the beginning of the week) of the machines current day 
            //calculating last monday
            int daysSinceLastMonday = ((((int)DateTime.Now.DayOfWeek - (int)DayOfWeek.Monday) + 7) % 7);
            DateTime lastMondayDate = (DateTime.Now.AddDays(-daysSinceLastMonday));
            //adding all self study hours since last monday to a list using a LINQ query
            double totalSelfStudyHrsThisWeek = (from st in CurrentSemester.selfStudyCompleted
                                                where st.DateCompleted >= lastMondayDate.Date && st.DateCompleted <= DateTime.Now && st.ModCode.Equals( this.Code) 
                                                select st.NumOfHours).ToList().Sum();
            return result -= totalSelfStudyHrsThisWeek;
        }
        /// <summary>
        /// Method that calculates the self study hours this module needs per week
        /// </summary>
        /// <returns> The amount of self study hours the module has</returns>
        public double selfStudyPerWeek() => ((double)(this.NumOfCredits * 10) / (int)CurrentSemester.NumOfWeeks) - this.ClassHoursPerWeek;
    }
}
