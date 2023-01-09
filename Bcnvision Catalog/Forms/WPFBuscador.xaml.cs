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
    /// Lógica de interacción para WPFBuscador.xaml
    /// </summary>
    public partial class WPFBuscador : Window
    {

        #region FIELDS 

        LanguageMode selectedLanguageMode;

        private bool mostrarPags = false;

        string clickedPath = "";

        string clickedFile = "";

        string mainPath = "";

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

        public List<FileInfo> FilesBuscador { get; set; }

        /// <summary>
        /// Ruta actual
        /// </summary>
        public DataXml Data { get; set; }

        public WrapPanel wrap;

        public string NextRuta { get; set; }
        public string NextHeader { get; set; }
        #endregion

        #region CONSTRUCTOR 

        public WPFBuscador(string lastPath, string MainPath, List<string> actualItems, LanguageMode SelectedLanguageMode)
        {
            InitializeComponent();

            //Set de la ruta actual
            PathAct = lastPath;
            mainPath = MainPath;
            
            //Items a mostrar en el buscador
            Items = new List<string>(actualItems);

            selectedLanguageMode = SelectedLanguageMode;

            //Elementos en la pagina
            FilesBuscador = new List<FileInfo>();
            SetWrapPanelPrincipal(mainPath, Items);

            //Resultados
            Titulo.Text = selectedLanguageMode == LanguageMode.spanish ? "Resultados de " : (selectedLanguageMode == LanguageMode.english ? "Results" : "Results");
        }


        #endregion

        #region METHODS 

        private void SetWrapPanelPrincipal(string path, List<string> itemsPanel, bool mostrarPaginasBuscador = false)
        {
            try
            {

                //cogemos el valor para el campo de mostrar Paginas en el buscador: útil para modificar XMLs
                mostrarPags = mostrarPaginasBuscador;

                //Contamos numero de elementos de cada tipo
                int numVideos = 0, numPdfs = 0, numPresentaciones = 0, numHtmls = 0, numPaginas = 0;

                FileInfo[] allFiles = new FileInfo[0];

                for (int i = 0; i < itemsPanel.Count; i++)
                {
                    //Obtenemos los archivos del directorio
                    
                    if (itemsPanel[i].Substring(0, 1) == "\\") itemsPanel[i] = itemsPanel[i].Substring(1, itemsPanel[i].Length - 1);
                    DirectoryInfo dir = Directory.GetParent(itemsPanel[i]);
                    if (dir.Exists)
                    {
                        dir.EnumerateFiles("*");
                        FileInfo[] files = dir.GetFiles();
                        files.OrderBy(f => f.Extension);

                        allFiles = allFiles.Concat(files).ToArray();
                    }
                }


                if (allFiles != null)
                {
                    //Ordenamos por extensión
                    Array.Sort(allFiles, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Extension, y.Extension));
                    List<string> addedFiles = new List<string>();
                    for (int i = 0; i < allFiles.Length; i++)
                    {
                        if (!addedFiles.Contains(allFiles[i].Name))
                        {
                            addedFiles.Add(allFiles[i].Name);

                            if (allFiles[i].Name.Contains(".mp4"))
                            {
                                //Añadimos botón de encabezado si toca
                                if (numVideos == 0)
                                    SetButton(selectedLanguageMode == LanguageMode.spanish ? "Videos" : ( selectedLanguageMode == LanguageMode.english ? "Videos" : "Vidéos"), false);

                                //Añadimos el boton del archivo
                                numVideos++;
                                SetButton(allFiles[i].FullName, true);
                            }
                            else if ((allFiles[i].Name.Contains(".pptx") || allFiles[i].Name.Contains(".ppt")) && !allFiles[i].Name.Contains(".xml"))
                            {
                                //Añadimos botón de encabezado si toca
                                if (numPresentaciones == 0)
                                    SetButton(selectedLanguageMode == LanguageMode.spanish ? "Presentaciones" : (selectedLanguageMode == LanguageMode.english ? "Presentations" : "Présentations"), false);

                                //Añadimos el boton del archivo
                                numPresentaciones++;
                                SetButton(allFiles[i].FullName, true);
                            }
                            else if (allFiles[i].Name.Contains(".pdf") && !allFiles[i].Name.Contains(".xml"))
                            {
                                //Añadimos botón de encabezado si toca
                                if (numPdfs == 0)
                                    SetButton(selectedLanguageMode == LanguageMode.spanish ? "Documentos" : (selectedLanguageMode == LanguageMode.english ? "Documents" : "Documents"), false);

                                //Añadimos el boton del archivo
                                numPdfs++;
                                SetButton(allFiles[i].FullName, true);
                            }
                            else if (allFiles[i].Name.Contains(".html") && !allFiles[i].Name.Contains(".xml"))
                            {
                                //Añadimos botón de encabezado si toca
                                if (numHtmls == 0)
                                    SetButton(selectedLanguageMode == LanguageMode.spanish ? "Informes" : (selectedLanguageMode == LanguageMode.english ? "Reports" : "Rapports"), false);

                                //Añadimos el boton del archivo
                                numHtmls++;
                                SetButton(allFiles[i].FullName, true);
                            }
                            else if (allFiles[i].Name.Contains(".xml") && mostrarPags)
                            {
                                //Añadimos botón de encabezado si toca
                                if (numPaginas == 0)
                                    SetButton("Páginas", false);

                                numPaginas++;

                                //Nos quedamos con el nombre de la página
                                FileInfo fileXML = new FileInfo(allFiles[i].FullName);
                                SetButton(fileXML.FullName, true);
                            }
                        }
                    }
                }                    
            }
            catch(Exception ex)
            {

            }        
        }

        private void SetButton(string text, bool isEnabled)
        {
            Button btnModel = new Button();

            btnModel.Margin = new Thickness(50, 0, 0, 0);
            //btnModel.Width = 1150;
            
            btnModel.Width = ((System.Windows.Controls.ScrollViewer)this.wrapPanelContent.Parent).ActualWidth - btnModel.ActualWidth;
            if (btnModel.ActualWidth <= 0) btnModel.Width = 900;
            //btnModel.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (File.Exists(text))
            {
                FileInfo file = new FileInfo(text);
                FilesBuscador.Add(file);
                btnModel.Content = file.Name.Replace(file.Extension, "");
            }
            else
                btnModel.Content = text;

            btnModel.HorizontalAlignment = HorizontalAlignment.Left;
            btnModel.Focusable = isEnabled;

            if (!isEnabled)
            {
                btnModel.Margin = new Thickness(50, 10, 0, 0);
                btnModel.Height = 60;
                btnModel.FontSize = 40;
                btnModel.Background = Brushes.LightGray;
                btnModel.BorderBrush = Brushes.Transparent;
                btnModel.Foreground = Brushes.Black;
                btnModel.Focusable = false;
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
        }

        #endregion

        #region EVENTS 
        //Declaración del evento
        public event EventHandler PathClicked;

        private void FileButton_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                FileInfo f = FilesBuscador.Find(i => i.Name == btn.Content.ToString() + i.Extension);

                string path = mainPath + btn.Content.ToString();

                if (File.Exists(f.FullName))
                {
                    if (!f.FullName.Contains(".xml"))
                    {
                        Process.Start(f.FullName);  

                    }
                    else
                    {
                        NextRuta = f.DirectoryName;
                        NextHeader = f.Directory.Name;
                        
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