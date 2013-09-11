using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using BirthdayBumper.Models;

namespace BirthdayBumper.Views
{
    public partial class WishFriend : PhoneApplicationPage
    {
        public WishFriend()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }

        private void WishFriendBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out id))
            {
                WishFriendBrowser.Navigate(new Uri("https://www.facebook.com/" + id));
            }

        }

        
    }
}