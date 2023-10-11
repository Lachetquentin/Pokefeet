using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Pokefeet2.Class;
using static Pokefeet2.Class.PkmnFetch;

namespace Pokefeet2.Components;

partial class InfiniteGame
{
	#region Inject

	[Inject] PkmnFetch PkmnFetchApi { get; set; } = default!;

	#endregion

	readonly Player _player = new();
	readonly List<PokemonInfo> _pokemonList = new();
	List<PokemonInfo> _filteredItems = new();
	bool _gameOver;
	bool _gameWon;
	bool _error;
	bool _guessStarted;
	bool _isLoading = true;
	PokemonInfo? _pokemonInfo;
	string? _pokemonName;
	List<PokemonInfo> _pokemons = new();
	readonly List<PokemonInfo> _drawnPokemons = new();
	Dictionary<int, PokemonInfo> _pokemonDictionary = new();

	int _streak;
	int _pkmnCount;
	int _level = 1;

	string? ImgPath { get; set; }
	string? PlayerAnswer { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_pokemons = await PkmnFetchApi.GetAllPokemons() ?? new List<PokemonInfo>();
			_pokemonDictionary = _pokemons.ToDictionary(p => p.Id);
			GetRandomPokemon();
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	void GetRandomPokemon()
	{
		if (_pokemonDictionary.Count == 0)
		{
			_error = true;
			ImgPath = ImageLoader.GetBase64Image("1", false);
			return;
		}

		if (_drawnPokemons.Count == _pokemonDictionary.Count) _gameWon = true;
		
		var random = new Random();
		int randomIndex;
		do
		{
			randomIndex = random.Next(_pokemonDictionary.Count);
		} while (_drawnPokemons.Contains(_pokemonDictionary[randomIndex]));

		var pkmnInfo = _pokemonDictionary[randomIndex];

		_drawnPokemons.Add(pkmnInfo);

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

		ImgPath = ImageLoader.GetBase64Image(_pokemonInfo.Id.ToString(), false);
		
		if (_pokemonInfo.Name == null) return;

		_pokemonName = _pokemonInfo.Name;
		_pokemonInfo.Name = _pokemonInfo.Name.ToLower().Trim();
		_pokemonInfo.Name = Helper.RemoveDiacritics(_pokemonInfo.Name);
	}

	string GetRowClass(PokemonInfo pokemon, string propertyName)
	{
		const string green = "row-card green";
		const string red = "row-card red";

		switch (propertyName)
		{
			case Constants.PokemonCategories.Color when _pokemonInfo != null && pokemon.Color == _pokemonInfo.Color:
			case Constants.PokemonCategories.IsLegendary when _pokemonInfo != null && pokemon.IsLegendary == _pokemonInfo.IsLegendary:
			case Constants.PokemonCategories.IsMythical when _pokemonInfo != null && pokemon.IsMythical == _pokemonInfo.IsMythical:
			case Constants.PokemonCategories.Generation when _pokemonInfo != null && pokemon.Generation == _pokemonInfo.Generation:
				return green;
			default:
				return red;
		}
	}

	string GetTypeRowClass(PokemonInfo pokemon, string propertyName)
	{
		const string green = "row-card green";
		const string red = "row-card red";
		const string orange = "row-card orange";

		if (_pokemonInfo == null)
			return red;
		
		switch (propertyName)
		{
			case Constants.PokemonCategories.Type1:

				if (Helper.AreTypesEqual(pokemon.Type1, _pokemonInfo.Type1))
					return green;
				
				return Helper.AreTypesEqual(pokemon.Type1, _pokemonInfo.Type2) ? orange : red;

			case Constants.PokemonCategories.Type2:

				if (Helper.AreTypesEqual(pokemon.Type2, _pokemonInfo.Type2))
					return green;
				
				return Helper.AreTypesEqual(pokemon.Type2, _pokemonInfo.Type1) ? orange : red;

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

	void SelectItem(PokemonInfo selectedItem)
	{
		PlayerAnswer = selectedItem.Name;
		SubmitAnswer(selectedItem);
	}

	void SubmitAnswer(PokemonInfo pokemonInfo)
	{
		if (PlayerAnswer != null)
		{
			PlayerAnswer = PlayerAnswer.ToLower().Trim();

			PlayerAnswer = Helper.RemoveDiacritics(PlayerAnswer);

			_guessStarted = true;

			PokemonInfo pkmn = _pokemonDictionary.FirstOrDefault(p => p.Value.Id == pokemonInfo.Id).Value;
			_pokemonList.Add(pkmn);
			_filteredItems.Clear();

			if (_pokemonInfo != null && PlayerAnswer.Equals(_pokemonInfo.Name))
			{
				_gameWon = true;
				_streak++;
				_pkmnCount++;
    
				const int maxHp = Constants.Game.MaxHp;
				const int maxStrike = Constants.Game.MaxStrike;
				int currentHp = _player.GetLife();
    
				if (currentHp < maxHp)
				{
					int hpToAdd = Math.Min(maxHp - currentHp, _streak == maxStrike ? 3 : 1);
					_player.AddLife(hpToAdd);
				}

				if (_streak == maxStrike)
				{
					_level++;
					_streak = 0;
				}
			}
			else
			{
				_player.RemoveLife();

				if (_player.GetLife() == 0)
				{
					_gameOver = true;
					_pokemonList.Clear();
					_guessStarted = false;
				}
			}
		}

		PlayerAnswer = string.Empty;
	}

	void ValidSubmit(KeyboardEventArgs keyboardEventArgs)
	{
		Search();

		if (keyboardEventArgs.Code != "Enter" || string.IsNullOrEmpty(PlayerAnswer)) return;

		if (_filteredItems.Count <= 0)
		{
			PlayerAnswer = string.Empty;
			return;
		}

		PlayerAnswer = _filteredItems.First().Name;
		SubmitAnswer(_filteredItems.First());
	}

	void ResetGame()
	{
		_isLoading = true;
		_player.Reset();
		_gameOver = false;
		_gameWon = false;
		_pokemonList.Clear();
		_drawnPokemons.Clear();
		_guessStarted = false;
		_pkmnCount = 0;
		_streak = 0;
		GetRandomPokemon();
		_isLoading = false;
		InvokeAsync(StateHasChanged);
	}

	void ContinueGame()
	{
		_isLoading = true;
		_gameOver = false;
		_gameWon = false;
		_pokemonList.Clear();
		_guessStarted = false;
		GetRandomPokemon();
		_isLoading = false;
		InvokeAsync(StateHasChanged);
	}
}
