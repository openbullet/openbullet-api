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
                        
                        ZipFolderRecursively(archive, dir, _configFolder);
                    }
                }

                return ms;
            }
        }

        private void ZipFolderRecursively(ZipArchive archive, string currentDir, string baseDir)
        {
            // Add all the files
            foreach (var file in Directory.EnumerateFiles(currentDir).Where(file => file.EndsWith(".loli")))
            {
                // Create the file entry and write the file content
                var zipArchiveEntry = archive.CreateEntry($"{file.Substring(baseDir.Length + 1)}", CompressionLevel.Fastest);
                var fileContent = File.ReadAllBytes(file);
                using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(fileContent, 0, fileContent.Length);
            }

            // Add subfolders recursively
            foreach (var dir in Directory.EnumerateDirectories(currentDir))
            {
                ZipFolderRecursively(archive, dir, baseDir);
            }
        }
        
        public async Task<bool> Upload(Stream file, string group, string name)
        {
            if (group != "" && name.EndsWith(".loli"))
            {
                var dir = Path.Combine(_configFolder, group);
                Directory.CreateDirectory(dir);
                var comb = Path.Combine(dir, Path.GetFileName(name));

                using (var stream = new FileStream(comb, System.IO.FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    return true;
                }
            }

            return false;
        }
    }
}
