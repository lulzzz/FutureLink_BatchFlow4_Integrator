using System;
using System.Collections.Generic;
using System.Text;
using FutureLink.BatchFlow4.Integrator.Info;
using System.Configuration;
using FutureLink.BatchFlow4.Integrator.API;
using System.Reflection;
using Data = System.Data;
using System.IO;
using FutureLink.BatchFlow4.Integrator.API.Helpers;
using System.Data;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;

namespace FutureLink.BatchFlow4.Integrator.BLL
{
    public class IntegratorClass
    {
        protected Info.LoggingClass _logger = new Info.LoggingClass("FutureLink.BatchFlow4.Integrator.BLL");
     
        public async Task<bool> DoRun(Info.ConfigurationInfo config)
        {
            try
            {
                if (config != null)
                {
                    string apiUrl = config.ApiUrl;
                    //var client = ApiHelper.GetAuthenticatedApiClient(apiUrl);
                    foreach (IntegrationConfigDetail row in config.Items)
                    {
                        string integrationType = row.Type;
                        switch (integrationType.ToLower())
                        {
                            case "import":
                                await doImport(row, apiUrl);
                                break;
                            case "export":
                                await doExport(row, apiUrl);
                                break;
                            default:
                                
                                break;
                        }
                    }
                   
                }
                else
                {

                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "DoRun", ex);
                return false;
            }
        }

        private async Task<bool> doExport(IntegrationConfigDetail config, string apiUrl)
        {
            try
            {
                Client client = await ApiHelper.GetAuthenticatedApiClient(apiUrl, config.Serial);
                ExportDataInfo data = await client.GetExportDataAsync().ConfigureAwait(false);

                string text = "";
                if (data.RestultValue != null)
                    text = System.Text.Encoding.UTF8.GetString(data.RestultValue);
                else
                    text = "";
                if (text.Length > 0)
                {
                    string result = saveDataToDisk(config.Destination, text, config.Type);
                    if (result.Length == 0)
                    {
                        await client.MarkExportDoneAsync(data.ProcId, data.DocIds);
                    } else
                    {
                        await client.MarkExportFailedAsync(data.ProcId, result, data.DocIds);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "doExport", ex);
                return false; ;
            }
            

        }

        private string saveDataToDisk(string folder, string data, string type)
        {
            try
            {
                string fileName = Path.Combine(Guid.NewGuid().ToString(), "." + string.Empty);
                string fullFileName = Path.Combine(folder, fileName);
                File.WriteAllText(fullFileName, data);
                return "";

            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "SaveDataToDisk", ex);
                return ex.Message;
            }
        }

        private async Task<bool> doImport(IntegrationConfigDetail config, string apiUrl)
        {
            try
            {
                Client client = (ApiHelper.GetAuthenticatedApiClient(apiUrl, config.Serial).Result) as Client;

                foreach (System.IO.FileInfo file in Directory.CreateDirectory(config.Source).GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    TimeSpan age = DateTime.UtcNow - new DateTime(Math.Max(file.CreationTimeUtc.Ticks, file.LastWriteTimeUtc.Ticks), DateTimeKind.Utc);
                    if (age.TotalMinutes > 1)
                    {
                        if (await importWorkOnFile(client, file, config.Format).ConfigureAwait(false))
                            file.MoveTo(Path.Combine(config.Succeeded, getNewFileName(file.Name)));
                        else
                            file.MoveTo(Path.Combine(config.Failed, getNewFileName(file.Name)));
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "doImport", ex);
                return false;
            }
        }

        private string getNewFileName(string fileName)
        {
            try
            {
                string cleanFileName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                return cleanFileName + "_" + Guid.NewGuid().ToString() + extension;

            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "getNewFileName", ex);
                return fileName;
            }
        }

        private async Task<bool> importWorkOnFile(Client client, System.IO.FileInfo file, string format)
        {
            try
            {
                string content = File.ReadAllText(file.FullName);
                switch (format.ToLower())
                {
                    case "oio":
                        await client.UploadGotoFileAsync(file.Name, "oio", content).ConfigureAwait(false);
                        break;
                    case "readsoft":
                        await client.UploadGotoFileAsync(file.Name, "oio", content).ConfigureAwait(false);
                        break;
                    case "xml":
                        await client.ImportMasterDataAsync(content).ConfigureAwait(false);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "importWorkOnFile", ex);
                return false;
            }
        }
         





    }
}
