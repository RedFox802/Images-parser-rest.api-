using System;
using System.Collections.Generic;

namespace ImgParserServer.Models
{
    [Serializable]
    public class ImagesList
    {
        public List<MyImage> Images { get; set; }

        public ImagesList(List<MyImage>  images)
        {
            Images = images;
        }
    }
}
