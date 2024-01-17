# Description
A web app that creates and processes vessel inspection questionnaires.  
The various questions are organized in categories, and are used for building questionnaires.  
Afterwards, the user creates a "Briefcase" and attaches questionnaires, selected from a list of all created questionnaires.  
This process results in a single .sdf file (SQL Compact), which is created by the web app and downloaded to the user's computer.  
This file is then processed (questionnaires answered) using another app, and the processed file is opened by the web app and transferred to the database as an "Attendance".

# How to run
cd AttendanceWeb   
cd Attendance  
dotnet run --urls http://0.0.0.0:8080
