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
        return "���Ǽ� �ִ� ���Դϴ�.\n������ �ؼ����ݴϴ�.";
    }

    public void OnInteract()
    {
        condition.Drink(50f);
    }
}