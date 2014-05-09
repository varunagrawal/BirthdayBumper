using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BirthdayBumper.Models
{
    /// <summary>
    /// Facebook Friends
    /// </summary>
    class FacebookFriend : Friend
    {
        public FacebookFriend(string _id, string _name, string _day, string _month, string _year, Uri _pic, string _site)
        {
            Id = _id;
            Name = _name;
            Day = _day;
            Month = _month;
            Year = _year;
            Picture = new BitmapImage(_pic);
            Wished = false;
            Site = _site;
        }

    }
}
