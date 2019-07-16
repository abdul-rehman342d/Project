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
-- Description:	Checks the user's existance as an application user.
-- =============================================
CREATE PROCEDURE spne_CheckUserExistanceByPhoneNumber
	-- Add the parameters for the stored procedure here
	@phoneNumber nvarchar(64), 
	@userExists bit OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS (SELECT 1
			   FROM [dbo].[ofUser]
			   WHERE username = @phoneNumber)
		SET @userExists = 1;		  
	ELSE
		SET @userExists = 0;
END
GO
