using System;
using System.IO;

namespace ConfigMgmt
{
    class Program
    {
        private static string tempfolder = @"c:/temp";

        private static string Website = @"Website";
        private static string WebConfig = @"Web.Config";

        private static string Demos = @"Demos";
        private static string GolfShop = @"GolfShop";
        private static string demoConfig = @"demoConfig.txt";
        private static string Registration = @"Registration";
        private static string demoRegConfig = @"demoRegConfig.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("--- Configuration Management ---");
            Console.WriteLine(" This application will save existing configuration files(under a new name) or");
            Console.WriteLine(" restore(overwrite) runtine configuration files from previous saved name.");
            Console.WriteLine();
            
            Console.WriteLine(" Files that get saved/restored are:");
            Console.WriteLine("             1) main Web.Config file, ");
            Console.WriteLine("             2) Golf Shop text configuration file,");
            Console.WriteLine("             3) Registration text configuration file.");
            Console.WriteLine();

            Console.WriteLine(" Note: default configuration files 'AWS', 'Demo' or 'Local' are static files and only be restored.");
            Console.WriteLine("   Set examples: 'set local', 'set aws', 'set demo', 'set myconfig'");
            Console.WriteLine("   Save example: 'save myconfig'  Note: new saved file are saved in the c:/temp folder");
            while (true)
            {
                Console.WriteLine();
                Console.Write(" Enter command?");
                var mCmd = Console.ReadLine();
                if (String.IsNullOrEmpty(mCmd)) continue;
                mCmd = mCmd.ToLower().Replace("'", "").Trim();
                if (mCmd.StartsWith("set"))
                {
                    var mSetName = mCmd.Replace("set", "").Trim();
                    if (!String.IsNullOrEmpty(mSetName))
                    {
                        var result = setconfigfiles(mSetName);
                        if (!String.IsNullOrEmpty(result))
                            Console.WriteLine(" Result:" + result);
                        break;
                    }
                }
                if (mCmd.StartsWith("save"))
                { // save under what name
                    var mSaveName = mCmd.Replace("save", "").Trim();
                    if (!String.IsNullOrEmpty(mSaveName))
                    {
                        var result = saveconfigFiles(mSaveName);
                        if (!String.IsNullOrEmpty(result))
                            Console.WriteLine(" Result:" + result);
                        break;
                    }
                }
                Console.WriteLine(" Command error, reenter");
            }
            Console.WriteLine(" Done...");
            Console.ReadLine();
        }

        private static string saveconfigFiles(string pFolderName)
        {
            if ((pFolderName == "local") || (pFolderName == "aws") || (pFolderName == "demo")) return pFolderName + " is a reserved name!";
            
            //C:\Development_OTP_System\MAC-OTP-System\Dev\R1.0\ConsoleApps\ConfigMgmt
            var myPath = Directory.GetCurrentDirectory();
            var i = myPath.IndexOf("ConsoleApps", StringComparison.Ordinal);
            var mRoot = myPath.Substring(0, i);
            Console.WriteLine(" Source root: " + mRoot);

            var mSrcWebConfigFilePath = Path.Combine(mRoot, Website, WebConfig);
            //Console.WriteLine(" mSrcWebConfigFilePath: " + mSrcWebConfigFilePath);
            if (File.Exists(mSrcWebConfigFilePath) == false)
            {
                Console.WriteLine(" Could not find Web.Config file in folder: " + Environment.NewLine + Path.Combine(mRoot, Website));
                return "Error";
            }
            Console.WriteLine("... found source " + WebConfig + " file");

            var mSrcGolfShopFilePath = Path.Combine(mRoot, Demos, GolfShop, demoConfig);
            //Console.WriteLine(" mSrcGolfShopFilePath: " + mSrcGolfShopFilePath);
            if (File.Exists(mSrcGolfShopFilePath) == false)
            {
                Console.WriteLine(" Could not find '" + demoConfig + "' file in folder: " + Environment.NewLine + Path.Combine(mRoot, Demos, GolfShop));
                return "Error";
            }
            Console.WriteLine(" ...found source " + demoConfig + " file");

            var mSrcRegFilePath = Path.Combine(mRoot, Demos, Registration, demoRegConfig);
            //Console.WriteLine("mSrcRegFilePath: " + mSrcRegFilePath);
            if (File.Exists(mSrcRegFilePath) == false)
            {
                Console.WriteLine(" Could not find '" + demoRegConfig + "' file in folder: " + Environment.NewLine + Path.Combine(mRoot, Demos, Registration));
                return "Error";
            }
            Console.WriteLine(" ...found source " + demoRegConfig + " file");
            
            var DestinationFolder = Path.Combine(tempfolder, pFolderName);
            if (!Directory.Exists(DestinationFolder))
            {
                Directory.CreateDirectory(DestinationFolder);
            }
            //------- do save ---------
            Console.WriteLine();
            Console.WriteLine(" save  " + WebConfig + " file");
            File.Copy(mSrcWebConfigFilePath, Path.Combine(DestinationFolder, WebConfig), true);

            Console.WriteLine(" save  " + demoConfig + " file");
            File.Copy(mSrcGolfShopFilePath, Path.Combine(DestinationFolder, demoConfig), true);

            Console.WriteLine(" save  " + demoRegConfig + " file");
            File.Copy(mSrcRegFilePath, Path.Combine(DestinationFolder, demoRegConfig), true);
 
            return null;
        }

