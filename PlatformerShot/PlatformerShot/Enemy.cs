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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PlatformerShot
{
    class Enemy
    {
        Ellipse enemy;
        public Rect en;
        public int health, lvl = 0;
        public bool dead = false;
        public void Spawn(Canvas work)
        {
            health = 20;
            enemy = new Ellipse();
            enemy.Width = 20;
            enemy.Height = 20;
            enemy.Margin = new Thickness(0, work.Height / 2, 0, 0);
            enemy.Fill = Brushes.Red;
            en = new Rect(enemy.Margin.Left, enemy.Margin.Top, enemy.Width, enemy.Height);
            dead = false;
            work.Children.Add(enemy);
        }
        public void movement(Rect player, double x)
        {
            double sp_y;
            if (player.Top > en.Top)
                sp_y = 1 + (lvl / 10);
            else
                sp_y = -(1 + (lvl / 10));
            enemy.Margin = new Thickness(enemy.Margin.Left + (5 + (lvl / 10)) + x, enemy.Margin.Top + sp_y, enemy.Margin.Right, enemy.Margin.Bottom);
            en.X = enemy.Margin.Left;
            en.Y = enemy.Margin.Top;
        }
        bool en_dead()
        {
            if (health <= 0)
            {
                //dying();
                return true;
            }
            return false;
        }
        public async void dying()
        {
            if (health <= 0)
            {
                DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));
                enemy.BeginAnimation(Ellipse.WidthProperty, da);
                enemy.BeginAnimation(Ellipse.HeightProperty, da);
                await Task.Delay(110);//System.Threading.Thread.Sleep(1000);
                dead = en_dead();
            }
        }
    }
}
