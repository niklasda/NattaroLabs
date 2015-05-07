using System;
using System.Windows;
using Microsoft.Phone.Controls;
using RestAssure.Languages;

namespace RestAssure
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            buttonFacts.Content = AppResources.FactsButton;
            buttonAdvice.Content = AppResources.AdviceButton;
            buttonCamera.Content = AppResources.CameraButton;
            buttonContact.Content = AppResources.ContactButton;

            ApplicationTitle.Text = AppResources.AllCompanyName;
            PageTitle.Text = AppResources.StartPageTitle;
        }

        private void buttonFacts_Click(object sender, RoutedEventArgs e)
        {
            var theUri = new Uri("/Views/FactsPage.xaml", UriKind.Relative);
            NavigationService.Navigate(theUri);
        }

        private void buttonAdvice_Click(object sender, RoutedEventArgs e)
        {
            var theUri = new Uri("/Views/AdvicePage.xaml", UriKind.Relative);
            NavigationService.Navigate(theUri);
        }

        private void buttonCamera_Click(object sender, RoutedEventArgs e)
        {
            var theUri = new Uri("/Views/CameraPage.xaml", UriKind.Relative);
            NavigationService.Navigate(theUri);
        }

        private void buttonContact_Click(object sender, RoutedEventArgs e)
        {
            var theUri = new Uri("/Views/ContactPage.xaml", UriKind.Relative);
            NavigationService.Navigate(theUri);
        }
    }
}