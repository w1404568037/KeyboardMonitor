using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardMonitor.API
{
	public class WinNet
	{
		public static bool InternetCheckConnection()
		{
			if (InternetCheckConnection("http://www.baidu.com", 1, 0))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		[System.Runtime.InteropServices.DllImport("winInet.dll")]
		private extern static bool InternetCheckConnection(String url, int flag, int ReservedValue);
	}
}
