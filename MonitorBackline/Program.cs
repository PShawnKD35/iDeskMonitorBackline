using System;
using System.Text;
using System.Threading.Tasks;
using Sisyphus.Libs;

namespace MonitorBackline
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\notification.wav");
            try
            {
                soundPlayer.Load();
            }catch(Exception ex)
            {
                Console.WriteLine(string.Format("Mission aborted! Unable to load notificaiont sound file '{0}':{1}{2}", soundPlayer.SoundLocation, Environment.NewLine, ex.Message));
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Please input your iDesk username:");
            string username = Console.ReadLine();
            Console.WriteLine("Please input your iDesk password:");
            string password = Console.ReadLine();

            IDeskRestAPI idesk = new IDeskRestAPI(username, password, null, IDeskRestAPI.ENV_PROD, 10, null, log => Console.WriteLine(DateTime.Now.ToShortTimeString() + " - " + log));


            while (true)
            {
                //Login
                if (!Task.Run(() => idesk.LoginAsync()).Result)
                {
                    Console.WriteLine("Login failed.");
                    Console.WriteLine("Press any key to close...");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("Start to monitor Multichannel-Backline");
#if DEBUG
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " - Logged in.");
#endif

                //Search queue
                //string q = @"((('Assigned Group' LIKE ""Frontline_%"" OR 'Assigned Group' LIKE ""%BacklineITSD"") AND (('ServiceCI' = ""order management"" OR 'ServiceCI' = ""cwis (central warehousing information service)"" OR 'ServiceCI' = ""astro [warehouse management system]"" OR 'ServiceCI' = ""astro"" OR 'ServiceCI' = ""ymm (yard management)"" OR 'ServiceCI' = ""centiro"" OR 'ServiceCI' = ""centiro application"")OR ('Description' LIKE ""MCS"" AND NOT ('ServiceCI' = ""isell"" OR 'ServiceCI' = ""isell application""))))AND ('Status' < ""Pending"" OR ('Status' = ""Resolved"" AND 'Status_Reason' =""Updated By Mail""))AND ('Country' = ""Australia""OR 'Country' = ""Austria"" OR 'Country' = ""Belgium""OR 'Country' = ""China"" OR 'Country' = ""Denmark""OR 'Country' = ""Finland""OR 'Country' = ""France"" OR 'Country' = ""Germany""OR 'Country' = ""Ireland""OR 'Country' = ""Italy"" OR 'Country' = ""Netherlands"" OR 'Country' = ""Norway""OR 'Country' = ""Poland""OR 'Country' = ""Portugal"" OR 'Country' = ""South Korea""OR 'Country' = ""Spain"" OR 'Country' = ""Sweden"" OR 'Country' = ""Switzerland"" OR 'Country' = ""United Kingdom""))";
                string q = @"((('Assigned Group' LIKE ""%BacklineITSD"") AND (('ServiceCI' = ""order management"" OR 'ServiceCI' = ""cwis (central warehousing information service)"" OR 'ServiceCI' = ""astro [warehouse management system]"" OR 'ServiceCI' = ""astro"" OR 'ServiceCI' = ""ymm (yard management)"" OR 'ServiceCI' = ""centiro"" OR 'ServiceCI' = ""centiro application"")OR ('Description' LIKE ""MCS"" AND NOT ('ServiceCI' = ""isell"" OR 'ServiceCI' = ""isell application""))))AND ('Status' < ""Pending"" OR ('Status' = ""Resolved"" AND 'Status_Reason' =""Updated By Mail""))AND ('Country' = ""Australia""OR 'Country' = ""Austria"" OR 'Country' = ""Belgium""OR 'Country' = ""China"" OR 'Country' = ""Denmark""OR 'Country' = ""Finland""OR 'Country' = ""France"" OR 'Country' = ""Germany""OR 'Country' = ""Ireland""OR 'Country' = ""Italy"" OR 'Country' = ""Netherlands"" OR 'Country' = ""Norway""OR 'Country' = ""Poland""OR 'Country' = ""Portugal"" OR 'Country' = ""South Korea""OR 'Country' = ""Spain"" OR 'Country' = ""Sweden"" OR 'Country' = ""Switzerland"" OR 'Country' = ""United Kingdom""))";
                IDeskSearchResult result = Task.Run(() => idesk.SearchIncidents(q, IDeskRestAPI.FIELDS_DEFAULT, IDeskRestAPI.SORT_DEFAULT)).Result;

                //Logout
                Task.Run(() => idesk.LogoutAsync()).Wait();
#if DEBUG
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " - Logged out.");
#endif

                if(result == null)
                {
                    Console.WriteLine("Press any key to close...");
                    Console.ReadKey();
                    return;
                }

                //Check if there is case to tend
                if (result.entries.Count > 0)
                {                    
                    foreach(Entry entry in result.entries)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append((string)entry.values["Assigned Group"]);
                        sb.Append(": ");
                        sb.Append((string)entry.values["Incident Number"]);
                        Console.WriteLine(sb.ToString());
                    }                    
                    soundPlayer.PlayLooping();
                    Console.WriteLine("Enter any key to stop the sound and continue...");
                    Console.ReadKey();
                    soundPlayer.Stop();
                }else
                {
                    Console.WriteLine("No cases found.");
                }

                //Repeat every 5 mins
                Task.Delay(300000).Wait();
            }
        }
    }
}
