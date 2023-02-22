using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TestModule1
{
    public class ImageHelper
    {
        public void ImageCreate(string text,string font,int size)
        {
            try
            {
                Bitmap bmp = CreateImage(text,font,size);
                bmp.Save("./临时文件夹/image.png", ImageFormat.Png);
                Console.WriteLine("[图片处理模块] 图片已保存！");
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[图片处理模块] Image生成时错误：" + ex.Message.ToString());
            }
        }
        public void base64ToFile(string i)
        {
            byte[] arr = Convert.FromBase64String(i);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();

            bmp.Save("./临时文件夹/base64ToFile.png", ImageFormat.Png);
            Console.WriteLine("[图片处理模块] 图片已保存！");
            bmp.Dispose();
        }
        public byte[] ImageCreateBase64(string text, string font, int size)
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                Bitmap bmp = CreateImage(text, font, size);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();

                bmp.Dispose();
                return arr;

            }catch (Exception ex)
            {
                return null;
            }
        }
        public Bitmap CreateImage(string text,string custom_font,int size)
        {
            int wid = 400;
            int high = 200;
            Font font;

            font = new Font(custom_font, size, FontStyle.Regular);

            //绘笔颜色
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            Bitmap image = new Bitmap(wid, high);
            Graphics g = Graphics.FromImage(image);
            SizeF sizef = g.MeasureString(text, font, PointF.Empty, format);//得到文本的宽高

            int width = (int)(sizef.Width + 1);
            int height = (int)(sizef.Height + 1);

            image.Dispose();
            image = new Bitmap(width, height);

            g = Graphics.FromImage(image);
            g.Clear(Color.White);//透明

            RectangleF rect = new RectangleF(0, 0, width, height);
            //绘制图片
            g.DrawString(text, font, brush, rect);
            //释放对象
            g.Dispose();

            Console.WriteLine("[图片处理模块] 处理完毕！");
            return image;
        }
    }
}
