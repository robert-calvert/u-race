using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uRace
{
    public partial class uRaceGame : Form
    {
        private Player player; // Player instance.
        private Graphics graphics; // Graphics object.
        private Bitmap bitmap; // Bitmap object.

        private Track currentTrack; // The track currently being played.
        private String playerName; // The name of the player.

        public double moveDir = 0, rotateDir = 0; // Movement and rotation values.
        private int elapsedMillis = 0; // Time since the game was loaded.

        // Constructor which accepts and track and a player name.
        public uRaceGame(Track track, String name)
        {
            InitializeComponent();
            currentTrack = track; // Set current track to one provided.

            player = new Player(track.getStart().X, track.getStart().Y); // Assign player instance, at track starting location.
            gamePanel.BackgroundImage = track.getImage(); // Set the panel's background to the track's image.
            bitmap = new Bitmap(gamePanel.BackgroundImage); // Get a bitmap from the track's image.

            // Fill up all labels on the form with information revelant to the track and the player.

            trackLabel.Text = track.getName();
            authorLabel.Text = "by " + track.getAuthor();

            playerName = name;
            nameLabel.Text = playerName;

            displayTopScores();

            // Prevent the game panel from flickering.
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, gamePanel, new object[] { true });
        }

        // In this method we draw the player onto the panel.
        private void gamePanel_Paint(object sender, PaintEventArgs e)
        {
            graphics = e.Graphics;
            player.draw(graphics);
        }

        // Event which is fired when a key is pressed down.
        private void uRaceGame_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                    rotateDir = -1.2;
                    break;
                case Keys.Right:
                    rotateDir = 1.2;
                    break;
            }
        }

        // Event which is fired when a key is no longer pressed and has been released.
        private void uRaceGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (rotateDir != 0)
            {
                if (e.KeyData == Keys.Left || e.KeyData == Keys.Right) rotateDir = 0;
            }
        }

        // A ever recurring timer which I use to call methods which manipulate the movements of the player.
        private void playerTimer_Tick(object sender, EventArgs e)
        {
            // Move the player based upon value updates.
            player.move(this, gamePanel);

            // If the car passes over yellow, reset the player to the start.
            if (isYellow(player.getLocation()))
            {
                player.reset();
            }

            // If the car passes over blue, finish the game.
            if (isBlue(player.getLocation()))
            {
                finishGame();
            }

            // Force the game panel to update.
            gamePanel.Invalidate();
        }

        // A timer which keeps track of the player's progress in the game, and is used to time game events.
        private void timingTimer_Tick(object sender, EventArgs e)
        {
            // Increment the elapsed each tick.
            elapsedMillis++;

            // After 3 seconds have passed, start the car moving.
            if (elapsedMillis > 300)
            {
                timeLabel.Text = formatMillis(elapsedMillis - 300);
                moveDir = 0.4;
                return;
            }

            // If we haven't reached 3 seconds, show a countdown sequence.
            if (elapsedMillis < 100)
                timeLabel.Text = "3";
            if (elapsedMillis == 100)
                timeLabel.Text = "2";
            if (elapsedMillis == 200)
                timeLabel.Text = "1";
        }

        // This method is executed when the player completes the track.
        private void finishGame()
        {
            HighScore existing = null;
            // Loop through all high scores for this track.
            foreach (HighScore score in currentTrack.getHighScores())
            {
                // If a high score with the name of the current player exists, grab that instance.
                if (score.getName().ToLower() == playerName.ToLower())
                {
                    existing = score;
                    break;
                }
            }

            // If we found a high score, then compare the latest score with the one saved.
            if (existing != null)
            {
                // If it is a better time, update the score object.
                if (elapsedMillis < existing.getTime()) existing.setTime(elapsedMillis);
            }
            else 
            {
                // If we didn't find anything, then create a new high score with this time.
                currentTrack.addScore(new HighScore(playerName, elapsedMillis));
            }

            // Save the high scores to file.
            currentTrack.saveHighScores();
            // Stop timers.
            playerTimer.Stop();
            timingTimer.Stop();

            MessageBox.Show("You successfully completed the track in " + timeLabel.Text + " \nReturn to the track selector to play again", "Completed");

            // Re-open the track selector, with the name pre-loaded.
            TrackSelector selector = new TrackSelector();
            selector.loadName(playerName);
            Hide();
            selector.ShowDialog();
        }

        // Displays the top 5 high scores for the current track on the form.
        private void displayTopScores()
        {
            // Make sure the label has been wiped.
            scoresLabel.Text = "";

            // Grab the size of the high scores list.
            int scoresCount = currentTrack.getHighScores().Count();

            // If there are no high scores, show a informative message.
            if (scoresCount == 0) scoresLabel.Text = "No scores yet!";
            
            // Loop through 5 times, or if less scores available, the size of the list.
            for (int i = 0; i < (scoresCount < 5 ? scoresCount : 5); i++)
            {
                // As the list has been sorted, the first element should be the best time.
                HighScore score = currentTrack.getHighScores().ElementAt(i);
                scoresLabel.Text += "[" + (i + 1) + "] " + score.getName() + " | " + formatMillis(score.getTime()) + Environment.NewLine;
            }
        }

        // Returns true if the provided point is over a yellow pixel.
        private Boolean isYellow(Point point)
        {
            // Grab the colour of the pixel at the specified point.
            Color colour = bitmap.GetPixel(point.X, point.Y);

            // Return if the RGB value of the colour is within the colour scope of yellow.
            return (colour.R > 220 && colour.G > 220 && colour.B < 200);
        }

        // Returns true if the provided point is over a blue pixel.
        private Boolean isBlue(Point point)
        {
            Color colour = bitmap.GetPixel(point.X, point.Y);
            return (colour.R < 140 && colour.G < 248 && colour.B > 105);
        }

        // Returns a formatted time string from the provided millisecond value.
        private String formatMillis(int millis)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(millis * 10);
            return time.ToString(@"mm\:ss\:ff");
        }
    }
}
