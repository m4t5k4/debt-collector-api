using System.IO;
using System.Threading.Tasks;

namespace DebtCollector.Application.Common.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(string fileName, Stream fileStream);
        Task DeleteImageAsync(string blobUrl);
    }
}
