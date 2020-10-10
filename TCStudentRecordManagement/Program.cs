using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TCStudentRecordManagement.Utils;
using Serilog;
using Serilog.Events;
using System.IO;
using CommandLine;
using System.Collections.Generic;
using Serilog.Sinks.SystemConsole.Themes;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CommandLine.Text;
using System.Reflection;

namespace TCStudentRecordManagement
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // Parse command line options. Configuration based on CommandLine library documentation/wiki:
            // Ref: https://github.com/commandlineparser/commandline/wiki
            Parser.Default.ParseArguments<StartupOptions>(args)
                  .WithParsed(options => RunServer(options, args))
                  .WithNotParsed(errors => ParseErrors(errors));

        }

        static void RunServer(StartupOptions options, string[] args)
        {
            // Serilog configuration
            // Based on documentation and examples at: https://github.com/serilog/serilog-aspnetcore
            LoggerConfiguration LogConfig = new LoggerConfiguration();

            // Process command line parameters

            // Debug
            if (options.Debug)
            {
                LogConfig.MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                    .MinimumLevel.Override("TCStudentRecordManagement", LogEventLevel.Debug)
                    .MinimumLevel.Override("TCStudentRecordManagement.Controllers", LogEventLevel.Debug)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Debug)
                    .Enrich.FromLogContext();
            }
            else
            {
                LogConfig.MinimumLevel.Error()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("TCStudentRecordManagement", LogEventLevel.Information)
                .MinimumLevel.Override("TCStudentRecordManagement.Controllers", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext();
            }

            // Log destination.
            // TODO reverse to default with console log being an option in the final version
            if (options.LogFile != null)
            {
                LogConfig.WriteTo.File(options.LogFile.FullName);
            }
            else
            {
                LogConfig.WriteTo.Console(theme: AnsiConsoleTheme.Code);
            }

            // Check if Super Admin is to be added to the DB
            if (options.CreateAdmin != string.Empty)
            {
                // Initialize the logger for the Super Admin addition 'session'
                //Log.Logger = LogConfig.CreateLogger();
                //CreateSuperAdmin(options.CreateAdmin);
            }

            // TODO See above - reverse for file logging being default
            // if (options.Console)

            // Show TECHCareers logo
            Logo.printLogo();
            Console.WriteLine("TECHCareers Student Record Management API\n");

            // Copyright from assembly info
            // Ref: https://stackoverflow.com/a/19384288/12802214
            // Console.WriteLine("(C) " + ((AssemblyCopyrightAttribute)typeof(Program).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0]).Copyright);

            // Initialize the logger for normal operation
            Log.Logger = LogConfig.CreateLogger();

            try
            {
                if (options.Debug) { Logger.Msg<Program>("Debug mode active", LogEventLevel.Debug); }

                Logger.Msg<Program>("Starting API interface...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Logger.Msg<Program>("Unexpected service termination.", LogEventLevel.Fatal);
                Logger.Msg(ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        internal static void ParseErrors(IEnumerable<Error> errors)
        {
            errors.ToList().ForEach(ex => Console.WriteLine(ex.Tag));
        }

        internal static void CreateSuperAdmin(string paramEmail)
        {
            // TODO: Email address validation check!!


            // Create a local IConfigurationRoot object to read data from appsettings.json and secrets.json associated with the project
            // Ref: https://stackoverflow.com/a/41738816/12802214
            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).AddUserSecrets<Startup>().Build();

            // Create a connection to the DB 
            using (SqlConnection dbConn = new SqlConnection(config.GetSection("sqldb:ConnectionString").Value))
            {
                dbConn.Open();
                SqlCommand sqlCmd = dbConn.CreateCommand();

                sqlCmd.CommandText = $"SELECT [StaffID], staff.[UserID], [SuperUser], [Email], [Active] from staff LEFT JOIN users on staff.UserID = users.UserID where users.Email = '{paramEmail}'";
                // Based on query above, column indices are StaffID = 0, UserID = 1, SuperUser = 2, Email = 3, Active = 4

                // Check if user exists
                SqlDataReader response = sqlCmd.ExecuteReader();
                if (response.HasRows)
                {
                    response.Read();
                    // Get the StaffID and UserID from the record
                    int StaffID = response.GetInt32(0);

                    // Check if the user has Super Admin rights
                    if (response.GetBoolean(2))
                    {
                        Logger.Msg<Program>($"Super Admin rights already assigned to user '{paramEmail}'", LogEventLevel.Error);
                        Environment.Exit(1);
                    }
                    else
                    {
                        response.Close();
                        // User is not a Super Admin, so change the record appropriately
                        sqlCmd.CommandText = $"UPDATE staff SET [SuperUser] = 1 WHERE [StaffID] = {StaffID};";

                        try
                        {
                            sqlCmd.ExecuteNonQuery();
                            Logger.Msg<Program>($"Super Admin rights granted to user '{paramEmail}'", LogEventLevel.Information);
                            Log.CloseAndFlush();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            Logger.Msg<Program>($"Super Admin rights could not be granted to user '{paramEmail}'", LogEventLevel.Error);
                            Logger.Msg<Program>(ex);
                            Log.CloseAndFlush();
                            Environment.Exit(1);
                        }


                    }
                }
                else
                {
                    try
                    {
                        // User does not exist in the database, need to create a new user first

                        Console.WriteLine($"User '{paramEmail}' does not exist in the database. A new user will be created.");
                        Console.Write("Enter new user's first name: ");
                        string firstName = Console.ReadLine().Trim();
                        Console.Write("Enter new user's last name: ");
                        string lastName = Console.ReadLine().Trim();

                        // Add user to the 'users' and 'staff' tables
                        sqlCmd.CommandText = $"INSERT INTO users ([Firstname], [Lastname], [Email], [Active]) VALUES (N'{firstName}', N'{lastName}', '{paramEmail}', 1);" +
                            $"INSERT INTO staff ([UserID], [SuperUser]) SELECT [UserID], 1 FROM users WHERE [email] = '{paramEmail}';";

                        Console.WriteLine(sqlCmd.CommandText);


                        int rowsAffected = sqlCmd.ExecuteNonQuery();
                        Logger.Msg<Program>($"Super Admin rights granted to user '{paramEmail}'", LogEventLevel.Information);
                        Log.CloseAndFlush();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Logger.Msg<Program>($"An error occured during SQL operation.", LogEventLevel.Error);
                        Logger.Msg<Program>(ex);
                        Log.CloseAndFlush();
                        Environment.Exit(1);
                    }

                }

            }
        }

    }




    // Command line options class
    // Ref: https://github.com/commandlineparser/commandline

    internal class StartupOptions
    {
        [Option('d', "debug", Default = false, Required = false, HelpText = "Increase log level to debug")]
        public bool Debug { get; set; }

        [Option('f', "logfile", Default = null, Required = false, HelpText = "Route log output to the specified file")]
        public FileInfo LogFile { get; set; }

        [Option("sslport", Default = 5001, Required = false, HelpText = "Listen on the specified port using SSL. Specify 0 to disable")]
        public int SSLPort { get; set; }

        [Option("httpport", Default = 5000, Required = false, HelpText = "Listen on the specified port. Specify 0 to disable")]
        public int HTTPPort { get; set; }

        [Option("createadmin", Required = false, HelpText = "Create a new or add existing user as a Super Admin")]
        public string CreateAdmin { get; set; }

    }


}
