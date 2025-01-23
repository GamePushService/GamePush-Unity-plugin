using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class DefineSymbolsManager
{
    // Метод для добавления символа компиляции
    public static void AddScriptingDefineSymbol(string symbol)
    {
        // Получаем текущую группу сборки
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        
        // Получаем текущие символы для группы сборки
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

        // Проверяем, если символ уже существует, то выходим
        if (currentSymbols.Contains(symbol))
        {
            //Debug.Log($"'{symbol}' уже существует.");
            return;
        }

        // Добавляем новый символ
        string newSymbols = string.IsNullOrEmpty(currentSymbols) ? symbol : $"{currentSymbols};{symbol}";
        PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);

        //Debug.Log($"Символ '{symbol}' успешно добавлен.");
    }

    // Метод для удаления символа компиляции
    public static void RemoveScriptingDefineSymbol(string symbol)
    {
        // Получаем текущую группу сборки
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        // Получаем текущие символы для группы сборки
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        // Проверяем, если символ не существует, то выходим
        if (!currentSymbols.Contains(symbol))
        {
            //Debug.Log($"Символ '{symbol}' не найден.");
            return;
        }

        // Удаляем символ и обновляем список
        string newSymbols = currentSymbols.Replace(symbol, "").Replace(";;", ";").Trim(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newSymbols);

        //Debug.Log($"Символ '{symbol}' успешно удален.");
    }
}
