using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardMonitor
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			while (true)
			{
				try
				{
					//窗体打开
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					ServiceBase[] ServicesToRun;
					ServicesToRun = new ServiceBase[]
					{
					new KeyboardMonitorService(),
					};
					ServiceBase.Run(ServicesToRun);
				}
				catch (Exception)
				{
					
				}
			}
		}
	}
}
