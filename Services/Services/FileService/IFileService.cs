namespace Challengify.Services;

/// <summary>
/// Represents a service for managing files.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Uploads a file to the specified path.
    /// </summary>
    /// <param name="path">The path where the file will be uploaded.</param>
    /// <param name="data">The data stream of the file.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UploadFileAsync(string path, Stream data, string contentType);

    /// <summary>
    /// Downloads a file from the specified path.
    /// </summary>
    /// <param name="path">The path of the file to download.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the downloaded file stream.</returns>
    public Task<Stream> DownloadFileAsync(string path);

    /// <summary>
    /// Deletes a file from the specified path.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteFileAsync(string path);
}