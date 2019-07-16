
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 05-Mar-2015
-- Description:	It updates the file shared date-time and adds number of recipients to the exisiting one.
-- =============================================
CREATE PROCEDURE spne_UpdateFileSharedDatenRecipientCountByFileID 
	-- Add the parameters for the stored procedure here
	@userID nvarchar(64), 
	@fileID varchar(32),
	@recipientCount tinyint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

	DECLARE @InvalidUserCode int = 50404;
   
	IF EXISTS (SELECT 1 
			   FROM ofUser
			   WHERE username = @userID)
	BEGIN
	
		UPDATE neSharedFile
		SET lastSharedDate = GETUTCDATE(), recipientCount = recipientCount + @recipientCount
		WHERE fileID = @fileID;
	END
	ELSE
		RAISERROR(@InvalidUserCode,-1,-1); 
END
GO
