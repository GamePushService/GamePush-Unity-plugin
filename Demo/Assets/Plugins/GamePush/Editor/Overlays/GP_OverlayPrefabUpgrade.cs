using UnityEditor;
using UnityEngine;
using GamePush.Overlays.Widgets;

namespace GamePushEditor.Overlays
{
    [InitializeOnLoad]
    static class GP_OverlayPrefabUpgrade
    {
        static GP_OverlayPrefabUpgrade()
        {
            EditorApplication.delayCall += UpgradeIfStale;
        }

        static void UpgradeIfStale()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;
            if (EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += UpgradeIfStale;
                return;
            }
            if (!GP_OverlayPrefabBuilder.IsTextMeshProReady)
                return;

            var skin = GP_OverlayPrefabBuilder.DefaultSkin;
            var row = skin != null ? skin.achievementRow : null;
            var achievement = row != null ? row.GetComponent<GP_AchievementRow>() : null;
            if (achievement != null && achievement.progressGroup != null)
                return;

            GP_OverlayPrefabBuilder.Rebuild(skin);
        }
    }
}
