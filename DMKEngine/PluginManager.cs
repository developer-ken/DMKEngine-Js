using Jint;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMKEngine
{
    class PluginManager
    {
        private Dictionary<Guid, JavascriptPlugin> plugins;
        public PluginManager()
        {
            plugins = new Dictionary<Guid, JavascriptPlugin>();
        }

        public Guid AddPlugin(JavascriptPlugin plugin)
        {
            Guid id = Guid.NewGuid();
            plugins.Add(id, plugin);
            return id;
        }

        public void RemovePlugin(Guid id)
        {
            plugins.Remove(id);
        }

        public void SetPluginEnabled(Guid id, bool enabled = true)
        {
            plugins[id].Enabled = enabled;
        }

        public void TriggerDanmaku(string json)
        {
            foreach (var p in plugins)
            {
                if (p.Value.TriggerDanmaku(json)) break;
            }
        }

        public void TriggerGift(string json)
        {
            foreach (var p in plugins)
            {
                if (p.Value.TriggerGift(json)) break;
            }
        }

        public void TriggerEnter(string json)
        {
            foreach (var p in plugins)
            {
                if (p.Value.TriggerEnter(json)) break;
            }
        }

        public void TriggerOther(string json)
        {
            foreach (var p in plugins)
            {
                if (p.Value.TriggerOther(json)) break;
            }
        }

        public void TriggerException(Exception ex)
        {
            foreach (var p in plugins)
            {
                if (p.Value.TriggerException(ex)) break;
            }
        }

        public void TriggerLoad()
        {
            foreach (var p in plugins)
            {
                p.Value.TriggerLoad();
            }
        }
    }
}
