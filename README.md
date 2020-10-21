

![Github logo]()
                                                       

# TECHCareers Capstone Project - myTECHCareers student record management API + frontend

Effective time management is an essential skill for software developers. To entrench this behavior, TECHCareers students are encouraged to keep an accurate record of the time spent on studying, practice problems and assignments. 
With large number of students and simultaneous cohorts, managing student data while relying on spreadsheets is ineffective. Furthermore, there is an opportunity to use timesheet data to gain insights into teaching effectiveness as well as students' understanding of the course content.

It was proposed to replace the existing spreadsheet-based system with a combination of a database backend and a web-based frontend:
- ASP.NET Core Web API and Microsoft SQL Server (Express) will be used as the backend, with the Entity Framework used to create the DB-Object model, which makes the solution relatively easy to adapt to alternative database solutions should it be required. 

- Frontend solution will be entirely based on the React library, with Bootstrap CSS used for application theming. These have been chosen due to popularity, standardisation, and codebase maturity. Frontend solution is intended as a mobile-first platform, with an adaptation for tablet and desktop users. 

## Features / Scope

- Minimum viable product: Service that accurately and reliably captures student timesheets, that is easy to maintain or expand in the future.
- Three-tiered access (Student, Staff, SuperAdmin), implemented through Google OAuth2 JWT token validation and implementation of an AuthorizationHandler in API. Google OAuth was chosen to avoid the need for password management by staff and is convenient as students already have UofA Google-based student mail accounts.
- Management of cohorts, students and staff accounts (mostly at API level)
- Student time management, directly linked to a maintainable record of assignments, categorized by instructional unit and type of assignment (Frontend)
- Student attendance management, linked to cohorts (API level)
- Basic reporting and analytics on student attendance and time management (Future work)

Internals:

- Asynchronous API, HTTPS protocol, 
- API server configuration via config file
- Customized logging interface, either to console or to file, with adjustable logging levels (debug/normal)
- Ability to setup Super Admin user for new installations from command line
- Use of Data Transfer Objects
- Microsoft SQL Server Express Data Storage
- React, Redux and Redux router for the frontend


## Requirements

- For compiling - Visual Studio 2019 with .NET Core 3.1 . Project will not compile for .NET Core 2.2 and below without modifications.
- Microsoft SQL Server Express database
- Apache/Nginx/IIS for frontend deployment, alternatively use of `node.js` with `npm`/`yarn` for testing purposes

## Deployment instructions

The following steps can be used to test deploy this project:

### ASP.NET Core backend

1. Clone this repository into a folder using `git clone` or by downloading a zipped file version from GitHub.
1. Set up Microsoft SQL Server Express and create an empty database.
1. Once set up, execute the SQL script `setup.sql` within the correct database context using a tool such as SSMS. Script can be found in the `Installation` folder in the repository and will create only the required schema. Sample data is included in the `sampledata.sql` script.
1. Use Microsoft Visual Studio to open the solution. If using from development environment, open the `secrets.json` file via "Manage user secrets" command in the Solution Explorer, and change the connection string to match your setup, i.e: 
`"sqldb:ConnectionString": "Server=<server_address>;Database=<database name>;User Id=<username>;Password=<your password here>;MultipleActiveResultSets=true"`
Alternatively, if deploying standalone, add the same line to `appsettings.json` file (at the top level) since `secrets.json` is used exclusively in dev. environment.

If deploying the backend,

1. Modify `appsettings.json` file and change `CORSAllowedOrigins` property to the address of the server hosting the frontend (multiple entries are allowed as a JSON array)
1. Modify `ValidEmailDomains` property to a list of domains from which successful authentication will be accepted (this has to match the domain your Google Auth token will allow).

### React-based frontend

