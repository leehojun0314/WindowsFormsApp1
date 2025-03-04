using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Mat processedImage;
        Mat originImage;
        string imagePath;
        string _defectPositions;
        string _defectSizes;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // PictureBox 크기 모드 설정
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private string OpenImageFileDialog()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                dlg.Title = "이미지 파일 선택";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine(dlg.FileName);
                    return dlg.FileName;
                }
                else
                {
                    return null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("flag 1");
                // 모델 초기화

                Console.WriteLine("flag 2");
                // 모델 로드

                imagePath = OpenImageFileDialog();
                if (string.IsNullOrEmpty(imagePath))
                {
                    return;
                }

                Console.WriteLine("이미지가 성공적으로 로드되었습니다.");
                LoadAndDisplayOriginalImage(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("예외 발생: " + ex.Message);
            }
        }
        private void LoadAndDisplayOriginalImage(string imagePath)
        {
            originImage = Cv2.ImRead(imagePath);
            if (originImage.Empty())
            {
                MessageBox.Show("이미지를 로드할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Console.WriteLine(originImage.ToString());
            pictureBox1.Image = BitmapConverter.ToBitmap(originImage);
            pictureBox1.Refresh(); // 폼 리프레시
            pictureBox2.Image = null;
            processedImage = originImage.Clone();
        }
        private void LoadAndDisplayProcessedImage(Mat mat)
        {
            pictureBox2.Image = BitmapConverter.ToBitmap(mat);
            pictureBox2.Refresh(); // 폼 리프레시
            processedImage = mat.Clone();
        }
        private void LoadAndDisplayOriginalImage(Mat mat)
        {
            pictureBox1.Image = BitmapConverter.ToBitmap(mat);
            pictureBox1.Refresh(); // 폼 리프레시
            originImage = mat.Clone();
        }
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (originImage == null || originImage.Empty())
                return;

            int zoomSize = 100;

            // PictureBox1 크기와 원본 이미지 크기의 비율 계산
            double scaleX = (double)originImage.Width / pictureBox1.Width;
            double scaleY = (double)originImage.Height / pictureBox1.Height;

            // 마우스 좌표를 원본 이미지 좌표로 변환
            int x = (int)(e.X * scaleX);
            int y = (int)(e.Y * scaleY);

            // ROI 좌표 계산 (이미지 경계 초과 방지)
            x = Math.Max(0, Math.Min(x - zoomSize / 2, originImage.Width - zoomSize));
            y = Math.Max(0, Math.Min(y - zoomSize / 2, originImage.Height - zoomSize));

            Rect zoomRect = new Rect(x, y, zoomSize, zoomSize);
            //에러 방지용 주석
            // `processedImage`에서 해당 영역 추출 후 확대
            Mat zoomedImage = new Mat(processedImage, zoomRect);
            Cv2.Resize(zoomedImage, zoomedImage, new OpenCvSharp.Size(zoomSize * 3, zoomSize * 3));

            pictureBox2.Image = BitmapConverter.ToBitmap(zoomedImage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //LoadAndDisplayProcessedImage(EmguLib.RunHeatmapEmgu(originImage));
            if (originImage == null || originImage.Empty())
            {
                MessageBox.Show("처리할 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 예시 데이터: 실제로는 테이블이나 파일 등에서 가져올 수 있습니다.
            //string defectPositions = "898:816,1207:564,1932:175,1205:1250,793:2648,1203:2402,2399:3096,3120:2871,3600:2867,3649:2853,4490:2666";
            //string defectSizes = "2:2,2:2,2:3,2:2,50:50,2:2,2:2,2:2,2:2,2:2,2:2";
            // 하이라이트된 이미지를 생성 (빨간색, 두께 2)
            //Mat resultImage = WaferDefectHighlighter.HighlightDefects(originImage, defectPositions, defectSizes, Scalar.Red, 2);
            Mat resultImage = WaferDefectHighlighter.HighlightDefectsByType(originImage, _defectPositions, _defectSizes, Scalar.Red, Scalar.Blue, 15);

            // 결과 이미지를 화면에 표시
            //LoadAndDisplayProcessedImage(resultImage);
            processedImage = resultImage.Clone();

            LoadAndDisplayOriginalImage(resultImage);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _defectPositions = textBox1.Text;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _defectSizes = textBox2.Text;
        }
    }
}
