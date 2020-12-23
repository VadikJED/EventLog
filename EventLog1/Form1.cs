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

    // количество событий (записей) которое хронит windows для каждого Event Source ~= 3345

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
            //// Create the source, if it does not already exist.
            //if (!EventLog.SourceExists(Sourse))
            //{
            //    //An event log source should not be created and immediately used.
            //    //There is a latency time to enable the source, it should be created
            //    //prior to executing the application that uses the source.
            //    //Execute this sample a second time to use the new source.
            //    EventLog.CreateEventSource(Sourse, LogName);
            //    Console.WriteLine("CreatedEventSource");
            //    Console.WriteLine("Exiting, execute the application a second time to use the source.");
            //    // The source is created.  Exit the application to allow it to be registered.
            //    return;
            //}

            //// Create an EventLog instance and assign its source.
            //EventLog myLog = new EventLog();
            //myLog.Source = Sourse;

            //// Write an informational entry to the event log.
            //myLog.WriteEntry("Writing to event log. From vados. START");

            //for (int i = 0; i < 1000; i++)
            //{
            //        myLog.WriteEntry("Writing to event log. From vados." + " # " + i.ToString());
            //        Console.WriteLine("Writing to event log. From vados." + " # " + i.ToString());
            //       // Thread.Sleep(50);
            //}

            //myLog.WriteEntry("Writing to event log. From vados. END");

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

           // myLog.WriteEntry("Writing to event log. From vados. START");



            for (int i = 0; i < 3 * 1000_000; i++)
            {


              
               // DateTime dt = Convert.ToDateTime(objDevice.deviceRecord.Current_date_time);
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

          //  myLog.WriteEntry("Writing to event log. From vados. END");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Delete_the_source();
        }

        public void Delete_the_source()
        {
            //string logName;

            //if (EventLog.SourceExists(Sourse))
            //{
            //    // Find the log associated with this source.
            //    //logName = EventLog.LogNameFromSourceName("MySource_VADOS");
            //    //// Make sure the source is in the log we believe it to be in.
            //    //if (logName != "MyNewLog_VADOS")
            //    //    return;
            //    // Delete the source and the log.
            //    EventLog.DeleteEventSource(Sourse);
            //    EventLog.Delete(LogName);

            //    Console.WriteLine(LogName + " deleted.");
            //}
            //else
            //{
            //    // Create the event source to make next try successful.
            //    //EventSourceCreationData mySourceData = new EventSourceCreationData("MySource_VADOS", "MyNewLog_VADOS");
            //    //mySourceData.MachineName = "MyNewLog_VADOS";
            //    //EventLog.CreateEventSource(mySourceData);
            //}


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
                //    // Create the event source to make next try successful.
                //    EventSourceCreationData mySourceData = new EventSourceCreationData("MySource", "MyLog");
                //    mySourceData.MachineName = "MyMachine";
                //    EventLog.CreateEventSource(mySourceData);
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
                    //for (int i = 0; i < a.Length; i++)
                    //{
                    //    //list.Add(a[i].Message);
                    //    //Console.WriteLine(i);
                    //    //
                    //}
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
            // SecureString kod = new SecureString();

            //kod.AppendChar('3');
            //kod.AppendChar('5');
            //kod.AppendChar('2');
            //kod.AppendChar('6');
            //kod.AppendChar('3');
            //kod.AppendChar('5');
            //kod.AppendChar('2');
            //kod.AppendChar('6');

            //EventLogSession S = new EventLogSession("747-ПК", "747-ПК", "747", kod, SessionAuthentication.Default);

            string queryString = "*";//"*[UserData/*]";// "*";//"*[System/Level=2]"; // XPATH Query

            SecureString kod = new SecureString();

            char[] sp = (
                //"35263526"
               // "777"
               // "lieksa2010"
               "1"
                ).ToCharArray();

            for(int i = 0; i < sp.Length; i ++)
            kod.AppendChar(sp[i]);
           
            ////kod.AppendChar('6');
            ////kod.AppendChar('3');
            ////kod.AppendChar('5');
            ////kod.AppendChar('2');
            ////kod.AppendChar('6');



            // Get log information.
            //                
            //EventLogSession session = new EventLogSession();
            //EventLogInformation logInfo = session.GetLogInformation("MaxiGraf222", PathType.LogName);
            //Console.WriteLine("The {0} log contains {1} events.", "MaxiGraf222", logInfo.RecordCount);

            
            var beforeCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                //using (var session = new EventLogSession("DESKTOP-PMFSLPC", "DESKTOP-PMFSLPC", /*"FEDOR"*/"Fedor", kod, SessionAuthentication.Default)
                //{
                //    var query = new EventLogQuery("System", PathType.LogName, queryString)
                //    {
                //        ReverseDirection = true,
                //        Session = session
                //    };

                //    using (var reader = new EventLogReader(query))
                //    {
                //        for (var record = reader.ReadEvent(); record != null; record = reader.ReadEvent())
                //        {
                //            // Read event records
                //            string message = record.FormatDescription();
                //        }
                //    }
                //}

                string domain = 
                    "747-ПК";
               // "DESKTOP-PMFSLPC";
                //"IVS-PC";

                string username =
                //"747";
                //"Fedor";
                //"EventLog";

                // "IVS";
                //"AD";
                "EventReader";

                string logname = "MaxiGraf";

                //EventLogPermission eventLogPermission = new EventLogPermission(EventLogPermissionAccess.Administer, domain);
                //eventLogPermission.Assert();

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



                EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);//, queryString);


                eventsQuery.Session = S;
                eventsQuery.TolerateQueryErrors = false;
                // eventsQuery.ReverseDirection = true;








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



                   


            // Debug.WriteLine(logReader.BatchSize);

            //int numberOfEvents = 0;

            //EventRecord eventInstance = logReader.ReadEvent();

            //Debug.WriteLine(eventInstance.ToString());

            //Debug.WriteLine(eventInstance.Properties[0].Value.ToString());
            //Debug.WriteLine(Encoding.UTF8.GetString((byte[])eventInstance.Properties[1].Value));

            //while (eventInstance != null)
            //{
            //    eventInstance = logReader.ReadEvent();

            //    if (eventInstance == null) break;

            //    Debug.WriteLine(eventInstance.Properties[0].Value.ToString());
            //    Debug.WriteLine(Encoding.UTF8.GetString((byte[])eventInstance.Properties[1].Value));
            //}

                //try
                //{
                //    S.ExportLog("MaxiGraf", PathType.LogName, "*", "@C:\archivedLog.evtx", true);
                //}
                //catch (Exception ex)
                //{
                //    Debug.WriteLine(ex.ToString());
                //}

                //try
                //{

                //    S.ExportLogAndMessages("MaxiGraf", PathType.LogName, "*", "@C:\archivedLog.evtx", true, CultureInfo.CurrentCulture);
                //}
                //catch(Exception ex)
                //{
                //    Debug.WriteLine(ex.ToString());
                //}

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


                EventLogWatcher watcher2;
                watcher2 = null;


                try
                {

                    // Subscribe to receive event notifications
                    // in the Application log. The query specifies
                    // that only level 2 events will be returned.
                    // EventLogQuery subscriptionQuery = new EventLogQuery("Application", PathType.LogName, "*[System/Level=2]");

                    //    SecureString kod = new SecureString();

                    //    kod.AppendChar('3');
                    //    kod.AppendChar('5');
                    //    kod.AppendChar('2');
                    //    kod.AppendChar('6');
                    //    kod.AppendChar('3');
                    //    kod.AppendChar('5');
                    //    kod.AppendChar('2');
                    //    kod.AppendChar('6');

                    //    EventLogSession S = new EventLogSession("747-ПК", "747-ПК", "747", kod, SessionAuthentication.Default);

                    //    EventLogQuery eventsQuery = new EventLogQuery("MaxiGraf", PathType.LogName);


                    //    eventsQuery.Session = S;
                    //    eventsQuery.TolerateQueryErrors = true;

                    //    EventLogReader logReader = new EventLogReader(eventsQuery);


                    //   watcher = new EventLogWatcher(eventsQuery);

                    //    // Set watcher to listen for the EventRecordWritten
                    //    // event.  When this event happens, the callback method
                    //    // (EventLogEventRead) will be called.
                    //    //watcher.EventRecordWritten += HandleEvent();
                    //    watcher.EventRecordWritten +=
                    //new EventHandler<EventRecordWrittenEventArgs>(
                    //    HandleEvent);

                    //    // Begin subscribing to events the events








                    //  SecureString kod2 = new SecureString();

                    //  kod2.AppendChar('7');
                    //  kod2.AppendChar('7');
                    //  kod2.AppendChar('7');
                    //  //kod.AppendChar('6');
                    //  //kod.AppendChar('3');
                    //  //kod.AppendChar('5');
                    //  //kod.AppendChar('2');
                    //  //kod.AppendChar('6');

                    //  EventLogSession S2 = new EventLogSession("DESKTOP-PMFSLPC", "DESKTOP-PMFSLPC", "Fedor", kod2, SessionAuthentication.Default);

                    //  EventLogQuery eventsQuery2 = new EventLogQuery("MaxiGraf222", PathType.LogName);


                    //  eventsQuery2.Session = S2;
                    //  eventsQuery2.TolerateQueryErrors = true;

                    ////  EventLogReader logReader2 = new EventLogReader(eventsQuery2);


                    //  watcher2 = new EventLogWatcher(eventsQuery2);

                    //  // Set watcher to listen for the EventRecordWritten
                    //  // event.  When this event happens, the callback method
                    //  // (EventLogEventRead) will be called.
                    //  //watcher.EventRecordWritten += HandleEvent();
                    //  watcher2.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(HandleEvent);

                    //  // Begin subscribing to events the events
                    //  watcher2.Enabled = true;

                    //  //    watcher.Enabled = true;



                    SecureString kod = new SecureString();

                    char[] sp = (
                       //"35263526"
                       // "777"
                       // "lieksa2010"
                       "1"
                        ).ToCharArray();

                    for (int i = 0; i < sp.Length; i++)
                        kod.AppendChar(sp[i]);

                    ////kod.AppendChar('6');
                    ////kod.AppendChar('3');
                    ////kod.AppendChar('5');
                    ////kod.AppendChar('2');
                    ////kod.AppendChar('6');



                    // Get log information.
                    //                
                    //EventLogSession session = new EventLogSession();
                    //EventLogInformation logInfo = session.GetLogInformation("MaxiGraf222", PathType.LogName);
                    //Console.WriteLine("The {0} log contains {1} events.", "MaxiGraf222", logInfo.RecordCount);


                    var beforeCulture = Thread.CurrentThread.CurrentCulture;

                    try
                    {

                       

                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                        //using (var session = new EventLogSession("DESKTOP-PMFSLPC", "DESKTOP-PMFSLPC", /*"FEDOR"*/"Fedor", kod, SessionAuthentication.Default)
                        //{
                        //    var query = new EventLogQuery("System", PathType.LogName, queryString)
                        //    {
                        //        ReverseDirection = true,
                        //        Session = session
                        //    };

                        //    using (var reader = new EventLogReader(query))
                        //    {
                        //        for (var record = reader.ReadEvent(); record != null; record = reader.ReadEvent())
                        //        {
                        //            // Read event records
                        //            string message = record.FormatDescription();
                        //        }
                        //    }
                        //}

                        string domain =
                        //   "747-ПК";
                        //"DESKTOP-PMFSLPC";
                        "IVS-PC";

                        string username =
                        //"747";
                        //"Fedor";
                        //"EventLog";

                        // "IVS";
                        "AD";
                        //"EventReader";

                        string logname = "MaxiGraf";

                        //EventLogPermission eventLogPermission = new EventLogPermission(EventLogPermissionAccess.Administer, domain);
                        //eventLogPermission.Assert();

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



                        EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);//, queryString);


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
                    //int i;
                    //for (i = 0; i <= 4; i++)
                    //{
                    //    if (i < 5)
                    //        // Wait for events to occur. 
                    //        System.Threading.Thread.Sleep(1000);
                    //}

                    //for (int i = 0; i < 5; i++)
                    //{
                    //    // Wait for events to occur. 
                    //    System.Threading.Thread.Sleep(10000);
                    //}


                }
            catch (EventLogReadingException e)
            {
                Console.WriteLine("Error reading the log: {0}", e.Message);
            }

            finally
            {

                    //// Stop listening to events
                    //watcher.Enabled = false;

                    //if (watcher != null)
                    //    watcher.Dispose();


                    //// Stop listening to events
                    //watcher.Enabled = false;

                    //if (watcher != null)
                    //{
                    //    watcher.Dispose();
                    //}
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

                    //for(int i = 0; i < arg.EventRecord.Properties.Count; i++)
                    //{
                    //    Console.WriteLine(arg.EventRecord.Properties[i].ToString());
                    //}


                    //////
                    // This section creates a list of XPath reference strings to select
                    // the properties that we want to display
                    // In this example, we will extract the User, TimeCreated, EventID and EventRecordID
                    //////
                    // Array of strings containing XPath references
                    // String[] xPathRefs = new String[9];
                    //xPathRefs[0] = "Event/System/TimeCreated/@SystemTime";
                    //xPathRefs[1] = "Event/System/Computer";
                    //xPathRefs[2] = "Event/EventData/Data[@Name=\"TargetUserName\"]";
                    //xPathRefs[3] = "Event/EventData/Data[@Name=\"TargetDomainName\"]";
                    //// Place those strings in an IEnumberable object
                    //IEnumerable<String> xPathEnum = xPathRefs;
                    //// Create the property selection context using the XPath reference
                    //EventLogPropertySelector logPropertyContext = new EventLogPropertySelector(xPathEnum);

                    //IList<object> logEventProps = ((EventLogRecord)arg.EventRecord);
                    //Console.WriteLine("Time: ", logEventProps[0]);
                    //Console.WriteLine("Computer: ", logEventProps[1]);
                    //Console.WriteLine("TargetUserName: ", logEventProps[2]);
                    //Console.WriteLine("TargetDomainName: ", logEventProps[3]);
                    //Console.WriteLine("---------------------------------------");

                    //Console.WriteLine("Description: ", arg.EventRecord.FormatDescription());
                }
                else
                {
                    Console.WriteLine("The event instance was null.");
                }
        }

        //public new static int Main(string[] args)
        //{

        //    // Start the event watcher
           
        //   // return 0;
        //}
    }


        public class EventLogSubscription
        {
            //public string login;
            
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
                                             //string Login,

                                             string Domain_name,
                                          string User_name,
                                          string Password,
                                         
                                          string Log_nаme,
                                          string Query = Query_Const
                                        )
            {
                    // login = Login;
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


                        //kod.Dispose();

                        EventLogConfiguration logCon = new EventLogConfiguration(EventLogCollectionSubscription[i].log_nаme, S);



                        List<string> d = new List<string>();

                        d.AddRange(S.GetLogNames());

                        var t = logCon.ProviderNames;



                        EventLogQuery eventsQuery = new EventLogQuery(logCon.LogName, PathType.LogName);//, queryString);


                        eventsQuery.Session = S;
                        eventsQuery.TolerateQueryErrors = false;


                        watcherS.Add(new EventLogWatcher(eventsQuery));

                      //  watcher = new EventLogWatcher(eventsQuery);

                     //   watcher.EventRecordWritten +=
                     //                                 new EventHandler<EventRecordWrittenEventArgs>(
                     //                                 HandleEvent);

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

                //// Stop listening to events
                //watcher.Enabled = false;

                //if (watcher != null)
                //    watcher.Dispose();

                
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

                //for(int i = 0; i < arg.EventRecord.Properties.Count; i++)
                //{
                //    Console.WriteLine(arg.EventRecord.Properties[i].ToString());
                //}


                //////
                // This section creates a list of XPath reference strings to select
                // the properties that we want to display
                // In this example, we will extract the User, TimeCreated, EventID and EventRecordID
                //////
                // Array of strings containing XPath references
                // String[] xPathRefs = new String[9];
                //xPathRefs[0] = "Event/System/TimeCreated/@SystemTime";
                //xPathRefs[1] = "Event/System/Computer";
                //xPathRefs[2] = "Event/EventData/Data[@Name=\"TargetUserName\"]";
                //xPathRefs[3] = "Event/EventData/Data[@Name=\"TargetDomainName\"]";
                //// Place those strings in an IEnumberable object
                //IEnumerable<String> xPathEnum = xPathRefs;
                //// Create the property selection context using the XPath reference
                //EventLogPropertySelector logPropertyContext = new EventLogPropertySelector(xPathEnum);

                //IList<object> logEventProps = ((EventLogRecord)arg.EventRecord);
                //Console.WriteLine("Time: ", logEventProps[0]);
                //Console.WriteLine("Computer: ", logEventProps[1]);
                //Console.WriteLine("TargetUserName: ", logEventProps[2]);
                //Console.WriteLine("TargetDomainName: ", logEventProps[3]);
                //Console.WriteLine("---------------------------------------");

                //Console.WriteLine("Description: ", arg.EventRecord.FormatDescription());
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
