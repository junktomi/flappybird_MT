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

        List<Image> esoCseppek = new List<Image>();
        bool esoAktiv = false;
        double esoJump = -4;
        bool kodAktiv = false;
        int kodIdozito = 0;


        DispatcherTimer gameTimer;
        double birdY = 50;
        double velocityY = 0;
        double gravity = 0.5;
        double normalJump = -8;
        double jumpStrength = -8;
        

        List<CsovekPar> csovek = new List<CsovekPar>();
        Random rand = new Random();
        int CsoSzamlalo = 0;

        bool gameOver = false;

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

        int pontszam = 0;
        void GameLoop(object sender, EventArgs e)
        {
            if (gameOver) return;

            velocityY += gravity;
            birdY += velocityY;
            Canvas.SetTop(Bird, birdY);

            if (birdY < 0 || birdY + Bird.Height > GameCanvas.Height)
            {
                EndGame();
            }

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
                    EndGame();
                }

                double birdX = Canvas.GetLeft(Bird);

                if (!cso.Pontozva &&
                    Canvas.GetLeft(cso.FelsoCso) + cso.FelsoCso.Width < birdX)
                {
                    pontszam++;
                    ScoreText.Text = pontszam.ToString();
                    cso.Pontozva = true;

                    if (pontszam == 2 || pontszam % 10 == 0)
                    {
                        EsoInditasa();
                    }
                    if (pontszam % 10 == 4)
                    {
                        EsoLeallitasa();
                    }


                }


            }

            if (esoAktiv)
            {
                foreach (var csepp in esoCseppek)
                {
                    double y = Canvas.GetTop(csepp);
                    y += 6;

                    if (y > GameCanvas.Height)
                    {
                        y = rand.Next(-200, 0);
                        Canvas.SetLeft(csepp, rand.Next(0, (int)GameCanvas.Width));
                    }

                    Canvas.SetTop(csepp, y);
                }
            }

            kodIdozito++;

            if (kodIdozito == 500)   
            {
                KodInditasa();
            }

            if (kodIdozito == 800) 
            {
                KodLeallitasa();
                kodIdozito = 0;
            }


        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && !gameOver)
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
            Canvas.GetLeft(Bird) + 4,
            Canvas.GetTop(Bird) + 4,
            Bird.Width -  4,
            Bird.Height - 4);

            Rect csoRect = new Rect(
                Canvas.GetLeft(cso),
                Canvas.GetTop(cso),
                cso.Width,
                cso.Height);

            return birdRect.IntersectsWith(csoRect);
        }

        void RestartGame()
        {
            pontszam = 0;
            ScoreText.Text = pontszam.ToString();
            birdY = 50;
            velocityY = 0;
            Canvas.SetLeft(Bird, 100);
            Canvas.SetTop(Bird, birdY);

            foreach (var cso in csovek)
            {
                GameCanvas.Children.Remove(cso.FelsoCso);
                GameCanvas.Children.Remove(cso.AlsoCso);
            }

            csovek.Clear();
            CsoSzamlalo = 0;
            gameOver = false;
            gameTimer.Start();
            EsoLeallitasa();
            jumpStrength = normalJump;
            KodLeallitasa();
            kodIdozito = 0;
        }


       void EndGame()
        {
            if (gameOver) return;

            gameOver = true;
            gameTimer.Stop();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBoxResult result = MessageBox.Show(
                    "Meghaltál!\nSzeretnéd újraindítani?",
                    "Game Over",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                    RestartGame();
                else
                {
                    Application.Current.Shutdown();
                }
            }));
        }

        void EsoInditasa()
        {
            esoAktiv = true;
            jumpStrength = esoJump;

            for (int i = 0; i < 20; i++)
            {
                Image csepp = new Image
                {
                    Width = 15,
                    Height = 45,
                    Source = new BitmapImage(new Uri("Images/eso.png", UriKind.Relative)),
                    Opacity = 0.7
                };

                Canvas.SetLeft(csepp, rand.Next(0, (int)GameCanvas.Width));
                Canvas.SetTop(csepp, rand.Next(-400, 0));

                Panel.SetZIndex(csepp, 1); 
                GameCanvas.Children.Add(csepp);
                esoCseppek.Add(csepp);
            }
        }

        void EsoLeallitasa()
        {
            esoAktiv = false;
            jumpStrength = normalJump;

            foreach (var csepp in esoCseppek)
            {
                GameCanvas.Children.Remove(csepp);
            }

            esoCseppek.Clear();
        }
        void KodInditasa()
        {
            kodAktiv = true;
            FogLayer.Opacity = 0.7;
        }

        void KodLeallitasa()
        {
            kodAktiv = false;
            FogLayer.Opacity = 0.0;
        }

    }
}
