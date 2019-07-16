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
ALTER PROCEDURE [dbo].[spne_UpsertFriendRequest] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64),
	@friendUId nvarchar(64),
	@status int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @requestId bigint;
	DECLARE @requestStatus int;
	DECLARE @recipientId nvarchar(64) = null;
	DECLARE @senderId nvarchar(64) = null;
	DECLARE @RecipientNearByMePrivateAccountType bit = null; 

	BEGIN TRY
		EXEC spne_IsUserExist @username;
		EXEC spne_IsUserExist @friendUId;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH

	EXEC spne_IsFriendRequestExist @username, @friendUId, @requestId output, @requestStatus output, @senderId output, @recipientId output;

	IF (@requestId IS NULL)
	BEGIN
		
		-- Check the account type if it is public accept the request.

		SELECT @RecipientNearByMePrivateAccountType = ISNULL(isPrivateAccount, 0)
		FROM [dbo].neNearByMeSetting
		WHERE username = @friendUId;

		IF @RecipientNearByMePrivateAccountType = 0
		BEGIN
			SET @status = 1;
		END

		INSERT INTO [dbo].[neNearByMeRoster]
           ([username]
           ,[fid]
           ,[status]
           ,[createdDate])
     VALUES
           (@username
           ,@friendUId
           ,@status
           ,GETUTCDATE())
	END
		
	IF @senderId = @username 
	BEGIN
		RETURN;
	END 

	IF @username = @recipientId AND @requestStatus = 0 AND @status = 0
	BEGIN
		SET @status = 1;
	END

	UPDATE [dbo].[neNearByMeRoster]
    SET [status] = CASE WHEN @requestStatus = 0 
						THEN  @status
						ELSE [status] 
						END
					,[updatedDate] = GETUTCDATE()
    WHERE 
		rosterID = @requestId;
END
