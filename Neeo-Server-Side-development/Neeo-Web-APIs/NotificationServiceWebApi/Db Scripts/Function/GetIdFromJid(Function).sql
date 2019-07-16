-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 07-Jan-2015
-- Description:	Gets user id from jid
-- =============================================
CREATE FUNCTION GetIdFromJid 
(
	-- Add the parameters for the function here
	@jid nvarchar(500)
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result nvarchar(100)
	DECLARE @delimeter char(1) = '@';
	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = SUBSTRING(@jid,0,CHARINDEX(@delimeter, @jid, 0));

	-- Return the result of the function
	RETURN @Result

END
GO

