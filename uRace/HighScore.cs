using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uRace
{
    public class HighScore
    {
        private string name; // The name associated with this score.
        private int time; // The time of this score, in milliseconds.

        public HighScore(string name, int millis)
        {
            this.name = name;
            this.time = millis;
        }

        // Get the name associated with this score.
        public string getName()
        {
            return this.name;
        }

        // Get the time in milliseconds for this score.
        public int getTime()
        {
            return this.time;
        }

        // Update the time of this score to a new provided value.
        public void setTime(int newTime)
        {
            this.time = newTime;
        }
    }
}
