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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jeu_ChateauAmbulant
{
    /// <summary>
    /// Logique d'interaction pour UCParam.xaml
    /// </summary>
    public partial class UCParam : UserControl
    {
		public event EventHandler VolumeChanged;

		public double VolumeValue
		{
			get { return sliderSon.Value; }// non fonctionel 
		}
		public UCParam()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)// fermer le  jeu
        {
            Application.Current.Shutdown();
        }
	}
}
