using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Threading;

namespace flappybird_MT
{
 
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer;
        double birdY = 50;
        double velocityY = 0;
        double gravity = 0.5;
        double jumpStrength = -8;

        List<CsovekPar> csovek = new List<CsovekPar>();
        Random rand = new Random();
        int CsoSzamlalo = 0;

        public MainWindow()
        {
            InitializeComponent();

            Canvas.SetLeft(Bird, 100);
            Canvas.SetTop(Bird, 200);

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        void GameLoop(object sender, EventArgs e)
        {
            velocityY += gravity;
            birdY += velocityY;
            Canvas.SetTop(Bird, birdY);

            CsoSzamlalo++;
            if (CsoSzamlalo > 100)
            {
                CsoGeneralas();
                CsoSzamlalo = 0;
            }

            foreach (var cso in csovek)
            {
                CsoMozgatas(cso);

                if (UtkozesVizsgalat(cso.FelsoCso) || UtkozesVizsgalat(cso.AlsoCso))
                {
                    //jatekvege
                }
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                velocityY = jumpStrength;
            }
        }


        void CsoGeneralas()
        {
            int gap = 120;
            int topHeight = rand.Next(50, 200);

            Image felsoCso = new Image
            {
                Width = 60,
                Height = topHeight,
                Source = new BitmapImage(new Uri("Images/felso_cso.png", UriKind.Relative)),
                Stretch = System.Windows.Media.Stretch.Fill
            };

            Image alsoCso = new Image
            {
                Width = 60,
                Height = GameCanvas.Height - topHeight - gap,
                Source = new BitmapImage(new Uri("Images/also_cso.png", UriKind.Relative)),
                
                Stretch = System.Windows.Media.Stretch.Fill
            };

            Canvas.SetLeft(felsoCso, GameCanvas.Width);
            Canvas.SetTop(felsoCso, 0);

            Canvas.SetLeft(alsoCso, GameCanvas.Width);
            Canvas.SetTop(alsoCso, topHeight + gap);

            GameCanvas.Children.Add(felsoCso);
            GameCanvas.Children.Add(alsoCso);

            csovek.Add(new CsovekPar
            {
                FelsoCso = felsoCso,
                AlsoCso = alsoCso
            });

        }


        void CsoMozgatas(CsovekPar cso)
        {
            Canvas.SetLeft(cso.FelsoCso, Canvas.GetLeft(cso.FelsoCso) - 3);
            Canvas.SetLeft(cso.AlsoCso, Canvas.GetLeft(cso.AlsoCso) - 3);
        }


        bool UtkozesVizsgalat(Image cso)
        {
            Rect birdRect = new Rect(
                Canvas.GetLeft(Bird),
                Canvas.GetTop(Bird),
                Bird.Width,
                Bird.Height);

            Rect csoRect = new Rect(
                Canvas.GetLeft(cso),
                Canvas.GetTop(cso),
                cso.Width,
                cso.Height);

            return birdRect.IntersectsWith(csoRect);
        }


    }
}
