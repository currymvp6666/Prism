using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Prism.Services
{
    public class MailService
    {
        // 配置你的发件人信息
        private readonly string _senderEmail = "2910882638@qq.com";
        private readonly string _authCode = "lubyzuhpvcimddih";
        private readonly string _smtpServer = "smtp.qq.com";

        public async Task<(bool success, string message)> SendCodeAsync(string targetEmail, string code)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_senderEmail, "系统管理中心");
                    mail.To.Add(targetEmail);
                    mail.Subject = "重置密码身份验证";
                    mail.Body = $@"
                        <h3>您好：</h3>
                        <p>您正在进行找回密码操作，您的验证码为：<br/>
                        <b style='font-size:20px; color:#007AFF;'>{code}</b></p>
                        <p>验证码在 5 分钟内有效，请勿泄露给他人。</p>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(_smtpServer, 587))
                    {
                        smtp.Credentials = new NetworkCredential(_senderEmail, _authCode);
                        smtp.EnableSsl = true; // 必须开启 SSL
                        await smtp.SendMailAsync(mail);
                    }
                }
                return (true, "发送成功");
            }
            catch (Exception ex)
            {
                return (false, $"发送失败: {ex.Message}");
            }
        }
    }
}