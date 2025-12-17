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
        
        private void affiche() //permet de superposer les UC et d'appeller les métodes de clic
        {
			VersRegles uc = new VersRegles(); //Normalement UCdemaragfe mais fonctionne que avec l'ancien nom
            ZoneMenu.Content = uc;
            uc.ButRegles.Click += AfficherRegles;
            uc.ButParam.Click += AfficherParam;
            uc.butniv1.Click += AfficherNiv1;
            uc.butniv3.Click += AfficherNiv3;
            uc.butniv2.Click += AfficherNiv2;
            // lancer les méthodes selon le click des boutons
        }

        private void affiche(object sender, RoutedEventArgs e) // surcharge qui permet de faire foncionner les butons retour 
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
            WindowJeu.niveau = 1; //donner la valeur du niveau avant de lancer la fenetre jeu
            WindowJeu maNouvelleFenetre = new WindowJeu(); // lance le jeu
			maNouvelleFenetre.Show();
            this.Close(); // fermetrure de la fenetre actuelle
        }
		private void AfficherNiv2(object sender, RoutedEventArgs e) // même logique que niveau1
		{
            WindowJeu.niveau = 2;//niveau 2
            WindowJeu maNouvelleFenetre = new WindowJeu();
			maNouvelleFenetre.Show();
			this.Close();
		}

		private void AfficherNiv3(object sender, RoutedEventArgs e)// même logique que niveau1
        {
            WindowJeu.niveau = 3;//niveau 3
            WindowJeu maNouvelleFenetre = new WindowJeu();
			maNouvelleFenetre.Show();
			this.Close();
		}

		private void AfficherParam(object sender, RoutedEventArgs e) // superposer l'UC paramètre
        {
            UCParam uc = new UCParam(); // vers les paramètres
            ZoneMenu.Content = uc;
            uc.butRetour.Click += affiche; //afficher l'UC de base
        }

        private void AfficherRegles(object sender, RoutedEventArgs e) //même logique mais pour les règles
        {
            UCconsignes uc = new UCconsignes(); //vers les règles
            ZoneMenu.Content = uc;
            uc.butRetour.Click += affiche; 
        }
    }
}
