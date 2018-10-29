/*
 * Created by SharpDevelop.
 * User: Nicolas
 * Date: 25/10/2018
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Steganov2
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	///    


	public partial class Window1 : Window
	{
		
		//https://www.junian.net/wpf-load-external-image/
        //https://www.codeproject.com/Articles/9727/Image-Processing-Lab-in-C
		Microsoft.Win32.OpenFileDialog dlg; //Représente une boîte de dialogue commune qui permet à un utilisateur de spécifier un nom de fichier 
											//pour un ou plusieurs fichiers à ouvrir.
        byte[] Pixels; // déclaration du tableau pixel
        bool photoDisplayed; // déclaration du booléen photodisplayed 
        BitmapImage bmp; // déclaration de la variable bmp au format bitmap image 
		
        public Window1()
		{
            InitializeComponent();
            dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Photo jpg (.jpg)|*.jpg| Photo .png|*.png"; // Filter files by extension / permet d'ouvrir le format Jpg et png
            photoDisplayed = false;
		}
		       private void OpenImage(object sender, RoutedEventArgs e)
        {

            // Show open file dialog box
            bool? result = dlg.ShowDialog(); // booleen a v3 valeur bool ?

            // Process open file dialog box results
            if (result == true)
            {
                bmp = new BitmapImage(new Uri(dlg.FileName));
                
                Cadre.Source = bmp;
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.pixelformat.bitsperpixel?view=netframework-4.7.2
                // stride = largeur de l'image
                // 
                int Stride = 4 * (((int)bmp.Width * bmp.Format.BitsPerPixel + 31) / 32);
                
                Pixels = new byte[(int)bmp.PixelHeight * Stride];// celui là
                
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
                int i = 0;
                while (i <(int)bmp.PixelHeight * Stride && Pixels[i +3] != 255)
                {
                    textbox.Text += (char)Pixels[i + 3];
                    i += 4;
               }
            }
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            if (photoDisplayed)
            {
                int i = 0;
                while (i < textbox.Text.Length && i < Pixels.Length )
                {
                    Pixels[i * 4 + 3] = (byte)textbox.Text[i];
                    i++;
                }
                int stride = 4 * (((int)bmp.Width * bmp.Format.BitsPerPixel + 31) / 32);
               PixelFormat pf = PixelFormats.Bgra32;
               var newImg = BitmapSource.Create((int)bmp.Width, (int)bmp.Height, bmp.DpiX, bmp.DpiY, pf, null, Pixels.ToArray(), stride);
                Cadre.Source = newImg;
               //Https://stackoverflow.com/questions/4161359/save-bitmapimage-to-file --> important
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(newImg));
                var newFile = dlg.FileName.Insert(dlg.FileName.Length -4, Guid.NewGuid().ToString());
                using (var fileStream = new System.IO.FileStream(newFile, System.IO.FileMode.Create))
				{
    				encoder.Save(fileStream);
				}
                bmp = new BitmapImage(new Uri(newFile));
                int Stride = 4 * (((int)bmp.Width * bmp.Format.BitsPerPixel + 31) / 32);
                Pixels = new byte[(int)bmp.PixelHeight * Stride];
                bmp.CopyPixels(Pixels, Stride, 0);
                photoDisplayed = true;
            }
        }
	}
}