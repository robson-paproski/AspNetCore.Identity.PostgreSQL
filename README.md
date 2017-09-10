# AspNetCore Identity Provider for PostgreSQL

Identity for PostgreSQL using the .Net Core 2.0

## Installation

Install as [NuGet package](https://www.nuget.org/packages/AspNetCore.Identity.PostgreSQL.dll/):

```
Install-Package AspNetCore.Identity.PostgreSQL.dll
```

## Usage

Follow the steps below to add to your project

1. Add 
```services.AddSingleton(_ => Configuration);``` inside the scope of ConfigureServices(IServiceCollection services) in your Startup.cs class. The identity gonna use the default Dependency Injection of .net core to use your connectionId from your environment specific file.

2. In your appsetings.Development and .Production, you should add a PostgreSQLBaseConnection string.
 ```  
   {
  "ConnectionStrings": {
    "PostgreSQLBaseConnection": "Server = SERVER; Port = 5432; Database = DB; User Id = USER; Password = PASSWORD; Pooling=false; Keepalive=10"
  }
}
```
3. In your ApplicationUser class inside Models folder, change ```using Microsoft.AspNetCore.Identity;``` to ```using AspNetCore.Identity.PostgreSQL;```

5. In your startup.cs class remove any reference to the ApplicationDbContext and the line   
   ```.AddEntityFrameworkStores<ApplicationDbContext>() ```
   below  
   services.AddIdentity\<ApplicationUser, IdentityRole\>()"

6. In your startup class, add the following lines bellow services.AddIdentity\<ApplicationUser, IdentityRole\>()  
   ```.AddUserStore<UserStore<ApplicationUser>>()  
   .AddRoleStore<RoleStore<IdentityRole>>()```

7. In your startup class, add ```using IdentityRole = AspNetCore.Identity.PostgreSQL.IdentityRole;``` in the using section.


8. Last but not least, execute the SQL script found in the solution items against your database in pgAdmin (or via the command line).


After all done, maybe appears some Conversions Guid to String errors. Just put .ToString after User.Id and that's it. That occurours 


This repo also contains a test project. You just need to modifiy the appSettings..json to connect to your database, and exec the SQL script included.

To configure the name of connection string that will be used, you can put ```IdentityDbConfig.StringConnectionName= "ConnectionStringName";``` in your Startup.cs

## For a brand new project

You should modify program.cs to include the appsettings.Development config file.

``` public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .Build();
```

If you have any questions feel free to open an issue
