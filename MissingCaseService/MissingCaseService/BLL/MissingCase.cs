using MissingCaseService.DAL;
using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.BLL
{
    class MissingCase
    {
        MissingCaseDataContext db = new MissingCaseDataContext();
        public List<MissingCaseModel> getMissingCase()
        {
            List<MissingCaseModel> MissingList = new List<MissingCaseModel>();
            var now = DateTime.Now.ToUniversalTime().AddHours(8).ToString();
            DataTable dt = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["CA"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("select t.QuestionId,t.Title,t.[Url],p.DisplayName,t.CreatedOn from [tbl_instances] as t join tbl_products as p on t.ProductId = p.Id where PlatformId = '3' and t.CreatedOn > GETDATE() - 2 order by t.CreatedOn desc"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }


            DataTable CAT = new DataTable();
            string constr2 = ConfigurationManager.ConnectionStrings["CAT"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr2))
            {
                using (SqlCommand cmd = new SqlCommand("select cat_externalid from cat_thread where cat_externalcreatedon > GETUTCDATE() - 2 order by cat_externalcreatedOn"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 600;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(CAT);
                    }
                }
            }

            var cat = new List<string>();
            foreach (DataRow t in CAT.Rows)
            {
                cat.Add(t[0].ToString());
            }
            HttpWebRequest myHttpWebRequest;
            foreach (DataRow t in dt.Rows)
            {
                if (!cat.Contains(t[0]))
                {
                    if (t[2].ToString().Contains("social.technet.microsoft.com"))
                    {
                        continue;
                    }
                    try
                    {
                        myHttpWebRequest = (HttpWebRequest)WebRequest.Create(t[2].ToString().Replace("&outputAs=xml", ""));
                        // Sends the HttpWebRequest and waits for a response.
                        HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                        myHttpWebResponse.Close();
                        MissingCaseModel MC = new MissingCaseModel();
                        MC.IsInsetintoCAT = false;
                        MC.Link = t[2].ToString().Replace("&outputAs=xml", "");
                        MC.PostDate = (DateTime)t[4];
                        MC.Product = t[3].ToString();
                        MC.ThreadId = t[0].ToString();
                        MC.Title = t[1].ToString();
                        MissingList.Add(MC);
                    }
                    catch
                    {
                        
                    }
                    
                }
            }

            return MissingList;
        }

        public List<MissingCaseModel> CheckMissingCase(List<MissingCaseModel>list)
        {
            List<MissingCaseModel> MissingList = new List<MissingCaseModel>();
            var date = DateTime.UtcNow.AddDays(-2);
            var dblist = db.MissingCaseModels.Where(m => m.PostDate > date).Select(x => x.ThreadId).ToList();

            foreach (MissingCaseModel mc in list)
            {
                if (!dblist.Contains(mc.ThreadId))
                {
                    MissingList.Add(mc);
                }
            }

            return MissingList;


        }






    }
}
