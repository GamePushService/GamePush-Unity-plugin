using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using TMPro;

public class TextTranslate : MonoBehaviour
{
    [SerializeField] List<TranslateField> translates;

    TMP_Text textField;

    private void OnEnable()
    {
        GP_Language.OnChangeLanguage += SetLang;
    }

    private void OnDisable()
    {
        GP_Language.OnChangeLanguage -= SetLang;
    }

    private void Awake()
    {
        textField = GetComponent<TMP_Text>();
        SetLang(Language.English);
    }

    private async void Start()
    {
        await GP_Init.Ready;

        Language currentLang = GP_Language.Current();

        SetLang(currentLang);
    }

    private void SetLang(Language lang)
    {
        foreach (TranslateField field in translates)
        {
            if (field.language == lang)
            {
                textField.text = field.text;
            }
        }
    }
}

[System.Serializable]
public class TranslateField
{
    public Language language;
    [TextArea]
    public string text;
}