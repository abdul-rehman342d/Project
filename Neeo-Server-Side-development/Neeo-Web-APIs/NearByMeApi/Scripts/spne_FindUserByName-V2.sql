ALTER PROCEDURE [dbo].[spne_FindUserByName]
	 @searchText nvarchar(100),
     @username nvarchar(64),
	 @latitude float,
     @longitude float,
	 @isCurrentLocation bit
 AS
 BEGIN
		SET NOCOUNT ON;

		BEGIN TRY
			EXEC spne_IsUserExist @username;
		END TRY
		BEGIN CATCH
			THROW 
		END CATCH

		CREATE TABLE #SearchUsers ([username] [nvarchar](64),
								   [latitude] [float],
								   [longitude] [float], 
								   [showInfo] [bit], 
								   [showProfileImage] [bit], 
								   [isPrivateAccount] [bit], 
								   [name] [nvarchar](100),
								   [status] int
								   );

		INSERT INTO #SearchUsers
		EXEC spne_GetNearByMeUserByLocation @username, @latitude, @longitude, @isCurrentLocation;
		
		SELECT	name,
				username [uid],
				latitude,
				longitude,
				isPrivateAccount,
				[status]
		FROM	#SearchUsers 
		WHERE	name like '%' + @SearchText + '%'
		ORDER BY name;
END
GO
