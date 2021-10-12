using ImgParserServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebScan
{
    class ImageWorker
    {

        private int ImgCount;
        private int ThreadCount;
        private List<string> WebLinks = new List<string>();
        private List<MyImage> Results = new List<MyImage>();

        public ImageWorker(string html, int imgCount, int threadCount)
        {
            if (imgCount != 0) ImgCount = imgCount;

            if (threadCount != 0)
            {
                //проверка на превышение максималного числа потоков
                int maxProcessorCount = Environment.ProcessorCount;
                ThreadCount = threadCount > maxProcessorCount ? maxProcessorCount : threadCount;
            }

            GetWebLinks(html);
        }

        public List<MyImage> GoImageDownload()
        {
            
            //опередяется количество изображений, которое будет обрабатывать каждый поток
            int theadImgCount = ImgCount / ThreadCount;

            //Циклическое создание необходимого числа потоков для скачивания
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < ThreadCount; i++)
            {
                //лист ссылок на изображения, с которыми будет работать каждый поток
                List<string> imageList = new List<string>();

                //каждому потоку выделяется нужное количество изображений, последнему потоку достаются все оставшиеся изображения 
                if (i == ThreadCount - 1)
                {
                    imageList = WebLinks;
                }
                else
                {
                    for (int j = 0; j < theadImgCount; j++)
                    {
                        imageList.Add(WebLinks[j]);
                        WebLinks.RemoveAt(j);
                    }
                }

                //Создание и запуск потока на преобразование и скачивание изображений
                Thread newThread = new Thread(new ParameterizedThreadStart(SaveImages));
                newThread.Start(imageList);
                threads.Add(newThread);
            }

            //DateTime endWork = DateTime.Now.AddMinutes(1); //время для выполнения задачи

            //while (true)
            //{
            //    int aliveCount = 0;
            //    foreach (var th in threads)
            //    {
            //        if (th.IsAlive) aliveCount++;
            //    }

            //    if (aliveCount == 0) break;

            //    if (DateTime.Now > endWork) //если минута истекла
            //    {
            //        foreach (var th in threads)
            //        {
            //            if (th.IsAlive) th.Abort();
            //        }
            //        break;
            //    }
            //}

            return Results;
        }


        //Для получения списка ссылок на другие сайты с определенного сайта
        public void GetWebLinks(string HtmlCode)
        {
            //нахождение тегов с сылками среди html
            Regex regex = new Regex(@"(<img([^>]+)>)");
            MatchCollection matches = regex.Matches(HtmlCode);

            if (matches.Count > 0)
            {
                //обрезка тегов до ссылок 
                for (int i = 0; i < matches.Count; i++)
                {
                    if (WebLinks.Count < ImgCount)
                    {
                        //выбор корректных изображений
                        if (matches[i].Value.IndexOf("https://") != -1 && (matches[i].Value.Contains("jpg") || matches[i].Value.Contains("png")))
                        {
                            //обрезка с начала ссылки https://
                            int indexStart = matches[i].Value.IndexOf("https://");
                            string img1 = matches[i].Value.Substring(indexStart);
                            //до  оконания ссылки
                            int indexEnd = img1.Contains("jpg".ToString()) ? img1.IndexOf("jpg") : img1.IndexOf("png");
                            string img = img1.Substring(0, indexEnd + 3);
                            if (!WebLinks.Contains(img)) WebLinks.Add(img);

                        }
                    }
                }
                //окончательное определение количества изображений и потоков
                ImgCount = WebLinks.Count;
                ThreadCount = ThreadCount > ImgCount ? ImgCount : ThreadCount;
            }

        }


        //Сохраняет изображения в папку
        private void SaveImages(Object obj)
        {
            List<string> urls = (List<string>)obj;
            //папка для сохранения
            string mypath = @"C:\Users\xiaomi\Documents\ImagesPath\";

            for (int i = 0; i < urls.Count; i++)
            {
                string name = urls[i].Substring(urls[i].LastIndexOf('/') + 1);//имя-последняя секция ссылки на изображение
                string fileName = mypath + $"{name}";//полный путь с именем
                Console.WriteLine(fileName);

                //скачивание
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(urls[i], fileName);
                        //определение размера изображения
                        FileInfo info = new FileInfo(fileName);
                        double imgSize = info.Length;

                        MyImage newImg = new MyImage(urls[i], $"{imgSize} bytes");
                        Results.Add(newImg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Не удалось скачать изображение {fileName}");
                    }                 
                }
            }
        }
    }
}
