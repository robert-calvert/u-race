using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace uRace
{
    public class Track
    {
        private String name, author; // The name and the author of this track.
        private Image image; // The image which the game panel is set to.
        private Point startingPoint; // The initial position the player starts at.
        private List<HighScore> highScores; // A list of high scores associated with this track.

        public Track(String n, String a, Image i, Point p)
        {
            name = n;
            author = a;
            image = i;
            startingPoint = p;
            highScores = new List<HighScore>();
        }

        // Save the high scores back to the CSV file.
        public void saveHighScores()
        {
            // Create a string builder
            StringBuilder builder = new StringBuilder();
            // Loop through all high scores.
            foreach (HighScore score in highScores)
            {
                // Append the highscore in a CSV format to the string builder.
                builder.Append(string.Format("{0},{1}{2}", score.getName(), score.getTime(), Environment.NewLine));
            }

            // Get the path of the track's data.
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/tracks/" + name;
            // Write the highscores to the file.
            File.WriteAllText(path + "/scores.csv", builder.ToString());
        }

        // Adds a score to the high scores list and then sorts them.
        public void addScore(HighScore score)
        {
            highScores.Add(score);
            highScores.Sort(delegate (HighScore s1, HighScore s2)
            {
                return s1.getTime().CompareTo(s2.getTime());
            });
        }

        // Returns the list of high scores.
        public List<HighScore> getHighScores()
        {
            return highScores;
        }

        // Returns the image of this track.
        public Image getImage()
        {
            return image;
        }

        // Returns the name of this track.
        public string getName()
        {
            return name;
        }

        // Returns the author of this track.
        public string getAuthor()
        {
            return author;
        }

        // Returns the starting point of this track.
        public Point getStart()
        {
            return startingPoint;
        }
    }
}
