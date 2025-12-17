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
            
			bouton_reparer.Visibility = Visibility.Hidden;
            bouton_reparer.IsEnabled = false;

            for (int i = 3; i > 0; i--)
            {
                label_DureeReparation.Content = $"Réparation : {i} s";
                await Task.Delay(1000);
            }

            label_DureeReparation.Content = "Réparation terminée !";
            WindowJeu.minuterie.Start();
            WindowJeu.minuterieEnemi.Start();
			WindowJeu.chateauDetruit = false;

			await Task.Delay(500);
            label_DureeReparation.Content = ""; 

			bouton_reparer.Visibility = Visibility.Visible;
            bouton_reparer.IsEnabled = true;
            this.Close();
        }
    }
   
}
