using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameClub.Helpers
{
    public class HelpMethods
    {
        static public string ImageToBase64(BitmapImage image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (image == null)
                    return null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
        static public string GenerateCode()
        {
            Random random = new Random();
            string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            string code = "";
            for (int i = 0; i < 6; i++)
            {
                code += letters[random.Next(0,letters.Length)];
            }
            return code;
        }
        static public BitmapImage Base64ToImage(string base64String)
        {
            if (base64String == null || base64String == "")
                return null;
            byte[] binaryData = Convert.FromBase64String(base64String);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            return bi;
        }
        static public bool Letters(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsLetter(s[i]))
                    return false;
            }
            return true;
        }
        static public string CalculateTime(DateTime start, DateTime endTime)
        {
            if (DateTime.Now > endTime)
                return "Время вышло";
            if (DateTime.Now < start)
                return start.ToString("t");
            else
                return (new DateTime() + (endTime - DateTime.Now)).ToString("T");

        }
        public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = GetFirstVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }

            return null;
        }
    }
}
