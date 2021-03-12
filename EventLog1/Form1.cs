using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Security;
using System.Globalization;


namespace EventLog1
{

    // количество событий (записей) которое хронит windows для каждого Event Source ~= 3345 (~512 kb)

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            CurrentEventSourceCreationData = new EventSourceCreationData(Sourse, LogName);
            CurrentEventSourceCreationData.MachineName = Machine;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Create_the_source();
        }

        string Sourse  =    "MaxiGraf_Log_Data";
        string LogName =    "MaxiGraf";
        string Machine =    ".";
        Int64 sizeKB   =    1048576; //1GB

        const int ConstRetentionDays = 7;

        int CurrentRetentionDays = ConstRetentionDays;


        OverflowAction CurrentOverflowAction = OverflowAction.OverwriteAsNeeded;

        EventSourceCreationData CurrentEventSourceCreationData;

        public void Create_the_source()
        {
           

            if (!EventLog.SourceExists(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName))
            {               
                EventLog.CreateEventSource(CurrentEventSourceCreationData);
                Console.WriteLine("CreatingEventSource");
                Console.WriteLine("Exiting, execute the application a second time to use the source.");


                if (EventLog.SourceExists(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName))
                {
                    EventLog[] eventLogs = EventLog.GetEventLogs();

                    foreach (EventLog log in eventLogs)
                    {
                        Console.WriteLine("Log: " + log.Log);
                        Console.WriteLine("LogDisplayName: " + log.LogDisplayName);
                        Console.WriteLine("OverflowAction: " + log.OverflowAction);

                        if (log.Log.ToString() == CurrentEventSourceCreationData.LogName)
                        {

                            if(CurrentOverflowAction == OverflowAction.DoNotOverwrite)
                            {
                                if (CurrentRetentionDays < 1 || CurrentRetentionDays > 365)
                                    CurrentRetentionDays = ConstRetentionDays;
                            }

                            log.ModifyOverflowPolicy(CurrentOverflowAction, CurrentRetentionDays);
                            log.MaximumKilobytes = sizeKB;

                            Console.WriteLine("*Log: " + log.Log);
                            Console.WriteLine("*LogDisplayName: " + log.LogDisplayName);
                            Console.WriteLine("*OverflowAction: " + log.OverflowAction);
                        }
                    }
                }

                 return;
            }


            EventLog myLog = new EventLog(CurrentEventSourceCreationData.LogName, CurrentEventSourceCreationData.MachineName, CurrentEventSourceCreationData.Source);

            byte[] arrProp = null;

         



            for (int i = 0; i < 3 * 1000_000; i++)
            {


              
             
                arrProp = BitConverter.GetBytes(DateTime.Now.Ticks);

                myLog.WriteEntry(
                    "Writing to event log. From vados." + " # " + i.ToString(),
                    EventLogEntryType.Information,
                    22,
                    1,
                    arrProp
                    );
                Console.WriteLine("Writing to event log. From vados." + " # " + i.ToString());               
            }

          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Delete_the_source();
        }

        public void Delete_the_source()
        {
            

            string logName;

            if (EventLog.SourceExists(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName))
            {
                // Find the log associated with this source.
                logName = EventLog.LogNameFromSourceName(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName);
                // Make sure the source is in the log we believe it to be in.
                if (logName != CurrentEventSourceCreationData.LogName)
                    return;
                // Delete the source and the log.
                EventLog.DeleteEventSource(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName);
                EventLog.Delete(logName, CurrentEventSourceCreationData.MachineName);

                Console.WriteLine(logName + " deleted.");
            }
            else
            {
              
                EventLog.CreateEventSource(CurrentEventSourceCreationData);
            }







        }

        private void button3_Click(object sender, EventArgs e)
        {
            Read_the_source();
        }

        public void Read_the_source()
        {
            EventLog[] remoteEventLogs;

            remoteEventLogs = EventLog.GetEventLogs();

            Console.WriteLine("Number of logs on computer: " + remoteEventLogs.Length);

            DateTime time = DateTime.Now;
            List<string> list = new List<string>();
            foreach (EventLog log in remoteEventLogs)
            {
                Console.WriteLine("Log: " + log.Log);


                

                if (log.Log.ToString() == CurrentEventSourceCreationData.LogName)
                {
                    int count = log.Entries.Count;

                    if (count < 1) return;

                    var a = new EventLogEntry[count];
                    log.Entries.CopyTo(a, 0);


                    //  list.AddRange(a.ToList().ConvertAll(new Converter<EventLogEntry, string>(GetString)));

                    Console.WriteLine(a[0].TimeGenerated);
                    Console.WriteLine(a[0].TimeWritten);
                    Console.WriteLine(a[0].Message);
                    Console.WriteLine(a[0].UserName); //Это свойство часто является пустым для событий в журналах, отличных от журнала безопасности.
                    Console.WriteLine(a[0].Source);
                    Console.WriteLine(a[0].MachineName);



                    long longVar = BitConverter.ToInt64(log.Entries[0].Data, 0);
                    DateTime dateTimeVar = DateTime.FromBinary(longVar);

                    Console.WriteLine(dateTimeVar);
                  
                }


            }
            System.Console.WriteLine((DateTime.Now - time).TotalSeconds);


        }



        public string GetString(EventLogEntry ELE)
        {
            return ELE.Message;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clear_the_source();
        }


        public void Clear_the_source()
        {


            string logName;


            if (EventLog.SourceExists(CurrentEventSourceCreationData.Source, CurrentEventSourceCreationData.MachineName))
            {

                EventLog[] remoteEventLogs;

                remoteEventLogs = EventLog.GetEventLogs();

                Console.WriteLine("Number of logs on computer: " + remoteEventLogs.Length);

              
                foreach (EventLog log in remoteEventLogs)
                {
                    Console.WriteLine("Log: " + log.Log);

                    if (log.Log.ToString() == CurrentEventSourceCreationData.LogName)
                    {

                        log.Clear();

                        Console.WriteLine(log.Log.ToString() + " Clear.");
                    }
                }


               

            }
        }



       public void Session()
       {
           

            string queryString = "*";

            SecureString kod = new SecureString();

            char[] sp = (
              
               "1"
                ).ToCharArray();

            for(int i = 0; i < sp.Length; i ++)
            kod.AppendChar(sp[i]);
           
          
            
            var beforeCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

               

                string domain = 
                    "747-ПК";
              

                string username =
               
                "EventReader";

                string logname = "MaxiGraf";


                EventLogSession S = new EventLogSession(
                domain,
                domain,
                username,
                kod, 
                SessionAuthentication.Default);


                kod.Dispose();

                EventLogConfiguration logCon = new EventLogConfiguration(logname, S);

               

                List<string> d = new List<string>();

                d.AddRange(S.GetLogNames());

                var t = logCon.ProviderNames;



                EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);


                eventsQuery.Session = S;
                eventsQuery.TolerateQueryErrors = false;
              








                using (EventLogReader logReader = new EventLogReader(eventsQuery))
                {

                    List<EventRecord> eventList = new List<EventRecord>();

                    EventRecord eventInstance = logReader.ReadEvent();

                    try 
                    {
                        for (; null != eventInstance; eventInstance = logReader.ReadEvent())
                        {
                            eventList.Add(eventInstance);
                            Debug.WriteLine(eventInstance.Properties[0].Value.ToString());
                            Debug.WriteLine(Encoding.UTF8.GetString((byte[])eventInstance.Properties[1].Value));
                        }
                    }
                    finally
                    {
                        if (eventInstance != null)
                            eventInstance.Dispose();
                    }
                }



                   


            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = beforeCulture;
            }



        }

        private void button5_Click(object sender, EventArgs e)
        {
            Session();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Watcher();
        }


        public void Watcher()
        {
            SubscribeToEventsExample eventWatcher = new SubscribeToEventsExample();

        }





        public class SubscribeToEventsExample
    {
        public SubscribeToEventsExample()
        {
                EventLogWatcher watcher;
                watcher = null;


                //EventLogWatcher watcher2;
                //watcher2 = null;


                try
                {

                


                    SecureString kod = new SecureString();

                    char[] sp = (
                    
                       "1"
                        ).ToCharArray();

                    for (int i = 0; i < sp.Length; i++)
                        kod.AppendChar(sp[i]);

                  


                    var beforeCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {

                       

                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                        

                        string domain =
                      
                        "IVS-PC";

                        string username =
                       
                        "AD";
                       

                        string logname = "MaxiGraf";

                       
                        EventLogSession S = new EventLogSession(
                        domain,
                        domain,
                        username,
                        kod,
                        SessionAuthentication.Default);


                        kod.Dispose();

                        EventLogConfiguration logCon = new EventLogConfiguration(logname, S);



                        List<string> d = new List<string>();

                        d.AddRange(S.GetLogNames());

                        var t = logCon.ProviderNames;



                        EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);


                        eventsQuery.Session = S;
                        eventsQuery.TolerateQueryErrors = false;




                        watcher = new EventLogWatcher(eventsQuery);

                           watcher.EventRecordWritten +=
                        new EventHandler<EventRecordWrittenEventArgs>(
                            HandleEvent);


                        watcher.Enabled = true;

                        Console.WriteLine("Waiting for events...");

                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = beforeCulture;
                    }
                  


                }
            catch (EventLogReadingException e)
            {
                Console.WriteLine("Error reading the log: {0}", e.Message);
            }

            finally
            {

                   
             }
        }

        // <summary>
        // Callback method that gets executed when an event is
        // reported to the subscription.
        // </summary>
        public void HandleEvent(object obj, EventRecordWrittenEventArgs arg)
        {

                // Make sure there was no error reading the event.
                if (arg.EventRecord != null)
                {
                    Console.WriteLine("Received event {0} from the subscription.", arg.EventRecord.Id);
                    Console.WriteLine("Description: {0}", arg.EventRecord.FormatDescription());

                    Debug.WriteLine(arg.EventRecord.Properties[0].Value.ToString());
                    Debug.WriteLine(Encoding.UTF8.GetString((byte[])(arg.EventRecord.Properties[1].Value)));

                 
                }
                else
                {
                    Console.WriteLine("The event instance was null.");
                }
        }

        
    }


        public class EventLogSubscription
        {
           
            
            public string domain_name;
            public string user_name;
            public string password;
            public string log_nаme;

            public const string Query_Const = "*";

            public string query = "*";

            public EventLogSubscription()
            {
            }

            public EventLogSubscription
                                        (
                                            
                                          string Domain_name,
                                          string User_name,
                                          string Password,
                                         
                                          string Log_nаme,
                                          string Query = Query_Const
                                        )
            {
                   
                     password = Password;
                     domain_name = Domain_name;
                     user_name = User_name;
                     log_nаme = Log_nаme;
                     password = Password;
                     query = Query;
            }


            public SecureString GetPreparedPassword()
            {
                SecureString kod = new SecureString();

                char[] sp = password.ToCharArray();

                for (int i = 0; i < sp.Length; i++)
                    kod.AppendChar(sp[i]);

                return kod;
            }
        }


        public List<EventLogSubscription> EventLogCollectionSubscription = new List<EventLogSubscription>();

        private void button7_Click(object sender, EventArgs e)
        {
            WatcherS();
        }



        public void WatcherS()
        {
            EventLogCollectionSubscription.Add(new EventLogSubscription("747-ПК", "EventReader", "1", "MaxiGraf"));

            EventLogCollectionSubscription.Add(new EventLogSubscription("DESKTOP-PMFSLPC", "EventReader", "1", "MaxiGrafTEST1"));


            List<EventLogWatcher> watcherS = new List<EventLogWatcher>();


            try
            {


                var beforeCulture = Thread.CurrentThread.CurrentCulture;

                try
                {



                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");


                    for (int i = 0; i < EventLogCollectionSubscription.Count; i++)
                    {

                        EventLogSession S = new EventLogSession(
                        EventLogCollectionSubscription[i].domain_name,
                        EventLogCollectionSubscription[i].domain_name,
                        EventLogCollectionSubscription[i].user_name,
                        EventLogCollectionSubscription[i].GetPreparedPassword(),
                        SessionAuthentication.Default);


                       

                        EventLogConfiguration logCon = new EventLogConfiguration(EventLogCollectionSubscription[i].log_nаme, S);



                        List<string> d = new List<string>();

                        d.AddRange(S.GetLogNames());

                        var t = logCon.ProviderNames;



                        EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);//, queryString);


                        eventsQuery.Session = S;
                        eventsQuery.TolerateQueryErrors = false;


                        watcherS.Add(new EventLogWatcher(eventsQuery));

                  

                        watcherS[watcherS.Count - 1].EventRecordWritten +=
                            new EventHandler<EventRecordWrittenEventArgs>(
                                HandleEventWatcherS);

                        watcherS[watcherS.Count - 1].Enabled = true;

                        Console.WriteLine("Waiting for events... " + i.ToString());
                    }

                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = beforeCulture;
                }

            }
            catch (EventLogReadingException e)
            {
                Console.WriteLine("Error reading the log: {0}", e.Message);
            }
            finally
            {

            }




        }

        // <summary>
        // Callback method кода в полписку приходит событие
        // </summary>
        public void HandleEventWatcherS(object obj, EventRecordWrittenEventArgs arg)
        {

            // Make sure there was no error reading the event.
            if (arg.EventRecord != null)
            {
                Console.WriteLine("Received event {0} from the subscription.", arg.EventRecord.Id);
                Console.WriteLine("Description: {0}", arg.EventRecord.FormatDescription());

                Debug.WriteLine(arg.EventRecord.Properties[0].Value.ToString());
                Debug.WriteLine(Encoding.UTF8.GetString((byte[])(arg.EventRecord.Properties[1].Value)));

               
            }
            else
            {
                Console.WriteLine("The event instance was null.");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "netplwiz";
            cmd.Start();

        }
    }
}
