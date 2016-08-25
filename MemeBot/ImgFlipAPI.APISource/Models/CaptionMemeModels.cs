using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgFlipAPI.APISource.Models
{
    public class CaptionMeme
    {
        public string url { get; set; }
        public string page_url { get; set; }
    }

    public class CaptionMemeRoot
    {
        public bool success { get; set; }
        public CaptionMeme data { get; set; }
    }
}
