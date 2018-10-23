using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //https://www.junian.net/wpf-load-external-image/
        //https://www.codeproject.com/Articles/9727/Image-Processing-Lab-in-C
        Microsoft.Win32.OpenFileDialog dlg;
        byte[] Pixels;
        bool photoDisplayed;
        BitmapImage bmp;
        public MainWindow()
        {
            InitializeComponent();
            dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Photo jpg (.jpg)|*.jpg| Photo .png|*.png"; // Filter files by extension
            photoDisplayed = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                bmp = new BitmapImage(new Uri(dlg.FileName));
                Cadre.Source = bmp;
                int Stride = 4 * (((int)bmp.Width * bmp.Format.BitsPerPixel + 31) / 32);
                Pixels = new byte[(int)bmp.PixelHeight * Stride];
                bmp.CopyPixels(Pixels, Stride, 0);
                photoDisplayed = true;
                //int i = 0;
                //while (i < (int)bmp.PixelHeight * Stride)
                //{
                //    System.Diagnostics.Debug.Write("R " + Pixels[i]);
                //    System.Diagnostics.Debug.Write(" G " + Pixels[i + 1]);
                //    System.Diagnostics.Debug.Write(" B " + Pixels[i + 2]);
                //    System.Diagnostics.Debug.WriteLine(" A " + Pixels[i + 3]);
                //    i += 4;
                //}
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (photoDisplayed)
            {
                int i = 0;
                while (i < textbox.Text.Length )
                {
                    Pixels[i * 4 + 3] = (byte)textbox.Text[i];
                    i++;
                }
                int stride = 4 * (((int)bmp.Width * bmp.Format.BitsPerPixel + 31) / 32);
               
                var newImg = BitmapSource.Create((int)bmp.Width, (int)bmp.Height, bmp.DpiX, bmp.DpiY, , null, Pixels.ToArray(), stride);
                Cadre.Source = newImg;
            }
        }
    }
}
