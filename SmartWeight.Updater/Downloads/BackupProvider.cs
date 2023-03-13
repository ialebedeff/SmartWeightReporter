using Entities;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    /// <summary>
    /// Бэкапер для Базы Данных
    /// </summary>
    public class BackupProvider : UpdateProvider
    {
        private Action<ProgressState>? _onProgressChanged;
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            _onProgressChanged = onProgressChanged;
            await BackupAsync();
        }

        /// <summary>
        /// Сделать бэкап БД
        /// </summary>
        /// <returns></returns>
        private async Task BackupAsync()
        {
            // Путь до бэкапа БД C:\Users\Public\Documents\Backups\Database\xx_xx_xxxx xx_xx_xx.sql
            var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Backups", "Database", DateTime.Now.ToString("dd_MM_yyyy hh_mm_ss.sql"));
            var builder = new MySqlConnectionStringBuilder();
           
            // ToDo: изменить билд строки подключения к бд
            // т.к не у всех может быть она, нужно сделать 
            // получение командной строки из личного кабинета,
            // убрать хардкод
            builder.Server = "localhost";
            builder.UserID = "root";
            builder.Password = "1q2w3e4r5T";
            builder.Database = "shark_vesy1";

            using (MySqlConnection conn = new MySqlConnection(builder.GetConnectionString(true)))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;

                        await conn.OpenAsync();

                        mb.ExportProgressChanged += Mb_ExportProgressChanged;
                        mb.ExportToFile(backupPath);

                        await conn.CloseAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Прогресс бэкапа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mb_ExportProgressChanged(object sender, ExportProgressArgs e)
        {
            _onProgressChanged?.Invoke(new ProgressState(
                e.TotalTables,
                e.CurrentTableIndex, 
                e.CurrentTableName, $"Копирование таблиц: {e.CurrentTableIndex} из {e.TotalTables}. {e.CurrentTableName}"));
        }
    }
}
