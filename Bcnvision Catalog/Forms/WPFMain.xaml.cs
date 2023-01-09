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
using Bcnvision_Catalog.Classes;
using Bcnvision_Catalog.Controls;
using Path = System.IO.Path;

namespace Bcnvision_Catalog.Forms
{
    /// <summary>
    /// Lógica de interacción para WPFMain.xaml
    /// </summary>
    public partial class WPFMain : Window
    {
        #region ENUM

        private enum Menu
        {
            Productos,
            Aplicaciones,
            Presentaciones,
            Informes
        };

        #endregion

        #region CONSTRUCTOR

        public WPFMain(CompanyMode company, DataXml dataXml, string mainPath)
        {
            InitializeComponent();
            data = dataXml;
            MainPath = mainPath;

            //Set del modo de empresa
            SetCompanyMode(company);
            colorPrincipal = new SolidColorBrush((Color)ColorConverter.ConvertFromString(SelectedCompanyName == CompanyMode.bcnvision ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));

            //Set del idioma
            SelectecLanguage = data.ConfigDataP.idioma == "spanish" ? LanguageMode.spanish : (data.ConfigDataP.idioma == "english" ? LanguageMode.english : LanguageMode.french);

            //Iniciamos por defecto con el menú 1 mostrándose en pantalla
            SetProductosButton();
            

        }

        #endregion

        #region FIELDS

        string MainPath;

        Menu selectedMenu;

        SolidColorBrush colorPrincipal;

        /// <summary>
        /// Boton productos
        /// </summary>
        ProductosButton ProdButton;

        /// <summary>
        /// Boton Presentaciones
        /// </summary>
        PresentacionesButton PresButton;

        /// <summary>
        /// Boton Aplicaciones
        /// </summary>
        ApplicationsButton AppButton;

        /// <summary>
        /// Boton Aplicaciones
        /// </summary>
        InformesButton InforButton;

        CompanyMode SelectedCompanyName;

        LanguageMode SelectecLanguage;
        
        DataXml data;

        #endregion

        #region PROPERTIES



        #endregion

        #region METHODS

        public void SetCompanyMode(CompanyMode companyMode)
        {
            //Si ya es el modo actual, pasamos
            if (SelectedCompanyName == companyMode)
                return;
            else
            {
                //Cambiamos modo:
                SelectedCompanyName = companyMode;

            }
        }
        
        public void SetProductosButton()
        {
            try
            {
                //Limpiamos el contenido previo
                _gridLayoutMain.Children.Clear();

                //Asignamos la pantalla Main al Grid
                ProdButton = new ProductosButton();

                //Pasamos la imagen contenida en carpeta, si la hay
                string rutaCarpeta = Path.Combine(MainPath, "MENU1");

                string[] imgs;
                imgs = GetFilteredFiles(rutaCarpeta, "*.png|*.jpg", SearchOption.TopDirectoryOnly);

                if (imgs.Length > 0)
                {
                    for (int j = 0; j < imgs.Length; j++)
                        ProdButton.imagePrincipal.Source = new BitmapImage(new Uri(imgs[j], UriKind.Absolute));
                }

                ProdButton.Borde.Background = colorPrincipal;
                ProdButton.header.Foreground = colorPrincipal;
                ProdButton.Line1.Foreground = colorPrincipal;
                ProdButton.Line2.Foreground = colorPrincipal;

                ProdButton.header.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu1Header_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu1Header_english : data.StyleDataP.Menu1Header_french);

                ProdButton.Line1.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu1DescriptionLine1_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu1DescriptionLine1_english : data.StyleDataP.Menu1DescriptionLine1_french);

                ProdButton.Line2.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu1DescriptionLine2_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu1DescriptionLine2_english : data.StyleDataP.Menu1DescriptionLine2_french);

                //Añadimos el contenido nuevo
                object MainButtonContent = ProdButton.Content;
                ProdButton.Content = null;

                //Guardamos el tipo de menu seleccionado
                selectedMenu = Menu.Productos;

