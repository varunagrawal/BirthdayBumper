using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayBumper.Models
{
    class BirthdayBumperContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/BirthdayBumper.sdf";

        // Pass the connection string to the base class.
        public BirthdayBumperContext(string connectionString): base(connectionString) 
        {
            this.Friends = this.GetTable<Friend>();
        }

        // Specify a single table for the to-do items.
        public Table<Friend> Friends { get; set; }
    }

}
