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

		private async void butBouclier_Click(object sender, RoutedEventArgs e)
		{
			

			butBouclier.Content = "3secondes";
			butBouclier.IsEnabled = false;
			await Task.Delay(3000);
			WindowJeu.bouclierActif = true;
			butBouclier.Content = "Bouclier actif pendant 2secondes";
			await Task.Delay(20000);

			WindowJeu.bouclierActif = false;

			this.Close(); 
		}
	}
}
