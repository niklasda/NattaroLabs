using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RestAssure.LongList
{
    public class LongListItem
    {
        public LongListItem(string grouping, string title, string imageUri, string linkUri)
        {
            Grouping = grouping;
            Title = title;
            ImageUri = imageUri;
            LinkUri = linkUri;

            LoadImage();
        }

        private ImageSource _img;

        public string Grouping { get; private set; }

        public string Title { get; private set; }

        private string ImageUri { get; set; }

        public string LinkUri { get; private set; }

        private void LoadImage()
        {
            if (!string.IsNullOrEmpty(ImageUri))
            {
                if (ImageUri.StartsWith("res://", StringComparison.CurrentCultureIgnoreCase))
                {
                    var resPath = ImageUri.Replace("res://", "OfflineFiles/");
                    var res = Application.GetResourceStream(new Uri(resPath, UriKind.Relative));
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(res.Stream);
                    _img = bitmapImage;
                }
                else
                {
                    var webClient = new WebClient();
                    webClient.OpenReadCompleted += wc_OpenReadCompleted;
                    webClient.OpenReadAsync(new Uri(ImageUri));
                }
            }
        }

        public ImageSource Image
        {
            get { return _img; }
        }

        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.Result);
                _img = bitmapImage;
            }
        }
    }
}