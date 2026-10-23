using TMPro;
using UnityEngine;
using System.Collections;
public class TextFlicker : MonoBehaviour
{
    public TMP_Text text;

    [Header("Настройки мигания")]
    public float minOnTime = 0.5f;
    public float maxOnTime = 3f;
    public float minOffTime = 0.03f;
    public float maxOffTime = 0.2f;

    [Header("Дополнительные сбои")]
    [Range(0f, 1f)]
    public float flickerChance = 0.3f;

    void Start()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            // Текст горит
            text.enabled = true;

            yield return new WaitForSeconds(
                Random.Range(minOnTime, maxOnTime)
            );

            // Иногда происходит серия сбоев
            if (Random.value < flickerChance)
            {
                int flashes = Random.Range(2, 6);

                for (int i = 0; i < flashes; i++)
                {
                    text.enabled = false;
                    yield return new WaitForSeconds(
                        Random.Range(minOffTime, maxOffTime)
                    );

                    text.enabled = true;
                    yield return new WaitForSeconds(
                        Random.Range(0.02f, 0.12f)
                    );
                }
            }

            // Обычное отключение лампы
            text.enabled = false;

            yield return new WaitForSeconds(
                Random.Range(minOffTime, maxOffTime)
            );
        }
    }
}
