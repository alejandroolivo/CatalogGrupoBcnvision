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
using System.IO;
using System.Diagnostics;
using System.Threading;
using Bcnvision_Catalog.Classes;

namespace Bcnvision_Catalog.Forms
{
    /// <summary>
    /// Lógica de interacción para WPFVersions.xaml
    /// </summary>
    public partial class WPFVersions : Window
    {
        #region FIELDS

        public bool IsUpdated = false;

        /// <summary>
        /// Ruta de la carpeta de configuracion del servidor
        /// </summary>
        private string serverConfigPath;

        /// <summary>
        /// Ruta de la carpeta de configuracion actual
        /// </summary>
        private string configPath;

        /// <summary>
        /// Info de la versión actual de la aplicación
        /// </summary>
        private DataXml.ConfigData actualConfigData;

        /// <summary>
        /// Info de la última versión en el servidor
        /// </summary>
        private DataXml.ConfigData serverConfigData;

        /// <summary>
        /// Ruta del batch que inicia la sincronización
        /// </summary>
        private string rutaSyncSett = "";

        /// <summary>
        /// Nueva version
        /// </summary>
        public string newVersion = "";

        /// <summary>
        /// Está el servidor disponible ?
        /// </summary>
        private bool serverReachable = false;
        #endregion

        #region CONSTRUCTOR
        public WPFVersions(LanguageMode Idioma, DataXml.ConfigData ConfigData, string ConfigPath, string ServerPath)
        {
            InitializeComponent();

            //Guardamos rutas
            serverConfigPath = ServerPath;
            configPath = ConfigPath;

            //Guardamos la info de la app
            actualConfigData = ConfigData;
                        
            //Ruta del batch que inicia la sincronización
            rutaSyncSett = Path.Combine(ConfigPath, "SyncSettings_YPublic_2_KContenido.ffs_batch");
                       
            //Rellenamos los lbls
            lblActualVersion.Content = ConfigData.appVersion;
            lblTitleActualVersion.Content = (Idioma == LanguageMode.spanish) ? "Versión actual:" : (Idioma == LanguageMode.english ? "Actual version:" : "Version actuelle:");
            lblTitleAvailableVersion.Content = (Idioma == LanguageMode.spanish) ? "Versión disp.:" : (Idioma == LanguageMode.english ? "Avail. version:" : "Version disp.:");
            lblTitleIsAvailable.Content = (Idioma == LanguageMode.spanish) ? "Disponible:" : (Idioma == LanguageMode.english ? "Available:" : "Disponible:");

            //Rellenamos los buttons
            btnReload.Content = (Idioma == LanguageMode.spanish) ? "Buscar" : (Idioma == LanguageMode.english ? "Search" : "Chercher");
            btnUpdate.Content = (Idioma == LanguageMode.spanish) ? "Actualizar" : (Idioma == LanguageMode.english ? "Update" : "Actualiser");

            //Rellenamos los lbls de lo nuevo
            lblAvailableVersion.Content = ConfigData.newVersion;
            lblIsAvailable.Content = "-";

        }

        #endregion

        #region METHODS
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();

            if (serverReachable)
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("¿Desea descargar el nuevo contenido del catálogo?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    //Descargamos contenido etc
                    Process.Start(rutaSyncSett);

                    //Indicamos que se ha actualizado
                    IsUpdated = true;

                    //Pausa
                    Thread.Sleep(1000);

                    this.Close();
                }
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();
        }

        public void CheckVersion()
        {

            try
            {
                //Mirámos la versión en el servidor si está disponible
                serverReachable = false;
                newVersion = actualConfigData.newVersion;

                //string rutaAppConfigServer = Path.Combine(ServerPath, @"\Config\Config Files");
                string rutaAppConfigServer = serverConfigPath;

                DirectoryInfo inf = new DirectoryInfo(rutaAppConfigServer);

                DirectoryInfo infoDire = inf.GetDirectories()[0];

                DirectoryInfo infoDireConfig = infoDire.GetDirectories("*Config*")[0];
                DirectoryInfo infoDireConfig2 = infoDireConfig.GetDirectories("*Config Files*")[0];

                FileInfo[] filesInfo = infoDireConfig2.GetFiles("*AppConfig*");
                
                foreach (var file in filesInfo)
                {
                    if (file.FullName.Contains("AppConfig.xml"))
                    {
                        serverReachable = true;

                        //cogemos el app config del servidor
                        serverConfigData = new DataXml(file.FullName.ToString()).ConfigDataP;

                        //actualizamos ultima versión
                        newVersion = serverConfigData.newVersion;

                        break;
                    }

                }
            }
            catch(Exception ex)
            {
                serverReachable = false;

            }
            finally
            {

                //Actualizamos labels
                lblAvailableVersion.Content = newVersion;
                lblIsAvailable.Content = serverReachable.ToString();
            }
            

        }

        #endregion

        #region EVENTS

        protected override void OnClosed(EventArgs e)
        {
            if (IsUpdated)
            {
                //Lanzamos un thread con el formulario de "Cargando contenido" en paralelo si ha sido actualizando
                Thread lThread = new Thread(() =>
                {
                    WPFUpdating updForm = new WPFUpdating();
                    updForm.Show();
                    updForm.waitXSeconds(2);
                    updForm.Close();
                    updForm.Closed += (sender2, e2) => updForm.Dispatcher.InvokeShutdown();
                    System.Windows.Threading.Dispatcher.Run();
                });
                lThread.SetApartmentState(ApartmentState.STA);
                lThread.Start();
            }
                       
        }

        #endregion

    }
}
