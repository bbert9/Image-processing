using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;

namespace ProjetTraitementImage_BLANQUE_BERTRAND
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            //Directory.SetCurrentDirectory(@"\\Mac\Home\Desktop\COURS\informatique\ProjetTraitementImage_BLANQUE_BERTRAND\ProjetTraitementImage_BLANQUE_BERTRAND\bin\Debug");
            
            string input = "";
            do
            {
                Console.Clear();
                Console.WriteLine("Bienvenue dans ce programme de traitement d'image");
                Console.WriteLine("Toutes les fonctions proposées danc ce main fonctionne, les voici:");
                Console.WriteLine();
                Console.WriteLine("Veuillez choisir une opération a faire");
                Console.WriteLine("1: agrandir une image");
                Console.WriteLine("2: retrecir une image");
                Console.WriteLine("3: passage en nuance de gris");
                Console.WriteLine("4: passage en noir et blanc");
                Console.WriteLine("5: rotation d'une image");
                Console.WriteLine("6: effet mirroir");
                Console.WriteLine("7: detection de contour");
                Console.WriteLine("8: renforcement des bords");
                Console.WriteLine("9: flou de gauss");
                Console.WriteLine("10: repoussage");
                Console.WriteLine("11: fractale");
                Console.WriteLine("12: histogramme d'une image");
                Console.WriteLine("13: coder et decoder une image dans une autre");
                Console.WriteLine("14: générateur de QR code");
                Console.WriteLine("q pour sortir");
                Console.WriteLine();
                Console.WriteLine("Projet créativité et innovation");
                Console.WriteLine("15: Encodage et decodage de données avec Run-Length encoding");
                Console.WriteLine("16: Encodage RLE sur image couleur (innovation complete)!");
                Console.WriteLine("17: Cryptage discret de données sur une image");
                Console.WriteLine();
                Console.WriteLine("BONUS!");
                Console.WriteLine("18: informations sur l'image");
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Agrandir une image");
                        Console.WriteLine("Choisissez un entier ou on pour agrandir l'image " +
                            "(je vous conseille entre 2 & 8 car c'est le petit carré noir");
                        double saisie = Convert.ToDouble(Console.ReadLine());
                        MyImage une = new MyImage("TEST001.bmp");
                        une.Show();
                        une.Agrandir(saisie);
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("Retrecir une image");
                        Console.WriteLine("Choisissez un entier pour retrecir une image");
                        int saisie2 = Convert.ToInt32(Console.ReadLine());
                        MyImage deux = new MyImage("TEST001_grand.bmp");
                        deux.Show();
                        deux.Retrecir(saisie2);
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("Passage en nuance de gris");
                        MyImage trois = new MyImage("coco.bmp");
                        trois.Show();
                        trois.Nuance_gris();
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("Passage en noir et blanc");
                        MyImage quatre = new MyImage("coco.bmp");
                        quatre.Show();
                        quatre.Noir_et_blanc();
                        break;
                    case "5":
                        Console.Clear();
                        Console.WriteLine("Rotation d'une image");
                        Console.WriteLine("Choisissez l'angle en degres pour la rotation (0-360°)");
                        int saisie5 = Convert.ToInt32(Console.ReadLine());
                        MyImage cinq = new MyImage("TEST001_grand.bmp");
                        cinq.Show();
                        cinq.Rotation(saisie5);
                        break;
                    case "6":
                        Console.Clear();
                        Console.WriteLine("Effet mirroir");
                        MyImage six = new MyImage("TEST001_grand.bmp");
                        six.Show();
                        six.Mirroir();
                        break;
                    case "7":
                        Console.Clear();
                        Console.WriteLine("Detection de contour");
                        MyImage sept = new MyImage("TEST001_grand.bmp");
                        sept.Show();
                        sept.Edge_detection();
                        break;
                    case "8":
                        Console.Clear();
                        Console.WriteLine("Renforcement des bords");
                        MyImage huit = new MyImage("TEST001_grand.bmp");
                        huit.Show();
                        huit.Renforcement_bord();
                        break;
                    case "9":
                        Console.Clear();
                        Console.WriteLine("Flou de gauss");
                        MyImage neuf = new MyImage("coco.bmp");
                        neuf.Show();
                        neuf.Gauss_blur();
                        break;
                    case "10":
                        Console.Clear();
                        Console.WriteLine("Repoussage");
                        MyImage dix = new MyImage("TEST001_grand.bmp");
                        dix.Show();
                        dix.Repoussage();
                        break;
                    case "11":
                        Console.Clear();
                        Console.WriteLine("Fractale");
                        MyImage onze = new MyImage("coco.bmp");
                        onze.Fractale();
                        break;
                    case "12":
                        Console.Clear();
                        Console.WriteLine("Histogramme d'une image");
                        MyImage douze = new MyImage("coco.bmp");
                        douze.Show();
                        douze.Histogramme();
                        break;
                    case "13":
                        Console.Clear();
                        Console.WriteLine("Coder et decoder une image dans une autre");
                        MyImage treize = new MyImage("coco.bmp");
                        MyImage treize_bis = new MyImage("TEST001_grand.bmp");
                        Console.WriteLine("Image dominante");
                        Console.WriteLine("press enter...");
                        Console.ReadLine();
                        treize.Show();
                        Console.Clear();
                        Console.WriteLine("Image a encodé");
                        Console.WriteLine("Press enter...");
                        Console.ReadLine();
                        treize_bis.Show();
                        Console.Clear();
                        Console.WriteLine("Encodage...");
                        Console.WriteLine("Press enter...");
                        Console.ReadLine();
                        Console.WriteLine("resultat");
                        treize.Encodage_photo(treize_bis);
                        Console.Clear();
                        Console.WriteLine("Decodage...");
                        treize.Decodage_photo();
                        break;
                    case "14":
                        Console.Clear();
                        Console.WriteLine("Generateur de QR code");
                        Console.WriteLine("Veuillez rentrez un texte...");
                        string saisie14 = Console.ReadLine();
                        QR_code qr = new QR_code(saisie14);
                        Console.WriteLine("60sec avant de retourner a l'accueil...");
                        Thread.Sleep(60000);
                        break;
                    case "15":
                        Console.Clear();
                        Console.WriteLine("Voici mon travail sur le RLE pour une image noir et blanc. J'ai encodé et tenté de décoder une" +
                            "image avec le RLE toutes les données sont ecrites juste en dessous.");
                        MyImage test = new MyImage("TEST002.bmp");
                        Console.WriteLine("Image a encoder...");
                        Console.WriteLine("Press enter...");
                        Console.ReadLine();
                        test.Show();
                        Run_length_encoding RLE = new Run_length_encoding(test);
                        Console.WriteLine();
                        Console.WriteLine("Image décodée...");
                        Console.WriteLine("Press enter...");
                        Console.ReadLine();
                        RLE.Decode();
                        Thread.Sleep(30000);
                        break;
                    case "16":
                        Console.Clear();
                        Console.WriteLine("Mon format RLE pour une image noir et blanc!");
                        MyImage noir_et_blanc = new MyImage("TEST001.bmp");
                        noir_et_blanc.Show();
                        Run_length_encoding innov = new Run_length_encoding(noir_et_blanc, 2);
                        Console.WriteLine("Attendre 10 sec... la suite arrive");
                        Thread.Sleep(10000);
                        Console.WriteLine("ATTENTION! COMPRESSION TRES LONGUE A EXECUTER (5mn) POUR IMAGE COULEUR, SE REFERRER AU RAPPORT" +
                            "POUR LES RESULTATS");
                        Console.WriteLine("q pour sortir, le reste pour continuer");
                        string choix = Console.ReadLine();
                        if (choix!="q")
                        {
                            MyImage coco = new MyImage("coco.bmp");
                            coco.Show();
                            Run_length_encoding innov_couleur = new Run_length_encoding(noir_et_blanc, 2);
                        }
                        break;
                    case "17":
                        Console.Clear();
                        Console.WriteLine("Cryptage discret de données");
                        Console.WriteLine("Veuillez rentrer un texte a crypter");
                        MyImage crypt = new MyImage("coco.bmp");
                        string text = Console.ReadLine();
                        crypt.Encodage(text);
                        Console.WriteLine();
                        Console.WriteLine("Image Encodée...");
                        crypt.Show();
                        Console.WriteLine("Decodage...");
                        Console.WriteLine("Press Enter...");
                        Console.ReadLine();
                        crypt.Decodage();
                        Console.WriteLine("delay: 10 sec...");
                        Thread.Sleep(10000);
                        break;
                    case "18":
                        Console.Clear();
                        Console.WriteLine("Informations sur l'image");
                        MyImage seize = new MyImage("TEST001.bmp");
                        seize.BMP_info("TEST001.bmp");
                        Console.WriteLine("delay: 10 sec...");
                        Thread.Sleep(10000);
                        break;
                    case "q":
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            } while (input!="q");
        }
    }
}
