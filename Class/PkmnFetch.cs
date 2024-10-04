namespace Pokefeet.Class;

public class PkmnFetch
{
	readonly HttpClient _httpClient;

	public PkmnFetch(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<PokemonJson?> GetDailyDataAsync()
	{
		try
		{
			return await _httpClient.GetFromJsonAsync<PokemonJson>("https://pkfapi.ushysder.me/daily");
		}
		catch (HttpRequestException)
		{
			return null;
		}
	}

	public async Task<PokemonInfo?> GetPokemonInfo(string id)
	{
		try
		{
			return await _httpClient.GetFromJsonAsync<PokemonInfo>($"https://pkfapi.ushysder.me/pokemon/{id}");
		}
		catch (HttpRequestException)
		{
			return null;
		}
	}

	public async Task<List<PokemonInfo>?> GetAllPokemons()
	{
		try
		{
			return await _httpClient.GetFromJsonAsync<List<PokemonInfo>>($"https://pkfapi.ushysder.me/all");
		}
		catch (HttpRequestException)
		{
			return null;
		}
	}

	public class PokemonJson
	{
		public DateTime Date { get; set; }
		public string? Name { get; set; }
	}

	public class PokemonInfo
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Type1 { get; set; }
		public string? Type2 { get; set; }
		public string? Color { get; set; }
		public bool IsLegendary { get; set; }
		public bool IsMythical { get; set; }
		public string? Generation { get; set; }
		public string? ImgUrl { get; set; }
	}
}
