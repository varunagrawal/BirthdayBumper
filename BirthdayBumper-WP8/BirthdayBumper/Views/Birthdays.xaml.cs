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
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;


namespace BirthdayBumper.Views
{
    public partial class Birthdays : PhoneApplicationPage, INotifyPropertyChanged
    {
        private BirthdayBumperContext birthdayDB;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<Friend> _friends;
        public ObservableCollection<Friend> Friends
        {
            get
            {
                return _friends;
            }
            set
            {
                if (_friends != value)
                {
                    _friends = value;
                    NotifyPropertyChanged("Friends");
                }
            }
        }

        FriendDataModel FriendData = new FriendDataModel();
        FriendDataModel Upcoming = new FriendDataModel();

        public Birthdays()
        {
            InitializeComponent();

            // Connect to the database and instantiate data context.
            birthdayDB = new BirthdayBumperContext(BirthdayBumperContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            this.Loaded += Birthdays_Loaded;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        // Load data for the ViewModel Items
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            bool done = false;

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.Text = "Loading birthdays...";

            txtLoading.Visibility = System.Windows.Visibility.Visible;
            upcomingTxtLoading.Visibility = System.Windows.Visibility.Visible;

            SetProgressBar(true);
            ApplicationBar.IsVisible = false;

            if (!settings.Contains("AlreadyLoggedIn"))
            {
                done = await GetAndStoreBirthdays();

                settings.Add("AlreadyLoggedIn", true);
            }
            else
            {
                done = true;
            }

            if (done)
            {
                LoadBirthdays();
            }

            txtLoading.Visibility = System.Windows.Visibility.Collapsed;
            upcomingTxtLoading.Visibility = System.Windows.Visibility.Collapsed;

            SetProgressBar(false);
            ApplicationBar.IsVisible = true;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.Loaded -= Birthdays_Loaded;
        }

        void Birthdays_Loaded(object sender, RoutedEventArgs e)
        {   
        }

        private void LoadBirthdays()
        {
            BirthdaysList.DataContext = null;
            UpcomingBirthdays.DataContext = null;

            FriendData.IsLoading = true;
            Upcoming.IsLoading = true;

            using (birthdayDB = new BirthdayBumperContext(BirthdayBumperContext.DBConnectionString))
            {
                string m = DateTime.Now.ToString("MMMM");
                string d = DateTime.Now.Day.ToString();
                string nextday = DateTime.Now.AddDays(1.0).Day.ToString();

                // Define the query to gather all of the to-do items.
                var friendsInDB = from Friend friend in birthdayDB.Friends
                                  where friend.Day == d && friend.Month == m
                                  select friend;

                var upcoming = from Friend friend in birthdayDB.Friends
                               where friend.Day == nextday && friend.Month == m
                               select friend;

                // Execute the query and place the results into a collection.
                FriendData.Friends = new ObservableCollection<Friend>(friendsInDB);
                Upcoming.Friends = new ObservableCollection<Friend>(upcoming);

                BirthdaysList.DataContext = FriendData.Friends;
                UpcomingBirthdays.DataContext = Upcoming.Friends;
            }
            
            FriendData.IsLoading = false;
            Upcoming.IsLoading = false;

            CheckZeroBirthdays();

        }

        // Retrieve Birthday information from social networks / contacts and store it in local database.
        private async Task<bool> GetAndStoreBirthdays()
        {
            // Check for Network Connectivity. If not available, then show message and exit.
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No Network Connectivity." + Environment.NewLine + "Please check if you are connected to the Internet.");
                return false;
            }

            List<Friend> friends = new List<Friend>();

            #region Google Friends
            if (GoogleAccount.IsConnected)
            {
                List<Friend> g = await FriendData.GetGoogleBirthdays();
                if (g != null)
                    friends = friends.Concat(g).ToList<Friend>();
            }
            #endregion

            #region Facebook Friends
            if (FacebookAccount.IsConnected)
            {
                List<Friend> f = await FriendData.GetFacebookBirthdays();
                if (f != null)
                    friends = friends.Concat(f).ToList<Friend>();

            }
            #endregion

            using (birthdayDB = new BirthdayBumperContext(BirthdayBumperContext.DBConnectionString))
            {
                // Define the query to gather all of the to-do items.
                var friendsInDB = from Friend friend in birthdayDB.Friends
                                  select friend;

                // Execute the query and place the results into a collection.
                var dbFriends = new ObservableCollection<Friend>(friendsInDB);

                birthdayDB.Friends.DeleteAllOnSubmit(dbFriends);
                birthdayDB.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);

                birthdayDB.Friends.InsertAllOnSubmit(friends);
                birthdayDB.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
            }


            //bool contactSearch = false;
            //while(!contactSearch)
            //    contactSearch = await FriendData.GetContactsBirthdays();

            return true;
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
            ApplicationBar.IsVisible = false;

            LoadBirthdays();

            ApplicationBar.IsVisible = true;
        }

        private void deleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete From list function pending");
        }

        private void CheckZeroBirthdays()
        {
            if(FriendData.Friends.Count == 0)
            {
                //MessageBox.Show("No Birthdays today");
                FriendData.Friends.Add(new Friend
                {
                    Name = "No Birthdays Today"
                });
            }
        }

        private void SetProgressBar(bool enable)
        {
            if (enable)
            {
            }

            SystemTray.ProgressIndicator.IsIndeterminate = enable;
            SystemTray.ProgressIndicator.IsVisible = enable;

            //ApplicationBar.IsVisible = !isVisible;
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Birthday Bumper developed by Varun Agrawal and Prakhar Gupta");
        }

        private void Readme_Click(object sender, EventArgs e)
        {
            MessageBox.Show("All birthdays may not be visible due to Privacy settings of the people in your social network.");
        }

        private void Sync_Click(object sender, EventArgs e)
        {
            // Needed as Facebook does not provide a way to check for expired tokens

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove("AlreadyLoggedIn");

            if (FacebookAccount.IsConnected)
            {
                FacebookAccount.IsConnected = false;
                FacebookAccount.isAuthenticated = false;

                NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else 
            {
                NavigationService.Navigate(NavigationService.CurrentSource);
            }

        }
    }

}