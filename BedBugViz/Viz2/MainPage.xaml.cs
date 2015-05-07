using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Viz2
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            _t = new Timer(TimerTick, null, 1000, 1000);
        }

        private void TimerTick(object state)
        {
            labelDay.Dispatcher.BeginInvoke(() => { labelDay.Content = _day++; });
            labelEggs.Dispatcher.BeginInvoke(() => { labelEggs.Content = Math.Max(_day - 2, 0); });
            labelNymphs.Dispatcher.BeginInvoke(() => { labelNymphs.Content = Math.Max(_day - 4, 0); });
            labelAdults.Dispatcher.BeginInvoke(() => { labelAdults.Content = Math.Max(_day - 6, 0); });
            labelBites.Dispatcher.BeginInvoke(() => { labelBites.Content = Math.Max(_day - 8, 0); });

            labelCost.Dispatcher.BeginInvoke(() => { labelCost.Content = string.Format("Cost: {0} 000 SEK", Math.Max(_day - 6, 0)); });

            labelCost.Dispatcher.BeginInvoke(() =>
                    {
                        if (_day < 10)
                        {
                            labelCost.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            labelCost.Background = new SolidColorBrush(Colors.Red);
                        }
                    });
        }

        private readonly Timer _t;
        private int _day = 0;

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}