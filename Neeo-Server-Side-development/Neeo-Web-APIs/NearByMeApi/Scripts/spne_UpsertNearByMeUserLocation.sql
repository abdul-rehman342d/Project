/****** Object:  StoredProcedure [dbo].[spne_GetUserBlockingStateByPhoneNumber]    Script Date: 3/11/2019 2:05:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 11/03/2019
-- Description:	The following procedure is to insert/update the NearByMe location for the user.
-- =============================================
CREATE PROCEDURE [dbo].[spne_UpsertNearByMeUserLocation] 
	-- Add the parameters for the stored procedure here
	@indexer bigint,
	@username nvarchar(64),
    @latitude float,
    @longitude float
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @geoLocation geography = geography::Point(@latitude, @longitude, 4326);

	IF NOT EXISTS (SELECT 1 
			   FROM [dbo].[neNearByMeUserLocation] 
			   WHERE [indexer] = @indexer)
	BEGIN
		INSERT INTO [dbo].[neNearByMeUserLocation]
           ([indexer]
           ,[username]
           ,[latitude]
           ,[longitude]
           ,[geoLocation])
		VALUES
           (@indexer
           ,@username
           ,@latitude
           ,@longitude
           ,@geoLocation);
	END
	ELSE
	BEGIN
		UPDATE [dbo].[neNearByMeUserLocation]
		SET [latitude] = @latitude
		   ,[longitude] = @longitude
		   ,[geoLocation] = @geoLocation
		 WHERE 
			[indexer] = @indexer;
	END
END
