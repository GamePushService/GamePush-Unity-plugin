using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using TMPro;

public class TextTranslate : MonoBehaviour
{
    [SerializeField]
    private List<TranslateField> _translates;
    private TMP_Text _textField;

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
        _textField = GetComponent<TMP_Text>();
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
        foreach (TranslateField field in _translates)
        {
            if (field.language == lang)
            {
                _textField.text = field.text;
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