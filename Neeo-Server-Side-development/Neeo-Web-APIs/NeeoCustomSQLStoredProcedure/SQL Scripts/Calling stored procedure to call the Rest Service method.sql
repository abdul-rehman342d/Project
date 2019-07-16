Declare @Response NVARCHAR(2000)

EXECUTE Northwind 'http://localhost:90/Service1.svc/Data/8',@Response OUT

SELECT @Response

GO 