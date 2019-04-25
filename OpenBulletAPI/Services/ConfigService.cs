using Microsoft.Extensions.Configuration;
using OpenBulletAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;

namespace OpenBulletAPI.Services
{
    public class ConfigService
    {
        private readonly string _configFolder;

        public ConfigService(IConfiguration configuration)
        {
            _configFolder = configuration.GetConnectionString("configFolder");
        }

        public MemoryStream Get(string[] groups)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    // List all the configs the user can access
                    foreach (var group in groups.Where(g => g.Trim() != ""))
                    {
                        var dir = Path.Combine(_configFolder, group);
                        if (Directory.Exists(dir))
                        {
                            foreach(var file in Directory.EnumerateFiles(dir).Where(file => file.EndsWith(".loli")))
                            {
                                var zipArchiveEntry = archive.CreateEntry(Path.GetFileName(file), CompressionLevel.Fastest);
                                var fileContent = File.ReadAllBytes(file);
                                using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(fileContent, 0, fileContent.Length);
                            }
                        }
                    }
                }

                return ms;
            }
        }

        /*
        public Config Get(string id)
        {
            return _configs.Find(config => config.Id == id).FirstOrDefault();
        }

        public Config Create(Config config)
        {
            _configs.Insert(config);
            return config;
        }

        public void Update(string id, Config config)
        {
            _configs.Update(config);
        }

        public void Remove(Config config)
        {
            _configs.Delete(c => c.Id == config.Id);
        }

        public void Remove(string id)
        {
            _configs.Delete(config => config.Id == id);
        }
        */
    }
}
