
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 16/02/2014
-- Description:	Update the user's information by user's phone number
-- =============================================
CREATE PROCEDURE [dbo].[spne_UpdateUserNameByPhoneNumber]  
	-- Add the parameters for the stored procedure here
	@phoneNumber nvarchar(64), 
	@name nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	UPDATE [dbo].[ofUser]
    SET [name] = @name
    WHERE username = @phoneNumber;
END
GO
