using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;

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
            }
        }


        public string Year { get; set; }

        public BitmapImage Picture { get; set; }

        public bool Wished { get; set; }

        public string Type { get; set; }

        public string Site { get; set; }
    }
}
