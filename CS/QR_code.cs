using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetTraitementImage_BLANQUE_BERTRAND
{
    /// <summary>
    /// Classe QR code, cette classe convertit un texte en une image au format QR code. Cette classe n'a besoin que du texte et peut
    /// en déduire tout ce qui suit.
    /// </summary>
    public class QR_code
    {
        public MyImage Image { get; set; }
        public string Version { get; set; }
        public string Mode { get; set; }
        public string CountIndicator { get; set; }
        public string EncodedData { get; set; }
        public int Required_number { get; set; }
        public int EccCount { get; set; }
        public string masque { get; set; }
        public string contenu { get; set; }
        public string Coder_texte(string texte)
        {
            string result = "";
            if(texte.Length%2==0)
            {
                for (int i = 0; i < texte.Length; i += 2)
                {
                    result += Convert.ToString(alpha(texte[i])*45+alpha(texte[i+1]),2).PadLeft(11,'0');
                }
            }
            else
            {
                for (int i = 0; i < texte.Length-1; i += 2)
                {
                    result += Convert.ToString(alpha(texte[i]) * 45 + alpha(texte[i + 1]), 2).PadLeft(11, '0');
                }
                result += Convert.ToString(alpha(texte[texte.Length - 1]),2).PadLeft(6, '0');
            }
            return result;
        }
        /// <summary>
        /// Alpha retourne le nombre correspondant en alphanumérique de la lettre en entrée.
        /// </summary>
        /// <param name="lettre"></param>
        /// <returns></returns>
        public int alpha(char lettre)
        {
            int result = 0;
            lettre=lettre.ToString().ToLower()[0];
            string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz ";
            for (int j = 0; j < 36; j++)
            {
                if (lettre == alphabet[j])
                {
                    result += j;
                }
            }
            return result;
        }
        /// <summary>
        /// Le constructeur prend le texte, instancit une image, crée les motifs, ecrit le masque et remplit le reste avec les 
        /// données et les données de correction qu'il aura calculé.
        /// </summary>
        /// <param name="texte"></param>
        public QR_code(string texte)
        {
            contenu = texte; 
            Mode = "0010";
            Console.WriteLine("Mode:0010");
            Console.WriteLine();
            EncodedData = Mode;
            if(texte.Length>16) // en fonction de la taille du texte instancit une image de la bonne taille.
            {
                Version = "2";
                EccCount = 10;
                Image = new MyImage(25, 25);
            }
            else
            {
                Version = "1";
                EccCount = 7;
                Image = new MyImage(21, 21+3);
            }
            CountIndicator = "";
            for (int k = 0; k < DecToBinary(texte.Length).Length; k++) //ajoute la longueur du texte en binaire
            {
                CountIndicator+= DecToBinary(texte.Length)[DecToBinary(texte.Length).Length -1- k];
            }
            CountIndicator = CountIndicator.PadLeft(9,'0'); //le count indicator sur 9 bits
            Console.WriteLine("CountIndicator:"+CountIndicator);
            Console.WriteLine();
            Console.WriteLine("Texte codé: "+Coder_texte(texte));
            Console.WriteLine();
            EncodedData += CountIndicator;
            EncodedData += Coder_texte(texte); //texte --> alphanumérique --> binaire a travers cette fonction
            if (Version=="1")
            {
                Required_number = 19*8; //1-L
            }
            if (Version == "2")
            {
                Required_number = 34*8;//2-L
            }
            Console.Write("'0' rajouté:");
            if (Required_number - EncodedData.Length > 0) //on rajoute jusqu'a 4 '0'
            {
                for (int m = 0; m < Required_number - EncodedData.Length & m<4; m++)
                {
                    Console.Write("0");
                    EncodedData += "0";
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("0 rajouté pour multiple de 8: ");
            if (EncodedData.Length%8!=0) //il faut que ca soit un multiple de 8
            {
                for (int m = 0; m < EncodedData.Length % 8; m++)
                {
                    Console.Write("0");
                    EncodedData += "0";
                }
            }
            string fill = "1110110000010001";
            int i = 0;
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Binaire répété pour remplir :");
            do // on fill avec 
            {
                if (i == 16) { i = 0;}
                EncodedData += fill[i];
                Console.Write(fill[i]);
                i++;
            } while (EncodedData.Length < Required_number);
            byte[] texte_b = new byte[(int)(EncodedData.Length/8)];
            for (int l = 0; l < EncodedData.Length/8; l+=1)
            {
                string r = "" + EncodedData[0 + 8 * l];
                r += EncodedData[1 + 8 * l];
                r += EncodedData[2 + 8 * l];
                r += EncodedData[3 + 8 * l];
                r += EncodedData[4 + 8 * l];
                r += EncodedData[5 + 8 * l];
                r += EncodedData[6 + 8 * l];
                r += EncodedData[7 + 8 * l];
                texte_b[l] = Convert.ToByte(Convert.ToInt32(r, 2));
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("correction:");
            //encode
            Encoding u8 = Encoding.UTF8; //partie ou j'essaye de generer la correction
            string a = contenu;
            int iBC = u8.GetByteCount(a);
            byte[] bytesa = u8.GetBytes(a);
            byte[] result = ReedSolomon.ReedSolomonAlgorithm.Encode(bytesa, 7, ReedSolomon.ErrorCorrectionCodeType.QRCode);
            foreach (byte val in result) Console.Write(val + " ");
            //encode
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("correction rajoutée:");
            for (int l = 0; l < result.Length; l++)
            {
                Console.Write(Image.int_to_8bit_tot(result[l]));
                EncodedData += Image.int_to_8bit_tot(result[l]);
            }
            Console.WriteLine();
            //création du QRcode
            for (int y = 0; y < Image.Hauteur; y++) //ce code est très long et difficile a comprendre mais genere les motifs du QR code
            {
                for (int j = 0; j < Image.Largeur; j++)
                {
                    Pixel noir2 = new Pixel(0, 0, 0);
                    Pixel blanc2 = new Pixel(255, 255, 255);
                    if(((y==0 || y==6 ) &(j<7 ||j>13)))//horizontal 2 en haut
                    {
                        Image.MatriceRGB[y, j] = noir2;
                    }
                    if (((j == 0 || j == 6 ) & ((y < 7 || y > 13))))//vert 2 en colonne
                    {
                        Image.MatriceRGB[y, j] = noir2;
                    }
                    if ((j == 14 || j == 20) & (y<7))
                    {
                        Image.MatriceRGB[y, j] = noir2;
                    }
                    if ((y == 14 || y == 20) & (j<7))
                    {
                        Image.MatriceRGB[y, j] = noir2;
                    }
                    if(((y>1&y<5)&(y>1&j<5)) ||((y<19&y>15)&(j>1&j<5))||((y < 5 & y > 1) & (j > 15 & j < 19)))
                    {
                        Image.MatriceRGB[y, j] = noir2;
                    }
                    if((y==6)&(j<14&j>6))
                    {
                        if(j%2==1)
                        {
                            Image.MatriceRGB[y, j] = blanc2;//blanc
                        }
                        else
                        {
                            Image.MatriceRGB[y, j] = noir2;//noir
                        }
                    }
                    if ((j == 6) & (y < 14 & y > 6))
                    {
                        if (y % 2 == 1)
                        {
                            Image.MatriceRGB[y, j] = blanc2;//blanc
                        }
                        else
                        {
                            Image.MatriceRGB[y, j] = noir2;//noir
                        }
                    }
                    //lignes bleues
                    if (j == 8 & (y < 9 || y > 12))
                    {
                        Image.MatriceRGB[y, j] = new Pixel(255, 0, 0);
                    }
                    if (y == 13 & (j < 9))
                    {
                        Image.MatriceRGB[y, j] = new Pixel(255, 0, 0);
                    }
                    if (y == 8 & (j < 9 || j > 12))
                    {
                        Image.MatriceRGB[y, j] = new Pixel(255, 0, 0);
                    }
                    Image.MatriceRGB[8, 13] = noir2;
                    //lignes bleues
                    if (((y==1) || (y==5))&((j>0&j<6) ||(j<20&j>14)))
                    {
                        Image.MatriceRGB[y, j] = blanc2;//b
                    }
                    if (((j == 1) || (j == 5)) & ((y > 0 & y < 6) || (y < 20 & y > 14)))
                    {
                        Image.MatriceRGB[y, j] = blanc2;//b
                    }
                    if ((j==15 ||j==19) &(y>0&y<6))
                    {
                        Image.MatriceRGB[y, j] = blanc2;//b
                    }
                    if ((y == 15 || y == 19) & (j > 0 & j < 6))
                    {
                        Image.MatriceRGB[y, j] = blanc2;//b
                    }
                    if (y == 7 & (j < 8 || j > 13))
                    {
                        Image.MatriceRGB[y, j] = blanc2;
                    }
                    if (j == 7 & (y < 8 || y > 13))
                    {
                        Image.MatriceRGB[y, j] = blanc2;
                    }
                    if(y==13&(j<8))
                    {
                        Image.MatriceRGB[y, j] = blanc2;
                    }
                    if (j == 13 & (y < 8))
                    {
                        Image.MatriceRGB[y, j] = blanc2;
                    }
                    if (j > 20)
                    {
                        Image.MatriceRGB[y, j] = new Pixel(128, 128, 128);
                    }
                    
                }
                
            }
            masque= "111011111000100";
            //on encode le masque
            for (int l = 0; l < 7; l++)
            {
                if (masque[l]=='0')
                {
                    Image.MatriceRGB[8, 20-l] = new Pixel(0, 0, 0);
                }
                if (masque[l] == '1')
                {
                    Image.MatriceRGB[8,20- l] = new Pixel(255, 255, 255);
                }
            }
            for (int l = 0; l < 7; l++)
            {
                if (l==6)
                {
                    l += 1;
                }
                if (masque[l] == '0')
                {
                    Image.MatriceRGB[l, 8] = new Pixel(0, 0, 0);
                }
                if (masque[l] == '1')
                {
                    Image.MatriceRGB[l, 8] = new Pixel(255, 255, 255);
                }
            }
            for (int l = 0; l <8; l++)
            {
                if (l==1)
                {

                }
                if (l > 1)
                {
                    if (masque[l + 6] == '0')
                    {
                        Image.MatriceRGB[8, 7 - l] = new Pixel(0, 0, 0);
                    }
                    if (masque[l + 6] == '1')
                    {
                        Image.MatriceRGB[8, 7 - l] = new Pixel(255, 255, 255);
                    }
                }
                else
                {
                    if (masque[l + 7] == '0')
                    {
                        Image.MatriceRGB[8, 8 - l] = new Pixel(0, 0, 0);
                    }
                    if (masque[l + 7] == '1')
                    {
                        Image.MatriceRGB[8, 8 - l] = new Pixel(255, 255, 255);
                    }
                }
            }
            for (int l = 0; l < 8; l++)
            {
                if (masque[7 + l] == '0')
                {
                    Image.MatriceRGB[l + 13, 8] = new Pixel(0, 0, 0);
                }
                if (masque[7 + l] == '1')
                {
                    Image.MatriceRGB[l + 13, 8] = new Pixel(255, 255, 255);
                }
            }
            Image.MatriceRGB[8,6] = new Pixel(0, 0, 0);
            Image.MatriceRGB[6, 8] = new Pixel(0, 0, 0);
            int index = 0;
            int colonne = 0;
            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);
            //remplissage du qr
            for (int m = 20; m > 0; m = m - 2) // on rentre les données apres fait motifs et masque
            {
                if (m==6)
                {
                    m -= 1;
                }
                if (colonne % 2 == 0) //une collonne sur deux a un sens particulier
                {
                    for (int l = 20; l > -1; l--)
                    {
                        if (Image.MatriceRGB[m, l].Red == 1)
                        {
                            if (EncodedData[index] == '1')
                            {
                                Image.MatriceRGB[m, l] = noir;
                            }
                            if (EncodedData[index] == '0')
                            {
                                Image.MatriceRGB[m, l] = blanc;
                            }
                            if (EncodedData[index + 1] == '1')
                            {
                                Image.MatriceRGB[m - 1, l] = noir;
                            }
                            if (EncodedData[index + 1] == '0')
                            {
                                Image.MatriceRGB[m - 1, l] = blanc;
                            }
                            index += 2;
                        }
                    }

                    colonne += 1;
                }
                else
                {
                    for (int l = 0; l < 21; l++)
                    {
                        if (Image.MatriceRGB[m, l].Red == 1)
                        {
                            if (EncodedData[index] == '1')
                            {
                                Image.MatriceRGB[m, l] = noir;
                            }
                            if (EncodedData[index] == '0')
                            {
                                Image.MatriceRGB[m, l] = blanc;
                            }
                            if (EncodedData[index + 1] == '1')
                            {
                                Image.MatriceRGB[m - 1, l] = noir;
                            }
                            if (EncodedData[index + 1] == '0')
                            {
                                Image.MatriceRGB[m - 1, l] = blanc;
                            }
                            index += 2;
                        }
                    }
                    colonne += 1;
                }

            }
            Console.WriteLine(index + "index");
            Image.From_Image_To_File("QR_code");
            Image.Nom_fichier = "QR_code.bmp";
            Image.Agrandir(7);
        }
        /// <summary>
        /// Transforme un entier decimal en binaire 8bits.
        /// </summary>
        /// <param name="nbre"></param>
        /// <returns></returns>
        public string DecToBinary(int nbre) 
        {
            string result = "";
            while(nbre>0)
            {
                result += nbre % 2;
                nbre = nbre / 2;
            }
            return result;
        }
    }
}
