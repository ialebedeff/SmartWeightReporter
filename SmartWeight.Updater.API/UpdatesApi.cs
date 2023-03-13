using Entities;

namespace SmartWeight.Updater.API
{
    /// <summary>
    /// Апи для работы с обновлениями
    /// </summary>
    public class UpdatesApi : RestApiClientBase
    {
        public UpdatesApi(HttpClient httpClient) : base(httpClient) { }
        /// <summary>
        /// Получить информацию по сборке
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        public Task<Build?> GetBuildInformationAsync(int buildNumber)
            => GetAsync<Build>($"api/updates/GetBuildInformation?buildNumber={buildNumber}");
        /// <summary>
        /// Получить информацию по всем сборкам
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Build>?> GetBuildsAsync()
            => GetAsync<IEnumerable<Build>?>("api/updates/GetBuilds");
        /// <summary>
        /// Получить информацию по файлам сборки
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <param name="binaryFileInformation"></param>
        /// <returns></returns>
        public Task<IEnumerable<BuildFileData>?> GetBuildFilesAsync(
            int buildNumber,
            IEnumerable<BinaryFileInformation> binaryFileInformation)
            => PostAsync<IEnumerable<BinaryFileInformation>, IEnumerable<BuildFileData>>(
                $"api/updates/GetBuildFiles?buildNumber={buildNumber}", binaryFileInformation);
        /// <summary>
        /// Скачать сборку
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <param name="buildFileNumber"></param>
        /// <returns></returns>
        public Task<byte[]?> GetBuildFileAsync(
            int buildNumber, int buildFileNumber)
            => DownloadAsync($@"api/updates/GetBuildFile?buildNumber={buildNumber}&buildFileNumber={buildFileNumber}");
    }
}