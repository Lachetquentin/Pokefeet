using Microsoft.AspNetCore.Components;
using Pokefeet2.Class;
using Pokefeet2.Ressources;
using static Pokefeet2.Class.PkmnFetch;

namespace Pokefeet2.Components;

partial class InfiniteGameResult
{
	[Parameter] public bool HasWin { get; set; }

	[Parameter] public PokemonInfo Pkmn { get; set; } = default!;

	[Parameter] public string PkmnName { get; set; } = default!;

	[Parameter] public int NbGuessed { get; set; }

	[Parameter] public Player Player { get; set; } = default!;

	[Parameter] public Action ReplayGame { get; set; } = default!;

	string _title = "";

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_title = HasWin ? Translation.Victory.ToUpper() : Translation.Defeat.ToUpper();
	}

	string GetColorClassH1() => HasWin ? "victory pkmnH1" : "defeat pkmnH1";
}
