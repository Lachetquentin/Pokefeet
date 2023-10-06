namespace Pokefeet2.Class;

public class Constants
{
	public const string AppName = "Pokefeet";

	public class Javascript
	{
		public const string AddPokemonGuess = "addPokemonGuess";
		public const string CheckDaily = "checkDaily";
		public const string CheckHasWinClassic = "checkHasWinClassic";
		public const string CheckLastPlayedDate = "checkLastPlayedDate";
		public const string CheckPokemonGuesses = "checkPokemonGuesses";
		public const string GetLocal = "getLocal";
		public const string HasWinClassic = "hasWinClassic";
		public const string Import = "import";
		public const string ImportPath = "./scripts/localstorage.js";
		public const string PokemonGuesses = "pokemonGuesses";
		public const string SaveLocal = "saveLocal";
	}

	public class Path
	{
		public const string DefaultFootprint = "/img/footprints/1.png";
		public const string RootFootprint = "/img/footprints/";
		public const string RootSprite = "/img/sprites/";
	}

	public class Player
	{
		public const int MaxHp = 8;
		public const int Points = 0;
	}

	public class PokemonCategories
	{
		public const string Color = "Color";
		public const string Generation = "Generation";
		public const string IsLegendary = "IsLegendary";
		public const string IsMythical = "IsMythical";
		public const string Type1 = "Type1";
		public const string Type2 = "Type2";
	}

	public class Url
	{
		public const string Daily = "/daily";
		public const string Home = "/";
		public const string Infinite = "/infinite";
	}
}
