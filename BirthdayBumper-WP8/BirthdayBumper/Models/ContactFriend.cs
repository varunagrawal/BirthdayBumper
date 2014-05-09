using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BirthdayBumper.Models
{
    /// <summary>
    /// Friends in your Phone Contacts
    /// </summary>
    class ContactFriend : Friend
    {
        public ContactFriend(string _id, string _name, string _day, string _month, string _year, BitmapImage _pic)
        {
            Id = _id;
            Name = _name;
            Day = _day;
            Month = _month;
            Picture = _pic;
            Wished = false;
        }

        
    }
}
