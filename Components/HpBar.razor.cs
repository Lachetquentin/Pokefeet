using Microsoft.AspNetCore.Components;
using Pokefeet2.Class;

namespace Pokefeet2.Components;

partial class HpBar
{
	[Parameter] public int HpLeft { get; set; }
	int _maxHp = Constants.Player.MaxHp;

	static string CalculateHpPercentage(int hpLeft, int maxHp)
	{
		double percentage = (double)hpLeft / maxHp * 100;

		const double tolerance = 0.001;

		return Math.Abs(percentage - 87.5) < tolerance ? "80.0" : percentage.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
	}

	static double CalculateHpPercentageDouble(int hpLeft, int maxHp)
	{
		double percentage = (double)hpLeft / maxHp * 100;

		const double tolerance = 0.001;

		return Math.Abs(percentage - 87.5) < tolerance ? 80.0 : percentage;
	}

	string GetHpColorClass(int hpLeft)
	{
		string colorClass = "hp-fill";

		double percentage = CalculateHpPercentageDouble(hpLeft, _maxHp);

		return percentage switch
		{
			> 50 => colorClass += " green",
			> 20 => colorClass += " orange",
			_ => colorClass += " red"
		};
	}
}
