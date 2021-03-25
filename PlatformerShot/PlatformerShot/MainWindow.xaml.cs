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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer t;
        LandSpawn ls = new LandSpawn();
        GameTime gm = new GameTime();
        //
        List<Ellipse> bullets = new List<Ellipse>();
        //
        Enemy en = new Enemy();
        Player player = new Player();
        //
        bool jump = false, dJump = false;
        //        
        double x = 0, speed = 10, j_power = 0;       
               
        public MainWindow()
        {
            InitializeComponent();
            t = new DispatcherTimer();
            dJump = false;
            ls.score = 0;
            en.lvl = 0;
            new_game();
            t.Interval = new TimeSpan(0, 0, 0, 0, 17);
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (!player.dead)
            {
                if(jump)
                {
                    j_power += (dJump) ? 0.2 : 0;
                }

                horizontal_move();
                vertical_move();
                Shoot_en_move();

                player.dying(Workspace.Height, en.en.Left);
                game_over();

                ls.land_die(/*land,*/ Workspace, player.r);
                Score_label.Content = "SCORE " + ls.score + "\nBOSS HEALTH " + en.health;     

                en.dying();
                if (en.dead)
                {
                    ls.score += 10 + en.lvl;
                    en.lvl++;
                    new_game();
                }              
            }
        }
        void game_over()
        {
            if (player.dead)
            {
                Workspace.Children.RemoveRange(1, Workspace.Children.Count - 1);
                Button retry = gm.new_Button("RETRY");
                retry.Margin = new Thickness(Workspace.Width/2 - (retry.ActualWidth / 2), Workspace.Height/2, 0, 0);
                Workspace.Children.Add(retry);
                retry.Click += Retry_Click;
            }
        }
        void new_game()
        {
            //Clear
            Workspace.Children.RemoveRange(1, Workspace.Children.Count - 1);
            ls.land.Clear();
            bullets.Clear();
            //Player 
            dJump = false;
            player.Spawn(240, 586, Workspace);
            //Enemy
            en.Spawn(Workspace);
            //Land
            ls.Create(Workspace, 10, 650, 650/*, land*/);
            ls.Spawn(Workspace, /*land,*/ player.r);
            ls.Create(Workspace, 900, 510, 200/*, land*/);
        }
        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            ls.score = 0;
            en.lvl = 0;
            new_game();
        }
        //
        // Bullets
        //
        void Shoot_en_move()
        {
            if (gm.gmtime(player.r.Top, ls.land[0].Margin.Left, j_power))
            {
                en.movement(player.r, x);
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].Margin = new Thickness(bullets[i].Margin.Left + speed * Math.Cos(Convert.ToDouble(bullets[i].Tag)), bullets[i].Margin.Top + speed * Math.Sin(Convert.ToDouble(bullets[i].Tag)), 0, 0);
                    if (bullets[i].Margin.Left > Workspace.Width || bullets[i].Margin.Left < -100
                        || bullets[i].Margin.Top > Workspace.Height || bullets[i].Margin.Top < -100)
                    {
                        Workspace.Children.Remove(bullets[i]);
                        bullets.RemoveAt(i);
                    }
                    bulletIntersection(i);
                }
            }
        }
        void bulletIntersection(int i)
        {
            if (i < bullets.Count)
            {
                Rect b = new Rect(bullets[i].Margin.Left, bullets[i].Margin.Top, bullets[i].ActualWidth, bullets[i].ActualHeight);
                for (int j = 0; j < ls.land.Count; j++)
                {
                    Rect l = new Rect(ls.land[j].Margin.Left, ls.land[j].Margin.Top, ls.land[j].ActualWidth, ls.land[j].ActualHeight);
                    if (IntersectsWith(b, l, false))
                    {
                        Workspace.Children.Remove(bullets[i]);
                        bullets.RemoveAt(i);
                    }
                }
                if(IntersectsWith(b, en.en, false) && bullets.Count < 10)
                {
                    Workspace.Children.Remove(bullets[i]);
                    bullets.RemoveAt(i);
                    en.health -= 1;
                }
            }
        }
        //
        // Movement
        //
        void horizontal_move()
        {            
            bool inter = Intersection(false);
            for(int i = 0; i < ls.land.Count; i++)
            {
                if(inter)
                {
                    ls.land[i].Margin = new Thickness(ls.land[i].Margin.Left + x, ls.land[i].Margin.Top, ls.land[i].Margin.Right, ls.land[i].Margin.Bottom);
                }
            }
        }
        void vertical_move()
        {
            if (Intersection(true))
            {
                player.Move(0, 5);
            }
            for (int i = 0; i < ls.land.Count; i++)
            {
                Rect l = new Rect(ls.land[i].Margin.Left, ls.land[i].Margin.Top, ls.land[i].ActualWidth, ls.land[i].ActualHeight);
                if (IntersectsWith(player.r, l, false))
                {
                    player.Transport(player.r.Left, ls.land[i].Margin.Top - player.r.Height);
                    dJump = true;
                }
            }
        }
        //
        // Intersection
        //
        bool IntersectsWith(Rect r_p, Rect r_l, bool fall)
        {
            if (fall)
                r_p.Height += 1;
            if (r_l.IntersectsWith(r_p))
            {
                return true;
            }
            return false;
        }
        bool Intersection(bool fall)
        {        
            int num = ls.land.Count;
            for (int i = 0; i < ls.land.Count; i++)
            {
                Rect l = new Rect(ls.land[i].Margin.Left, ls.land[i].Margin.Top, ls.land[i].ActualWidth, ls.land[i].ActualHeight);
                if (!IntersectsWith(player.r, l, fall))
                {
                    num--;
                }
            }           
            if (num != ls.land.Count)
            {
                return true;
            }
            return false;
        }
        //
        // Mouse and keyboard
        //
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    x = 0;
                    break;
                case Key.D:
                    x = 0;
                    break;
                case Key.Space:
                    {
                        if (dJump)
                        {
                            double num = (player.r.Top - (50 * j_power) < 0) ? player.r.Top : 50 * j_power;
                            player.Move(0, -num);
                            j_power = 0;
                            dJump = false;
                            jump = false;
                        }
                    }
                    break;
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Workspace.Height = GameSpace.ActualHeight - 40;
            Workspace.Width = GameSpace.ActualWidth - 20;
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point poi = e.GetPosition(Workspace);
            player.Rotate(poi);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!player.dead & player.arm.RenderTransform.ToString() == "System.Windows.Media.RotateTransform")
            {
                Ellipse bullet = new Ellipse();
                bullet.Height = 5;
                bullet.Width = 10;
                bullet.Fill = Brushes.Black;
                bullet.Name = "Bullet";
                RotateTransform rt = (RotateTransform)player.arm.RenderTransform;
                bullet.Margin = new Thickness(player.arm.Margin.Left + player.arm.ActualWidth * Math.Cos(rt.Angle / 180 * Math.PI), (player.arm.Margin.Top + player.arm.ActualHeight / 2) + player.arm.ActualWidth * Math.Sin(rt.Angle / 180 * Math.PI), 0, 0);
                bullet.RenderTransformOrigin = new Point(0.5, 0.5);
                Point p = Mouse.GetPosition(Workspace);
                bullet.Tag = Math.Atan2(p.Y - bullet.Margin.Top, p.X - bullet.Margin.Left);
                RotateTransform brt = new RotateTransform(Convert.ToInt32(bullet.Tag) * 180 / Math.PI);
                bullet.RenderTransform = brt;
                Workspace.Children.Add(bullet);
                bullets.Add(bullet);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Space:
                    jump = true;
                    break;
                case Key.A:
                    x = 5;
                    break;
                case Key.D:
                    x = -5;
                    break;
            }
        }
    }
}
