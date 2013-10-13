using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BirthdayBumper.Models
{
    class FacebookFriend : Friend
    {
        public FacebookFriend(string _id, string _name, string _day, string _month, Uri _pic)
        {
            Id = _id;
            Name = _name;
            Day = _day;
            Month = _month;
            Picture = new BitmapImage(_pic);
            Wished = false;
        }


    }
}
