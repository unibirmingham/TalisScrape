using System;

namespace TalisScraper.Events
{
    public delegate void ResourceScrapedHandler(object sender, ResourceScrapedEventArgs args);


    public class ResourceScrapedEventArgs : EventArgs
    {
        public string ResourceValue { get; internal set; }        
        public string ResourceType { get; internal set; }

        public ResourceScrapedEventArgs(string resourceValue, string resourceType)
        {
            ResourceValue = ResourceValue;            
            ResourceType = resourceType;
        }
    }
}
