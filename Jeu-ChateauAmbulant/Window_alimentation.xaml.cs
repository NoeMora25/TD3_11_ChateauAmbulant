using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique d'interaction pour Window_alimentation.xaml
    /// </summary>
    public partial class Window_alimentation : Window
    {
        public static Window_alimentation Instance;
        public static int nombre_buches { get; set; } = 4;
        public Window_alimentation()
        {
            InitializeComponent();
            Instance = this;
            Verif_alimentation();
        }

        private async void bouton_nourrir_Click(object sender, RoutedEventArgs e)
        {

            nombre_buches++;
            Verif_alimentation();

            // 1. On cache et désactive le bouton pour éviter les doubles clics
            bouton_nourrir.Visibility = Visibility.Hidden;
            bouton_nourrir.IsEnabled = false;

            // 2. On lance une boucle pour le compte à rebours (ici 3 secondes)
            // La variable 'i' commence à 3 et diminue de 1 à chaque tour tant qu'elle est supérieure à 0
            for (int i = 3; i > 0; i--)
            {
                // Mise à jour du texte
                label_DureeAlimentation.Content = $"Alimentation : {i} s";

                // On attend 1 seconde (1000 millisecondes) sans bloquer l'interface
                await Task.Delay(1000);
            }

            // 3. Une fois la boucle terminée (0 secondes)
            label_DureeAlimentation.Content = "Nourri !";

            
            await Task.Delay(500);
            label_DureeAlimentation.Content = ""; // On vide le label si besoin

            // 4. On réaffiche le bouton
            bouton_nourrir.Visibility = Visibility.Visible;
            bouton_nourrir.IsEnabled = true;

        }

        private void Bouton_Retour_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // fermeture si le bouton est cliqué
        }



        public static void Verif_alimentation(object? sender = null, EventArgs e = null)
        {
            
            if (Instance == null) return; //si ce n'est pas ouvert on lance rien pour éviter les problèmes

            if (Window_alimentation.nombre_buches < 1)
            {
                WindowJeu.minuterie.Stop();
                // 3. On change le fond via l'Instance
                Instance.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/image_cheminee_eteinte_fond.png")));
            }
            else if (Window_alimentation.nombre_buches > 5)
            {
                //feu brûlé
                Instance.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/image_cheminee_brule_fond.png")));
            }
            else
            {
                // Cas normal : on remet le fond normal et on relance le jeu si besoin
                WindowJeu.minuterie.Start();
                Instance.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/image_cheminee_fond.png")));
            }
        

        }


    }

}
