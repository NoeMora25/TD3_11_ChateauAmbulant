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
    /// Logique d'interaction pour WindowMenu_Pause.xaml
    /// </summary>
    public partial class WindowMenu_Pause : Window
    {
        private WindowJeu _fenetreJeu;
        public WindowMenu_Pause(WindowJeu fenetreJeu)
        {
            InitializeComponent();
            _fenetreJeu = fenetreJeu; // On sauvegarde la référence
        }

        private void boutton_reprendre_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            WindowJeu.minuterie.Start();
           
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// 1. Arrêter tous les timers proprement
			WindowJeu.minuterie.Stop();
			WindowJeu.minuterieEnemi.Stop();
			WindowJeu.minuterieTimer.Stop();

			Window1Selection uc = new Window1Selection();
			uc.Show(); // afficher la nouvelle fenetre

			// 3. Fermer les fenêtres
			_fenetreJeu.Close();
			this.Close();
		}
	}
}
