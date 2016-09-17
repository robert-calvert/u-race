using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace uRace
{
    class Player
    {
        private double angle; // The angle of rotation the rectangle is currently at.
        private Image image; // The representative image of the player.
        private Rectangle rectangle; // The rectangle which we manipulate.
        private Point origin; // The original point at which the player is placed on the track.

        private float xScale, yScale; // The scale factor on the X and Y coords.

        public Player(int x, int y) // The starting X and Y values are provided, because they are different on each track.
        {
            image = Properties.Resources.Car; // Assign image to car image from resources.
            origin = new Point(x, y); // Create origin point from given values.
            rectangle = new Rectangle(x, y, 45, 35); // Instantiate the rectangle.
            xScale = (float) rectangle.Width / (float) image.Width; // Calculate the scale factor.
            yScale = (float) rectangle.Height / (float) image.Height;
            rectangle.Location = new Point((int) (x / xScale), (int) (y / yScale)); // Scale location of rectangle.
        }

        // This method draws the player onto the panel's graphics.
        public void draw(Graphics g)
        {
            // Calculate the offset of the centre of the rectangle.
            int dx = rectangle.X + (rectangle.Height / 2);
            int dy = rectangle.Y + (rectangle.Width / 2);

            // Scale everything by our created factors.
            g.ScaleTransform(xScale, yScale);
            // Translate it upon the center of the rectangle.
            g.TranslateTransform(dx, dy);
            // Rotate our graphics object by our angle in radians.
            g.RotateTransform((float) ((180 * angle) / Math.PI));
            // Move back to the top corner of the rectangle.
            g.TranslateTransform(-dx, -dy);
            // Draw the image on our translated graphics.
            g.DrawImage(image, rectangle.X, rectangle.Y);
            // Reset the graphics object.
            g.ResetTransform();
        }

        // This method updates the position of the player on the panel based upon movement and rotation value manipulations.
        public void move(uRaceGame game, Panel panel)
        {
            // Find the cosine and sine of our angle, and set some default values.
            double cos = Math.Cos(angle), sin = Math.Sin(angle);
            int xLocation = 200;
            int yLocation = 200;

            // Set X and Y to the rotated factors of their current position incremented.
            xLocation = (int) Math.Floor(rectangle.X + (cos * game.moveDir * 60)); 
            yLocation = (int) Math.Floor(rectangle.Y + (sin * game.moveDir * 60)); 
                                                    
            // Do some fancy trig to update the angle of rotation.                                   
            angle = (angle + (game.rotateDir * (Math.PI / 128))) % (Math.PI * 2);
            
            // Ensure that we aren't moving off the panel with this move - if so, return before we can set the location.
            if (xLocation * xScale > panel.Width - (rectangle.Width * cos) || yLocation * yScale > panel.Height - (rectangle.Width * sin) - 5 || xLocation * xScale < 0 || yLocation * yScale < 5) return;
           
            // Update the location of the rectangle.
            rectangle.Location = new Point(xLocation, yLocation);
        }

        // This method provides the true location of the player.
        public Point getLocation()
        {
            // Because the rectangle's X and Y values have been scaled, we must undo the scaling.
            return new Point((int) ((rectangle.X + rectangle.Width) * xScale), (int) ((rectangle.Y  + (rectangle.Height / 2)) * yScale));
        }

        // Updates the player's position to the given location.
        public void setLocation(int x, int y)
        {
            // The provided x and y values are not scaled, so we must scale them.
            rectangle.Location = new Point((int) (x / xScale), (int) (y / yScale));
        }

        // Resets the players attributes to their defaults.
        public void reset()
        {
            angle = 0;
            setLocation(origin.X, origin.Y);
        }
    }
}
