using System.Diagnostics;
using System.IO;
using TUnit.Assertions;
using TUnit.Core;

namespace ConsoleStarter.Tests;

public class ParityTests
{
    [Test]
    public async Task Baseline_Parity_Should_Pass_5_Consecutive_Runs()
    {
        // Пуленепробиваемый поиск корня репо
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "src")))
            dir = dir.Parent;

        if (dir == null) throw new Exception("❌ Не найден корень репозитория (папка src/)");

        var projectPath = Path.Combine(dir.FullName, "src", "ConsoleStarter", "ConsoleStarter.csproj");

        var successCount = 0;
        for (int i = 1; i <= 5; i++)
        {
            var startInfo = new ProcessStartInfo("dotnet", $"run --project \"{projectPath}\" -c Release")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            var output = new List<string>();
            process.OutputDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) output.Add(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) output.Add(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var started = false;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 10000 && !process.HasExited)
            {
                if (output.Any(l => l.Contains("Application started") || l.Contains("Worker started")))
                {
                    started = true;
                    break;
                }
                await Task.Delay(100);
            }

            if (!process.HasExited) process.Kill();
            await process.WaitForExitAsync();

            if (!started) throw new Exception($"[Run {i}] Не запустилось.\n{string.Join("\n", output)}");

            // Проверяем только что приложение стартовало, код выхода не важен (Kill даёт -1)
            await Assert.That(output.Any(l => l.Contains("Application started") || l.Contains("Worker started"))).IsTrue();
            successCount++;
        }
        Console.WriteLine($"✅ Parity passed: {successCount}/5");
    }
}