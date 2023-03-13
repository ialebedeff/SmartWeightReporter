using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class HashHelper
    {
        public static async Task<IEnumerable<string>> GetFilesHashesAsync(IEnumerable<string> paths, Action<ProgressState>? onProgressChanged = null)
        {
            var hashes = new List<string>();

            foreach (var path in paths)
            {
                onProgressChanged?.Invoke(new ProgressState(path, $"Проверка файла: {path}"));
                hashes.Add(await GetFileHashAsync(path));
            }

            return hashes;
        }
        public static async Task<string> GetFileHashAsync(string filePath)
        {
            using (var hash = SHA256.Create())
            using (var stream = new FileStream(filePath, FileMode.Open))
                return Convert.ToBase64String(await hash.ComputeHashAsync(stream));
        }
    }
}
