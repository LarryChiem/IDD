# AdminUI

## Project Setup

Open AdminUI.sln in Visual Studio

In the Package Manager Console, run the following commands:

```
	Update-Database -Context AdminUIUserContext
	Update-Database -Context SubmissionContext
	Update-Database -Context PayPeriodContext
```

If you see an error along the lines of "Database xxx already exists, pick a different name",
then go to view -> SQL Server Object Explorer -> (localdb)\MSSQL.... -> Databases -> Right Click AzureDB -> Delete.
Now re-run the commands and it should work.

If you're adding a field to the Submission.cs Mode, then you need to first Delete the AzureDB and then run:

```
Add-Migration -Context SubmissionContext
Update-Database -Context SubmissionContext
```

## How to run

Compile with IISExpress

## What it does

This is a Web App that connects to an Azure DB that stores timesheets fron Multnomah County caretakers.
The app loads the timesheets from the database and displays them in a table sortable by each element.
The admin/user can then access each timesheet and process them by either approving or rejecting them.

By default, two accounts are created to play around with:

```
Username: Admin		Password: password
Username: Employee	Password: password
```

# Unit Tests

To run unit tests: 

1. Open VS
2. Open SolutionsExplorer
3. If you don't see the AdminUITests project, then you need to right click on the Solution 'AdminUI' -> 'Add' -> 'Existing Project' -> Navigate into the AdminUITests folder and select the 'AdminUITest.csproj' file.
4. Now that you have the AdminUITest in the Solution Explorer, click 'Test' in the navbar and 'Run All Tests'.
5. You should have successfully ran all tests. 
