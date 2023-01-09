using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bcnvision_Catalog.Forms;
using Bcnvision_Catalog.Classes;
using Path = System.IO.Path;

namespace Bcnvision_Catalog
{

    #region ENUM

    public enum CompanyMode
    {
        bcnvision = 0,
        nevitec = 1
    };

    public enum LanguageMode
    {
        spanish = 0,
        english = 1,
        french = 2
    };

    public enum ExplorerMode
    {
        Folders = 0,
        Items = 1
    };

    #endregion

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    public partial class MainWindow : Window
    {

        #region CONSTS

        /// <summary>
        /// Mantener el modo de empresa hasta Cierre de la app tras cambio
        /// </summary>
        const bool KeepPrevStyle = true;

        #endregion

        #region CONSTRUCTOR

        public MainWindow()
        {
            try
            {
                //Lol ahí vamo'
                InitializeComponent();

                //TODO - Serializable en el futuro 
                string testDrive = "K";
                string rutaVeracrypt = @"H:\BcnCatalog\ContenidoCatalogo.hc";
                string folderAppConfigUsers = @"H:\BcnCatalog";
                string pwdMK = "bcnvision2022";

                //Preparamos el encriptado de contraseñas
                Icon icono = Properties.Resources.Icon1;
                byte[] mk = BcnCryptography.imageToByteArray(icono.ToBitmap());
                crypt = new BcnCryptography(mk);

                //Lanzamos formulario de inicio, para entrar con user y password
                WPFStart login = new WPFStart();

                //Por defecto se muestra usuario de windows logueado
                login.txtUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(@"BCNVISION\", "");

                bool logged = false;
                string user = "";
                               
                //Inicio de sesión
                do
                {
                    //Mostramos formulario
                    if (login.ShowDialog() == true)
                    {

                        //Obtengo el valor introducido
                        user = (string)login.txtUser.Text;
                        string pwd = (string)login.txtPassword.Password;
                              
                        //Miramos el config con la info de los usuarios
                        DataXml c = new DataXml(Path.Combine(folderAppConfigUsers, "AppConfig.xml"));

                        //Añadir nuevo usuario
                        if (user.Contains("add-"))
                        {
                            //Lanzamos formulario de inicio, para entrar con user y password
                            WPFPasswordMK WPFpwd = new WPFPasswordMK();

                            //Mostramos formulario
                            if (WPFpwd.ShowDialog() == true)
                            {
                                //Contraseña introducida
                                string pw = (string)WPFpwd.txtPassword.Password;

                                if (pw == pwdMK)
                                {
                                    //Contraseña de MK correcta, creamos el usuario
                                    user = user.Replace("add-", "");
                                    currentUser = user;
                                    c.ConfigDataP.users.Add(new DataXml.User(user, crypt.Encrypt(user, pwd)));
                                    c.ExportConfigData(c.ConfigPathXML);
                                }
                                else
                                {
                                    //Contraseña incorrecta, cerramos todo y bloqueamos contenido por si acaso
                                    VeraCryptSharp.VeraCrypt v2 = new VeraCryptSharp.VeraCrypt(@"C:\Program Files\VeraCrypt\VeraCrypt.exe");
                                    v2.DismountAll(); 
                                    this.Close();
                                }
                            }                            
                        }
                        
                        //Miramos que exista y tenga esa contraseña
                        foreach (DataXml.User us in c.ConfigDataP.users)
                        {
                            if (us.Name == user)
                            {
                                logged = crypt.Encrypt(us.Name, pwd) == us.Pwd;
                                break;
                            }
                        }
                        if (!logged)
                        {
                            System.Windows.MessageBox.Show("Este usuario no existe o no ha informado bien la contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                            VeraCryptSharp.VeraCrypt v1 = new VeraCryptSharp.VeraCrypt(@"C:\Program Files\VeraCrypt\VeraCrypt.exe");

                            v1.Dismount("K", true);

                            this.Close();
                        }

                    }
                    else
                    {
                        //Cancel
                        
                        //Bloqueamos contenido
                        VeraCryptSharp.VeraCrypt v3 = new VeraCryptSharp.VeraCrypt(@"C:\Program Files\VeraCrypt\VeraCrypt.exe");
                        v3.Dismount("K", true);

                        //Check nosequé
                        logged = false;

                        //Fin
                        this.Close();
                    }
                } while (!logged);

                //Info de inicio de sesion para Logs

                currentUser = user;
                horaDeInicio = DateTime.Now;

                //Procedemos a la desencriptación del contenido
                VeraCryptSharp.VeraCrypt v = new VeraCryptSharp.VeraCrypt(@"C:\Program Files\VeraCrypt\VeraCrypt.exe");                
                v.Mount(rutaVeracrypt, pwdMK, VeraCryptSharp.Enums.HashAlgorithm.Auto, testDrive, true);                

                //Iniciamos el gestor de rutas y directorios
                string unit_path = testDrive + @":\Contenido Catalogo";
                PathManager = new PathManagement(unit_path);

                //Copiar toda la carpeta de estilo de la K a la H y coger info de ahí, lo que permite hacer la sincronización de los archivos de Estilo
                string styleHFolder = Path.Combine(folderAppConfigUsers, "Estilo");
                if (!Directory.Exists(styleHFolder)) Directory.CreateDirectory(styleHFolder);
                string styleHFile = Path.Combine(styleHFolder, "StyleConfig.xml");

                DirectoryInfo styleKDirInfo = new DirectoryInfo(PathManager.StylePath);
                FileInfo[] filesStyleFolderK = styleKDirInfo.GetFiles();

                foreach(FileInfo file in filesStyleFolderK)
                {
                    file.CopyTo(Path.Combine(styleHFolder, file.Name), true);
                }
                
                //Sustituímos carpeta de Estilo en el Path manager
                PathManager.StylePath = styleHFolder;

                //Cogemos la info
                string configFilePath = Path.Combine(PathManager.ConfigPath, "AppConfig.xml");
                string styleFilePath = Path.Combine(PathManager.StylePath, "StyleConfig.xml");
                string tagsInfoPath = Path.Combine(PathManager.ConfigPath, "TagsInfo.json");

                data = new DataXml(configFilePath, tagsInfoPath, styleFilePath);

                //Guardamos el usuario que ha inciado sesión
                data.ConfigDataP.lastUserLogged = currentUser;
                data.ExportConfigData(data.ConfigPathXML);

                //Update del Log con el inicio de sesión
                UpdateLog(Path.Combine(PathManager.LogsPath, "Logs_" + currentUser + ".xml"), "Info Inicio: " + currentUser + " @ " + DateTime.Now.ToLongTimeString());
                
                //Set del idioma y el modo
                SelectedCompanyName = data.ConfigDataP.empresa == CompanyMode.bcnvision.ToString() ? CompanyMode.bcnvision : CompanyMode.nevitec;
                SelectedLanguageMode = data.ConfigDataP.idioma == "spanish" ? LanguageMode.spanish :
                                    (data.ConfigDataP.idioma == "english" ? LanguageMode.english : LanguageMode.french);
                                                
                //Iniciamos la app
                mainGrid = _grid;
                cmbBuscar.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                          new System.Windows.Controls.TextChangedEventHandler(ComboBox_TextChanged));


                //Ruta actual = principal
                PathManager.SetExplorerView(SelectedCompanyName.ToString(), true);

                //Añadimos el formulario Main al MainWindow
                SetWPFMain(true);

                //Finalmente vamos a actualizar los tags
                data.TagsDataP = ExplorarTagsToJsonFile();
                data.ExportTagsData(data.TagsPathXML);

            }
            catch (Exception ex)
            {

            }
                     
        }
        #endregion

        #region FIELDS

        private BcnCryptography crypt;

        /// <summary>
        /// Usuario usando la aplicacion y correctamente Logueado
        /// </summary>
        private string currentUser;

        /// <summary>
        /// Hora a la que se inicia la aplicación
        /// </summary>
        private DateTime horaDeInicio;
        
        /// <summary>
        /// Gestor de rutas
        /// </summary>
        private PathManagement PathManager;

        /// <summary>
        /// Datos de la aplicación: config, tags e items
        /// </summary>
        private DataXml data;

        /// <summary>
        /// Formulario pantalla Main
        /// </summary>
        private static WPFMain WPFMainForm;

        /// <summary>
        /// Formulario pantalla Explorer
        /// </summary>
        private static WPFExplorer WPFExplorerForm;

        /// <summary>
        /// Formulario pantalla Content
        /// </summary>
        private static WPFContent WPFContentForm;

        /// <summary>
        /// Formulario pantalla Buscador
        /// </summary>
        private static WPFBuscador WPFBuscadorForm;

        /// <summary>
        /// Formulario pantalla Host
        /// </summary>
        private static WPFHost WPFHostForm;

        /// <summary>
        /// formulario pantalla Settings
        /// </summary>
        private WPFLanguage langWindow;

        /// <summary>
        /// formulario pantalla Versions
        /// </summary>
        private WPFVersions versionsWindow;

        /// <summary>
        /// Grid principal
        /// </summary>
        private Grid mainGrid;

        private string defaultMainFolderPath = "";

        /// <summary>
        /// Modo de empresa seleccionado
        /// </summary>
        private CompanyMode SelectedCompanyName;

        /// <summary>
        /// Idioma seleccionado
        /// </summary>
        private LanguageMode SelectedLanguageMode;

        private string lastBuscarTextInput = "";
        
        #endregion
        
        #region METHODS

        /// <summary>
        /// Función para cargar el formulario buscador 
        /// </summary>
        public void Buscar()
        {
            try
            {
                //Items a mostrar, añadir al Dictionary, y eliminar
                List<string> showItems = new List<string>();
                List<string> deleteItems = new List<string>();
                List<string> addItems = new List<string>();

                //Cogemos los paths de los archivos a buscar
                if (cmbBuscar.Items.Count >= 1)
                {
                    for(int k = 0; k < cmbBuscar.Items.Count; k++)
                    {
                        ///Buscamos directamente en el Dictionary deserializado
                        List<string> currentList = data.TagsDataP.tags[cmbBuscar.Items[k].ToString()];
                                               
                        //Añadimos los archivos asociados a cada tag
                        foreach (string item in currentList)
                        {

                            if (File.Exists(item))
                            {
                                //Añadimos el item a los items que mostraremos
                                showItems.Add(item);
                            }
                            else
                            {
                                //Añadimos el item no existente a los items que borraremos
                                deleteItems.Add(item);
                            }
                        }

                        foreach (string itemToDelete in deleteItems)
                        {
                            data.TagsDataP.tags[cmbBuscar.Items[k].ToString()].Remove(itemToDelete);
                        }

                        foreach (string itemToAdd in addItems)
                        {
                            data.TagsDataP.tags[cmbBuscar.Items[k].ToString()].Add(itemToAdd);
                        }

                        //Exportamos el nuevo JSON
                        DataXml.TagsData tgs = data.TagsDataP;
                        data.TagsDataP = new DataXml.TagsData();

                        //Primero exportamos un archivo vacío
                        data.ExportTagsData(data.TagsPathXML);
                        data.TagsDataP = tgs;

                        //Segundo volvemos a rellenar y volvemos a exportar
                        data.ExportTagsData(data.TagsPathXML);
                    }

                    //Creamos pantalla WPFBuscador
                    WPFBuscadorForm = new WPFBuscador(PathManager.ActualPath, PathManager.MainPath, showItems, SelectedLanguageMode);
                    WPFBuscadorForm.Titulo.Text += "...\r\n" + cmbBuscar.Text;

                    //Color del progress bar
                    WPFBuscadorForm.progressPage.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SelectedCompanyName.ToString() == "bcnvision" ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));

                    //Set del logo
                    WPFBuscadorForm.LogoPrincipal.Source = new BitmapImage(
                        new Uri(Path.Combine(PathManager.StylePath, ("LogoPrincipal_" + SelectedCompanyName.ToString() + ".png"))));
                    
                    //Suscripción a eventos de WPFBuscador
                    WPFBuscadorForm.PathClicked += c_BuscadorPathClick;

                    //Añadimos el formulario
                    object MainWindowFContent = WPFBuscadorForm.Content;
                    WPFBuscadorForm.Content = null;
                    _grid.Children.Clear();
                    _grid.Children.Add(MainWindowFContent as UIElement);

                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Seleccionar un tipo modo de empresa
        /// </summary>
        /// <param name="companyMode"></param>
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

        /// <summary>
        /// Formulario de selecciónd de idioma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButtonIdioma_Checked(object sender, RoutedEventArgs e)
        {
            CompanyMode prevEmpresa = SelectedCompanyName;

            //Abro ventana de Settings
            langWindow = new WPFLanguage(SelectedCompanyName, SelectedLanguageMode);

            if (langWindow.ShowDialog() == false)
            {

                //Obtengo el valor introducido
                string idiomaSel = (string)langWindow.cmbIdiomas.SelectedItem.ToString();
                string empresaSel = (string)langWindow.cmbEmpresa.SelectedItem.ToString();

                if (langWindow.Selection)
                {
                    //Set del idioma
                    if (idiomaSel == "Español")
                        SelectedLanguageMode = LanguageMode.spanish;
                    else if (idiomaSel == "English")
                        SelectedLanguageMode = LanguageMode.english;
                    else if (idiomaSel == "Français")
                        SelectedLanguageMode = LanguageMode.french;

                    Bcnvision_Catalog.Properties.Settings.Default.LanguageMode = SelectedLanguageMode.ToString();

                    //Cambiamos el modo de empresa según la selección
                    if (empresaSel.ToLower() == "bcnvision")
                        SelectedCompanyName = CompanyMode.bcnvision;
                    else if (empresaSel.ToLower() == "nevitec")
                        SelectedCompanyName = CompanyMode.nevitec;

                }
                               
                //Exportamos la data amigo
                data.ConfigDataP.idioma = SelectedLanguageMode.ToString();
                data.ConfigDataP.empresa = SelectedCompanyName.ToString();
                data.ExportConfigData(data.ConfigPathXML);

                //Devolvemos el modo empresa al previo si toca
                if (KeepPrevStyle)
                    SelectedCompanyName = prevEmpresa;

            }
            else
            {
                //Si no tiene nada seleccionado, muestro aviso


            }

            btnIdioma.IsChecked = false;
        }

        /// <summary>
        /// Cerrar formulario de idioma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIdioma_Unchecked(object sender, RoutedEventArgs e)
        {
            langWindow.Close();
        }
                                
        /// <summary>
        /// Función para calcular la diferencia de minutos y segundos entre 2 DateTimes y devolver un string
        /// </summary>
        /// <param name="horaDeInicio"></param>
        /// <param name="horaFinal"></param>
        /// <returns></returns>
        private string diferenciaDeHoras(DateTime horaDeInicio, DateTime horaFinal)
        {
            //Restamos minutos y segundos
            string segs = horaDeInicio.Second <= horaFinal.Second ? (horaFinal.Second - horaDeInicio.Second).ToString() : (60 + horaFinal.Second - horaDeInicio.Second).ToString();
            string mins = horaDeInicio.Minute <= horaFinal.Minute ? (horaFinal.Minute - horaDeInicio.Minute).ToString() : (60 + horaFinal.Minute - horaDeInicio.Minute).ToString();

            //Corregimos
            if (Convert.ToInt32(segs) >= 60)
            {
                segs = (Convert.ToInt32(segs) - 60).ToString();
                mins = (Convert.ToInt32(mins) + 1).ToString();
            }

            //Devolvemos el TimeSpan casero
            return mins + "mins" + segs + "segs";
        }

        /// <summary>
        /// Método para agregar una entrada al log
        /// </summary>
        /// <param name="message"></param>
        public void UpdateLog(string file, string message)
        {
            //Añadimos extensión si no la tiene
            file += file.Contains(".xml") ? "" : ".xml";

            //Creamos objecto del Log
            LogManagement log = new LogManagement("Info", file);

            //Pasamos el mensaje y cerramos
            log.Info(message);
            log.Close();
        }

        /// <summary>
        /// Método para crear un json con todas las tags y rutas asociadas
        /// </summary>
        /// <returns></returns>
        private DataXml.TagsData ExplorarTagsToJsonFile()
        {

            string pth = @"K:\Contenido Catalogo";
            DataXml dataJson = new DataXml();

            dataJson.TagsDataP = new DataXml.TagsData();
            
            //Obtener files de la ruta
            DirectoryInfo infoDir = new DirectoryInfo(pth);
            FileInfo[] files = infoDir.GetFiles("*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                //Solventa problema de creación de archivos de serialización que MK comete a veces
                string xmlFileCorr = files[i].FullName.Replace(".xml.xml", ".xml");

                string asoFileName = "";

                if (File.Exists(xmlFileCorr.Replace(".xml", ".mp4"))) asoFileName = xmlFileCorr.Replace(".xml", ".mp4");
                else if (File.Exists(xmlFileCorr.Replace(".xml", ".pdf"))) asoFileName = xmlFileCorr.Replace(".xml", ".pdf");
                else if (File.Exists(xmlFileCorr.Replace(".xml", ".html"))) asoFileName = xmlFileCorr.Replace(".xml", ".html");
                else if (File.Exists(xmlFileCorr.Replace(".xml", ".pptx"))) asoFileName = xmlFileCorr.Replace(".xml", ".pptx");

                if (asoFileName != "")
                {
                    //Deserializamos
                    DataXml.ItemData item = SerializationManager.ReadItemData(files[i].FullName);

                    //Get every tag
                    //string[] split = item.Tags.Replace(" ", "").Split(',');
                    string[] split = item.Tags.Split(',');

                    if (dataJson.TagsDataP.tags == null)
                        dataJson.TagsDataP.tags = new Dictionary<string, List<string>>();

                    //Si no está guardado lo guardamos
                    for (int j = 0; j < split.Length; j++)
                    {

                        //Si no existe ese tag, lo añadimos
                        if (!dataJson.TagsDataP.tags.Keys.Contains(split[j].Replace(" ", "")))
                            dataJson.TagsDataP.tags.Add(split[j].Replace(" ", ""), new List<string>());

                        //Si no existe esa ruta, la guardamos
                        if (!dataJson.TagsDataP.tags[split[j].Replace(" ", "")].Contains(asoFileName))
                        {
                            dataJson.TagsDataP.tags[split[j].Replace(" ", "")].Add(asoFileName);
                        }

                    }

                }

            }

            //Exportamos la info cada vez
            string fileName = Path.Combine(PathManager.ConfigPath,"TagsInfo.json");
            SerializationManager.WriteTagsData(fileName, dataJson.TagsDataP);

            return dataJson.TagsDataP;
        }

        #endregion

        #region METHODS: Gestión de Pantallas

        /// <summary>
        /// Función para mostrar la pantalla principal: "Home"
        /// </summary>
        /// <param name="firstTime">True si es al inicio de la App</param>
        void SetWPFMain(bool firstTime)
        {
            try
            {
                //Si el botón de buscar está checked, lo uncheckeamos
                if ((bool)btnBuscar.IsChecked) btnBuscar.IsChecked = false;

                if (!KeepPrevStyle || firstTime)
                {
                    //Set del background principal
                    _grid.Background = new ImageBrush(new BitmapImage(
                        new Uri(Path.Combine(PathManager.StylePath, ("BackgroundPrincipal_" + SelectedCompanyName.ToString() + ".jpg")))));


                    //Set del color principal
                    headerPrincipal.Background = SelectedCompanyName == CompanyMode.bcnvision ? new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_bcnvision))
                                                                                              : new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_nevitec));
                }

                //Creamos pantalla WPFMain
                WPFMainForm = new WPFMain(SelectedCompanyName, data, PathManager.CompanyPath);

                //Suscripción a eventos de WPFMain
                WPFMainForm.BtnProductosClicked += c_BtnProductosClick;
                WPFMainForm.BtnPresentacionesClicked += c_BtnPresentacionesClick;
                WPFMainForm.BtnAplicacionesClicked += c_BtnAplicacionesClick;
                WPFMainForm.BtnInformesClicked += c_BtnInformesClick;

                //Set del logo
                WPFMainForm.LogoPrincipal.Source = new BitmapImage(
                    new Uri(Path.Combine(PathManager.StylePath, ("LogoPrincipal_" + SelectedCompanyName.ToString() + ".png"))));


                //Set de colores de botones
                if (true)
                {
                    WPFMainForm.btnAplicaciones.Foreground = SelectedCompanyName == CompanyMode.bcnvision ? new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_bcnvision))
                                                                                              : new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_nevitec));
                    WPFMainForm.btnProductos.Foreground = SelectedCompanyName == CompanyMode.bcnvision ? new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_bcnvision))
                                                                                              : new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_nevitec));
                    WPFMainForm.btnInformes.Foreground = SelectedCompanyName == CompanyMode.bcnvision ? new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_bcnvision))
                                                                                              : new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_nevitec));
                    WPFMainForm.btnPresentaciones.Foreground = SelectedCompanyName == CompanyMode.bcnvision ? new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_bcnvision))
                                                                                          : new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data.StyleDataP.ColorPrincipal_nevitec));
                }

                //Set de tags del buscador
                cmbBuscar.Text = SelectedLanguageMode == LanguageMode.spanish ? data.StyleDataP.tagBuscador_spanish :
                                                     (SelectedLanguageMode == LanguageMode.english ? data.StyleDataP.tagBuscador_english : data.StyleDataP.tagBuscador_french);


                //Set de títulos por idioma
                WPFMainForm.btnProductos.Content = SelectedLanguageMode == LanguageMode.spanish ? data.StyleDataP.Menu1Header_spanish :
                                                     (SelectedLanguageMode == LanguageMode.english ? data.StyleDataP.Menu1Header_english : data.StyleDataP.Menu1Header_french);
                WPFMainForm.btnAplicaciones.Content = SelectedLanguageMode == LanguageMode.spanish ? data.StyleDataP.Menu2Header_spanish :
                                                     (SelectedLanguageMode == LanguageMode.english ? data.StyleDataP.Menu2Header_english : data.StyleDataP.Menu2Header_french);
                WPFMainForm.btnPresentaciones.Content = SelectedLanguageMode == LanguageMode.spanish ? data.StyleDataP.Menu3Header_spanish :
                                                     (SelectedLanguageMode == LanguageMode.english ? data.StyleDataP.Menu3Header_english : data.StyleDataP.Menu3Header_french);
                WPFMainForm.btnInformes.Content = SelectedLanguageMode == LanguageMode.spanish ? data.StyleDataP.Menu4Header_spanish :
                                                     (SelectedLanguageMode == LanguageMode.english ? data.StyleDataP.Menu4Header_english : data.StyleDataP.Menu4Header_french);


                //Añadimos el formulario
                object MainWindowFContent = WPFMainForm.Content;
                WPFMainForm.Content = null;
                _grid.Children.Clear();
                _grid.Children.Add(MainWindowFContent as UIElement);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Función para mostrar el explorador en pantalla principal
        /// </summary>
        void SetWPFExplorer(string headerText, string defaultFolderImagePath, string MainFolderImagePath)
        {
            try
            {
                //Ruta actual = principal
                PathManager.SetExplorerView(headerText);

                //Obtenemos la ruta de la imagen que será la imagen principal de la siguiente pantalla
                string rutaImagen = "";
                //DirectoryInfo imageFolder = new DirectoryInfo(defaultFolderImagePath);
                DirectoryInfo imageFolder = new DirectoryInfo(PathManager.ActualPath);
                string[] imgs;
                do
                {
                    imgs = Directory.GetFiles(imageFolder.FullName);
                    //imgs = Directory.GetFiles(PathManager.ActualPath);

                    for (int i = 0; i < imgs.Length; i++)
                    {
                        if (imgs[i].ToLower().Contains(".png") || imgs[i].ToLower().Contains(".jpg") || imgs[i].ToLower().Contains(".bmp"))
                        {
                            rutaImagen = imgs[i];
                            break;
                        }
                    }


                    imageFolder = imageFolder.Parent;
                }
                while (rutaImagen == "");


                if (PathManager.ChildNames.Count > 0)
                {
                    //Creamos pantalla WPFExplorer
                    WPFExplorerForm = new WPFExplorer(PathManager.ActualPath, PathManager.MainPath, PathManager.ChildNames, null, ExplorerMode.Folders.ToString(), rutaImagen, data, SelectedCompanyName, SelectedLanguageMode);

                    //Suscripción a eventos de WPFExplorer
                    WPFExplorerForm.ContentCardClicked += c_ContentCardClick;
                    WPFExplorerForm.PathClicked += c_ExpPathClick;

                    //Ponemos el logo que toca
                    WPFExplorerForm.LogoPrincipal.Source = new BitmapImage(
                        new Uri(Path.Combine(PathManager.StylePath, ("LogoPrincipal_" + SelectedCompanyName.ToString() + ".png"))));

                    //Color del progress bar
                    WPFExplorerForm.progressPage.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SelectedCompanyName.ToString() == "bcnvision" ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));

                    //Añadimos el formulario
                    object MainWindowFContent = WPFExplorerForm.Content;
                    WPFExplorerForm.Content = null;
                    _grid.Children.Clear();
                    _grid.Children.Add(MainWindowFContent as UIElement);
                }
                else if (PathManager.ChildNames.Count == 0)
                {
                    //Creamos a pantalla WPFContent
                    WPFContentForm = new WPFContent(PathManager.ActualPath, PathManager.MainPath, PathManager.ChildNames, rutaImagen, data, SelectedCompanyName, SelectedLanguageMode);

                    //Suscripción a eventos
                    WPFContentForm.PathClicked += c_ContentPathClick;
                    WPFContentForm.VideoClicked += c_VideoClick;
                    WPFContentForm.DocumentClicked += c_DocumentClick;

                    //Rellenamos el título
                    if(File.Exists(Path.Combine(PathManager.ActualPath, headerText) + ".xml"))
                    {
                        //Deserializamos la traducción
                        DataXml.ItemData titleData = SerializationManager.ReadItemData(Path.Combine(PathManager.ActualPath, headerText) + ".xml");
                        if (SelectedLanguageMode == LanguageMode.spanish) WPFContentForm.Titulo.Text = titleData.TituloES == "" ? headerText : titleData.TituloES;
                        else if (SelectedLanguageMode == LanguageMode.english) WPFContentForm.Titulo.Text = titleData.TituloIN == "" ? headerText : titleData.TituloIN;
                        else if (SelectedLanguageMode == LanguageMode.french) WPFContentForm.Titulo.Text = titleData.TituloFR == "" ? headerText : titleData.TituloFR;
                        else WPFContentForm.Titulo.Text = headerText;
                    }
                    else
                    {
                        //Nombre de la carpeta
                        WPFContentForm.Titulo.Text = headerText;

                    }

                    //Color del progress bar
                    WPFContentForm.progressPage.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SelectedCompanyName.ToString() == "bcnvision" ? data.StyleDataP.ColorPrincipal_bcnvision : data.StyleDataP.ColorPrincipal_nevitec));

                    //Ponemos el logo que toca
                    WPFContentForm.LogoPrincipal.Source = new BitmapImage(
                        new Uri(Path.Combine(PathManager.StylePath, ("LogoPrincipal_" + SelectedCompanyName.ToString() + ".png"))));

                    //Guardamos las TAGs para tener el buscador listo
                    //SaveTagsFromXMLs(PathManager.ActualPath);

                    //Añadimos el formulario
                    object MainWindowFContent = WPFContentForm.Content;
                    WPFContentForm.Content = null;
                    _grid.Children.Clear();
                    _grid.Children.Add(MainWindowFContent as UIElement);


                }



            }
            catch (Exception ex)
            {

            }
        }
        
        #endregion

        #region EVENTS

        /// <summary>
        /// Evento Click del botón de "Home"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Mostramos form Main en la pantalla principal
                SetWPFMain(false);

                //Set del path
                PathManager.ActualPath = PathManager.MainPath + "\\" + SelectedCompanyName.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Método para el evento de cambio de texto en el cmb de buscar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_TextChanged(object sender, RoutedEventArgs e)
        {

            //TODO - corregir primer caracter escrito
            string str = cmbBuscar.Text.ToLower().Replace(" ","");

            //if (str != "")
            //{
            //    cmbBuscar.IsDropDownOpen = true;
            //    SendKeys.SendWait("{RIGHT}");

            //}
            //else
            //    cmbBuscar.IsDropDownOpen = false;

            if (str != lastBuscarTextInput && str.Length > 2)
            {
                lastBuscarTextInput = str;
                cmbBuscar.Items.Clear();

                //cmbBuscar.IsTextSearchEnabled = true;

                string[] tagsArray = data.TagsDataP.tags.Keys.ToArray();

                for (int i = 0; i < tagsArray.Length; i++)
                {
                    if (tagsArray[i].ToLower().Replace(" ", "").Contains(str))
                        cmbBuscar.Items.Add(tagsArray[i]);
                }

            }

            cmbBuscar.StaysOpenOnEdit = true;
            
        }

        /// <summary>
        /// Evento Click en elemento de la barra de rutas inferior en el formulario Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_ExpPathClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFExplorer
                string newPath = WPFExplorerForm.NextRuta.Replace(WPFExplorerForm.NextHeader, "");
                string newHeader = WPFExplorerForm.NextHeader;
                
                PathManager.ActualFolder = newPath;
                PathManager.ActualPath = newPath;

                //Si es de folders, creamos nuevo WPFExplorer
                SetWPFExplorer(newHeader, newPath, WPFExplorerForm.DefaultCardImagePath);

            }
            catch (Exception ex)
            {

            }

        }

        void c_VideoClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFHost
                string newPath = WPFContentForm.NextRutaVideo;
                string newHeader = WPFContentForm.NextHeader;
                string imgPath = WPFContentForm.ImagePath;

                //Creamos nuevo WPFHost
                WPFHost WPFHostForm = new WPFHost(true, newPath, imgPath);
                WPFHostForm.Titulo.Text = newHeader.Replace(".mp4","");
                WPFHostForm.videoHostcontrol.Source = new System.Uri(newPath);
                WPFHostForm.videoHostcontrol.Stretch = Stretch.UniformToFill;
                
                //Suscripción a eventos de WPFHost
                WPFHostForm.BtnAtrasClicked += c_HostBackClick;

                //Añadimos el formulario
                object MainWindowFContent = WPFHostForm.Content;
                WPFHostForm.Content = null;
                _grid.Children.Clear();
                _grid.Children.Add(MainWindowFContent as UIElement);

            }
            catch (Exception ex)
            {

            }

        }

        void c_DocumentClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFHost
                string newPath = WPFContentForm.NextRutaVideo;
                string newHeader = WPFContentForm.NextHeader;
                string newPdfPath = WPFContentForm.NextRuta;
                string imgPath = WPFContentForm.ImagePath;

                //Creamos nuevo WPFHost
                WPFHostForm = new WPFHost(false, newPdfPath, imgPath);
                WPFHostForm.Titulo.Text = newHeader.Replace(".pdf", "").Replace(".html","");

                //Suscripción a eventos de WPFHost
                WPFHostForm.BtnAtrasClicked += c_HostBackClick;

                //Añadimos el formulario
                object MainWindowFContent = WPFHostForm.Content;
                WPFHostForm.Content = null;
                _grid.Children.Clear();
                _grid.Children.Add(MainWindowFContent as UIElement);

            }
            catch (Exception ex)
            {

            }

        }
               
        /// <summary>
        /// Evento Click en elemento de la barra de rutas inferior en el formulario Content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_ContentPathClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFExplorer
                string newPath = WPFContentForm.NextRuta.Replace(WPFContentForm.NextHeader, "");
                string newHeader = WPFContentForm.NextHeader;

                PathManager.ActualFolder = newPath;
                PathManager.ActualPath = newPath;

                //Si es de folders, creamos nuevo WPFExplorer
                SetWPFExplorer(newHeader, newPath, WPFExplorerForm.DefaultCardImagePath);

            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Evento Click en elemento de la barra de rutas inferior en el formulario Content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_BuscadorPathClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFExplorer
                string newPath = WPFBuscadorForm.NextRuta.Replace(WPFBuscadorForm.NextHeader, "");
                string newHeader = WPFBuscadorForm.NextHeader;

                PathManager.ActualFolder = newPath;
                PathManager.ActualPath = newPath;

                //Si es de folders, creamos nuevo WPFExplorer
                SetWPFExplorer(newHeader, newPath, "");

            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Evento click en tarjeta del formulario Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_ContentCardClick(object sender, EventArgs e)
        {
            try
            {
                //Nuevo Path para el siguiente WPFExplorer
                string newPath = WPFExplorerForm.LastCardSelectedPath;
                string newHeader = WPFExplorerForm.LastCardSelectedHeader;

                //Si es de folders, creamos nuevo WPFExplorer
                SetWPFExplorer(newHeader, newPath, WPFExplorerForm.DefaultCardImagePath);

            }
            catch(Exception ex)
            {

            }

        }

        /// <summary>
        /// Evento click en el botón Atrás del formulario Host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_HostBackClick(object sender, EventArgs e)
        {
            try
            {
                string spt = (PathManager.ActualPath.Split('\\'))[PathManager.ActualPath.Split('\\').Length - 1];

                //Nuevo Path para el siguiente WPFExplorer
                string s = @"\" + PathManager.ActualFolder;
                string newPath = PathManager.ActualPath.Replace(s, "");
                PathManager.ActualPath = newPath;
                string newHeader = PathManager.ActualFolder;

                //Si es de folders, creamos nuevo WPFExplorer
                SetWPFExplorer(newHeader, newPath, "");

            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Evento Click en botón de productos producido en WPFMain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_BtnProductosClick(object sender, EventArgs e)
        {
            try
            {

                defaultMainFolderPath = Path.Combine(PathManager.ActualPath, "MENU1");

                ////Imagen
                //object obj = WPFMainForm._gridLayoutMain.Children[0];
                //Controls.ProductosButton btn = new Controls.ProductosButton();
                //btn.Content = (ContentControl)obj;                

                //Mostramos form Explorer en la pantalla principal
                SetWPFExplorer("MENU1", defaultMainFolderPath, Path.Combine(defaultMainFolderPath,"image.png"));

                //Set del path
                PathManager.SetExplorerView(PathManager.ActualPath);

                //DataXml actualData = new DataXml(Path.Combine(PathManager.ActualPath, "PRODUCTOS.xml"), true, false);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Evento Click en botón de presentaciones producido en WPFMain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_BtnPresentacionesClick(object sender, EventArgs e)
        {
            try
            {

                defaultMainFolderPath = Path.Combine(PathManager.ActualPath, "MENU3");

                ////Imagen
                //object obj = WPFMainForm._gridLayoutMain.Children[0];
                //Controls.PresentacionesButton btn = new Controls.PresentacionesButton();
                //btn.Content = (ContentControl)obj;


                //Mostramos form Explorer en la pantalla principal
                SetWPFExplorer("MENU3", defaultMainFolderPath, Path.Combine(defaultMainFolderPath, "image.png"));

                //Set del path
                PathManager.SetExplorerView(PathManager.ActualPath);

            }
            catch (Exception ex)
            {

            }
        }
        
        /// <summary>
        /// Evento Click en botón de informes producido en WPFMain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_BtnInformesClick(object sender, EventArgs e)
        {
            try
            {
                defaultMainFolderPath = Path.Combine(PathManager.ActualPath, "MENU4");

                ////Imagen
                //object obj = WPFMainForm._gridLayoutMain.Children[0];
                //Controls.InformesButton btn = new Controls.InformesButton();
                //btn.Content = (ContentControl)obj;

                //Mostramos form Explorer en la pantalla principal
                SetWPFExplorer("MENU4", defaultMainFolderPath, Path.Combine(defaultMainFolderPath, "image.png"));
                                
                //Set del path
                PathManager.SetExplorerView(PathManager.ActualPath);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Evento Click en botón de aplicaciones producido en WPFMain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void c_BtnAplicacionesClick(object sender, EventArgs e)
        {
            try
            {

                defaultMainFolderPath = Path.Combine(PathManager.ActualPath, "MENU2");

                ////Imagen
                //Grid grid = WPFMainForm._gridLayoutMain;
                //Controls.ApplicationsButton btn1 = new Controls.ApplicationsButton();
                //Border brd = (Border)grid.Children[0];

                
                //Mostramos form Explorer en la pantalla principal
                SetWPFExplorer("MENU2", defaultMainFolderPath, Path.Combine(defaultMainFolderPath, "image.png"));

                //Set del path
                PathManager.SetExplorerView(PathManager.ActualPath);

            }
            catch (Exception ex)
            {

            }
        }
        
        /// <summary>
        /// Evento pulsar Enter tras escribir en el buscador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbBuscar_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //enter key is down

                //Si el botón de buscar está checked, lo uncheckeamos
                if (!(bool)btnBuscar.IsChecked) btnBuscar.IsChecked = true;

                Buscar();
            }
        }

        /// <summary>
        /// Evento pulsar el botón de buscar...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(btnBuscar.IsChecked == true)
                {
                    //Al pulsar poner a true el check de buscar
                    Buscar();
                }
                else
                {
                    //Si el botón de buscar está checked, lo uncheckeamos
                    if ((bool)btnBuscar.IsChecked) btnBuscar.IsChecked = false;

                    //Mostramos form Main en la pantalla principal
                    SetWPFMain(false);

                    //Set del path
                    PathManager.ActualPath = PathManager.MainPath + "\\" + SelectedCompanyName.ToString();

                }



            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Evento de cuando se pulsa en el combobox del buscador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            //cmbBuscar.Foreground = new SolidColorBrush(Color.FromRgb(255,255,255));
            cmbBuscar.Text = "";
            cmbBuscar.IsDropDownOpen = true;
        }

        /// <summary>
        /// Pulsar el botón home
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHome_Checked(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Pulsar el botón Buscar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBuscar_Checked(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Pulsa el botón de actualizar aplicación - versiones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnVersiones_Click(object sender, RoutedEventArgs e)
        {

            string rutaServerConfigFolder = data.ConfigDataP.serverContentMainPath;// + @"\Contenido Catalogo\Config\Config Files"; // @"\\dcsrv01\Bcnvision\Public\07.MARKETING\ALEJANDRO_OLIVO_MK\PROYECTO APP\Contenido Catalogo\Config\Config Files";

            //Abro ventana de Settings
            versionsWindow = new WPFVersions(SelectedLanguageMode, data.ConfigDataP, PathManager.ConfigPath, rutaServerConfigFolder);

            if (versionsWindow.ShowDialog() == false)  
            {

                //Obtengo el valor introducido
                data.ConfigDataP.newVersion = (string)versionsWindow.newVersion;

                if (versionsWindow.IsUpdated)
                    data.ConfigDataP.appVersion = (string)versionsWindow.newVersion;

                data.ExportConfigData(data.ConfigPathXML);


            }

            btnVersiones.IsChecked = false;
        }
        
        /// <summary>
        /// Al cerrar la aplicación
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            this.Background = null;

            
            //Update del log con info de cierre de sesión
            UpdateLog(Path.Combine(PathManager.LogsPath, "Logs_" + currentUser + ".xml"), "Tiempo de uso por " + currentUser + " de " + diferenciaDeHoras(horaDeInicio, DateTime.Now) + ".");
            UpdateLog(Path.Combine(PathManager.LogsPath, "Logs_" + currentUser + ".xml"), "Cierre de sesión por " + currentUser + " a las " + DateTime.Now.ToLongTimeString());

            //Exportar Logs
            Process.Start(Path.Combine(PathManager.ConfigPath, "ExportLgs_KContenido_2_YPublic.ffs_batch"));

            if (File.Exists(Path.Combine(@"G:\bcnvision\bcnvision\Demo\Default", "acceso.csv")))
            {
                if (Directory.Exists(Path.Combine(@"\\netapp\backup","0-AppInfo")))
                {
                    File.Copy(Path.Combine(@"G:\bcnvision\bcnvision\Demo\Default", "acceso.csv"), Path.Combine(@"\\netapp\backup\0-AppInfo", "acceso_" + currentUser + ".csv"), true);
                }

            }

            //Process[] p = Process.GetProcessesByName("Bcnvision Catalog");

            //Cerramos las Apps de proceso si estan abiertas para volver a iniciarlas
            foreach (Process proc in Process.GetProcessesByName("Bcnvision Catalog"))
            {
                
                //bitlockerMng.bitlocker.Lock();
                proc.Kill();
                proc.WaitForExit();
            }

        }
        
        #endregion
                
    }        

}

