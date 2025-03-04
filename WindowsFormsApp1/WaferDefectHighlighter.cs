using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class WaferDefectHighlighter
    {
        /// <summary>
        /// 결함 좌표와 크기 정보를 기반으로 이미지에 하이라이트(사각형)를 그립니다.
        /// </summary>
        /// <param name="image">원본 이미지 (Mat)</param>
        /// <param name="defectPositions">결함 좌표 문자열 (예: "504:1042,514:1027,...")</param>
        /// <param name="defectSizes">결함 크기 문자열 (예: "2:2,3:4,...")</param>
        /// <param name="color">하이라이트 색상 (예: Scalar.Red)</param>
        /// <param name="thickness">사각형 테두리 두께</param>
        /// <returns>결함 하이라이트가 적용된 이미지</returns>
        public static Mat HighlightDefects(Mat image, string defectPositions, string defectSizes, Scalar color, int thickness)
        {
            if (image == null || image.Empty())
                return image;

            // 원본 이미지 클론
            Mat output = image.Clone();

            string[] posArray = defectPositions.Split(',');
            string[] sizeArray = defectSizes.Split(',');

            for (int i = 0; i < posArray.Length; i++)
            {
                // "x:y" 형태로 좌표 파싱
                string[] posParts = posArray[i].Split(':');
                if (posParts.Length != 2)
                    continue;

                if (!int.TryParse(posParts[0], out int x) || !int.TryParse(posParts[1], out int y))
                    continue;

                if (i >= sizeArray.Length)
                    break;

                // "width:height" 형태로 크기 파싱
                string[] sizeParts = sizeArray[i].Split(':');
                if (sizeParts.Length != 2)
                    continue;

                if (!int.TryParse(sizeParts[0], out int w) || !int.TryParse(sizeParts[1], out int h))
                    continue;

                // 좌표 (x,y)를 중심으로 사각형의 좌측 상단 좌표 계산
                int rectX = x - w / 2;
                int rectY = y - h / 2;

                // 결함 영역에 사각형 그리기
                Cv2.Rectangle(output, new Rect(rectX, rectY, w, h), color, thickness);
            }

            return output;
        }
        /// <summary>
        /// 결함 좌표와 크기 정보를 기반으로 이미지에 결함 종류별 하이라이트(정사각형/타원)를 그립니다.
        /// </summary>
        /// <param name="image">원본 이미지 (Mat)</param>
        /// <param name="defectPositions">결함 좌표 문자열 (예: "504:1042,514:1027,...")</param>
        /// <param name="defectSizes">결함 크기 문자열 (예: "2:2,3:4,...")</param>
        /// <param name="mediumColor">Medium 결함에 사용할 색상 (예: Scalar.Red)</param>
        /// <param name="smallColor">Small 결함에 사용할 색상 (예: Scalar.Blue)</param>
        /// <param name="thickness">하이라이트 선 두께</param>
        /// <returns>결함 하이라이트가 적용된 이미지</returns>
        public static Mat HighlightDefectsByType(Mat image, string defectPositions, string defectSizes, Scalar mediumColor, Scalar smallColor, int thickness)
        {
            if (image == null || image.Empty())
                return image;

            // 원본 이미지를 클론하여 작업
            Mat output = image.Clone();

            // 좌표와 크기 문자열을 콤마(,) 기준으로 분리
            string[] posArray = defectPositions.Split(',');
            string[] sizeArray = defectSizes.Split(',');

            for (int i = 0; i < posArray.Length; i++)
            {
                // "x:y" 형태로 좌표 파싱
                string[] posParts = posArray[i].Split(':');
                if (posParts.Length != 2)
                    continue;
                if (!int.TryParse(posParts[0], out int x) || !int.TryParse(posParts[1], out int y))
                    continue;

                // 결함 크기 문자열이 부족하면 중단
                if (i >= sizeArray.Length)
                    break;

                // "width:height" 형태로 크기 파싱
                string[] sizeParts = sizeArray[i].Split(':');
                if (sizeParts.Length != 2)
                    continue;
                if (!int.TryParse(sizeParts[0], out int w) || !int.TryParse(sizeParts[1], out int h))
                    continue;

                // Medium 결함: 가로 또는 세로 중 하나라도 512 이상이면
                if (w >= 512 || h >= 512)
                {
                    // 정사각형 박스의 한 변 길이는 두 값 중 큰 값 사용
                    int side = (w >= h) ? w : h;
                    // defect 중심을 기준으로 정사각형의 좌측 상단 좌표 계산
                    int rectX = x - side / 2;
                    int rectY = y - side / 2;

                    Cv2.Rectangle(output, new Rect(rectX, rectY, side, side), mediumColor, thickness);
                }
                else
                {
                    // Small 결함: defect의 중심과 defect 크기를 이용해 타원(윤곽선) 그리기
                    // 타원은 중심, 축 길이(각 축의 길이의 절반) 및 회전각을 인자로 받습니다.
                    Cv2.Ellipse(output, new Point(x, y), new Size(w / 2, h / 2), 0, 0, 360, smallColor, thickness);
                }
            }

            return output;
        }
    }
}
