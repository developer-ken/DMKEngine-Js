using DMKEngine.Exceptions;
using Jint;
using Jint.Runtime.Interop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DMKEngine
{
    class JavascriptPlugin
    {
        public string Name;
        public string Version;
        public string Author;
        public bool NeedLogin;

        /// <summary>
        /// Enabled=false的插件无法收到事件
        /// </summary>
        internal bool Enabled;

        private List<Func<string, bool>> DanmakuListeners;
        private List<Func<string, bool>> SuperchatListeners;
        private List<Func<string, bool>> PersonEnterListeners;
        private List<Func<string, bool>> GiftListeners;
        private List<Func<string, bool>> UnrecognizedEventListeners;
        private List<Func<string, bool>> StreamStateChangeListeners;
        private List<Func<Exception, bool>> ExceptionListeners;

        private const string INIT_CODE = "" +
            "function Log(text,color,newline){" +
                "color=color||15;" +
                "newline=newline||true" +
                "plugin.Log(text,color,newline);" +
            "}";

        private Dictionary<string, dynamic> GlobalVars;

        private Engine engine;

        public JavascriptPlugin(string jsCode)
        {
            Enabled = true;
            try
            {
                engine = new Engine();
                engine.SetValue("plugin", this);
                engine.Execute(jsCode);
            }
            catch
            {

            }
            GlobalVars = new Dictionary<string, dynamic>();
            DanmakuListeners = new List<Func<string, bool>>();
            SuperchatListeners = new List<Func<string, bool>>();
            PersonEnterListeners = new List<Func<string, bool>>();
            GiftListeners = new List<Func<string, bool>>();
            UnrecognizedEventListeners = new List<Func<string, bool>>();
            ExceptionListeners = new List<Func<Exception, bool>>();
            if (Name == null)
            {
                throw new MalformatedPluginException(jsCode);
            }
        }

        public void Log(string content, int colorcode, bool line)
        {
            Console.Write(DateTime.Now.ToString("G") + "[" + Name + "] ");
            Console.ForegroundColor = (ConsoleColor)colorcode;
            Console.Write(content);
            if (line) Console.WriteLine();
            Console.ResetColor();
        }

        public void Log(string content)
        {
            Log(content, 15, true);
        }

        public void Log(string content, int colorcode)
        {
            Log(content, colorcode, true);
        }

        public Thread RunInNewThread(Action methold)
        {
            var t = new Thread(new ThreadStart(() =>
            {
                methold.Invoke();
            }));
            t.Start();
            return t;
        }

        public void PutGlobalVar(string name, dynamic value)
        {
            if (GlobalVars.ContainsKey(name)) GlobalVars[name] = value;
            else GlobalVars.Add(name, value);
        }

        public dynamic GetGlobalVar(string name)
        {
            if (GlobalVars.ContainsKey(name)) return GlobalVars[name];
            else return null;
        }

        public void OnDanmaku(Func<string, bool> callback)
        {
            try
            {
                DanmakuListeners.Add(callback);
            }
            catch (Exception err)
            {
                Log(err.Message, (int)ConsoleColor.Yellow);
            }
        }
        public void OnSuperchat(Func<string, bool> callback)
        {
            SuperchatListeners.Add(callback);
        }
        public void OnGift(Func<string, bool> callback)
        {
            GiftListeners.Add(callback);
        }
        public void OnUserEnter(Func<string, bool> callback)
        {
            PersonEnterListeners.Add(callback);
        }
        public void OnOtherEvents(Func<string, bool> callback)
        {
            UnrecognizedEventListeners.Add(callback);
        }
        public void OnStreamStartEnd(Func<string, bool> callback)
        {
            StreamStateChangeListeners.Add(callback);
        }

        public void OnException(Func<Exception, bool> callback)
        {
            ExceptionListeners.Add(callback);
        }


        internal bool TriggerDanmaku(string json)
        {
            if (!Enabled) return false;
            foreach (var c in DanmakuListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }

            }
            return false;
        }
        internal bool TriggerSuperchat(string json)
        {
            if (!Enabled) return false;
            foreach (var c in SuperchatListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }

        internal bool TriggerGift(string json)
        {
            if (!Enabled) return false;
            foreach (var c in GiftListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }

        internal bool TriggerEnter(string json)
        {
            if (!Enabled) return false;
            foreach (var c in PersonEnterListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }
        internal bool TriggerStreamStartEnd(string json)
        {
            if (!Enabled) return false;
            foreach (var c in StreamStateChangeListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }

        internal bool TriggerException(Exception e)
        {
            if (!Enabled) return false;
            foreach (var c in ExceptionListeners)
            {
                try
                {
                    if (c.Invoke(e)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }

        internal void TriggerLoad()
        {
            try
            {
                engine.Execute("Load();");
            }
            catch (Exception err)
            {
                Log(err.Message, (int)ConsoleColor.Yellow);
            }
        }

        internal bool TriggerOther(string json)
        {
            if (!Enabled) return false;
            foreach (var c in UnrecognizedEventListeners)
            {
                try
                {
                    if (c.Invoke(json)) return true;
                }
                catch (Exception err)
                {
                    Log(err.Message, (int)ConsoleColor.Yellow);
                }
            }
            return false;
        }

        internal string RunCode(string code)
        {
            return engine.Execute(code).GetCompletionValue().ToString();
        }
    }
}