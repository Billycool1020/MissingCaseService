using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.DAL
{
    class MissingCaseDataContext :DbContext
    {
        public MissingCaseDataContext()
           : base("name=MissingCaseDataContext")
        {
            Database.SetInitializer<MissingCaseDataContext>(null);
        }
        public virtual DbSet<Threads> Threads { get; set; }
        public virtual DbSet<MissingCaseModel> MissingCaseModels { get; set; }
    }
}
