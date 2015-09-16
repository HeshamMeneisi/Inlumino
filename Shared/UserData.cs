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
        public string __S = "";// star data
        public string __P = "";// package lock state
        public UserData()
        {}
        /// <summary>
        /// Interfacing properties
        /// </summary>      
        [XmlIgnore]
        internal Dictionary<PackageType, List<int>> PkgStars = new Dictionary<PackageType, List<int>>();
        [XmlIgnore]
        internal Dictionary<PackageType, bool> PackageAvailability = new Dictionary<PackageType, bool>
        {
            {PackageType.Beach,true}
        };
        public int getStars(PackageType pack, int indx)
        {
            if (!PkgStars.Keys.Contains(pack)) return 0;
            if (indx < PkgStars[pack].Count) return PkgStars[pack][indx];
            else return 0;
        }
        public void setStars(PackageType pack, int indx, int s)
        {
            if (!PkgStars.ContainsKey(pack)) PkgStars.Add(pack, new List<int>());
            List<int> temp = PkgStars[pack];
            while (indx >= temp.Count)
                temp.Add(0);
            temp[indx] = Math.Max(temp[indx], s);
            PkgStars[pack] = temp;
        }

        public void PrepareForSaving()
        {
            _timestamp = DateTime.Now;

            // package lock state
            List<string> data = new List<string>();
            foreach (PackageType pack in PackageAvailability.Keys)
                data.Add((int)pack + "|" + (PackageAvailability[pack] ? 1 : 0));
            __P = string.Join("~", data);
            // stars                        
            data = new List<string>();
            foreach (PackageType pack in PkgStars.Keys)
                data.Add((int)pack + "|" + String.Join("#",PkgStars[pack]));
            __S = String.Join("~", data);

            //EncryptStrings();
        }

        public void PostLoad()
        {
            // DecryptStrings();
            string[] data = __S.Split('~');
            foreach (string s in data)
            {
                try
                {
                    if (s == "") continue;
                    string[] pair = s.Split('|');
                    PackageType p = (PackageType)int.Parse(pair[0]);
                    PkgStars.Add(p, pair[1].Split('#').Select(t => int.Parse(t)).ToList());
                }
                catch { }
            }
            data = __P.Split('~');
            foreach (string s in data)
            {
                try
                {
                    if (s == "") continue;
                    string[] pair = s.Split('|');
                    PackageType p = (PackageType)int.Parse(pair[0]);
                    PackageAvailability.Add(p, pair[1] == "1");
                }
                catch { }
            }
        }
    }
}
