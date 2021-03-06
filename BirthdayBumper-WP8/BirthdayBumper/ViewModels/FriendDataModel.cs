﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Phone.UserData;
using System.Windows.Media.Imaging;

using Facebook;
using BirthdayBumper.ViewModels;
using BirthdayBumper.Models;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace BirthdayBumper.ViewModels
{
    class FriendDataModel : INotifyPropertyChanged
    {
        public ObservableCollection<Friend> Friends;

        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");

            }
        }

        /// <summary>
        /// Get Birthdays of Phone Contacts
        /// </summary>
        public Task<bool> GetContactsBirthdays()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            ObservableCollection<Friend> ContactFriends = new ObservableCollection<Friend>();

            Contacts cons = new Contacts();

            cons.SearchCompleted += (sender, e) =>
            {
                tcs.SetResult(false);

                List<Contact> contacts = new List<Contact>(e.Results);

                if (contacts.Count == 0)
                {
                    tcs.TrySetResult(true);
                }

                using (BirthdayBumperContext db = new BirthdayBumperContext(BirthdayBumperContext.DBConnectionString))
                {
                    foreach (var c in contacts)
                    {
                        DateTime d = DateTime.Now.AddDays(-1.0);

                        List<DateTime> birthdays = new List<DateTime>(c.Birthdays);
                        if (birthdays.Count > 0)
                            d = birthdays.First<DateTime>();

                        try
                        {
                            if (d.Date.Equals(DateTime.Today.Date))
                            {
                                //BitmapImage img = new BitmapImage();
                                //img.SetSource(c.GetPicture());

                                ContactFriend f = new ContactFriend(
                                    //c.GetHashCode().ToString(),
                                    c.PhoneNumbers.First() != null ? c.PhoneNumbers.First().ToString() : c.GetHashCode().ToString(),
                                    c.CompleteName.ToString(),
                                    d.Day.ToString(),
                                    d.Month.ToString(),
                                    d.Year.ToString(),
                                    "ContactImage"
                                    );

                                ContactFriends.Add(f);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }

                    db.Friends.InsertAllOnSubmit(ContactFriends);
                }
                
                tcs.TrySetResult(true);
            };

            cons.SearchAsync(String.Empty, FilterKind.None, "Contacts Sync");

            return tcs.Task;
        }


        public async Task<List<Friend>> GetFacebookBirthdays()
        {
            List<Friend> f = await FacebookAccount.GetFacebookBirthdays();
            return f;
        }

        public async Task<List<Friend>> GetGoogleBirthdays()
        {
            List<Friend> g = await GoogleAccount.GetGoogleBirthdays();
            return g;
        }

        private void AddToFriends(List<Friend> list)
        {
            foreach (Friend f in list)
            {
                Friends.Add(f);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
