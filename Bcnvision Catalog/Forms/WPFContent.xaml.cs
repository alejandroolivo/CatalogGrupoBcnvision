using Bcnvision_Catalog.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Lógica de interacción para WPFContent.xaml
    /// </summary>
    public partial class WPFContent : Window
    {

        #region FIELDS 

        string clickedPath = "";

        string clickedFile = "";

        string mainPath = "";

        DataXml.ConfigData data;

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
        public string LastCardSelectedHeader { get; set; }

        /// <summary>
        /// Ruta de la imagen a mostrar por defecto en tarjetas
        /// </summary>
        public string ImagePath { get; set; }
        
        /// <summary>
        /// Ruta actual
        /// </summary>
        public List<string> Items { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public DataXml Data { get; set; }

        private DataXml.StyleData styleData;

        public WrapPanel wrap;
        
        public List<string> FolderNamesWrapPanel { get; set; }
        public List<string> FolderTitlesWrapPanel { get; set; }

        public string NextRuta { get; set; }
        public string NextHeader { get; set; }
        public string NextRutaVideo { get; set; }
        #endregion

        #region CONSTRUCTOR 

        public WPFContent(string ActualPath, string MainPath, List<string> actualItems, string cardImagePath, DataXml Data, CompanyMode selCompanyMode, LanguageMode selLanguageMode)
        {
            InitializeComponent();

            //Set de la ruta actual
            PathAct = ActualPath;
            mainPath = MainPath;
            data = Data.ConfigDataP;
            styleData = Data.StyleDataP;
            
            SelectedCompanyMode = selCompanyMode;
            SelectedLanguageMode = selLanguageMode;

            //Ponemos la image 
            ImagePath = cardImagePath;
            folderImage.Source = new BitmapImage(new Uri(cardImagePath, UriKind.Absolute));

            //Items en la carpeta
            Items = new List<string>(actualItems);

            //Elementos en la pagina
            SetWrapPanelPrincipal(ActualPath);

            //Set de los labels del wrapPanel
            SetWrapPanel(PathAct);

        }

        #endregion

        #region METHODS 

        void SetWrapPanelPrincipal(string path)
        {
            //Obtenemos los archivos del directorio
            DirectoryInfo dir = new DirectoryInfo(path);
            dir.EnumerateFiles("*");
            FileInfo[] files = dir.GetFiles();
            files.OrderBy(f => f.Extension);

            //Cambiar idiomas de tipos de archivo
            string videosStr = data.idioma == LanguageMode.spanish.ToString() ? "Vídeos" : (data.idioma == LanguageMode.english.ToString() ? "Videos" : "Vidéos");
            string presentacionesStr = data.idioma == LanguageMode.spanish.ToString() ? "Presentaciones" : (data.idioma == LanguageMode.english.ToString() ? "Presentations" : "Présentations");
            string documentosStr = data.idioma == LanguageMode.spanish.ToString() ? "Documentos" : (data.idioma == LanguageMode.english.ToString() ? "Documents" : "Documents");
            string informesStr = data.idioma == LanguageMode.spanish.ToString() ? "Informes" : (data.idioma == LanguageMode.english.ToString() ? "Reports" : "Rapports");
                       
            //Contamos numero de elementos de cada tipo
            int numVideos = 0, numPdfs = 0, numPresentaciones = 0, numHtmls = 0;
            
            for (int i=0; i < files.Length; i++)
            {
                if (!files[i].FullName.Contains(".xml"))
                {
                    if (files[i].FullName.Contains(".mp4"))
                    {
                        //Añadimos botón de encabezado si toca
                        if (numVideos == 0)
                            SetButton(videosStr, false);

                        //Añadimos el boton del archivo
                        numVideos++;
                        SetButton(files[i].Name, true);
                    }
                    else if (files[i].FullName.Contains(".pptx") || files[i].FullName.Contains(".ppt"))
                    {
                        //Añadimos botón de encabezado si toca
                        if (numPresentaciones == 0)
                            SetButton(presentacionesStr, false);

                        //Añadimos el boton del archivo
                        numPresentaciones++;
                        SetButton(files[i].Name, true);
                    }
                    else if (files[i].FullName.Contains(".pdf"))
                    {
                        //Añadimos botón de encabezado si toca
                        if (numPdfs == 0)
                            SetButton(documentosStr, false);

                        //Añadimos el boton del archivo
                        numPdfs++;
                        SetButton(files[i].Name, true);
                    }
                    if (files[i].FullName.Contains(".html"))
                    {
                        //Añadimos botón de encabezado si toca
                        if (numHtmls == 0)
                            SetButton(informesStr, false);

                        //Añadimos el boton del archivo
                        numHtmls++;
                        SetButton(files[i].Name, true);
                    }
                }
                
            }

            //Set del wrap panel a pie de página
            wrap = wrapPanel;

        }

        void SetButton(string text, bool isEnabled)
        {
            Button btnModel = new Button();

            btnModel.Margin = new Thickness(50, 0, 0, 0);
            btnModel.Content = text;
            btnModel.HorizontalAlignment = HorizontalAlignment.Stretch;
            btnModel.Width = 1000;
            btnModel.Focusable = isEnabled;

            if (!isEnabled)
            {
                btnModel.Margin = new Thickness(50, 10, 0, 0);
                btnModel.Height = 50;
                btnModel.FontSize = 40;
                btnModel.Background = Brushes.LightGray;
                btnModel.BorderBrush = Brushes.Transparent;
                btnModel.Foreground = Brushes.Black;
            }
            else
            {
                btnModel.Margin = new Thickness(50, 0, 0, 0);

                btnModel.Height = 50;
            btnModel.FontSize = 25;
                btnModel.Background = Brushes.Transparent;
                btnModel.BorderBrush = Brushes.LightGray;
                btnModel.Foreground = Brushes.Black;
            }

            wrapPanelContent.Children.Add(btnModel);

            btnModel.Click += FileButton_Clicked;
            btnModel.MouseRightButtonUp += FileButton_RightClicked;
        }


        void SetWrapPanel(string wholePath)
        {
            string progressPath = mainPath;
            string[] split = wholePath.Replace(mainPath + "\\", "").Split(new string[] { "\\" }, 6, StringSplitOptions.RemoveEmptyEntries);

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
                else if (split[k].Contains("MENU"))
                {
                    if (split[k] == "MENU1")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(styleData.Menu1Header_spanish == "" ? split[k] : styleData.Menu1Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(styleData.Menu1Header_english == "" ? split[k] : styleData.Menu1Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(styleData.Menu1Header_french == "" ? split[k] : styleData.Menu1Header_french);

                    }
                    else if (split[k] == "MENU2")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(styleData.Menu2Header_spanish == "" ? split[k] : styleData.Menu2Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(styleData.Menu2Header_english == "" ? split[k] : styleData.Menu2Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(styleData.Menu2Header_french == "" ? split[k] : styleData.Menu2Header_french);

                    }
                    else if (split[k] == "MENU3")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(styleData.Menu3Header_spanish == "" ? split[k] : styleData.Menu3Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(styleData.Menu3Header_english == "" ? split[k] : styleData.Menu3Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(styleData.Menu3Header_french == "" ? split[k] : styleData.Menu3Header_french);

                    }
                    else if (split[k] == "MENU4")
                    {

                        if (SelectedLanguageMode == LanguageMode.spanish) FolderTitlesWrapPanel.Add(styleData.Menu4Header_spanish == "" ? split[k] : styleData.Menu4Header_spanish);
                        else if (SelectedLanguageMode == LanguageMode.english) FolderTitlesWrapPanel.Add(styleData.Menu4Header_english == "" ? split[k] : styleData.Menu4Header_english);
                        else if (SelectedLanguageMode == LanguageMode.french) FolderTitlesWrapPanel.Add(styleData.Menu4Header_french == "" ? split[k] : styleData.Menu4Header_french);

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


        void SetWrapPanel2(string wholePath)
        {

            string[] split = wholePath.Replace(mainPath + "\\", "").Split(new string[] { "\\" }, 6, StringSplitOptions.RemoveEmptyEntries);

            Button btnFirstPath = new Button();

            btnFirstPath.Margin = new Thickness(30, 0, 0, 0);
            btnFirstPath.Content = split[0].ToUpper();
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

                btnNext.Margin = new Thickness(5, 0, 0, 0);
                btnNext.Content = "►";
                btnNext.FontSize = 20;
                btnNext.Height = 50;
                btnNext.HorizontalAlignment = HorizontalAlignment.Left;
                btnNext.Background = Brushes.Transparent;
                btnNext.Foreground = Brushes.Black;
                btnNext.BorderBrush = Brushes.Transparent;
                btnNext.Focusable = false;

                //Añadimos botón Next
                wrapPanel.Children.Add(btnNext);

                //Creamos botón de la carpeta
                Button btnPaths = new Button();

                btnPaths.Margin = new Thickness(5, 0, 0, 0);
                btnPaths.Content = split[k];
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


        #endregion

        #region EVENTS 

        //Declaración del evento
        public event EventHandler FileClicked;
        public event EventHandler VideoClicked;
        public event EventHandler DocumentClicked;

        private void FileButton_Clicked(object sender, RoutedEventArgs e)
        {

            try
            {
                Button btn = (Button)sender;

                //Ruta completa del archivo
                string path = Path.Combine(PathAct, btn.Content.ToString());

                if (!path.Contains(".mp4") && !path.Contains(".pdf"))
                {
                    //Si el archivo no es un video
                    Process.Start(path);
                }
                else if(path.Contains(".mp4"))
                {
                    //Si es un video iniciamos el formulario WPFHost
                    NextRutaVideo = path;
                    NextHeader = btn.Content.ToString();

                    //Invocamos el evento
                    VideoClicked?.Invoke(sender, e);

                }
                else if (path.Contains(".pdf") || path.Contains(".html"))
                {
                    NextHeader = btn.Content.ToString();
                    NextRuta = path;

                    //Invocamos el evento
                    DocumentClicked?.Invoke(sender, e);
                }



            }
            catch(Exception ex)
            {

            }

        }

        private void FileButton_RightClicked(object sender, RoutedEventArgs e)
        {

            try
            {
                Button btn = (Button)sender;

                //Ruta completa del archivo
                string path = Path.Combine(PathAct, btn.Content.ToString());

                //Si el archivo no es un video
                Process.Start(path);

            }
            catch (Exception ex)
            {

            }

        }

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
        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
        }

        #endregion

    }
}