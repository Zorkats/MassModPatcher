﻿// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Immutable;
using System.Collections.Generic;
using GM3P;

namespace GM3P.Program;

class Program
{
    static void Main(string[] args = null)
    {
        //Store version as a double and print full version #
        double Version = 0.4;
        Console.WriteLine("GM3P v" + Version + ".1");

        //Create logging file and start logging
        string startTime = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("zz");
        GM3P.Main.output = GM3P.Main.pwd + "/output";
        Directory.CreateDirectory(GM3P.Main.@output + "/Cache");
        Directory.CreateDirectory(GM3P.Main.@output + "/Cache/Logs/");
        File.Create(GM3P.Main.@output + "/Cache/Logs/" + startTime + ".txt").Close();
        using (var cc = new ConsoleCopy(GM3P.Main.@output + "/Cache/Logs/" + startTime + ".txt"))
        {
            //CLI parser
            if (args != null)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Starting Console App...");
                    consoleApp();

                }
                var command = args[0];
                switch (command)
                {
                    case "massPatch":
                        GM3P.Main.vanilla2 = args[1];

                        GM3P.Main.output = GM3P.Main.pwd + "/output";
                        if (args.Length > 5)
                        {
                            GM3P.Main.game_change = Convert.ToBoolean(args[5]);
                            if (args.Length > 6)
                            {
                                GM3P.Main.output = args[6];
                            }
                        }
                        GM3P.Main.DeltaPatcher = GM3P.Main.pwd + "/xdelta3-3.0.11-x86_64.exe";
                        if (OperatingSystem.IsLinux())
                        {
                            GM3P.Main.DeltaPatcher = "xdelta3 ";
                        }
                        GM3P.Main.gameEngine = args[2];
                        GM3P.Main.modAmount = Convert.ToInt32(args[3]);
                        GM3P.Main.CreateCombinerDirectories();
                        GM3P.Main.CopyVanilla();
                        GM3P.Main.massPatch(args[4].Split(",").ToArray());
                        break;
                    case "compare":
                        GM3P.Main.output = GM3P.Main.pwd + "/output";
                        GM3P.Main.modAmount = Convert.ToInt32(args[1]);
                        if (args.Length > 2)
                        {
                            GM3P.Main.modTool = GM3P.Main.pwd + "/UTMTCLI/UndertaleModCli.exe";
                            if (OperatingSystem.IsLinux())
                            {
                                GM3P.Main.modTool = "dotnet \"" + GM3P.Main.@pwd + "/UTMTCLI/UndertaleModCli.dll\" ";
                            }
                            if (args.Length > 4)
                            {
                                GM3P.Main.output = args[4];
                            }
                            if (args[2] == "true")
                            {
                                GM3P.Main.dump();
                            }

                        }
                        GM3P.Main.modifiedListCreate();
                        GM3P.Main.CompareCombine();
                        if (args.Length > 3)
                        {
                            if (args[3] == "true")
                            {
                                GM3P.Main.import();
                            }
                        }
                        break;
                    case "result":

                        GM3P.Main.DeltaPatcher = GM3P.Main.pwd + "/xdelta3-3.0.11-x86_64.exe";
                        if (OperatingSystem.IsLinux())
                        {
                            GM3P.Main.DeltaPatcher = "xdelta3 ";
                        }
                        if (args.Length > 2)
                        {
                            GM3P.Main.combined = Convert.ToBoolean(args[2]);
                            if (args.Length > 3)
                            {
                                GM3P.Main.modAmount = Convert.ToInt32(args[3]);
                            }
                            if (args.Length > 4)
                            {
                                GM3P.Main.output = args[4];
                            }
                        }
                        GM3P.Main.result(args[1]);
                        break;
                    case "cache":
                        break;
                    case "console":
                        Console.WriteLine("Starting Console App...");
                        consoleApp();
                        break;
                    case "clear":
                        if (args.Length > 1)
                        {
                            GM3P.Main.output = args[1];
                        }
                        GM3P.Main.clear();
                        break;
                    case null:
                        consoleApp();
                        break;
                    case "help":
                        if (args.Length > 1)
                        {
                            string commandHelp = args[1];
                            if (commandHelp == "massPatch")
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Makes a bunch of patched, single-mod data.win files quickly.");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine("Command Santax:   GM3P.exe massPatch [Vanilla Copy] [Game Engine] [Amount of Mods] [Mod File Paths] [(optional) whether or not the game uses game_change()] [(optional) Output Folder]");
                                Console.WriteLine(" ");
                                Console.WriteLine("Example:          GM3P.exe massPatch \"C:/Program Files(x86)/Steam/steamapps/common/DELTARUNE/chapter3_windows/data.win\" GM 2 \",,F:/Downloads/a.xDelta,F:/Downloads/b.csx\" false \"C:/Undertale Mods\"");
                                Console.WriteLine(" ");
                                Console.WriteLine("Note: The Mod File Paths arg must be encased with a double quote (\"), but the individual paths cannot be encased with, nor contain, a double quote. Mod File Paths are delimited by commas (,) and paths entered before the second delimiter are ingored unless game_change is true.");
                            }
                            if (commandHelp == "clear")
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Clears the xDeltaCombiner folders for reuse on future uses.");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine("Command Santax:   GM3P.exe clear [(optional) Output Folder]");
                                Console.WriteLine(" ");
                                Console.WriteLine("Example:          GM3P.exe clear \"C:/Undertale Mods\"");
                            }
                            if (commandHelp == "result")
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Makes .xDelta(s) and .win(s) for the resulting modpack.");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine("Command Santax:   GM3P.exe result [whether or not compare was called before] [(required if the previous arg is \"false\", otherwise ingored) amount of mods or chapters] [(optional) output folder]");
                                Console.WriteLine(" ");
                                Console.WriteLine("Example:          GM3P.exe result true 4 \"C:/Undertale Mods\"");
                            }
                            if (commandHelp == "console")
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Launches console app.");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine("Command Santax:   GM3P.exe console");
                                Console.WriteLine(" ");
                                Console.WriteLine("Example:          GM3P.exe console");
                                Console.WriteLine(" ");
                                Console.WriteLine("Note: The console app can also be launched if no command is provided. ");
                            }
                            if (commandHelp == "compare")
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Compares and combine GM objects. Dumping and importing optional.");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine(" ");
                                Console.WriteLine("Command Santax:   GM3P.exe compare [Amount of Mods] [(optional) Dump] [(optional) Import] [(optional) Output Folder]");
                                Console.WriteLine(" ");
                                Console.WriteLine("Example:          GM3P.exe compare 2 false true \"C:/Undertale Mods\"");
                                Console.WriteLine(" ");
                                Console.WriteLine("Note: Can only be successfully called if massPatch was called before. Will be buggy.");
                            }
                        }
                        if (args.Length == 1)
                        {
                            Console.WriteLine("Avalible commands:\nhelp        Display a satanax for a command and exit (use \"GM3P.exe help *command*\")\nmassPatch   Patches a lot of data.win files with a single mod each\nconsole     launches console app\nclear       Clears the xDeltaCombiner folder for future use.\ncompare     Compares modded GM Objects to vanilla and puts changes in a list. Dumping and Importing optional.\nresult      Name and copy modpack to a folder\n");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
            else
            {
                consoleApp();
            }
            //Console App
            static void consoleApp()
            {
                Console.WriteLine("Insert the path to the vanilla data.win, or type \"skip\" if you just want to compare and combine:");
                GM3P.Main.vanilla2 = Console.ReadLine().Replace("\"", "");
                GM3P.Main.output = GM3P.Main.@pwd + "/output";
                GM3P.Main.DeltaPatcher = GM3P.Main.@pwd + "/xdelta3-3.0.11-x86_64.exe";
                if (OperatingSystem.IsLinux())
                {
                    GM3P.Main.DeltaPatcher = "xdelta3 ";
                }
                GM3P.Main.game_change = UtilsConsole.Confirm("Did you enter a directory to a GameMaker game that uses game_change? If you are unsure or are linking directly to the data.win, hit \"N\": ");
                if (!GM3P.Main.game_change)
                {
                    Console.WriteLine("Type however many mods you want to patch: ");
                }
                else
                {
                    Console.WriteLine("Type however many chapters the game has: ");
                }
                GM3P.Main.modAmount = Convert.ToInt32(Console.ReadLine());
                if (GM3P.Main.vanilla2 != "skip")
                {
                    //Console.WriteLine("Enter in the path of a xdelta3 executable: ");

                    GM3P.Main.CreateCombinerDirectories();
                    GM3P.Main.CopyVanilla();
                    Console.WriteLine("Now Enter in the xDeltas, one at a time: ");
                    GM3P.Main.massPatch();
                }
                if (!GM3P.Main.game_change)
                {
                    Console.WriteLine("Enter in the Mod Tool (e.g. UnderTaleModTool for GameMaker Games). If you want to use the included tool, just hit enter. If you want to manually dump and import enter \"skip\"");
                    Console.WriteLine("If you don't want to combine patches and just wanted to apply them, you may exit the terminal now");
                    GM3P.Main.modTool = Console.ReadLine();
                    if (GM3P.Main.modTool == null || GM3P.Main.modTool == "")
                    {
                        GM3P.Main.modTool = GM3P.Main.@pwd + "/UTMTCLI/UndertaleModCli.exe";
                        if (OperatingSystem.IsLinux())
                        {
                            GM3P.Main.modTool = "dotnet '" + GM3P.Main.@pwd + "/UTMTCLI/UndertaleModCli.dll' ";
                        }
                    }
                    if (GM3P.Main.modTool != "skip")
                    {
                        GM3P.Main.dump();
                        Console.WriteLine("The dumping process(es) are finished");
                    }
                    else
                    {
                        Console.WriteLine("In order to dump manually, load up the data.win in each of the /xDeltaCombiner/ subfolders into the GUI version of UTMT and run the script ExportAllCode.csx. Select \"C:/xDeltaCombiner/*currentsubfolder*/Objects/\" as your destination. Once finished, exit without saving.");
                        Console.WriteLine("Press Enter when done with the above instructions");
                    }
                    Console.ReadLine();
                    GM3P.Main.modifiedListCreate();
                    GM3P.Main.CompareCombine();

                    File.WriteAllLines(GM3P.Main.@output + "/xDeltaCombiner/1/modifedAssets.txt", GM3P.Main.modifedAssets);
                    Console.WriteLine("Comparing is done. Hit Enter to Continue.");
                    Console.ReadLine();
                    GM3P.Main.import();
                }
                Console.WriteLine("To save your modpack, name it: ");
                GM3P.Main.result(Console.ReadLine());
                Console.WriteLine("Press Enter To Clean up (Will delete output/xDeltaCombiner) and exit");
                Console.ReadLine();
                GM3P.Main.clear();
                Environment.Exit(1);
            }

        }
    }
}