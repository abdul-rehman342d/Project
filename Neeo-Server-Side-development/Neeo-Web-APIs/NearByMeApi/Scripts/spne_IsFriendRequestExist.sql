/****** Object:  StoredProcedure [dbo].[spne_InsertSharedFileInformationByUserID]    Script Date: 3/11/2019 2:09:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 11-03-2019
-- Description:	It checks whether friend request exists or not.
-- =============================================
ALTER PROCEDURE [dbo].[spne_IsFriendRequestExist]
	@username nvarchar(64),
	@friendUId nvarchar(64),
	@id bigint output,
	@status int output,
	@senderId nvarchar(64) output,
	@recipientId nvarchar(64) output
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		EXEC spne_IsUserExist @username;
		EXEC spne_IsUserExist @friendUId;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH

	SELECT @id = rosterID,
		   @status = [status],
		   @senderId = username,
		   @recipientId = fid 
	FROM [dbo].[neNearByMeRoster]
	WHERE ((username = @username AND fid = @friendUId) OR (username = @friendUId AND fid = @username));
END
