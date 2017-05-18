using MissingCaseService.DAL;
using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.BLL
{
    class Database
    {

        MissingCaseDataContext db = new MissingCaseDataContext();
        public void Save(List<Threads> list)
        {
            if (list.Count > 0)
            {
                db.Threads.AddRange(list);
                db.SaveChanges();
            }

        }

        public void Save(List<MissingCaseModel> list)
        {
            if (list.Count > 0)
            {
                db.MissingCaseModels.AddRange(list);
                db.SaveChanges();
            }

        }
    }
}
