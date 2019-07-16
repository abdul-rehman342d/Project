/****** Object:  StoredProcedure [dbo].[spne_GetUserBlockingStateByPhoneNumber]    Script Date: 3/11/2019 2:05:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 11/03/2019
-- Description:	The following procedure is to insert/update the NearByMe settings for the user.
-- =============================================
ALTER PROCEDURE [dbo].[spne_UpsertNearByMeSetting] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64),
    @enabled bit,
    @notificationTone tinyint,
    @notificationOn bit,
    @showInfo bit,
    @showProfileImage bit,
	@isPrivateAccount bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

	BEGIN TRY
		EXEC spne_IsUserExist @username;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH

	IF NOT EXISTS (SELECT 1 
			   FROM [dbo].[neNearByMeSetting] 
			   WHERE [username] = @username)
	BEGIN
		INSERT INTO [dbo].[neNearByMeSetting]
           ([username]
           ,[enabled]
           ,[enableDate]
           ,[notificationTone]
           ,[notificationOn]
           ,[showInfo]
           ,[showProfileImage]
           ,[createDate]
		   ,[isPrivateAccount])
     VALUES
           (@username
           ,@enabled
           ,CASE WHEN @enabled = 1 THEN GETUTCDATE() END
           ,@notificationTone
           ,@notificationOn
           ,@showInfo
           ,@showProfileImage
           ,GETUTCDATE()
		   ,@isPrivateAccount);
	END
	ELSE
	BEGIN
		DECLARE @indexer bigint;
		DECLARE @isNearByMeEnabled bit;

		SELECT @indexer = indexer, @isNearByMeEnabled = [enabled] 
		FROM [dbo].[neNearByMeSetting] 
		WHERE [username] = @username;

		--IF (@isNearByMeEnabled = 1 AND @enabled = 0)
		--BEGIN
		--	EXEC spne_DeleteNearByMeUserLocation @indexer;
		--END

		UPDATE [dbo].[neNearByMeSetting]
		SET  [enabled] = @enabled
			,[enableDate] = CASE WHEN [enabled] = 0 AND @enabled = 1 THEN GETUTCDATE() ELSE [enableDate] END
			,[updateDate] = GETUTCDATE()
			,[notificationTone] = @notificationTone
			,[notificationOn] = @notificationOn
			,[showInfo] = @showInfo
			,[showProfileImage] = @showProfileImage
			,[isPrivateAccount] = @isPrivateAccount
		WHERE 
		[username] = @username;

	END
END
