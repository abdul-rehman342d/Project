use Northwind;

EXEC sp_configure 'clr enabled', 1
 go
 RECONFIGURE
 go
EXEC sp_configure 'clr enabled'
 go