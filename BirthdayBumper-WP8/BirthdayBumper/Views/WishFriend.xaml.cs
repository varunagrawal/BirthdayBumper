using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BirthdayBumper.Views
{
    public partial class WishFriend : PhoneApplicationPage
    {
        public WishFriend()
        {
            InitializeComponent();
        }

        private void WishFriendBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            string site;
            if (NavigationContext.QueryString.TryGetValue("site", out site))
            {
                if (!string.IsNullOrEmpty(site))
                {
                    WishFriendBrowser.Navigate(new Uri(site));
                }
                else 
                {
                    MessageBox.Show("Entity site not present.");
                    NavigationService.GoBack();
                }

                //string jsFunc = "function(){ var x = querySelectorAll('textarea.textInput')[0]; x.innerHTML = \"Happy Birthday!\" }";   
            }

        }
    }
}