using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;
using System.Configuration;
using System.IO;
using log4net.Config;
using LuceneServer.Filters;
using LuceneServer.Services;
using Topshelf;

namespace LuceneServer
{
    class Program
    {
        static void Main(string[] args)
        {
			var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
			XmlConfigurator.ConfigureAndWatch(logCfg);

			HostFactory.Run(c =>
            {
                c.Service<LnServer>();
                c.RunAsLocalSystem();
                c.SetServiceName("LuceneServer");
                c.SetDisplayName("Lucene.net服务");
                c.SetDescription("提供全文搜索功能服务");
            });
        }
    }
}
