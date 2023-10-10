using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Pokefeet2.Class;
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
	Dictionary<int, PokemonInfo> _pokemonDictionary = new();

	string? ImgPath { get; set; }
	string? PlayerAnswer { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_apiResponse = await PkmnFetchApi.GetDailyDataAsync();
			_pokemons = await PkmnFetchApi.GetAllPokemons() ?? new List<PokemonInfo>();
			_pokemonDictionary = _pokemons.ToDictionary(p => p.Id);

			if (_apiResponse == null) ImgPath = ImageLoader.GetBase64Image("1", false);
			if (_apiResponse?.Name == "_") ImgPath = ImageLoader.GetBase64Image("1", false);

			ImgPath = ImageLoader.GetBase64Image(_apiResponse.Name, false);

			if (_apiResponse?.Name != null)
			{
				var pkmnInfo = _pokemonDictionary.FirstOrDefault(p => p.Value.Id.ToString().Equals(_apiResponse.Name)).Value;

				_pokemonInfo = new PokemonInfo
				{
					Id = pkmnInfo.Id,
					Name = pkmnInfo.Name,
					Type1 = pkmnInfo.Type1,
					Type2 = pkmnInfo.Type2,
					Color = pkmnInfo.Color,
					ImgUrl = pkmnInfo.ImgUrl,
					Generation = pkmnInfo.Generation,
					IsLegendary = pkmnInfo.IsLegendary,
					IsMythical = pkmnInfo.IsMythical
				};
			}
		
			if (_pokemonInfo?.Name != null)
			{
				_pokemonName = _pokemonInfo.Name;
				_pokemonInfo.Name = _pokemonInfo.Name.ToLower().Trim();
				_pokemonInfo.Name = Helper.RemoveDiacritics(_pokemonInfo.Name);
			}
			
			_jsRef = await Js.InvokeAsync<IJSObjectReference>(Constants.Javascript.Import, Constants.Javascript.ImportPath);
			
			if (_jsRef != null)
			{
				_hasWinClassic = await _jsRef.InvokeAsync<string>(Constants.Javascript.GetLocal, Constants.Javascript.HasWinClassic);

				string? pokemonGuessesJson = await _jsRef.InvokeAsync<string>(Constants.Javascript.GetLocal, Constants.Javascript.PokemonGuesses);

				if (!string.IsNullOrEmpty(pokemonGuessesJson))
				{
					int[]? pokemonGuesses = JsonSerializer.Deserialize<int[]>(pokemonGuessesJson);

					if (pokemonGuesses.Length > 0)
					{
						_guessStarted = true;
						foreach (int id in pokemonGuesses)
						{
							if (_pokemonDictionary.TryGetValue(id, out var pkmn))
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
					item.Name != null && Helper.RemoveDiacritics(item.Name).StartsWith(Helper.RemoveDiacritics(PlayerAnswer), StringComparison.OrdinalIgnoreCase) && _pokemonList.All(p => p.Id != item.Id))
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

			PlayerAnswer = Helper.RemoveDiacritics(PlayerAnswer);

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

			PokemonInfo pkmn = _pokemonDictionary.FirstOrDefault(p => p.Value.Id == pokemonInfo.Id).Value;
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
}
