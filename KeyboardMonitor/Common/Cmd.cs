using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardMonitor.Common
{
	public class Cmd:IDisposable
	{
		public Process process =null;
		public Cmd()
		{
			this.InitCmd();
		}

		private void InitCmd()
		{
			process = new Process();
		}

		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <returns></returns>
		public string ExecCommand(string command)
		{
			return this.ExecCommand(command, false, true, true, true, true);
		}
		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <param name="UseShellExecute">是否使用操作系统shell启动</param>
		/// <returns></returns>
		public string ExecCommand(string command, bool UseShellExecute)
		{
			return this.ExecCommand(command, UseShellExecute, true, true, true, true);
		}
		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <param name="UseShellExecute">是否使用操作系统shell启动</param>
		/// <param name="RedirectStandardInput">接受来自调用程序的输入信息</param>
		/// <returns></returns>
		public string ExecCommand(string command, bool UseShellExecute, bool RedirectStandardInput)
		{
			return this.ExecCommand(command, UseShellExecute, RedirectStandardInput, true, true, true);
		}
		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <param name="UseShellExecute">是否使用操作系统shell启动</param>
		/// <param name="RedirectStandardInput">接受来自调用程序的输入信息</param>
		/// <param name="RedirectStandardOutput">由调用程序获取输出信息</param>
		/// <returns></returns>
		public string ExecCommand(string command, bool UseShellExecute, bool RedirectStandardInput, bool RedirectStandardOutput)
		{
			return this.ExecCommand(command, UseShellExecute, RedirectStandardInput, RedirectStandardOutput, true, true);
		}
		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <param name="UseShellExecute">是否使用操作系统shell启动</param>
		/// <param name="RedirectStandardInput">接受来自调用程序的输入信息</param>
		/// <param name="RedirectStandardOutput">由调用程序获取输出信息</param>
		/// <param name="RedirectStandardError">重定向标准错误输出</param>
		/// <returns></returns>
		public string ExecCommand(string command, bool UseShellExecute, bool RedirectStandardInput, bool RedirectStandardOutput, bool RedirectStandardError)
		{
			return this.ExecCommand(command, UseShellExecute, RedirectStandardInput, RedirectStandardOutput, RedirectStandardError, true);
		}
		/// <summary>
		/// 执行cmd命令
		/// </summary>
		/// <param name="command">需要执行的cmd命令</param>
		/// <param name="UseShellExecute">是否使用操作系统shell启动</param>
		/// <param name="RedirectStandardInput">接受来自调用程序的输入信息</param>
		/// <param name="RedirectStandardOutput">由调用程序获取输出信息</param>
		/// <param name="RedirectStandardError">重定向标准错误输出</param>
		/// <param name="CreateNoWindow">不显示程序窗口</param>
		/// <returns></returns>
		public string ExecCommand(string command,bool UseShellExecute,bool RedirectStandardInput,bool RedirectStandardOutput,bool RedirectStandardError,bool CreateNoWindow)
		{
			try
			{

				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
				process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
				process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
				process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
				process.StartInfo.CreateNoWindow = true;//不显示程序窗口
				process.Start();//启动程序

				//向cmd窗口发送输入信息
				process.StandardInput.WriteLine(command + "&exit");

				process.StandardInput.AutoFlush = true;
				//p.StandardInput.WriteLine("exit");
				//向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
				//同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

				//获取cmd窗口的输出信息
				string result = process.StandardOutput.ReadToEnd();
				process.WaitForExit();//等待程序执行完退出进程
				process.Close();
				return result;
			}
			catch (Exception)
			{

				throw;
			}
			finally
			{
				this.Dispose(true);
			}
		}
		~Cmd()
		{
			this.process = null;
			this.Dispose(true);
		}
		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: 释放托管状态(托管对象)。
				}

				// TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
				// TODO: 将大型字段设置为 null。

				disposedValue = true;
			}
		}

		// TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
		// ~Cmd() {
		//   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
		//   Dispose(false);
		// }

		// 添加此代码以正确实现可处置模式。
		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
			// TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
