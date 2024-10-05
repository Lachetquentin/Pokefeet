using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokeApiNet;

namespace Pokefeet.Class;

public class PokemonData
{
	public static async Task DownloadPkmnImage()
	{
		const string jsonFilePath = "D:\\Projets\\PokefeetAPI\\data\\pokemonData.json";

		try
		{
			string jsonText = await File.ReadAllTextAsync(jsonFilePath);

			JArray jsonArray = JArray.Parse(jsonText);

			using (HttpClient httpClient = new HttpClient())
			{
				foreach (var jsonItem in jsonArray)
				{
					string imageUrl = jsonItem["ImgUrl"]?.ToString();

					if (imageUrl == null) continue;

					string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

					string localPath = Path.Combine("D:\\Projets\\PokefeetAPI\\assets\\", fileName);

					if (!File.Exists(localPath))
						await Helper.DownloadImage(httpClient, imageUrl, localPath);
					else
						Console.WriteLine($"Image '{fileName}' already exists. Skipping download.");
				}
			}

			Console.WriteLine("Images downloaded successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
		}
	}

	/// <summary>
	/// Generate the random daily list pokemon (1 day 1 pokemon) for every pokemon footprint in img/footprint
	/// Generate pokemon.json
	/// </summary>
	public static void GeneratePokemonData()
	{
		const string outputPath = "D:\\Projets\\PokefeetAPI\\data\\pokemon.json";

		const string folderPath = "D:\\Projets\\PokefeetAPI\\img\\footprints\\";

		List<string> fileNames = Directory.GetFiles(folderPath)
			.Select(Path.GetFileNameWithoutExtension)
			.Where(name => !string.IsNullOrEmpty(name))
			.ToList();

		if (fileNames.Count == 0) return;

		List<string> pokemonList = new(fileNames);
		pokemonList = Helper.ShuffleList(pokemonList);

		DateTime currentDate = DateTime.Today;

		List<PokemonDailiesInfo> finalPokemonList = [];

		for (int i = 0; i < pokemonList.Count; i++)
		{
			DateTime date = currentDate.AddDays(i);
			finalPokemonList.Add(new PokemonDailiesInfo { Date = date, Name = pokemonList[i] });
		}

		JsonSerializerSettings settings = new()
		{
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateTimeZoneHandling = DateTimeZoneHandling.Utc
		};

		// Serialize the final Pokemon list to JSON
		string json = JsonConvert.SerializeObject(finalPokemonList, settings);

		// Write the JSON data to the output file
		File.WriteAllText(outputPath, json);
	}

	public static List<PokemonInfo> GetAllPokemons()
	{
		var emptyList = new List<PokemonInfo>();

		try
		{
			const string jsonFileName = "pokemonData.json";
			string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", jsonFileName);

			if (File.Exists(jsonPath))
			{
				string json = File.ReadAllText(jsonPath);

				List<PokemonInfo> pokemonList = JsonConvert.DeserializeObject<List<PokemonInfo>>(json);

				if (pokemonList != null) return pokemonList;

				Console.WriteLine("PokemonData - PokemonList is null");
				return emptyList;
			}

			Console.WriteLine($"PokemonData - JSON file '{jsonFileName}' not found at path '{jsonPath}'.");
			return emptyList;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"PokemonData - An error occurred: {ex.Message}");
			return emptyList;
		}
	}

