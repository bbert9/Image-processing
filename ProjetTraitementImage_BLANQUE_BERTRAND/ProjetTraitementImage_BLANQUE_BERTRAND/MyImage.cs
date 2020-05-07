using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ProjetTraitementImage_BLANQUE_BERTRAND
{
    /// <summary>
    /// MyImage correspond a une image au format BMP avec tous ses parametres qui vont avec.
    /// </summary>
    public class MyImage
    {
        public string Type { get; set; }
        public string Nom_fichier { get; set; }
        public int TailleFichier { get; set; }
        public byte[] TailleFichier_byte { get; set; }
        public int TailleOffset { get; set; }
        public int Largeur { get; set; }
        public byte[] Largeur_byte { get; set; }
        public byte[] Hauteur_byte { get; set; }
        public int Hauteur { get; set; }
        public int NbreBitCouleur { get; set; }
        public byte[] Header { get; set; }
        public Pixel[,] MatriceRGB { get; set; } // faire une matrice de pixels 

        /// <summary>
        /// Constructeur qui crée un objet de MyImage a partir d'une fichier bmp deja existant.
        /// </summary>
        /// <param name="fichier"></param>
        public MyImage(string fichier)
        {
            Nom_fichier = fichier;
            if (fichier.Contains("bmp"))
            {
                this.Type = "bmp";

                byte[] myfile = File.ReadAllBytes(fichier);
                byte[] Header = new byte[54];

                for (int j = 0; j < 54; j++)
                {
                    Header[j] = myfile[j];
                }
                this.Header = Header;
                Largeur = Convertir_Endian_To_Int(Header.Skip(18).Take(4).ToArray());
                Hauteur = Convertir_Endian_To_Int(Header.Skip(22).Take(4).ToArray());
                this.TailleFichier_byte = new byte[4];
                TailleFichier_byte = Header.Skip(2).Take(4).ToArray();
                this.TailleFichier = myfile.Length;
                this.Hauteur_byte = new byte[4];
                this.Largeur_byte = new byte[4];
                this.Hauteur_byte = BitConverter.GetBytes(Hauteur);
                this.Largeur_byte = BitConverter.GetBytes(Largeur);

                this.MatriceRGB = new Pixel[(int)Hauteur, (int)Largeur];
                int g = 0;
                int file_index = 54;

                for (int i = 0; i < Hauteur; i = i + 1)
                {
                    for (int j = 0; j < 3 * Largeur; j = j + 3) //ligne multiple de 4 (taille=100 pixel don 400 bytes)
                    {
                        file_index = i * Largeur * 3 + j + 54;
                        //suit l'index du tableau file
                        if (file_index < (myfile.Length-2))
                        {
                            Pixel UnPixel = new Pixel(0, 0, 0);
                            
                            
                            UnPixel.Red = (int)myfile[file_index];
                            UnPixel.Green = (int)myfile[file_index + 1];
                            UnPixel.Blue = (int)myfile[file_index + 2];

                            //this.MatriceRGB[i, g] = new Pixel(myfile[file_index], myfile[file_index + 1], myfile[file_index + 2]);
                            this.MatriceRGB[i, g] = UnPixel;
                            g =g+1;
                        }
                    }
                    g = 0;
                }
                this.NbreBitCouleur = 24; //on code toujours en 3*8bits donc en 24 couleurs.
                //Process.Start(fichier);
            }
        }
        /// <summary>
        /// Crée un objet MyImage a partir de 0, on lui donne juste la hauteur et la largeur du fichier.
        /// </summary>
        /// <param name="hauteur"></param>
        /// <param name="largeur"></param>
        public MyImage(int hauteur, int largeur)
        {
            int taille = hauteur * largeur * 3+54;
            TailleFichier_byte = BitConverter.GetBytes(taille);
            Largeur_byte = BitConverter.GetBytes(largeur);
            Hauteur_byte = BitConverter.GetBytes(hauteur);
            Largeur = largeur;
            Hauteur = hauteur;
            TailleFichier = hauteur * largeur * 3 + 54 + 900;
            byte[] t = TailleFichier_byte;
            byte[] largeur_b = Largeur_byte;
            byte[] hauteur_b = Hauteur_byte;
            byte[] numbers = { 66, 77, t[0], t[1], t[2], t[3], 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, largeur_b[0], largeur_b[1], largeur_b[2], largeur_b[3], hauteur_b[0], hauteur_b[1], hauteur_b[2], hauteur_b[3], 1, 0, 24, 0, 0, 0, 0, 0, t[0], t[1], t[2], t[3], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            Header = numbers;
            MatriceRGB = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    Pixel un = new Pixel(1, 0, 1);
                    MatriceRGB[i, j] = un;
                }
            }
        }
        /// <summary>
        /// Permet de lancer une visualisation de l'image.
        /// </summary>
        public void Show()
        {
            Process.Start(Nom_fichier);
        }
        /// <summary>
        /// Donne les informations de l'image: header, valeurs des pixels.
        /// </summary>
        /// <param name="fichier"></param>
        public void BMP_info(string fichier)
        {
            byte[] myfile = File.ReadAllBytes(fichier);

            Console.WriteLine("Header");
            for (int i = 0; i < 14; i++)
            {
                Console.Write(myfile[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Header info");
            for (int i = 14; i < 54; i++)
            {
                Console.Write(myfile[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 54; i < myfile.Length; i = i + 100)
            {
                for (int j = i; j < i + 100; j++)
                {
                    Console.Write(myfile[j] + " ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// A partir d'un objet MyImage écrit un fichier BMP dans le bin --> debug.
        /// </summary>
        /// <param name="file"></param>
        public void From_Image_To_File(string file) //constructeur en chemin inverse
        {
            byte[] result = new byte[TailleFichier];
            //Console.WriteLine(TailleFichier);
            //Console.WriteLine(Convertir_Endian_To_Int(TailleFichier_byte));
            int R;
            int G;
            int B;
            //on réecrit le header dans un premier temps
            for (int h = 0; h < 54; h++)
            {
                result[h] = Header[h];
            }
            for (int i = 0; i < 4; i++)
            {
                result[i + 18] = Largeur_byte[i];
                result[i + 22] = Hauteur_byte[i];
                result[i + 2] = TailleFichier_byte[i];
            }
            int index = 0;

            for (int i = 0; i < this.Hauteur; i++)
            {
                for (int j = 0; j < this.Largeur; j = j + 1)
                {
                    index = i * Largeur * 3 + j * 3 + 54;
                    R = MatriceRGB[i, j].Red; 
                    G = MatriceRGB[i, j].Green;
                    B = MatriceRGB[i, j].Blue;
                    result[index] = (byte)R;
                    result[index + 1] = (byte)G;
                    result[index + 2] = (byte)B;
                }
            }
            File.WriteAllBytes(file + ".bmp", result);
            Process.Start(file + ".bmp");
        }
        /// <summary>
        /// Convertit un endian en entier
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int result;
            result = (int)Math.Pow(255, 0) * (int)tab[0] + (int)Math.Pow(255, 1) * (int)tab[1] + (int)Math.Pow(255, 2) * (int)tab[2] + (int)Math.Pow(255, 3) * (int)tab[3];
            return result;
        }
        /// <summary>
        /// Convertit un entier en endian de longueur 4.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] result = new byte[4];
            int Power3 = val % (int)(Math.Pow(255, 3));
            val = val - Power3 * (int)(Math.Pow(255, 3));
            int Power2 = val % (int)(Math.Pow(255, 2));
            val = val - Power2 * (int)(Math.Pow(255, 2));
            int Power1 = val % (int)(Math.Pow(255, 1));
            val = val - Power1 * (int)(Math.Pow(255, 1));
            int Power0 = val % (int)(Math.Pow(255, 0));
            val = val - Power0 * (int)(Math.Pow(255, 0));
            result[3] = (byte)Power3;
            result[2] = (byte)Power2;
            result[1] = (byte)Power1;
            result[0] = (byte)Power0;
            return result;
        }
        /// <summary>
        /// Prend un image et renvoit la meme image en nuance de gris.
        /// </summary>
        public void Nuance_gris()
        {
            int sum = 0;
            //Passage d’une photo couleur à une photo en nuances de gris
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    sum += MatriceRGB[i, j].Red + MatriceRGB[i, j].Green + MatriceRGB[i, j].Blue;
                    sum = (int)sum / 3;
                    MatriceRGB[i, j].Red = sum;
                    MatriceRGB[i, j].Green = sum;
                    MatriceRGB[i, j].Blue = sum;
                    sum = 0;
                }
            }
            From_Image_To_File("Nuance_gris");
        }
        /// <summary>
        /// Prend une image et renvoit la même image en noir et blanc
        /// </summary>
        public void Noir_et_blanc()
        {
            //Passage d’une photo couleur à une photo en noir et blanc
            int sum = 0;
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    sum += MatriceRGB[i, j].Red + MatriceRGB[i, j].Green + MatriceRGB[i, j].Blue;
                    sum = (int)(sum / 3);
                    if (sum >= (255 / 2))
                    {
                        MatriceRGB[i, j].Red = 255;
                        MatriceRGB[i, j].Green = 255;
                        MatriceRGB[i, j].Blue = 255;
                    }
                    if (sum < (255 / 2))
                    {
                        MatriceRGB[i, j].Red = 0;
                        MatriceRGB[i, j].Green = 0;
                        MatriceRGB[i, j].Blue = 0;
                    }
                    sum = 0;
                }
            }
            From_Image_To_File("Noir_et_blanc");
        }
        /// <summary>
        /// A partir d'une image et d'un angle en degres effectue une rotation de l'image de cet angle.
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(double angle)
        {
            angle = (3.14 / 180) * angle;
            //faire une rotation de l'image
            //ne marche pas pour l'instant
            MyImage Rotation = new MyImage(Hauteur, Largeur); //on copie lobjet myimage
            Rotation.Nom_fichier = "Rotation.bmp";
            Rotation.Largeur =Largeur;
            Rotation.Hauteur = Hauteur;
            Rotation.TailleFichier = TailleFichier;
            
            int x = 0;
            int y = 0;
            //Passage d’une photo couleur à une photo en nuances de gris
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    x = (int)(Math.Cos(angle) * (i - Hauteur / 2) - Math.Sin(angle) * (j - Largeur / 2));
                    y = (int)(Math.Cos(angle) * (j - Largeur / 2) + Math.Sin(angle) * (i - Hauteur / 2));
                    x += Hauteur / 2;
                    y += Largeur / 2;
                    if (x < Hauteur & x > 0 & y < Largeur & y > 0)
                    {
                        Rotation.MatriceRGB[x, y] = MatriceRGB[i, j];
                    }

                }
            }
            Console.WriteLine(Rotation.MatriceRGB[0,0].Red);
            Rotation.From_Image_To_File("Rotation");
        }
        /// <summary>
        /// Retrcit une image d'un certain ratio 0-100%
        /// </summary>
        /// <param name="ratio"></param>
        public void Retrecir(int ratio)
        {
            //ratio = pourcentage de hauteur et de largeur que l'on garde
            if(ratio==100) // ne marche pas pour 100
            {
                ratio = 99;
            }
            double diminution_facteur = (ratio * 0.01);
            int width = (int)(Largeur * diminution_facteur);
            int height = (int)(Hauteur * diminution_facteur);
            Largeur_byte = BitConverter.GetBytes(width);
            Hauteur_byte = BitConverter.GetBytes(height);
            byte[] file_size = BitConverter.GetBytes(width * height * 3);
            //on doit les convertir en bytes pour les mettre dans le header
            for (int i =0; i<4; i++)//4 bytes a mettre dans le header.
            {
                Header[i + 2] = file_size[i];
            }
            //On effectue une reduction AVEC REECHANTILLONAGE
            int index_i = 0;
            int index_j = 0;
            float coeff = (float)(1 / (ratio * 0.01));
            Pixel[,] new_matriceRGB = new Pixel[height, width];
            for (int i = 0; i < height; i++) //1er essai avec une reduction 1/4
            {
                for (int j = 0; j < width; j++)
                {
                    //on prend la moyenne de 4 pixels
                    index_i = (int)(i * coeff);
                    index_j = (int)(j * coeff);// pour une reduction 1/4
                    Pixel Nouveau_pi = new Pixel(0, 0, 0);
                    new_matriceRGB[i, j] = Nouveau_pi;
                    new_matriceRGB[i, j].Red = (MatriceRGB[index_i, index_j + 1].Red + MatriceRGB[index_i + 1, index_j].Red + MatriceRGB[index_i + 1, index_j + 1].Red) / 3;
                    new_matriceRGB[i, j].Green = (MatriceRGB[index_i, index_j + 1].Green + MatriceRGB[index_i + 1, index_j].Green + MatriceRGB[index_i + 1, index_j + 1].Green) / 3;
                    new_matriceRGB[i, j].Blue = (MatriceRGB[index_i, index_j + 1].Blue + MatriceRGB[index_i + 1, index_j].Blue + MatriceRGB[index_i + 1, index_j + 1].Blue) / 3;
                }
            }
            MatriceRGB = new_matriceRGB;
            Hauteur = height;
            Largeur = width;
            From_Image_To_File("Image_retrecie");
        }
        /// <summary>
        /// On agrandit une image d'un certain ratio 0-100%
        /// </summary>
        /// <param name="ratio"></param>
        public void Agrandir(double ratio)
        {
            //ratio = pourcentage de hauteur et de largeur que l'on garde
            double diminution_facteur = (ratio * 0.01);
            int width = (int)(Largeur * ratio);
            int height = (int)(Hauteur * ratio);
            Largeur_byte = BitConverter.GetBytes(width);
            Hauteur_byte = BitConverter.GetBytes(height);
            byte[] file_size = BitConverter.GetBytes(width * height * 3+54);
            //on doit les convertir en bytes pour les mettre dans le header
            for (int i = 0; i < 4; i++)//4 bytes a mettre dans le header.
            {
                Header[i + 2] = file_size[i];
            }
            int index_i = 0;
            int index_j = 0;
            float coeff = (float)(1 / (ratio * 0.01));
            Pixel[,] new_matriceRGB = new Pixel[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    index_i = (int)(i/ratio);
                    index_j = (int)(j/ratio);
                    Pixel Nouveau_pi = new Pixel(0, 0, 0);
                    new_matriceRGB[i, j] = Nouveau_pi;

                    new_matriceRGB[i,j].Red = MatriceRGB[index_i, index_j].Red;
                    new_matriceRGB[i, j].Green = MatriceRGB[index_i, index_j].Green;
                    new_matriceRGB[i, j].Blue = MatriceRGB[index_i, index_j].Blue;
                }
            }
            MatriceRGB = new_matriceRGB;
            Hauteur = height;
            Largeur = width; 
            TailleFichier = Hauteur * Largeur * 3+54+1000;
            From_Image_To_File("Image_agrandie");
        }
        /// <summary>
        /// retourne une image vu à travers un mirroir.
        /// </summary>
        public void Mirroir()
        {
            MyImage Mirroir = new MyImage(Hauteur, Largeur);
            Pixel[] tab = new Pixel[Largeur];
            int L = Largeur-1;
            Pixel[,] copie = MatriceRGB;
            //Echange de chaque pixel par symétrie avec le milieu de la photo
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    Mirroir.MatriceRGB[i, j] = MatriceRGB[i,Largeur-1-j];
                }
            }
            Mirroir.From_Image_To_File("Miroir");
        }
        /// <summary>
        /// A partir d'une matrice de pixel, d'une matrice de convolution et d'un diviseur effectue la matrice de convolution
        /// sur toute la matrice de pixel.
        /// </summary>
        /// <param name="matrice1"></param>
        /// <param name="matrice2"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public Pixel Calcul_convolution(Pixel[,] matrice1, int[,] matrice2, int divisor)
        {
            // Cette fonction calcule le resultat de l'application d'une matrice de convolution.
            Pixel Pixel_de_sortie = new Pixel(0, 0, 0);
            int result = 0;
            // for RED
            result +=matrice1[0, 0].Red * matrice2[0, 0] + matrice1[0, 1].Red * matrice2[0, 1] + matrice1[0, 2].Red * matrice2[0, 2];
            result += matrice1[1, 0].Red * matrice2[1, 0] + matrice1[1, 1].Red * matrice2[1, 1] + matrice1[1, 2].Red * matrice2[1, 2];
            result += matrice1[2, 0].Red * matrice2[2, 0] + matrice1[2, 1].Red * matrice2[2, 1] + matrice1[2, 2].Red * matrice2[2, 2];
            result = result / divisor;
            Pixel_de_sortie.Red = result;
            result = 0;

            //for GREEN
            result += matrice1[0, 0].Green * matrice2[0, 0] + matrice1[0, 1].Green * matrice2[0, 1] + matrice1[0, 2].Green * matrice2[0, 2];
            result += matrice1[1, 0].Green * matrice2[1, 0] + matrice1[1, 1].Green * matrice2[1, 1] + matrice1[1, 2].Green * matrice2[1, 2];
            result += matrice1[2, 0].Green * matrice2[2, 0] + matrice1[2, 1].Green * matrice2[2, 1] + matrice1[2, 2].Green * matrice2[2, 2];
            result = result / divisor;
            Pixel_de_sortie.Green = result;
            result = 0;

            // for BLUE
            result += matrice1[0, 0].Blue * matrice2[0, 0] + matrice1[0, 1].Blue * matrice2[0, 1] + matrice1[0, 2].Blue * matrice2[0, 2];
            result += matrice1[1, 0].Blue * matrice2[1, 0] + matrice1[1, 1].Blue * matrice2[1, 1] + matrice1[1, 2].Blue * matrice2[1, 2];
            result += matrice1[2, 0].Blue * matrice2[2, 0] + matrice1[2, 1].Blue * matrice2[2, 1] + matrice1[2, 2].Blue * matrice2[2, 2];
            result = result / divisor;
            Pixel_de_sortie.Blue = result;
            result = 0;

            return Pixel_de_sortie;
        }
        /// <summary>
        /// Effectue l'identité d'une certain matrice de pixel.
        /// </summary>
        public void Identité() 
        {
            Pixel[,] Matrice_pi = new Pixel[3, 3];
            int[,] Matrice_convolution = new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

            Pixel Bordure = new Pixel(0, 0, 0);
            for (int i = 0; i < Hauteur - 1; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    //condition sur i,j pour mettre le contour sur 0
                    if (i < 1 || j < 1 || i >= Hauteur - 1 || j >= Largeur - 1)
                    {

                        // pour les cotés de la matrice on les mets en noir. ==> ce ne sont pas les cotes de la photo.
                    }
                    else
                    {
                        Matrice_pi[0, 0] = MatriceRGB[i - 1, j - 1];
                        Matrice_pi[0, 1] = MatriceRGB[i - 1, j];
                        Matrice_pi[0, 2] = MatriceRGB[i - 1, j + 1];

                        Matrice_pi[1, 0] = MatriceRGB[i, j - 1];
                        Matrice_pi[1, 1] = MatriceRGB[i, j];
                        Matrice_pi[1, 2] = MatriceRGB[i, j + 1];

                        Matrice_pi[2, 0] = MatriceRGB[i + 1, j - 1];
                        Matrice_pi[2, 1] = MatriceRGB[i + 1, j];
                        Matrice_pi[2, 2] = MatriceRGB[i + 1, j + 1];
                        Pixel resultat = Calcul_convolution(Matrice_pi, Matrice_convolution,1);
                        MatriceRGB[i, j] = resultat;
                    }

                }
            }
            From_Image_To_File("Identite");
        }
        /// <summary>
        /// Effectue un flou de Gauss sur une certaine matrice de Pixel.
        /// </summary>
        public void Gauss_blur() //flou de gauss
        {
            Pixel[,] Matrice_pi = new Pixel[3, 3];
            int[,] Matrice_convolution = new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
            int divisor = 16;

            Pixel Bordure = new Pixel(0, 0, 0);
            for (int i = 0; i < Hauteur-1; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    //condition sur i,j pour mettre le contour sur 0
                    if (i < 1 || j < 1 || i >= Hauteur-1 || j >= Largeur-1 )
                    {

                       // pour les cotés de la matrice on les mets en noir. ==> ce ne sont pas les cotes de la photo.
                    }
                    else
                    {
                        Matrice_pi[0, 0] = MatriceRGB[i - 1, j - 1];
                        Matrice_pi[0, 1] = MatriceRGB[i - 1, j];
                        Matrice_pi[0, 2] = MatriceRGB[i - 1, j+1];

                        Matrice_pi[1, 0] = MatriceRGB[i , j-1];
                        Matrice_pi[1, 1] = MatriceRGB[i , j];
                        Matrice_pi[1, 2] = MatriceRGB[i, j+1];

                        Matrice_pi[2, 0] = MatriceRGB[i + 1, j-1];
                        Matrice_pi[2, 1] = MatriceRGB[i + 1, j];
                        Matrice_pi[2, 2] = MatriceRGB[i + 1, j+1];
                        Pixel resultat = Calcul_convolution(Matrice_pi, Matrice_convolution, divisor);
                        MatriceRGB[i, j] = resultat;
                    }
                    
                }
            }
            From_Image_To_File("Gauss_Blur");
        }
        /// <summary>
        /// Effectue la detection des contours sur une image
        /// </summary>
        public void Edge_detection() //Detection des contours
        {
            Pixel[,] Matrice_pi = new Pixel[3, 3];
            int[,] Matrice_convolution = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };

            Pixel Bordure = new Pixel(0, 0, 0);
            for (int i = 0; i < Hauteur ; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    //condition sur i,j pour mettre le contour sur 0
                    if (i < 1 || j < 1 || i >= Hauteur- 1 || j >= Largeur - 1)
                    {

                        MatriceRGB[i, j] = Bordure; // pour les cotés de la matrice on les mets en noir. ==> ce ne sont pas les cotes de la photo.
                    }
                    else
                    {
                        Matrice_pi[0, 0] = MatriceRGB[i - 1, j - 1];
                        Matrice_pi[0, 1] = MatriceRGB[i - 1, j];
                        Matrice_pi[0, 2] = MatriceRGB[i - 1, j + 1];

                        Matrice_pi[1, 0] = MatriceRGB[i, j - 1];
                        Matrice_pi[1, 1] = MatriceRGB[i, j];
                        Matrice_pi[1, 2] = MatriceRGB[i, j + 1];

                        Matrice_pi[2, 0] = MatriceRGB[i + 1, j - 1];
                        Matrice_pi[2, 1] = MatriceRGB[i + 1, j];
                        Matrice_pi[2, 2] = MatriceRGB[i + 1, j + 1];
                        Pixel resultat = Calcul_convolution(Matrice_pi, Matrice_convolution, 16); //diviseur de 16 pour cette matrice.
                        MatriceRGB[i, j] = resultat;
                    }

                }
            }
            From_Image_To_File("Edge_detection");
        }
        /// <summary>
        /// renforces les bords et contours d'une image
        /// </summary>
        public void Renforcement_bord() 
        {
            Pixel[,] Matrice_pi = new Pixel[3, 3];
            int[,] Matrice_convolution = new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

            Pixel Bordure = new Pixel(0, 0, 0);
            for (int i = 0; i < Hauteur - 1; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    //condition sur i,j pour mettre le contour sur 0
                    if (i < 1 || j < 1 || i >= Hauteur - 1 || j >= Largeur - 1)
                    {
                        MatriceRGB[i, j].Red = MatriceRGB[i, j].Red / 2;
                        MatriceRGB[i, j].Green = MatriceRGB[i, j].Green / 2;
                        MatriceRGB[i, j].Blue = MatriceRGB[i, j].Blue / 2;
                        // pour les cotés de la matrice on les mets en noir. ==> ce ne sont pas les cotes de la photo.
                    }
                    else
                    {
                        Matrice_pi[0, 0] = MatriceRGB[i - 1, j - 1];
                        Matrice_pi[0, 1] = MatriceRGB[i - 1, j];
                        Matrice_pi[0, 2] = MatriceRGB[i - 1, j + 1];

                        Matrice_pi[1, 0] = MatriceRGB[i, j - 1];
                        Matrice_pi[1, 1] = MatriceRGB[i, j];
                        Matrice_pi[1, 2] = MatriceRGB[i, j + 1];

                        Matrice_pi[2, 0] = MatriceRGB[i + 1, j - 1];
                        Matrice_pi[2, 1] = MatriceRGB[i + 1, j];
                        Matrice_pi[2, 2] = MatriceRGB[i + 1, j + 1];
                        Pixel resultat = Calcul_convolution(Matrice_pi, Matrice_convolution, 5);
                        MatriceRGB[i, j] = resultat;
                    }

                }
            }
            From_Image_To_File("Renforcement_bords");
        }
        /// <summary>
        /// effectue un repoussage sur une image
        /// </summary>
        public void Repoussage()
        {
            Pixel[,] Matrice_pi = new Pixel[3, 3];
            int[,] Matrice_convolution = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } }; //matrice de conv pour repoussage

            Pixel Bordure = new Pixel(0, 0, 0);
            for (int i = 0; i < Hauteur - 1; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    //condition sur i,j pour mettre le contour sur 0
                    if (i < 1 || j < 1 || i >= Hauteur - 1 || j >= Largeur - 1)
                    {

                        // pour les cotés de la matrice on les mets en noir. ==> ce ne sont pas les cotes de la photo.
                    }
                    else
                    {
                        Matrice_pi[0, 0] = MatriceRGB[i - 1, j - 1];
                        Matrice_pi[0, 1] = MatriceRGB[i - 1, j];
                        Matrice_pi[0, 2] = MatriceRGB[i - 1, j + 1];

                        Matrice_pi[1, 0] = MatriceRGB[i, j - 1];
                        Matrice_pi[1, 1] = MatriceRGB[i, j];
                        Matrice_pi[1, 2] = MatriceRGB[i, j + 1];

                        Matrice_pi[2, 0] = MatriceRGB[i + 1, j - 1];
                        Matrice_pi[2, 1] = MatriceRGB[i + 1, j];
                        Matrice_pi[2, 2] = MatriceRGB[i + 1, j + 1];
                        Pixel resultat = Calcul_convolution(Matrice_pi, Matrice_convolution, 2); //je prends le maximum de la matrice
                        MatriceRGB[i, j] = resultat;
                    }

                }
            }
            From_Image_To_File("Renforcement_bords");
        }
        /// <summary>
        /// Renvoit un histogramme de couleurs RGB d'une image.
        /// </summary>
        public void Histogramme()
        {
            MyImage output = new MyImage(256, 256*3);
            int[] hist_R = new int[256];
            int[] hist_G = new int[256];
            int[] hist_B = new int[256];
            for (int i = 0; i < 256; i++)
            {
                hist_R[i] = 0;
                hist_G[i] = 0;
                hist_B[i] = 0;
            }
            for (int i = 0; i <Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    hist_R[MatriceRGB[i, j].Red] += 1;
                    hist_G[MatriceRGB[i, j].Green] += 1;
                    hist_B[MatriceRGB[i, j].Blue] += 1;
                }
            }

            for (int j = 0; j <256; j++)
            {
                for (int i = 0; i < hist_R[j]/10-2; i++)
                {
                    Pixel rouge = new Pixel(0, 0, 255);
                    output.MatriceRGB[i,j]=rouge;
                }
            }
            for (int j = 256; j < 256*2; j++)
            {
                for (int i = 0; i < hist_B[j-256] / 10 - 2; i++)
                {
                    Pixel bleu = new Pixel(150, 0, 0);
                    output.MatriceRGB[i, j] = bleu;
                }
            }
            for (int j = 256*2; j < output.Largeur; j++)
            {
                for (int i = 0; i < hist_G[j-2*256] / 10 - 2; i++)
                {
                    Pixel vert = new Pixel(0, 100, 0);
                    output.MatriceRGB[i, j] = vert;
                }
            }
            output.Nom_fichier = "hist";
            output.From_Image_To_File("hist");
            //output.Show();
        }
        /// <summary>
        /// Renvoit la fractale de Mandelbrot
        /// </summary>
        public void Fractale()
        {
            double ite = 50;
            double c_x= 0;
            double c_y = 0;
            double z_x = 0;
            double z_y = 0;
            double y = 0;
            MyImage fractale = new MyImage(120*4, 120*4);
            for (int i = 0; i < fractale.Hauteur; i++)
            {
                for (int j = 0; j < fractale.Largeur; j++)
                {
                    c_x = (2.8 / fractale.Hauteur) * i - 2.1;
                    c_y = (2.4 / fractale.Largeur) * j - 1.2;
                    z_x = 0;
                    z_y = 0;
                    y = 0;
                    do
                    {
                        double tmp = z_x;
                        z_x = z_x * z_x -  z_y*z_y+ c_x;
                        z_y = 2 * z_y *tmp+ c_y;
                        y += 1;
                    } while (z_x * z_x + z_y * z_y < 4 & y < ite);
                    if(y==ite)
                    {
                        Pixel blanc = new Pixel(255, 255, 255);
                        fractale.MatriceRGB[i, j] = blanc;
                    }
                    else
                    {
                        Pixel vert = new Pixel(0, (int)(y * 255 / ite), 0);
                        fractale.MatriceRGB[i, j]=vert;
                    }
                }
            }
            fractale.From_Image_To_File("fractale");
        }
        /// <summary>
        /// Convertit un texte en binaire.
        /// </summary>
        /// <param name="texte"></param>
        /// <returns></returns>
        public string string_to_int(string texte)
        {
            //fonction utile pour encodage
            string result = "";
            foreach(char item in texte)
            {
                //on converti en binaire donc parametre=2
                //on rempli de 0 a gauche avec padleft pour une taille de 8 au total
                Encoding.ASCII.GetBytes("texte");
                result += Convert.ToString(item, 2).PadLeft(8, '0');
            }
            return result;
        }
        /// <summary>
        /// fonction d'essaie autre que l'encodage photo
        /// </summary>
        /// <param name="texte"></param>
        public void Encodage(string texte)
        {
            //fonction pour encoder un mot en plain text dans une image
            //converti le mot en binaire
            int index_binaire = 0;
            string binaire = string_to_int(texte);
            Console.WriteLine("le mot texte en binaire (codage) : "+binaire);
            binaire+= "00000000";//on rajoute 8 zero pour marquer la fin du codage
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur-2; j++)
                {
                    index_binaire = i * Largeur + j * 3;
                    //tous les derniers termes sont sur 0
                    MatriceRGB[i, j].Red = MatriceRGB[i, j].Red - MatriceRGB[i, j].Red % 2;
                    MatriceRGB[i, j].Green = MatriceRGB[i, j].Green - MatriceRGB[i, j].Green % 2;
                    MatriceRGB[i, j].Blue = MatriceRGB[i, j].Blue - MatriceRGB[i, j].Blue % 2;
                    //tous terminent en 0 donc on peut voir le 1 ou pas
                    //on peut ajouter les termes en binaires du mot qu'on veut
                    if (index_binaire >= binaire.Length)
                    {
                        //on a atteint la fin
                        break;
                    }
                    MatriceRGB[i, j].Red =MatriceRGB[i,j].Red + Convert.ToInt32(Convert.ToString(binaire[index_binaire]));
                    if (index_binaire+1 >= binaire.Length)
                    {
                        //on a atteint la fin
                        break;
                    }
                    MatriceRGB[i, j].Green += Convert.ToInt32(Convert.ToString(binaire[index_binaire+1]));
                    if (index_binaire+2 >= binaire.Length)
                    {
                        //on a atteint la fin
                        break;
                    }
                    MatriceRGB[i, j].Blue += Convert.ToInt32(Convert.ToString(binaire[index_binaire+2]));
                }
                if (index_binaire >= binaire.Length)
                {
                    //on a atteint la fin
                    break;
                }
            }
        }
        /// <summary>
        /// renvoit une suite de bytes pour un format binaire en entrée 
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public string int_to_string(string binary)
        {
            //fonction utile pour le décodage de l'image
            List<Byte> format_byte = new List<Byte>();
            string result = "";
            for (int i = 0; i < binary.Length-8; i+=8)
            {
                //on converti chaque parcelle de 8 caracteres en un byte que l'on converti en string
                //je n'ai pas trouvé de moyen plus court
                format_byte.Add(Convert.ToByte(binary.Substring(i,8),2));
            }
            result = Encoding.Default.GetString(format_byte.ToArray());
            return result;
        }
        /// <summary>
        /// Fonction qui verifie la condition d'arret de 8 '0' de suite
        /// </summary>
        /// <param name="binaire"></param>
        /// <returns></returns>
        public bool Zero_compteur(string binaire)
        {
            //fonction qui vérifie la condition d'arret (qui est 8 zeros)
            if(binaire.Length<8)
            {
                return false;
            }
            else
            {
                int compte = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (binaire[binaire.Length - 1 - i] == 0)
                    {
                        compte += 1;
                    }
                }
                if (compte >= 8)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Decodage()
        {
            //on décode la photo
            bool arret = false;
            string binaire = "";
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    binaire += MatriceRGB[i, j].Red % 2;
                    binaire += MatriceRGB[i, j].Green % 2;
                    binaire += MatriceRGB[i, j].Blue % 2;
                    arret = Zero_compteur(binaire);
                    if(binaire.Contains("00000000")) // condition d'arret
                    {
                        arret = true;
                    }
                    if(arret==true)
                    {
                        break;
                    }
                }
                if(arret==true)
                {
                    break;
                }
            }
            string result = "";
            for (int i = 0; i < binaire.Length-8; i++)
            {
                result = binaire;
            }
            string texte = int_to_string(binaire);
            Console.WriteLine("Texte trouvé! : "+texte);
        }
        /// <summary>
        /// Prends les 4 premiers termes (poids lourd) d'une conversion de byte en binaire
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string int_to_8bit(int input)
        {
            //return a string of 8-bit binary
            string output = Convert.ToString(input, 2).PadLeft(8, '0');
            string result = "";
            for (int i = 0; i < 4; i++)
            {
                result += output[i];
            }
            return result;
        }
        /// <summary>
        /// prends les 8 termes d'une conversion de byte en binaire.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string int_to_8bit_tot(int input)
        {
            //return a string of 8-bit binary
            string output = Convert.ToString(input, 2).PadLeft(8,'0');
            return output;
        }
        /// <summary>
        /// Convertit une serie binaire de 8 bits au format string en un byte au format entier.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int bit_to_int(string input)
        {
            int result = Convert.ToInt32(input, 2);
            return result;
        }
        /// <summary>
        /// Fonction qui encode une image dans une autre.
        /// </summary>
        /// <param name="image2"></param>
        public void Encodage_photo(MyImage image2)
        {
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    if (i<image2.Hauteur & j<image2.Largeur)
                    {
                        var im_1_red = int_to_8bit(MatriceRGB[i, j].Red);
                        var im_2_red = int_to_8bit(image2.MatriceRGB[i, j].Red);//recupere le poid lourd de chaque
                        var tot_R = im_1_red + im_2_red; // on rajoute les 4 premiers termes de chaque.
                        MatriceRGB[i, j].Red = bit_to_int(tot_R);

                        var im_1_green = int_to_8bit(MatriceRGB[i, j].Green);
                        var im_2_green = int_to_8bit(image2.MatriceRGB[i, j].Green);
                        var tot_G = im_1_green + im_2_green;
                        MatriceRGB[i, j].Green = bit_to_int(tot_G);

                        var im_1_blue = int_to_8bit(MatriceRGB[i, j].Blue);
                        var im_2_blue = int_to_8bit(image2.MatriceRGB[i, j].Blue);
                        var tot_B = im_1_blue + im_2_blue;
                        MatriceRGB[i, j].Blue = bit_to_int(tot_B);
                    }
                    if (j==image2.Largeur & i==image2.Hauteur) //pour faire signe au decodage que j'ai finit d'encoder
                        //je place ce pixel aux valeur 11, 12, 13
                    {
                        MatriceRGB[i, j].Red = 11;
                        MatriceRGB[i, j].Green = 12;
                        MatriceRGB[i, j].Blue = 13;
                    }
                }
            }
            From_Image_To_File("Image_codée");
        }
        /// <summary>
        /// Decode une image qui était cachée dans une autre.
        /// </summary>
        public void Decodage_photo()
        {
            bool stop = false;
            int haut = 0;
            int larg = 0;
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    if (MatriceRGB[i, j].Red == 11 & MatriceRGB[i, j].Green == 12 & MatriceRGB[i, j].Blue == 13)
                    { //ma condition d'arret pour savoir qu'on a finit de dechiffre une image est que les valeurs 11,12,13 soient
                        //prises par un pixel.
                        haut = i;
                        larg = j;
                        stop = true;
                        break;
                    }
                }
                if (stop == true)
                {
                    break;
                }
            }
            MyImage im2 = new MyImage(haut, larg);
            for (int i = 0; i < haut; i++)
            {
                for (int j = 0; j < larg; j++)
                {
                    
                    var binaire_R = int_to_8bit_tot(MatriceRGB[i, j].Red);
                    var binaire_G = int_to_8bit_tot(MatriceRGB[i, j].Green);
                    var binaire_B = int_to_8bit_tot(MatriceRGB[i, j].Blue);
                    var im_1_R = binaire_R.Substring(3) + "0000";           //recupere les deux poids lourds differents
                    var im_2_R = binaire_R.Substring(4, 4) + "0000";         // rajoute 4 '0' et assigne a une nouvelle image
                    MatriceRGB[i, j].Red = bit_to_int(im_1_R);
                    im2.MatriceRGB[i, j].Red = bit_to_int(im_2_R);

                    var im_1_G = binaire_G.Substring(3) + "0000";
                    var im_2_G = binaire_G.Substring(4, 4) + "0000";
                    MatriceRGB[i, j].Green = bit_to_int(im_1_G);
                    im2.MatriceRGB[i, j].Green = bit_to_int(im_2_G);

                    var im_1_B = binaire_B.Substring(3) + "0000";
                    var im_2_B = binaire_B.Substring(4, 4) + "0000";
                    MatriceRGB[i, j].Blue = bit_to_int(im_1_B);
                    im2.MatriceRGB[i, j].Blue = bit_to_int(im_2_B);
                }
            }
            im2.From_Image_To_File("Image_decodée");
        }
    }
}
