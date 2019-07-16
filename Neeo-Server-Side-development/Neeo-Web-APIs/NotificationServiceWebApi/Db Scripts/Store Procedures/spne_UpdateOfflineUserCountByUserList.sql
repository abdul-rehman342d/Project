SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 29-Dec-2014
-- Description:	Updates the offline message count for the given users.
-- =============================================
CREATE PROCEDURE spne_UpdateOfflineUserCountByUserList
	-- Add the parameters for the stored procedure here
    @userList nvarchar(max),
	@delimeter varchar(1)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE neOfflineUserMessageCount 
	SET neOfflineUserMessageCount.messageCount = neOfflineUserMessageCount.messageCount + 1
	WHERE username IN (SELECT * FROM SplitToTable (@userList, @delimeter));
    
END
GO

