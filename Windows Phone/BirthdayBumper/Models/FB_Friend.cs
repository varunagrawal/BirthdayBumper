using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayBumper.Models
{
    class FB_Friend : Friend
    {
        public FB_Friend(string _id, string _name, string _day, string _month, Uri _pic)
        {
            Id = _id;
            Name = _name;
            Day = _day;
            Month = _month;
            PictureUri = _pic;
            Wished = false;
        }


    }
}
