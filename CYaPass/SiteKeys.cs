using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Windows.Storage;

namespace CYaPass
{
    class SiteKeys : List<SiteKey>, IPersistable
    {
        public String fileName { get; set; }
        public StorageFolder localFolder { get; set; }
        public String allJson { get; set; }
        public SiteKeys()
        {
            String exeDir = Directory.GetCurrentDirectory();
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            fileName = "cyapass.json";
        }
        public async Task<bool> Save()
        {

            String allSitesAsJson = JsonConvert.SerializeObject(this);
            StorageFile siteKeyFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(siteKeyFile, allSitesAsJson);

            return true;
        }

        public async Task<bool> Read()
        {
            if (File.Exists(Path.Combine(localFolder.Path, fileName)))
            {
                StorageFile siteKeyFile = await localFolder.GetFileAsync("cyapass.json");
                allJson = await FileIO.ReadTextAsync(siteKeyFile);
                return true;
            }

            return false;

        }
    }
}
