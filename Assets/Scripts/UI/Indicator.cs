using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public Image Image;
    public float FlashSpeed;

    private Coroutine Coroutine;

    void Start()
    {
        CharacterManager.Instance.Player.condition.OnTakeDamage += FlashDamage;
        CharacterManager.Instance.Player.condition.OnWarmHeal += FlashWarm;
    }

    public void FlashDamage()
    {
        Flash(new Color(1f, 0.5f, 0.2f));
    }

    public void FlashWarm()
    {
        Flash(new Color(0.2f, 0.8f, 1f));
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
    }
}
