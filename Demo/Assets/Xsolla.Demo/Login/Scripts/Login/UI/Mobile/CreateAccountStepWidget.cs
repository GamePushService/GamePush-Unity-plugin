using System;
using UnityEngine;
using Xsolla.Demo;

public class CreateAccountStepWidget : MonoBehaviour
{
    [SerializeField] SimpleButton backButton = default;
    [SerializeField] SimpleButton nextButton = default;
    
    public Action onBackButtonClick;
    public Action onNextButtonClick;

    private void Awake()
    {
        backButton.onClick += onBackButtonClick;
        nextButton.onClick += onNextButtonClick;
    }
}
