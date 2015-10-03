using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    public class PostInfo
    {
        public bool Posted = false;
        public object Tag { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public PostInfo(string title, string desc, string link)
        {
            Title = title; Description = desc; Link = link;
        }
    }
}
