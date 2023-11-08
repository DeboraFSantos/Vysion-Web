namespace Vysion.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public string ConenctionString 
        {
            get
            {
                return $"mongodb://{Host}:{Port}";
            }
        }
    }
}