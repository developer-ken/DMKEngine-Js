using System;
using System.Collections.Generic;
using System.Text;

namespace DMKEngine.Exceptions
{
    class MalformatedPluginException : Exception
    {
        public string JSCode { get; private set; }
        public string FilePath { get; private set; }
        public MalformatedPluginException(string jscode, string filepath="", string msg = "Malformated Plugin Detected.") : base(msg)
        {
            JSCode = jscode;
            FilePath = filepath;
        }
    }
}
