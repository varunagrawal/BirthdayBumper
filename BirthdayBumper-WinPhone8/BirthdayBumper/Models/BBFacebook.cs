using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Linq;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

using Facebook;
using Facebook.Client;

namespace BirthdayBumper.Models
{
    class BBFacebook
    {
        public static readonly string App_Id = "587441381302332";

        public static string AccessToken = String.Empty;
        public static string FacebookId = String.Empty;
        public static bool isAuthenticated = false;
        public static FacebookSessionClient fbSessionClient = new FacebookSessionClient(App_Id);

        

    }
}
