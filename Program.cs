﻿using DbUp;
using DbUp.ScriptProviders;
using DbUp.Engine;
using DbUp.Engine.Output;
using System;
using System.Linq;
using System.IO;

class Program
             
{
    static int Main(string[] args)
    {
      var databases = new[]
        {
            new { Name = "testDB", ConnectionString = "", ScriptPath = "DBScripts" },
        };

        var logsFolder = "AllDBLogs";
        Directory.CreateDirectory(logsFolder);

        var logFile = Path.Combine(logsFolder, $"UpgradeLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        using var logWriter = new StreamWriter(logFile);

        void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
            logWriter.WriteLine(message);
            logWriter.Flush();
        }

        bool hasErrors = false;

        foreach (var db in databases)
        {
            Log($"====== Upgrading {db.Name} ======", ConsoleColor.Cyan);
            EnsureDatabase.For.SqlDatabase(db.ConnectionString);

            var scripts = new FileSystemScriptProvider(db.ScriptPath).GetScripts(null).ToList();

            foreach (var script in scripts)
            {
                var upgrader = DeployChanges.To
                    .SqlDatabase(db.ConnectionString)
                    .WithScripts(new[] { script })
                    .WithTransactionPerScript()
                    .WithExecutionTimeout(TimeSpan.FromMinutes(5))
                    .LogToConsole()
                    .Build();

                var result = upgrader.PerformUpgrade();

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (!result.Successful)
                {
                    hasErrors = true;
                    Log($"{timestamp} | {db.Name} | {script.Name} | FAILED | {result.Error.Message}", ConsoleColor.Red);
                    break;
                }
                else
                {
                    Log($"{timestamp} | {db.Name} | {script.Name} | SUCCESS", ConsoleColor.Green);
                }
            }
        }

        if (hasErrors)
        {
            Log("Completed with errors across one or more databases. Check logs.", ConsoleColor.Yellow);
            return -1;
        }

        Log("All databases upgraded successfully.", ConsoleColor.Green);
        return 0;
    
    }
}
