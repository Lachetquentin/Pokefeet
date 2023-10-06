using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Pokefeet2.Class;
using Pokefeet2.Ressources;
using static Pokefeet2.Class.PkmnFetch;

namespace Pokefeet2.Components;

partial class DailyGame
{
	#region Inject

	[Inject] PkmnFetch PkmnFetchApi { get; set; } = default!;

	#endregion

	readonly Player _player = new();
	readonly List<PokemonInfo> _pokemonList = new();
	PokemonJson? _apiResponse;
	List<PokemonInfo> _filteredItems = new();
	bool _gameOver;
	bool _gameWon;
	bool _guessStarted;
	string _hasWinClassic = "";
	bool _isLoading = true;
	IJSObjectReference? _jsRef;
	PokemonInfo? _pokemonInfo;
	string? _pokemonName;
	List<PokemonInfo> _pokemons = new();

	string? ImgPath { get; set; }
	string? PlayerAnswer { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_jsRef = await Js.InvokeAsync<IJSObjectReference>(Constants.Javascript.Import, Constants.Javascript.ImportPath);
			
			if (_jsRef != null)
			{
				_hasWinClassic = await _jsRef.InvokeAsync<string>(Constants.Javascript.GetLocal, Constants.Javascript.HasWinClassic);

				_pokemons = await PkmnFetchApi.GetAllPokemons() ?? new List<PokemonInfo>();
				Dictionary<int, PokemonInfo> pokemonDictionary = _pokemons.ToDictionary(p => p.Id);

				string? pokemonGuessesJson = await _jsRef.InvokeAsync<string>(Constants.Javascript.GetLocal, Constants.Javascript.PokemonGuesses);

				if (!string.IsNullOrEmpty(pokemonGuessesJson))
				{
					int[]? pokemonGuesses = JsonSerializer.Deserialize<int[]>(pokemonGuessesJson);

					if (pokemonGuesses.Length > 0)
					{
						_guessStarted = true;
						foreach (int id in pokemonGuesses)
						{
							if (pokemonDictionary.TryGetValue(id, out var pkmn))
							{
								_pokemonList.Add(pkmn);
							}
						}
					}

					_player.RemoveLife(_pokemonList.Count);
				}
			}

			switch (_hasWinClassic)
			{
				case "1":
					_gameWon = true;
					break;
				case "2":
					_gameOver = true;
					break;
			}

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	protected override async Task OnInitializedAsync()
	{
		_apiResponse = await PkmnFetchApi.GetDailyDataAsync();

		if (_apiResponse == null) ImgPath = Constants.Path.DefaultFootprint;
		if (_apiResponse?.Name == "_") ImgPath = Constants.Path.DefaultFootprint;

		ImgPath = $"{Constants.Path.RootFootprint}{_apiResponse?.Name}.png";

		if (_apiResponse?.Name != null)
			_pokemonInfo = await PkmnFetchApi.GetPokemonInfo(_apiResponse.Name);

		if (_pokemonInfo?.Name != null)
		{
			_pokemonName = _pokemonInfo.Name;
			_pokemonInfo.Name = _pokemonInfo.Name.ToLower().Trim();
			_pokemonInfo.Name = RemoveDiacritics(_pokemonInfo.Name);
		}
	}

	static string RemoveDiacritics(string text)
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

	string GetRowClass(PokemonInfo pokemon, string propertyName)
	{
		const string green = "row-card green";
		const string red = "row-card red";

		switch (propertyName)
		{
			case Constants.PokemonCategories.Type1 when _pokemonInfo != null && pokemon.Type1 == _pokemonInfo.Type1:
			case Constants.PokemonCategories.Type2 when _pokemonInfo != null && pokemon.Type2 == _pokemonInfo.Type2:
			case Constants.PokemonCategories.Color when _pokemonInfo != null && pokemon.Color == _pokemonInfo.Color:
			case Constants.PokemonCategories.IsLegendary when _pokemonInfo != null && pokemon.IsLegendary == _pokemonInfo.IsLegendary:
			case Constants.PokemonCategories.IsMythical when _pokemonInfo != null && pokemon.IsMythical == _pokemonInfo.IsMythical:
			case Constants.PokemonCategories.Generation when _pokemonInfo != null && pokemon.Generation == _pokemonInfo.Generation:
				return green;
			default:
				return red;
		}
	}

	string GetSearchClass() => _filteredItems.Count > 0 ? "card-search mt-1" : "";

	void GoBack() => Navigation.NavigateTo(Constants.Url.Home);

	void Search()
	{
		_filteredItems = string.IsNullOrEmpty(PlayerAnswer)
			? new List<PokemonInfo>()
			: _pokemons
				.Where(item =>
					item.Name != null && RemoveDiacritics(item.Name).StartsWith(RemoveDiacritics(PlayerAnswer), StringComparison.OrdinalIgnoreCase) && _pokemonList.All(p => p.Id != item.Id))
				.ToList();
	}

	async Task SelectItem(PokemonInfo selectedItem)
	{
		PlayerAnswer = selectedItem.Name;
		await SubmitAnswer(selectedItem);
	}

	async Task SubmitAnswer(PokemonInfo pokemonInfo)
	{
		if (PlayerAnswer != null)
		{
			PlayerAnswer = PlayerAnswer.ToLower().Trim();

			PlayerAnswer = RemoveDiacritics(PlayerAnswer);

			Console.WriteLine(PlayerAnswer);

			if (_pokemonInfo != null && PlayerAnswer.Equals(_pokemonInfo.Name))
			{
				_gameWon = true;
				if (_jsRef != null) await _jsRef.InvokeVoidAsync(Constants.Javascript.SaveLocal, Constants.Javascript.HasWinClassic, "1");
			}
			else
			{
				_player.RemoveLife();

				if (_player.GetLife() == 0)
				{
					_gameOver = true;
					if (_jsRef != null) await _jsRef.InvokeVoidAsync(Constants.Javascript.SaveLocal, Constants.Javascript.HasWinClassic, "2");
				}
			}

			PokemonInfo pkmn = await PkmnFetchApi.GetPokemonInfo(pokemonInfo.Id.ToString());
			_pokemonList.Add(pkmn);

			if (_jsRef != null)
			{
				if (pkmn != null)
					await _jsRef.InvokeVoidAsync(Constants.Javascript.AddPokemonGuess, pkmn.Id);
			}

			_filteredItems.Clear();
		}

		PlayerAnswer = string.Empty;
		_guessStarted = true;
	}

	async Task ValidSubmit(KeyboardEventArgs keyboardEventArgs)
	{
		Search();

		if (keyboardEventArgs.Code != "Enter" || string.IsNullOrEmpty(PlayerAnswer)) return;

		if (_filteredItems.Count <= 0)
		{
			PlayerAnswer = string.Empty;
			return;
		}

		PlayerAnswer = _filteredItems.First().Name;
		await SubmitAnswer(_filteredItems.First());
	}

	static string GetPokemonImg(int pokemonId) => $"{Constants.Path.RootSprite}{pokemonId}.png";

	static string TranslateBool(bool value) => value ? Translation.Yes : Translation.No;
}
