using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardMonitor.Common
{
	public static class LogHelper
	{
		private static string logFilePath = global::KeyboardMonitor.Properties.Resource.LogFilePath;

		public static object myLock = new object();
		static FileStream fileStream = null;
		/// <summary>
		/// 创建日志文件
		/// </summary>
		public static FileStream CreateLogFile()
		{
			return File.OpenWrite(logFilePath);
		}
		public static void Clear()
		{
			if (File.Exists(logFilePath))
			{
				File.Delete(logFilePath);
			}
		}
		/// <summary>
		/// 写入日志文件
		/// </summary>
		/// <param name="content"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void WriteLog(string content)
		{
			content = "\n" + content + "\n";
			fileStream = CreateLogFile();
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Close();
		}

		private static void WriteLogSuccess(IAsyncResult ar)
		{
			fileStream.Dispose();
			fileStream = null;
		}
	}
}
