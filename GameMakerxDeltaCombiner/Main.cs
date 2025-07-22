﻿using GM3P.modNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
namespace GM3P
{
    internal class Main
    {
        /// <summary>
        /// The path to the vanilla game
        /// </Summary>
        public static string? vanilla2 { get;  set; }
        //public static string vanilla = Main.vanilla2.Replace("\"", "");
        /// <summary>
        /// Current working directory
        /// </summary>
        public static string @pwd = @Convert.ToString(Directory.GetParent(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
        /// <summary>
        /// Output folder
        /// </summary>
        public static string? output { get; set; }
        /// <summary>
        /// path to an xDelta patcher, e.g. xDelta3 or Deltapatcher
        /// </summary>
        public static string? DeltaPatcher {get; set;}
        /// <summary>
        /// Amount of mods to merge
        /// </summary>
        public static int modAmount { get; set; }
        /// <summary>
        /// Currently unused except as a CLI arg, but this will be used to determine what Game Engine the game is in in a far future release. Use "GM" is for GameMaker
        /// </summary>
        public static string ?gameEngine {  get; set; }
        /// <summary>
        /// Whether or not the game uses game_change
        /// </summary>
        public static bool game_change { get; set; }
        /// <summary>
        /// A (probably) temporary bool to tell if compareCombine() has been called
        /// </summary>
        public static bool combined { get; set; }
        /// <summary>
        /// Path to the modTool for Dumping
        /// </summary>
        public static string ?modTool { get; set; }
        /// <summary>
        /// Returns a line from a text file as a string
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string GetLine(string fileName, int line)
        {
        using (var sr = new StreamReader(fileName))
            {
               for (int i = 1; i<line; i++)
                  sr.ReadLine();
               return sr.ReadLine();
            }
        }
        /// <summary>
        /// Creates the folders where other functions in this class works in
        /// </summary>
        public static void CreateCombinerDirectories()
        {


            Directory.CreateDirectory(@output + @"\xDeltaCombiner");
            for (int modNumber = 0; modNumber < (Main.modAmount + 2); modNumber++)
            {

                Directory.CreateDirectory(@output + "\\xDeltaCombiner\\" + modNumber);
                Directory.CreateDirectory(@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects");
                Directory.CreateDirectory(@output + "\\Cache\\vanilla");

            }
        ;
        }
        /// <summary>
        /// Copy vanilla files as much as needed
        /// </summary>
        public static void CopyVanilla()
        {
            if (!game_change)
            {
                for (int modNumber = 0; modNumber < (Main.modAmount + 2); modNumber++)
                {
                    File.Copy(Main.@vanilla2, @output + "\\xDeltaCombiner\\" + modNumber + "\\data.win", true);
                }
            }
            else
            {
                string[] vanilla = Directory.GetFiles(Main.@vanilla2, "*.win", SearchOption.AllDirectories);
                for (int modNumber = 0; modNumber < (Main.modAmount +1); modNumber++)
                {
                    Directory.CreateDirectory(@output + "\\xDeltaCombiner\\" + modNumber + "\\vanilla");
                    File.Copy(vanilla[modNumber], @output + "\\xDeltaCombiner\\" + modNumber + "\\data.win", true);
                    File.Copy(vanilla[modNumber], @output + "\\xDeltaCombiner\\" + modNumber + "\\vanilla\\data.win", true);
                }
            }
        }
        public static string[] xDeltaFile { get; set; }
        /// <summary>
        /// The titular function; patches a bunch of mods into data.win files
        /// </summary>
        public static void massPatch(string[] filepath = null)
        {
            int extra = 2;
            int asdf = 2;
            xDeltaFile = new string[(modAmount + 2)];
            if (!game_change)
            {
                extra = 2;
                asdf = 2;
            }
            else
            {
                extra = 1;
                asdf = 0;
            }
                if (filepath == null)
            {
                for (int modNumber = asdf; modNumber < (Main.modAmount + extra); modNumber++)
                {
                    xDeltaFile[modNumber] = Console.ReadLine().Replace("\"", "");

                }
            }
            else
            {
                for (int modNumber = asdf; modNumber < (Main.modAmount + extra); modNumber++)
                {
                    xDeltaFile[modNumber] = filepath[modNumber].Replace("\"", "");

                }
            }
            
                for (int modNumber = asdf; modNumber < (Main.modAmount + extra); modNumber++)
                {
                    //Check if the mod is a UTMT script. If so, patch it.
                    if (Path.GetExtension(xDeltaFile[modNumber]) == ".csx")
                    {
                        using (var modToolProc = new Process())
                        {
                            modToolProc.StartInfo.FileName = Main.@modTool;
                            modToolProc.StartInfo.Arguments = "load " + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win " + "--verbose --output " + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win" + " --scripts " + xDeltaFile[modNumber];
                            modToolProc.StartInfo.CreateNoWindow = false;
                            modToolProc.StartInfo.UseShellExecute = false;
                            modToolProc.StartInfo.RedirectStandardOutput = true;
                            modToolProc.Start();
                            // Synchronously read the standard output of the spawned process.
                            StreamReader reader = modToolProc.StandardOutput;
                            string ProcOutput = reader.ReadToEnd();

                            // Write the redirected output to this application's window.
                            Console.WriteLine(ProcOutput);

                            modToolProc.WaitForExit();
                        }
                    }
                    //If it's a full data.win, copy the file
                    else if (Path.GetExtension(xDeltaFile[modNumber]) == ".win")
                    {
                        File.Copy(xDeltaFile[modNumber], @output + "\\xDeltaCombiner\\" + modNumber + "\\data.win" + " ", true);
                    }
                    //else if (Path.GetExtension(xDeltaFile[modNumber]) == "" || Path.GetExtension(xDeltaFile[modNumber]) == null)
                    //{
                        
                    //}
                    //Otherwise, patch the xDelta
                    else
                    {
                        File.WriteAllText(@output + "\\Cache\\modNumbersCache.txt", Convert.ToString(modNumber));
                        using (var bashProc = new Process())
                        {
                            bashProc.StartInfo.FileName = Main.@DeltaPatcher;
                            bashProc.StartInfo.Arguments = "-v -d -f -s \"" + Main.@output + "\\xDeltaCombiner\\"+ modNumber +"\\data.win\"" + " \"" + xDeltaFile[modNumber] + "\" \"" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\dat.win" + "\" ";
                            bashProc.StartInfo.CreateNoWindow = false;
                            bashProc.StartInfo.UseShellExecute = false;
                            bashProc.StartInfo.RedirectStandardOutput = true;
                            bashProc.Start();
                            // Synchronously read the standard output of the spawned process.
                            StreamReader reader = bashProc.StandardOutput;
                            string ProcOutput = reader.ReadToEnd();

                            // Write the redirected output to this application's window.
                            Console.WriteLine(ProcOutput);

                            bashProc.WaitForExit();
                        }
                        File.Delete("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win");
                        File.Move("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\dat.win","" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win");
                    }
                    Console.WriteLine("Patched "+xDeltaFile[modNumber]);
                }
            }
        
        public static List<string> modifedAssets = new List<string> { "Asset Name                       Hash (SHA1 in Base64)" };
        public static void modifiedListCreate() {
            if (!File.Exists(@output + "\\xDeltaCombiner\\1\\modifedAssets.txt"))
            {
                File.Create(@output + "\\xDeltaCombiner\\1\\modifedAssets.txt").Close();
            }

        }
        /// <summary>
        /// Dumps objects from mod
        /// </summary>
        public static void dump()
        {
            Console.WriteLine("Starting dump, this may take up to a minute per mod (and vanilla)");
            for (int modNumber = 0; modNumber < (Main.modAmount + 2); modNumber++)
            {
                Directory.CreateDirectory(@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\CodeEntries");
                File.WriteAllText(@output + "\\Cache\\modNumbersCache.txt", Convert.ToString(modNumber));
                if (modNumber != 1)
                {
                    using (var modToolProc = new Process())
                    {
                        modToolProc.StartInfo.FileName = Main.@modTool;
                        modToolProc.StartInfo.Arguments = "load \"" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win\" " + "--verbose --output \"" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win" + "\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ExportAllTexturesGrouped.csx\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ExportAllCode.csx\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ExportAssetOrder.csx\"";
                        modToolProc.StartInfo.CreateNoWindow = false;
                        modToolProc.StartInfo.UseShellExecute = false;
                        modToolProc.StartInfo.RedirectStandardOutput = true;
                        modToolProc.Start();

                        // Synchronously read the standard output of the spawned process.
                        StreamReader reader = modToolProc.StandardOutput;
                        string ProcOutput = reader.ReadToEnd();

                        // Write the redirected output to this application's window.
                        Console.WriteLine(ProcOutput);

                        modToolProc.WaitForExit();
                    }
                }
            }
        }
        /// <summary>
        /// The function that's the main draw of GM3P; Compares and Combines objects
        /// </summary>
        public static void CompareCombine()
        {
            
            string[] vanillaFiles = Directory.GetFiles("" + Main.@output + "\\xDeltaCombiner\\0\\Objects\\", "*", SearchOption.AllDirectories);
            string[] vanillaFilesName = vanillaFiles.Select(Path.GetFileName).ToArray();
            int vanillaFileCount = Convert.ToInt32(vanillaFiles.Length);
            for (int modNumber = 2; modNumber < (Main.modAmount + 2); modNumber++)
            {
                int modFileCount = Convert.ToInt32(Directory.GetFiles("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\", "*", SearchOption.AllDirectories).Length);
                string[] modFiles = Directory.GetFiles("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\", "*", SearchOption.AllDirectories);
                string[] modFilesName = modFiles.Select(Path.GetFileName).ToArray();
                string[] modFileAdditions = modFilesName.Except(vanillaFilesName).ToArray();
                int modFileAdditionsCount = Convert.ToInt32(modFileAdditions.Length);
                for (int i = 0; i < modFileCount; i++)
                {
                    int k = 0;
                    string? modFileDir = Directory.GetParent(Path.GetDirectoryName(modFiles[i]))?.Name + "\\" + Directory.GetParent(modFiles[i])?.Name;
                    for (int j = 0; j < vanillaFileCount; j++)
                    {
                        //For Debugging Copying Files
                        //if (i == 0 && k == 0)
                        //{
                        //    for (int k2 = 0; k2 < vanillaFileCount; k2++)
                        //    {
                        //        string? vanillaFileDir = Directory.GetParent(Path.GetDirectoryName(vanillaFiles[k2])).Name + "\\" + Directory.GetParent(vanillaFiles[k2]).Name;
                        //        if (vanillaFileDir != "Objects\\CodeEntries")
                        //        {
                        //            if (vanillaFileDir == ("0\\Objects"))
                        //            {

                        //                File.Copy(Path.GetDirectoryName(vanillaFiles[k2]) + "\\" + Path.GetFileName(vanillaFiles[k2]), Main.output + "\\xDeltaCombiner\\1\\Objects\\" + Path.GetFileName(vanillaFiles[k2]), true);
                        //            }
                        //            if (vanillaFileDir != ("0\\Objects"))
                        //            {
                        //                Directory.CreateDirectory(Main.output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir);

                        //                File.Copy(Path.GetDirectoryName(vanillaFiles[k2]) + "\\" + Path.GetFileName(vanillaFiles[k2]), Main.output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir + "\\" + Path.GetFileName(vanillaFiles[k2]), true);
                        //            }
                        //        }
                        //        Console.WriteLine("Copying" + vanillaFiles[k2]);
                        //    }
                        //    k++;
                        //}
                        if (Path.GetFileName(vanillaFiles[j]) == Path.GetFileName(modFiles[i]) && (modFilesName[i] != "AssetOrder.txt"))
                        {
                            Console.WriteLine("Currently Comparing " + Path.GetFileName(vanillaFiles[j]) + " to " + Path.GetFileName(modFiles[i]));

                            if (vanillaFileCount <= modFileCount)
                            {
                                SHA1 vanillaHashing = new SHA1CryptoServiceProvider();

                                using (FileStream fs = File.OpenRead(vanillaFiles[j]))
                                {
                                    string vanillaHash = Convert.ToBase64String(vanillaHashing.ComputeHash(fs));
                                    SHA1 modHashing = new SHA1CryptoServiceProvider();

                                    using (FileStream fx = File.OpenRead(modFiles[j]))
                                    {
                                        string modHash = Convert.ToBase64String(modHashing.ComputeHash(fx));
                                        Console.WriteLine(modHash);

                                        if (modHash != vanillaHash)
                                        {
                                            Console.WriteLine(vanillaHash);
                                            Console.WriteLine(modFileDir);
                                            if (modFileDir == ("Objects\\CodeEntries"))
                                            {
                                                Console.WriteLine("Copying " + Path.GetFileName(modFiles[i]));
                                                File.Copy(Path.GetDirectoryName(modFiles[i]) + "\\" + Path.GetFileName(modFiles[i]), @output + "\\xDeltaCombiner\\1\\Objects\\CodeEntries\\" + Path.GetFileName(vanillaFiles[j]), true);
                                                Main.modifedAssets.Add(modFilesName[i] + "        " + modHash);
                                            }
                                            if (modFileDir != ("Objects\\CodeEntries"))
                                            {


                                                if (Path.GetExtension(modFiles[i]) == ".png")
                                                {
                                                    try
                                                    {
                                                        using (Bitmap image1 = new(@modFiles[i]))
                                                        {

                                                            using (Bitmap image2 = new(@vanillaFiles[j]))
                                                            {

                                                                [DllImport("msvcrt.dll")]
                                                                static extern int memcmp(IntPtr b1, IntPtr b2, long count);

                                                                static bool CompareMemCmp(Bitmap b1, Bitmap b2)
                                                                {
                                                                    if ((b1 == null) != (b2 == null)) return false;
                                                                    if (b1.Size != b2.Size) return false;

                                                                    var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                                                    var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                                                                    try
                                                                    {
                                                                        IntPtr bd1scan0 = bd1.Scan0;
                                                                        IntPtr bd2scan0 = bd2.Scan0;

                                                                        int stride = bd1.Stride;
                                                                        int len = stride * b1.Height;

                                                                        return memcmp(bd1scan0, bd2scan0, len) == 0;
                                                                    }
                                                                    finally
                                                                    {
                                                                        b1.UnlockBits(bd1);
                                                                        b2.UnlockBits(bd2);
                                                                    }
                                                                }


                                                                if (!CompareMemCmp(image1, image2))
                                                                {
                                                                    Directory.CreateDirectory(@output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir);

                                                                    File.Copy(Path.GetDirectoryName(modFiles[i]) + "\\" + Path.GetFileName(modFiles[i]), @output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir + "\\" + Path.GetFileName(vanillaFiles[j]), true);
                                                                    Main.modifedAssets.Add(modFilesName[i] + "        " + modHash);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex) 
                                                    { 
                                                        Console.WriteLine(ex.Message);
                                                        Console.WriteLine(modFiles[i]);
                                                    }


                                                }

                                            }
                                        }


                                    }

                                }
                            }
                            if (vanillaFileCount > modFileCount)
                            {
                                SHA1 modHashing = new SHA1CryptoServiceProvider();

                                using (FileStream fs = File.OpenRead(modFiles[i]))
                                {
                                    string modHash = Convert.ToBase64String(modHashing.ComputeHash(fs));
                                    SHA1 vanillaHashing = new SHA1CryptoServiceProvider();

                                    using (FileStream fx = File.OpenRead(vanillaFiles[i]))
                                    {
                                        try
                                        {
                                            string vanillaHash = Convert.ToBase64String(vanillaHashing.ComputeHash(fx));
                                            Console.WriteLine(modHash);

                                            if (modHash != vanillaHash)
                                            {
                                                Console.WriteLine(vanillaHash);
                                                Console.WriteLine(modFileDir);
                                                if (modFileDir == (modNumber + "\\Objects"))
                                                {

                                                    File.Copy(Path.GetDirectoryName(modFiles[i]) + "\\" + Path.GetFileName(modFiles[i]), @output + "\\xDeltaCombiner\\1\\Objects\\" + Path.GetFileName(vanillaFiles[j]), true);
                                                    modifedAssets.Add(modFilesName[i] + "        " + modHash);
                                                }
                                                if (modFileDir != (modNumber + "\\Objects"))
                                                {
                                                    Directory.CreateDirectory(@output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir);

                                                    File.Copy(Path.GetDirectoryName(modFiles[i]) + "\\" + Path.GetFileName(modFiles[i]), @output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir + "\\" + Path.GetFileName(vanillaFiles[j]), true);
                                                    modifedAssets.Add(modFilesName[i] + "        " + modHash);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }

                                    }

                                }
                            }

                        }

                    }

                }
                if (modFileAdditionsCount >= 1)
                {
                    string[] modFileAddtionsDir = Directory.GetFiles("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\", "*", SearchOption.AllDirectories).Select(Path.GetFullPath).ToArray();
                    for (int i = 0; i < modFileAdditionsCount; i++)
                    {


                        string[] modFilesParent = Directory.GetFiles("" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\", modFileAdditions[i], SearchOption.AllDirectories);
                        string? modFileDir = Directory.GetParent(Path.GetDirectoryName(modFilesParent[0])).Name + "\\" + Directory.GetParent(modFilesParent[0]).Name;

                        if (modFileDir == (modNumber + "\\Objects"))
                        {

                            File.Copy(Path.GetDirectoryName(modFiles[0]) + "\\" + Path.GetFileName(modFilesParent[0]), @output + "\\xDeltaCombiner\\1\\Objects\\" + Path.GetFileName(modFileAdditions[i]), true);
                            modifedAssets.Add(Path.GetFileName(modFilesParent[0]) + "        ");
                        }
                        if (modFileDir != (modNumber + "\\Objects"))
                        {
                            Directory.CreateDirectory(@output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir);

                            File.Copy(Directory.GetParent(modFilesParent[0]) + "\\" + Path.GetFileName(modFilesParent[0]), @output + "\\xDeltaCombiner\\1\\Objects\\" + modFileDir + "\\" + Path.GetFileName(modFileAdditions[i]), true);
                            modifedAssets.Add(Path.GetFileName(modFilesParent[0]) + "        ");
                        }
                        Console.WriteLine("Currently Copying " + Path.GetFileName(modFileAdditions[i]));

                        Console.WriteLine(modFileAdditionsCount);
                    }
                }

                //string assetOrderSeperator = "a bunch of random characters to define this";
                //var vanillaAssetOrder = File.ReadAllLines(@output + "\\xDeltaCombiner\\0\\Objects\\AssetOrder.txt").ToList();
                //int vanillaAssetOrderCount = vanillaAssetOrder.Count;
                //var modAssetOrder = File.ReadAllLines(@output + "\\xDeltaCombiner\\" + modNumber + "\\Objects\\AssetOrder.txt").ToList();
                //int modAssetOrderCount = modAssetOrder.Count;
                //if (!File.Exists(@output + "\\xDeltaCombiner\\1\\Objects\\AssetOrder.txt"))
                //{
                //    File.Create(@output + "\\xDeltaCombiner\\1\\Objects\\AssetOrder.txt").Close();
                //}

                //var finalModAssetOrder = File.ReadAllLines(@output + "\\xDeltaCombiner\\1\\Objects\\AssetOrder.txt").ToList();
                //if (modNumber == 2)
                //{
                //    finalModAssetOrder = vanillaAssetOrder;
                //}
                //for (int i = 0; i < modAssetOrderCount; i++)
                //{
                //    Console.WriteLine("Comparing asset order of vanilla to mod # " + (modNumber - 1) + ", Line " + i);
 
                //    string modAssetOrderLine = modAssetOrder[i];
                //    for (int j = 0; j < vanillaAssetOrderCount; j++)
                //    {
                //        string vanillaAssetOrderLine = vanillaAssetOrder[j];
                //        bool modAssetOrderinvanillaAssetOrder = vanillaAssetOrder.Contains(modAssetOrderLine);
                //        if (modAssetOrderLine == vanillaAssetOrderLine && modAssetOrderLine.StartsWith("@@"))
                //        {
                //            assetOrderSeperator = modAssetOrderLine;
                //        }

                //        if (modAssetOrderLine == vanillaAssetOrderLine && !finalModAssetOrder.Contains(assetOrderSeperator) && !modAssetOrderinvanillaAssetOrder && !finalModAssetOrder.Contains(modAssetOrderLine))
                //        {
                            
                //            finalModAssetOrder.Add(modAssetOrderLine); 
                         
                //        }
                //        if (modAssetOrderLine != vanillaAssetOrderLine && !modAssetOrderinvanillaAssetOrder && !finalModAssetOrder.Contains(modAssetOrderLine))
                //        {
                //            finalModAssetOrder.Add(modAssetOrderLine); 
                         
                //        }
                //    }
                //    File.WriteAllLines(@output + "\\xDeltaCombiner\\1\\Objects\\AssetOrder.txt", finalModAssetOrder);
                //}
            }
            Main.combined = true;
        }
        /// <summary>
        /// Imports resulting GameMaker Objects from the "CompareCombine()" function into a data.win
        /// </summary>
        public static void import()
        {
            if (Main.modTool == "skip")
            {
                Console.WriteLine("In order to replace and import manually, load up the data.win in \\xDeltaCombiner\\1\\ into the GUI version of UTMT and run the script ImportGML.csx. Select \"C:\\xDeltaCombiner\\*currentsubfolder*\\Objects\\\" as the import folder. Once finished, exit and saving.");
            }
            else
            {
                using (var modToolProc = new Process())
                {
                    modToolProc.StartInfo.FileName = Main.@modTool;
                    modToolProc.StartInfo.Arguments = "load \"" + Main.@output + "\\xDeltaCombiner\\1\\data.win\" " + "--verbose --output \"" + Main.@output + "\\xDeltaCombiner\\1\\data.win" + "\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ImportGraphicsAdvanced.csx\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ImportGML.csx\"";
                    modToolProc.StartInfo.CreateNoWindow = false;
                    modToolProc.StartInfo.UseShellExecute = false;
                    modToolProc.StartInfo.RedirectStandardOutput = true;
                    modToolProc.Start();
                    // Synchronously read the standard output of the spawned process.
                    StreamReader reader = modToolProc.StandardOutput;
                    string ProcOutput = reader.ReadToEnd();

                    // Write the redirected output to this application's window.
                    Console.WriteLine(ProcOutput);
                    modToolProc.WaitForExit();
                }
                for (int modNumber = 2; modNumber > modAmount + 2; modNumber++)
                {
                    using (var modToolProc = new Process())
                    {
                        modToolProc.StartInfo.FileName = Main.@modTool;
                        modToolProc.StartInfo.Arguments = "load \"" + Main.@output + "\\xDeltaCombiner\\1\\data.win\" " + "--verbose --output \"" + Main.@output + "\\xDeltaCombiner\\1\\data.win" + "\" --scripts \"" + Main.@pwd + "\\UTMTCLI\\Scripts\\ImportAssetOrder.csx\"";
                        modToolProc.StartInfo.CreateNoWindow = false;
                        modToolProc.StartInfo.UseShellExecute = false;
                        modToolProc.StartInfo.RedirectStandardOutput = true;
                        modToolProc.Start();
                        // Synchronously read the standard output of the spawned process.
                        StreamReader reader = modToolProc.StandardOutput;
                        string ProcOutput = reader.ReadToEnd();

                        // Write the redirected output to this application's window.
                        Console.WriteLine(ProcOutput);
                        modToolProc.WaitForExit();
                    }
                }
            }
        }
        public static void result(string modname)
        {
            if (modname != null && modname != "")
            {
                if (combined)
                {
                    Directory.CreateDirectory(@output + "\\result\\" + modname + "\\");
                    using (var bashProc = new Process())
                    {
                        bashProc.StartInfo.FileName = Main.@DeltaPatcher;
                        bashProc.StartInfo.Arguments = "-v -e -f -s \"" + Main.@output + "\\xDeltaCombiner\\0\\data.win" + "\" \"" + Main.@output + "\\xDeltaCombiner\\1\\data.win" + "\" \"" + Main.@output + "\\result\\" + modname + "\\" + modname + ".xdelta\"";
                        bashProc.StartInfo.CreateNoWindow = false;
                        bashProc.Start();
                        bashProc.WaitForExit();
                    }
                    File.Copy(@output + "\\xDeltaCombiner\\1\\data.win", @output + "\\result\\" + modname + "\\data.win");
                    File.Copy(@output + "\\xDeltaCombiner\\1\\modifedAssets.txt", @output + "\\result\\" + modname + "\\modifedAssets.txt");
                }
                else
                {
                    for (int modNumber = 0; modNumber < (Main.modAmount + 1); modNumber++)
                    {
                        Directory.CreateDirectory(@output + "\\result\\" + modname + "\\" + modNumber);
                        using (var bashProc = new Process())
                        {
                            bashProc.StartInfo.FileName = Main.@DeltaPatcher;
                            bashProc.StartInfo.Arguments = "-v -e -f -s \"" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\vanilla\\data.win" + "\" \"" + Main.@output + "\\xDeltaCombiner\\" + modNumber + "\\data.win" + "\" \"" + Main.@output + "\\result\\" + modname + "\\" + modNumber + ".xdelta\"";
                            bashProc.StartInfo.CreateNoWindow = false;
                            bashProc.Start();
                            bashProc.WaitForExit();
                        }
                        File.Copy(@output + "\\xDeltaCombiner\\"+modNumber+"\\data.win", @output + "\\result\\" + modname + "\\" + modNumber + "\\data.win");
                    }
                }
            }
        }
        public static void clear()
        {
            //for (int modNumber = 0; modNumber < Directory.GetDirectories(GM3P.Main.@output + "\\xDeltaCombiner\\").Length; modNumber++)
            //{
                //if (modNumber != 1)
                //{
                Directory.Delete(GM3P.Main.@output + "\\xDeltaCombiner\\", true);
                //}
            //}
        }
/// <summary>
/// Error to return if load() fails
/// </summary>
public static string loadError { get; set; }
        /// <summary>
        /// Loads Template
        /// </summary>
        /// <param name="filepath"></param>
        public static void load(string filepath = null)
        {
            if(filepath == null)
            { 
                if (File.Exists(Main.pwd + "\\template.xrune")) 
                {
                    filepath = Main.pwd + "\\template.xrune";
                }
            
            }
            if(filepath != null)
            {
                if (Main.GetLine(filepath, 1) == "0.4")
                {
                    string OpToPerform = Main.GetLine(filepath, 2);
                    modAmount = Convert.ToInt32(Main.GetLine(filepath, 3));
                    vanilla2 = Main.GetLine(filepath, 4);
                    output = Main.GetLine(filepath, 5);
                    DeltaPatcher = Main.GetLine(filepath, 6);
                    modTool = Main.GetLine(filepath, 7);
                    game_change = Convert.ToBoolean(Main.GetLine(filepath, 10));
                    if (OpToPerform == "regular")
                    {
                        CreateCombinerDirectories();
                        CopyVanilla();
                        massPatch(Main.GetLine(filepath, 8).Split(",").ToArray());
                        modifiedListCreate();
                        CompareCombine();
                        result(Main.GetLine(filepath, 9));
                    }
                }
                else
                {
                    loadError = "The template's version is not supported";
                }
            }
            if(filepath == null)
            {
                loadError = "The Template doesn't exists";
            }
            
        }
}
}
//A logging class C+Ped from https://stackoverflow.com/questions/420429/mirroring-console-output-to-a-file
class ConsoleCopy : IDisposable
{

    FileStream fileStream;
    StreamWriter fileWriter;
    TextWriter doubleWriter;
    TextWriter oldOut;

    class DoubleWriter : TextWriter
    {

        TextWriter one;
        TextWriter two;

        public DoubleWriter(TextWriter one, TextWriter two)
        {
            this.one = one;
            this.two = two;
        }

        public override Encoding Encoding
        {
            get { return one.Encoding; }
        }

        public override void Flush()
        {
            one.Flush();
            two.Flush();
        }

        public override void Write(char value)
        {
            one.Write(value);
            two.Write(value);
        }

    }

    public ConsoleCopy(string path)
    {
        oldOut = Console.Out;

        try
        {
            fileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);

            fileWriter = new StreamWriter(fileStream);
            fileWriter.AutoFlush = true;

            doubleWriter = new DoubleWriter(fileWriter, oldOut);
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot open file for writing");
            Console.WriteLine(e.Message);
            return;
        }
        Console.SetOut(doubleWriter);
    }

    public void Dispose()
    {
        Console.SetOut(oldOut);
        if (fileWriter != null)
        {
            fileWriter.Flush();
            fileWriter.Close();
            fileWriter = null;
        }
        if (fileStream != null)
        {
            fileStream.Close();
            fileStream = null;
        }
    }

}

//A console bool response thingy from https://stackoverflow.com/a/54127216
class UtilsConsole
{
    public static bool Confirm(string title)
    {
        ConsoleKey response;
        do
        {
            Console.Write($"{title} [y/n] ");
            response = Console.ReadKey(false).Key;
            if (response != ConsoleKey.Enter)
            {
                Console.WriteLine();
            }
        } while (response != ConsoleKey.Y && response != ConsoleKey.N);

        return (response == ConsoleKey.Y);
    }
}

