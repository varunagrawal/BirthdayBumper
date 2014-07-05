using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace BirthdayBumper.Models
{
    /// <summary>
    /// Google Friends
    /// </summary>
    class GoogleFriend : Friend
    {
        public GoogleFriend()
        {
            Type = "google";
        }

        public GoogleFriend(string _id, string _name, string _day, string _month, string _year, Uri _pic, string _site)
        {
            Id = _id;
            Name = _name;
            Day = _day;
            Month = _month;
            Picture = _pic.ToString();
            Wished = false;
            Site = _site;
            Type = "google";
        }
    }

    // Classes for XML Deserialization
    [XmlRoot(ElementName = "feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Feed
    {
        [XmlElement("entry")]
        public List<Entry> Entries { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("category")]
        public string Category { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

    }

    public class Entry
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("content")]
        public string Content { get; set; }

        [XmlElement("link")]
        public List<Link> Links { get; set; }        // for user image

        [XmlElement("name", Namespace = "http://schemas.google.com/g/2005")]
        public GdName Name { get; set; }

        [XmlElement("birthday", Namespace = "http://schemas.google.com/contact/2008")]
        public GBirthday Birthday { get; set; }

        [XmlElement("organization", Namespace = "http://schemas.google.com/g/2005")]
        public Organization Org { get; set; }

        [XmlElement("email", Namespace = "http://schemas.google.com/g/2005")]
        public List<GEmail> Email { get; set; }

        [XmlElement("phoneNumber", Namespace = "http://schemas.google.com/g/2005")]
        public List<string> PhoneNumber { get; set; }

        [XmlElement("website", Namespace = "http://schemas.google.com/contact/2008")]
        public List<Website> Websites { get; set; }

        [XmlElement("groupMembershipInfo", Namespace = "http://schemas.google.com/contact/2008")]
        public List<Group> Groups { get; set; }

    }

    public class GBirthday
    {
        [XmlAttribute("when")]
        public string Date { get; set; }
    }

    public class GEmail
    {
        [XmlAttribute("address")]
        public string Address { get; set; }
    }

    public class Organization
    {
        [XmlElement("orgName", Namespace = "http://schemas.google.com/g/2005")]
        public string Name { get; set; }
    }

    public class Website
    {
        [XmlAttribute("href")]
        public string Url { get; set; }
    }

    public class Group
    {
        [XmlAttribute("href")]
        public string Url { get; set; }
    }

    public class GdName
    {
        [XmlElement("fullName")]
        public string FullName { get; set; }

        [XmlElement("givenName")]
        public string GivenName { get; set; }

        [XmlElement("familyName")]
        public string FamilyName { get; set; }
    }

    public class Link
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("href")]
        public string Image { get; set; }
    }
}
