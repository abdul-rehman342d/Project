
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 08-Jan-2014
-- Description:	Checks the message from group chat or one to one chat
-- =============================================
CREATE FUNCTION IsFromGroup 
(
	-- Add the parameters for the function here
	@jid varchar(100)
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @XMPPDomainKey char(11) = 'xmpp.domain';
	DECLARE @True bit = 1;
	DECLARE @False bit = 0;
	DECLARE @Result bit;
	DECLARE @domain nvarchar(100) = (SELECT propValue
									 FROM ofProperty
									 WHERE name = @XMPPDomainKey);
	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = CASE WHEN CHARINDEX(('@'+ @domain), @jid, 0) = @False THEN @True ELSE @False END;

	-- Return the result of the function
	RETURN @Result

END
GO

