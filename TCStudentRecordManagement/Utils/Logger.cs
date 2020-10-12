using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

namespace TCStudentRecordManagement.Utils
{
    public class Logger
    {
        public static void Msg(string message, LogEventLevel level = LogEventLevel.Information)
        {
            switch (level)
            {
                case (LogEventLevel.Debug):
                    Log.Debug(message);
                    break;
                case (LogEventLevel.Error):
                    Log.Error(message);
                    break;
                case (LogEventLevel.Fatal):
                    Log.Fatal(message);
                    break;
                case (LogEventLevel.Information):
                    Log.Information(message);
                    break;
                case (LogEventLevel.Verbose):
                    Log.Verbose(message);
                    break;
                case (LogEventLevel.Warning):
                    Log.Warning(message);
                    break;
                default:
                    break;


            }
        }
    
    
        // Pass a class as a parameter to a method
        // Ref: https://stackoverflow.com/questions/18806579/how-to-pass-a-class-as-parameter-for-a-method

        public static void Msg<T>(string message, LogEventLevel level = LogEventLevel.Information)
        {
            string assembly = typeof(T).Name == "Program" ? "" : $"[{typeof(T).Name}] ";

            switch (level)
            {
                case (LogEventLevel.Debug):
                    Log.ForContext<T>().Debug($"{assembly}{message}");
                    break;
                case (LogEventLevel.Error):
                    Log.ForContext<T>().Error($"{assembly}{message}");
                    break;
                case (LogEventLevel.Fatal):
                    Log.ForContext<T>().Fatal($"{assembly}{message}");
                    break;
                case (LogEventLevel.Information):
                    Log.ForContext<T>().Information($"{assembly}{message}");
                    break;
                case (LogEventLevel.Verbose):
                    Log.ForContext<T>().Verbose($"{assembly}{message}");
                    break;
                case (LogEventLevel.Warning):
                    Log.ForContext<T>().Warning($"{assembly}{message}");
                    break;
                default:
                    break;
            } 
        }

        // Exception logger with Context specification
        public static void Msg<T>(Exception ex, LogEventLevel level = LogEventLevel.Error)
        {
            string assembly = typeof(T).Name == "Program" ? "" : $"[{typeof(T).Name}] ";

            Msg<T>(ex.Message, level);
            Log.ForContext<T>().Debug(ex.StackTrace);
            Log.ForContext<T>().Debug(ex.Source);

            // Check for inner exceptions
            if (ex.InnerException != null)
            {
                Msg<T>(ex.InnerException.Message, level);
                Log.ForContext<T>().Debug(ex.InnerException.StackTrace);
                Log.ForContext<T>().Debug(ex.InnerException.Source);
            }
        }

        // Generic Exception logger (no context)

        public static void Msg(Exception ex)
        {
            // Log the basic message at Error log level.
            // Log stack trace and source at Debug log level.

            Log.Error(ex.Message);
            Log.Debug(ex.StackTrace);
            Log.Debug(ex.Source);

            // Check for inner exceptions
            if (ex.InnerException != null)
            {
                Log.Error(ex.InnerException.Message);
                Log.Debug(ex.InnerException.StackTrace);
                Log.Debug(ex.InnerException.Source);
            }

        }

        // Exception list parser
        public static void Msg(List<Exception> exList)
        {
            // Deal with exception lists
            exList.ForEach(ex => Msg(ex));
            
        }


    }

}
