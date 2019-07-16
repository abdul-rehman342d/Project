SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 01-Oct-2014
-- Description:	Deletes a shared file.
-- =============================================
CREATE PROCEDURE spne_DeleteSharedFileByFileID 
	@fileID varchar(32)

AS
BEGIN
	SET NOCOUNT OFF;
	DELETE FROM neSharedFile
	WHERE fileID = @fileID;
END
GO
