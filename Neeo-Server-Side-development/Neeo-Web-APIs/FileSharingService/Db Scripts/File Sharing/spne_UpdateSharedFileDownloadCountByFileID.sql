SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 01-Oct-2014
-- Description:	Updates the download count of a shared file.
-- =============================================
CREATE PROCEDURE spne_UpdateSharedFileDownloadCountByFileID 
	@userID nvarchar(64),
	@fileID varchar(32)

AS
BEGIN
	DECLARE @InvalidUserCode int = 50404;
	SET NOCOUNT OFF;
	IF EXISTS (SELECT 1 FROM ofUser
			   WHERE username = @userID)
	BEGIN
		UPDATE [dbo].[neSharedFile]
		SET downloadCount = downloadCount + 1
		WHERE fileID = @fileID;  
	END
	ELSE
	BEGIN
		RAISERROR(@InvalidUserCode,-1,-1);
	END;
END
GO
