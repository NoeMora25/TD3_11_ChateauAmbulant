using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique principale du jeu gérant le mouvement, les ennemis et les conditions de victoire.
    /// </summary>
    public partial class WindowJeu : Window
    {
        // --- Paramètres de temps et progression ---
        private TimeSpan tempsTrajetTotal = TimeSpan.FromMinutes(1);      // Temps nécessaire pour atteindre l'arrivée
        private TimeSpan tempsMaxAutorise = TimeSpan.FromMinutes(1.15);   // Limite de temps absolue avant défaite
        private static int pasFond = 10;                                  // Vitesse de défilement du décor
        private int nb = 0;                                               // Compteur pour l'indexation de l'animation
        private BitmapImage[] chateaux = new BitmapImage[9];              // Tableau stockant les frames de l'animation du château

        // --- Timers et chronomètres ---
        public static DispatcherTimer minuterie { get; set; } = new DispatcherTimer();      // Timer principal (décor/château)
        public static DispatcherTimer minuterieEnemi { get; set; } = new DispatcherTimer(); // Timer dédié aux ennemis
        public static DispatcherTimer minuterieTimer { get; set; } = new DispatcherTimer(); // Timer de mise à jour de l'affichage

        Stopwatch chronoFeu = new Stopwatch();   // Gère le rythme de consommation du bois
        Stopwatch chronoJeu = new Stopwatch();   // Mesure la progression réelle vers l'arrivée
        Stopwatch chronoTotal = new Stopwatch(); // Mesure le temps global écoulé depuis le début

        // --- États du jeu et ennemis ---
        private bool corbeauEnVol = false;
        private int delaifeu = 10;               // Intervalle de temps avant de perdre une bûche (selon niveau)
        private const int PAS_CORBEAU = 18;      // Vitesse horizontale du corbeau
        private bool avionEnVol = false;
        private const int PAS_AVION = 18;        // Vitesse horizontale de l'avion
        private bool bombeEnVol = false;
        private const int PAS_BOMBE = 9;         // Vitesse de chute de la bombe

        private bool casse = false;              // Indique si le château est immobilisé (froid/surchauffe)
        public static bool chateauDetruit { get; set; } = false;
        public static int niveau { get; set; }   // Niveau de difficulté sélectionné
        public static bool bouclierActif { get; set; } = false; // État de protection contre les ennemis

        public WindowJeu()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeImages();
            this.Closed += WindowJeu_Closed;

            // Configuration de la difficulté en fonction du niveau
            if (niveau == 1)
            { delaifeu = 10; tempsMaxAutorise = TimeSpan.FromMinutes(1.25); }
            else if (niveau == 2)
            { delaifeu = 7; tempsMaxAutorise = TimeSpan.FromMinutes(1.15); }
            else if (niveau == 3)
            { delaifeu = 5; tempsMaxAutorise = TimeSpan.FromMinutes(1.05); }
        }

        // Nettoyage automatique des ressources à la fermeture de la fenêtre
        private void WindowJeu_Closed(object sender, EventArgs e) => Arreter();

        /// <summary>
        /// Initialise tous les timers du jeu.
        /// </summary>
        private void InitializeTimer()
        {
            minuterie.Interval = TimeSpan.FromMilliseconds(16);      // Env. 60 FPS
            minuterieEnemi.Interval = TimeSpan.FromMilliseconds(16);
            minuterieTimer.Interval = TimeSpan.FromMilliseconds(100);

            minuterie.Tick += Jeu;
            minuterieEnemi.Tick += Corbeau;
            minuterieEnemi.Tick += Avion;
            minuterieTimer.Tick += MAJTimer;

            minuterie.Start();
            minuterieEnemi.Start();
            minuterieTimer.Start();
            chronoTotal.Start();
        }

        /// <summary>
        /// Met à jour le label du chronomètre et vérifie les conditions de fin de partie.
        /// </summary>
        private void MAJTimer(object? sender, EventArgs e)
        {
            TimeSpan ecoule = chronoJeu.Elapsed;
            double restantPourGagner = Math.Max(0, (tempsTrajetTotal - ecoule).TotalSeconds);
            double tempsLimiteVie = Math.Max(0, (tempsMaxAutorise - chronoTotal.Elapsed).TotalSeconds);

            label_chrono.Content = $"Arrivée dans : {restantPourGagner:F1}s | Limite critique : {tempsLimiteVie:F1}s";

            // Victoire : le trajet est terminé sans encombre
            if (restantPourGagner == 0 && !chateauDetruit && !casse)
                TerminerPartie(true);

            // Défaite : la limite de temps critique est atteinte
            if (restantPourGagner >= tempsLimiteVie)
                TerminerPartie(false);
        }

        /// <summary>
        /// Affiche l'écran de fin (gagné ou perdu).
        /// </summary>
        private void TerminerPartie(bool victoire)
        {
            Arreter();
            label_fin.Visibility = Visibility.Visible;
            bouton_rejouer.Visibility = Visibility.Visible;

            string imageFin = victoire ? "image_fin_gagne.png" : "image_fin_perd.png";
            label_fin.Background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/images/{imageFin}")));
        }

        /// <summary>
        /// Boucle principale : défilement, animation et gestion du combustible.
        /// </summary>
        private async void Jeu(object? sender, EventArgs e)
        {
            chronoJeu.Start();
            Deplace(Background1, pasFond);
            Deplace(Background2, pasFond);

            // Gestion de l'animation par changement de source d'image toutes les 8 frames
            nb++;
            if (nb == chateaux.Length * 8) nb = 0;
            if (nb % 8 == 0) chateau1.Source = chateaux[nb / 8];

            chronoFeu.Start();

            if (chateauDetruit) Arreter();

            // Consommation automatique du bois
            if (chronoFeu.Elapsed.TotalSeconds > delaifeu)
            {
                Window_alimentation.nombre_buches--;
                chronoFeu.Restart();
                Window_alimentation.Verif_alimentation();

                // Décompte visuel sur l'interface
                for (int i = delaifeu; i >= 1; i--)
                {
                    label_buches.Content = $"{i}s";
                    await Task.Delay(1000);
                }
            }

            // --- Vérification des états du feu ---
            if (Window_alimentation.nombre_buches < 1) // Trop froid
            {
                casse = true;
                thermometres.Source = new BitmapImage(new Uri("pack://application:,,,/images/thermometre_froid.png"));
                chateau1.Source = new BitmapImage(new Uri("pack://application:,,,/images/imgChateau_glace.png"));
                ArrêterMouvements();

                label_timer_froid.Visibility = Visibility.Visible;
                for (int i = 10; i >= 1; i--)
                {
                    label_timer_froid.Content = $"alimenter avant : {i}s";
                    await Task.Delay(1000);
                }
                if (Window_alimentation.nombre_buches < 1) label_timer_froid.Content = "Loupé !";
                else label_timer_froid.Visibility = Visibility.Hidden;
            }
            else if (Window_alimentation.nombre_buches > 5) // Surchauffe
            {
                casse = true;
                thermometres.Source = new BitmapImage(new Uri("pack://application:,,,/images/thermometre_brulant.png"));
                chateau1.Source = new BitmapImage(new Uri("pack://application:,,,/images/imgChateau_Enflamme.png"));
                bouton_eteindre.Visibility = Visibility.Visible;
                ArrêterMouvements();
            }
            else // État normal
            {
                thermometres.Source = new BitmapImage(new Uri($"pack://application:,,,/images/thermometre_{Window_alimentation.nombre_buches}.png"));
                casse = false;
            }
        }

        // Stoppe les chronomètres de progression et le timer principal
        private void ArrêterMouvements()
        {
            chronoJeu.Stop();
            minuterie.Stop();
        }

        /// <summary>
        /// IA et mouvement du corbeau.
        /// </summary>
        private void Corbeau(object? sender, EventArgs e)
        {
            Random randint = new Random();
            if (!corbeauEnVol && randint.Next(1, 400) == 1)
            {
                Canvas.SetRight(img_corbeau, 0);
                corbeauEnVol = true;
            }

            if (corbeauEnVol)
            {
                Deplace(img_corbeau, PAS_CORBEAU);
                if (Canvas.GetLeft(img_corbeau) <= 220) // Collision avec le château
                {
                    if (!bouclierActif)
                    {
                        DetruireChateau();
                    }
                    Canvas.SetLeft(img_corbeau, 1900);
                    bouclierActif = false; // Le bouclier est consommé après une attaque
                    corbeauEnVol = false;
                }
            }
        }

        // Change l'image du château pour l'état détruit et arrête le jeu
        private void DetruireChateau()
        {
            chateau1.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/maisonCassee.png"));
            chateauDetruit = true;
            ArrêterMouvements();
        }

        /// <summary>
        /// IA de l'avion et largage de la bombe.
        /// </summary>
        private void Avion(object? sender, EventArgs e)
        {
            Random randint = new Random();
            if (!avionEnVol && randint.Next(1, 500) == 1)
            {
                Canvas.SetRight(img_avion, 0);
                avionEnVol = true;
            }
            if (avionEnVol)
            {
                Deplace(img_avion, PAS_AVION);
                if (Canvas.GetLeft(img_avion) <= 220 && !bombeEnVol) // Largage
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
                if (Canvas.GetBottom(img_bombe) <= 100) // Impact
                {
                    DetruireChateau();
                    bombeEnVol = false;
                    Canvas.SetBottom(img_bombe, 800);
                }
            }
        }

        // --- Méthodes de déplacement d'objets sur le Canvas ---
        public void Deplace(Image image, int pas)
        {
            Canvas.SetLeft(image, Canvas.GetLeft(image) - pas);
            if (Canvas.GetLeft(image) + image.Width <= 0)
                Canvas.SetLeft(image, image.Width - pas);
        }

        public void DeplaceVertical(Image image, int pas)
        {
            Canvas.SetBottom(image, Canvas.GetBottom(image) - pas);
            if (Canvas.GetBottom(image) + image.Height <= 0)
                Canvas.SetBottom(image, image.Height - pas);
        }

        // Initialise les ressources graphiques et masque les éléments d'UI de fin
        private void InitializeImages()
        {
            for (int i = 0; i < 9; i++)
                chateaux[i] = new BitmapImage(new Uri($"pack://application:,,,/images/imgChateau{i + 1}.gif"));

            label_fin.Visibility = Visibility.Hidden;
            bouton_rejouer.Visibility = Visibility.Hidden;
            bouton_eteindre.Visibility = Visibility.Hidden;
            label_timer_froid.Visibility = Visibility.Hidden;
        }

        // --- Gestion des événements de l'interface utilisateur (Clics boutons) ---
        private void ButPause_Click(object sender, RoutedEventArgs e)
        {
            WindowMenu_Pause fenetre = new WindowMenu_Pause(this);
            fenetre.Show();
            minuterie.Stop();
        }

        private void bouton_temperature_Click(object sender, RoutedEventArgs e) => new Window_alimentation().Show();
        private void bouton_reparation_Click(object sender, RoutedEventArgs e) => new Window_reparer_bombe().Show();
        private void bouton_bouclier_Click(object sender, RoutedEventArgs e) => new Window_sort_protection().Show();

        private void bouton_rejouer_Click(object sender, RoutedEventArgs e)
        {
            new Window1Selection().Show();
            this.Close();
        }

        private void bouton_eteindre_Click(object sender, RoutedEventArgs e)
        {
            casse = false;
            Window_alimentation.nombre_buches = 1; // Réinitialise à un état stable
            minuterie.Start();
            chronoJeu.Start();
        }

        /// <summary>
        /// Nettoie tous les abonnements et stoppe les processus pour éviter les fuites mémoire.
        /// </summary>
        private void Arreter()
        {
            minuterie?.Stop();
            minuterieEnemi?.Stop();
            minuterieTimer?.Stop();
            chronoTotal?.Stop();
            chronoFeu?.Stop();
            chronoJeu?.Stop();

            minuterie.Tick -= Jeu;
            minuterieEnemi.Tick -= Corbeau;
            minuterieEnemi.Tick -= Avion;
            minuterieTimer.Tick -= MAJTimer;

            chateauDetruit = false;
            bouclierActif = false;
        }
    }
}