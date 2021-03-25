using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PlatformerShot
{
    class LandSpawn
    {
        Random r = new Random();
        public int score = 0;
        public List<Rectangle> land = new List<Rectangle>();
        public void Create(Canvas work, double x, double y, double width)
        {
            Rectangle rect = new Rectangle();
            rect.Name = "Land";
            rect.Height = 20;
            rect.Width = width;
            rect.Margin = new Thickness(x, y, 0, 0);
            rect.Stroke = Brushes.Black;
            rect.Fill = Brushes.Green;
            work.Children.Add(rect);
            land.Add(rect);
        }
        public void Spawn(Canvas work, Rect player)
        {
            double p = -20;
            int height = Convert.ToInt32(work.Height - 20);
            {
                p = Convert.ToDouble(r.Next(Convert.ToInt32(player.Height), height));
            } while (!Search(work, p));
            Create(work, work.Width, p, r.Next(100, Convert.ToInt32(work.Width / 2))/*, land*/);
        }
        bool Search(Canvas work, double point)
        {
            for(int i = 0; i < work.Children.Count; i++)
            {
                if(work.Children[i].ToString() == "Rectangle")
                {
                    Rectangle r = (Rectangle)work.Children[i];
                    if (r.Name == "Land")
                    {
                        Rect p = new Rect(work.ActualWidth, point, 1, 20);
                        Rect c = new Rect(r.Margin.Left, r.Margin.Top, r.ActualWidth, r.ActualHeight);
                        if (p.IntersectsWith(c))
                            return false;
                    }
                }
                
            }
            return true;
        }
        public void land_die(Canvas work, Rect r)
        {
            for (int i = 0; i < land.Count; i++)
            {
                if (land[i].Margin.Left + land[i].Width <= 0)
                {
                    work.Children.Remove(land[i]);
                    land.RemoveAt(i);
                    Spawn(work/*, land*/, r);
                    score++;
                }
            }
        }
    }
}
