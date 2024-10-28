using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public Image Image;
    public float FlashSpeed;

    private Coroutine Coroutine;
    private bool isDamageFlashActive;

    void Start()
    {
        CharacterManager.Instance.Player.condition.OnTakeDamage += FlashDamage;
        CharacterManager.Instance.Player.condition.OnWarmHeal += FlashWarm;
    }

    public void FlashDamage()
    {
        isDamageFlashActive = true;
        Flash(new Color(0.8f, 0.1f, 0.1f));
    }

    public void FlashWarm()
    {
        if (!isDamageFlashActive)
        {
            Flash(new Color(0.2f, 0.8f, 1f));
        }
    }

    private void Flash(Color color)
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
        }

        Image.enabled = true;
        Image.color = color;

        Coroutine = StartCoroutine(FadeAway(color));
    }

    private IEnumerator FadeAway(Color color)
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0)
        {
            a -= (startAlpha / FlashSpeed) * Time.deltaTime;
            Image.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        Image.enabled = false;
        isDamageFlashActive = false;
    }
}