/****** Object:  StoredProcedure [dbo].[spne_GetUserBlockingStateByPhoneNumber]    Script Date: 3/11/2019 2:05:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 26/04/2019
-- Description:	The following procedure is to update the GPS location in database.
-- =============================================
CREATE PROCEDURE [dbo].[spne_DeleteFriendRequest] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64),
	@friendUId nvarchar(64)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @requestId bigint;
	DECLARE @requestStatus int;
	DECLARE @recipientId nvarchar(64) = null;
	DECLARE @senderId nvarchar(64) = null;

	BEGIN TRY
		EXEC spne_IsUserExist @username;
		EXEC spne_IsUserExist @friendUId;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH

	EXEC spne_IsFriendRequestExist @username, @friendUId, @requestId output, @requestStatus output, @senderId output, @recipientId output;

	DELETE 
	FROM [dbo].[neNearByMeRoster]
    WHERE 
		rosterID = @requestId;
END
