using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public int Id;
    public string Title;
    public string Description;
    public int Attack;
    public int Hp;
    public int Mana;
    public string ImageUrl = "https://picsum.photos/200/300";
}
