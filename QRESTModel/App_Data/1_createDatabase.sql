	--THIS FILE CREATES THE QREST DATABASE


/***************************************************************** */
/*************DROP EXISTING DATABASE (only use if refreshing DB*** */
/***************************************************************** */
/* 
	  EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'QREST'
	  GO
	  USE [master]
	  GO
	  ALTER DATABASE [QREST] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	  GO
	  USE [master]
	  GO
	  DROP DATABASE [QREST]
	  GO
*/

/***************************************************************** */
/*************CREATE DATABASE                                  *** */
/***************************************************************** */

CREATE DATABASE QREST
GO


/************************************************************************* */
/*************CREATE USER AND GRANT RIGHTS******************************** */
/************************************************************************* */
IF EXISTS (SELECT * FROM sys.server_principals WHERE name = N'qrest_login')
DROP LOGIN [qrest_login]

use [QREST]  --ni Azure you may need to manually switch in SSMS

Create login qrest_login with password='B$57WjpN!17h';
EXEC sp_defaultdb @loginame='qrest_login', @defdb='QREST' 
Create user [qrest_user] for login [qrest_login]; 
exec sp_addrolemember 'db_owner', 'qrest_user'; 