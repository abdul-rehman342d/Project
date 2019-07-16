/****** Object:  StoredProcedure [dbo].[spne_GetUserBlockingStateByPhoneNumber]    Script Date: 3/11/2019 2:05:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 11/03/2019
-- Description:	The following procedure is to get user by their location.
-- =============================================
ALTER PROCEDURE [dbo].[spne_GetNearByMeUserByLocation] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64),
    @latitude float,
    @longitude float,
	@isCurrentLocation bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @userGeoLocation geography = geography::Point(@latitude, @longitude, 4326);
	DECLARE @indexer bigint;
	DECLARE @isNearByMeEnabled bit;
	DECLARE @radiusInKm int = 50000; -- 50 km

	BEGIN TRY
		EXEC spne_IsUserExist @username;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH
	
	SELECT @indexer = indexer, @isNearByMeEnabled = [enabled] 
	FROM [dbo].[neNearByMeSetting] 
	WHERE [username] = @username;

	--IF @isNearByMeEnabled = 0
	--BEGIN
	--	RETURN;
	--END

	IF @isCurrentLocation = 1
	BEGIN
		EXEC spne_UpsertNearByMeUserLocation @indexer, @username, @latitude, @longitude;
	END

	--Fetch rejected users for this users
	SELECT [username]
	INTO #RejectedUser
	FROM [dbo].[neNearByMeRoster]
	WHERE [fid] = @username AND [status] = 2
	UNION
	SELECT [fid]
	FROM [dbo].[neNearByMeRoster]
	WHERE [username] = @username AND [status] = 2;-- Rejected 

	--Fetch rejected users for this users
	SELECT [username], [status]
	INTO #PendingAcceptedUser
	FROM [dbo].[neNearByMeRoster]
	WHERE [fid] = @username AND [status] <> 2
	UNION
	SELECT [fid], [status]
	FROM [dbo].[neNearByMeRoster]
	WHERE [username] = @username AND [status] <> 2;-- not Rejected 

	

	SELECT S.[username]
		  ,[latitude]
          ,[longitude]
		  ,ISNULL([showInfo], 0) [showInfo]
		  ,ISNULL([showProfileImage], 1) [showProfileImage]
		  ,ISNULL([isPrivateAccount], 0) [isPrivateAccount]
		  ,U.[name]
		  ,ISNULL(A.[status], -1) [status]
	FROM [dbo].[neNearByMeUserLocation] UL
	INNER JOIN [dbo].[ofUser] U ON UL.username = U.username
	LEFT OUTER JOIN [dbo].[neNearByMeSetting] S ON UL.indexer = S.indexer
	LEFT OUTER JOIN #RejectedUser RU ON U.username = RU.username
	LEFT OUTER JOIN #PendingAcceptedUser A ON U.username = A.username
	WHERE 
		RU.username IS NULL AND 
		UL.[indexer] <> @indexer AND S.[enabled] = 1 AND GeoLocation.STDistance(@userGeoLocation ) <= @radiusInKm;

	DROP TABLE #RejectedUser, #PendingAcceptedUser;
END
