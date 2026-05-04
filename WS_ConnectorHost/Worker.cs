using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using WS_Haimdall.Model_Class;

namespace WS_ConnectorHost
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly appSettings _settings; 
        private static BusinessLayer bl; private PeriodicTimer? _timer;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks_Oth = new();
        public Worker(ILogger<Worker> logger, IOptions<appSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
            bl = new BusinessLayer(_settings.DB_Connection);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          
            await ConnectorHostLogs("Success: Service Started.");
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                
                try
                {
                    string[] IPMStextFIle = Directory.GetFiles(_settings.filepath, "*.txt");


                    if (IPMStextFIle.Count() != 0)
                    {

                        for (var i = 0; i < IPMStextFIle.Count(); i++)
                        {
                            string filename = Path.GetFileName(IPMStextFIle[i]);

                            DataTable dataTable = new DataTable();
                            List<string[]> rows = new List<string[]>();
                            using (StreamReader reader = new StreamReader(IPMStextFIle[i]))
                            {
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();
                                    string[] columns = line.Split(',');
                                    rows.Add(columns);
                                }
                            }
                            if (rows.Count > 0)
                            {
                                int sr = 1;
                                foreach (string column in rows[0])
                                {
                                    dataTable.Columns.Add("Value" + sr, typeof(string)); // Change the data type accordingly if needed
                                    sr++;
                                }

                                for (int j = 0; j < rows.Count; j++) // Start from index 1 to skip the header row
                                {
                                    dataTable.Rows.Add(rows[j]);
                                }
                            }

                            if (dataTable.Rows.Count > 0)
                            {
                                int filecode = Convert.ToInt32(bl.MaxFileCode());
                                try
                                {
                                    string jsonData = ConvertDataTableToJson(dataTable);

                                    bl.InsertProductionDetailsBulk(jsonData, filename,filecode );
                                    
                                    await ConnectorHostLogs($"File Ingested Successfully({filename})");
                                    File.Delete(IPMStextFIle[i]);
                                                                    
                                }
                                catch (Exception ex)
                                {
                                    bl.FileDelete(filecode);
                                    await ConnectorHostLogs($"Error while File Ingested({IPMStextFIle[i]}), details-{ex.Message.ToString()}");
                                }


                            }

                        }
                    }
                }
                catch (IOException ex)
                {
                    await ConnectorHostLogs(ex.ToString());
                }
            }
        }

        private string ConvertDataTableToJson(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt);
        }
        private async Task ConnectorHostLogs(string s)
        {
            #region New
            try
            {
                string currentDirectory = Path.Combine(_settings.LogPath, "ConnectorHostLogs");
                Directory.CreateDirectory(currentDirectory);

                string filePath = Path.Combine(currentDirectory, "ConnectorHostLogs.txt");

                // Get or create lock for this specific line
                var fileLock = _fileLocks_Oth.GetOrAdd("ConnectorHostLogs", _ => new SemaphoreSlim(1, 1));

                await fileLock.WaitAsync();

                try
                {
                    // Append log asynchronously
                    using (var stream = new FileStream(
                        filePath,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Read,
                        4096,
                        useAsync: true))
                    using (var writer = new StreamWriter(stream))
                    {
                        await writer.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {s}");
                    }

                    // Check file size
                    FileInfo fileInfo = new FileInfo(filePath);
                    long maxSizeInBytes = 1_000_000;

                    if (fileInfo.Exists && fileInfo.Length > maxSizeInBytes)
                    {
                        string newFileName = $"ConnectorHostLogs_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                        string newFilePath = Path.Combine(currentDirectory, newFileName);

                        File.Move(filePath, newFilePath);
                    }
                }
                finally
                {
                    fileLock.Release();
                }
            }
            catch
            {
                // log silently or handle properly
            }
            #endregion

        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            await ConnectorHostLogs("Service Stopping...");

            _timer?.Dispose();   // 🔥 Dispose timer here

            

            await base.StopAsync(cancellationToken);

            await ConnectorHostLogs("Service Stopped.");
        }
    }
}
