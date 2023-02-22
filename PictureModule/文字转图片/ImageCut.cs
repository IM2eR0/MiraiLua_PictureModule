using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 文字转图片
{
    public class ThumbnailMaker
    {
        /// <summary>
        /// 制作图片的缩略图
        /// </summary>
        /// <param name="originalImage">原图</param>
        /// <param name="width">指定的宽度</param>
        /// <param name="height">指定的高度</param>
        /// <param name="mode">指定的模式</param>
        /// <remarks>
        ///     <paramref name="mode">
        ///         <para>ThumbnailMode.UsrHeightWidth:指定高宽缩放（可能变形）</para>
        ///         <para>ThumbnailMode.UsrHeightWidthBound:指定高宽缩放（可能变形,过小则不变）</para>
        ///         <para>ThumbnailMode.UsrWidth:指定宽，高按比例</para>
        ///         <para>ThumbnailMode.UsrWidthBound:指定宽（过小则不变），高按比例</para>
        ///         <para>ThumbnailMode.UsrHeight:指定高，宽按比例</para>
        ///         <para>ThumbnailMode.UsrHeightBound:指定高（过小则不变,宽按比例)</para>
        ///     </paramref>
        /// </remarks>
        /// <returns></retCut,urns>
        public static Image MakeThumbnail(Image originalImage, int width, int height, ThumbnailMode mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int initWidth = originalImage.Width;
            int initHeight = originalImage.Height;


            switch (mode)
            {
                case ThumbnailMode.UsrHeightWidth: //指定高宽缩放（可能变形）
                    break;
                case ThumbnailMode.UsrHeightWidthBound: //指定高宽缩放（可能变形）（过小则不变）
                    if (originalImage.Width <= width && originalImage.Height <= height)
                    {
                        return originalImage;
                    }
                    if (originalImage.Width < width)
                    {
                        towidth = originalImage.Width;
                    }
                    if (originalImage.Height < height)
                    {
                        toheight = originalImage.Height;
                    }
                    break;
                case ThumbnailMode.UsrWidth: //指定宽，高按比例
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.UsrWidthBound: //指定宽（过小则不变），高按比例
                    if (originalImage.Width <= width)
                    {
                        return originalImage;
                    }
                    else
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    break;
                case ThumbnailMode.UsrHeight: //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.UsrHeightBound: //指定高（过小则不变），宽按比例
                    if (originalImage.Height <= height)
                    {
                        return originalImage;
                    }
                    else
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    break;
                case ThumbnailMode.Cut: //指定高宽裁减（不变形）
                    //计算宽高比
                    double srcScale = (double)originalImage.Width / (double)originalImage.Height;
                    double destScale = (double)towidth / (double)toheight;
                    //宽高比相同
                    if (srcScale - destScale >= 0 && srcScale - destScale <= 0.001)
                    {
                        x = 0;
                        y = 0;
                        initWidth = originalImage.Width;
                        initHeight = originalImage.Height;
                    }
                    //源宽高比大于目标宽高比
                    //(源的宽比目标的宽大)
                    else if (srcScale > destScale)
                    {
                        initWidth = originalImage.Height * towidth / toheight;
                        initHeight = originalImage.Height;
                        x = (originalImage.Width - initWidth) / 2;
                        y = 0;
                    }
                    //源宽高比小于目标宽高小，源的高度大于目标的高度
                    else
                    {
                        initWidth = originalImage.Width;
                        initHeight = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - initHeight) / 2;
                    }
                    break;
                default:
                    break;
            }
            Image bitmap = new Bitmap(towidth, toheight);
            //新建一个画板
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //设置高质量插值法
                g.CompositingQuality = CompositingQuality.HighQuality;
                //设置高质量,低速段呈现的平滑程度
                g.SmoothingMode = SmoothingMode.HighQuality;

                //在指定的位置上,并按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, initWidth, initHeight), GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        public static Image MakeThumbnail(Stream originalStream, int width, int height, ThumbnailMode mode)
        {
            Image originalImage = Image.FromStream(originalStream);
            try
            {
                return MakeThumbnail(originalImage, width, height, mode);
            }
            catch (Exception)
            {
                originalStream.Dispose();
                return null;
            }
        }

        public static void MakeThumbnail(Image originalImage, string savePath, int width, int height, ThumbnailMode mode)
        {
            Image image = MakeThumbnail(originalImage, width, height, mode);
            try
            {
                image.Save(savePath);
            }
            catch (Exception)
            {
                image.Dispose();
            }
        }

        public static void MakeThumbnail(string originalImagePath, string savePath, int width, int height, ThumbnailMode mode)
        {
            Image image = Image.FromFile(originalImagePath);
            try
            {
                MakeThumbnail(image, savePath, width, height, mode);
            }
            catch (Exception)
            {
                image.Dispose();
            }
        }

    }


    public enum ThumbnailMode
    {
        /// <summary>
        /// 宽高缩放模式,可能变形
        /// </summary>
        UsrHeightWidth,
        UsrHeightWidthBound,
        /// <summary>
        /// 指定宽度,高按比例
        /// </summary>
        UsrWidth,
        /// <summary>
        /// 指定宽（过小则不变），高按比例
        /// </summary>
        UsrWidthBound,
        /// <summary>
        /// 自定高度,宽按比例
        /// </summary>
        UsrHeight,
        /// <summary>
        /// 指定高(过小则不变),宽按比例
        /// </summary>
        UsrHeightBound,
        /// <summary>
        /// 剪切
        /// </summary>
        Cut,
        NONE,
    }
}