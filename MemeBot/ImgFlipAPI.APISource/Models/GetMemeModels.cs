using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImgFlipAPI.APISource.Models
{
    public class Meme
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class MemeList
    {
        public List<Meme> memes { get; set; }
    }

    public class GetMemeRoot
    {
        public bool success { get; set; }
        public MemeList data { get; set; }
    }
}
