using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemEffectHandler : MonoBehaviour
{
    private PlayerController controller;
    private PlayerCondition condition;

    public Image speedBoostImage;
    public Image doubleJumpImage;
    public Image infiniteStaminaImage;

    public GameObject speedBoostEffectObject;
    public GameObject doubleJumpEffectObject;
    public GameObject infiniteStaminaEffectObject;

    private Coroutine speedBoostCoroutine;
    private Coroutine doubleJumpCoroutine;
    private Coroutine infiniteStaminaCoroutine;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;

        speedBoostEffectObject.SetActive(false);
        doubleJumpEffectObject.SetActive(false);
        infiniteStaminaEffectObject.SetActive(false);
    }

    public void StartSpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }
        speedBoostCoroutine = StartCoroutine(ApplySpeedBoost(multiplier, duration));
    }

    public void StartDoubleJump(float duration)
    {
        if (doubleJumpCoroutine != null)
        {
            StopCoroutine(doubleJumpCoroutine);
        }
        doubleJumpCoroutine = StartCoroutine(ApplyDoubleJump(duration));
    }

    public void StartInfiniteStamina(float duration)
    {
        if (infiniteStaminaCoroutine != null)
        {
            StopCoroutine(infiniteStaminaCoroutine);
        }
        infiniteStaminaCoroutine = StartCoroutine(ApplyInfiniteStamina(duration));
    }

    private IEnumerator ApplySpeedBoost(float speedMultiplier, float duration)
    {
        Debug.Log("Speed Boost Activated");
        controller.SetSpeedMultiplier(speedMultiplier);

        if (speedBoostEffectObject != null)
            speedBoostEffectObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            speedBoostImage.fillAmount = 1 - (elapsedTime / duration);
            yield return null;
        }

        controller.ResetSpeedMultiplier();
        speedBoostImage.fillAmount = 0;

        if (speedBoostEffectObject != null)
            speedBoostEffectObject.SetActive(false);

        Debug.Log("Speed Boost Deactivated");
        speedBoostCoroutine = null;
    }

    private IEnumerator ApplyDoubleJump(float duration)
    {
        Debug.Log("Double Jump Activated");
        controller.EnableDoubleJump(true);

        if (doubleJumpEffectObject != null)
            doubleJumpEffectObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            doubleJumpImage.fillAmount = 1 - (elapsedTime / duration);
            yield return null;
        }

        controller.EnableDoubleJump(false);
        doubleJumpImage.fillAmount = 0;

        if (doubleJumpEffectObject != null)
            doubleJumpEffectObject.SetActive(false);

        Debug.Log("Double Jump Deactivated");
        doubleJumpCoroutine = null;
    }

    private IEnumerator ApplyInfiniteStamina(float duration)
    {
        Debug.Log("Infinite Stamina Activated");
        condition.EnableInfiniteStamina(true);

        if (infiniteStaminaEffectObject != null)
            infiniteStaminaEffectObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            infiniteStaminaImage.fillAmount = 1 - (elapsedTime / duration);
            yield return null;
        }

        condition.EnableInfiniteStamina(false);
        infiniteStaminaImage.fillAmount = 0;

        if (infiniteStaminaEffectObject != null)
            infiniteStaminaEffectObject.SetActive(false);

        Debug.Log("Infinite Stamina Deactivated");
        infiniteStaminaCoroutine = null;
    }
}
