namespace Serialization
{
    public class AppItemsResult
    {
        public string appId;
        public string name;
        public string description;
        public string team;
        public string teamId;
        public string[] categories;
        public string releaseDate;
        public AppPlatform[] platform;
        public string status;
        public string appStore;
        public string engine;
        public string website;
        public string[] gallery;
        public string createdBy;
        public string createdAt;
        public string updatedAt;
        
        public ItemResult[] items;
    }
}