        private static string setconfigfiles(string pFolderName)
        {
            Console.WriteLine();
            //C:\Development_OTP_System\MAC-OTP-System\Dev\R1.0\ConsoleApps\ConfigMgmt
            var myPath = Directory.GetCurrentDirectory();
            var i = myPath.IndexOf("ConsoleApps", StringComparison.Ordinal);
            var mRoot = myPath.Substring(0, i);
            Console.WriteLine(" Target root: : " + mRoot);

            //C:\Development_OTP_System\MAC-OTP-System\Dev\R1.0\Website\web.config
            //Console.WriteLine(Path.Combine(mRoot, "Website"));
            var mDestWebConfigFilePath = Path.Combine(mRoot, Website, WebConfig);
            //Console.WriteLine(" mDestWebConfigFilePath: " + mDestWebConfigFilePath);
            if (File.Exists(mDestWebConfigFilePath) == false)
            {
                Console.WriteLine(" Could not find Web.Config file in folder: " + Environment.NewLine + Path.Combine(mRoot, Website));
                return "Error";
            }
            Console.WriteLine("... found target " + WebConfig + " file");

            //C:\Development_OTP_System\MAC-OTP-System\Dev\R1.0\Demos\GolfShop
            //Console.WriteLine(Path.Combine(mRoot, "Demos","GolfShop"));
            var mDestGolfShopFilePath = Path.Combine(mRoot, Demos, GolfShop, demoConfig);
            //Console.WriteLine(" mDestGolfShopFilePath: " + mDestGolfShopFilePath);
            if (File.Exists(mDestGolfShopFilePath) == false)
            {
                Console.WriteLine(" Could not find '" + demoConfig + "' file in folder: " + Environment.NewLine + Path.Combine(mRoot, Demos, GolfShop));
                return "Error";
            }
            Console.WriteLine(" ...found target " + demoConfig + " file");

            //C:\Development_OTP_System\MAC-OTP-System\Dev\R1.0\Demos\GolfShop
            //Console.WriteLine(Path.Combine(mRoot, "Demos", "Registration"));
            var mDestRegFilePath = Path.Combine(mRoot, Demos, Registration, demoRegConfig);
            //Console.WriteLine("mDestRegFilePath: " + mDestRegFilePath);
            if (File.Exists(mDestRegFilePath) == false)
            {
                Console.WriteLine(" Could not find '" + demoRegConfig + "' file in folder: " + Environment.NewLine + Path.Combine(mRoot, Demos, Registration));
                return "Error";
            }
            Console.WriteLine(" ...found target " + demoRegConfig + " file");
            // get source folder
            var mSourceFolder = Path.Combine(tempfolder, pFolderName);
            var tag = string.Empty;
            // find source files
            if ((pFolderName == "local") || (pFolderName == "aws") || (pFolderName == "demo"))
            {
                mSourceFolder = myPath;
                tag = pFolderName + ".";
            }

            // is there a web.config in the source folder
            var mSrcWebConfigFilePath = Path.Combine(mSourceFolder, tag + WebConfig);
            //Console.WriteLine("mSrcWebConfigFilePath: " + mSrcWebConfigFilePath);
            if (File.Exists(mSrcWebConfigFilePath) == false)
            {
                Console.WriteLine(" Could not find source '" + WebConfig + "' file in folder: " + Environment.NewLine + mSourceFolder);
                return "Src error";
            }
            Console.WriteLine(" ...found source " + WebConfig + " file");

            // is there a golf shop configuration file in the source folder
            var mSrcGolfShopFilePath = Path.Combine(mSourceFolder, tag + demoConfig);
            //Console.WriteLine("mSrcGolfShopFilePath: " + mSrcGolfShopFilePath);
            if (File.Exists(mSrcGolfShopFilePath) == false)
            {
                Console.WriteLine(" Could not find "  + demoConfig + " file in folder: " + Environment.NewLine + mSourceFolder);
                return "Src error";
            }
            Console.WriteLine(" ...found source " + demoConfig + " file");

            // is there a Registration configuration file in the source folder
            var mSrcRegFilePath = Path.Combine(mSourceFolder, tag + demoRegConfig);
            //Console.WriteLine("mSrcRegFilePath: " + mSrcRegFilePath);
            if (File.Exists(mSrcRegFilePath) == false)
            {
                Console.WriteLine(" Could not find "  + demoRegConfig +  "file in folder: " + Environment.NewLine + mSourceFolder);
                return "Src error";
            }
            Console.WriteLine(" ...found source " + demoRegConfig + " file");

            //------- do set ---------
            Console.WriteLine();
            Console.WriteLine(" set  " + WebConfig + " file");
            File.Copy(mSrcWebConfigFilePath, mDestWebConfigFilePath, true);

            Console.WriteLine(" set  " + demoConfig + " file");
            File.Copy(mSrcGolfShopFilePath, mDestGolfShopFilePath, true);

            Console.WriteLine(" set  " + demoRegConfig + " file");
            File.Copy(mSrcRegFilePath, mDestRegFilePath, true);

            return null;
        }
    }
}
