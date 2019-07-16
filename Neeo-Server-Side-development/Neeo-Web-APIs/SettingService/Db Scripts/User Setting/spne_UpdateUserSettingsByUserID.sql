
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 29-Sep-2014
-- Description:	Updates the im tone setting for the provided user.
-- =============================================
CREATE PROCEDURE spne_UpdateUserSettingsByUserID 
	-- Add the parameters for the stored procedure here
	@userID nvarchar(64), 
	@imTone tinyint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	UPDATE neUserExtension
	SET imTone = @imTone
	WHERE username = @userID;
END
GO