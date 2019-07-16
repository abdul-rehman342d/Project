-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 16/02/2014
-- Description:	Delete the user's blocked state from the neBlockedUser
-- =============================================
CREATE PROCEDURE spne_DeleteUserBlockedStateByPhoneNumber
	-- Add the parameters for the stored procedure here
	@phoneNumber nvarchar(64)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[neBlockedUser]
	WHERE phoneNumber = @phoneNumber;
END
GO
