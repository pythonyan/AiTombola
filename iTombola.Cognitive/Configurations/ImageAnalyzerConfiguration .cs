using Microsoft.Extensions.Configuration;

namespace iTombola.Cognitive.Configurations
{
    internal class ImagenAnalyzerConfiguration
    {
        const string ConfigRootName = "ImageAnalyzer";
        public string SubscriptionKey { get; set; }
        public string Endpoint { get; set; }

        public static ImagenAnalyzerConfiguration Load(IConfiguration config)
        {
            var retVal = new ImagenAnalyzerConfiguration();
            retVal.SubscriptionKey = config[$"{ConfigRootName}:SubscriptionKey"];
            retVal.Endpoint = config[$"{ConfigRootName}:Endpoint"];
            return retVal;
        }
    }
}
