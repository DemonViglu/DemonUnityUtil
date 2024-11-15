using Newtonsoft.Json;
using System;
using System.IO;
namespace demonviglu.config 
{
    public class BaseConfig
    {
        public static string SaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string SaveFolderName = "DemonConfig";

        public BaseConfig()
        {

        }

        public void Save()
        {
            string saveFullPath = Path.Combine(SaveFolderPath, SaveFolderName, GetType().Name);

            string data = JsonConvert.SerializeObject(this, Formatting.Indented);

            if (!Directory.Exists(Path.Combine(SaveFolderPath, SaveFolderName)))
            {
                Directory.CreateDirectory(Path.Combine(SaveFolderPath, SaveFolderName));
            }

            File.WriteAllText(saveFullPath, data);
        }

        public static T Load<T>() where T : BaseConfig, new()
        {
            if (File.Exists(Path.Combine(SaveFolderPath, SaveFolderName, typeof(T).Name)))
            {
                string data = File.ReadAllText(Path.Combine(SaveFolderPath, SaveFolderName, typeof(T).Name));

                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                return new T();
            }
        }
    }
}