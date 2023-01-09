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
    /// Lógica de interacción para WPFStart.xaml
    /// </summary>
    public partial class WPFStart : Window
    {
        #region Constructor 
        public WPFStart()
        {
            InitializeComponent();

        }

        #endregion

        #region Form Methods

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtUser.Text == "" || txtPassword.Password == "")
                {
                    MessageBox.Show("Incorrecto", "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //ManageDomain mng = new ManageDomain();
                    //int pass = mng.ValidateCredentials(txtUser.Text, txtPassword.Text);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();

        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //enter key is down
                if (txtUser.Text == "" || txtPassword.Password == "")
                {
                    MessageBox.Show("Error", "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //ManageDomain mng = new ManageDomain();

                    //int pass = mng.ValidateCredentials(txtUser.Text, txtPassword.Text);

                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        #endregion
    }
}
