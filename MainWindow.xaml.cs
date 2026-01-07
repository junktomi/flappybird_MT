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
        double birdY = 200;
        double velocityY = 0;
        double gravity = 0.5;
        double jumpStrength = -8;

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
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                velocityY = jumpStrength;
            }
        }
    }
}
