using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Rental_status
{
    public class UmbrellaStatus
    {
        public string _id { get; set; }
        public int umbrellaNumber { get; set; }
        public int __v { get; set; }
        public int status { get; set; }
        public DateTime endDate { get; set; }
        public DateTime startDate { get; set; }
    }

    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            label2.Parent = button1;
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(label2.Left - button1.Left, label2.Top - button1.Top);

            label3.Parent = button2;
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(label3.Left - button2.Left, label3.Top - button2.Top);

            label4.Parent = button3;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(label4.Left - button3.Left, label4.Top - button3.Top);

            label5.Parent = button4;
            label5.BackColor = Color.Transparent;
            label5.Location = new Point(label5.Left - button4.Left, label5.Top - button4.Top);

            label6.Parent = button5;
            label6.BackColor = Color.Transparent;
            label6.Location = new Point(label6.Left - button5.Left, label6.Top - button5.Top);

            label7.Parent = button6;
            label7.BackColor = Color.Transparent;
            label7.Location = new Point(label7.Left - button6.Left, label7.Top - button6.Top);

            pictureBox2.Parent = button1;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Location = new Point(pictureBox2.Left - button1.Left, pictureBox2.Top - button1.Top);

            pictureBox3.Parent = button2;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.Location = new Point(pictureBox3.Left - button2.Left, pictureBox3.Top - button2.Top);

            pictureBox4.Parent = button3;
            pictureBox4.BackColor = Color.Transparent;
            pictureBox4.Location = new Point(pictureBox4.Left - button3.Left, pictureBox4.Top - button3.Top);

            pictureBox5.Parent = button4;
            pictureBox5.BackColor = Color.Transparent;
            pictureBox5.Location = new Point(pictureBox5.Left - button4.Left, pictureBox5.Top - button4.Top);

            pictureBox6.Parent = button5;
            pictureBox6.BackColor = Color.Transparent;
            pictureBox6.Location = new Point(pictureBox6.Left - button5.Left, pictureBox6.Top - button5.Top);

            pictureBox7.Parent = button6;
            pictureBox7.BackColor = Color.Transparent;
            pictureBox7.Location = new Point(pictureBox7.Left - button6.Left, pictureBox7.Top - button6.Top);

            await UpdateUmbrellaStatus();
        }

        private async Task UpdateUmbrellaStatus()
        {
            try
            {
                string apiUrl = "https://port-0-cloud-lylb047299de6c8f.sel5.cloudtype.app/umb/status";
                string response = await httpClient.GetStringAsync(apiUrl);
                var umbrellaStatuses = JsonConvert.DeserializeObject<List<UmbrellaStatus>>(response);

                var sortedStatuses = umbrellaStatuses.OrderBy(x => x.umbrellaNumber).ToList();

                for (int i = 0; i < sortedStatuses.Count; i++)
                {
                    var status = sortedStatuses[i];
                    switch (status.umbrellaNumber)
                    {
                        case 1:
                            UpdateUmbrellaUI(status, button1, label2, label8, label14);
                            break;
                        case 2:
                            UpdateUmbrellaUI(status, button2, label3, label9, label15);
                            break;
                        case 3:
                            UpdateUmbrellaUI(status, button3, label4, label10, label16);
                            break;
                        case 4:
                            UpdateUmbrellaUI(status, button4, label5, label11, label17);
                            break;
                        case 5:
                            UpdateUmbrellaUI(status, button5, label6, label12, label18);
                            break;
                        case 6:
                            UpdateUmbrellaUI(status, button6, label7, label13, label19);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("우산 상태 업데이트 중 오류가 발생했습니다: " + ex.Message);
            }
        }

        private void UpdateUmbrellaUI(UmbrellaStatus status, Button button, Label statusLabel, Label numberLabel, Label dateLabel)
        {
            this.Invoke((MethodInvoker)delegate
            {
                numberLabel.Text = $"우산 {status.umbrellaNumber}번";
                if (status.status == 0)
                {
                    button.BackColor = Color.Red;
                    statusLabel.Text = "대여불가능";
                }
                else
                {
                    button.BackColor = Color.Green;
                    statusLabel.Text = "대여가능";
                }
                dateLabel.Text = $"{status.startDate.ToLocalTime():yyyy-MM-dd} ~ {status.endDate.ToLocalTime():yyyy-MM-dd}";
            });
        }

        private async Task BorrowUmbrella(int umbrellaNumber)
        {
            try
            {
                string statusApiUrl = "https://port-0-cloud-lylb047299de6c8f.sel5.cloudtype.app/umb/status";
                string statusResponse = await httpClient.GetStringAsync(statusApiUrl);
                var umbrellaStatuses = JsonConvert.DeserializeObject<List<UmbrellaStatus>>(statusResponse);

                var umbrella = umbrellaStatuses.FirstOrDefault(x => x.umbrellaNumber == umbrellaNumber);

                if (umbrella == null)
                {
                    MessageBox.Show("우산 정보를 찾을 수 없습니다.");
                    return;
                }

                if (umbrella.status == 0)
                {
                    MessageBox.Show("이미 대여 중인 우산입니다.");
                    return;
                }

                string apiUrl = $"https://port-0-cloud-lylb047299de6c8f.sel5.cloudtype.app/borrow/{umbrellaNumber}";
                var response = await httpClient.PostAsync(apiUrl, null);
                string responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (response.IsSuccessStatusCode && result.success == true)
                {
                    MessageBox.Show($"우산 {umbrellaNumber}번 대여 성공!\n이메일: {result.email}");
                    await UpdateUmbrellaStatus();
                }
                else
                {
                    MessageBox.Show($"우산 {umbrellaNumber}번 대여 실패: {result.message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"우산 대여 중 오류 발생: {ex.Message}");
            }
        }

        private async Task ReturnUmbrella(int umbrellaNumber)
        {
            try
            {
                string apiUrl = $"https://port-0-cloud-lylb047299de6c8f.sel5.cloudtype.app/borrow/return/{umbrellaNumber}";
                var response = await httpClient.PostAsync(apiUrl, null);
                string responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (response.IsSuccessStatusCode && result.success == true)
                {
                    MessageBox.Show($"우산 {umbrellaNumber}번 반납 성공!\n이메일: {result.email}");
                    await UpdateUmbrellaStatus();
                }
                else
                {
                    MessageBox.Show($"우산 {umbrellaNumber}번 반납 실패: {result.message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"우산 반납 중 오류 발생: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e) => await BorrowUmbrella(1);
        private async void button2_Click(object sender, EventArgs e) => await BorrowUmbrella(2);
        private async void button3_Click(object sender, EventArgs e) => await BorrowUmbrella(3);
        private async void button4_Click(object sender, EventArgs e) => await BorrowUmbrella(4);
        private async void button5_Click(object sender, EventArgs e) => await BorrowUmbrella(5);
        private async void button6_Click(object sender, EventArgs e) => await BorrowUmbrella(6);

        // 반납 버튼 이벤트 추가
        private async void returnButton1_Click(object sender, EventArgs e) => await ReturnUmbrella(1);
        private async void returnButton2_Click(object sender, EventArgs e) => await ReturnUmbrella(2);
        private async void returnButton3_Click(object sender, EventArgs e) => await ReturnUmbrella(3);
        private async void returnButton4_Click(object sender, EventArgs e) => await ReturnUmbrella(4);
        private async void returnButton5_Click(object sender, EventArgs e) => await ReturnUmbrella(5);
        private async void returnButton6_Click(object sender, EventArgs e) => await ReturnUmbrella(6);
    }
}
