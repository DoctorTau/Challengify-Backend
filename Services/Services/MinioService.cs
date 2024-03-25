using DotNetEnv;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace Challengify.Services;

public class MinioService : IFileService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioService()
    {
        Env.Load(Path.Combine("..", ".env")); // Load the .env file at the application startup

        var endpoint = "minio:9000";
        var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        var secretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("MinIO access key and/or secret key are not set in environment variables.");
        }

        _minioClient = new MinioClient().WithEndpoint(endpoint)
                                        .WithCredentials(accessKey, secretKey)
                                        .WithSSL(false)
                                        .Build();

        _bucketName = Environment.GetEnvironmentVariable("MINIO_BUCKET_NAME")!;
    }

    public async Task DeleteFileAsync(string path)
    {
        await CheckObjectExists(path);

        RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs().WithBucket(_bucketName)
                                                                 .WithObject(path);

        await _minioClient.RemoveObjectAsync(removeObjectArgs);
    }

    public async Task<Stream> DownloadFileAsync(string path)
    {
        var memoryStream = new MemoryStream();
        await CheckObjectExists(path);

        GetObjectArgs getObjectArgs = new GetObjectArgs().WithBucket(_bucketName)
                                                         .WithObject(path).WithCallbackStream((stream) =>
                                                         {
                                                             stream.CopyTo(memoryStream);
                                                         });

        await _minioClient.GetObjectAsync(getObjectArgs);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task UploadFileAsync(string path, Stream data, string contentType)
    {
        await _minioClient.PutObjectAsync(new PutObjectArgs().WithBucket(_bucketName)
                                                             .WithObject(path)
                                                             .WithStreamData(data)
                                                             .WithObjectSize(data.Length)
                                                             .WithContentType(contentType));
    }

    private async Task CheckObjectExists(string path)
    {
        StatObjectArgs statObjectArgs = new StatObjectArgs().WithBucket(_bucketName)
                                                            .WithObject(path);

        var result = await _minioClient.StatObjectAsync(statObjectArgs);
        if (result == null || result.Size == 0)
        {
            throw new KeyNotFoundException("File not found");
        }
    }
}