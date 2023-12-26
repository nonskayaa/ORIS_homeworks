using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyDodoPizzaServer.Configuration
{
    public class Configuration
    {
        private const string configFileName = "appsettings.json";

        public static Appsettings Config { get; private set; }

        public Configuration()
        {
            Config = new Appsettings();
        }
        public static Appsettings GetConfiguration()
        {
            try
            {
                using (var configFile = new FileStream(configFileName, FileMode.Open))
                {
                    Config = JsonSerializer.Deserialize<Appsettings>(configFile) ?? throw new Exception("Ошибка. Невозможно открыть файл");
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Невозможно найти {configFileName}");
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            return Config;
        }
    }
}
