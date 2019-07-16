USE [XMPPDb]
GO
/****** Object:  UserDefinedFunction [dbo].[SplitToTable]    Script Date: 4/7/2014 5:43:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 26/03/2014
-- Description:	Splits the given input string into table with distinct rows based on given delimeter
-- =============================================
CREATE FUNCTION [dbo].[SplitToTable] 
(
	@inputString varchar(max),
	@delimeter varchar(1)
)
RETURNS  @Result TABLE(value varchar(max))
AS
BEGIN
	DECLARE @startingIndex bigint = 0;
	DECLARE @readingCharLength bigint = 0;
	DECLARE @previouslyrReadCharLength bigint = 0;
	DECLARE @currentValue varchar(max);
	DECLARE @empty varchar(1) = '';

	SET @inputString = @inputString + @delimeter;
	WHILE (1=1)
	BEGIN
		SET @readingCharLength = (SELECT CHARINDEX(@delimeter,@inputString, @startingIndex));
		IF (@startingIndex != 0 and @readingCharLength != 0)       
		BEGIN
		--CHARINDEX gives the delimeter index value from the start of the string. So subtracting sum of previously 
		--read characters to get the exact character length to read.
			set @readingCharLength -= @previouslyrReadCharLength ; 
		END
		ELSE IF (@readingCharLength = 0)
		BEGIN
			BREAK;
		END
		SET @currentValue = (SELECT RTRIM(LTRIM(SUBSTRING(@inputString,@startingIndex,@readingCharLength))));
		IF NOT EXISTS(SELECT 1
					  FROM @Result
					  WHERE value = @currentValue) AND @currentValue != @empty
		BEGIN
			INSERT INTO @Result 
			VALUES
			(@currentValue);
		END
		SET @startingIndex += @readingCharLength + 1; -- Including delimeter character.
		SET @previouslyrReadCharLength += @readingCharLength + 1; -- Including delimeter character.
		SET @readingCharLength = 0;
	END
	RETURN;
END 
 