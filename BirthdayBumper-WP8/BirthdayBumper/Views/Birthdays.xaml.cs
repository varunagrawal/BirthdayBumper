using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;

using Facebook;
using BirthdayBumper.Models;
using BirthdayBumper.ViewModels;
using Microsoft.Phone.Net.NetworkInformation;

namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage
    {
        bool BirthdaysLoaded = false;
        FriendDataModel FriendData = new FriendDataModel();
        Object lockObject = new Object();

        // Variable to count the number of times layout has been updated and display progress bar acc.
        static int i = -1;

        public Birthdays()
        {
            InitializeComponent();
            this.Loaded += Birthdays_Loaded;            
        }

        void Birthdays_Loaded(object sender, RoutedEventArgs e)
        {
            //SetupProgressBar();

            if (!BirthdaysLoaded)
            {
                txtLoading.Visibility = System.Windows.Visibility.Visible;

                GetFriendsBirthdays();

                BirthdaysLoaded = true;

                txtLoading.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

        }

        private async void GetFriendsBirthdays()
        {
            // Check for Network Connectivity. If not available, then show message and exit.
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No Network Connectivity." + Environment.NewLine + "Please check if you are connected to the Internet.");
                return;
            }

            BirthdaysList.DataContext = null;

            FriendData.Friends = new ObservableCollection<Friend>();

            List<Friend> f = null, g = null;
            List<Friend> friends = new List<Friend>();

            if (FacebookAccount.IsConnected)
            {
                string site;
                NavigationContext.QueryString.TryGetValue("from", out site);
                if(site == "facebook")
                {
                    f = await FriendData.GetFacebookBirthdays();
                    if (f != null)
                        friends = friends.Concat(f).ToList<Friend>();
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.RelativeOrAbsolute));
                    return;
                }
                
            }

            if (GoogleAccount.IsConnected)
            {
                g = await FriendData.GetGoogleBirthdays();
                if (g != null)
                    friends = friends.Concat(g).ToList<Friend>();
            }
            
            FriendData.Friends = new ObservableCollection<Friend>(friends.Distinct().ToList<Friend>());

            bool contactSearch = await FriendData.GetContactsBirthdays();
            
            CheckZeroBirthdays();

            BirthdaysList.DataContext = FriendData.Friends;
        }


        // Handle selection changed on LongListSelector
        private void BirthdayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (BirthdaysList.SelectedItem == null)
                return;

            Friend f = BirthdaysList.SelectedItem as Friend;

            // Reset selected item to null (no selection)
            BirthdaysList.SelectedItem = null;

            int index = BirthdaysList.ItemsSource.IndexOf(f);
            //FriendData.Friends.RemoveAt(index);

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/Views/WishFriend.xaml?site=" + f.Site + "&type=" + f.GetType(), UriKind.RelativeOrAbsolute));

        }


        private void Accounts_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Accounts.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            //SetupProgressBar();

            lock(lockObject)
            {
                txtLoading.Visibility = System.Windows.Visibility.Visible;

                GetFriendsBirthdays();

                txtLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete From list function pending");
        }

        private void CheckZeroBirthdays()
        {
            if(FriendData.Friends.Count == 0)
            {
                MessageBox.Show("No Birthdays today");

                i = 0;
            }
        }

        private void SetupProgressBar()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;

            ApplicationBar.IsVisible = false;

            if (i < 7 || i == 9 || i == 12) // Values of i for which Progress of Bar should be displayed
            {
                i = 7;
            }
            else
            {
                i = 0;
            }
        }

        private void BirthdaysList_LayoutUpdated(object sender, EventArgs e)
        {
            /*if (SystemTray.ProgressIndicator != null && (i > 7 || i == 0))
            {
                SystemTray.ProgressIndicator.IsIndeterminate = false;
                SystemTray.ProgressIndicator.IsVisible = false;

                ApplicationBar.IsVisible = true;
            }

            i++;*/
        }

    }
}