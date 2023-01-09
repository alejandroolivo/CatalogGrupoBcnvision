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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bcnvision_Catalog.Forms
{
    /// <summary>
    /// Lógica de interacción para WPFHost.xaml
    /// </summary>
    public partial class WPFHost : Window
    {
        #region ENUM

        public enum CompanyMode
        {
            Bcnvision = 0,
            Nevitec = 1
        };

        public enum Idioma
        {
            Esp = 0,
            Ing = 1,
            Fra = 2
        };


        #endregion

        #region FIELDS
        bool isVideoMode = true;
        public string filePath = "";
        public string folderPath = "";
        public string backHeader = "";
        public string backImage = "";

        #endregion

        #region CONSTRUCTOR
        public WPFHost(bool isVideoOrPdf, string path, string image)
        {
            //Vemos si tendremos un video o un documento a alojar
            InitializeComponent();
            isVideoMode = isVideoOrPdf;
            filePath = path;
            backImage = image;

            FileInfo file = new FileInfo(filePath);
            folderPath = file.Directory.ToString();
            backHeader = (folderPath.Split('\\'))[folderPath.Split('\\').Length - 1];
            
            //Iniciamos contador para el tema del tiempo de reproducción
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            if (isVideoOrPdf)
            {
                //Mostramos botónes de control
                BtnPlay.Visibility = Visibility.Visible;
                BtnStop.Visibility = Visibility.Visible;
                Btn10Mas.Visibility = Visibility.Visible;
                Btn10Menos.Visibility = Visibility.Visible;

                BtnPlay.IsChecked = true;

                PlayVideo();

                videoHostcontrol.Stretch = Stretch.Uniform;
            }
            else
            {
                //Si es un Pdf o un Html nos cargamos el Mediaelement y metemos un browser a saco
                BtnPlay.Visibility = Visibility.Collapsed;
                BtnStop.Visibility = Visibility.Collapsed;
                Btn10Mas.Visibility = Visibility.Collapsed;
                Btn10Menos.Visibility = Visibility.Collapsed;
                videoHostcontrol.Visibility = Visibility.Collapsed;

                stackPrincipal.Children.Clear();

                System.Windows.Controls.WebBrowser browser = new System.Windows.Controls.WebBrowser();

                //Tamao del browser
                browser.Height = stackPrincipal.RenderSize.Height;
                browser.Width = stackPrincipal.RenderSize.Width;

                //Añadimos el control al formulario
                stackPrincipal.Children.Add(browser);

                //añadimos path del archivo
                browser.Navigate(filePath);
                
            }


            //Progreso de pagina
            if (isVideoOrPdf)
            {
                //Mostramos si es video
                progressPage.Visibility = Visibility.Visible;
                lblStatus.Visibility = Visibility.Visible;
            }
            else
            {
                //Ocultamos si es PDF
                progressPage.Visibility = Visibility.Hidden;
                lblStatus.Visibility = Visibility.Hidden;
            }
        }

        #endregion
        
        #region METHODS
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isVideoMode)
                {
                    //Actualizamos contador de tiempo
                    if (videoHostcontrol.Source != null)
                    {
                        videoHostcontrol.Stretch = Stretch.Uniform;

                        if (videoHostcontrol.NaturalDuration.HasTimeSpan)
                            lblStatus.Content = String.Format("{0} / {1}", videoHostcontrol.Position.ToString(@"mm\:ss"), videoHostcontrol.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

                        //Actualizamos progressBar
                        if (videoHostcontrol.NaturalDuration != null)
                        {
                            progressPage.Maximum = videoHostcontrol.NaturalDuration.TimeSpan.Seconds + (videoHostcontrol.NaturalDuration.TimeSpan.Minutes * 60);
                            progressPage.Value = videoHostcontrol.Position.Seconds + (videoHostcontrol.Position.Minutes * 60);
                        }
                    }
                    else
                        lblStatus.Content = "No file selected...";


                    MediaElement media = (MediaElement)stackPrincipal.Children[0];
                    //Tamaño del browser
                    media.Height = _gridLayoutExplorer.RenderSize.Height;
                    media.Width = _gridLayoutExplorer.RenderSize.Width;
                }
                else
                {
                    WebBrowser browser = (WebBrowser)stackPrincipal.Children[0];
                    //Tamaño del browser
                    browser.Height = _gridLayoutExplorer.RenderSize.Height;
                    browser.Width = _gridLayoutExplorer.RenderSize.Width;
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        public void PauseVideo()
        {
            videoHostcontrol.Pause();
        }

        public void PlayVideo()
        {
            videoHostcontrol.Play();
        }

        public void ReloadVideo()
        {
            System.Uri uri = videoHostcontrol.Source;
            videoHostcontrol.Source = null;

            videoHostcontrol.Source = uri;

        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnPlay.IsChecked)
            {
                PlayVideo();

            }
            else
            {
                PauseVideo();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                videoHostcontrol.Stop();
                BtnPlay.IsChecked = false;
            }
            catch(Exception ex)
            {
                ReloadVideo();
            }
        }
        
        private void Btn10Mas_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Btn10Menos_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Btn10Menos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (videoHostcontrol.Position >= TimeSpan.FromSeconds(10))
                {
                    videoHostcontrol.Position -= TimeSpan.FromSeconds(10);
                }
            }
            catch (Exception ex)
            {
                ReloadVideo();
            }
        }

        private void Btn10Mas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                videoHostcontrol.Position += TimeSpan.FromSeconds(10);
            }
            catch (Exception ex)
            {
                ReloadVideo();
            }
        }

        private void VideoHostcontrol_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BtnPlay_Click(BtnPlay, e);
        }
        #endregion

        #region EVENTS 

        //Declaración del evento
        public event EventHandler BtnAtrasClicked;

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            //Invocamos el evento
            BtnAtrasClicked?.Invoke(sender, e);


        }

        #endregion

        
    }
}
