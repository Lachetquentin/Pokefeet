﻿using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Pokefeet.Class;
using static Pokefeet.Class.PokemonData;

namespace Pokefeet.Components;

partial class DailyGame
{
	readonly Player _player = new();
	readonly List<PokemonInfo> _pokemonList = [];
	PokemonDailiesInfo _dailyPokemon;
	List<PokemonInfo> _filteredItems = [];
	bool _gameOver;
	bool _gameWon;
	bool _guessStarted;
	string _hasWinClassic = "";
	bool _isLoading = true;
	IJSObjectReference? _jsRef;
	PokemonInfo _pokemonInfo;
	string _pokemonName;
	List<PokemonInfo> _pokemons = [];
	Dictionary<int, PokemonInfo> _pokemonDictionary = [];
	bool _error;

	string? ImgPath { get; set; }
	string? PlayerAnswer { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_pokemons = GetAllPokemons();
			_pokemonDictionary = _pokemons.ToDictionary(p => p.Id);
			await SetupGame();
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	async Task SetupGame()
	{
		_dailyPokemon = GetDailyPokemon("today");

		if(_dailyPokemon.Name == "_")
		{
			_error = true;
			return;
		}

		if (_pokemonDictionary.Count == 0)
		{
			_error = true;
			return;
		}

		ImgPath = ImageLoader.GetBase64Image(_dailyPokemon.Name, false);

			var pkmnInfo = _pokemonDictionary.FirstOrDefault(p => p.Value.Id.ToString().Equals(_dailyPokemon.Name)).Value;

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
			
			_pokemonName = _pokemonInfo.Name;
			_pokemonInfo.Name = _pokemonInfo.Name.ToLower().Trim();
			_pokemonInfo.Name = Helper.RemoveDiacritics(_pokemonInfo.Name);
			
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
	}

	string GetRowClass(PokemonInfo pokemon, string propertyName)
	{
		const string green = "row-card green";
		const string red = "row-card red";

		switch (propertyName)
		{
			case Constants.PokemonCategories.Type1 when pokemon.Type1 == _pokemonInfo.Type1:
			case Constants.PokemonCategories.Type2 when pokemon.Type2 == _pokemonInfo.Type2:
			case Constants.PokemonCategories.Color when pokemon.Color == _pokemonInfo.Color:
			case Constants.PokemonCategories.IsLegendary when pokemon.IsLegendary == _pokemonInfo.IsLegendary:
			case Constants.PokemonCategories.IsMythical when pokemon.IsMythical == _pokemonInfo.IsMythical:
			case Constants.PokemonCategories.Generation when pokemon.Generation == _pokemonInfo.Generation:
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

		switch (propertyName)
		{
			case Constants.PokemonCategories.Type1:
				if (Helper.AreTypesEqual(pokemon.Type1, _pokemonInfo.Type1))
				{
					return green;
				}
				else if (Helper.AreTypesEqual(pokemon.Type1, _pokemonInfo.Type2))
				{
					return orange;
				}
				else
				{
					return red;
				}
			case Constants.PokemonCategories.Type2:
				if (Helper.AreTypesEqual(pokemon.Type2, _pokemonInfo.Type2))
				{
					return green;
				}
				else if (Helper.AreTypesEqual(pokemon.Type2, _pokemonInfo.Type1))
				{
					return orange;
				}
				else
				{
					return red;
				}

			default:
				return red;
		}
	}

	string GetSearchClass() => _filteredItems.Count > 0 ? "card-search mt-1" : "";

	void GoBack() => Navigation.NavigateTo(Constants.Url.Home);

	void Search()
	{
		_filteredItems = string.IsNullOrEmpty(PlayerAnswer)
			? []
			: _pokemons
				.Where(item =>
					Helper.RemoveDiacritics(item.Name).StartsWith(Helper.RemoveDiacritics(PlayerAnswer), StringComparison.OrdinalIgnoreCase) && _pokemonList.All(p => p.Id != item.Id))
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

			if (PlayerAnswer.Equals(_pokemonInfo.Name))
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
