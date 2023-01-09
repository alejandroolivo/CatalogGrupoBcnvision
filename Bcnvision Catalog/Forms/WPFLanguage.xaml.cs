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

namespace Bcnvision_Catalog.Forms
{
    /// <summary>
    /// Lógica de interacción para WPFLanguage.xaml
    /// </summary>
    public partial class WPFLanguage : Window
    {
        #region CONSTRUCTOR
        public WPFLanguage(CompanyMode prevEmpresa, LanguageMode prevIdioma)
        {
            InitializeComponent();

            //Rellenamos el combobox de idiomas a mano, cada idioma en su idioma
            cmbIdiomas.Items.Add("Español");
            cmbIdiomas.Items.Add("English");
            cmbIdiomas.Items.Add("Français");

            cmbIdiomas.SelectedIndex = prevIdioma == LanguageMode.spanish ? 0 : (prevIdioma == LanguageMode.english ? 1 : 2);

            //Rellenamos el combobox de tipo de empresa
            cmbEmpresa.Items.Add("Bcnvision");
            cmbEmpresa.Items.Add("Nevitec");
            
            cmbIdiomas.SelectedIndex = prevEmpresa == CompanyMode.bcnvision ? 0 : 1;
        }

        #endregion

        #region FIELDS

        public bool Selection = false;

        #endregion

        #region EVENTS
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Selection = true;
            this.Close();
        }

        #endregion

    }
}
