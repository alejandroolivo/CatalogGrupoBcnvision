using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bcnvision_Catalog.Classes
{

    /// <summary>
    /// Clase para hacer la serialización y deserialización en XML o JSON
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    public class SerializationManager
    {

        #region CONSTRUCTOR

        public SerializationManager()
        {

        }

        #endregion

        #region READ METHODS

        public static DataXml.ConfigData ReadConfigData(string file_combined)
        {
            //Objeto de la clase Folder
            DataXml.ConfigData confConfig = new DataXml.ConfigData();

            //Stream reader
            using (StreamReader sr = new StreamReader(file_combined))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataXml.ConfigData));

                confConfig = (DataXml.ConfigData)serializer.Deserialize(sr);

                sr.Close();
            }

            return confConfig;
        }

        public static DataXml.StyleData ReadStyleData(string file_combined)
        {
            //Objeto de la clase Folder
            DataXml.StyleData styleData = new DataXml.StyleData();

            //Stream reader
            using (StreamReader sr = new StreamReader(file_combined))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataXml.StyleData));

                styleData = (DataXml.StyleData)serializer.Deserialize(sr);

                sr.Close();
            }

            return styleData;
        }

        public static DataXml.ItemData ReadItemData(string file_combined)
        {
            //Objeto de la clase Folder
            DataXml.ItemData confItem = new DataXml.ItemData();

            //Stream reader
            using (StreamReader sr = new StreamReader(file_combined))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataXml.ItemData));

                confItem = (DataXml.ItemData)serializer.Deserialize(sr);

                sr.Close();
            }

            return confItem;
        }

        public static DataXml.TagsData ReadTagsData(string file_combined)
        {

            string pth = file_combined.Replace(".xml", ".json");

            // read file into a string and deserialize JSON to a type
            //DataXml.TagsData tags = JsonConvert.DeserializeObject<DataXml.TagsData>(File.ReadAllText(file_combined.Replace(".xml",".json")));

            DataXml.TagsData tags;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(pth))
            {
                JsonSerializer serializer = new JsonSerializer();
                tags = (DataXml.TagsData)serializer.Deserialize(file, typeof(DataXml.TagsData));
            }


            return tags;
        }

        #endregion

        #region WRITE METHODS

        public static void WriteConfigData(string file_combined, DataXml.ConfigData config_Data)
        {
            using (StreamWriter sr = new StreamWriter(file_combined))
            {
                XmlSerializer xSerializer = new XmlSerializer(typeof(DataXml.ConfigData));

                DataXml.ConfigData configData = new DataXml.ConfigData();
                configData = config_Data;

                xSerializer.Serialize(sr, (DataXml.ConfigData)configData);
                sr.Close();
            }
        }

        public static void WriteStyleData(string file_combined, DataXml.StyleData style_Data)
        {
            using (StreamWriter sr = new StreamWriter(file_combined))
            {
                XmlSerializer xSerializer = new XmlSerializer(typeof(DataXml.StyleData));

                DataXml.StyleData styleData = new DataXml.StyleData();
                styleData = style_Data;

                xSerializer.Serialize(sr, (DataXml.StyleData)styleData);
                sr.Close();
            }
        }

        public static void WriteTagsData(string file_combined, DataXml.TagsData tags_Data)
        {
            string pth = "";

            if (!file_combined.Contains(".json"))
                if (file_combined.Contains(".xml"))
                    pth = file_combined.Replace(".xml", ".json");
                else
                    pth = file_combined + ".json";
            else
                pth = file_combined;


            var json = JsonConvert.SerializeObject(tags_Data, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            using (StreamWriter file = File.CreateText(pth))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, tags_Data);
            }

        }

        public static void WriteItemData(string file_combined, DataXml.ItemData item_Data)
        {
            using (StreamWriter sr = new StreamWriter(file_combined))
            {
                XmlSerializer xSerializer = new XmlSerializer(typeof(DataXml.ItemData));

                DataXml.ItemData itemData = new DataXml.ItemData();
                itemData = item_Data;

                xSerializer.Serialize(sr, (DataXml.ItemData)itemData);
                sr.Close();
            }
        }

        #endregion

    }
}
