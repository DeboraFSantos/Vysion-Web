using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Vysion.Settings
{
    public class MongoDbSettings
    {
        public async Task ConnectToMongoDB()
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromConnectionString(ConnectionString);
                settings.ConnectTimeout = TimeSpan.FromSeconds(30); // Define o tempo limite de conexão
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30); // Define o tempo limite de seleção do servidor

                MongoClient client = new MongoClient(settings);

                // Testar a conexão
                await client.ListDatabaseNamesAsync();
                
                Console.WriteLine("Conexão bem-sucedida!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao MongoDB: {ex.Message}");
            }
        }

        public string ConnectionString 
        {
            get
            {
                return "mongodb+srv://adeboraferreiras:vEUS6R2geVRTVTjj@cluster0.zzvcofe.mongodb.net/?retryWrites=true&w=majority&connect=replicaSet";
            }
        }
    }
}
