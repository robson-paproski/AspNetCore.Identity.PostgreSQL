# AspNetCore Identity Provider for PostgreSQL

This is a preview of Identity for PostgreSQL using the last .Net Core

Follow the steps below to add to your project

1. Add the AspNetCore.Identity.PostgreSQL project as reference to your main MVC project. Also available on Nuget

2. In your MVC project, replace all references from "using Microsoft.AspNetCore.Identity.EntityFrameworkCore;" with "using AspNetCore.Identity.PostgreSQL;".

3. You should remove every reference to entity framework usage for identity.

4. In this version, the string connection is taken by config.json file, containing  
   "  
   {  
     "DefaultConnection": "Server = xxx; Port = 5432; Database = db; User Id = postgres; Password = pass"  
   }"  

5. If you were using entity framework for identity, in your startup.cs class remove the line   
   .AddEntityFrameworkStores\<ApplicationDbContext\>() 
   below  
   services.AddIdentity\<ApplicationUser, IdentityRole\>()"

6. In your startup class, add the following lines bellow services.AddIdentity\<ApplicationUser, IdentityRole\>()  
   .AddUserStore\<UserStore\<ApplicationUser\>\>()  
   .AddRoleStore\<RoleStore\<IdentityRole\>\>()

7. Last but not least, execute the SQL script found in the solution against your database in pgAdmin (or via the command line).


This repo also contains a test project. You just need to modifiy the config.json to connect to your database, and exec the SQL script included.

If you have any questions feel free to open an issue
