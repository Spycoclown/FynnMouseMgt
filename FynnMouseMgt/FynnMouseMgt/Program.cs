using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FynnMouseMgt
{
    class Program
    {
        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        public static bool fast = false;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);

        static void Main(string[] args)
        {

            // Schnell machen:
            HotKeyManager.RegisterHotKey(Keys.A, KeyModifiers.None);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);

            // Langsam machen:
            SlowHotKeyManager.RegisterHotKey(Keys.B, KeyModifiers.Alt);
            SlowHotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressedSlow);

            Console.ReadLine();      
        }

        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (fast)
            {
                return;
            }
            fast = true;

            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                uint.Parse("10"),
                0);
            Console.WriteLine("Fast!");

        }

        static void HotKeyManager_HotKeyPressedSlow(object sender, HotKeyEventArgs e)
        {
            if(!fast)
            {
                return;
            }
            fast = false;
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                uint.Parse("5"),
                0);
            Console.WriteLine("Slow!");

        }
    }
}
