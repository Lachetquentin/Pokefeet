using Microsoft.AspNetCore.Components;
using Pokefeet.Class;

namespace Pokefeet.Components;

partial class HpBar
{
	[Parameter] public int HpLeft { get; set; }
	[Parameter] public int Streak { get; set; }
	[Parameter] public int Level { get; set; } = 1;
	
	int _maxHp = Constants.Game.MaxHp;

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

	int CalculateXpPercentage(int currentXp)
	{
		const double segmentPercentage = 100.0 / Constants.Game.MaxStrike;
		return (int)(currentXp * segmentPercentage);
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

	string SetLevelString() => $"Lv{Level}";
}
