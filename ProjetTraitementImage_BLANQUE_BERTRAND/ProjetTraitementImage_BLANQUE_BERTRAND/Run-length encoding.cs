using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetTraitementImage_BLANQUE_BERTRAND
{
    /// <summary>
    /// Classe qui encode une image en noir et blanc en un tableau (string[]) du format RLE.
    /// </summary>
    class Run_length_encoding
    {
        public string[] donnees { get; set; }
        public int Hauteur { get; set; }
        public int Largeur { get; set; }
        public string[] donnees_binaire { get; set; }
        /// <summary>
        /// Constructeur qui pour une image donné va crée un objet de la classe RLE. Lit toutes la matrice et la convertit en
        /// nnombre de fois noir de suite puis nombre de fois blanc de suite.
        /// </summary>
        /// <param name="UneImage"></param>
        public Run_length_encoding(MyImage UneImage)
        {
            Hauteur = UneImage.Hauteur;
            Largeur = UneImage.Largeur;
            int count = 0;
            char state = 'C';
            int index = 0;
            string[] donnees = new string[UneImage.Hauteur*UneImage.Largeur];
            for (int i = 0; i < UneImage.Hauteur; i++)
            {
                for (int j = 0; j < UneImage.Largeur; j++)
                {
                    if (i == UneImage.Hauteur - 1 & j == UneImage.Largeur - 1)
                    {
                        donnees[index] = count + "";
                        donnees[index] += state;
                    }
                    else
                    {
                        if (UneImage.MatriceRGB[i, j].Red == 255 & UneImage.MatriceRGB[i, j].Blue == 255 & UneImage.MatriceRGB[i, j].Green == 255)
                        {
                            if (state == 'B')
                            {
                                count += 1;
                            }
                            else if (state == 'C')
                            {
                                state = 'B';
                                count = 1;
                            }
                            else
                            {
                                donnees[index] = count + "N";
                                index += 1;
                                state = 'B';
                                count = 1;
                            }
                        }
                        if (UneImage.MatriceRGB[i, j].Red == 0 & UneImage.MatriceRGB[i, j].Blue == 0 & UneImage.MatriceRGB[i, j].Green == 0)
                        {
                            if (state == 'N')
                            {
                                count += 1;
                            }
                            else if (state == 'C')
                            {
                                state = 'N';
                                count = 1;
                            }
                            else
                            {
                                donnees[index] = count + "B";
                                index += 1;
                                state = 'N';
                                count = 1;
                            }
                        }
                    }
                }
            }
            int nbre = 0;
            for (int i = 0; i < donnees.Length; i++)
            {
                if (donnees[i]!=null)
                {
                    nbre += 1;
                }
            }
            string[] donnees2 = new string[nbre];
            for (int i = 0; i < nbre; i++)
            {
                donnees2[i] = donnees[i];
            }
            for (int i = 0; i < donnees2.Length; i++)
            {
                Console.Write(donnees2[i]+" ");
            }
            this.donnees = donnees2;
        }
        /// <summary>
        /// Fonction permettant de decoder un objet de la classe RLE. Il prend le tableau de de nombre de fois noir, nombre de fois blanc
        /// de suite et en deduit la matrice.
        /// </summary>
        public void Decode()
        {
            Pixel noir = new Pixel(0,0,0);
            Pixel blanc = new Pixel(255, 255, 255);
            MyImage RLE = new MyImage(Hauteur, Largeur);
            int index = 0;
            int ite = 0;
            char state = 'C';
            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    if (index<donnees.Length)
                    {
                        if (ite == 0)
                        {
                            if (donnees[index].Contains('B'))
                            {
                                ite = Convert.ToInt32(donnees[index].Substring(0, donnees[index].Length - 1));
                                index += 1;
                                state = 'B';
                            }
                            else if (donnees[index].Contains('N'))
                            {
                                ite = Convert.ToInt32(donnees[index].Substring(0, donnees[index].Length - 1));
                                index += 1;
                                state = 'N';
                            }
                        }
                        else
                        {
                            if (state == 'B')
                            {
                                RLE.MatriceRGB[i, j] = blanc;
                                ite -= 1;
                            }
                            if (state == 'N')
                            {
                                RLE.MatriceRGB[i, j] = noir;
                                ite -= 1;
                            }
                        }
                    }
                }
            }
            RLE.Nom_fichier = "decoded_with_RLE.bmp";
            RLE.From_Image_To_File("decoded_with_RLE");
        }
        /// <summary>
        /// permet de coder une matrice de pixels couleur au format que j'ai imaginé
        /// </summary>
        public Run_length_encoding(MyImage UneImageCouleur, int constrcuteur2)
        {
            Hauteur = UneImageCouleur.Hauteur;
            Largeur = UneImageCouleur.Largeur;
            string donnees = "";
            int index = 0;
            for (int i = 0; i < Hauteur; i++)
            {
                Console.Clear();
                Console.WriteLine(i+"/"+Hauteur+"%");
                for (int j = 0; j < Largeur; j++)
                {
                    donnees += UneImageCouleur.int_to_8bit_tot(UneImageCouleur.MatriceRGB[i, j].Red);
                    donnees += UneImageCouleur.int_to_8bit_tot(UneImageCouleur.MatriceRGB[i, j].Green);
                    donnees += UneImageCouleur.int_to_8bit_tot(UneImageCouleur.MatriceRGB[i, j].Blue);
                    //Console.Write(UneImageCouleur.int_to_8bit_tot(UneImageCouleur.MatriceRGB[i, j].Blue));
                }
            }
            char state = 'C';
            int count = 0;
            string[] donnees_binaire = new string[Hauteur*Largeur*3*8];
            for (int i = 0; i < donnees.Length; i++)
            {
                if (i == donnees.Length-1)
                {
                    donnees_binaire[index] = count + "";
                    donnees_binaire[index] += state;
                }
                else
                {
                    if (donnees[i]=='0')
                    {
                        if (state == 'B')
                        {
                            count += 1;
                        }
                        else if (state == 'C')
                        {
                            state = 'B';
                            count = 1;
                        }
                        else
                        {
                            donnees_binaire[index] = count + "N";
                            index += 1;
                            state = 'B';
                            count = 1;
                        }
                    }
                    if (donnees[i]=='1')
                    {
                        if (state == 'N')
                        {
                            count += 1;
                        }
                        else if (state == 'C')
                        {
                            state = 'N';
                            count = 1;
                        }
                        else
                        {
                            donnees_binaire[index] = count + "B";
                            index += 1;
                            state = 'N';
                            count = 1;
                        }
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Données en RLE:");
            for (int i = 0; i < donnees_binaire.Length; i++)
            {
                if (donnees_binaire[i]!=null)
                {
                    Console.Write(donnees_binaire[i] + " ");
                }
            }
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
        /// passe d'un byte a un binaire 8 bits
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string int_to_8bit_tot(int input)
        {
            //return a string of 8-bit binary
            string output = Convert.ToString(input, 2).PadLeft(8, '0');
            return output;
        }
    }
}
