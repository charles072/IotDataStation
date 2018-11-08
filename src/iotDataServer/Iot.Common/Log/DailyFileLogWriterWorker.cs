using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Iot.Common.Util;

namespace Iot.Common.Log
{
	public class DailyFileLogWriterWorker
	{
		private static DailyFileLogWriterWorker _instance = null;
	    private static Regex _regexYearMonthOnlog = new Regex("_(?<yearMonth>[0-9]+-[0-9]+)-[0-9]+.log", RegexOptions.Compiled |RegexOptions.IgnoreCase);

        private readonly ConcurrentQueue<FileLogEvent> _logQueue = null;
		private int _intIsWorkerRunnung = 0;
		private int _intHasItem = 0;
		private readonly object _lockObject = new object();
		private readonly object _lockBackup = new object();
        private string _lastFileCheckYearMonth = "";
        private List<string> _logFolerList = new List<string>();


        private DailyFileLogWriterWorker()
		{
			_logQueue = new ConcurrentQueue<FileLogEvent>();
		}

		public static DailyFileLogWriterWorker GetInstance()
		{
			return _instance ?? (_instance = new DailyFileLogWriterWorker());
		}
        public void AddLogFolder(string logFolder)
	    {
	        lock (_logFolerList)
	        {
	            if (!_logFolerList.Contains(logFolder))
	            {
	                _logFolerList.Add(logFolder);
                }
	        }
	    }

        public void Enqueue(FileLogEvent fileLogEvent)
		{
			_logQueue.Enqueue(fileLogEvent);
			WorkerStart();
		}

		private void WorkerStart()
		{
		    int hadItem = Interlocked.Exchange(ref _intHasItem, 1);
			if (hadItem == 0)
			{
				Task.Factory.StartNew(DoWork);
			}
		}

		private void DoWork()
		{
		    int wasWorkerRunnung = Interlocked.Exchange(ref _intIsWorkerRunnung, 1);

		    if (wasWorkerRunnung > 0)
		    {
		        return;
            }

			while (_logQueue.TryDequeue(out FileLogEvent fileLogEvent))
			{
			    LogEvent logEvent = fileLogEvent.LogEvent;

                string logMessage = $"{logEvent.TimeStampUtc.ToLocalTime():HH:mm:ss}: [{logEvent.ModuleName}][{logEvent.LogLevel}] {logEvent.Message}";
                if (logEvent.Exception != null)
				{
					logMessage += $"{Environment.NewLine}{logEvent.Exception}";
				}

				using (StreamWriter writer = new StreamWriter(fileLogEvent.Filename, true))
				{
					writer.WriteLine(logMessage);
				}
			}

		    Interlocked.Exchange(ref _intIsWorkerRunnung, 0);
		    Interlocked.Exchange(ref _intHasItem, 0);

            lock (_lockBackup)
			{
				CheckLogFile();
			}
		}

		private void CheckLogFile()
		{
		    string currenYearMonth = CachedDateTime.Now.ToString("yyyy-MM");
		    if (currenYearMonth == _lastFileCheckYearMonth)
		    {
		        return;
		    }
		    _lastFileCheckYearMonth = currenYearMonth;

		    string[] logFolders;
		    lock (_logFolerList)
		    {
		        logFolders = _logFolerList.ToArray();
		    }

		    if (logFolders?.Length == 0)
		    {
		        return;
		    }

            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            string zipYearMonth = (CachedDateTime.Now.AddMonths(-1)).ToString("yyyy-MM");
            foreach (string logFolder in _logFolerList)
		    {
		        MakeZip(logFolder, zipYearMonth, appName);
		    }
        }

	    private void MakeZip(string logFolder, string zipYearMonth, string appName)
	    {

	        string zipCreatePath = Path.Combine(logFolder, $"{appName}_log_{zipYearMonth}.zip");

	        if (File.Exists(zipCreatePath))
	        {
	            return;
	        }
	        using (ZipArchive archive = ZipFile.Open(zipCreatePath, ZipArchiveMode.Create))
	        {
	            foreach (string fileFullpath in Directory.GetFiles(logFolder, "*.log", SearchOption.TopDirectoryOnly))
	            {
	                try
	                {
	                    Match match = _regexYearMonthOnlog.Match(fileFullpath);
	                    if (match.Success)
	                    {
	                        string fileYearMonth = match.Groups["yearMonth"].Value;
	                        if (fileYearMonth.CompareTo(zipYearMonth) <= 0)
	                        {
	                            string fileName = Path.GetFileName(fileFullpath);
	                            archive.CreateEntryFromFile(fileFullpath, fileName);
	                            File.Delete(fileFullpath);
	                        }
	                    }
	                }
                    catch
	                {
	                }
	            }
	        }
	    }
	}
}
