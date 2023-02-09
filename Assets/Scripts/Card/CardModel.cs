
public class CardModel 
{
    public int Id { get; private set; }
    public int Attack { get; private set; }
    public int Hp { get; private set; }
    public int Mana { get; private set; }

    public CardModel(int id, int attack,int hp,int mana)
    {
        Id = id;
        SetAttack(attack);
        SetHp(hp);
        SetMana(mana);
    }

    public void SetAttack(int value)
    {
        Attack = value;
    }
    public void SetHp(int value)
    {
        Hp = value;
    }
    public void SetMana(int value)
    {
        Mana = value;
    }
}
