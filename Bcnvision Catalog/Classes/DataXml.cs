using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bcnvision_Catalog.Classes
{
    /// <summary>
    /// Clase que define la información serializable de la aplicación
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    public class DataXml
    {
        #region ATTRIBUTES

        // Path donde se encuentra el XML
        private string _itemPathXML;
        private string _configPathXML;
        private string _stylePathXML;
        private string _tagsPathJSON;

        ConfigData _configData = null;
        StyleData _styleData = null;
        ItemData _itemData = null;
        TagsData _tagsData = null;
        
        #endregion
        
        #region PROPERTIES

        public string ItemPathXML
        {
            get { return _itemPathXML; }
            set { _itemPathXML = value; }
        }
        public string StylePathXML
        {
            get { return _stylePathXML; }
            set { _stylePathXML = value; }
        }
        public string ConfigPathXML
        {
            get { return _configPathXML; }
            set { _configPathXML = value; }
        }
        public string TagsPathXML
        {
            get { return _tagsPathJSON; }
            set { _tagsPathJSON = value; }

        }
        public ConfigData ConfigDataP
        {
            get { return _configData; }
            set { _configData = value; }
        }

        public StyleData StyleDataP
        {
            get { return _styleData; }
            set { _styleData = value; }
        }

        public ItemData ItemDataP
        {
            get { return _itemData; }
            set { _itemData = value; }
        }

        public TagsData TagsDataP
        {
            get { return _tagsData; }
            set { _tagsData = value; }
        }

        #endregion

        #region CONSTRUCTOR

        public DataXml(string ConfigPath)
        {
            //Ruta del archivo
            _configPathXML = ConfigPath;

            //Cargamos configuración
            LoadConfigData(_configPathXML);
        }

        public DataXml(string ConfigPath, string TagsPath, string StylePath)
        {
            //Ruta del archivo
            _configPathXML = ConfigPath;
            _tagsPathJSON = TagsPath;

            //Cargamos configuración
            LoadConfigData(_configPathXML);
            LoadTagsData(_tagsPathJSON);

            if(_configData.empresa.ToLower() == "nevitec")
            {
                _stylePathXML = StylePath.Replace(".xml", "") + "_nevitec.xml";
            }
            else
            {
                _stylePathXML = StylePath.Replace(".xml", "") + "_bcnvision.xml";
            }

            LoadStyleData(_stylePathXML);
        }

        public DataXml(string ConfigPath, string TagsPath, string StylePath, string ItemPath)
        {
            //Ruta del archivo
            _itemPathXML = ItemPath;
            _configPathXML = ConfigPath;
            _tagsPathJSON = TagsPath;
            _stylePathXML = StylePath;

            //Cargamos item y configuracion
            LoadItemData(_itemPathXML);
            LoadConfigData(_configPathXML);
            LoadStyleData(_stylePathXML);
            LoadTagsData(_tagsPathJSON);
        }

        public DataXml()
        {

        }

        #endregion

        #region METHODS 

        /// <summary>
        /// Función para comprobar que existe un archivo y lanzar una excepción si no
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        internal void FileExists(string filePath)
        {
            string message = string.Empty;

            if (System.IO.File.Exists(filePath) == false)
            {
                message = "File : " + filePath + " not found";
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Método para cargar la info de un item
        /// </summary>
        public void LoadItemData(string path)
        {
            try
            {
                //Guardamos el path
                _itemPathXML = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_itemPathXML))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _itemData = SerializationManager.ReadItemData(_itemPathXML);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método para cargar la info de configuración
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfigData(string path)
        {
            try
            {
                //Guardamos el path
                _configPathXML = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_configPathXML))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _configData = SerializationManager.ReadConfigData(_configPathXML);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método para cargar la info de estilo
        /// </summary>
        /// <param name="path"></param>
        public void LoadStyleData(string path)
        {
            try
            {
                //Guardamos el path
                _stylePathXML = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_stylePathXML))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _styleData = SerializationManager.ReadStyleData(_stylePathXML);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Método para cargar la info de los tags
        /// </summary>
        /// <param name="path"></param>
        public void LoadTagsData(string path)
        {
            try
            {
                //Guardamos el path
                _tagsPathJSON = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_tagsPathJSON))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _tagsData = SerializationManager.ReadTagsData(_tagsPathJSON);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportItemData(string nameDir)
        {
            if (!nameDir.Contains(".xml"))
                nameDir += ".xml";

            if (_itemData != null)
                SerializationManager.WriteItemData(nameDir, _itemData);
        }

        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportConfigData(string nameDir)
        {

            if (!nameDir.Contains(".xml"))
                nameDir += ".xml";

            if (_configData != null)
                SerializationManager.WriteConfigData(nameDir, _configData);
        }

        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportStyleData(string nameDir)
        {

            if (!nameDir.Contains(".xml"))
                nameDir += ".xml";

            if (_styleData != null)
                SerializationManager.WriteStyleData(nameDir, _styleData);
        }
        
        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportTagsData(string nameDir)
        {

            if (!nameDir.Contains(".json"))
                nameDir += ".json";

            if (_tagsData != null)
                SerializationManager.WriteTagsData(nameDir, _tagsData);
        }
        
        #endregion
               
        #region Clases Serializables

        /// <summary>
        /// Clase serializable que define los parámetros de configuración de la app
        /// </summary>
        [Serializable()]
        [XmlRoot("Config", Namespace = "", IsNullable = false)]
        public class ConfigData
        {
            #region Properties
            public string empresa { get; set; }
            public string idioma { get; set; }
            public string appVersion { get; set; }
            public string newVersion { get; set; }
            public string isSurfaceOrPC { get; set; }
            public string lastUserLogged { get; set; }
            public ObservableCollection<User> users { get; set; }

            public string serverContentMainPath = "";

            #endregion

        }

        /// <summary>
        /// Clase serializable que define los parámetros de estilo de la app
        /// </summary>
        [Serializable()]
        [XmlRoot("Style", Namespace = "", IsNullable = false)]
        public class StyleData
        {
            #region Properties
            public string ColorPrincipal_bcnvision { get; set; }
            public string ColorPrincipal_nevitec { get; set; }
            public string tagBuscador_spanish { get; set; }
            public string tagBuscador_english { get; set; }
            public string tagBuscador_french { get; set; }

            //Menús principales WPFMain
            public string Menu1Header_spanish { get; set; }
            public string Menu1Header_french { get; set; }
             public string Menu1Header_english  { get; set; }
            public string Menu2Header_spanish  { get; set; }
            public string Menu2Header_french { get; set; }
             public string Menu2Header_english  { get; set; }
            public string Menu3Header_spanish  { get; set; }
            public string Menu3Header_french { get; set; }
             public string Menu3Header_english  { get; set; }
            public string Menu4Header_spanish  { get; set; }
            public string Menu4Header_french { get; set; }
             public string Menu4Header_english  { get; set; }

            //Tarjetas de menús principales
            public string Menu1DescriptionLine1_spanish { get; set; }
            public string Menu1DescriptionLine1_french { get; set; }
            public string Menu1DescriptionLine1_english { get; set; }
            public string Menu1DescriptionLine2_spanish { get; set; }
            public string Menu1DescriptionLine2_french { get; set; }
            public string Menu1DescriptionLine2_english { get; set; }

            public string Menu2DescriptionLine1_spanish { get; set; }
            public string Menu2DescriptionLine1_french { get; set; }
            public string Menu2DescriptionLine1_english { get; set; }
            public string Menu2DescriptionLine2_spanish { get; set; }
            public string Menu2DescriptionLine2_french { get; set; }
            public string Menu2DescriptionLine2_english { get; set; }

            public string Menu3DescriptionLine1_spanish { get; set; }
            public string Menu3DescriptionLine1_french { get; set; }
            public string Menu3DescriptionLine1_english { get; set; }
            public string Menu3DescriptionLine2_spanish { get; set; }
            public string Menu3DescriptionLine2_french { get; set; }
            public string Menu3DescriptionLine2_english { get; set; }

            public string Menu4DescriptionLine1_spanish { get; set; }
            public string Menu4DescriptionLine1_french { get; set; }
            public string Menu4DescriptionLine1_english { get; set; }
            public string Menu4DescriptionLine2_spanish { get; set; }
            public string Menu4DescriptionLine2_french { get; set; }
            public string Menu4DescriptionLine2_english { get; set; }

            #endregion

        }

        /// <summary>
        /// Clase serializable que define los tags de la app
        /// </summary>
        [Serializable()]
        [XmlRoot("Tags", Namespace = "", IsNullable = false)]
        public class TagsData
        {
            #region Properties

            public Dictionary<string, List<string>> tags;

            #endregion


        }

        /// <summary>
        /// Clase serializable que define los parámetros asociados a un item del catálogo
        /// </summary>
        [Serializable()]
        [XmlRoot("Item", Namespace = "", IsNullable = false)]
        public class ItemData
        {
            public string TituloES { get; set; }
            public string TituloIN { get; set; }
            public string TituloFR { get; set; }
            public string Tags { get; set; }
            public string DescripcionES { get; set; }
            public string DescripcionIN { get; set; }
            public string DescripcionFR { get; set; }
        }

        public class User : IEquatable<User>
        {
            #region Properties
            public string Name { get; set; }
            public string Pwd { get; set; }
            #endregion

            #region Constructor
            public User(string Name, string pwd)
            {
                this.Name = Name;
                this.Pwd = pwd;
            }
            public User()
            { }
            #endregion

            public bool Equals(User other)
            {
                return other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }


        #endregion

    }

    
}