	/// <summary>
	/// Return the pokemon on the period given.
	/// </summary>
	/// <param name="period"></param>
	/// <returns></returns>
	public static PokemonDailiesInfo GetDailyPokemon(string period)
	{
		var emptypkmn = new PokemonDailiesInfo(DateTime.Now, "_");

		try
		{
			const string jsonFileName = "pokemon.json";
			string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", jsonFileName);

			if (!File.Exists(jsonPath))
			{
				Console.WriteLine($"PokemonDailies - JSON file '{jsonFileName}' not found at path '{jsonPath}'.");
				return emptypkmn;
			}

			string json = File.ReadAllText(jsonPath);

			JsonSerializerSettings settings = new()
			{
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc
			};

			List<PokemonDailiesInfo> pokemonList = JsonConvert.DeserializeObject<List<PokemonDailiesInfo>>(json, settings);

			if (pokemonList == null)
			{
				Console.WriteLine("PokemonDailies - PokemonList is null");
				return emptypkmn;
			}

			switch (period)
			{
				case "today":
				{
					PokemonDailiesInfo dailyPokemonDailies = pokemonList.FirstOrDefault(pkmn => pkmn.Date.Date == DateTime.UtcNow.Date);

					if (dailyPokemonDailies != null) return dailyPokemonDailies;

					Console.WriteLine("DailyPokemon is null");
					return emptypkmn;
				}
				case "yesterday":
				{
					PokemonDailiesInfo yesterdayPokemonDailies = pokemonList.FirstOrDefault(pkmn => pkmn.Date.Date == DateTime.UtcNow.Date.AddDays(-1));

					if (yesterdayPokemonDailies != null) return yesterdayPokemonDailies;

					Console.WriteLine("YesterdayPokemon is null");
					return emptypkmn;
				}
				default:
					Console.WriteLine("Period unknown.");
					return emptypkmn;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
			return emptypkmn;
		}
	}

	/// <summary>
	/// Get Pokemon info with the ID given.
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static PokemonInfo GetPokemon(string id)
	{
		var emptypkmn = new PokemonInfo();

		try
		{
			const string jsonFileName = "pokemonData.json";
			string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", jsonFileName);

			if (File.Exists(jsonPath))
			{
				string json = File.ReadAllText(jsonPath);

				List<PokemonInfo> pokemonList = JsonConvert.DeserializeObject<List<PokemonInfo>>(json);

				if (pokemonList != null) return pokemonList.Find(pkmn => pkmn.Id.ToString() == id);

				Console.WriteLine("PokemonData - PokemonList is null");
				return emptypkmn;
			}

			Console.WriteLine($"PokemonData - JSON file '{jsonFileName}' not found at path '{jsonPath}'.");
			return emptypkmn;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"PokemonData - An error occurred: {ex.Message}");
			return emptypkmn;
		}
	}

	/// <summary>
	/// Fetch informations for every pokemon in img/footprint
	/// Generate pokemonData.json
	/// </summary>
	/// <returns></returns>
	public static async Task GetPokemonDataFromApi()
	{
		const string outputPath = "D:\\Projets\\PokefeetAPI\\data\\pokemonData.json";
		const string folderPath = "D:\\Projets\\PokefeetAPI\\img\\footprints\\";
		PokeApiClient pokeClient = new();

		List<string> fileNames = Directory.GetFiles(folderPath)
			.Select(Path.GetFileNameWithoutExtension)
			.Where(name => !string.IsNullOrEmpty(name))
			.ToList();

		if (fileNames.Count == 0) return;

		List<string> pokemonList = new (fileNames);

		List<PokemonInfo> finalPokemonList = [];

		int totalPokemonCount = pokemonList.Count;

		Stopwatch stopwatch = new();
		stopwatch.Start();

		for (int i = 1; i <= totalPokemonCount; i++)
		{
			var pkmn = await pokeClient.GetResourceAsync<Pokemon>(i);
			PokemonSpecies species = await pokeClient.GetResourceAsync(pkmn.Species);
			List<PokeApiNet.Type> type = await pokeClient.GetResourceAsync(pkmn.Types.Select(t => t.Type));
			PokemonColor color = await pokeClient.GetResourceAsync(species.Color);

			List<string> pkmnType = type.Select(t => t.Names[3].Name).ToList();

			var type2 = pkmnType.Count > 1 ? pkmnType[1] : "Aucun";
			
			string frenchcolor = color.Names[3].Name;

			finalPokemonList.Add(new PokemonInfo
			{
				Id = pkmn.Id,
				Name = species.Names[4].Name,
				Type1 = pkmnType[0],
				Type2 = type2,
				Color = frenchcolor,
				IsLegendary = species.IsLegendary,
				IsMythical = species.IsMythical,
				Generation = species.Generation.Name.Replace("generation-", "").ToUpper(),
				ImgUrl = pkmn.Sprites.FrontDefault
			});

			var progress = (i * 100) / totalPokemonCount;
			Console.Write($"\rProgress: {progress}% | Elapsed Time: {stopwatch.Elapsed:hh\\:mm\\:ss}");
			Console.SetCursorPosition(0, Console.CursorTop);
		}

		stopwatch.Stop();
		TimeSpan elapsedTime = stopwatch.Elapsed;
		Console.WriteLine($"\nData has been generated in {elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}");

		string json = JsonConvert.SerializeObject(finalPokemonList);

		await File.WriteAllTextAsync(outputPath, json);
	}

	public class PokemonDailiesInfo
	{
		public PokemonDailiesInfo() { }

		public PokemonDailiesInfo(DateTime date, string name)
		{
			Date = date;
			Name = name;
		}

		public DateTime Date { get; set; }
		public string Name { get; set; }
	}

	public class PokemonInfo
	{
		public string Color { get; set; }
		public string Generation { get; set; }
		public int Id { get; set; }
		public string ImgUrl { get; set; }
		public bool IsLegendary { get; set; }
		public bool IsMythical { get; set; }
		public string Name { get; set; }
		public string Type1 { get; set; }
		public string Type2 { get; set; }
	}
}
