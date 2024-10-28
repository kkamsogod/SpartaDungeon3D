using UnityEngine;

public class Water : MonoBehaviour, IInteractable
{
    private PlayerCondition condition;

    private void Start()
    {
        condition = CharacterManager.Instance.Player.condition;
    }
    public string GetInteractPrompt()
    {
        return "마실수 있는 물입니다.\n갈증을 해소해줍니다.";
    }

    public void OnInteract()
    {
        condition.Drink(50f);
    }
}