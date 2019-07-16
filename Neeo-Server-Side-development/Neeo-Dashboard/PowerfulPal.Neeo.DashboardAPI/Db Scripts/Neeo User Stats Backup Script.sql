
/***********************************************************
Script: Neeo User Stats Backup Script for Neeo Dashboard
Author : Zohaib Hanif
Date : 18 Jan 2015
Description : It takes backup of the users from the live db to stats db.
***********************************************************/


INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Script is starting!!!');

INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Checking for \"Temp\" table!!!');
IF OBJECT_ID('[NeeoDashboard].[dbo].[Temp]') IS NOT NULL 
BEGIN
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Dropping \"Temp\" table!!!');
	DROP TABLE [NeeoDashboard].[dbo].[Temp];
END
BEGIN
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - \"Temp\" table does not exist!!!');
END
INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Copying data into \"Temp\" table!!!');
SELECT * INTO [NeeoDashboard].[dbo].[Temp] FROM [XMPPDb].[dbo].[neUserExtension];
INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Copying data into \"Temp\" table is completed!!!');
IF((SELECT COUNT(1) FROM [NeeoDashboard].[dbo].[Temp]) > 0 )
BEGIN
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Checking for \"neUserExtension\" table!!!');
	IF OBJECT_ID('[NeeoDashboard].[dbo].[neUserExtension]') IS NOT NULL
	BEGIN
		INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Dropping \"neUserExtension\" table!!!');
		DROP TABLE [NeeoDashboard].[dbo].[neUserExtension];
	END
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Copying data into \"neUserExtension\" table!!!');
	SELECT * INTO [NeeoDashboard].[dbo].[neUserExtension] FROM [NeeoDashboard].[dbo].[Temp];
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Dropping \"Temp\" table!!!');
	DROP TABLE  [NeeoDashboard].[dbo].[Temp];
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Script is completed!!!');
END
ELSE
BEGIN
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - No data is in "\Temp\" table!!!');
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Dropping "\Temp\" table!!!');
	DROP TABLE  [NeeoDashboard].[dbo].[Temp];
	INSERT INTO [NeeoDashboard].[dbo].[neLog] ([Log]) VALUES ('[' + (SELECT CAST(GETDATE() AS varchar(30))) + '] - Script is completed!!!');
END

