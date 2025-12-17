using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique d'interaction pour Window1Selection.xaml
    /// </summary>
    public partial class Window1Selection : Window
    {
		private MediaPlayer mediaPlayer = new MediaPlayer();
		public Window1Selection()
        {
            InitializeComponent();
			affiche();
		}
        
        private void affiche()
        {
			VersRegles uc = new VersRegles();
            ZoneMenu.Content = uc;
            uc.ButRegles.Click += AfficherRegles;
            uc.ButParam.Click += AfficherParam;
            uc.butniv1.Click += AfficherNiv1;
            uc.butniv3.Click += AfficherNiv3;
            uc.butniv2.Click += AfficherNiv2;
        }

        private void affiche(object sender, RoutedEventArgs e)
        {
            VersRegles uc = new VersRegles();
            ZoneMenu.Content = uc;
            uc.ButRegles.Click += AfficherRegles;
            uc.ButParam.Click += AfficherParam;
            uc.butniv1.Click += AfficherNiv1;
			uc.butniv3.Click += AfficherNiv3;
			uc.butniv2.Click += AfficherNiv2;
		}

        private void AfficherNiv1(object sender, RoutedEventArgs e)
        {
            WindowJeu.niveau = 1;
            WindowJeu maNouvelleFenetre = new WindowJeu();
            
			maNouvelleFenetre.Show();
            this.Close();
        }
		private void AfficherNiv2(object sender, RoutedEventArgs e)
		{
            WindowJeu.niveau = 2;
            WindowJeu maNouvelleFenetre = new WindowJeu();
			maNouvelleFenetre.Show();
			this.Close();
		}

		private void AfficherNiv3(object sender, RoutedEventArgs e)
		{
            WindowJeu.niveau = 3;
            WindowJeu maNouvelleFenetre = new WindowJeu();
			maNouvelleFenetre.Show();
			this.Close();
		}

		private void AfficherParam(object sender, RoutedEventArgs e)
        {
            UCParam uc = new UCParam();
            ZoneMenu.Content = uc;
            uc.butRetour.Click += affiche;
        }

        private void AfficherRegles(object sender, RoutedEventArgs e)
        {
            UCconsignes uc = new UCconsignes();
            ZoneMenu.Content = uc;
            uc.butRetour.Click += affiche;
        }
    }
}
