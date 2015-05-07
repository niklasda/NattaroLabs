using System;
using System.Reflection;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using RestAssure.Languages;
using RestAssure.Settings;

namespace RestAssure.Views
{
    public partial class ContactPage : PhoneApplicationPage
    {
        public ContactPage()
        {
            InitializeComponent();
            ApplicationTitle.Text = AppResources.AllCompanyName;
            PageTitle.Text = AppResources.ContactTitle;
        }

        private string getVersion()
        {
            string versionString;
            try
            {
                var assembly = Assembly.GetExecutingAssembly().FullName;
                string ver = assembly.Split('=')[1].Split(',')[0];
                versionString = ver.Substring(0, ver.IndexOf('.', 2));
            }
            catch (Exception)
            {
                versionString = "™";
            }
            return versionString;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string line1 = string.Format(AppResources.ContactPage_Line1, getVersion());

            textBlockInfo.Text = line1 + Environment.NewLine +
                                 AppResources.ContactPage_Line2 +
                                 AppResources.ContactPage_Line3 +
                                 AppResources.ContactPage_Line4 + Environment.NewLine +
                                 AppResources.ContactPage_Line5 + Environment.NewLine +
                                 AppResources.ContactPage_Line6 + Environment.NewLine +
                                 Environment.NewLine;

            hyperlinkButtonMail.Content = RestAssureSettings.Email;
            hyperlinkButtonUrl.Content = RestAssureSettings.Url;
        }

        private void hyperlinkButtonMail_Click(object sender, RoutedEventArgs e)
        {
            var emailcomposer = new EmailComposeTask();
            emailcomposer.To = RestAssureSettings.Email;
            emailcomposer.Subject = AppResources.ContactMailSubject;
            emailcomposer.Body = Environment.NewLine + Environment.NewLine + AppResources.MailFooter;
            emailcomposer.Show();
        }

        private void hyperlinkButtonCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var web = new WebBrowserTask();
                web.Uri = new Uri(RestAssureSettings.Url);
                web.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppResources.LoadingProblem1, MessageBoxButton.OK);
            }
        }
    }
}