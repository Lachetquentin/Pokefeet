namespace Pokefeet.Class;

public class Player
{
	int Life { get; set; } = Constants.Game.MaxHp;

	public void AddLife() => Life++;
	public void AddLife(int hp) => Life += hp;
	public int GetLife() => Life;
	public void RemoveLife() => Life--;
	public void RemoveLife(int hp) => Life -= hp;
	public void Reset() => Life = Constants.Game.MaxHp;
	public void SetLife(int hp) => Life = hp;
}
