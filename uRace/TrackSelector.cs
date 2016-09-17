using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uRace
{
    public partial class TrackSelector : Form
    {
        private List<Track> tracks; // List of all tracks, to be filled up once loaded.

        public TrackSelector()
        {
            InitializeComponent();
            tracks = new List<Track>();

            loadAllTracks();
            enterTracks();
        }

        // Creates track objects from each valid track folder in the game's core files.
        private void loadAllTracks()
        {
            // Creates an array of all paths within the "tracks" directory within uRace/bin/Debug
            string[] trackPaths = Directory.GetDirectories(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tracks");
            foreach (string path in trackPaths)
            {
                // If one of the required files doesn't exist, try the next path.
                if (!File.Exists(Path.Combine(path, "info.txt")) || !File.Exists(Path.Combine(path, "scores.csv"))) continue;

                // Instantiate our track object, but don't give it a value yet.
                Track track = null;

                // First we read the name, author and resource name from the info file.
                var infoReader = new StreamReader(File.OpenRead(Path.Combine(path, "info.txt")));
                while (!infoReader.EndOfStream)
                {
                    // Split our line by ':' to get our three pieces of information.
                    var split = infoReader.ReadLine().Split(':');
                    // As the file should only have one line, we can assign the track object within the while loop.
                    // As the name of the resource is dependant on user input, we must cast an object from the resource manager.
                    track = new Track(split[0], split[1], (Image) Properties.Resources.ResourceManager.GetObject(split[2]), new Point(int.Parse(split[3]), int.Parse(split[4])));
                }

                // Let go of the file.
                infoReader.Dispose();

                // If the while loop never finds anything, track will remain null and this means the file is invalid - so we try the next folder.
                if (track == null) continue;

                // Now we can process the high scores file by filling up our high scores list.
                var scoresReader = new StreamReader(File.OpenRead(Path.Combine(path, "scores.csv")));
                while (!scoresReader.EndOfStream)
                {
                    // We split the line by a comma, as it is a CSV file.
                    var split = scoresReader.ReadLine().Split(',');

                    if (split.Length == 1) continue;
                    // Create a new high score object from this line.
                    track.addScore(new HighScore(split[0], int.Parse(split[1])));
                }

                // Let go of the file.
                scoresReader.Dispose();

                // We add the completed track object.
                tracks.Add(track);
            }
        }

        // Enter all of the tracks into the selection list box.
        private void enterTracks()
        {
            // Loop through all uploaded tracks.
            foreach (Track track in tracks)
            {
                trackBox.Items.Add("➤ " + track.getName());
            }
        }

        // Listen for the click event of this button.
        private void playButton_Click(object sender, EventArgs e)
        {
            // Check if anything has been entered into the text field.
            if (nameBox.Text == "")
            {
                showError("You must enter your name!");
                return;
            }

            // Check that the name is not too long.
            // We do this so that the high scores do not display beyond the bounds of the form.
            if (nameBox.Text.Length > 10)
            {
                showError("That name is too long!");
                return;
            }

            // Using RegEx, check if the entered name is only made up of letters.
            var matcher = new Regex("^[a-zA-Z]+$");
            if (!matcher.IsMatch(nameBox.Text))
            {
                showError("You must have only letters in your name!");
                return;
            }

            // Check that they have chosen a track to play.
            if (trackBox.SelectedItem == null)
            {
                showError("You have not selected a track!");
                return;
            }

            // Because the list boxes items were entered in the order of tracks, the index of the selected item should
            // be the same index of the track which it is representing. This allows to grab the correct track.
            Track selectedTrack = tracks.ElementAt(trackBox.Items.IndexOf(trackBox.SelectedItem));

            // Make a new game instance, open it - then close this form.
            uRaceGame game = new uRaceGame(selectedTrack, nameBox.Text);
            Hide();
            game.ShowDialog();
            Close();
        }

        // A utility method which shows the provided string as an error to the user.
        private void showError(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Pre load a name into the name field from outside the class.
        public void loadName(string name)
        {
            nameBox.Text = name;
        }
    }
}
