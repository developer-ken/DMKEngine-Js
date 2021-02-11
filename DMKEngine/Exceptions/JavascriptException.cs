using System;
using System.Collections.Generic;
using System.Text;

namespace DMKEngine.Exceptions
{
    class JavascriptException : Exception
    {
        public string JSCode { get; private set; }
        public string FilePath { get; private set; }
        public JavascriptException(string jscode, string filepath = "", string msg = "Malformated Js Code") : base(msg)
        {
            JSCode = jscode;
            FilePath = filepath;
        }
    }
}
