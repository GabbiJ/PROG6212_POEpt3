using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ComponentModel.Design;

namespace POEClassLibrary
{
    /// <summary>
    /// This class contains information pertaining to a user (student) 
    /// </summary>
    public class Student
    {
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Constructor that takes values for username and password
        /// </summary>
        /// <param name="username">Student's username</param>
        /// <param name="password">Student's password</param>
        public Student(string username, string password)
        {
            Username = username;
            Password = password;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public Student() { }
    }
}
