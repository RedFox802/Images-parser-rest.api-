using System;

namespace ImgParserServer.Models
{
    [Serializable]
    public class MyImage
    {
        public string ImageUrl { get; set; }
        public string ImageSize { get; set; }

        public MyImage(string imageUrl,string imageSize)
        {
            ImageSize = imageSize;

            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"{ImageUrl}\n size: {ImageSize}";
        }
    }
}
