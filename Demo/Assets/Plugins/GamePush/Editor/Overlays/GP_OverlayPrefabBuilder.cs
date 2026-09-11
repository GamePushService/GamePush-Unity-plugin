using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GamePush.Overlays;
using GamePush.Overlays.Views;
using GamePush.Overlays.Widgets;
using static GamePushEditor.Overlays.GP_OverlayUIFactory;

namespace GamePushEditor.Overlays
{
    /// <summary>
    /// Generates the default overlay prefabs. Prefab YAML cannot be written by hand, so the
    /// hierarchy is assembled in code and saved with PrefabUtility. The result is a normal set of
    /// editable assets; rerunning the builder only recreates the defaults.
    /// </summary>
    public static class GP_OverlayPrefabBuilder
    {
        const string PluginFolder = "Assets/Plugins/GamePush";
        const string ResourcesFolder = PluginFolder + "/Resources/GamePush";
        const string OverlaysFolder = ResourcesFolder + "/Overlays";
        const string RowsFolder = OverlaysFolder + "/Rows";
        const string GeneratedFolder = OverlaysFolder + "/Generated";
        const string IconsFolder = GeneratedFolder + "/Icons";
        const string RoundedSpritePath = GeneratedFolder + "/Rounded12.png";
        const string SkinPath = ResourcesFolder + "/GP_OverlaySkin.asset";

        public static void RebuildMenu()
        {
            if (!EnsureTextMeshPro())
                return;
            if (!EditorUtility.DisplayDialog("GamePush",
                    "Rebuild all default overlay prefabs?\n\n" +
                    "Assets inside Resources/GamePush/Overlays will be overwritten. Custom copies are not affected.",
                    "Rebuild", "Cancel"))
                return;
            Rebuild();
            EditorUtility.DisplayDialog("GamePush",
                "Default overlay prefabs rebuilt in\n" + OverlaysFolder, "OK");
        }

        public static GP_OverlaySkin DefaultSkin => LoadOrCreateSkin();

        /// <summary>Rebuilds silently. Callers are responsible for confirming destructive changes.</summary>
        public static void RebuildCli() => Rebuild();

