using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace WebScan
{
    public partial class ParserForm : Form
    {   
        public ParserForm()
        {
            InitializeComponent();

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string myUrl = txtUrl.Text;
            int myImgCount = Convert.ToInt32(boxImgQuantity.Text);
            int myThreadCount = Convert.ToInt32(boxStreamQuantity.Text);

            //проверка url на корректрость
            try
            {
                WebRequest request = WebRequest.Create(myUrl);
                request.GetResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Введен некорректный url", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(myImgCount<=0 || myThreadCount <= 0)
            {
                MessageBox.Show("Введены некорректные параметры", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CreateResponse(myUrl, myImgCount, myThreadCount);

        }

        private void CreateResponse(string url,int imgCount,int threadCount)
        {
            //задается url сервера
            string URL = "https://localhost:44315/ImgParser";
            //параметры для метода get
            string urlParameters = $"?url={url}&imgCount={imgCount}&threadCount={threadCount}";

            //отправка запроса и получение результата
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                //сохранение ответа сервера на запрос в файл json
                using (StreamWriter SW = new StreamWriter(new FileStream("Result_of_request.json", FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    SW.Write(response.Content.ReadAsStringAsync().Result.ToString());
                    SW.Close();
                }
            }
            client.Dispose();
            MessageBox.Show("Ответ на запрос получен и загружен в файл Result_of_request.json", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
