using System.Diagnostics;
using System.Globalization;
using System.Text;
using Pokefeet.Ressources;

namespace Pokefeet.Class;

public static class Helper
{
	public static string RemoveDiacritics(string text)
	{
		string? normalizedString = text.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder();

		foreach (var c in normalizedString.EnumerateRunes())
		{
			var unicodeCategory = Rune.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}

		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	public static string GetPokemonImg(int pokemonId) => ImageLoader.GetBase64Image(pokemonId.ToString(), true);

	public static string TranslateBool(bool value) => value ? Translation.Yes : Translation.No;

	public static bool AreTypesEqual(string type1, string type2) => type1 == type2;

	public static async Task DownloadImage(HttpClient httpClient, string imageUrl, string localPath)
	{
		try
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

			if (response.IsSuccessStatusCode)
			{
				await using (Stream imageStream = await response.Content.ReadAsStreamAsync())
				await using (FileStream fileStream = File.Create(localPath))
				{
					await imageStream.CopyToAsync(fileStream);
				}

				stopwatch.Stop();
				Console.WriteLine($"Downloaded {Path.GetFileName(localPath)} in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
			}
			else
			{
				Console.WriteLine($"Error downloading image: {response.ReasonPhrase}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error downloading image: {ex.Message}");
		}
	}

	/// <summary>
	/// Randomize the order of pokemon
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static List<T> ShuffleList<T>(List<T> list)
	{
		Random rng = new();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[k], list[n]) = (list[n], list[k]);
		}
		return list;
	}
}
