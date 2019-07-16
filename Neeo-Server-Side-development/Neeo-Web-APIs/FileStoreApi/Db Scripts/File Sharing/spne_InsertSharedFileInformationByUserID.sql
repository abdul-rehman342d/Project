SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 01-Oct-2014
-- Description:	Inserts a new record about shared file in the table
-- =============================================
CREATE PROCEDURE spne_InsertSharedFileInformationByUserID
	@ownerJID nvarchar(64),
	@mediaType tinyint,
	@mimeType tinyint,
	@creationDate smalldatetime,
	@recipientCount tinyint,
	@fileID varchar(32),
	@fullPath text
AS
BEGIN
	DECLARE @InvalidUserCode int = 50404;
	SET NOCOUNT OFF;
	IF EXISTS (SELECT 1 
			   FROM ofUser
			   WHERE username = @ownerJID)
	BEGIN
		INSERT INTO [dbo].[neSharedFile]
			   ([ownerJID]
			   ,[mediaType]
			   ,[mimeType]
			   ,[creationDate]
			   ,[recipientCount]
			   ,[fileID]
			   ,[fullPath])
		 VALUES
			   (@ownerJID,
				@mediaType,
				@mimeType,
				@creationDate,
				@recipientCount,
				@fileID,
				@fullPath);
	END
	ELSE
	BEGIN
		RAISERROR(@InvalidUserCode,-1,-1);
	END;
END
GO
