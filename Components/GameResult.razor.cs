using Microsoft.AspNetCore.Components;
using static Pokefeet2.Class.PkmnFetch;

namespace Pokefeet2.Components;

partial class GameResult : IDisposable
{
	string _desc = "";

	string _title = "";

	string _countdownTime = "";

	Timer? _countdownTimer;

	[Parameter]
	public bool HasWin {get;set;}

	[Parameter] public PokemonInfo Pkmn { get; set; } = default!;

	[Parameter] public string PkmnName { get; set; } = default!;

	public void Dispose() => _countdownTimer?.Dispose();

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (HasWin)
		{
			_title = "VICTOIRE !";
			_desc = "Tu as deviné:";
		}
		else
		{
			_title = "DÉFAITE !";
			_desc = "Tu n'as pas deviné:";
		}

		StartCountdown();
	}

	string GetColorClassH1() => HasWin ? "victory pkmnH1" : "defeat pkmnH1";

	string GetColorClass() => HasWin ? "victory" : "defeat";

	void StartCountdown()
	{
		var currentTime = DateTime.UtcNow;
		var resetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 22, 0, 0, DateTimeKind.Utc);

		if (currentTime > resetTime)
			resetTime = resetTime.AddDays(1);
	
		_countdownTimer = new Timer(UpdateCountdown, null, 0, 1000);
	}

	void UpdateCountdown(object state)
	{
		var currentTime = DateTime.UtcNow;
		var resetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 22, 0, 0, DateTimeKind.Utc);

		if (currentTime > resetTime)
			resetTime = resetTime.AddDays(1);
	
		var timeRemaining = resetTime - currentTime;

		if (timeRemaining.TotalSeconds <= 0)
		{
			_countdownTime = "00:00:00";
			StartCountdown();
		}
		else
		{
			_countdownTime = timeRemaining.ToString(@"dd\:hh\:mm\:ss");
			InvokeAsync(StateHasChanged);
		}
	}

	static string GetPokemonImg(int pokemonId) => $"/img/sprites/{pokemonId}.png";
}