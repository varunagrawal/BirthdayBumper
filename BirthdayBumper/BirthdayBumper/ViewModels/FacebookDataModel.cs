using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using BirthdayBumper.Models;

namespace BirthdayBumper.ViewModels
{
    class FacebookDataModel
    {
        public ObservableCollection<FB_Friend> Friends { get; set; }

        public FacebookDataModel()
        {
            this.Friends = new ObservableCollection<FB_Friend>();
        }
    }
}
