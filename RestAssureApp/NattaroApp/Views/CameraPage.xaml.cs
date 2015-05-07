using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using RestAssure.Languages;
using RestAssure.Settings;

namespace RestAssure.Views
{
    public partial class CameraPage : PhoneApplicationPage
    {
        private object _locker = new object();
        private static volatile BitmapImage _bitmap = new BitmapImage();

        public CameraPage()
        {
            InitializeComponent();

            ApplicationTitle.Text = AppResources.AllCompanyName;
            PageTitle.Text = AppResources.CameraTitle;
            textBlockInstructions.Text = AppResources.CameraInstructions;

            buttonCamera.Content = AppResources.CameraOpenButton;
            buttonSend.Content = AppResources.CameraSendButton;

            if (_bitmap != null)
            {
                captureBrush.ImageSource = _bitmap;
            }
        }

        private void cct_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var stream = e.ChosenPhoto;
                Dispatcher.BeginInvoke(delegate
                {
                    _bitmap.SetSource(stream);
                    captureBrush.ImageSource = _bitmap;
                });
            }
            else
            {
                // either user clicked back, or USB cable is in.
                MessageBox.Show(AppResources.CameraCancel);
            }
        }

        private void buttonCamera_Click(object sender, RoutedEventArgs e)
        {
            var cct = new CameraCaptureTask();
            cct.Completed += cct_Completed;

            cct.Show();
        }

        private void SavePictureToCameraRoll()
        {
            var writable = new WriteableBitmap(_bitmap);
            var stream = new MemoryStream();
            writable.SaveJpeg(stream, _bitmap.PixelWidth, _bitmap.PixelHeight, 0, 75);
            stream.Position = 0;

            var mlib = new MediaLibrary();
            string fileName = computeFilename(mlib);
            mlib.SavePictureToCameraRoll(fileName, stream);
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (_bitmap.PixelHeight > 0 && _bitmap.PixelWidth > 0)
            {
                SavePictureToCameraRoll();
            }

            //*---

            var emailTask = new EmailComposeTask();
            emailTask.To = RestAssureSettings.Email;
            emailTask.Subject = AppResources.CameraMailSubject;
            emailTask.Body = AppResources.CameraMailIntro +
                Environment.NewLine + Environment.NewLine + AppResources.MailFooter;
            emailTask.Show();
        }

        private string computeFilename(MediaLibrary mlib)
        {
            int highestNumber = 0;
            const string FileNameStart = "bedbug";
            const string FileNameExt = ".jpg";

            var albs = mlib.RootPictureAlbum;

            foreach (var a in albs.Albums)
            {
                var albumName = a.Name;
                if (albumName.Equals("Camera Roll", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (var picture in a.Pictures)
                    {
                        var pictureName = picture.Name;
                        if (pictureName.StartsWith(FileNameStart, StringComparison.CurrentCultureIgnoreCase) &&
                            pictureName.EndsWith(FileNameExt, StringComparison.CurrentCultureIgnoreCase))
                        {
                            pictureName = pictureName.Substring(FileNameStart.Length);
                            string fileNameSequenceNumber = pictureName.Substring(0, pictureName.Length - FileNameExt.Length);

                            int number;
                            if (int.TryParse(fileNameSequenceNumber, out number))
                            {
                                if (number > highestNumber)
                                {
                                    highestNumber = number;
                                }
                            }
                        }
                    }
                }
            }

            return string.Format("{0}{1}{2}", FileNameStart, ++highestNumber, FileNameExt);
        }
    }
}