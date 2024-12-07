using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using SareeDesigns.Utils.ConfigOptions;

namespace SareeDesigns.Services
{
    public interface ICloudStorageService
    {
        Task<string> GetSignedUrlAsync(string fileNameToRead,int timeOutInMinutes=30);
        Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave);
        Task DeleteFileAsync(string fileNameToDelete);
    }
    public class CloudStorageService : ICloudStorageService
    {
        private readonly GCSConfigOptions _options;
        private readonly ILogger<CloudStorageService> _logger;
        private readonly GoogleCredential _googleCredential;

        public CloudStorageService(IOptions<GCSConfigOptions> options, ILogger<CloudStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == Environments.Production)
                {
                    // Store the json file in Secrets
                    _googleCredential = GoogleCredential.FromJson(_options.GCPStorageAuthFile);
                }
                else
                {
                    _googleCredential = GoogleCredential.FromFile(_options.GCPStorageAuthFile);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
                

        public async Task DeleteFileAsync(string fileNameToDelete)
        {
            try
            {
                using (var storageClient = StorageClient.Create(_googleCredential))
                {
                    await storageClient.DeleteObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToDelete);
                    _logger.LogInformation($"File {fileNameToDelete} deleted succesfully");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete file {fileNameToDelete}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetSignedUrlAsync(string fileNameToRead, int timeOutInMinutes = 30)
        {
            try
            {
                var sac = _googleCredential.UnderlyingCredential as ServiceAccountCredential;
                var urlSigner = UrlSigner.FromCredential(sac);
                //provides limited permission and time to make a request: time here is mentioned 30 minutes
                var signedUrl = await urlSigner.SignAsync(_options.GoogleCloudStorageBucketName, fileNameToRead,
                    TimeSpan.FromMinutes(timeOutInMinutes));
                _logger.LogInformation($"signed url obtained for file: {fileNameToRead}");
                return signedUrl.ToString();
            } catch (Exception ex)
            {
                _logger.LogError($"Error occured for obtaining signed URL for file: {fileNameToRead}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave)
        {
            try
            {
                _logger.LogInformation($"Uploading File: {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                using (var memoryStream = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(memoryStream);
                    //create storage client from google credential
                    using (var storageClient = StorageClient.Create(_googleCredential))
                    {
                        //upload file stream
                        var uploadedFile = await storageClient.UploadObjectAsync(_options.GoogleCloudStorageBucketName,
                            fileNameToSave, fileToUpload.ContentType, memoryStream);
                        _logger.LogInformation($"Uploaded File: {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                        return uploadedFile.MediaLink;

                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError($"Error while uploading file: {fileNameToSave}: {ex.Message}");
                throw;
            }
        }
    }
}
