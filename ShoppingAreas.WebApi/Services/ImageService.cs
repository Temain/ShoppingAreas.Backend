using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ShoppingAreas.Web.Services
{
	public class ImageService : IImageService
    {
		private readonly IHostingEnvironment _hostingEnvironment;

		public ImageService(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
		}

		public async Task<string> GetImageAsync(string path, CancellationToken cancellationToken)
		{
			var wwwRoot = _hostingEnvironment.WebRootPath;
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new Exception("Ошибка при получении изображения. Неверный путь к файлу.");
			}

			try
			{
				var imagePath = Path.Combine(wwwRoot, path.Substring(1).Replace("/", @"\"));
				var imageArray = await File.ReadAllBytesAsync(imagePath, cancellationToken);

				return Convert.ToBase64String(imageArray);
			}
			catch (Exception ex)
			{
				throw new Exception("Ошибка при получении изображения.", ex);
			}
		}

		public async Task UploadImageAsync(string path, string base64image
			, CancellationToken cancellationToken)
		{
			var wwwRoot = _hostingEnvironment.WebRootPath;
			
			try
			{
				var imagePath = Path.Combine(wwwRoot, path.Substring(1).Replace("/", @"\"));
				var bytes = Convert.FromBase64String(base64image);
				await File.WriteAllBytesAsync(imagePath, bytes, cancellationToken);
			}
			catch(Exception ex)
			{
				throw new Exception("Ошибка при сохранении изображения.", ex);
			}
		}

		public async Task DeleteImageAsync(string path, CancellationToken cancellationToken)
		{
			try
			{
				File.Delete(path);
			}
			catch(Exception ex)
			{
				throw new Exception("Ошибка при удалении изображения.", ex);
			}
		}
	}
}
