using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Bcnvision_Catalog.Classes
{
    /// <summary>
    /// Clase para registrar los eventos y errores de la aplicación en un fichero
    /// de log
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    public class LogManagement
    {
        #region INTERN CLASS

        /// <summary>
        /// Clase para contener las propiedades asignadas al log
        /// </summary>
        /// <typeparam name="?"></typeparam>
        public class Property
        {
            public string this[string key]
            {
                get
                {
                    return log4net.MDC.Get(key);
                }
                set
                {
                    log4net.MDC.Set(key, value);
                }
            }
        }

        #endregion

        #region FIELDS

        /// <summary>
        /// Identificador único para este log
        /// </summary>
        private ILog _log;
        private string _id;
        private RollingFileAppender _appender;
        private Property _property;

        private static bool basicConfigurationDone = false;

        #endregion

        #region PROPPERTIES

        /// <summary>
        /// Propiedad
        /// </summary>
        public Property Properties
        {
            get
            {
                return _property;
            }
        }

        /// <summary>
        /// Ruta y nombre del fichero donde registrar los eventos
        /// </summary>
        public string FilePath
        {
            get
            {
                if (_appender == null) { return null; }
                return _appender.File;
            }
            set
            {
                if (_appender == null) { return; }
                _appender.File = value;
                _appender.ActivateOptions();
            }
        }

        /// <summary>
        /// Diseño de cada una de las entradas en el log
        /// </summary>
        public string PatternLayout
        {
            set
            {
                if (_appender == null) { return; }
                PatternLayout layout = new PatternLayout();
                layout.ConversionPattern = value;
                layout.ActivateOptions();
                _appender.Layout = layout;
                _appender.ActivateOptions();
            }
        }

        #endregion

        #region CONSTRUCTOR

        public LogManagement()
        {
            _log = null;
            _id = "";
            _appender = new RollingFileAppender();
            _property = new Property();
        }

        /// <summary>
        /// Configuración del registro de eventos. Creará ficheros de 1000 ficheros de 1 MB antes de volver a 
        /// empezar a escribir el primero
        /// </summary>
        /// <param name="id">identificador único para este log</param>
        /// <param name="fileName">Ruta y nombre del fichero donde registrar los eventos</param>
        public LogManagement(string id, string fileName)
            : this(id, fileName, 1048576, 1000, "%date %-5level %message %newline")
        {


        }

        /// <summary>
        /// Configuración del registro de eventos
        /// </summary>
        /// <param name="id">identificador único para este log</param>
        /// <param name="fileName">Ruta y nombre del fichero donde registrar los eventos</param>
        /// <param name="maxFileSize">Máximo tamaño en bytes de cada fichero</param>
        /// <param name="maxSizeRollBackups">Número máximo de ficheros a crear antes de volver a sobreescribir el primero</param>
        /// <param name="patternLayout">Diseño de cada una de las entradas en el log</param>
        public LogManagement(string id, string fileName, long maxFileSize, int maxSizeRollBackups, string patternLayout)
            : this()
        {
            _id = id;
            _log = LogManager.GetLogger(_id);
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)_log.Logger;
            // Establecemos todos los parámetros del appender
            if (null == _appender)
            {
                _appender = new log4net.Appender.RollingFileAppender();
            }
            _appender.Encoding = Encoding.UTF8;
            _appender.Name = _id;
            _appender.File = fileName;
            _appender.StaticLogFileName = true;
            _appender.MaxFileSize = maxFileSize;
            _appender.MaxSizeRollBackups = maxSizeRollBackups;
            _appender.AppendToFile = true;
            _appender.SecurityContext = log4net.Util.NullSecurityContext.Instance;
            _appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            _appender.CountDirection = 0;
            _appender.LockingModel = new FileAppender.MinimalLock();

            if (null == _property)
            {
                _property = new Property();
            }

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = patternLayout;
            layout.ActivateOptions();
            _appender.Layout = layout;
            _appender.ActivateOptions();

            l.Level = log4net.Core.Level.All;
            l.AddAppender(_appender);

            // Add a console appender to the root logger
            // If this is called several times, multiple console appenders will be added and the console will show repeated entries
            // Do this only one time per application
            if (basicConfigurationDone == false)
            {
                BasicConfigurator.Configure();
                basicConfigurationDone = true;
            }
        }

        #endregion

        #region METHODS

        #region Escritura en el fichero de log

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo debug
        /// </summary>
        public void Debug(object message)
        {
            _log.Debug(message);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo debug
        /// </summary>
        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo error
        /// </summary>
        public void Error(object message)
        {
            _log.Error(message);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo error
        /// </summary>
        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo fatal
        /// </summary>
        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo fatal
        /// </summary>
        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo información
        /// </summary>
        public void Info(object message)
        {
            _log.Info(message);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo información
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo warn
        /// </summary>
        public void Warn(object message)
        {
            _log.Warn(message);
        }

        /// <summary>
        /// Registra el mensaje pasado como parámetro como un mensaje de tipo warn
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        #endregion

        #region Abrir/Cerrar fichero

        public void Close()
        {
            if (null != _appender)
            {
                log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)_log.Logger;
                l.RemoveAppender(_appender);
                _appender = null;
            }
        }
        #endregion

        #endregion
    }
}


