using System.Globalization;
using System.Text;
using Pokefeet2.Ressources;

namespace Pokefeet2.Class;

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
}
