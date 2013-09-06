using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BirthdayBumper.Models
{
    public class FBFriendDataContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/FB_Friends.sdf";

        public FBFriendDataContext(string connectionString): base(connectionString) { }

        public Table<FBFriend> FBFriends;
    }


    [Table]
    public class FBFriend : INotifyPropertyChanged
    {
        private string id;

        [Column(IsPrimaryKey = true, CanBeNull = false, DbType = "nvarchar")]
        public string Id
        {
            get
            {
                return id;
            }
                
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        
        private string name;

        [Column(CanBeNull = false, DbType = "nvarchar")]
        public string Name
        {
            get 
            {
                return name;
            }

            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        
        private string day;

        [Column(CanBeNull = false, DbType = "nvarchar")]
        public string Day
        {
            get 
            {
                return day;
            }

            set 
            {
                day = value;
                NotifyPropertyChanged("Day");
            }
        }

        
        private string month;

        [Column(CanBeNull = false, DbType = "nvarchar")]
        public string Month
        {
            get
            {
                return month;
            }

            set
            {
                month = value;
                NotifyPropertyChanged("Month");
            }
        }


        [Column(CanBeNull = false, DbType = "nvarchar")]
        public Uri PictureUri { get; set; }


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
