using _1.WorkingMVC.Areas.Admin.Interfaces;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
namespace _1.WorkingMVC.Areas.Admin.Services;

public class ImageService : IImageService
{
	private readonly IConfiguration configuration;

	public ImageService(IConfiguration configuration)
	{
		this.configuration = configuration;
	}

	public async Task<string> UploadImageAsync(IFormFile file)
	{
		try
		{
			using var ms = new MemoryStream();
			await file.CopyToAsync(ms);
			var fileName = Path.GetRandomFileName() + ".webp";
			var bytes = ms.ToArray();
			using var image = Image.Load(bytes);
			image.Mutate(x => x.Resize(new ResizeOptions
			{
				Size = new Size(600, 600),
				Mode = ResizeMode.Max
			}));

			var dirImageName = configuration["DirImageName"] ?? "Image";
			var path = Path.Combine(Directory.GetCurrentDirectory(), dirImageName, fileName);
			await image.SaveAsync(path, new WebpEncoder());
			
			return fileName;
		}
		catch
		{
			return String.Empty;
		}
	}
}
