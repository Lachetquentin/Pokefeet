using Microsoft.AspNetCore.Components;
using Pokefeet.Class;

namespace Pokefeet.Components;

partial class ShopItem
{
	[Parameter] public PkShopItem Item { get; set; }
	[Parameter] public int PokeDollars { get; set; }
	[Parameter] public int HP { get; set; }
	[Parameter] public EventCallback<PkShopItem> OnBuy { get; set; }

	private bool IsButtonDisabled()
	{
		if (Item.Price > PokeDollars) return true;

		if (Item.HealingAmount.HasValue)
		{
			int newHp = HP + Item.HealingAmount.Value;
			if (newHp > Constants.Game.MaxHp) return true;
		}

		return false;
	}

	private async Task BuyItem()
	{
		if (Item.Price <= PokeDollars)
			await OnBuy.InvokeAsync(Item);
	}

	static string GetPkDollarsIcon() => "<image width=\"20\" height=\"20\" xlink:href=\"/img/PokeDollar.png\" />";
}
