using Microsoft.Win32;
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
using System.Windows.Threading;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window1Selection_Loaded;
        }

        private void Window1Selection_Loaded(object sender, RoutedEventArgs e)
        {
            MusicAmbiance.Play();
        }

        private void OuvrirFenetre_Click(object sender, RoutedEventArgs e)
        {
            Window1Selection maNouvelleFenetre = new Window1Selection();
            maNouvelleFenetre.Show();
            this.Close();
        }
	}
}
