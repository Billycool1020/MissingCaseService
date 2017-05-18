using HtmlAgilityPack;
using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MissingCaseService.BLL
{
    class IIS
    {
        public List<Threads> IISRoot()
        {

            List<Threads> list = new List<Threads>();
            var asproot = "https://forums.iis.net/";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(asproot);
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//td[@class='col1']").ToArray();
            foreach (HtmlNode item in nodes)
            {
                if (item.ChildNodes.Count > 1)
                {
                    var subhref = item.ChildNodes[0].FirstChild.Attributes["href"].Value;
                    var subhref1 = subhref.Split('?')[0];
                    subhref1 = subhref1.Remove(subhref1.Length - 1);
                    var subhref2 = subhref.Split('?')[1];

                    for (var i = 1; i < 2; i++)
                    {
                        var url = asproot + subhref1 + i.ToString() + "?";
                        list.AddRange(IISSub(url, subhref2));

                    }
                }
            }

            return list;
        }

        public List<Threads> IISSub(string url, string subhref)
        {
            List<Threads> list = new List<Threads>();
            PostedTime pt = new PostedTime();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url + subhref);
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//a[@title]").ToArray();
            string Product = subhref.Replace("+", " ");

            foreach (HtmlNode item in nodes)
            {

                if (item.Attributes["title"].Value.Contains("[Unanswered]") || item.Attributes["title"].Value.Contains("[Answered]"))
                {
                    Threads thread = new Threads();

                    string LastPostTime = item.ParentNode.NextSibling.NextSibling.ChildNodes[4].InnerText;
                    LastPostTime = LastPostTime.Remove(0, 1);
                    if (pt.RecentPost(LastPostTime))
                    {
                        Regex r = new Regex(@"/t/\d{7}");
                        var ThreadId = r.Match(item.Attributes["href"].Value).Value;
                        if (ThreadId.Length == 0)
                        {
                            r = new Regex(@"/t/\d{6}");
                            ThreadId = r.Match(item.Attributes["href"].Value).Value.Substring(3);
                        }
                        else
                        {
                            ThreadId = ThreadId.Substring(3);
                        }

                        XmlReader reader = XmlReader.Create("https://forums.iis.net/t/rss/" + ThreadId);
                        SyndicationFeed feed = SyndicationFeed.Load(reader);
                        reader.Close();
                        SyndicationItem i = feed.Items.FirstOrDefault();

                        string PostDate = i.LastUpdatedTime.DateTime.ToString();

                        if (pt.RecentPost(PostDate))
                        {
                            string Title = i.Title.Text;
                            string Link = "https://forums.iis.net";
                            Link = Link + item.Attributes["href"].Value;
                            thread.Link = Link;
                            thread.ThreadId = ThreadId;
                            thread.PostDate = i.LastUpdatedTime.DateTime;
                            thread.Title = Title;
                            thread.Product = Product;
                            list.Add(thread);

                            Console.WriteLine(ThreadId);
                            Console.WriteLine(Title);
                            Console.WriteLine(Product);
                            Console.WriteLine(PostDate);
                            Console.WriteLine(Link);
                            Console.WriteLine();
                        }
                    }


                }
            }

            return list;
        }
    }
}
