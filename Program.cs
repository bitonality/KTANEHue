using Q42.HueApi;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTANE_Hue {

     class Program {
        private static ILocalHueClient Client;
        private static ColorThief.ColorThief CT = new ColorThief.ColorThief();
        private static List<string> LightIDs = new List<string>();
        private static StreamReader LogReader;
        private static System.Timers.Timer QueueTimer;
        private static ConcurrentQueue<string> LogLines = new ConcurrentQueue<string>();
        public static void Main(string[] args) {
            if(args.Length < 3) {
                Console.WriteLine("Please run the program with the following arguments:");
                Console.WriteLine("ktanehue.exe <Hue IP> <Hue user key> <light>");
                Console.WriteLine("OR");
                Console.WriteLine("ktanehue.exe <Hue IP> <Hue user key> <light1> <light2> <light3>...");
                return;
            }
            Client = new LocalHueClient(args[0]);
            Client.Initialize(args[1]);
            for (int i = 2; i < args.Length; i++) {
                LightIDs.Add(args[i]);
            }
            try {
                Console.WriteLine("Starting initial file read. This might take a while depending on the size of the log file. It's recommended to wait to play until the file is done reading.");
                FileStream stream = File.Open(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/logs/ktane.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                LogReader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, true);
                // Moves the reader to the end of the file.
                while (!LogReader.EndOfStream) {
                    LogReader.ReadLine();
                }
                Console.Write("File done reading - Safe to play game now");
                QueueTimer = new System.Timers.Timer(500);
                // Hook up the Elapsed event for the timer. 
                QueueTimer.Elapsed += UpdateQueue;
                QueueTimer.AutoReset = true;
                QueueTimer.Enabled = true;
            }
            catch (Exception e) {
                Console.WriteLine("The file could not be read. Please make sure you start this program after you start the Keep Talking game and that the executable is located in the Keep Talking game directory.");
                Console.WriteLine(e.Message);
            }
            Thread.Sleep(Timeout.Infinite);
        }

      

        private static async void UpdateQueue(object sender, EventArgs e) {
            QueueTimer.Enabled = false;
            while (!LogReader.EndOfStream) {
                String line = LogReader.ReadLine();
                if (line.Contains("[State] Enter GameplayState")) {
                    LightCommand command = new LightCommand();
                    command.SetColor(new Q42.HueApi.ColorConverters.RGBColor(255, 0, 0));
                    command.On = true;
                    command.Brightness = 255;
                    command.Alert = Alert.None;
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
                else if (line.Contains("Boom")) {
                    LightCommand command = new LightCommand();
                    command.On = false;
                    command.Alert = Alert.None;
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
                else if (line.Contains("SetupState")) {
                    LightCommand command = new LightCommand();
                    command.On = true;
                    command.Brightness = 255;
                    command.SetColor(new Q42.HueApi.ColorConverters.RGBColor(75, 0, 130));
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
                else if (line.Contains("[Bomb] Strike!")) {
                    LightCommand command = new LightCommand();
                    command.Alert = Alert.Multiple;
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
                else if (line.Contains("A winner is you!!")) {
                    LightCommand command = new LightCommand();
                    command.On = true;
                    command.Brightness = 255;
                    command.Alert = Alert.None;
                    command.SetColor(new Q42.HueApi.ColorConverters.RGBColor(25, 25, 255));
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
                else if (line.Contains("[BombComponent] Pass")) {
                    LightCommand command = new LightCommand();
                    command.Alert = Alert.Once;
                    HueResults results = await Client.SendCommandAsync(command, LightIDs);
                }
            }
            QueueTimer.Enabled = true;
        }
    }
}
