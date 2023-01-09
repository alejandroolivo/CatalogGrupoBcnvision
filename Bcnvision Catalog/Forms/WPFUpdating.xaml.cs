using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para WPFUpdating.xaml
    /// </summary>
    public partial class WPFUpdating : Window
    {
        public WPFUpdating()
        {
            InitializeComponent();
            //this.Show();
            this.DataContext = this;
        }


        public void waitXSeconds(int x)
        {
            //await Task.Delay(x*1000);
            System.Threading.Thread.Sleep(x * 1000);
            return;
        }
    }
}
