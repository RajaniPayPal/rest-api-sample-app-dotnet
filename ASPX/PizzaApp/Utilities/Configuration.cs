using System.Collections.Generic;

namespace PizzaApp
{
    public static class Configuration
    {
        // Creates a configuration map containing endpoints
        public static Dictionary<string, string> GetConfiguration()
        {
            Dictionary<string, string> configurationMap = new Dictionary<string, string>();

            // Endpoints are varied depending on whether sandbox OR live is chosen for mode
            configurationMap.Add("mode", "sandbox");

            return configurationMap;
        }    
    }
}