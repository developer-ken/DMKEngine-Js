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

        public delegate bool EventListener(string json);
        public delegate bool ExceptionReceiver(Exception ex);

        private List<EventListener> DanmakuListeners;
        private List<EventListener> PersonEnterListeners;
        private List<EventListener> GiftListeners;
        private List<EventListener> UnrecognizedEventListeners;
        private List<ExceptionReceiver> ExceptionListeners;

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
            DanmakuListeners = new List<EventListener>();
            PersonEnterListeners = new List<EventListener>();
            GiftListeners = new List<EventListener>();
            UnrecognizedEventListeners = new List<EventListener>();
            ExceptionListeners = new List<ExceptionReceiver>();
            if (Name == null)
            {
                throw new MalformatedPluginException(jsCode);
            }
        }

        public void Log(string content, int colorcode = 15, bool line = true)
        {
            Console.Write(DateTime.Now.ToString("G") + "[" + Name + "] ");
            Console.ForegroundColor = (ConsoleColor)colorcode;
            Console.Write(content);
            if (line) Console.WriteLine();
            Console.ResetColor();
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

        public void OnDanmaku(EventListener callback)
        {
            DanmakuListeners.Add(callback);
        }
        public void OnGift(EventListener callback)
        {
            GiftListeners.Add(callback);
        }
        public void OnUserEnter(EventListener callback)
        {
            PersonEnterListeners.Add(callback);
        }
        public void OnOtherEvents(EventListener callback)
        {
            UnrecognizedEventListeners.Add(callback);
        }
        public void OnException(ExceptionReceiver callback)
        {
            ExceptionListeners.Add(callback);
        }

        internal bool TriggerDanmaku(string json)
        {
            if (!Enabled) return false;
            foreach (var c in DanmakuListeners)
            {
                if (c.Invoke(json)) return true;
            }
            return false;
        }

        internal bool TriggerGift(string json)
        {
            if (!Enabled) return false;
            foreach (var c in GiftListeners)
            {
                if (c.Invoke(json)) return true;
            }
            return false;
        }

        internal bool TriggerEnter(string json)
        {
            if (!Enabled) return false;
            foreach (var c in PersonEnterListeners)
            {
                if (c.Invoke(json)) return true;
            }
            return false;
        }

        internal bool TriggerException(Exception e)
        {
            if (!Enabled) return false;
            foreach (var c in ExceptionListeners)
            {
                if (c.Invoke(e)) return true;
            }
            return false;
        }

        internal void TriggerLoad()
        {
            engine.Execute("Load();");
        }

        internal bool TriggerOther(string json)
        {
            if (!Enabled) return false;
            foreach (var c in UnrecognizedEventListeners)
            {
                if (c.Invoke(json)) return true;
            }
            return false;
        }
    }
}