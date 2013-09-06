using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BirthdayBumper.Models;

namespace BirthdayBumper.ViewModels
{
    class FacebookDataModel : INotifyPropertyChanged
    {

        public ObservableCollection<FBFriend> Friends { get; private set; }
        
        public FacebookDataModel()
        {
            this.Friends = new ObservableCollection<FBFriend>(); 
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
