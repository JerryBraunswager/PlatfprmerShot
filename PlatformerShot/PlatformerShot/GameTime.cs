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
    class GameTime
    {
        Point pr_loc;
        public Button new_Button(string name)
        {
            Button b = new Button();
            b.FontSize = 14;
            b.FontFamily = new FontFamily("Showcard Gothic");
            b.Content = name;
            return b;
        }

        public bool gmtime(double top, double left, double power)
        {
            //if (pr_loc != null)
                if (pr_loc.Y == top && pr_loc.X == left)
                    if (power == 0)
                        return false;
            pr_loc.Y = top;
            pr_loc.X = left;
            return true;
        }
    }
}
