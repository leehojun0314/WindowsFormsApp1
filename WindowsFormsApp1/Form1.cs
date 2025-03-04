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

        private void button2_Click(object sender, EventArgs e)
        {
            //LoadAndDisplayProcessedImage(EmguLib.RunHeatmapEmgu(originImage));
            if (originImage == null || originImage.Empty())
            {
                MessageBox.Show("처리할 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 예시 데이터: 실제로는 테이블이나 파일 등에서 가져올 수 있습니다.
            string defectPositions = "898:816,1207:564,1932:175,1205:1250,793:2648,1203:2402,2399:3096,3120:2871,3600:2867,3649:2853,4490:2666";
            string defectSizes = "2:2,2:2,2:3,2:2,50:50,2:2,2:2,2:2,2:2,2:2,2:2";
            // 하이라이트된 이미지를 생성 (빨간색, 두께 2)
            //Mat resultImage = WaferDefectHighlighter.HighlightDefects(originImage, defectPositions, defectSizes, Scalar.Red, 2);
            Mat resultImage = WaferDefectHighlighter.HighlightDefectsByType(originImage, defectPositions, defectSizes, Scalar.Red, Scalar.Blue, 15);

            // 결과 이미지를 화면에 표시
            //LoadAndDisplayProcessedImage(resultImage);
            processedImage = resultImage.Clone();

            LoadAndDisplayOriginalImage(resultImage);
        }
    }
}
