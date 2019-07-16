-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 13-Jan-2015
-- Modification
-- [03-Feb-2015] - Updated to reset the offline count and mcr count.[zohaib]
-- Description:	Resets the user offline message count.
-- =============================================
CREATE PROCEDURE spne_ResetOffineMessageCountByUserID 
	-- Add the parameters for the stored procedure here
	@userID nvarchar(64) = 0,
	@offlineCountType tinyint,

	--Constants
	@OfflineCount tinyint= 1,
	@McrCount tinyint = 2
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	DECLARE @InvalidUserCode int = 50404;
	SET NOCOUNT OFF;

	IF EXISTS (SELECT 1 FROM ofUser
			   WHERE username = @userID)
	BEGIN
		IF (@offlineCountType = @OfflineCount)
			UPDATE neOfflineUserMessageCount
			SET messageCount = 0
			WHERE username = @userID;  
		ELSE IF (@offlineCountType = @McrCount)
			UPDATE neOfflineUserMessageCount
			SET mcrCount = 0
			WHERE username = @userID;
	END
	ELSE
	BEGIN
		RAISERROR(@InvalidUserCode,-1,-1);
	END;
	
END
GO
