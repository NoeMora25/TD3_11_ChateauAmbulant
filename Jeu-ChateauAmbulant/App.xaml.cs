using System.Windows;
using System.Windows.Media;
using System.IO;
using System;
using System.Configuration;
using System.Data;


namespace Jeu_ChateauAmbulant
{

    public partial class App : Application
    {
        // Instance du lecteur multimédia pour gérer la lecture audio
        private MediaPlayer mediaPlayer = new MediaPlayer();

        private void musique_Fond()
        {
            // Note : UCParam semble être instancié ici mais n'est pas utilisé dans cette méthode.
            UCParam uc = new UCParam();

            // Définition du chemin relatif vers le fichier audio
            string relativePath = @"sons/musi.mp3";

            // Construction du chemin absolu pour s'assurer que le fichier est trouvé peu importe l'emplacement d'exécution
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            // Chargement et configuration du lecteur
            mediaPlayer.Open(new Uri(fullPath));
            mediaPlayer.Volume = 0.9; // Volume réglé à 90%

            // Lancement de la lecture
            mediaPlayer.Play();
        }

        protected override void OnStartup(StartupEventArgs e)//appel automatique
        {
            // Appel de la méthode de base pour préserver le comportement standard de WPF
            base.OnStartup(e);

            // Démarrage de la musique dès l'ouverture du jeu
            musique_Fond();
        }
    }
}