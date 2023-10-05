namespace Pokefeet2.Class;

public class Player
{
	int Life { get; set; } = 8;
	int Points { get; set; }

	public void AddPts() => Points++;
	public int GetLife() => Life;
	public int GetPts() => Points;
	public void RemoveLife() => Life--;
	public void RemoveLife(int hp) => Life -= hp;

	public void Reset()
	{
		Points = 0;
		Life = 8;
	}
}
