using Pokefeet2.Ressources;
using System.Diagnostics;
using SkiaSharp;

namespace Pokefeet2.Class;

public static class ImageLoader
{
	/// <summary>
	/// The cached base 64 images that were already loaded from the resources.
	/// </summary>
	static readonly Dictionary<string, string> _base64imagesByNameSprite = new();
	static readonly Dictionary<string, string> _base64imagesByNameFootprint = new();

	/// <summary>
	/// Gets the base 64 image from a resource name in <see cref="Img"/> or <see cref="Footprint"/>.
	/// If the image cannot be found, <see cref="Img._1"/> or <see cref="Footprint._1"/> is used.
	/// </summary>
	public static string GetBase64Image(string imageName, bool isSprite)
	{
		Dictionary<string, string> imageDictionary = isSprite ? _base64imagesByNameSprite : _base64imagesByNameFootprint;

		if (imageDictionary.TryGetValue(imageName, out var image))
			return image;

		byte[] byteArray;

		try
		{
			byteArray = (byte[])(isSprite ? Img.ResourceManager : Footprint.ResourceManager).GetObject(imageName) ?? (isSprite ? Img._1 : Footprint._1);
		}
		catch (Exception ex)
		{
			byteArray = isSprite ? Img._1 : Footprint._1;
			Debug.Fail($"Cannot find or load image {imageName}", ex.Message);
		}

		string base64 = GetBase64Image(byteArray, SKEncodedImageFormat.Png);

		imageDictionary[imageName] = base64;
    
		return base64;
	}

	/// <summary>
	/// Gets the base 64 image from a <c>byte[]</c>.
	/// </summary>
	/// <returns></returns>
	public static string GetBase64Image(byte[] img, SKEncodedImageFormat format)
	{
		string mediaType = GetMimeType(format);
		string base64Img = ConvertToBase64(img);
		return $"data:{mediaType};base64,{base64Img}";
	}

	static string ConvertToBase64(byte[] image) => Convert.ToBase64String(image);

	/// <summary>
	/// Gets the MIME type corresponding to a SkiaSharp format.
	/// </summary>
	static string GetMimeType(SKEncodedImageFormat format)
	{
		return format switch
		{
			SKEncodedImageFormat.Jpeg => "image/jpeg",
			SKEncodedImageFormat.Png => "image/png",
			SKEncodedImageFormat.Gif => "image/gif",
			SKEncodedImageFormat.Bmp => "image/bmp",
			_ => string.Empty
		};
	}
}
