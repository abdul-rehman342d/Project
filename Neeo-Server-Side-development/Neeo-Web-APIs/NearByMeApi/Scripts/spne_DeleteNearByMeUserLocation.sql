/****** Object:  StoredProcedure [dbo].[spne_GetUserBlockingStateByPhoneNumber]    Script Date: 3/11/2019 2:05:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 11/03/2019
-- Description:	The following procedure is to delete the NearByMe location for the user.
-- =============================================
CREATE PROCEDURE [dbo].[spne_DeleteNearByMeUserLocation] 
	-- Add the parameters for the stored procedure here
	@indexer bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[neNearByMeUserLocation]
      WHERE [indexer] = @indexer
END
