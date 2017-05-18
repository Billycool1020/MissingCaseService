using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.BLL
{
    class Email
    {
        public void Sendmail(List<MissingCaseModel> List)
        {
            if (List.Count > 0)
            {
                var threads = from l in List
                              group l by l.Product into g
                              select g;
                string content = "<br /> ";
                content = content + "Missing Case:<br /> ";
                foreach (var thread in threads)
                {
                   
                    content = content + thread.Key + "<br />";

                    foreach (MissingCaseModel t in thread)
                    {
                        content = content + "<a href=" + t.Link + ">" + t.Title + "</a><br />";
                    }
                    content = content + "<br />";

                }
                Send("", content);
            }
        }

        public bool Send(string target, string content)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            smtpClient.EnableSsl = true;
            //mailMessage.To.Add(new MailAddress(target));
            //foreach (var t in teamleaderlist)
            //{
            //    mailMessage.CC.Add(new MailAddress(t));
            //}
            mailMessage.To.Add(new MailAddress("Jie.Rong@microsoft.com"));            
            mailMessage.To.Add(new MailAddress("v-yanjxu@microsoft.com"));
            mailMessage.To.Add(new MailAddress("v-waxia5@microsoft.com"));
            mailMessage.To.Add(new MailAddress("billyliu.cool@hotmail.com"));
            mailMessage.Subject = "Missing Case";
            mailMessage.Body = content;
            mailMessage.IsBodyHtml = true;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
