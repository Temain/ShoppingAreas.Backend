using System.Threading;
using System.Threading.Tasks;

namespace ShoppingAreas.Web.Services
{
	public interface IImageService
    {
		/// <summary>
		/// Получение изображения по относительному пути.
		/// </summary>
		/// <returns>Избражение в формате Base64.</returns>
		Task<string> GetImageAsync(string path, CancellationToken cancellationToken);

		/// <summary>
		/// Загрузка изображения в формате Base64 на сервер.
		/// </summary>
		/// <returns>Возвращает относительный путь к изображению.</returns>
		Task UploadImageAsync(string path, string base64image, CancellationToken cancellationToken);

		/// <summary>
		/// Удаление изображения по относительному пути.
		/// </summary>
		Task DeleteImageAsync(string path, CancellationToken cancellationToken);
	}
}
