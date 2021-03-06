USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_GetUserInfoByUserID]    Script Date: 12/22/2014 12:55:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Usman Saeed
-- Create date: 11/23/2014 (MM/dd/yyyy)
-- Description:	It returns the Device VenderID and application Id of a single user. 
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetUserInfoByUserID] 
	-- Add the parameters for the stored procedure here
		@userID nvarchar(64)

AS
BEGIN
	DECLARE @InvalidUserCode int = 50404;
	SET NOCOUNT Off;
	IF EXISTS (SELECT 1 FROM ofUser
			   WHERE username = @userID)

    BEGIN
	SELECT deviceVenderID,applicationID
    From neUserExtension 
    WHERE username =@userID;
	END
	ELSE
	BEGIN
		RAISERROR(@InvalidUserCode,-1,-1);
	END;



	END
	
