using Jint;
using System;
using System.IO;

namespace DMKEngine
{
    class Program
    {
        private static PluginManager pman = new PluginManager();
        static void Main(string[] args)
        {
            Directory.CreateDirectory("plugins");
            var files = Directory.GetFiles("plugins", "*.plugin.js");
            Log("DMKEngine " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " by Developer_ken");
            Log("Loading plugins...");
            foreach (string f in files)
            {
                try
                {
                    string loadPath = Path.GetFullPath(f);
                    string fname = Path.GetFileNameWithoutExtension(loadPath);
                    Log("Loading \"" + fname + "\"...");
                    var js = new JavascriptPlugin(File.ReadAllText(loadPath));
                    pman.AddPlugin(js);
                    Log("<" + js.Name + "> " + js.Version + " by " + js.Author);
                }
                catch (Exception e)
                {
                    Log("Fail to load plugin " + f, (int)ConsoleColor.Red);
                    Log(e.Message, (int)ConsoleColor.Yellow);
                    Log(e.StackTrace, (int)ConsoleColor.Yellow);
                }
            }
            Log("Initializing engine... ");
        }

        public static void Log(string content, int colorcode = 15, bool line = true)
        {
            Console.Write(DateTime.Now.ToString("G") + " [Host] ");
            Console.ForegroundColor = (ConsoleColor)colorcode;
            Console.Write(content);
            if (line) Console.WriteLine();
            Console.ResetColor();
        }
    }
}
