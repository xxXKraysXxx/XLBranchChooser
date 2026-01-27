using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Policy;
using System.Text.Json.Nodes;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using Microsoft.Win32;

namespace XLBranchChooser
{
    public partial class Form1 : Form
    {
        public static JObject? theObject;
        public class TrackRoot
        {
            public Dictionary<string, Track> Tracks { get; set; }
        }

        public class Track
        {
            public string Key { get; set; }
            public string ApplicableGameVersion { get; set; }
            public string RuntimeVersion { get; set; }
            public string Alias { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }
        public static string exePath = GetInjectorPath();
        public static string launcherjsonPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\XIVLauncher\launcherConfigV3.json";
        public static string GameVersion = "";

        public JObject LoadJson()
        {
            using (StreamReader r = new StreamReader(launcherjsonPath))
            {
                string jsonContent = r.ReadToEnd();
                return JObject.Parse(jsonContent);
            }
        }
        public void SaveJson(JObject jsonObject)
        {
            File.WriteAllText(launcherjsonPath, jsonObject.ToString());

        }

        public string? GetGamePath()
        {
            using (StreamReader r = new StreamReader(launcherjsonPath))
            {
                string jsonContent = r.ReadToEnd();
                var jsonObj = JObject.Parse(jsonContent);

                return jsonObj["GamePath"]?.ToString();
            }
        }
        public static string GetInjectorPath()
        {
            string hooksPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"XIVLauncher\addon\Hooks"
            );

            if (!Directory.Exists(hooksPath))
                return ""; 

            // dev folder has priority
            string devPath = Path.Combine(hooksPath, "dev", "Dalamud.Injector.exe");
            if (File.Exists(devPath))
                return devPath;

         
            var injectors = Directory.GetDirectories(hooksPath)
                .Select(dir => Path.Combine(dir, "Dalamud.Injector.exe"))
                .Where(File.Exists)
                .Select(path => new FileInfo(path))
                .OrderByDescending(fi => fi.LastWriteTime)
                .ToList();

            if (injectors.Count == 0)
                return ""; 

            return injectors.First().FullName;
        }

        public static bool IsDotNetRuntimeInstalled(string requiredVersion)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = "--list-runtimes",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                foreach (string line in output.Split('\n'))
                {
                    if (line.StartsWith("Microsoft.NETCore.App"))
                    {
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2 && Version.TryParse(parts[1], out Version? ver))
                        {
                            if (ver >= Version.Parse(requiredVersion))
                                return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {



            JObject jsonObject = LoadJson();
            if (jsonObject["DalamudBetaKind"] != null)
            {
                textBoxKind.Text = jsonObject["DalamudBetaKind"]!.ToString();
            }
            else
            {
                jsonObject["DalamudBetaKind"] = "release";
            }

            if (jsonObject["DalamudBetaKey"] != null)
            {
                textBoxKey.Text = jsonObject["DalamudBetaKey"]!.ToString();
            }
            else
            {
                jsonObject["DalamudBetaKey"] = "";
            }
            SaveJson(jsonObject);
            //MessageBox.Show("HELP", "DEBUG");
            theObject = jsonObject;
            checkVer();
            grabBranches();



        }


        private void checkVer()
        {

            string? gameverpath = GetGamePath();
            if (gameverpath != null)
            { 
                gameverpath += "\\game\\ffxivgame.ver";
                GameVersion = File.ReadAllText(gameverpath);
            }
            
        }
        async private void grabBranches()
        {
            string linktojson = "https://kamori.goats.dev/Dalamud/Release/Meta";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Fetch the content from the URL
                    string content = await client.GetStringAsync(linktojson);



                    // Deserialize the YAML into the root object
                    dynamic tracksData = JsonConvert.DeserializeObject(content);

                    if (tracksData == null)
                        throw new Exception(tracksData);

                    // Output the deserialized data
                    int y = 20; // Initial vertical position






                    foreach (var track in tracksData)
                    {
                        string name = track.Value["track"];
                        if (name == null)
                            continue;
                        bool isCompatible = (track.Value["isApplicableForCurrentGameVer"] ?? true);



                        string key = track.Value["key"] ?? "";

                        var radioButton = new RadioButton
                        {
                            Text = name,
                            Tag = key, // Store key in Tag property because thats easy
                            Location = new System.Drawing.Point(10, y),
                            AutoSize = true,
                            Enabled = isCompatible,
                            BackColor = isCompatible ? Control.DefaultBackColor : Color.LightGray,
                        };
                        
                        if (name == "release" && !isCompatible)
                        {
                            radioButton.Enabled = true;
                        }

                        radioButton.CheckedChanged += RadioButton_CheckedChanged;

                        groupBox1.Controls.Add(radioButton);
                        y += 30; // Move the vertical position for the next radio button


                    }


                    pictureBox.Image = XLBranchChooser.Properties.Resources.sad;
                }
                catch (Exception ex)
                {
                    pictureBox.Image = XLBranchChooser.Properties.Resources.ded;
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private async void inject_button_Click(object sender, EventArgs e)
        {
            pictureBox.Image = XLBranchChooser.Properties.Resources.Scared_Hamster;

            if (!IsDotNetRuntimeInstalled("10.0.0"))
            {
                var result = MessageBox.Show(
                    "Кажется, что .NET Runtime 10.0.0 не установлен.\n" +
                    "Инжект не запустится без его установки.\n\n" +
                    "Открыть страницу загрузки?",
                    "Нет .NET Runtime",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=10.0.0",
                        UseShellExecute = true
                    });
                    pictureBox.Image = XLBranchChooser.Properties.Resources.sad;
                    return;
                }

            }

            exePath = GetInjectorPath();
            if (string.IsNullOrEmpty(exePath))
            {
                MessageBox.Show("Я не смог найти инжектор.. Даламуд не скачан?", "Ошибка :(");
            }

            JObject jsonObject = LoadJson();




        Process[] pname = Process.GetProcessesByName("ffxiv_dx11");
            if (pname.Length == 0)
                MessageBox.Show("Ты это, для инжекта саму финалку то запусти");
            else
                {
                System.Diagnostics.Process.Start(exePath);
                await Task.Delay(1500);
                Application.Exit();
                }

        }

        private void textBoxKind_KeyDown(object sender, KeyEventArgs e)
        {
            pictureBox.Image = XLBranchChooser.Properties.Resources.que;

        }

        private void textBoxKey_KeyDown(object sender, KeyEventArgs e)
        {
            pictureBox.Image = XLBranchChooser.Properties.Resources.que;

        }

        private async void save_button_Click(object sender, EventArgs e)
        {
            theObject!["DalamudBetaKind"] = textBoxKind.Text;
            theObject!["DalamudBetaKey"] = textBoxKey.Text;
            SaveJson(theObject);
            await Task.Delay(500);
            Application.Exit();
        }
        private void RadioButton_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Checked)
            {
                string kind = radioButton.Text!.ToString(); 
                string key;
                if (radioButton.Tag != null)
                { 
                    key = radioButton.Tag.ToString()!; 
                } else
                {
                    key = ""; // я долбаеб кажется
                }
                textBoxKind.Text = kind;
                textBoxKey.Text = key;
            }
            pictureBox.Image = XLBranchChooser.Properties.Resources.sad;
        }
    }
}
