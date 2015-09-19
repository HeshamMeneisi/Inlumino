using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    public class UserData
    {
        public DateTime _timestamp = DateTime.Now;
        public string SData = "";// star data
        public string PData = "";// package lock state
        public UserData()
        { Encrypted = true; }
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
        internal void UpdateFrom(UserData online)
        {
            _timestamp = DateTime.Now;
            foreach (KeyValuePair<PackageType, bool> p in online.PackageAvailability)
            {
                if (p.Value)
                    MakeAvailable(p.Key);
            }
            foreach (KeyValuePair<PackageType, List<int>> p in online.PkgStars)
            {
                if (!PkgStars.ContainsKey(p.Key)) PkgStars.Add(p.Key, p.Value);
                else
                    for (int i = 0; i < p.Value.Count; i++)
                        setStars(p.Key, i, p.Value[i]);
            }
        }
        internal int getStars(PackageType pack, int indx)
        {
            if (!PkgStars.Keys.Contains(pack)) return 0;
            if (indx < PkgStars[pack].Count) return PkgStars[pack][indx];
            else return 0;
        }
        internal void setStars(PackageType pack, int indx, int s)
        {
            if (!PkgStars.ContainsKey(pack)) PkgStars.Add(pack, new List<int>());
            List<int> temp = PkgStars[pack];
            while (indx >= temp.Count)
                temp.Add(0);
            temp[indx] = Math.Max(temp[indx], s);
            PkgStars[pack] = temp;
        }
        internal void MakeAvailable(PackageType pack)
        {
            if (!PackageAvailability.ContainsKey(pack)) PackageAvailability.Add(pack, true);
            else PackageAvailability[pack] = true;
        }
        internal void UpdateRawData()
        {
            _timestamp = DateTime.Now;

            // package lock state
            List<string> data = new List<string>();
            foreach (PackageType pack in PackageAvailability.Keys)
                data.Add((int)pack + "~" + (PackageAvailability[pack] ? 1 : 0));
            PData = string.Join("|", data);
            // stars                        
            data = new List<string>();
            foreach (PackageType pack in PkgStars.Keys)
                data.Add((int)pack + "~" + String.Join(",", PkgStars[pack]));
            SData = String.Join("|", data);

            Encrypted = false;
        }

        internal void LoadRawData()
        {
            if (Encrypted) DecryptStrings();
            string[] data = SData.Split('|');
            foreach (string s in data)
            {
                try
                {
                    if (s == "") continue;
                    string[] pair = s.Split('~');
                    PackageType p = (PackageType)int.Parse(pair[0]);
                    PkgStars.Add(p, pair[1].Split(',').Select(t => int.Parse(t)).ToList());
                }
                catch { }
            }
            data = PData.Split('|');
            foreach (string s in data)
            {
                try
                {
                    if (s == "") continue;
                    string[] pair = s.Split('~');
                    PackageType p = (PackageType)int.Parse(pair[0]);
                    PackageAvailability.Add(p, pair[1] == "1");
                }
                catch { }
            }
        }        
        [XmlIgnore]
        public bool Encrypted { get; private set; }
        internal void EncryptStrings()
        {
            if (Encrypted) return;
            SData = SecurityProvider.Encrypt(SData);
            PData = SecurityProvider.Encrypt(PData);
            Encrypted = true;
        }
        internal void DecryptStrings()
        {
            if (!Encrypted) return;
            SData = SecurityProvider.Decrypt(SData);
            PData = SecurityProvider.Decrypt(PData);
            Encrypted = false;
        }
    }
}
