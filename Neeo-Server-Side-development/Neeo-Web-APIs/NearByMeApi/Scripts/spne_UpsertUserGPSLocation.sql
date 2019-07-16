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
CREATE PROCEDURE [dbo].[spne_UpsertUserGPSLocation] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64),
	@latitude float,
    @longitude float
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @indexer bigint;

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
           ([username],
			[enabled],
		    [createDate])
		VALUES
           (@username,
		    1,
		    GETUTCDATE());
	END
	
	--SET @indexer = SCOPE_IDENTITY();
	SELECT @indexer = indexer --, @isNearByMeEnabled = [enabled] 
	FROM [dbo].[neNearByMeSetting] 
	WHERE [username] = @username;

	--IF @isNearByMeEnabled = 0
	--BEGIN
	--	RETURN;
	--END

	EXEC spne_UpsertNearByMeUserLocation @indexer, @username, @latitude, @longitude;
END
