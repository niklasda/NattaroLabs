using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using RestAssure.Languages;
using RestAssure.LongList;
using RestAssure.Settings;

namespace RestAssure.Views
{
    public partial class AdvicePage : PhoneApplicationPage
    {
        private static IList<LongListItem> _realItems;
        private readonly Uri _adviceLandingPage;

        public AdvicePage()
        {
            InitializeComponent();

            _adviceLandingPage = new Uri(AppResources.AdviceUrl2);
            ApplicationTitle.Text = AppResources.AllCompanyName;
            PageTitle.Text = AppResources.AdviceTitle;

            ShowList();
        }

        private void SetStaticList(string g, string i)
        {
            var dummyItems = new List<LongListItem>();
            dummyItems.Add(new LongListItem(g, i, null, null));

            var groupedList = from item in dummyItems
                              group item by item.Grouping
                              into grouping
                              select new LongListGroup<LongListItem>(grouping.Key, grouping);
            longListSelectorAdvice.ItemsSource = groupedList.ToList();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // This test seems to not work, at least not in emulator
            if (RestAssureSettings.IsInternetAvailable())
            {
                LoadHtmlItems(_adviceLandingPage);
            }
            else
            {
                _realItems = new List<LongListItem>();
                PopulateList(AppResources.OfflineAdviceMenu);
                ShowList();
            }
        }

        private void LoadHtmlItems(Uri adviceUri)
        {
            var client = new WebClient();
            client.OpenReadCompleted += client_OpenReadCompleted;
            client.OpenReadAsync(adviceUri);
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            _realItems = new List<LongListItem>();
            if (e.Error != null)
            {
                PopulateList(AppResources.OfflineAdviceMenu);
            }
            else
            {
                using (TextReader rd = new StreamReader(e.Result))
                {
                    string s = rd.ReadToEnd();
                    PopulateList(s);
                }
            }

            // NOTE crappy wait fake
            var tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(2);
            tmr.Tick += OnTimerTick;
            tmr.Start();
        }

        private void PopulateList(string html)
        {
            var parser = new HtmlStringParser(html);
            LongListItem item;
            while ((item = parser.ParseItem()) != null)
            {
                _realItems.Add(item);
            }
        }

        private void ShowList()
        {
            if (_realItems != null && _realItems.Count > 0)
            {
                var groupedList = from item in _realItems
                                  group item by item.Grouping
                                  into grouping
                                  select new LongListGroup<LongListItem>(grouping.Key, grouping);

                longListSelectorAdvice.ItemsSource = groupedList.ToList();
            }
            else
            {
                SetStaticList(AppResources.Loading1, AppResources.Loading2);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var t = (DispatcherTimer)sender;
            t.Stop();

            ShowList();
        }

        private void longListSelectorAdvice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // should be 1 since it's not multiselect
            if (e.AddedItems.Count > 0)
            {
                object oItem = e.AddedItems[0];
                var item = oItem as LongListItem;
                if (item != null && !string.IsNullOrEmpty(item.LinkUri))
                {
                    string url;
                    if (item.LinkUri.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var uri = new Uri(item.LinkUri, UriKind.Absolute);
                        url = uri.AbsoluteUri;
                    }
                    else
                    {
                        var uri = new Uri(_adviceLandingPage, item.LinkUri);
                        url = uri.AbsoluteUri;
                    }

                    PopPage(new Uri(url, UriKind.Absolute));
                }
            }
        }

        private void PopPage(Uri uri)
        {
            var theUri = new Uri("/Views/IePage.xaml?uri=" + Uri.EscapeDataString(uri.AbsoluteUri) + "&title=" + AppResources.AdviceTitle, UriKind.Relative);
            NavigationService.Navigate(theUri);
        }
    }
}
