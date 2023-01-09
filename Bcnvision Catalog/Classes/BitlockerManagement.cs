using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitlockerManager;

namespace Bcnvision_Catalog
{
    /// <summary>
    /// Clase para gestionar el lock y unlock de Drives que están encriptados mediante Bitlocker
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    class BitlockerManagement
    {
        #region CONSTRUCTOR

        public BitlockerManagement(string drive)
        {
            //Incializamos el objeto Drive Info con la letra de la unidad a desencriptar
            DriveInfo driveInfo = new DriveInfo(drive);

            //Creamos el objeto del tipo Botlocker manager con la info del Drive
            bitlocker = new BitLockerManager.BitLockerManager(driveInfo);
        }

        #endregion

        #region FIELDS

        /// <summary>
        /// Objeto Manager de Btlocker
        /// </summary>
        private BitLockerManager.BitLockerManager bitlocker;

        #endregion

        #region METHODS

        /// <summary>
        /// Método para bloquear el disco
        /// </summary>
        public void LockContent()
        {
            try
            {
                //Bloquear disco
                bitlocker.Lock();

            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Método para desbloquear el disco, al que le pasamos la contraseña tal cual
        /// </summary>
        /// <param name="password"></param>
        public void UnlockContent(string password)
        {
            try
            {
                //Desbloquear disco con contraseña
                bitlocker.UnlockDriveWithPassphrase(password);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }

    #region ex sharedpoint download
    //private static void GetFoldersAndFiles(Folder mainFolder, ClientContext clientContext, string pathString)
    //{
    //    clientContext.Load(mainFolder, k => k.Files, k => k.Folders);
    //    clientContext.ExecuteQuery();
    //    foreach (var folder in mainFolder.Folders)
    //    {
    //        string folderPath = string.Format(@"{0}\{1}", pathString, folder.Name);
    //        System.IO.Directory.CreateDirectory(folderPath);

    //        GetFoldersAndFiles(folder, clientContext, folderPath);
    //    }

    //    foreach (var file in mainFolder.Files)
    //    {
    //        var fileRef = file.ServerRelativeUrl;
    //        var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, fileRef);
    //        var fileName = Path.Combine(pathString, file.Name);
    //        using (var fileStream = System.IO.File.Create(fileName))
    //        {

    //            fileInfo.Stream.CopyTo(fileStream);
    //        }
    //    }
    //}
    #endregion

}
