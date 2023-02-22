using SuperfastBlur;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using 文字转图片;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace TestModule1
{
    public class AnimeImageHelper
    {
        public void CreateImage(string s,string _f,int _i)
        {
            int wid = 400;
            int high = 200;
            Font font = new Font(_f, _i, FontStyle.Regular);

            //绘笔颜色
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            Bitmap image = new Bitmap(wid, high);
            Graphics g = Graphics.FromImage(image);
            SizeF sizef = g.MeasureString(s, font, PointF.Empty, format);//得到文本的宽高
            int width = (int)(sizef.Width + 1);
            int height = (int)(sizef.Height + 1);
            Console.WriteLine($"文本信息\n宽：{width}\n高：{height}");

            int _off = 40;
            int _width = width + _off;
            int _height = height + _off;
            string api_url = "https://api.yimian.xyz/img/?display=1024-*x1024-*";
            var client = new WebClient();
            Console.WriteLine("[图片处理模块] 在线图片获取中...");
            var sw = Stopwatch.StartNew();
            var sm = client.OpenRead(api_url); // 通过 API接口 下载在线图片
            var bm = new Bitmap(sm);
            client.Dispose();

            sm.Flush();
            sm.Close();

            if (bm != null)
            {
                Console.WriteLine($"[图片处理模块] 在线图片获取完成！用时：{sw.ElapsedMilliseconds}毫秒");
            }

            // 模糊化处理图像

            Console.WriteLine("[图片处理模块] ACI: Blur Image...");
            var sw2 = Stopwatch.StartNew();
            
            GaussianBlur gb = new GaussianBlur(bm);
            var gbd = gb.Process(5);

            gbd.Save("./临时文件夹/image_pre.png", ImageFormat.Png);

            // 等比缩放图片
            var gbd2 = ThumbnailMaker.MakeThumbnail(gbd, _width, _height, ThumbnailMode.UsrHeightWidthBound);

            // 往上打字
            g = Graphics.FromImage(gbd2);
            float dpi = g.DpiY;
            RectangleF rect = new RectangleF(0, 0, width, height);

            using (GraphicsPath path = GetStringPath(s, dpi, rect, font, format))
            {
                RectangleF off = rect;
                RectangleF off2 = rect;
                off.Offset(21, 21);
                off2.Offset(20, 20);
                using (GraphicsPath offPath = GetStringPath(s,dpi,off,font,format))
                {
                    Brush b = new SolidBrush(Color.FromArgb(100,0,0,0));
                    g.FillPath(b, offPath);
                    b.Dispose();
                    
                }

                using (GraphicsPath offPath2 = GetStringPath(s, dpi, off2, font, format))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(Pens.Black, offPath2);
                    g.FillPath(Brushes.White, offPath2);
                }
            }

            g.Dispose();
            bm.Dispose();

            gbd.Dispose();
            gbd2.Save("./临时文件夹/image_a.png", ImageFormat.Png);
            gbd2.Dispose();
            Console.WriteLine($"[图片处理模块] ACI: Image Saved in {sw2.ElapsedMilliseconds}ms");
        }
        GraphicsPath GetStringPath(string s,float dpi,RectangleF rect,Font font,StringFormat format)
        {
            GraphicsPath path = new GraphicsPath();
            float emSize = dpi * font.SizeInPoints / 72;
            path.AddString(s,font.FontFamily,(int)font.Style, emSize, rect, format);

            return path;
        }
    }
}