                _gridLayoutMain.Children.Add(MainButtonContent as UIElement);
            }
            catch (Exception ex)
            {

            }
        }

        public void SetApplicationsButton()
        {
            try
            {
                //Limpiamos el contenido previo
                _gridLayoutMain.Children.Clear();

                //Asignamos la pantalla Main al Grid
                AppButton = new ApplicationsButton();

                //Pasamos la imagen contenida en carpeta, si la hay
                string rutaCarpeta = Path.Combine(MainPath, "MENU2");

                string[] imgs;
                imgs = GetFilteredFiles(rutaCarpeta, "*.png|*.jpg", SearchOption.TopDirectoryOnly);

                if (imgs.Length > 0)
                {
                    for (int j = 0; j < imgs.Length; j++)
                        AppButton.imagePrincipal.Source = new BitmapImage(new Uri(imgs[j], UriKind.Absolute));
                }

                AppButton.Borde.Background = colorPrincipal;
                AppButton.header.Foreground = colorPrincipal;
                AppButton.Line1.Foreground = colorPrincipal;
                AppButton.Line2.Foreground = colorPrincipal;

                AppButton.header.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu2Header_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu2Header_english : data.StyleDataP.Menu2Header_french);

                AppButton.Line1.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu2DescriptionLine1_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu2DescriptionLine1_english : data.StyleDataP.Menu2DescriptionLine1_french);

                AppButton.Line2.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu2DescriptionLine2_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu2DescriptionLine2_english : data.StyleDataP.Menu2DescriptionLine2_french);

                //Añadimos el contenido nuevo
                object MainButtonContent = AppButton.Content;
                AppButton.Content = null;

                //Guardamos el tipo de menu seleccionado
                selectedMenu = Menu.Aplicaciones;

                _gridLayoutMain.Children.Add(MainButtonContent as UIElement);
            }
            catch (Exception ex)
            {

            }
        }

        public void SetPresentacionesButton()
        {
            try
            {
                //Limpiamos el contenido previo
                _gridLayoutMain.Children.Clear();

                //Asignamos la pantalla Main al Grid
                PresButton = new PresentacionesButton();

                //Pasamos la imagen contenida en carpeta, si la hay
                string rutaCarpeta = Path.Combine(MainPath, "MENU3");

                string[] imgs;
                imgs = GetFilteredFiles(rutaCarpeta, "*.png|*.jpg", SearchOption.TopDirectoryOnly);

                if (imgs.Length > 0)
                {
                    for (int j = 0; j < imgs.Length; j++)
                        PresButton.imagePrincipal.Source = new BitmapImage(new Uri(imgs[j], UriKind.Absolute));
                }

                PresButton.Borde.Background = colorPrincipal;
                PresButton.header.Foreground = colorPrincipal;
                PresButton.Line1.Foreground = colorPrincipal;
                PresButton.Line2.Foreground = colorPrincipal;

                PresButton.header.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu3Header_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu3Header_english : data.StyleDataP.Menu3Header_french);

                PresButton.Line1.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu3DescriptionLine1_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu3DescriptionLine1_english : data.StyleDataP.Menu3DescriptionLine1_french);

                PresButton.Line2.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu3DescriptionLine2_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu3DescriptionLine2_english : data.StyleDataP.Menu3DescriptionLine2_french);


                //Añadimos el contenido nuevo
                object MainButtonContent = PresButton.Content;
                PresButton.Content = null;

                //Guardamos el tipo de menu seleccionado
                selectedMenu = Menu.Presentaciones;

                _gridLayoutMain.Children.Add(MainButtonContent as UIElement);
            }
            catch (Exception ex)
            {

            }
        }

        public void SetInformesButton()
        {
            try
            {
                //Limpiamos el contenido previo
                _gridLayoutMain.Children.Clear();

                //Asignamos la pantalla Main al Grid
                InforButton = new InformesButton();

                //Pasamos la imagen contenida en carpeta, si la hay
                string rutaCarpeta = Path.Combine(MainPath, "MENU4");

                string[] imgs;
                imgs = GetFilteredFiles(rutaCarpeta, "*.png|*.jpg", SearchOption.TopDirectoryOnly);

                if (imgs.Length > 0)
                {
                    for (int j = 0; j < imgs.Length; j++)
                        InforButton.imagePrincipal.Source = new BitmapImage(new Uri(imgs[j], UriKind.Absolute));
                }

                InforButton.Borde.Background = colorPrincipal;
                InforButton.header.Foreground = colorPrincipal;
                InforButton.Line1.Foreground = colorPrincipal;
                InforButton.Line2.Foreground = colorPrincipal;

                InforButton.header.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu4Header_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu4Header_english : data.StyleDataP.Menu4Header_french);

                InforButton.Line1.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu4DescriptionLine1_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu4DescriptionLine1_english : data.StyleDataP.Menu4DescriptionLine1_french);

                InforButton.Line2.Content = SelectecLanguage == LanguageMode.spanish ? data.StyleDataP.Menu4DescriptionLine2_spanish :
                                            (SelectecLanguage == LanguageMode.english ? data.StyleDataP.Menu4DescriptionLine2_english : data.StyleDataP.Menu4DescriptionLine2_french);

                //Añadimos el contenido nuevo
                object MainButtonContent = InforButton.Content;
                InforButton.Content = null;

                //Guardamos el tipo de menu seleccionado
                selectedMenu = Menu.Informes;

                _gridLayoutMain.Children.Add(MainButtonContent as UIElement);
            }
            catch (Exception ex)
            {

            }
        }

        public void EnterMenu(object sender, MouseButtonEventArgs e)
        {
            try
            {

                System.Threading.Thread.Sleep(50);

                switch (selectedMenu)
                {
                    case Menu.Productos:

                        //Invocamos el evento
                        BtnProductosClicked?.Invoke(sender, e);

                        break;

                    case Menu.Aplicaciones:

                        //Invocamos el evento
                        BtnAplicacionesClicked?.Invoke(sender, e);

                        break;

                    case Menu.Presentaciones:

                        //Invocamos el evento
                        BtnPresentacionesClicked?.Invoke(sender, e);

                        break;

                    case Menu.Informes:

                        //Invocamos el evento
                        BtnInformesClicked?.Invoke(sender, e);

                        break;

                    default:

                        break;

                }
            }
            catch (Exception ex)
            {

            }
        }

        private static string[] GetFilteredFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }
        #endregion

        #region EVENTS

        //Declaración del evento
        public event EventHandler BtnAplicacionesClicked;

        //Declaración del evento
        public event EventHandler BtnProductosClicked;

        //Declaración del evento
        public event EventHandler BtnPresentacionesClicked;

        //Declaración del evento
        public event EventHandler BtnInformesClicked;
        
        //EVENTOS DE MOUSE OVER
        
        private void BtnProductos_MouseEnter(object sender, MouseEventArgs e)
        {
            //Si el modo el PC, pasar por encima activa la visualización del botón
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
                SetProductosButton();

        }

        private void BtnAplicaciones_MouseEnter(object sender, MouseEventArgs e)
        {
            //Si el modo el PC, pasar por encima activa la visualización del botón
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
                SetApplicationsButton();
        }

        private void BtnPresentaciones_MouseEnter(object sender, RoutedEventArgs e)
        {
            //Si el modo el PC, pasar por encima activa la visualización del botón
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
                SetPresentacionesButton();
        }
        
        private void BtnInformes_MouseEnter(object sender, MouseEventArgs e)
        {
            //Si el modo el PC, pasar por encima activa la visualización del botón
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
                SetInformesButton();
        }
        
        //EVENTOS DE SINGLE CLICK

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            //Si el modo el PC, al pulsar se entra al menú
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
            {
                //Invocamos el evento
                BtnProductosClicked?.Invoke(sender, e);
            }
            else
            {
                SetProductosButton();
            }

        }

        private void BtnAplicaciones_Click(object sender, RoutedEventArgs e)
        {
            //Si el modo el PC, al pulsar se entra al menú
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
            {
                //Invocamos el evento
                BtnAplicacionesClicked?.Invoke(sender, e);
            }
            else
            {
                SetApplicationsButton();
            }

        }

        private void BtnPresentaciones_Click(object sender, RoutedEventArgs e)
        {
            //Si el modo el PC, al pulsar se entra al menú
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
            {
                //Invocamos el evento
                BtnPresentacionesClicked?.Invoke(sender, e);
            }
            else
            {
                SetPresentacionesButton();
            }
        }

        private void BtnInformes_Click(object sender, RoutedEventArgs e)
        {
            //Si el modo el PC, al pulsar se entra al menú
            if (data.ConfigDataP.isSurfaceOrPC.ToLower() == "pc")
            {
                //Invocamos el evento
                BtnInformesClicked?.Invoke(sender, e);
            }
            else
            {
                SetInformesButton();

            }
        }

        private void _gridLayoutMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EnterMenu(sender, e);
        }

        //EVENTOS DE DOBLE CLICK    

        private void BtnProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnterMenu(sender, e);
        }

        private void BtnAplicaciones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnterMenu(sender, e);
        }

        private void BtnPresentaciones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnterMenu(sender, e);
        }

        private void BtnInformes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EnterMenu(sender, e);
        }
        #endregion

    }
}
