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
        byte[] Pixels; // déclaration du tableau pixel qui contiendra les  pixel du bitmapimage ouvert grace à dlg
        int Stride =0;// 4 octect * par largeur de l'image stride == RowSize sur le wiki 
        bool photoDisplayed; // déclaration du booléen photodisplayed qui permet de savoir si une photo est déjà affichée
        BitmapImage bmp; // déclaration de la variable bmp au format bitmap image qui correspond au fichier image choisi grace à dlg
		
        public Window1()
		{
            InitializeComponent(); // initialiser le xaml
            //initialisation de dlg
            dlg = new Microsoft.Win32.OpenFileDialog(); 
            dlg.DefaultExt = ".jpg"; // initialisation du filtre par défaut de dlg
            dlg.Filter = "Photo .jpg|*.jpg| Photo .png|*.png"; // Filter files by extension / Photo jpg (.jpg) c'est du visuel et |*.jpg| expression réguliere permet d'ouvrir le format .Jpg et .png
            photoDisplayed = false;
		}
		       private void OpenImage(object sender, RoutedEventArgs e)
        {

            // Show open file dialog box
            bool? result = dlg.ShowDialog(); // booleen a v3 valeur bool ? appel de la méthode OpenFileDialog.showdialog https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.win32.commondialog.showdialog?view=netframework-4.7.2#Microsoft_Win32_CommonDialog_ShowDialog

            // Process open file dialog box results
            if (result == true) // on vérifie qu'un fichier a bien été séléctionné dans dlg
            {
            	textbox.Text ="";
                bmp = new BitmapImage(new Uri(dlg.FileName)); // vu qu'on a a filtré dlg pour qu'il n'ouvre que des .jpg ou .png, on sait que le fichier séléctionné peut être ouvert comme une bitmap image
                // https://fr.wikipedia.org/wiki/Uniform_Resource_Identifier
                Cadre.Source = bmp; // on affiche dans notre cadre l'image séléctionnée dans dlg
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.pixelformat.bitsperpixel?view=netframework-4.7.2
                // stride = largeur de l'image
                // permet de faire la différencce entre une image qui fait 10 000 pixels
                // Pixel a une dimension 
                // si le stride est à 100 elle fait 100*100
                // si le stride fait 50 elle fait 50*200
                Stride = 4 * ((bmp.PixelWidth * bmp.Format.BitsPerPixel + 31) / 32);// 4 octect * par largeur de l'image stride == RowSize sur le wiki 

                // https://en.wikipedia.org/wiki/BMP_file_format
                int PixelArraySize = bmp.PixelHeight * Stride;
                Pixels = new byte[PixelArraySize];// Pixels va pouvoir contenir toutes les infos sur tous nos pixels, puisque un pixel fait 4 octets,si l'image fait 10 000 px Pixels fait 10 000 * 4 = 40 000 octets
                
                bmp.CopyPixels(Pixels, Stride, 0); //
                
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
                //on regarde si un message est déjà encodé dans l'image, si c'est le cas on affiche ce message dans le textBox
                int i = 0;
                while (i < PixelArraySize && Pixels[i + 3] != 255)// On sais que le chanel Alpha par ddéfaut a la valeur 255 donc si le nombre du chanel alpha est different de 255 il y a un message dedans
                {
                	textbox.Text += (char)Pixels[i + 3]; // Pixels qui à la base est un bytre, est casté en char (cast possible puisque byte et char font la même taille = 1 octet)
					//	                	TextBox.Text est un String donc on peut y ajouter des char grace à l'opérateur += qui concaténe le string et le char
					i += 4; // parce qu'on lit px par px et que chaque px fait 4 octets
               }
            }
        }

        private void SaveImage(object sender, RoutedEventArgs e)
        {
            if (photoDisplayed) // évite de crash si aucune photo n'est affichée
            {
                int i = 0;
                while (i < textbox.Text.Length && i < Pixels.Length ) // tant qu'il y a des lettres dans TextBox.text ET qu'on est pas arrivé au bout du tableau de Pixels
                {
                	Pixels[i * 4 + 3] = (byte)textbox.Text[i];// on avance de 4 par 4 car on ecrit uniquement dans le chanel alpha
                    i++;
                }
               PixelFormat pf = PixelFormats.Bgra32; // On utilise le format de pixel standard
               var newImg = BitmapSource.Create(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, pf, null, Pixels.ToArray(), Stride); // on créé une nouvelle bmp, en se basant sur les infos de la précédente
                Cadre.Source = newImg; // on met notre image modifié dans le cadre
               //https://stackoverflow.com/questions/4161359/save-bitmapimage-to-file --> important
                BitmapEncoder encoder = new PngBitmapEncoder(); // On a besoin d'un encoder pour pouvoir enregistrer notre image sur le disque /!\ On utilise un encoder PNG puisque l'encoder jpg, 
                //écrase les valeurs du channel alpha 
                // En gros on encode une image png dans un fichier qu'on appelle jpg, pour préserver le channel alpha
                
                encoder.Frames.Add(BitmapFrame.Create(newImg)); // on ajoute les images que l'on souhaite sauvegarder dans l'encoder
                var newFile = dlg.FileName.Insert(dlg.FileName.Length -4, Guid.NewGuid().ToString()); //On crée une nom pour notre nouvelle image (elle se trouvera à côté de l'image qui a été séléctionnée)
                
                //Tu fais un genre de dlg pour séléctionner un dossier ShowDialog
                //tu récupére le dossier séléctionné,
                //Tu ouvres une popup avec un textBox dedans pour que le mec choissise le nom de son image
                //newFile = dossierSelectionné + nomImage + ".jpg";
                
                // C/Users/Nicolas/Picture/sassassin-s-creed-iv-black-flag-37220-wp.jpg ==> variable dlg.Filename
                // dans cette variable, on insert à la position Length - 4 C/Users/Nicolas/Picture/sassassin-s-creed-iv-black-flag-37220-wp(Length - 4).jpg
                // un guid pour le résultat C/Users/Nicolas/Picture/sassassin-s-creed-iv-black-flag-37220-wpMONNOUVEAUGUID.jpg
                // C'est pourquoi la nouvelle image est sauvegardée à côté de celle qui a été séléctionnée à la base

                using (var fileStream = new System.IO.FileStream(newFile, System.IO.FileMode.Create))
				{
    				encoder.Save(fileStream);
				}
                // utiliser using, revient à écrire le code suivant :
                //var fileStream = new System.IO.FileStream(newFile, System.IO.FileMode.Create);
                //encoder.Save(fileStream);
                //fileStream.Dispose();
                // using permet d'évité les fuites memoire si le dev oublie d'annuler le .Dispose
                
                // puisque l'image affichée est celle qui est désormais sauvegardée sur le disque, on met à jour toutes les variables, comme si on venait d'ouvrir cette image dans OpenImage

  	            Cadre.Source = null; // vide le cadre
                textbox.Text =""; // vide la texte box
                photoDisplayed = false; // passe la variable photo display  à faux

                
                // popup.box xpf 
            }
        }
	}
}