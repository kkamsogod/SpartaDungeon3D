using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition thirst;
    public Condition cold;
    public Condition stamina;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.UICondition = this;
    }
}