using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using RestAssure.Languages;

namespace RestAssure.Views
{
    public partial class IePage : PhoneApplicationPage
    {
        public IePage()
        {
            InitializeComponent();

            ApplicationTitle.Text = AppResources.AllCompanyName;
            PageTitle.Text = AppResources.FactsTitle;

            webBrowser.Navigated += webBrowser_Navigated;
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            _navCount++;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (_navCount > 1)
            {
                // get rid of the topmost item...
                // NavigationStack.Pop();
                // now navigate to the next topmost item
                // note that this is another Pop - as when the navigate occurs a Push() will happen
                _navCount = 0;
                webBrowser.Navigate(_pageUri);
                e.Cancel = true;
                return;
            }

            base.OnBackKeyPress(e);
        }

        private string _frag;
        private int _navCount;
        private Uri _pageUri;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string uri = NavigationContext.QueryString["uri"];
            string title = NavigationContext.QueryString["title"];
            PageTitle.Text = title;

            _pageUri = new Uri(uri, UriKind.Absolute);
            if (!string.IsNullOrEmpty(_pageUri.Fragment))
            {
                _frag = _pageUri.Fragment.TrimStart('#');
            }

            LoadHtmlItems(_pageUri);
        }

        private void LoadHtmlItems(Uri pageUri)
        {
            if (pageUri.AbsoluteUri.StartsWith("res://", StringComparison.CurrentCultureIgnoreCase))
            {
                var resPath = pageUri.AbsoluteUri.Replace("res://", "OfflineFiles/");
                var res = Application.GetResourceStream(new Uri(resPath, UriKind.Relative));
                using (var sr = new StreamReader(res.Stream))
                {
                    string html = sr.ReadToEnd();
                    var htmlPart = SkipToAnchor(html);
                    var htmlImg = InlineImages(htmlPart);
                    webBrowser.NavigateToString(htmlImg);
                }
            }
            else
            {
                var client = new WebClient();
                client.OpenReadCompleted += client_OpenReadCompleted;
                client.OpenReadAsync(pageUri);
            }
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            using (TextReader rd = new StreamReader(e.Result))
            {
                string html = rd.ReadToEnd();
                var htmlPart = SkipToAnchor(html);
                var htmlImg = InlineImages(htmlPart);
                webBrowser.NavigateToString(htmlImg);
            }
        }

        private string InlineImages(string html)
        {
            int pos = 0;

            while (pos != -1)
            {
                int pos1 = html.IndexOf("res://Images", pos, StringComparison.CurrentCultureIgnoreCase);
                if (pos1 != -1)
                {
                    int pos2 = html.IndexOf(".png", pos1, StringComparison.CurrentCultureIgnoreCase);
                    if (pos2 != -1)
                    {
                        string img = html.Substring(pos1, pos2 - pos1 + 4);
                        var resPath = img.Replace("res://", "OfflineFiles/");
                        var res = Application.GetResourceStream(new Uri(resPath, UriKind.Relative));

                        using (var sr = new BinaryReader(res.Stream))
                        {
                            var buffer = new byte[res.Stream.Length];
                            sr.Read(buffer, 0, (int)res.Stream.Length);

                            string img64 = string.Concat("data:image/png;base64,", Convert.ToBase64String(buffer));
                            html = html.Replace(img, img64);
                            pos = pos1 + img64.Length;
                        }
                    }
                }

                if (pos1 == -1)
                {
                    pos = -1;
                }
            }

            return html;
        }

        private string SkipToAnchor(string s)
        {
            int hi1 = s.IndexOf("<html", StringComparison.CurrentCultureIgnoreCase);
            if (hi1 > 0)
            {
                s = s.Substring(hi1, s.Length - hi1);
            }

            int i1 = s.IndexOf("<body>", StringComparison.CurrentCultureIgnoreCase);
            int i2 = s.IndexOf(string.Format("<a id=\"{0}\">", _frag), StringComparison.CurrentCultureIgnoreCase);

            string html;
            try
            {
                string header = s.Substring(0, i1 + 6);
                string end = s.Substring(i2, s.Length - i2);

                html = header + end;
            }
            catch
            {
                html = s;
            }

            html = html.Replace("<header>", "<p style=\"font-family:Arial, Helvetica, sans-serif; font-size:18px; font-weight:bold;\">");
            html = html.Replace("</header>", "</p>");
            html = html.Replace("å", "&aring;").Replace("ä", "&auml;").Replace("ö", "&ouml;");
            html = html.Replace("Å", "&Aring;").Replace("Ä", "&Auml;").Replace("Ö", "&Ouml;");
            html = html.Replace("é", "&eacute;").Replace("É", "&Eacute;");
            html = html.Replace("°", "&deg;").Replace("√", "&radic;");
            return html;
        }
    }
}