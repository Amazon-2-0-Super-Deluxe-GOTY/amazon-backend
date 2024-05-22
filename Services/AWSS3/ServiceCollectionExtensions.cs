using Amazon.Runtime;
using Amazon.S3;

namespace amazon_backend.Services.AWSS3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddS3Client(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var s3Settings = new S3Settings();
                configuration.GetSection("S3Settings").Bind(s3Settings);

                var awsCredentials = new BasicAWSCredentials(s3Settings.accessKey, s3Settings.secretKey);

                var s3Client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.GetBySystemName(s3Settings.region));

                services.AddSingleton<IAmazonS3>(s3Client);
                services.AddSingleton(s3Settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting up AWS S3 client");
            }

            return services;
        }
    }
}
