﻿using System;
using System.Windows.Forms;

namespace FynnMouseMgt
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine("Hit me!");
            HotKeyManager.RegisterHotKey(Keys.A, KeyModifiers.Alt);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            Console.ReadLine();      
        }

        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            Console.WriteLine("Hit me!");
        }
    }
}
