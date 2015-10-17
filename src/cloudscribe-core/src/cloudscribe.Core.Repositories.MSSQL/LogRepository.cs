﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
//	Author:                 Joe Audette
//  Created:			    2011-08-18
//	Last Modified:		    2015-10-16
// 

using cloudscribe.Core.Models.DataExtensions;
using cloudscribe.Core.Models.Logging;
using cloudscribe.DbHelpers.MSSQL;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace cloudscribe.Core.Repositories.MSSQL
{
    public class LogRepository : ILogRepository
    {
        public LogRepository(
            IOptions<MSSQLConnectionOptions> connectionOptions)
        {
            if (connectionOptions == null) { throw new ArgumentNullException(nameof(connectionOptions)); }
            //if (loggerFactory == null) { throw new ArgumentNullException(nameof(loggerFactory)); }

            //logFactory = loggerFactory;
            //log = loggerFactory.CreateLogger(typeof(GeoRepository).FullName);

            readConnectionString = connectionOptions.Value.ReadConnectionString;
            writeConnectionString = connectionOptions.Value.WriteConnectionString;

            // passing in null for loggerFactory because we don't want to allow debug sql logging for log related sql activity
            // it would cause a never ending loop condition if logging events generate more logging events
            dbSystemLog = new DBSystemLog(readConnectionString, writeConnectionString, null);
        }

        //private ILoggerFactory logFactory;
       // private ILogger log;
        private string readConnectionString;
        private string writeConnectionString;
        private DBSystemLog dbSystemLog;

        public int AddLogItem(
            DateTime logDate,
            string ipAddress,
            string culture,
            string url,
            string shortUrl,
            string thread,
            string logLevel,
            string logger,
            string message)
        {
            return  dbSystemLog.Create(
                logDate,
                ipAddress,
                culture,
                url,
                shortUrl,
                thread,
                logLevel,
                logger,
                message);
        }

        public async Task<int> GetCount()
        {
            return await dbSystemLog.GetCount();
        }

        public async Task<List<ILogItem>> GetPageAscending(
            int pageNumber,
            int pageSize)
        {
            List<ILogItem> logItems = new List<ILogItem>();
            using (DbDataReader reader = await dbSystemLog.GetPageAscending(pageNumber, pageSize))
            {
                while (reader.Read())
                {
                    LogItem logitem = new LogItem();
                    logitem.LoadFromReader(reader);
                    logItems.Add(logitem);
                }
            }

            return logItems;
        }

        public async Task<List<ILogItem>> GetPageDescending(
            int pageNumber,
            int pageSize)
        {
            List<ILogItem> logItems = new List<ILogItem>();
            using (DbDataReader reader = await dbSystemLog.GetPageDescending(pageNumber, pageSize))
            {
                while (reader.Read())
                {
                    LogItem logitem = new LogItem();
                    logitem.LoadFromReader(reader);
                    logItems.Add(logitem);
                }
            }

            return logItems;
        }

        public async Task<bool> DeleteAll()
        {
            return await dbSystemLog.DeleteAll();
        }

        public async Task<bool> Delete(int logItemId)
        {
            return await dbSystemLog.Delete(logItemId);
        }

        public async Task<bool> DeleteOlderThan(DateTime cutoffDateUtc)
        {
            return await dbSystemLog.DeleteOlderThan(cutoffDateUtc);
        }

        public async Task<bool> DeleteByLevel(string logLevel)
        {
            return await dbSystemLog.DeleteByLevel(logLevel);
        }

    }
}
