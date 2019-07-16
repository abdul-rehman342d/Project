SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 10-Jun-2015
-- Description:	Gets the last seen time of the given user.
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetLastSeenTimeByUserIDnResource] 
	@userID nvarchar(64),
	@resource nvarchar(100),
	@online bit output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @InvalidUserCode int = 50404;
	
	IF EXISTS (SELECT 1 FROM ofUser
			   WHERE username = @userID)
	BEGIN
		SELECT lastLogoffDate
		FROM userStatus 
		WHERE username = @userID AND resource = @resource;

		SELECT @online = userStatus.online
		FROM userStatus 
		WHERE username = @userID AND resource = @resource;
	END
	ELSE
		RAISERROR(@InvalidUserCode,-1,-1);
END
GO
