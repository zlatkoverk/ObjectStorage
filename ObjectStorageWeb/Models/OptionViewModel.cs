namespace ObjectStorageWeb.Models
{
    public class OptionViewModel
    {
        public dynamic Object { get; set; }

        public string Id
        {
            get => Object.Id.ToString();
        }
        
        public string Value
        {
            get => Object.ToString();
        }

        public override string ToString()
        {
            return Object.ToString();
        }
    }
}