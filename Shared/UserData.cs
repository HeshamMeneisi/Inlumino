using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    public class UserData
    {
        public string _ref = "";
        public DateTime _timestamp = DateTime.Now;
        public string _stars = "";

        /// <summary>
        /// Interfacing properties
        /// </summary>
        [XmlIgnore]
        internal int[] Stars
        {
            get
            {
                if (_stars == "") Stars = (new int[Common.MainLevelNames.Length]);
                return Encoding.ASCII.GetString(Convert.FromBase64String(_stars)).Split('#').Select((v) => Convert.ToInt32(v)).ToArray();
            }
            set
            {
                _stars = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Join("#", value)));
            }
        }
    }
}
