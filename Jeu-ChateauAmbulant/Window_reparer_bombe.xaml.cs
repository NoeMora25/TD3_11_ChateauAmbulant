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
using System.Windows.Shapes;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique d'interaction pour Window_reparer_bombe.xaml
    /// </summary>
    public partial class Window_reparer_bombe : Window
    {
        public Window_reparer_bombe()
        {
            InitializeComponent();
        }

        private async void bouton_reparer_Click(object sender, RoutedEventArgs e)
        {
            
			bouton_reparer.Visibility = Visibility.Hidden; //cacher le bouton pour éviter de clicker plusieurs fois
            bouton_reparer.IsEnabled = false; //désactiver le bouton

            for (int i = 3; i > 0; i--)
            {
                label_DureeReparation.Content = $"Réparation : {i} s";//afficher le temps de progression
                await Task.Delay(1000);//attendre 1 seconde
            }

            label_DureeReparation.Content = "Réparation terminée !";
            WindowJeu.minuterie.Start(); 
            WindowJeu.minuterieEnemi.Start();
			WindowJeu.chateauDetruit = false;
            //recommencer le jeu 


			await Task.Delay(500);
            label_DureeReparation.Content = ""; //attendre un poil avant de raficher le bouton

			bouton_reparer.Visibility = Visibility.Visible;
            bouton_reparer.IsEnabled = true;
            this.Close(); // fermeture fenetre
        }
    }
   
}
