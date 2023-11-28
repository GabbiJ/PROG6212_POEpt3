using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary
{
    /// <summary>
    /// Class that stores an individual study time session's information
    /// </summary>
    public class StudyTime
    {
        public DateTime DateCompleted { get; set; }
        public double NumOfHours { get; set; }
        public string ModCode { get; set; }

        /// <summary>
        /// Constructor that assignes data to all StudyTime properties 
        /// </summary>
        /// <param name="dateCompleted">The date the study time was completed</param>
        /// <param name="numOfHours">The number of hours studied</param>
        /// <param name="moduleCode">The module code of the module studied</param>
        public StudyTime(DateTime dateCompleted, double numOfHours, string moduleCode)
        {
            DateCompleted = dateCompleted;
            NumOfHours = numOfHours;
            ModCode = moduleCode;
        }
    }
}
