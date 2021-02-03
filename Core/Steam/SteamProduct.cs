using System.IO;

namespace Core
{
    public class SteamProduct
    {
        protected string _location;

        public SteamProduct(string pLocation)
        {
            _location = pLocation;
        }

        public virtual bool Validate()
        {
            return Directory.Exists(_location);
        }

        public string Location => _location;
    }
}
