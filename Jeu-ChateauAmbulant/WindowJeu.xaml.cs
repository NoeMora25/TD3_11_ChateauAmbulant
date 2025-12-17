using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace Jeu_ChateauAmbulant
{
	/// <summary>
	/// Logique d'interaction pour UCJeu.xaml
	/// </summary>
	public partial class WindowJeu : Window
	{
		private TimeSpan tempsTrajetTotal = TimeSpan.FromMinutes(1);
		private TimeSpan tempsMaxAutorise = TimeSpan.FromMinutes(1.15); 
		private static int pasFond = 10;
		private int nb = 0;
		private BitmapImage[] chateaux = new BitmapImage[9];
		public static DispatcherTimer minuterie { get; set; } = new DispatcherTimer();
		public static DispatcherTimer minuterieEnemi { get; set; } = new DispatcherTimer();
		Stopwatch chronoFeu = new Stopwatch();
		Stopwatch chronoJeu = new Stopwatch();
		private bool corbeauEnVol = false;
		private int delaifeu = 10;
		private const int PAS_CORBEAU = 18;
        private bool avionEnVol = false;
        private const int PAS_AVION = 18;
        private bool bombeEnVol = false;
        private const int PAS_BOMBE = 9;
        public int alimentation_feu = 1;
        private bool casse = false;
		public static bool chateauDetruit { get; set; } = false ;
        public static int niveau { get; set; }
		public static bool bouclierActif { get; set; } = false;
        public static DispatcherTimer minuterieTimer { get; set; } = new DispatcherTimer();
        Stopwatch chronoTotal = new Stopwatch();

        public WindowJeu()
		{
			InitializeComponent();
			InitializeTimer();
			InitializeImages();
			this.Closed += WindowJeu_Closed;

            if(niveau == 1)
            { delaifeu = 10; tempsMaxAutorise = TimeSpan.FromMinutes(1.25); }
            else if (niveau == 2)
                {delaifeu = 7; tempsMaxAutorise = TimeSpan.FromMinutes(1.15); }
            else if(niveau == 3)
			    {delaifeu = 5; tempsMaxAutorise = TimeSpan.FromMinutes(1.05); }

		}
		private void WindowJeu_Closed(object sender, EventArgs e)
		{
			Arreter();
		}

		private void InitializeTimer()
		{
			minuterie = new DispatcherTimer();
			minuterieEnemi = new DispatcherTimer();

			minuterie.Interval = TimeSpan.FromMilliseconds(16);
			minuterieEnemi.Interval = TimeSpan.FromMilliseconds(16);
			minuterie.Start();
			minuterieEnemi.Start();

			minuterie.Tick += Jeu;

			minuterieEnemi.Tick += Corbeau;
            minuterieEnemi.Tick += Avion;

            minuterie.Start();
			minuterieEnemi.Start();

            minuterieTimer.Interval = TimeSpan.FromMilliseconds(100);
            minuterieTimer.Tick += MAJTimer; 
            minuterieTimer.Start();
            chronoTotal.Start();
        }

		private void MAJTimer(object? sender, EventArgs e)
		{
			TimeSpan ecoule = chronoJeu.Elapsed;

			double restantPourGagner = Math.Max(0, (tempsTrajetTotal - ecoule).TotalSeconds);
			double tempsLimiteVie = Math.Max(0, (tempsMaxAutorise - chronoTotal.Elapsed).TotalSeconds);

			label_chrono.Content = $"Arrivée dans : {restantPourGagner:F1}s | Limite critique : {tempsLimiteVie:F1}s";

			if (restantPourGagner == 0 && !chateauDetruit && !casse)
			{
				TerminerPartie(true);
			}

			if (restantPourGagner >= tempsLimiteVie)
			{
				TerminerPartie(false);
			}
		}
		private void TerminerPartie(bool victoire)
		{
			minuterie.Stop();
			minuterieEnemi.Stop();
			minuterieTimer.Stop();
			chronoTotal.Stop();
			chronoJeu.Stop();

			label_fin.Visibility = Visibility.Visible;
			bouton_rejouer.Visibility = Visibility.Visible;

			if (victoire)
			{
				label_fin.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/image_fin_gagne.png")));
			}
			else
			{
				label_fin.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/image_fin_perd.png")));
			}
		}

        private async void Jeu(object? sender, EventArgs e)
        {
            chronoJeu.Start();
            Deplace(Background1, pasFond);
            Deplace(Background2, pasFond);
            nb++;

            if (nb == chateaux.Length * 8)
                nb = 0;
            if (nb % 8 == 0)
                chateau1.Source = chateaux[nb / 8];

            chronoFeu.Start();


            if (chateauDetruit)
            {
                minuterie.Stop();
                chronoJeu.Stop();
            }

            if (chronoFeu.Elapsed.TotalSeconds > delaifeu)
            {
                Window_alimentation.nombre_buches += -1;
                chronoFeu.Restart();
                Window_alimentation.Verif_alimentation();
                for (int i = delaifeu; i >= 1; i--)
                {
                    label_buches.Content = $"{i}s";
                    await Task.Delay(1000);
                }

            }

            if (Window_alimentation.nombre_buches < 1)
            {
                casse = true;
                thermometres.Source = new BitmapImage(new Uri($"pack://application:,,,/images/thermometre_froid.png"));
                chateau1.Source = new BitmapImage(new Uri($"pack://application:,,,/images/imgChateau_glace.png"));
                chronoJeu.Stop();
                minuterie.Stop();
                label_timer_froid.Visibility = Visibility.Visible;
                for (int i = 10; i >= 1; i--)
                {
                    label_timer_froid.Content = $"alimenter avant : {i}s";
                    await Task.Delay(1000);
                }

                if (Window_alimentation.nombre_buches < 1)
                {
                    casse = true;
                    label_timer_froid.Content = "Loupé !";
                }
                else
                {
                    label_timer_froid.Visibility = Visibility.Hidden;
                    label_timer_froid.Content = "";
                }

            }

            else if (Window_alimentation.nombre_buches > 5)
            {
                casse = true;
                thermometres.Source = new BitmapImage(new Uri($"pack://application:,,,/images/thermometre_brulant.png"));
                chateau1.Source = new BitmapImage(new Uri($"pack://application:,,,/images/imgChateau_Enflamme.png"));
                bouton_eteindre.Visibility = Visibility.Visible;

            }
            else
            {

                InitializeImages();
                thermometres.Source = new BitmapImage(new Uri($"pack://application:,,,/images/thermometre_{Window_alimentation.nombre_buches}.png"));
                casse = false;
            }


            if (casse)
            {
                chronoJeu.Stop();
                minuterie.Stop();
                return;
            }
        }

		private void Corbeau(object? sender, EventArgs e)
		{
            minuterieEnemi.Start();
			Random randint = new Random();
			int probaEnemi = randint.Next(1, 400); 

			if (!corbeauEnVol)
			{
				if (probaEnemi == 1)
				{
					Canvas.SetRight(img_corbeau, 0); 
					corbeauEnVol = true;
				}
			}

			if (corbeauEnVol)
			{
				Deplace(img_corbeau, PAS_CORBEAU);

				if (Canvas.GetLeft(img_corbeau) <= 220)
				{
                    if (!bouclierActif)
                    {
						chateau1.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/maisonCassee.png"));
						corbeauEnVol = false;						
						chronoJeu.Stop();
						minuterie.Stop();
						chateauDetruit = true;
					}
					Canvas.SetLeft(img_corbeau, 1900);
                    bouclierActif = false;
                    corbeauEnVol = false;
				}
			}
		}
		
		public void Deplace(System.Windows.Controls.Image image, int pas)
        {
            Canvas.SetLeft(image, Canvas.GetLeft(image) - pas);
            if (Canvas.GetLeft(image) + image.Width <= 0)
                Canvas.SetLeft(image, image.Width - pas);
        }

        public void DeplaceVertical(System.Windows.Controls.Image image, int pas)
        {
			Canvas.SetBottom(image, Canvas.GetBottom(image) - pas);
			if (Canvas.GetBottom(image) + image.Height <= 0)
				Canvas.SetBottom(image, image.Height - pas);
		}

        private void Avion(object? sender, EventArgs e)
        {
            minuterieEnemi.Start();
            Random randint = new Random();
            int probaEnemi = randint.Next(1, 500);
            if (!avionEnVol)
            {
                if (probaEnemi == 1)
                {
                    Canvas.SetRight(img_avion, 0);
                    avionEnVol = true;
                }
            }
            if (avionEnVol)
            {
                Deplace(img_avion, PAS_AVION);
                if (Canvas.GetLeft(img_avion) <= 220 && !bombeEnVol)
                {
					Canvas.SetLeft(img_bombe, 220);
					Canvas.SetBottom(img_bombe, 700);
					img_bombe.Visibility = Visibility.Visible;
					bombeEnVol = true;
				}
                if (Canvas.GetLeft(img_avion) <= 0)
                {
                    avionEnVol = false;
					Canvas.SetLeft(img_avion, 1900);
				}
            }
            if (bombeEnVol)
            {
                DeplaceVertical(img_bombe, PAS_BOMBE);
                if (Canvas.GetBottom(img_bombe) <= 100)
                {
					
					chateau1.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/maisonCassee.png"));
                    bombeEnVol = false;
                    Canvas.SetBottom(img_bombe, 800);
                    chateauDetruit = true;
                    chronoJeu.Stop();
                    minuterie.Stop();
                }
			}
		}
		private void InitializeImages()
        {
            for (int i = 0; i < 9; i++)
                chateaux[i] = new BitmapImage(new Uri($"pack://application:,,,/images/imgChateau{i + 1}.gif"));

            label_fin.Visibility = Visibility.Hidden;
            bouton_rejouer.Visibility = Visibility.Hidden;
            bouton_eteindre.Visibility = Visibility.Hidden;
            label_timer_froid.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AfficheMenuPause();
        }

        public void AfficheMenuPause()
        {
            WindowMenu_Pause fenetre = new WindowMenu_Pause(this);
            fenetre = new WindowMenu_Pause(this);
            // associe l'écran au conteneur
            fenetre.Show();
            minuterie.Stop();
        }

        public void AfficherJeu()
        {
            WindowJeu fenetre = new WindowJeu();
            fenetre.Show();
            this.Close();
        }

        private void ButPause_Click(object sender, RoutedEventArgs e)
        {
            AfficheMenuPause();
        }

        private void bouton_temperature_Click(object sender, RoutedEventArgs e)
        {
            Window_alimentation fenetre = new Window_alimentation();
            fenetre.Show();

        }

        private void bouton_rejouer_Click(object sender, RoutedEventArgs e)
        {

			Window1Selection fenetre = new Window1Selection();
			fenetre.Show();
			this.Close();

		}

        private void bouton_reparation_Click(object sender, RoutedEventArgs e)
        {
            Window_reparer_bombe fenetre = new Window_reparer_bombe();
            fenetre.Show();
        }

        private void bouton_bouclier_Click(object sender, RoutedEventArgs e)
        {
            Window_sort_protection fenetre = new Window_sort_protection();
            fenetre.Show(); 
        }

        private void bouton_eteindre_Click(object sender, RoutedEventArgs e)
        {
            casse = false;
            Window_alimentation.nombre_buches = 1;
            bouton_eteindre.Visibility = Visibility.Visible;
            minuterie.Start();
            chronoJeu.Start();
        }

		private void Arreter()
		{
			// Arrêt des Timers
			minuterie?.Stop();
			minuterieEnemi?.Stop();
			minuterieTimer?.Stop();

			// Arrêt des Stopwatch
			chronoTotal?.Stop();
			chronoFeu?.Stop();
			chronoJeu?.Stop();

			// DÉSABONNEMENT (Crucial pour éviter les freezes au prochain lancement)
			minuterie.Tick -= Jeu;
			minuterieEnemi.Tick -= Corbeau;
			minuterieEnemi.Tick -= Avion;
			minuterieTimer.Tick -= MAJTimer;

			// Réinitialisation des variables statiques si nécessaire
			chateauDetruit = false;
			bouclierActif = false;
		}


		////private void MenuItem_Click(object sender, RoutedEventArgs e)
		//{
		//    minuterie.Stop();
		//    ParametreWindow parametreWindow = new ParametreWindow();
		//    bool? rep = parametreWindow.ShowDialog();
		//    if (rep == true)
		//    {
		//        minuterie.Start();
		//        double vitesse = parametreWindow.slidVitesse.Value;
		//        if (vitesse == 2)
		//        {
		//            pasFond = 2;
		//            if (nb == chateaux.Length * 4)
		//                nb = 0;
		//            if (nb % 4 == 0)
		//                chateau1.Source = chateaux[nb / 4];

		//        }
		//        else if (vitesse == 1)
		//        {
		//            pasFond = 1;
		//            if (nb == chateaux.Length * 8)
		//                nb = 0;
		//            if (nb % 8 == 0)
		//                chateau1.Source = chateaux[nb / 8];
		//        }
		//        if (vitesse == 3)
		//        {
		//            pasFond = 4;
		//            if (nb == chateaux.Length * 2)
		//                nb = 0;
		//            if (nb % 2 == 0)
		//                chateau1.Source = chateaux[nb / 2];
		//        }
	}
}

