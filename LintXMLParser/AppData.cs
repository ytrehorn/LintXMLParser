using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LintXMLParser
{
    class AppData
    {
        private int appID;
        private String smellid;
        private String affectedClass;
        private String line;
        private String package;
        private String method;
        private String appName;

        public int AppID
        {
            set { appID = value; }
            get {return appID;}
        }

        public String AppName
        {
            set { appName = value; }
            get { return appName; }
        }

        public String SmellID
        {
            set { smellid = value; }
            get { return smellid; }
        }

        public String AffectedClass
        {
            set { affectedClass = value; }
            get { return affectedClass; }
        }

        public String Line
        {
            set { line = value; }
            get { return line; }
        }

        public String Package
        {
            set { package = value; }
            get { return package; }
        }

        public String Method
        {
            set { method = value; }
            get { return method; }
        }
    }
}