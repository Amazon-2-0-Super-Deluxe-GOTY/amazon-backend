
using Amazon.S3;
using Amazon.S3.Model;


namespace amazon_backend.Services.AWSS3
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3client;
        private readonly S3Settings _settings;
        private readonly ILogger<S3Service> _logger;
        public S3Service(IAmazonS3 s3client, S3Settings settings, ILogger<S3Service> logger)
        {
            _s3client = s3client;
            _settings = settings;
            _logger = logger;
        }
        public async Task<bool> DeleteFile(string filePath)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.bucketName,
                    Key = filePath
                };

                var response = await _s3client.DeleteObjectAsync(deleteObjectRequest);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;
        }

        public async Task<string> UploadFile(IFormFile file, string filePath)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var tempPath = Path.Combine(Path.GetTempPath(), fileName);
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var key = filePath + "/" + fileName;
                PutObjectRequest request = new()
                {
                    BucketName = _settings.bucketName,
                    Key = key,
                    FilePath = tempPath
                };
                PutObjectResponse res = await _s3client.PutObjectAsync(request);
                File.Delete(tempPath);
                return key;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<List<string>> UploadFilesFromRange(List<IFormFile> files,string filePath)
        {
            List<string> filePaths = new();
            foreach (var file in files)
            {
                try
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var tempPath = Path.Combine(Path.GetTempPath(), fileName);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var key = filePath + "/" + fileName;
                    PutObjectRequest request = new()
                    {
                        BucketName = _settings.bucketName,
                        Key = key,
                        FilePath = tempPath
                        };
                    PutObjectResponse res = await _s3client.PutObjectAsync(request);
                    filePaths.Add(key);
                    File.Delete(tempPath);
                }
                catch (AmazonS3Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }
            return filePaths;
        }
    }
}
