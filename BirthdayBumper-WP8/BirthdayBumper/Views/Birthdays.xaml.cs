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
using System.Windows.Data;
using System.Windows.Threading;

namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage
    {
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
            LoadBirthdays();
        }

        private void LoadBirthdays()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();

            //Binding binding = new Binding("IsLoading") { Source = FriendData };
            //BindingOperations.SetBinding(
            //    SystemTray.ProgressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            //binding = new Binding("IsLoading") { Source = FriendData };
            //BindingOperations.SetBinding(
            //    SystemTray.ProgressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

            //SystemTray.ProgressIndicator.Text = "Loading birthdays...";

            txtLoading.Visibility = System.Windows.Visibility.Visible;
            SetProgressBar(true);

            GetFriendsBirthdays();

            txtLoading.Visibility = System.Windows.Visibility.Collapsed;


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);

            timer.Tick += (timer_sender, timer_args) =>
            {
                SetProgressBar(false);
                timer.Stop();
            };

            timer.Start();
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
            FriendData.IsLoading = true;

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

            FriendData.IsLoading = false;

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
            lock(lockObject)
            {
                LoadBirthdays();
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
            }
        }

        private void SetProgressBar(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;

            ApplicationBar.IsVisible = !isVisible;
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Birthday Bumper developed by Varun Agrawal and Prakhar Gupta");
        }

        private void Readme_Click(object sender, EventArgs e)
        {
            MessageBox.Show("All birthdays may not be visible due to Privacy settings of the people in your social network.");
        }

    }
}