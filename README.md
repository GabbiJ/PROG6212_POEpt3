> **_NOTE:_** This README file is copied from the original project thus previous parts of the project are referenced. This was kept in to demonstrate the feedback implementation in this project.

 # PROG6212 POE Part 3

This program is aimed to aid Lerato in her studies by allowing her to keep track of important information about her current semester. This program also allows other students to track their semester information using an account system.


# System Requirements

* The system must use the Windows operating system (Windows 7 or a later version).

# Installing and Starting the Program

## Installing the Program
1. Navigate to the GitHub repository linked at the bottom of this README file.
2. Download the zipped folder containing the program (or clone the repository onto your local machine)
3. Unzip the folder in the desired location on your local device.
## Starting the Program
To start the program the user must have Visual Studio installed on their machine and be able to run a ASP.Net Core web application.

1. Within the unzipped folder navigate to the solution file using the following file path as reference:
**ST10034968_PROG6212_POE_WebApp\ST10034968_PROG6212_POE_WebApp.sln**
2. Open the solution file in visual studio and click the green play button to run the file.
# Using the Program
* When the program starts the user will be greeted with a greeting window. The user can choose whether they want to login or register their account.
## Registering a User
* To register a user the user must click the "Register" button on the greeting page.
* On the registration window the user can enter a unique username and password that they desire and click the "register" button to register a user.
## Logging In
* To log into the application the user can click the "Log In" button on the greeting window.
* The user can then enter their username and their password and click the "Login" button.
* If the username and password are valid the user will be greeted with the home window.
## Editing Current Semester Information
The current semester information can be changed at any time. 
* To change the current semester's information navigate to the "Edit" button next to the semester information at the top of the home window.
* This will take the user to the window in which they can input the current semester information into. 
* This window is also where the user can select which module they want to study on a certain day of the week.
* The user can enter the new information and click "Enter"
* The home window will reappear and the user will see the updated semester information along with which module they plan to study that day at the top.
## Adding a Module
* To add a module the user must click the "Add Module" button under the  table on the home window. 
* This will open up a window in which the user will enter the relevant information about the module.
* The user should then click the "Add" button to add the module information to to the database.
* This will close the window.
* For the user to see the updated information the user can click the refresh button in their browser.
## Adding Study Time
* To add a study time session to be stored in the database, the user will click the "Add Study Time" button on the home window.
* A window will appear and the user will enter the relevant information about their study time session. 
* To add the study time to memory the user will click the "Add" button and the study time information will be added to the database.
# Changes Made for POE Part 3
* The application is now a web application using the MVC ASP.Net Core architecture.
* The user can now select what they plan to study on certain days of the week when editing their semester information.
	* The user will also be reminded on the home page what module they have planned to study that day.
# Changes Made according to POE Part 2 Feedback
I achieved a mark of 100% for my POE part 2 and hence no suggestions were given for the application and thus no amendments were made to the code used for the POE Part 2 and carried over into this project. 

