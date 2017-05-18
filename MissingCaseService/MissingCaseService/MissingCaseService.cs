using MissingCaseService.BLL;
using MissingCaseService.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService
{
    public partial class MissingCaseService : ServiceBase
    {
        public static int flag = 0;
        public MissingCaseService()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MissingCaseSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MissingCaseSource", "MissingCaseNewLog");
            }
            eventLog1.Source = "MissingCaseSource";
            eventLog1.Log = "MissingCaseNewLog";
        }
        
        protected override void OnStart(string[] args)
        {

            eventLog1.WriteEntry("In OnStart");            
            SaveMissingCase();
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 600000; // 600 seconds   600000
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {

            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("In OnTimer  Time: " + DateTime.Now.ToString(), EventLogEntryType.Information);
            //getNewCaseList();
            var hour = DateTime.Now.Hour;
            if (flag == 0)
            {
               
                if (hour == 5 || hour== 9|| hour==16)
                {
                    SaveMissingCase();
                    flag++;
                }
            }else if (hour == 6 || hour == 10 || hour == 17)
            {
                flag--;
            }
        }

        public void SaveMissingCase()
        {
            
            List<MissingCaseModel> MissingList = new List<MissingCaseModel>();
            MissingCase mc = new MissingCase();
            MissingList = mc.getMissingCase();
            eventLog1.WriteEntry("Before Check:"+MissingList.Count.ToString());
            MissingList = mc.CheckMissingCase(MissingList);
            eventLog1.WriteEntry("After Check:" + MissingList.Count.ToString());
            Database data = new Database();

            data.Save(MissingList);
            Email email = new Email();
            email.Sendmail(MissingList);
            eventLog1.WriteEntry("Sendmail"+DateTime.Now.ToUniversalTime().ToString());
        }

        static void getNewCaseList()
        {

            List<Threads> List = new List<Threads>();
            Console.WriteLine(DateTime.Now.ToUniversalTime());
            Msdn msdn = new Msdn();
            List.AddRange(msdn.MsdnRoot());

            Asp asp = new Asp();
            List.AddRange(asp.AspRoot());

            IIS iis = new IIS();
            List.AddRange(iis.IISRoot());

            Console.WriteLine(DateTime.Now.ToUniversalTime());
            Database data = new Database();
            data.Save(List);
        }


        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }
    }
}
