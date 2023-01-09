using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Bcnvision_Catalog.Classes
{
    /// <summary>
    /// Clase para la gestión de usuarios con contraseñas mediante encriptación
    /// Autor: bcnvision 
    /// </summary>
    class BcnCryptography
    {

        #region FIELDS
        private RijndaelManaged Algorithm;
        private MemoryStream memStream;
        private ICryptoTransform EncryptorDecryptor;
        private CryptoStream crStream;
        private StreamWriter strWriter;
        private StreamReader strReader;
        private byte[] key;
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mk">Clave maestra</param>
        public BcnCryptography(byte[] mk)  // Masterkey
        {
            key = new byte[32];

            if (mk.Length < 32)
            {
                throw new ArgumentNullException("Tamaño de la clave menor que 32 bytes");
            }

            // Crea una clave de 32 bytes a partir de una que será mayor
            SHA256 sh = SHA256.Create();
            byte[] mk2 = sh.ComputeHash(mk);

            Array.Copy(mk2, key, 32);  // Registra la clave maestra
        }

        #endregion

        #region METHODS

        /// <summary>
        /// analiza la integridad de un bloque de datos calculando el hash256 para obtener un checsum de 32 256 bits que luego se codifica.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CRC(string data)
        {
            SHA1 sh = SHA1.Create();
            byte[] xdata = Encoding.ASCII.GetBytes(data);
            byte[] bdata = sh.ComputeHash(xdata);
            return Convert.ToBase64String(bdata);
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="usr">usuario del que se encripta la contraseña</param>
        /// <param name="psw">contraseña a encriptar</param>
        /// <returns></returns>
        public string Encrypt(string usr, string psw)
        {
            //new instance of algorithm creation
            Algorithm = new RijndaelManaged();

            //setting algorithm bit size
            Algorithm.BlockSize = 256;
            Algorithm.KeySize = 256;

            //creating new instance of Memory stream
            memStream = new MemoryStream();

            //creating new instance of the Encryptor
            byte[] x = Encoding.ASCII.GetBytes(usr);
            byte[] iv = new byte[32]; for (int i = 0; i < 32; i++) iv[i] = 0;
            for (int i = 0; i < (int)(32 / x.Length); i++)
            {
                Array.Copy(x, 0, iv, i * x.Length, x.Length);
            }
            EncryptorDecryptor = Algorithm.CreateEncryptor(key, iv);

            //creating new instance of CryptoStream
            crStream = new CryptoStream(memStream, EncryptorDecryptor, CryptoStreamMode.Write);

            //creating new instance of Stream Writer
            strWriter = new StreamWriter(crStream);

            //cipher input string
            strWriter.Write(psw);

            //clearing buffer for currnet writer and writing any 
            //buffered data to //the underlying device
            strWriter.Flush();
            crStream.FlushFinalBlock();

            //storing cipher string as byte array 
            byte[] pwd_byte = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(pwd_byte, 0, (int)pwd_byte.Length);

            //storing cipher string as Unicode string
            string pwd_str = Convert.ToBase64String(pwd_byte);

            return pwd_str;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="usr">usuario del que se desencripta la contraseña</param>
        /// <param name="psw">contraseña a desencriptar</param>
        /// <returns></returns>
        public string Decrypt(string usr, string psw)
        {
            try
            {
                //new instance of algorithm creation
                Algorithm = new RijndaelManaged();

                //setting algorithm bit size
                Algorithm.BlockSize = 256;
                Algorithm.KeySize = 256;

                //creating new Memory stream as stream for input string      
                //MemoryStream memStream = new MemoryStream(new UnicodeEncoding().GetBytes(s));
                MemoryStream memStream = new MemoryStream(Convert.FromBase64String(psw));

                //Decryptor creating 
                byte[] x = Encoding.ASCII.GetBytes(usr);
                byte[] iv = new byte[32]; for (int i = 0; i < 32; i++) iv[i] = 0;
                for (int i = 0; i < (int)(32 / x.Length); i++)
                {
                    Array.Copy(x, 0, iv, i * x.Length, x.Length);
                }
                ICryptoTransform EncryptorDecryptor = Algorithm.CreateDecryptor(key, iv);

                //setting memory stream position
                memStream.Position = 0;

                //creating new instance of Crupto stream
                CryptoStream crStream = new CryptoStream(memStream, EncryptorDecryptor, CryptoStreamMode.Read);

                //reading stream
                strReader = new StreamReader(crStream);

                //returnig decrypted string
                return strReader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GenerateAPassKey(string passphrase)
        {
            // Pass Phrase can be any string
            string passPhrase = passphrase;
            // Salt Value can be any string(for simplicity use the same value as used for the pass phrase)
            string saltValue = passphrase;
            // Hash Algorithm can be "SHA1 or MD5"
            string hashAlgorithm = "SHA1";
            // Password Iterations can be any number
            int passwordIterations = 2;
            // Key Size can be 128,192 or 256
            int keySize = 256;
            // Convert Salt passphrase string to a Byte Array
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            // Using System.Security.Cryptography.PasswordDeriveBytes to create the Key
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            //When creating a Key Byte array from the base64 string the Key must have 32 dimensions.
            byte[] Key = pdb.GetBytes(keySize / 11);
            String KeyString = Convert.ToBase64String(Key);

            return KeyString;
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        #endregion
    }
}
