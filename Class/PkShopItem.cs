namespace Pokefeet.Class;

public class PkShopItem(string imageSrc, string itemName, string description, int price, int id, PkItemType type, int? healingAmount)
{
	public int Id { get; set; } = id;
	public string ImageSrc { get; set; } = imageSrc;
	public string Name { get; set; } = itemName;
	public string Description { get; set;} = description;
	public int Price { get; set; } = price;
	public PkItemType Type { get; set; } = type;
	public int? HealingAmount { get; set; } = healingAmount;

	public static List<PkShopItem> GetShopItems()
	{
		return
		[
			new PkShopItem("/img/shop/skitty.png", "Queue Skitty", "La Queue Skitty permet de fuir pendant un combat contre un Pokémon.", 10, 1, PkItemType.Items, null),
			new PkShopItem("/img/shop/potion.png", "Potion", "Cet objet permet de rendre jusqu'à 1 PV.", 8, 2, PkItemType.Medicine, 1),
			new PkShopItem("/img/shop/superpotion.png", "Super Potion", "Cet objet permet de rendre jusqu'à 2 PV.", 10, 3, PkItemType.Medicine, 2),
			new PkShopItem("/img/shop/hyperpotion.png", "Hyper Potion", "Cet objet permet de rendre jusqu'à 3 PV.", 12, 4, PkItemType.Medicine, 3),
			new PkShopItem("/img/shop/potionmax.png", "Potion Max", "Cet objet permet de rendre la totalité de ses PV.", 20, 5, PkItemType.Medicine, null)
		];
	}
}

public enum PkItemType
{
	Items,
	Medicine
}



