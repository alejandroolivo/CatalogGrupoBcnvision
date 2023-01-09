using Bcnvision_Catalog.Classes;
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
using Path = System.IO.Path;

namespace Bcnvision_Catalog.Forms
{
    /// <summary>
    /// Lógica de interacción para WPFExplorer.xaml
    /// </summary>
    public partial class WPFExplorer : Window
    {

        #region FIELDS 

        int pageNumber = 0;

        int totalPages;

        DataXml data;

        #endregion

        #region PROPPERTIES 

        public CompanyMode SelectedCompanyMode;
        public LanguageMode SelectedLanguageMode;

        /// <summary>
        /// Ruta actual
        /// </summary>
        public string PathAct { get; set; }
        /// <summary>
        /// Ruta actual
        /// </summary>
        public string PathMain { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public string LastCardSelectedPath { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public string LastCardSelectedHeader { get; set; }

        /// <summary>
        /// Ruta de la imagen a mostrar por defecto en tarjetas
        /// </summary>
        public string DefaultCardImagePath { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public List<string> Folders { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public List<string> Items { get; set; }

        public List<string> FolderNamesWrapPanel { get; set; }
        public List<string> FolderTitlesWrapPanel { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public DataXml Data { get; set; }

        public string NextRuta { get; set; }

        public string NextHeader { get; set; }

        public WrapPanel wrap;
        #endregion

        #region CONSTRUCTOR 

        public WPFExplorer(string ActualPath, string MainPath, List<string> actualFolders, List<string> actualItems /*Folder Names*/, string explorerMode, string defaultCardImagePath, DataXml dataXml, CompanyMode selCompanyMode, LanguageMode selLanguageMode)
        {
            InitializeComponent();
            data = dataXml;

            //Set de la ruta actual
            PathAct = ActualPath;
            PathMain = MainPath;
            LastCardSelectedPath = ActualPath;
            DefaultCardImagePath = defaultCardImagePath;

            SelectedCompanyMode = selCompanyMode;
            SelectedLanguageMode = selLanguageMode;

            int numberOfItems = 0;
            int page = 0 ;

            //Si se pasa lista de carpetas
            if (actualFolders != null)
            {
                Folders = new List<string>(actualFolders);
                numberOfItems = actualFolders.Count;

                //set del número de páginas
                totalPages = Convert.ToInt32(Math.Floor(Convert.ToDouble(numberOfItems) / 6.0)) + 1;

                //mostramos las cards necesarias
                SetCards(numberOfItems, page, actualFolders, defaultCardImagePath);
            }
            
            //set del número de páginas
            totalPages = Convert.ToInt32(Math.Floor(Convert.ToDouble(numberOfItems)/6.0)) + 1;
            SetPage(page);

            //Set de los labels del wrapPanel
            SetWrapPanel(PathAct);


        }


        #endregion

        #region METHODS 

        void SetWrapPanel(string wholePath)
        {
            string progressPath = PathMain;
            string[] split = wholePath.Replace(PathMain + "\\", "").Split(new string[] { "\\" }, 6, StringSplitOptions.RemoveEmptyEntries);

            FolderNamesWrapPanel = new List<string>();
            FolderTitlesWrapPanel = new List<string>();

            Button btnFirstPath = new Button();

            FolderNamesWrapPanel.Add(split[0]);
            progressPath = Path.Combine(progressPath, split[0]);

            btnFirstPath.Margin = new Thickness(30, 0, 0, 0);
            btnFirstPath.Content = split[0].ToUpper();

            FolderTitlesWrapPanel.Add(split[0].ToUpper());

            btnFirstPath.FontSize = 20;
            btnFirstPath.Height = 50;
            btnFirstPath.HorizontalAlignment = HorizontalAlignment.Left;
            btnFirstPath.Background = Brushes.Transparent;
            btnFirstPath.Foreground = Brushes.Black;
            btnFirstPath.BorderBrush = Brushes.Transparent;
            btnFirstPath.Click += PathButton_Clicked;

            wrapPanel.Children.Add(btnFirstPath);

            

            for (int k = 1; k < split.Length; k++)
            {
                //Creamos botón Next
                Button btnNext = new Button();
                #region etc
                btnNext.Margin = new Thickness(5, 0, 0, 0);
                btnNext.Content = "►";
                btnNext.FontSize = 20;
                btnNext.Height = 50;
                btnNext.HorizontalAlignment = HorizontalAlignment.Left;
                btnNext.Background = Brushes.Transparent;
                btnNext.Foreground = Brushes.Black;
                btnNext.BorderBrush = Brushes.Transparent;
                btnNext.Focusable = false;
                #endregion
                //Añadimos botón Next
                wrapPanel.Children.Add(btnNext);

                //Creamos botón de la carpeta
                Button btnPaths = new Button();

                FolderNamesWrapPanel.Add(split[k]);
                progressPath = Path.Combine(progressPath, split[k]);

                //Rellenamos el título
                if (File.Exists(Path.Combine(progressPath, split[k]) + ".xml"))
                {
                    //Deserializamos la traducción
                    DataXml.ItemData titleData = SerializationManager.ReadItemData(Path.Combine(progressPath, split[k]) + ".xml");
                    if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(titleData.TituloES == "" ? split[k] : titleData.TituloES);
                    else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(titleData.TituloIN == "" ? split[k] : titleData.TituloIN);
                    else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(titleData.TituloFR == "" ? split[k] : titleData.TituloFR);
                    else FolderTitlesWrapPanel[k] = split[k];
                }
                else if(split[k].Contains("MENU"))
                {
                    if (split[k] == "MENU1")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu1Header_spanish == "" ? split[k] : data.StyleDataP.Menu1Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu1Header_english == "" ? split[k] : data.StyleDataP.Menu1Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu1Header_french == "" ? split[k] : data.StyleDataP.Menu1Header_french);

                    }
                    else if (split[k] == "MENU2")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu2Header_spanish == "" ? split[k] : data.StyleDataP.Menu2Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu2Header_english == "" ? split[k] : data.StyleDataP.Menu2Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu2Header_french == "" ? split[k] : data.StyleDataP.Menu2Header_french);

                    }
                    else if (split[k] == "MENU3")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu3Header_spanish == "" ? split[k] : data.StyleDataP.Menu3Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu3Header_english == "" ? split[k] : data.StyleDataP.Menu3Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu3Header_french == "" ? split[k] : data.StyleDataP.Menu3Header_french);

                    }
                    else if (split[k] == "MENU4")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu4Header_spanish == "" ? split[k] : data.StyleDataP.Menu4Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu4Header_english == "" ? split[k] : data.StyleDataP.Menu4Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(data.StyleDataP.Menu4Header_french == "" ? split[k] : data.StyleDataP.Menu4Header_french);

                    }

                }
                else
                {
                    FolderTitlesWrapPanel.Add(split[k]);
                }

