using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BirthdayBumper.Models
{
    class Friend
    {
        private string id;

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
