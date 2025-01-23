using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class DefineSymbolsManager
{
    private static NamedBuildTarget GetBuildTarget()// Получаем текущую группу сборки
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        return buildTarget;
    }
    private static string GetSymbols()// Получаем текущие символы для группы сборки
    {
        // Получаем текущие символы для группы сборки
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(GetBuildTarget());
        return currentSymbols;
    }

    private static void SetSymbols(string symbols)
    {
        PlayerSettings.SetScriptingDefineSymbols(GetBuildTarget(), symbols);
    }
    // Метод для добавления символа компиляции
    public static void AddScriptingDefineSymbol(string symbol)
    {
        // Получаем текущие символы для группы сборки
        string currentSymbols = GetSymbols();

        // Проверяем, если символ уже существует, то выходим
        if (currentSymbols.Contains(symbol))
        {
            //Debug.Log($"'{symbol}' уже существует.");
            return;
        }

        // Добавляем новый символ
        string newSymbols = string.IsNullOrEmpty(currentSymbols) ? symbol : $"{currentSymbols};{symbol}";
        SetSymbols(newSymbols);

    }

    // Метод для удаления символа компиляции
    public static void RemoveScriptingDefineSymbol(string symbol)
    {
        // Получаем текущие символы для группы сборки
        string currentSymbols = GetSymbols();

        // Проверяем, если символ не существует, то выходим
        if (!currentSymbols.Contains(symbol))
        {
            //Debug.Log($"Символ '{symbol}' не найден.");
            return;
        }

        // Удаляем символ и обновляем список
        string newSymbols = currentSymbols.Replace(symbol, "").Replace(";;", ";").Trim(';');
        SetSymbols(newSymbols);
    }
}
