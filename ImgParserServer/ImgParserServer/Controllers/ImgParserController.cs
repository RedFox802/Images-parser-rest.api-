using System.Collections.Generic;
using System.Text.Json;
using ImgParserServer.Models;
using Microsoft.AspNetCore.Mvc;
using WebScan;
//using Newtonsoft.Json;

namespace ImgParserServer.Controllers
{
    [ApiController]
    [Route("ImgParser")]
    public class ImgParserController : ControllerBase
    {

        [HttpGet]
        public string Get(string url= "https://oir.mobi/664757-njashnye-kotiki.html", int imgCount=1, int threadCount=8)
        {
            //считывание кода страницы
            UrlWorker urlWok = new UrlWorker(url);
            //запуск основного процесса 
            ImageWorker imgWok = new ImageWorker(urlWok.GetHtml(), imgCount, threadCount);
            List<MyImage> images = imgWok.GoImageDownload();
            //запись результата в класс для сериализации 
            ImagesList result = new ImagesList(images);
            //результат в виде jsons
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize<ImagesList>(result, options);
        }
    }
}