1. Installation of node.js is required for the build (latest stable version can be found [here](https://nodejs.org))
1. Run `npm install` or `yarn` in the folder of choice to download all libraries/dependencies. This will take a while.
1. To test the code locally, start a local webserver by executing `npm start` or `yarn start` in the project folder.
1. To deploy the website on a server, make sure that the correct server location is specified using the `homepage` field in `package.json`.
1. Generate own Google OAuth 2.0 Client ID at [link](https://console.developers.google.com) and paste the client ID into the `.env` file, `GOOGLE_AUTH_CLIENT_ID` key. Make sure that the Authorised Javascript origins match the URI of your React dev. server (incl. port). Authorised redirect URIs should match the same location.

If deploying the frontend:

1. Modify .env file and update the `APIURL` reference to the API server address and port (deployment only)
1. Use `npm build` or `yarn build` to generate a deployment package and upload to target web server.
1. If deployed, it is recommended to keep the API server behind a reverse proxy (Apache, Nginx) to implement SSL encryption and keep the API running on a local interface.

## Testing instructions 

List of test instructions is provided in the `Testplan` folder in the repository.

## Trello project board

The project board can be found at the following [link](https://trello.com/b/Ri0WfHqA/capstone-project)

## Citations

### Authentication

1. Authentication and authorization topics from "ASP.NET Core 3 and React", Carl Rippon, Pakt Publishing 2019 (ISBN: 9781789950229)
1. Add host address from localsettings.json: [link](https://stackoverflow.com/a/41330941/12802214)
1. CORS configuration: [link](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1)
1. Dealing with JWTBearer events: [link](https://stackoverflow.com/a/50451116/12802214)
1. Extending Identity class and methods: [link](https://stackoverflow.com/a/40655593/12802214)

### App configuration, startup and logging

1. Application shutdown handling: [link](https://thinkrethink.net/2017/03/09/application-shutdown-in-asp-net-core/)
1. Command line options handling: [link](https://github.com/commandlineparser/commandline) , [link](https://github.com/commandlineparser/commandline/wiki)
1. IConfigurationRoot implementation: [link](https://stackoverflow.com/a/41738816/12802214)
1. Serilog configuration: [link](https://github.com/serilog/serilog-aspnetcore)

### Databases, Entity framework, WebAPI

1. Crow notation and ERD diagrams: [link](http://www2.cs.uregina.ca/~bernatja/crowsfoot.html#_msocom_11)
1. SQL Server data types: [link](https://docs.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql?view=sql-server-ver15)
1. Recommended length of an email field in DB: [link](https://dba.stackexchange.com/questions/37014/in-what-data-type-should-i-store-an-email-address-in-database)
1. Add Context with SQL Server connection with parameters from appsettings.json: [link](https://elanderson.net/2019/11/entity-framework-core-no-database-provider-has-been-configured-for-this-dbcontext/)
1. JSON timespan deserialization helper: [link](https://stackoverflow.com/a/58284103/12802214)
1. Decimal data type and issues with precision in EF: [link](https://www.computerworld.com/article/2909612/working-with-decimal-precision-in-net-with-mssql-server-and-entity-framework.html)
1. Precision and scale for decimal datatypes: [link](https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=fluent-api%2Cwithout-nrt#precision-and-scale)
1. Guide to choosing HTTP methods: [link](https://www.w3schools.com/tags/ref_httpmethods.asp)
1. Entity framework logging infrastructure: [link](https://elanderson.net/2018/10/entity-framework-core-logging/)
1. Getting related data using EF: [link](https://docs.microsoft.com/en-us/ef/core/querying/related-data/)

### General C# references

1. Passing class as method parameter: [link](https://stackoverflow.com/questions/18806579/how-to-pass-a-class-as-parameter-for-a-method)
1. Getting a base class of an object: [link](https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype?view=netcore-3.1)
1. Practical uses for the internal classes/methods: [link](https://stackoverflow.com/questions/165719/practical-uses-for-the-internal-keyword-in-c-sharp)
1. Regex for testing for a valid email address: [link](https://stackoverflow.com/a/45177249/12802214)