        /// <summary>Rebuilds silently. Callers are responsible for confirming destructive changes.</summary>
        public static void Rebuild(GP_OverlaySkin selectedSkin = null)
        {
            EnsureFolders();
            var skin = selectedSkin != null ? selectedSkin : LoadOrCreateSkin();
            GP_OverlayUIFactory.Skin = skin;
            EnsureDefaultSprites(skin);

            skin.achievementRow = Save(BuildAchievementRow(), RowsFolder + "/AchievementRow.prefab");
            skin.leaderboardRow = Save(BuildLeaderboardRow(), RowsFolder + "/LeaderboardRow.prefab");
            skin.messageRow = Save(BuildMessageRow(), RowsFolder + "/MessageRow.prefab");
            skin.memberRow = Save(BuildMemberRow(), RowsFolder + "/MemberRow.prefab");
            skin.gameCard = Save(BuildGameCard(), RowsFolder + "/GameCard.prefab");
            skin.feedbackRow = Save(BuildFeedbackRow(), RowsFolder + "/FeedbackRow.prefab");

            foreach (GP_OverlayKind kind in System.Enum.GetValues(typeof(GP_OverlayKind)))
            {
                var prefab = Save(BuildScreen(kind), OverlaysFolder + "/" + kind + ".prefab");
                ConfigureScreen(skin, kind, prefab);
            }

            EditorUtility.SetDirty(skin);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RebuildScreen(GP_OverlayKind kind, GP_OverlaySkin selectedSkin = null)
        {
            EnsureFolders();
            var skin = selectedSkin != null ? selectedSkin : LoadOrCreateSkin();
            GP_OverlayUIFactory.Skin = skin;
            EnsureDefaultSprites(skin);
            BuildRowsFor(kind, skin);
            var prefab = Save(BuildScreen(kind), OverlaysFolder + "/" + kind + ".prefab");
            ConfigureScreen(skin, kind, prefab);
            EditorUtility.SetDirty(skin);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static GameObject BuildPreview(GP_OverlayKind kind, GP_OverlaySkin selectedSkin)
        {
            var skin = selectedSkin != null ? selectedSkin : LoadOrCreateSkin();
            GP_OverlayUIFactory.Skin = skin;
            EnsureDefaultSprites(skin);
            return BuildScreen(kind);
        }

        internal static GameObject BuildRowPreview(GP_OverlayKind kind)
        {
            GP_OverlayUIFactory.Skin = LoadOrCreateSkin();
            EnsureDefaultSprites(Skin);
            switch (kind)
            {
                case GP_OverlayKind.Achievements: return BuildAchievementRow();
                case GP_OverlayKind.Chat: return BuildMessageRow();
                case GP_OverlayKind.Feedbacks: return BuildFeedbackRow();
                case GP_OverlayKind.Leaderboard: return BuildLeaderboardRow();
                default: return BuildAchievementRow();
            }
        }

        internal static GameObject BuildMemberRowPreview()
        {
            GP_OverlayUIFactory.Skin = LoadOrCreateSkin();
            EnsureDefaultSprites(Skin);
            return BuildMemberRow();
        }

        static GameObject BuildScreen(GP_OverlayKind kind)
        {
            switch (kind)
            {
                case GP_OverlayKind.Confirm: return BuildConfirm();
                case GP_OverlayKind.Achievements: return BuildAchievements();
                case GP_OverlayKind.Leaderboard: return BuildLeaderboard();
                case GP_OverlayKind.Chat: return BuildChat();
                case GP_OverlayKind.Document: return BuildDocument();
                case GP_OverlayKind.GamesCollections: return BuildGamesCollections();
                case GP_OverlayKind.Feedbacks: return BuildFeedbacks();
                case GP_OverlayKind.AdCountdown: return BuildAdCountdown();
                case GP_OverlayKind.AdFailed: return BuildAdFailed();
                default: throw new System.ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        static void BuildRowsFor(GP_OverlayKind kind, GP_OverlaySkin skin)
        {
            if (kind == GP_OverlayKind.Achievements)
                skin.achievementRow = Save(BuildAchievementRow(), RowsFolder + "/AchievementRow.prefab");
            if (kind == GP_OverlayKind.Leaderboard)
                skin.leaderboardRow = Save(BuildLeaderboardRow(), RowsFolder + "/LeaderboardRow.prefab");
            if (kind == GP_OverlayKind.Chat || kind == GP_OverlayKind.Feedbacks)
                skin.messageRow = Save(BuildMessageRow(), RowsFolder + "/MessageRow.prefab");
            if (kind == GP_OverlayKind.Chat)
                skin.memberRow = Save(BuildMemberRow(), RowsFolder + "/MemberRow.prefab");
            if (kind == GP_OverlayKind.GamesCollections)
                skin.gameCard = Save(BuildGameCard(), RowsFolder + "/GameCard.prefab");
            if (kind == GP_OverlayKind.Feedbacks)
                skin.feedbackRow = Save(BuildFeedbackRow(), RowsFolder + "/FeedbackRow.prefab");
        }

        static void ConfigureScreen(GP_OverlaySkin skin, GP_OverlayKind kind, GameObject prefab)
        {
            switch (kind)
            {
                case GP_OverlayKind.Confirm:
                    SetScreen(skin, kind, prefab, 1.6f, new Vector2(420f, 260f), new Vector2(820f, 620f));
                    break;
                case GP_OverlayKind.Achievements:
                    SetScreen(skin, kind, prefab, 1.55f, new Vector2(360f, 400f), new Vector2(1500f, 1500f));
                    break;
                case GP_OverlayKind.Leaderboard:
                    SetScreen(skin, kind, prefab, 1.45f, new Vector2(360f, 400f), new Vector2(1450f, 1500f));
                    break;
                case GP_OverlayKind.Chat:
                case GP_OverlayKind.Feedbacks:
                    SetScreen(skin, kind, prefab, 1.65f, new Vector2(360f, 400f), new Vector2(1650f, 1500f));
                    break;
                case GP_OverlayKind.Document:
                    SetScreen(skin, kind, prefab, 2f, new Vector2(480f, 320f), new Vector2(2400f, 1800f));
                    break;
                case GP_OverlayKind.GamesCollections:
                    SetScreen(skin, kind, prefab, 1.65f, new Vector2(360f, 400f), new Vector2(1550f, 1500f));
                    break;
                case GP_OverlayKind.AdCountdown:
                    SetScreen(skin, kind, prefab, 1.6f, new Vector2(360f, 220f), new Vector2(720f, 480f));
                    break;
                case GP_OverlayKind.AdFailed:
                    SetScreen(skin, kind, prefab, 1.6f, new Vector2(360f, 220f), new Vector2(760f, 520f));
                    break;
            }
        }

        /// <summary>
        /// TextMeshPro renders nothing without its Essential Resources, so offer the import
        /// instead of producing prefabs full of missing fonts.
        /// </summary>
        public static bool EnsureTextMeshPro()
        {
            if (TMP_Settings.instance != null)
                return true;

            var import = EditorUtility.DisplayDialog("GamePush",
                "TextMeshPro Essential Resources are not imported yet. The overlay prefabs need them.\n\n" +
                "Import now?", "Import", "Cancel");
            if (!import)
                return false;

            EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");
            return TMP_Settings.instance != null;
        }

        public static bool IsTextMeshProReady => TMP_Settings.instance != null;

        #region Assets

        static void EnsureFolders()
        {
            EnsureFolder(PluginFolder + "/Resources");
            EnsureFolder(ResourcesFolder);
            EnsureFolder(OverlaysFolder);
            EnsureFolder(RowsFolder);
            EnsureFolder(GeneratedFolder);
            EnsureFolder(IconsFolder);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;
            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var leaf = Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(leaf))
                return;
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        static GP_OverlaySkin LoadOrCreateSkin()
        {
            var skin = AssetDatabase.LoadAssetAtPath<GP_OverlaySkin>(SkinPath);
            if (skin != null)
                return skin;
            skin = ScriptableObject.CreateInstance<GP_OverlaySkin>();
            AssetDatabase.CreateAsset(skin, SkinPath);
            return skin;
        }

        static void EnsureDefaultSprites(GP_OverlaySkin skin)
        {
            if (skin == null)
                return;

            var rounded = LoadOrCreateRoundedSprite();
            if (rounded != null)
            {
                if (skin.panelSprite == null)
                    skin.panelSprite = rounded;
                if (skin.rowSprite == null)
                    skin.rowSprite = rounded;
                if (skin.buttonSprite == null)
                    skin.buttonSprite = rounded;
            }

            if (skin.closeIcon == null)
                skin.closeIcon = LoadSprite(IconsFolder + "/IconClose.png");
            if (skin.checkIcon == null)
                skin.checkIcon = LoadSprite(IconsFolder + "/IconCheck.png");
            if (skin.lockIcon == null)
                skin.lockIcon = LoadSprite(IconsFolder + "/IconLock.png");
            if (skin.plusIcon == null)
                skin.plusIcon = LoadSprite(IconsFolder + "/IconPlus.png");
            if (skin.backIcon == null)
                skin.backIcon = LoadSprite(IconsFolder + "/IconBack.png");
            if (skin.sendIcon == null)
                skin.sendIcon = LoadSprite(IconsFolder + "/IconSend.png");
            if (skin.membersIcon == null)
                skin.membersIcon = LoadSprite(IconsFolder + "/IconMembers.png");
            if (skin.muteIcon == null)
                skin.muteIcon = LoadSprite(IconsFolder + "/IconMute.png");
            if (skin.kickIcon == null)
                skin.kickIcon = LoadSprite(IconsFolder + "/IconKick.png");
            if (skin.circleSprite == null)
                skin.circleSprite = LoadSprite(IconsFolder + "/Circle.png");
        }

        static Sprite LoadSprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

        static Sprite LoadOrCreateRoundedSprite()
        {
            var sprite = LoadSprite(RoundedSpritePath);
            if (sprite != null)
                return sprite;

            const int size = 48;
            const float radius = 12f;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "GamePush Rounded 12";
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = Mathf.Max(radius - x - 0.5f, 0f, x + 0.5f - (size - radius));
                    var dy = Mathf.Max(radius - y - 0.5f, 0f, y + 0.5f - (size - radius));
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var alpha = Mathf.Clamp01(radius - distance + 0.5f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply();
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? "", RoundedSpritePath),
                texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(RoundedSpritePath, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(RoundedSpritePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(radius, radius, radius, radius);
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
            return LoadSprite(RoundedSpritePath);
        }

        static void SetScreen(GP_OverlaySkin skin, GP_OverlayKind kind, GameObject prefab, float maxAspect,
            Vector2 minSize, Vector2 maxSize)
        {
            var entry = skin.screens.Find(item => item != null && item.kind == kind);
            if (entry == null)
            {
                entry = new GP_OverlayPrefabEntry { kind = kind };
                skin.screens.Add(entry);
            }
            entry.prefab = prefab;
            entry.maxAspect = maxAspect;
            entry.minSize = minSize;
            entry.maxSize = maxSize;
        }

        static GameObject Save(GameObject instance, string path)
        {
            var asset = PrefabUtility.SaveAsPrefabAsset(instance, path);
            Object.DestroyImmediate(instance);
            return asset;
        }

        #endregion

        #region Screen chrome

        struct Shell
        {
            public GameObject root;
            public RectTransform panel;
            public RectTransform header;
            public RectTransform body;
            public TMP_Text title;
            public TMP_Text status;
            public Button close;
            public Image backdrop;
            public Button backdropButton;
        }

        /// <summary>Backdrop + responsive panel + header + status label, shared by every screen.</summary>
        static Shell BuildShell(string name, bool withLayoutMode = true)
        {
            var shell = new Shell();

            var root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup));
            Stretch((RectTransform)root.transform);
            shell.root = root;

            var backdrop = Panel("Backdrop", root.transform, GP_OverlayColorRole.Backdrop);
            shell.backdrop = backdrop;
            shell.backdropButton = backdrop.gameObject.AddComponent<Button>();
            shell.backdropButton.transition = Selectable.Transition.None;
            shell.backdropButton.targetGraphic = backdrop;

            var panel = Panel("Panel", root.transform, GP_OverlayColorRole.Panel, Skin.panelSprite);
            shell.panel = (RectTransform)panel.transform;
            panel.gameObject.AddComponent<GP_OverlayResponsive>();
            if (withLayoutMode)
                panel.gameObject.AddComponent<GP_OverlayLayoutMode>();
            Vertical(panel.gameObject, new RectOffset(0, 0, 0, 0), 0f);

            var header = Panel("Header", panel.transform, GP_OverlayColorRole.Header);
            shell.header = (RectTransform)header.transform;
            Horizontal(header.gameObject,
                new RectOffset((int)Skin.contentPadding, (int)Skin.spacing, (int)Skin.spacingSmall,
                    (int)Skin.spacingSmall), Skin.spacing);
            Element(header.gameObject, minHeight: 72f, preferredHeight: 72f, flexibleHeight: 0f);

            shell.title = Text("Title", header.transform, "", Skin.titleSize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineLeft);
            shell.title.overflowMode = TextOverflowModes.Ellipsis;
            shell.title.textWrappingMode = TextWrappingModes.NoWrap;
            Element(shell.title.gameObject, flexibleWidth: 1f);

            shell.close = IconButton("Close", header.transform, Skin.closeIcon, GP_OverlayColorRole.Button);

            var body = Rect("Body", panel.transform);
            shell.body = body;
            Element(body.gameObject, flexibleHeight: 1f);

            // The status label floats over the body so an empty list still shows a message.
            shell.status = Text("Status", body, "", Skin.bodySize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.Center);
            Stretch((RectTransform)shell.status.transform);
            var ignore = shell.status.gameObject.AddComponent<LayoutElement>();
            ignore.ignoreLayout = true;

            return shell;
        }

        static void Wire(GP_OverlayView view, Shell shell)
        {
            view.panel = shell.panel;
            view.backdrop = shell.backdrop;
            view.backdropButton = shell.backdropButton;
            view.closeButton = shell.close;
            view.titleLabel = shell.title;
            view.statusLabel = shell.status;
            view.canvasGroup = shell.root.GetComponent<CanvasGroup>();
        }

        static void PinStatus(TMP_Text status, Transform host)
        {
            if (status == null || host == null)
                return;
            var rect = (RectTransform)status.transform;
            rect.SetParent(host, false);
            Stretch(rect);
            rect.SetAsLastSibling();
        }

        #endregion

        #region Screens

        static GameObject BuildConfirm()
        {
            var shell = BuildShell("Confirm", false);
            var view = shell.root.AddComponent<GP_ConfirmView>();
            Wire(view, shell);

            Vertical(shell.body.gameObject, new RectOffset(32, 32, 24, 24), 24f);

            view.messageLabel = Text("Message", shell.body, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.Center);
            Element(view.messageLabel.gameObject, flexibleHeight: 1f, minHeight: 96f);

            var buttons = Rect("Buttons", shell.body);
            Horizontal(buttons.gameObject, new RectOffset(0, 0, 0, 0), 16f).childForceExpandWidth = true;
            Element(buttons.gameObject, minHeight: 96f, preferredHeight: 96f);

            view.cancelButton = Button("Cancel", buttons, "", GP_OverlayColorRole.Button, out var cancelLabel);
            view.cancelLabel = cancelLabel;
            view.cancelBackground = view.cancelButton.GetComponent<Image>();
            Element(view.cancelButton.gameObject, flexibleWidth: 1f);

            view.confirmButton = Button("Confirm", buttons, "", GP_OverlayColorRole.Accent, out var confirmLabel);
            view.confirmLabel = confirmLabel;
            view.confirmBackground = view.confirmButton.GetComponent<Image>();
            Element(view.confirmButton.gameObject, flexibleWidth: 1f);

            return shell.root;
        }

        static GameObject BuildAchievements()
        {
            var shell = BuildShell("Achievements");
            var view = shell.root.AddComponent<GP_AchievementsView>();
            Wire(view, shell);
            var mode = shell.panel.GetComponent<GP_OverlayLayoutMode>();
            view.layoutMode = mode;

            view.counterLabel = Text("Counter", shell.header, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            view.counterLabel.richText = true;
            view.counterLabel.overflowMode = TextOverflowModes.Ellipsis;
            view.counterLabel.textWrappingMode = TextWrappingModes.NoWrap;
            view.counterLabel.transform.SetSiblingIndex(1);
            Element(view.counterLabel.gameObject, minWidth: 0f, preferredWidth: 200f, flexibleWidth: 0.4f);

            Vertical(shell.body.gameObject, new RectOffset(0, 0, 0, 0), 0f);

            var compactHost = Rect("CompactGroups", shell.body);
            Element(compactHost.gameObject, minHeight: 80f, preferredHeight: 80f);
            mode.compactOnly.Add(compactHost.gameObject);
            var compactScroll = compactHost.gameObject.AddComponent<ScrollRect>();
            compactScroll.horizontal = true;
            compactScroll.vertical = false;
            compactScroll.movementType = ScrollRect.MovementType.Clamped;
            var compactViewport = Rect("Viewport", compactHost);
            compactViewport.gameObject.AddComponent<RectMask2D>();
            var compactViewportImage = compactViewport.gameObject.AddComponent<Image>();
            compactViewportImage.color = Color.clear;
            var compactRail = Rect("GroupStrip", compactViewport);
            compactRail.anchorMin = new Vector2(0f, 0f);
            compactRail.anchorMax = new Vector2(0f, 1f);
            compactRail.pivot = new Vector2(0f, 0.5f);
            var compactLayout = Horizontal(compactRail.gameObject, new RectOffset(16, 16, 12, 12),
                Skin.spacingSmall);
            compactLayout.childForceExpandHeight = true;
            var compactFitter = compactRail.gameObject.AddComponent<ContentSizeFitter>();
            compactFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            compactScroll.viewport = compactViewport;
            compactScroll.content = compactRail;
            view.compactGroupRail = compactRail;
            var compactTemplate = Chip("GroupButtonTemplate", compactRail, true);
            compactTemplate.gameObject.SetActive(false);
            view.compactGroupButtonTemplate = compactTemplate;

            var columns = Rect("Columns", shell.body);
            Horizontal(columns.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(columns.gameObject, flexibleHeight: 1f);

            var wideHost = Rect("WideGroups", columns);
            Background(wideHost, GP_OverlayColorRole.Sidebar);
            Element(wideHost.gameObject, minWidth: 240f, preferredWidth: 280f);
            mode.wideOnly.Add(wideHost.gameObject);

            var rail = Rect("GroupRail", wideHost);
            Vertical(rail.gameObject, new RectOffset(16, 16, 16, 16), Skin.spacingSmall);
            var railFitter = rail.gameObject.AddComponent<ContentSizeFitter>();
            railFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            view.groupRail = rail;

            var template = Chip("GroupButtonTemplate", rail, true, true);
            template.gameObject.SetActive(false);
            view.groupButtonTemplate = template;

            var list = ScrollList("List", columns, true, 12f);
            view.list = list;
            PinStatus(shell.status, list.transform);
            var achievementGrid = list.content.GetComponent<GP_FlexibleGrid>();
            view.grid = achievementGrid;
            achievementGrid.minCellWidth = 360f;
            achievementGrid.compactColumns = 1;
            achievementGrid.wideColumns = 2;
            achievementGrid.cellHeight = 120f;
            achievementGrid.cellRatio = 0.28f;
            achievementGrid.compactCellRatio = 0.28f;
            achievementGrid.wideCellRatio = 0.28f;

            return shell.root;
        }

        static GameObject BuildLeaderboard()
        {
            var shell = BuildShell("Leaderboard");
            var view = shell.root.AddComponent<GP_LeaderboardView>();
            Wire(view, shell);
            view.layoutMode = shell.panel.GetComponent<GP_OverlayLayoutMode>();

            view.subtitleLabel = Text("Subtitle", shell.header, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            view.subtitleLabel.transform.SetSiblingIndex(1);
            Element(view.subtitleLabel.gameObject, minWidth: 160f, preferredWidth: 220f);

            Vertical(shell.body.gameObject, new RectOffset(0, 0, 0, 0), 0f);

            var header = LeaderboardRowHierarchy(shell.body);
            header.gameObject.name = "Header";
            Element(header.gameObject, minHeight: 48f, preferredHeight: 48f, flexibleHeight: 0f);
            view.headerRow = header;

            view.list = ScrollList("List", shell.body, false);

            var holder = Rect("SelfRow", shell.body);
            Element(holder.gameObject, minHeight: 96f, preferredHeight: 96f, flexibleHeight: 0f);
            view.selfRowHolder = holder.gameObject;
            var self = LeaderboardRowHierarchy(holder);
            Stretch((RectTransform)self.transform);
            view.selfRow = self;
            holder.gameObject.SetActive(false);

            return shell.root;
        }

        static GameObject BuildChat()
        {
            var shell = BuildShell("Chat");
            var view = shell.root.AddComponent<GP_ChatView>();
            Wire(view, shell);
            var mode = shell.panel.GetComponent<GP_OverlayLayoutMode>();
            view.layoutMode = mode;

            Vertical(shell.body.gameObject, new RectOffset(0, 0, 0, 0), 0f);

            var compactTabs = Rect("CompactTabs", shell.body);
            Background(compactTabs, GP_OverlayColorRole.Sidebar);
            Horizontal(compactTabs.gameObject, new RectOffset(16, 16, 10, 10), Skin.spacingSmall)
                .childForceExpandWidth = true;
            Element(compactTabs.gameObject, minHeight: 76f, preferredHeight: 76f, flexibleHeight: 0f);
            view.compactTabs = compactTabs.gameObject;
            view.messagesTab = Chip("MessagesTab", compactTabs, false, true);
            view.membersTab = Chip("MembersTab", compactTabs, false, true);

            var columns = Rect("Columns", shell.body);
            Horizontal(columns.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(columns.gameObject, flexibleHeight: 1f);

            var messages = Rect("MessagesPanel", columns);
            Vertical(messages.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(messages.gameObject, flexibleWidth: 2f, flexibleHeight: 1f);
            view.messagesPanel = messages.gameObject;
            view.messageList = ScrollList("Messages", messages, false, Skin.spacingSmall);
            PinStatus(shell.status, messages);

            var members = Rect("Members", columns);
            Background(members, GP_OverlayColorRole.Sidebar);
            Vertical(members.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(members.gameObject, minWidth: 260f, preferredWidth: 320f, flexibleWidth: 1f);
            view.membersPanel = members.gameObject;
            view.membersTitle = Text("MembersTitle", members, GP_OverlayStrings.Members, Skin.bodySize,
                GP_OverlayColorRole.Text, TextAlignmentOptions.MidlineLeft);
            Element(view.membersTitle.gameObject, minHeight: 56f, preferredHeight: 56f);
            view.memberList = ScrollList("MemberList", members, false);

            var composer = Rect("Composer", shell.body);
            Background(composer, GP_OverlayColorRole.Sidebar);
            Horizontal(composer.gameObject, new RectOffset(16, 16, 12, 12), Skin.spacing);
            Element(composer.gameObject, minHeight: 88f, preferredHeight: 88f, flexibleHeight: 0f);
            view.composer = composer;

            view.input = InputField("Input", composer);
            Element(view.input.gameObject, flexibleWidth: 1f);

            view.sendButton = Button("Send", composer, GP_OverlayStrings.Send, GP_OverlayColorRole.Accent, out _);
            Element(view.sendButton.gameObject, minWidth: 140f, preferredWidth: 180f,
                minHeight: Skin.controlHeight, preferredHeight: Skin.controlHeight);
            view.compactTabs.SetActive(false);

            return shell.root;
        }

        static GameObject BuildDocument()
        {
            var shell = BuildShell("Document", false);
            var view = shell.root.AddComponent<GP_DocumentView>();
            Wire(view, shell);

            Horizontal(shell.body.gameObject, new RectOffset(0, 0, 0, 0), 0f).childAlignment =
                TextAnchor.UpperCenter;

            var scrollRect = Rect("Scroll", shell.body);
            var scroll = scrollRect.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            Element(scrollRect.gameObject, flexibleWidth: 1f, flexibleHeight: 1f);

            var viewport = Rect("Viewport", scrollRect);
            viewport.gameObject.AddComponent<RectMask2D>();
            var viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0f);

            var content = Rect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            Horizontal(content.gameObject, new RectOffset(32, 32, 24, 24), 0f).childAlignment =
                TextAnchor.UpperCenter;
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var body = Text("Text", content, "", 42f, GP_OverlayColorRole.Text, TextAlignmentOptions.TopLeft);
            body.textWrappingMode = TextWrappingModes.Normal;
            body.overflowMode = TextOverflowModes.Overflow;
            view.contentLabel = body;
            view.contentLayout = body.gameObject.AddComponent<LayoutElement>();
            view.maxTextWidth = 0f;
            view.contentLayout.preferredWidth = -1f;
            view.contentLayout.flexibleWidth = 1f;

            scroll.viewport = viewport;
            scroll.content = content;
            view.scrollRect = scroll;

            return shell.root;
        }

        static GameObject BuildGamesCollections()
        {
            var shell = BuildShell("GamesCollections");
            var view = shell.root.AddComponent<GP_GamesCollectionsView>();
            Wire(view, shell);

            Vertical(shell.body.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            var list = ScrollList("List", shell.body, true, 16f);
            view.list = list;
            view.grid = list.content.GetComponent<GP_FlexibleGrid>();
            view.grid.cellRatio = 1.25f;
            view.grid.minCellWidth = 280f;

            return shell.root;
        }

        static GameObject BuildFeedbacks()
        {
            var shell = BuildShell("Feedbacks");
            var view = shell.root.AddComponent<GP_FeedbacksView>();
            Wire(view, shell);
            var mode = shell.panel.GetComponent<GP_OverlayLayoutMode>();
            view.layoutMode = mode;

            view.backButton = IconButton("Back", shell.header, Skin.backIcon, GP_OverlayColorRole.Button);
            view.backButton.transform.SetSiblingIndex(0);
            view.newButton = IconButton("New", shell.header, Skin.plusIcon, GP_OverlayColorRole.Accent);
            view.newButton.transform.SetSiblingIndex(2);

            var columns = Rect("Columns", shell.body);
            Horizontal(columns.gameObject, new RectOffset(0, 0, 0, 0), 0f);

            var listPanel = Rect("ListPanel", columns);
            Vertical(listPanel.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(listPanel.gameObject, minWidth: 340f, preferredWidth: 420f, flexibleWidth: 1f);
            view.listPanel = listPanel.gameObject;
            view.feedbackList = ScrollList("List", listPanel, false);
            PinStatus(shell.status, listPanel);

            var threadPanel = Rect("ThreadPanel", columns);
            Background(threadPanel, GP_OverlayColorRole.Sidebar);
            Vertical(threadPanel.gameObject, new RectOffset(0, 0, 0, 0), 0f);
            Element(threadPanel.gameObject, flexibleWidth: 2f);
            view.threadPanel = threadPanel.gameObject;

            view.threadTitle = Text("ThreadTitle", threadPanel, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineLeft);
            Element(view.threadTitle.gameObject, minHeight: 72f, preferredHeight: 72f);

            view.threadList = ScrollList("Thread", threadPanel, false, 10f);

            var composer = Rect("Composer", threadPanel);
            Horizontal(composer.gameObject, new RectOffset(16, 16, 12, 12), 12f);
            Element(composer.gameObject, minHeight: 88f, preferredHeight: 88f, flexibleHeight: 0f);
            view.composer = composer;

            view.input = InputField("Input", composer);
            Element(view.input.gameObject, flexibleWidth: 1f);

            view.sendButton = Button("Send", composer, GP_OverlayStrings.Send, GP_OverlayColorRole.Accent, out _);
            Element(view.sendButton.gameObject, minWidth: 140f, preferredWidth: 180f,
                minHeight: Skin.controlHeight, preferredHeight: Skin.controlHeight);

            return shell.root;
        }

        static GameObject BuildAdCountdown()
        {
            var shell = BuildShell("AdCountdown", false);
            var view = shell.root.AddComponent<GP_AdCountdownView>();
            Wire(view, shell);
            shell.close.gameObject.SetActive(false);
            shell.header.gameObject.SetActive(false);

            Vertical(shell.body.gameObject, new RectOffset(32, 32, 32, 32), 16f).childAlignment =
                TextAnchor.MiddleCenter;

            view.captionLabel = Text("Caption", shell.body, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.Center);
            Element(view.captionLabel.gameObject, minHeight: 56f);

            view.countdownLabel = Text("Countdown", shell.body, "", Skin.titleSize * 2f, GP_OverlayColorRole.Accent,
                TextAlignmentOptions.Center);
            Element(view.countdownLabel.gameObject, minHeight: 140f, flexibleHeight: 1f);

            view.skipButton = Button("Skip", shell.body, "", GP_OverlayColorRole.Button, out _);
            Element(view.skipButton.gameObject, minHeight: 88f, preferredHeight: 88f);

            return shell.root;
        }

        static GameObject BuildAdFailed()
        {
            var shell = BuildShell("AdFailed", false);
            var view = shell.root.AddComponent<GP_AdFailedView>();
            Wire(view, shell);
            shell.header.gameObject.SetActive(false);

            Vertical(shell.body.gameObject, new RectOffset(32, 32, 32, 32), 24f).childAlignment =
                TextAnchor.MiddleCenter;

            view.textLabel = Text("Text", shell.body, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.Center);
            Element(view.textLabel.gameObject, flexibleHeight: 1f, minHeight: 96f);

            view.okButton = Button("Ok", shell.body, "", GP_OverlayColorRole.Accent, out _);
            Element(view.okButton.gameObject, minHeight: 96f, preferredHeight: 96f);

            return shell.root;
        }

        #endregion

        #region Rows

        static GameObject BuildAchievementRow()
        {
            var root = new GameObject("AchievementRow", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            Size(rect, new Vector2(520f, 200f));

            var row = root.AddComponent<GP_AchievementRow>();
            row.background = root.AddComponent<Image>();
            row.background.color = Skin.row;
            GP_OverlayTone.Bind(row.background.gameObject, GP_OverlayColorRole.Row);
            row.background.sprite = Skin.rowSprite;
            row.background.type = Skin.rowSprite != null ? Image.Type.Sliced : Image.Type.Simple;

            Horizontal(root, new RectOffset(16, 16, 16, 16), 16f, false);

            row.icon = Avatar("Icon", rect, 88f);
            GP_LayoutSquare.Lock(row.icon, 88f);

            var text = Rect("Text", rect);
            Vertical(text.gameObject, new RectOffset(0, 0, 0, 0), 6f).childAlignment = TextAnchor.MiddleLeft;
            Element(text.gameObject, flexibleWidth: 1f);

            row.titleLabel = Text("Title", text, "", Skin.bodySize, GP_OverlayColorRole.Text);
            row.titleLabel.overflowMode = TextOverflowModes.Ellipsis;
            row.titleLabel.textWrappingMode = TextWrappingModes.NoWrap;
            row.descriptionLabel = Text("Description", text, "", Skin.captionSize, GP_OverlayColorRole.TextMuted);
            row.descriptionLabel.overflowMode = TextOverflowModes.Ellipsis;
            row.descriptionLabel.textWrappingMode = TextWrappingModes.Normal;

            var progressRow = Rect("ProgressRow", text);
            Horizontal(progressRow.gameObject, new RectOffset(0, 0, 0, 0), 8f, false).childAlignment =
                TextAnchor.MiddleLeft;
            Element(progressRow.gameObject, minHeight: 24f, preferredHeight: 24f, flexibleHeight: 0f);

            var bar = Panel("ProgressBar", progressRow, GP_OverlayColorRole.Input, Skin.buttonSprite);
            bar.raycastTarget = false;
            Element(bar.gameObject, flexibleWidth: 1f, minHeight: 8f, preferredHeight: 8f, flexibleHeight: 0f);
            var fill = Panel("Fill", bar.transform, GP_OverlayColorRole.Accent, Skin.buttonSprite);
            fill.raycastTarget = false;
            fill.type = Image.Type.Sliced;
            var fillRect = (RectTransform)fill.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.35f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillRect.pivot = new Vector2(0f, 0.5f);
            row.progressFill = fill;

            row.progressLabel = Text("Progress", progressRow, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            Element(row.progressLabel.gameObject, minWidth: 72f, preferredWidth: 88f);
            row.progressGroup = progressRow.gameObject;

            row.unlockedBadge = StatusChip("Unlocked", rect, Skin.checkIcon, Skin.accent,
                GP_OverlayStrings.Unlocked, Skin.accent);
            row.lockedBadge = StatusChip("Locked", rect, Skin.lockIcon, Skin.textMuted,
                GP_OverlayStrings.Locked, Skin.textMuted);

            return root;
        }

        static GameObject BuildLeaderboardRow()
        {
            var holder = new GameObject("LeaderboardRow", typeof(RectTransform));
            var row = LeaderboardRowHierarchy((RectTransform)holder.transform);
            // The standalone prefab is just the row itself.
            var result = row.gameObject;
            result.transform.SetParent(null, false);
            Object.DestroyImmediate(holder);
            Size((RectTransform)result.transform, new Vector2(900f, 108f));
            return result;
        }

        static GP_LeaderboardRow LeaderboardRowHierarchy(RectTransform parent)
        {
            var rect = Rect("Row", parent);
            Size(rect, new Vector2(900f, 108f));
            var row = rect.gameObject.AddComponent<GP_LeaderboardRow>();
            row.background = rect.gameObject.AddComponent<Image>();
            row.background.color = Skin.row;
            GP_OverlayTone.Bind(row.background.gameObject, GP_OverlayColorRole.Row);
            row.background.sprite = Skin.rowSprite;
            row.background.type = Skin.rowSprite != null ? Image.Type.Sliced : Image.Type.Simple;

            Horizontal(rect.gameObject, new RectOffset(16, 16, 8, 8), 16f, false);

            row.positionLabel = Text("Position", rect, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.Center);
            Element(row.positionLabel.gameObject, minWidth: 80f, preferredWidth: 80f);

            row.avatar = Avatar("Avatar", rect, 72f);

            row.nameLabel = Text("Name", rect, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineLeft);
            row.nameLabel.textWrappingMode = TextWrappingModes.NoWrap;
            row.nameLabel.overflowMode = TextOverflowModes.Ellipsis;
            Element(row.nameLabel.gameObject, flexibleWidth: 1f, minWidth: 64f, preferredWidth: 80f);

            var extras = Rect("Extras", rect);
            Horizontal(extras.gameObject, new RectOffset(0, 0, 0, 0), 12f, false);
            Element(extras.gameObject, flexibleWidth: 0f);
            row.extraColumns = extras;

            var template = Text("ExtraTemplate", extras, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            template.textWrappingMode = TextWrappingModes.NoWrap;
            template.overflowMode = TextOverflowModes.Ellipsis;
            Element(template.gameObject, minWidth: GP_LeaderboardRow.ColumnWidth,
                preferredWidth: GP_LeaderboardRow.ColumnWidth, flexibleWidth: 0f);
            template.gameObject.SetActive(false);
            row.extraColumnTemplate = template;

            row.scoreLabel = Text("Score", rect, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineRight);
            row.scoreLabel.textWrappingMode = TextWrappingModes.NoWrap;
            row.scoreLabel.overflowMode = TextOverflowModes.Ellipsis;
            Element(row.scoreLabel.gameObject, minWidth: GP_LeaderboardRow.ColumnWidth,
                preferredWidth: GP_LeaderboardRow.ColumnWidth, flexibleWidth: 0f);

            return row;
        }

        static GameObject BuildMessageRow()
        {
            var root = new GameObject("MessageRow", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            Size(rect, new Vector2(900f, 140f));

            var row = root.AddComponent<GP_MessageRow>();
            row.layout = Horizontal(root, new RectOffset(12, 12, 8, 8), 12f, false);
            row.layout.childAlignment = TextAnchor.UpperLeft;

            var fitter = root.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            row.avatar = Avatar("Avatar", rect, 72f);

            var bubbleImage = Panel("Bubble", rect, GP_OverlayColorRole.Row, Skin.rowSprite);
            row.bubble = bubbleImage;
            Vertical(bubbleImage.gameObject, new RectOffset(16, 16, 12, 12), 4f);
            Element(bubbleImage.gameObject, flexibleWidth: 1f);
            var bubbleFitter = bubbleImage.gameObject.AddComponent<ContentSizeFitter>();
            bubbleFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var head = Rect("Head", bubbleImage.transform);
            Horizontal(head.gameObject, new RectOffset(0, 0, 0, 0), 8f);
            Element(head.gameObject, minHeight: 32f, preferredHeight: 32f);

            row.authorLabel = Text("Author", head, "", Skin.captionSize, GP_OverlayColorRole.TextMuted);
            Element(row.authorLabel.gameObject, flexibleWidth: 1f);
            row.timeLabel = Text("Time", head, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            Element(row.timeLabel.gameObject, minWidth: 100f, preferredWidth: 110f);

            row.textLabel = Text("Text", bubbleImage.transform, "", Skin.bodySize, GP_OverlayColorRole.Text);

            row.deleteButton = IconButton("Delete", rect, Skin.closeIcon, GP_OverlayColorRole.Button,
                Skin.compactControlHeight);
            row.deleteButton.gameObject.SetActive(false);

            return root;
        }

        static GameObject BuildMemberRow()
        {
            var root = new GameObject("MemberRow", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            Size(rect, new Vector2(400f, 96f));

            var row = root.AddComponent<GP_MemberRow>();
            row.background = root.AddComponent<Image>();
            row.background.color = Skin.row;
            GP_OverlayTone.Bind(row.background.gameObject, GP_OverlayColorRole.Row);
            row.background.sprite = Skin.rowSprite;
            row.background.type = Skin.rowSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            Horizontal(root, new RectOffset(12, 12, 8, 8), 12f, false);

            row.onlineDot = Panel("Online", rect, GP_OverlayColorRole.TextMuted, Skin.circleSprite);
            row.onlineDot.type = Image.Type.Simple;
            row.onlineDot.preserveAspect = true;
            Size((RectTransform)row.onlineDot.transform, new Vector2(14f, 14f));
            Element(row.onlineDot.gameObject, minWidth: 14f, preferredWidth: 14f, minHeight: 14f,
                preferredHeight: 14f, flexibleWidth: 0f, flexibleHeight: 0f);
            GP_LayoutSquare.Lock(row.onlineDot, 14f);

            row.avatar = Avatar("Avatar", rect, 56f);

            row.nameLabel = Text("Name", rect, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineLeft);
            Element(row.nameLabel.gameObject, flexibleWidth: 1f);

            row.stateLabel = Text("State", rect, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineLeft);
            Element(row.stateLabel.gameObject, minWidth: 48f, preferredWidth: 72f);

            row.muteButton = IconButton("Mute", rect, Skin.muteIcon, GP_OverlayColorRole.Button,
                Skin.compactControlHeight);
            row.kickButton = IconButton("Kick", rect, Skin.kickIcon, GP_OverlayColorRole.Danger,
                Skin.compactControlHeight);

            return root;
        }

        static GameObject BuildGameCard()
        {
            var root = new GameObject("GameCard", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            Size(rect, new Vector2(320f, 400f));

            var card = root.AddComponent<GP_GameCard>();
            card.background = root.AddComponent<Image>();
            card.background.color = Skin.row;
            GP_OverlayTone.Bind(card.background.gameObject, GP_OverlayColorRole.Row);
            card.background.sprite = Skin.rowSprite;
            card.background.type = Skin.rowSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            card.button = root.AddComponent<Button>();
            card.button.targetGraphic = card.background;

            Vertical(root, new RectOffset(12, 12, 12, 12), 8f).childAlignment = TextAnchor.UpperCenter;

            card.icon = Avatar("Icon", rect, 200f);
            Element(card.icon.gameObject, flexibleHeight: 1f, minHeight: 160f);

            card.nameLabel = Text("Name", rect, "", Skin.captionSize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.Center);
            Element(card.nameLabel.gameObject, minHeight: 56f);

            card.playLabel = Text("Play", rect, "", Skin.captionSize, GP_OverlayColorRole.Accent,
                TextAlignmentOptions.Center);
            Element(card.playLabel.gameObject, minHeight: 44f);

            return root;
        }

        static GameObject BuildFeedbackRow()
        {
            var root = new GameObject("FeedbackRow", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            Size(rect, new Vector2(420f, 140f));

            var row = root.AddComponent<GP_FeedbackRow>();
            row.background = root.AddComponent<Image>();
            row.background.color = Skin.row;
            GP_OverlayTone.Bind(row.background.gameObject, GP_OverlayColorRole.Row);
            row.background.sprite = Skin.rowSprite;
            row.background.type = Skin.rowSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            row.button = root.AddComponent<Button>();
            row.button.targetGraphic = row.background;

            var selected = Panel("SelectedBar", rect, GP_OverlayColorRole.Accent, Skin.buttonSprite);
            selected.raycastTarget = false;
            var selectedRect = (RectTransform)selected.transform;
            selectedRect.anchorMin = new Vector2(0f, 0f);
            selectedRect.anchorMax = new Vector2(0f, 1f);
            selectedRect.pivot = new Vector2(0f, 0.5f);
            selectedRect.sizeDelta = new Vector2(6f, -16f);
            selectedRect.anchoredPosition = new Vector2(8f, 0f);
            var ignoreBar = selected.gameObject.AddComponent<LayoutElement>();
            ignoreBar.ignoreLayout = true;
            selected.gameObject.SetActive(false);
            row.selectedBar = selected;

            Vertical(root, new RectOffset(20, 16, 12, 12), 6f);

            row.textLabel = Text("Text", rect, "", Skin.bodySize, GP_OverlayColorRole.Text);
            Element(row.textLabel.gameObject, flexibleHeight: 1f, minHeight: 56f);

            var footer = Rect("Footer", rect);
            Horizontal(footer.gameObject, new RectOffset(0, 0, 0, 0), 8f);
            Element(footer.gameObject, minHeight: 36f, preferredHeight: 36f);

            row.statusLabel = Text("Status", footer, "", Skin.captionSize, GP_OverlayColorRole.TextMuted);
            Element(row.statusLabel.gameObject, flexibleWidth: 1f);
            row.dateLabel = Text("Date", footer, "", Skin.captionSize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineRight);
            Element(row.dateLabel.gameObject, minWidth: 160f, preferredWidth: 170f);

            return root;
        }

        #endregion

        static TMP_InputField InputField(string name, Transform parent)
        {
            var background = Panel(name, parent, GP_OverlayColorRole.Input, Skin.rowSprite);
            var field = background.gameObject.AddComponent<TMP_InputField>();
            field.targetGraphic = background;
            StyleSelectable(field, Skin.input);
            field.lineType = TMP_InputField.LineType.SingleLine;

            var viewport = Rect("TextArea", background.transform);
            viewport.offsetMin = new Vector2(16f, 8f);
            viewport.offsetMax = new Vector2(-16f, -8f);
            viewport.gameObject.AddComponent<RectMask2D>();

            var placeholder = Text("Placeholder", viewport, "", Skin.bodySize, GP_OverlayColorRole.TextMuted,
                TextAlignmentOptions.MidlineLeft);
            var text = Text("Text", viewport, "", Skin.bodySize, GP_OverlayColorRole.Text,
                TextAlignmentOptions.MidlineLeft);

            field.textViewport = viewport;
            field.textComponent = text;
            field.placeholder = placeholder;
            field.fontAsset = Skin.font;
            field.pointSize = Skin.bodySize;
            return field;
        }
    }
}
