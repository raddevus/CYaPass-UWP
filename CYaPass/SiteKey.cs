using System;

namespace CYaPass
{
    class SiteKey
    {
        public String Key { get; set; }
        public SiteKey(String key)
        {
            this.Key = key;
        }
        public override string ToString()
        {
            return this.Key;
        }
    }
}
