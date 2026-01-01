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
        public static string exePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\XIVLauncher\addon\Hooks\dev\Dalamud.Injector.exe";
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
