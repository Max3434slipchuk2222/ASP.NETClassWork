namespace _1.WorkingMVC.Areas.Admin.Interfaces;

public interface IImageService
{
	public Task<string> UploadImageAsync(IFormFile file);
}
