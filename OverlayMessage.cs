using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SilverReaderApp
{
    class OverlayMessage
    {
        private TextBlock messageBox;
        private DispatcherTimer timer;

        public OverlayMessage(TextBlock messageTextBlock, int displayTimeInMs = 1000)
        {
            messageBox = messageTextBlock;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(displayTimeInMs);
            timer.Tick += Timer_Tick;
        }

        public void ShowMessage(string message, int timePeriodInMs = 1000)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            messageBox.Text = message;
            messageBox.Visibility = Visibility.Visible;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            messageBox.Visibility = Visibility.Hidden;
        }
    }
}
