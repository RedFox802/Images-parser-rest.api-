using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebScan
{
    class UrlWorker
    {
        private string Url;
        private string Html;

        public UrlWorker(string url)
        {
            Url = url;
            Html= ReadHtml(Url);
        }

        public string GetUrl() { return Url; }
        public string GetHtml() { return Html; }

        private string ReadHtml(string url)
        {
            //для хранения html кода сайта
            string data = "";

            //запрос на сервер
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //поток содержащий данные ответа
                Stream receiveStream = response.GetResponseStream();

                //Считываем поток
                StreamReader readStream = null;
                try
                {
                    readStream = String.IsNullOrWhiteSpace(response.CharacterSet) ? new StreamReader(receiveStream) :
                        new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                catch (System.ArgumentException)
                {
                    return null;
                }

                //результат считывание в строке 
                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();     
            }
            return data;
        }
    }
}
