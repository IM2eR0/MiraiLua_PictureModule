using KeraLua;
using MiraiLua;
using TestModule1;

namespace MiraiLua_TXTtoIMG
{
    public class MiraiLuaModule
    {
        static object o = new object();
        static public Data Init()
        {
            o = ModuleFunctions.GetLock();//获取线程锁，防止异步操作崩Lua

            var data = new Data();
            data.WriteString("文字转图片模块");
            data.WriteString("3.3 公开版");
            data.WriteString("初雪 OriginalSnow");

            var lua = ModuleFunctions.GetLua();
            Util.PushFunction("imgapi", "makeimg", lua, Make);
            Util.PushFunction("imgapi", "makeaci", lua, MakeA);

            return data;
        }

        static int Make(IntPtr p)
        {
            var lua = Lua.FromIntPtr(p);
            try
            {
                Console.WriteLine("[图片处理模块] 正在处理图片...");
                ImageHelper im = new ImageHelper();

                string s = lua.CheckString(1);
                string font = lua.CheckString(2);
                int fontSize = (int)lua.CheckNumber(3);

                im.ImageCreate(s, font, fontSize);

                lua.PushString("临时文件夹/image.png");

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("[图片处理模块] 错误：" + e.Message);
                return 0;
            }
        }
        static int MakeA(IntPtr p)
        {
            var lua = Lua.FromIntPtr(p);
            try
            {
                Console.WriteLine("[图片处理模块] ACI正在处理图片...");
                AnimeImageHelper aci = new AnimeImageHelper();

                string s = lua.CheckString(1);
                string font = lua.CheckString(2);
                int size = (int)lua.CheckNumber(3);
                aci.CreateImage(s,font,size);

                lua.PushString("临时文件夹/image_a.png");

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("[图片处理模块] ACI错误：" + e.Message);
                return 0;
            }
        }
    }
}