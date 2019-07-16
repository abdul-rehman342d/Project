USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_GetAppVersionByContactList]    Script Date: 11/26/2014 3:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Usman Saeed
-- Create date: 11/19/2014 (MM/dd/yyyy)
-- Updated by : Zohaib Hanif
-- Modification date : 11/26/2014 (MM/dd/yyyy)
-- Description:	It returns the App version and devicePlatform of multiple users. 
-- =============================================

CREATE PROCEDURE [dbo].[spne_GetAppVersionByContactList] 
	-- Add the parameters for the stored procedure here
		@userID nvarchar(64),
	    @contacts nvarchar(max)

AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @InvalidUserCode int = 50404;
	DECLARE @delimeter char(1) = ',';
	DECLARE @contactsTable  table (contact nvarchar(64));

	IF EXISTS (SELECT 1 FROM ofUser
			   WHERE username = @userID)
    BEGIN
		INSERT INTO @contactsTable (contact)
			SELECT * FROM [dbo].[SplitToTable](@contacts, @delimeter); 

		SELECT username AS contact, CAST(devicePlatform AS varchar(1)) + '-' + 
			CASE WHEN appVersion != '' THEN appVersion ELSE 
				CASE WHEN devicePlatform = 1 THEN 'NEEO-2.0.0' ELSE 'Neeo-1.0.0' END
			END AS userVersion
		From neUserExtension 
		WHERE username IN (SELECT contact 
						   FROM @contactsTable);
	END
	ELSE
	BEGIN
		RAISERROR(@InvalidUserCode,-1,-1);
	END;

END

