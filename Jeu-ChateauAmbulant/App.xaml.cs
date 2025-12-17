using System.Windows;
using System.Windows.Media;
using System.IO;
using System;
using System.Configuration;
using System.Data;


namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private void musique_Fond()
        {
            UCParam uc = new UCParam();

            string relativePath = @"sons/musi.mp3";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            mediaPlayer.Open(new Uri(fullPath));
            mediaPlayer.Volume = 0.1;
            mediaPlayer.Play();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            musique_Fond();
        }
    }
}