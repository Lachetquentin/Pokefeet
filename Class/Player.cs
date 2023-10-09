namespace Pokefeet2.Class;

public class Player
{
	int Life { get; set; } = Constants.Player.MaxHp;

	public int GetLife() => Life;
	public void RemoveLife() => Life--;
	public void RemoveLife(int hp) => Life -= hp;
	public void Reset() => Life = Constants.Player.MaxHp;
}
