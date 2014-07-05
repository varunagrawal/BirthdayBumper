using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;

namespace BirthdayBumper.Models
{
    [Table]
    [InheritanceMapping(Code = "F", Type = typeof(FacebookFriend))]
    [InheritanceMapping(Code = "G", Type = typeof(GoogleFriend))]
    [InheritanceMapping(Code = "R", Type = typeof(Friend), IsDefault = true)]
    public class Friend : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int uid;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Uid
        {
            get
            {
                return uid;
            }

            set
            {
                if (uid != value)
                {
                    NotifyPropertyChanging("Uid");
                    uid = value;
                    NotifyPropertyChanged("Uid");
                }
            }
        }

        private string id;
        //[Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "NVARCHAR(50) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        [Column]
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                if (id != value)
                {
                    NotifyPropertyChanging("Id");
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        [Column(IsDiscriminator = true)]
        public string DiscKey;

        private string name;
        [Column]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    NotifyPropertyChanging("Name");
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }


        private string day;
        [Column]
        public string Day
        {
            get
            {
                return day;
            }
            set
            {
                if (day != value)
                {
                    NotifyPropertyChanging("Day");
                    day = value;
                    NotifyPropertyChanged("Day");
                }
            }
        }


        private string month;
        [Column]
        public string Month
        {
            get
            {
                return month;
            }
            set
            {
                if (month != value)
                {
                    NotifyPropertyChanging("Month");
                    month = value;
                    NotifyPropertyChanged("Month");
                }
            }
        }

        private string year;
        [Column]
        public string Year 
        {
            get
            {
                return year;
            }
            set
            {
                if (year != value)
                {
                    NotifyPropertyChanging("Year");
                    year = value;
                    NotifyPropertyChanged("Year");
                }
            }
        }

        private string picture;
        [Column]
        public string Picture 
        {
            get
            {
                return picture;
            }
            set
            {
                if (picture != value)
                {
                    NotifyPropertyChanging("Picture");
                    picture = value;
                    NotifyPropertyChanged("Picture");
                }
            }
        }

        private bool wished;
        [Column]
        public bool Wished 
        {
            get
            {
                return wished;
            }
            set
            {
                if (wished != value)
                {
                    NotifyPropertyChanging("Wished");
                    wished = value;
                    NotifyPropertyChanged("Wished");
                }
            }
        }

        [Column]
        public string Type { get; set; }

        private string site;
        [Column]
        public string Site 
        {
            get
            {
                return site;
            }
            set
            {
                if (site != value)
                {
                    NotifyPropertyChanging("Site");
                    site = value;
                    NotifyPropertyChanged("Site");
                }
            }
        }

        public static Byte[] ConvertToByteArray(BitmapImage bmi)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteableBitmap btmMap = new WriteableBitmap(bmi.PixelWidth, bmi.PixelHeight);
                Extensions.SaveJpeg(btmMap, ms, bmi.PixelWidth, bmi.PixelHeight, 0, 100);

                return ms.ToArray();
            }
        }

        public static BitmapImage ConvertToBitmapImage(Byte[] byteArray)
        {
            MemoryStream stream = new MemoryStream(byteArray);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
