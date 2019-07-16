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
CREATE PROCEDURE [dbo].[spne_GetFriendRequestDetails] 
	-- Add the parameters for the stored procedure here
	@username nvarchar(64)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		EXEC spne_IsUserExist @username;
	END TRY
	BEGIN CATCH
		THROW 
	END CATCH


	SELECT [username] [uId]
		,[status]
	INTO #FriendList
	FROM [dbo].[neNearByMeRoster] 
	WHERE fid = @username AND [status] IN (0, 2)
	UNION
	SELECT [username] [uId]
		,[status]
	FROM [dbo].[neNearByMeRoster] 
	WHERE fid = @username AND [status] = 1
	UNION
	SELECT [fid] [uId]
		,[status]
	FROM [dbo].[neNearByMeRoster] 
	WHERE username = @username AND [status] = 1;


	SELECT F.[uId]
		  ,ISNULL([latitude], 0)
          ,ISNULL([longitude], 0)
		  ,ISNULL([showInfo], 0) [showInfo]
		  ,ISNULL([showProfileImage], 1) [showProfileImage]
		  ,ISNULL([isPrivateAccount], 0) [isPrivateAccount]
		  ,U.[name]
		  ,F.[status]
	FROM #FriendList F
	INNER JOIN [dbo].[ofUser] U ON F.[uId] = U.username
	LEFT OUTER JOIN [dbo].[neNearByMeUserLocation] UL ON UL.username = F.[uId]
	LEFT OUTER JOIN [dbo].[neNearByMeSetting] S ON F.[uId] = S.username
	
	DROP TABLE #FriendList;
END
