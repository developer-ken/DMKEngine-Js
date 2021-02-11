using BililiveRecorder.Core;
using Jint;
using System;
using System.IO;
using System.Net.Sockets;

namespace DMKEngine
{
    class Program
    {
        private static PluginManager pman = new PluginManager();
        private static StreamMonitor sm;
        static void Main(string[] args)
        {
            int RoomId = 2064239;
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
            sm = new StreamMonitor(RoomId, new Func<TcpClient>(() => { return new TcpClient(); }));
            sm.Start();
            sm.ReceivedDanmaku += Sm_ReceivedDanmaku;
            pman.TriggerLoad();
            JavascriptPlugin interactive = new JavascriptPlugin("" +
                "plugin.Name='Interactive Console';" +
                "plugin.Version='0000';" +
                "plugin.Author='Developer_ken';" +
                "plugin.NeedLogin=false;");
            pman.AddPlugin(interactive);
            while (true)
            {
                try
                {
                    var result = interactive.RunCode(Console.ReadLine());
                    Log(result);
                }
                catch (Exception e)
                {
                    Log("Fail to execute:" + e.Message, (int)ConsoleColor.Red);
                }
            }
        }

        private static void Sm_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            switch (e.Danmaku.MsgType)
            {
                //case MsgTypeEnum.SuperChat:
                //    pman.TriggerSuperchat(e.Danmaku.RawData);
                //    break;
                case MsgTypeEnum.Comment:
                    pman.TriggerDanmaku(e.Danmaku.RawData);
                    break;
                case MsgTypeEnum.GuardBuy:
                case MsgTypeEnum.GiftSend:
                    pman.TriggerGift(e.Danmaku.RawData);
                    break;
                case MsgTypeEnum.WelcomeGuard:
                case MsgTypeEnum.Welcome:
                    pman.TriggerEnter(e.Danmaku.RawData);
                    break;
                case MsgTypeEnum.LiveEnd:
                case MsgTypeEnum.LiveStart:
                    pman.TriggerStreamStartEnd(e.Danmaku.RawData);
                    break;
                default:
                    pman.TriggerOther(e.Danmaku.RawData);
                    break;
            }
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
