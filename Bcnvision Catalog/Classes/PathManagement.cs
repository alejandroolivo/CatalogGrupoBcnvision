using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcnvision_Catalog.Classes
{
    /// <summary>
    /// Clase para la gestión de rutas y directorios
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    class PathManagement
    {
        #region CONSTRUCTOR

        public PathManagement(string mainPath)
        {

            //Asignamos el main path
            MainPath = mainPath;

            //Asignamos el Actual path
            ActualPath = mainPath;

            //Asignamos el Config Path
            ConfigPath = Path.Combine(Path.Combine(mainPath, "Config"), "Config Files");

            //Asignamos el Style Path de la K
            StylePath = Path.Combine(Path.Combine(mainPath, "Config"), "Estilo");

            //Asignamos el Style Path
            LogsPath = Path.Combine(ConfigPath, "Logs");

        }

        #endregion

        #region FIELDS



        #endregion

        #region PROPERTIES

        /// <summary>
        /// Ruta de la carpeta que contiene TODO el contenido
        /// </summary>
        public string MainPath { get; set; }

        /// <summary>
        /// Ruta de la carpeta que contiene los archivos de Configuracion
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// Ruta de la carpeta que contiene los archivos de Estilo
        /// </summary>
        public string StylePath { get; set; }

        /// <summary>
        /// Ruta de la carpeta que contiene los archivos de Estilo
        /// </summary>
        public string LogsPath { get; set; }

        /// <summary>
        /// Ruta de la carpeta actual
        /// </summary>
        public string ActualPath { get; set; }

        /// <summary>
        /// Ruta de la carpeta con el contenido de la empresa seleccionada
        /// </summary>
        public string CompanyPath { get; set; }

        /// <summary>
        /// Ruta de la carpeta actual
        /// </summary>
        public string ActualFolder { get; set; }

        /// <summary>
        /// Array con las rutas de las carpetas contenidas en el ActualPath
        /// </summary>
        public List<string> ChildPaths { get; set; }

        /// <summary>
        /// Array con las carpetas contenidas en el ActualPath
        /// </summary>
        public List<string> ChildNames { get; set; }

        /// <summary>
        /// Ruta de la carpeta contenedora
        /// </summary>
        public string ParentPath { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Método para preparar la siguiente ruta a explorar: extraer children, update de carpeta actual(parent etc)
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="setCompanyPath">Indicar si en esta ejecución hay que setear el nombre de la carpeta según el modo de empresa seleccionado</param>
        public void SetExplorerView(string folderName, bool setCompanyPath = false)
        {
            try
            {
                //Asignamos nueva ruta actual
                ActualPath = Path.Combine(ActualPath, folderName);

                if (setCompanyPath)
                {
                    CompanyPath = ActualPath;
                }

                //Guardamos el nombre de la carpeta
                ActualFolder = folderName;

                //Cogemos las carpetas contenidas
                GetChildrenPaths(ActualPath);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Método para extraer el nombre de una carpeta a partir de su ruta
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetCardNameFromPath(string path)
        {

            string name = "";

            try
            {
                //info del directorio                                
                DirectoryInfo info = new DirectoryInfo(path);

                //Recogemos el nombre de la última carpeta contenedora
                name = path.Replace(info.Parent.FullName, "");
                name = name.Replace("\\", "");

            }
            catch (Exception ex)
            {

            }

            return name;
        }

        /// <summary>
        /// Método que encuentra las carpetas contenidas en un Path, y las guarda en las propiedades correspondientes
        /// </summary>
        /// <param name="actualPath"></param>
        public void GetChildrenPaths(string actualPath)
        {
            try
            {
                //info del directorio                                
                DirectoryInfo info = new DirectoryInfo(actualPath);

                ChildPaths = new List<string>();
                ChildNames = new List<string>();

                DirectoryInfo[] dirInfo = info.GetDirectories();

                for (int i = 0; i < dirInfo.Length; i++)
                {
                    ChildPaths.Add(dirInfo[i].FullName);
                    ChildNames.Add(dirInfo[i].Name);
                }


            }
            catch (Exception ex)
            {

            }
        }

        #endregion

    }
}