                btnPaths.Content = FolderTitlesWrapPanel[k];


                btnPaths.Margin = new Thickness(5, 0, 0, 0);
                //btnPaths.Content = split[k];
                btnPaths.FontSize = 20;
                btnPaths.Height = 50;
                btnPaths.HorizontalAlignment = HorizontalAlignment.Left;
                btnPaths.Background = Brushes.Transparent;
                btnPaths.Foreground = Brushes.Black;
                btnPaths.BorderBrush = Brushes.Transparent;
                btnPaths.Click += PathButton_Clicked;
                btnPaths.Focusable = true;

                //Añadimos el botón
                wrapPanel.Children.Add(btnPaths);

            }

        }

        void SetPage(int pageNmb)
        {
            //Set del numero de página
            pageNumber = pageNmb;

            //flecha derecha
            if (pageNmb >= totalPages - 1)
            {
                rightArrow.IsEnabled = false;
                rightArrow.Opacity = 0.4;
            }
            else
            {
                rightArrow.IsEnabled = true;
                rightArrow.Opacity = 1;
            }

            //flecha izquierda
            if (pageNmb <= 0)
            {
                leftArrow.IsEnabled = false;
                leftArrow.Opacity = 0.4;
            }
            else
            {
                leftArrow.IsEnabled = true;
                leftArrow.Opacity = 1;
            }


            //Progreso de pagina
            progressPage.Maximum = totalPages;
            progressPage.Value = pageNmb + 1;
            
        }

        private static string[] GetFilteredFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }

        void SetCards(int numberItems, int actualPage, List<string> actualFolderPath, string defaultImage)
        {
            try
            {
                int cardsPerPage = 6;
                Controls.FolderCard[] cards = new Controls.FolderCard[6] { contentCard00, contentCard01, contentCard02, contentCard03, contentCard04, contentCard05 };

                //Editamos cada tarjeta de contenido y mostramos si hay item para ella
                for (int i = 0; i < cardsPerPage; i++)
                {
                    if (Folders.Count > cardsPerPage * actualPage + i)
                    {
                        //Asignamos contenido a mostrar en tarjeta
                        
                        //Buscamos XML con mismo título en cada carpeta para ver si hay que cambiar el título
                        cards[i].lblTitulo.Text = Folders[cardsPerPage * actualPage + i];
                        cards[i].folderName = Folders[cardsPerPage * actualPage + i];

                        string xmlPath = Path.Combine(Path.Combine(PathAct, Folders[cardsPerPage * actualPage + i]), Folders[cardsPerPage * actualPage + i] + ".xml");

                        if (File.Exists(xmlPath))
                        {
                            DataXml.ItemData folderXml = SerializationManager.ReadItemData(xmlPath);

                            if (SelectedLanguageMode == LanguageMode.spanish && !string.IsNullOrEmpty(folderXml.TituloES))
                                cards[i].lblTitulo.Text = folderXml.TituloES;
                            else if (SelectedLanguageMode == LanguageMode.french && !string.IsNullOrEmpty(folderXml.TituloFR))
                                cards[i].lblTitulo.Text = folderXml.TituloFR;
                            else if (SelectedLanguageMode == LanguageMode.english && !string.IsNullOrEmpty(folderXml.TituloIN))
                                cards[i].lblTitulo.Text = folderXml.TituloIN;
                        }

                        cards[i].g.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(SelectedCompanyMode == CompanyMode.bcnvision ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));
                        cards[i].lblTitulo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(SelectedCompanyMode == CompanyMode.bcnvision ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));
                        
                        //Mostramos imagen contenida en carpeta, si la hay
                        string rutaCarpeta = Path.Combine(PathAct, actualFolderPath[cardsPerPage * actualPage + i]);

                        string[] imgs;
                        imgs = GetFilteredFiles(rutaCarpeta, "*.png|*.jpg", SearchOption.TopDirectoryOnly);

                        if (imgs.Length == 0)
                        {
                            imgs = new string[1] { defaultImage };
                        }
                        
                        if (imgs.Length > 0)
                        {
                            //Set de la imagen de la tarjeta
                            for (int j = 0; j < imgs.Length;j++)
                                    cards[i].cardImage.Source = new BitmapImage(new Uri(imgs[j], UriKind.Absolute));
                        }
                        else
                        {
                            //Set de la imagen de la tarjeta 
                            cards[i].cardImage.Source = new BitmapImage(new Uri(DefaultCardImagePath, UriKind.Absolute));
                        }


                        //Mostramos la tarjeta
                        cards[i].Opacity = 1;
                        cards[i].IsEnabled = true;
                    }
                    else
                    {
                        //Hacemos a tarjeta invisible y la deshabilitamos
                        cards[i].Opacity = 0;
                        cards[i].IsEnabled = false;

                    }
                }

            }
            catch (Exception ex)
            {

            }
            


        }

        #endregion

        #region EVENTS 
        
        //Declaración del evento
        public event EventHandler ContentCardClicked;

        //Declaración del evento
        public event EventHandler PathClicked;

        private void PathButton_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                //Guardamos la ruta a la que nos vamos
                for (int i = 0; i < wrapPanel.Children.Count; i++)
                {
                    Button btn = wrapPanel.Children[i].IsFocused == true ? (Button)wrapPanel.Children[i] : null;


                    if (btn != null)
                    {
                        string folderNameFromTitle = FolderNamesWrapPanel[FolderTitlesWrapPanel.IndexOf(btn.Content.ToString())];

                        NextRuta = PathAct.Substring(0, PathAct.IndexOf(folderNameFromTitle) + folderNameFromTitle.Length);
                        NextHeader = folderNameFromTitle;

                        //Invocamos el evento
                        PathClicked?.Invoke(sender, e);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            SetPage(++pageNumber);

            SetCards(Folders.Count, pageNumber, Folders, DefaultCardImagePath);
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            SetPage(--pageNumber);

            SetCards(Folders.Count, pageNumber, Folders, DefaultCardImagePath);
        }

        private void ContentCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                
                //Invocamos el evento
                ContentCardClicked?.Invoke(sender, e);

            }
            catch (Exception ex)
            {
                
            }


        }


        private void ContentCard00_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[0 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard00.folderName;
            }
            catch (Exception ex)
            {


            }

        }

        private void ContentCard01_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[1 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard01.folderName;
            }
            catch (Exception ex)
            {


            }
        }

        private void ContentCard02_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[2 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard02.folderName;
            }
            catch (Exception ex)
            {


            }
        }

        private void ContentCard03_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[3 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard03.folderName;
            }
            catch (Exception ex)
            {


            }
        }

        private void ContentCard04_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[4 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard04.folderName;
            }
            catch (Exception ex)
            {


            }
        }

        private void ContentCard05_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                LastCardSelectedPath = Path.Combine(PathAct, Folders[5 + pageNumber * 6]);
                LastCardSelectedHeader = contentCard05.folderName;
            }
            catch (Exception ex)
            {


            }
        }

        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
        }
            #endregion

    }
}
