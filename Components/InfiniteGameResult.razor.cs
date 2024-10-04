using Microsoft.AspNetCore.Components;
using Pokefeet.Ressources;
using static Pokefeet.Class.PkmnFetch;

namespace Pokefeet.Components;

partial class InfiniteGameResult
{
	[Parameter] public bool HasWin { get; set; }

	[Parameter] public PokemonInfo Pkmn { get; set; } = default!;

	[Parameter] public string PkmnName { get; set; } = default!;

	[Parameter] public Action ReplayGame { get; set; } = default!;

	[Parameter] public int PkmnCount { get; set; }

	string _title = "";
	string _desc = "";

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (HasWin)
		{
			_title = Translation.Victory.ToUpper();
			_desc = Translation.YouGuessed;
		}
		else
		{
			_title = Translation.Defeat.ToUpper();
			_desc = Translation.NoGuessed;
		}
	}

	string GetColorClassH1() => HasWin ? "victory pkmnH1" : "defeat pkmnH1";

	string GetColorClass() => HasWin ? "victory" : "defeat";
}
