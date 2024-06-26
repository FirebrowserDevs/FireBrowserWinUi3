﻿using System;
using System.IO;

namespace FireBrowserExceptions;
public static class ExceptionLogger
{
    private static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "firebrowserwinui.log");

    public static void LogException(Exception ex)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"Exception occurred at {DateTime.Now}:");
                LogExceptionDetails(ex, writer);
                writer.WriteLine("------------------------------------------------------------------");
            }
        }
        catch (Exception logEx)
        {
            // Log the exception to the console if an error occurs while logging
            Console.WriteLine($"Error while logging exception: {logEx.Message}");
        }
    }

    private static void LogExceptionDetails(Exception ex, StreamWriter writer)
    {
        if (ex != null)
        {
            writer.WriteLine($"Type: {ex.GetType().FullName}");
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"StackTrace: {ex.StackTrace}");
            writer.WriteLine();

            LogExceptionDetails(ex.InnerException, writer);
        }
    }
}