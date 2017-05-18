using HtmlAgilityPack;
using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.BLL
{
    class Msdn
    {
        public List<Threads> MsdnRoot()
        {
            List<Threads> list = new List<Threads>();

            List<string> forums = new List<string>();
            forums.Add("wcf");
            foreach (var f in forums)
            {
                for (var i = 1; i < 2; i++)
                {
                    list.AddRange(MsdnSub(f, i));
                }

            }
            return list;
        }
        public List<Threads> MsdnSub(string Forum, int page)
        {
            List<Threads> list = new List<Threads>();
            PostedTime pt = new PostedTime();
            string time;
            var msdnroot = "https://social.msdn.microsoft.com/Forums/en-US/home?forum=";
            var fliterstring = "&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page=";
            var url = msdnroot + Forum + fliterstring + page.ToString();
            url = "https://social.msdn.microsoft.com/Forums/en-US/home?filter=alltypes&sort=firstpostdesc&brandIgnore=true&page=" + page;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//a[@data-block='main']").ToArray();
            foreach (HtmlNode item in nodes)
            {
                HtmlNode timenode = item.ParentNode.ParentNode.SelectSingleNode("div[3]/span[7]/span[2]");
                if (timenode.InnerText.Split(',').Length > 1)
                {
                    time = timenode.InnerText.Split(',')[1];
                }
                else
                {
                    time = timenode.InnerText;
                }
                if (pt.RecentPost(time))
                {
                    Threads thread = new Threads();
                    thread.Link = item.Attributes["href"].Value;
                    thread.PostDate = pt.Caltime(time);
                    thread.Product = Forum;
                    thread.ThreadId = item.Attributes["data-threadId"].Value;
                    thread.Title = item.InnerText;
                    list.Add(thread);

                    Console.WriteLine(item.InnerText);
                    Console.WriteLine(pt.Caltime(time));
                    Console.WriteLine();
                }
            }
            return list;
        }
    }
}
