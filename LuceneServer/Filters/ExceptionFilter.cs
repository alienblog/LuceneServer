﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using NetworkSocket.Fast;

namespace LuceneServer.Filters
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    internal class ExceptionFilter : FastFilterAttribute
    {
		readonly ILog _log = LogManager.GetLogger(typeof(LnServer));

		protected override void OnException(ExceptionContext filterContext)
        {
	        _log.Error(filterContext.Exception);
            filterContext.ExceptionHandled = true;
        }
    }
}
