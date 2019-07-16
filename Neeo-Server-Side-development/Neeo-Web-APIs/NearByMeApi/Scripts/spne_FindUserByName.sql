ALTER PROCEDURE [dbo].[spne_FindUserByName]
	 @searchText nvarchar(100),
     @username nvarchar(64),
	 @pageNumber int,
	 @pageSize   int,
	 @pageTotal  int = 0 output
 AS
 BEGIN
		SET NOCOUNT ON;
		DECLARE @TotalRecords bigint = 0;

		BEGIN TRY
			EXEC spne_IsUserExist @username;
		END TRY
		BEGIN CATCH
			THROW 
		END CATCH

		SET @TotalRecords = (SELECT COUNT(1) FROM [dbo].[ofUser] WHERE name like '%' + @searchText + '%');

		IF(@TotalRecords > 0)
		BEGIN
			SET @pageTotal = @TotalRecords / @pageSize;

			IF((@TotalRecords % @pageSize) > 0)
			BEGIN
				SET @pageTotal = @pageTotal + 1;
			END
		END
		ELSE
		BEGIN
			SET @pageTotal = 0;
		END

		SELECT	name,
				U.username [uid],
				L.latitude,
				L.longitude
		FROM	[dbo].[ofUser] U
				LEFT OUTER JOIN [dbo].[neNearByMeUserLocation] L ON U.username = L.username
		WHERE	name like '%' + @SearchText + '%'
		ORDER BY name
		OFFSET @pageSize * (@pageNumber - 1) ROWS
		FETCH NEXT @pageSize ROWS ONLY OPTION (RECOMPILE);
END
GO
