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
    /// Logique d'interaction pour Window_sort_protection.xaml
    /// </summary>
    public partial class Window_sort_protection : Window
    {
        public Window_sort_protection()
        {
            InitializeComponent();
        }

		private async void butBouclier_Click(object sender, RoutedEventArgs e) // donne un bouclier apres 3 secondes d'attente pendant 20secondes 
		{
			butBouclier.Content = "3secondes";
			butBouclier.IsEnabled = false;
			await Task.Delay(3000);
			WindowJeu.bouclierActif = true;//définir que le bouclier éxiste
			butBouclier.Content = "Bouclier actif pendant 20 secondes";
			await Task.Delay(20000); //20secondes d'attente

			WindowJeu.bouclierActif = false; //fin du bouclier

			this.Close(); //fermeture fenetre
		}
	}
}
