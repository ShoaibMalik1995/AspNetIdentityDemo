----******************* Authentication & Authorization (ASP.NET CORE) ************************--------------
https://www.youtube.com/watch?v=feuDV9eyqoI&list=PLFJQnCcZXWjuHP03Kgf46FrX5L2fRzDsx&index=3 (Tutorial Demo)

-> Install Required Packages:
0- Microsoft.AspNetCore.Identity
1- Microsoft.AspNetCore.Identity.EntityFrameworkCore
2- Microsoft.EntityFrameworkCore.SqlServer
3- Microsoft.EntityFrameworkCore.Tools
4- Microsoft.AspNetCore.Authentication.JwtBearer
5- Microsoft.AspNetCore.WebUtilities

Database Migrations Commands:
1- Add-Migration InitialDatabase
2- Update-Database

***Default Table Of AspNetIdentity***
SELECT * FROM AspNetRoleClaims 
SELECT * FROM AspNetRoles
SELECT * FROM AspNetUserClaims
SELECT * FROM AspNetUserLogins
SELECT * FROM AspNetUserRoles
SELECT * FROM AspNetUsers
SELECT * FROM AspNetUserTokens


-----********************** SendGrid Email Confirmation ****************************---------
install Package :
1- SendGrid


