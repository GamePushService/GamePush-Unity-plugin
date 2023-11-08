using UnityEditor;
using UnityEngine;

using GP_Utilities.Console;

[CustomEditor(typeof(GP_ConsoleController))]
public class GP_Editor : Editor
{
    #region Serialized Property
    SerializedProperty AchievementsConsoleLogs;
    SerializedProperty AdsConsoleLogs;
    SerializedProperty AnalyticsConsoleLogs;
    SerializedProperty AppConsoleLogs;
    SerializedProperty AvatarGeneratorConsoleLogs;
    SerializedProperty ChannelConsoleLogs;
    SerializedProperty DeviceConsoleLogs;
    SerializedProperty DocumentsConsoleLogs;
    SerializedProperty FilesConsoleLogs;
    SerializedProperty FullscreenConsoleLogs;
    SerializedProperty GameConsoleLogs;
    SerializedProperty GamesCollectionsConsoleLogs;
    SerializedProperty LanguageConsoleLogs;
    SerializedProperty LeaderboardConsoleLogs;
    SerializedProperty LeaderboardScopedConsoleLogs;
    SerializedProperty PaymentsConsoleLogs;
    SerializedProperty PlatformConsoleLogs;
    SerializedProperty PlayerConsoleLogs;
    SerializedProperty PlayersConsoleLogs;
    SerializedProperty ServerConsoleLogs;
    SerializedProperty SocialsConsoleLogs;
    SerializedProperty SystemConsoleLogs;
    SerializedProperty VariablesConsoleLogs;
    #endregion

    private void OnEnable()
    {
        AchievementsConsoleLogs = serializedObject.FindProperty("AchievementsConsoleLogs");
        AdsConsoleLogs = serializedObject.FindProperty("AdsConsoleLogs");
        AnalyticsConsoleLogs = serializedObject.FindProperty("AnalyticsConsoleLogs");
        AppConsoleLogs = serializedObject.FindProperty("AppConsoleLogs");
        AvatarGeneratorConsoleLogs = serializedObject.FindProperty("AvatarGeneratorConsoleLogs");
        ChannelConsoleLogs = serializedObject.FindProperty("ChannelConsoleLogs");
        DeviceConsoleLogs = serializedObject.FindProperty("DeviceConsoleLogs");
        DocumentsConsoleLogs = serializedObject.FindProperty("DocumentsConsoleLogs");
        FilesConsoleLogs = serializedObject.FindProperty("FilesConsoleLogs");
        FullscreenConsoleLogs = serializedObject.FindProperty("FullscreenConsoleLogs");
        GameConsoleLogs = serializedObject.FindProperty("GameConsoleLogs");
        GamesCollectionsConsoleLogs = serializedObject.FindProperty("GamesCollectionsConsoleLogs");
        LanguageConsoleLogs = serializedObject.FindProperty("LanguageConsoleLogs");
        LeaderboardConsoleLogs = serializedObject.FindProperty("LeaderboardConsoleLogs");
        LeaderboardScopedConsoleLogs = serializedObject.FindProperty("LeaderboardScopedConsoleLogs");
        PaymentsConsoleLogs = serializedObject.FindProperty("PaymentsConsoleLogs");
        PlatformConsoleLogs = serializedObject.FindProperty("PlatformConsoleLogs");
        PlayerConsoleLogs = serializedObject.FindProperty("PlayerConsoleLogs");
        PlayersConsoleLogs = serializedObject.FindProperty("PlayersConsoleLogs");
        ServerConsoleLogs = serializedObject.FindProperty("ServerConsoleLogs");
        SocialsConsoleLogs = serializedObject.FindProperty("SocialsConsoleLogs");
        SystemConsoleLogs = serializedObject.FindProperty("SystemConsoleLogs");
        VariablesConsoleLogs = serializedObject.FindProperty("VariablesConsoleLogs");
    }

    public override void OnInspectorGUI()
    {
        GP_ConsoleController controller = (GP_ConsoleController)target;

        serializedObject.Update();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Enable All"))
            controller.SwitchAll(true);
        if (GUILayout.Button("Disable All"))
            controller.SwitchAll(false);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(AchievementsConsoleLogs);
        EditorGUILayout.PropertyField(AdsConsoleLogs);
        EditorGUILayout.PropertyField(AnalyticsConsoleLogs);
        EditorGUILayout.PropertyField(AppConsoleLogs);
        EditorGUILayout.PropertyField(AvatarGeneratorConsoleLogs);
        EditorGUILayout.PropertyField(ChannelConsoleLogs);
        EditorGUILayout.PropertyField(DeviceConsoleLogs);
        EditorGUILayout.PropertyField(DocumentsConsoleLogs);
        EditorGUILayout.PropertyField(FilesConsoleLogs);
        EditorGUILayout.PropertyField(FullscreenConsoleLogs);
        EditorGUILayout.PropertyField(GameConsoleLogs);
        EditorGUILayout.PropertyField(GamesCollectionsConsoleLogs);
        EditorGUILayout.PropertyField(LanguageConsoleLogs);
        EditorGUILayout.PropertyField(LeaderboardConsoleLogs);
        EditorGUILayout.PropertyField(LeaderboardScopedConsoleLogs);
        EditorGUILayout.PropertyField(PaymentsConsoleLogs);
        EditorGUILayout.PropertyField(PlatformConsoleLogs);
        EditorGUILayout.PropertyField(PlayerConsoleLogs);
        EditorGUILayout.PropertyField(PlayersConsoleLogs);
        EditorGUILayout.PropertyField(ServerConsoleLogs);
        EditorGUILayout.PropertyField(SocialsConsoleLogs);
        EditorGUILayout.PropertyField(SystemConsoleLogs);
        EditorGUILayout.PropertyField(VariablesConsoleLogs);
        EditorGUILayout.Space(10);


        serializedObject.ApplyModifiedProperties();
    }
}

