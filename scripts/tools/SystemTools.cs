using GodotLauncher.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GodotLauncher.Scripts.Tools;

public static class SystemTools
{
    public static void OpenPath(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", path);
        }
    }

    public static void StartProcess(string command, string args = "")
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            UseShellExecute = false
        };
        Process.Start(psi);
    }

    public static ExeBitness GetExeBitness(string exePath)
    {
        if (string.IsNullOrEmpty(exePath) || !System.IO.File.Exists(exePath))
            return ExeBitness.Unknown;

        using var fs = new FileStream(exePath, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        // DOS header
        if (br.ReadUInt16() != 0x5A4D) // "MZ"
            return ExeBitness.Unknown;

        fs.Seek(0x3C, SeekOrigin.Begin);
        int peHeaderOffset = br.ReadInt32();

        fs.Seek(peHeaderOffset, SeekOrigin.Begin);
        if (br.ReadUInt32() != 0x00004550) // "PE\0\0"
            return ExeBitness.Unknown;

        ushort machine = br.ReadUInt16();

        return machine switch
        {
            0x014c => ExeBitness.x86,  
            0x8664 => ExeBitness.x64,  
            _ => ExeBitness.Unknown
        };
    }

    public static void CancelWorker(BackgroundWorker backgroundWorker, int timeout = 2000)
    {
        if (!backgroundWorker.IsBusy) return;

        backgroundWorker.CancelAsync();

        while (backgroundWorker.IsBusy)
        {
            if (timeout <= 0) return;
            Thread.Sleep(10);
            timeout -= 10;
        }
    }
}
