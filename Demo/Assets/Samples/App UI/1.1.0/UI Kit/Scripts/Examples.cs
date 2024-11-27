using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using AnimationMode = Unity.AppUI.UI.AnimationMode;
using Button = Unity.AppUI.UI.Button;
using Toggle = Unity.AppUI.UI.Toggle;
using Dropdown = Unity.AppUI.UI.Dropdown;

namespace Unity.AppUI.Samples
{
    public class Examples : MonoBehaviour
    {
        public UIDocument uiDocument;
        
        public Texture2D avatarPicture;

        const int DELETE_ACTION = 99;

        const int DISMISS_ACTION = 1;

        // Start is called before the first frame update
        void Start()
        {
            if (uiDocument)
                SetupDataBinding(uiDocument.rootVisualElement, avatarPicture);
        }

        void Update() 
        {
            if (uiDocument)
            {
                var progressValue = Mathf.Repeat(Time.realtimeSinceStartup, 10f) / 10f;
                uiDocument.rootVisualElement.Q<CircularProgress>("determinateCircularProgress").value = progressValue;
                uiDocument.rootVisualElement.Q<CircularProgress>("determinateCircularProgressWithLabel").value = progressValue;
                uiDocument.rootVisualElement.Q<Text>("determinateCircularProgressLabel").text = $"{Mathf.RoundToInt(progressValue * 100f)}%";
                uiDocument.rootVisualElement.Q<LinearProgress>("determinateLinearProgress").value = progressValue;
            }
        }

        public static void SetupDataBinding(VisualElement root, Texture2D avatarPicture = null)
        {
            root.Q<ActionButton>("alert-trigger").clickable.clickedWithEventInfo += evt =>
            {
                if (evt.target is ActionButton btn)
                {
                    var dialog = new AlertDialog
                    {
                        title = "Delete 3 documents",
                        description = "The selected documents will be deleted.",
                        variant = AlertSemantic.Destructive
                    };
                    dialog.SetPrimaryAction(DELETE_ACTION, "Delete", () => Debug.Log("Deleted"));
                    dialog.SetCancelAction(DISMISS_ACTION, "Cancel");
                    var modal = Modal.Build(btn, dialog);
                    modal.dismissed += (modalElement, dismissType) => Debug.Log("Dismissed Alert");
                    modal.Show();
                }

            };
            
            var themeSwitcher = root.Q<RadioGroup>("theme-switcher");
            var scaleSwitcher = root.Q<RadioGroup>("scale-switcher");
            var dirSwitcher = root.Q<RadioGroup>("dir-switcher");
            var panel = root.Q<Panel>("root-panel") ?? root.GetFirstAncestorOfType<Panel>();

            void OnSystemThemeChanged(string systemTheme)
            {
                panel.theme = systemTheme;
            }

            void SetTheme()
            {
                Platform.systemThemeChanged -= OnSystemThemeChanged;
                switch (themeSwitcher.value)
                {
                    case 0:
                        panel.theme = Platform.systemTheme;
                        Platform.systemThemeChanged += OnSystemThemeChanged;
                        break;
                    case 1:
                        panel.theme = "dark";
                        break;
                    case 2:
                        panel.theme = "light";
                        break;
                    case 3:
                        panel.theme = "editor-dark";
                        break;
                    case 4:
                        panel.theme = "editor-light";
                        break;
                }
                PlayerPrefs.SetInt("theme", themeSwitcher.value);
            }

            void SetScale()
            {
                switch (scaleSwitcher.value)
                {
                    case 0:
                        panel.scale = "small";
                        break;
                    case 1:
                        panel.scale = "medium";
                        break;
                    case 2:
                        panel.scale = "large";
                        break;
                }
                PlayerPrefs.SetInt("scale", scaleSwitcher.value);
            }
            
            void SetDir()
            {
                switch (dirSwitcher.value)
                {
                    case 0:
                        panel.layoutDirection = Dir.Ltr;
                        break;
                    case 1:
                        panel.layoutDirection = Dir.Rtl;
                        break;
                }
                PlayerPrefs.SetInt("dir", dirSwitcher.value);
            }

            if (themeSwitcher != null)
            {
                themeSwitcher.RegisterValueChangedCallback(_ => SetTheme());
                themeSwitcher.SetValueWithoutNotify(PlayerPrefs.GetInt("theme", 1));
                SetTheme();
            }

            if (scaleSwitcher != null)
            {
                scaleSwitcher.RegisterValueChangedCallback(_ => SetScale());
                scaleSwitcher.SetValueWithoutNotify(PlayerPrefs.GetInt("scale", 1));
                SetScale();
            }
            
            if (dirSwitcher != null)
            {
                dirSwitcher.RegisterValueChangedCallback(_ => SetDir());
                dirSwitcher.SetValueWithoutNotify(PlayerPrefs.GetInt("dir", 0));
                SetDir();
            }

            var localizationVariables = new Dictionary<string, object>
            {
                { "playerName", "Toto" },
                { "appleCount", 3 }
            };
            root.Query<LocalizedTextElement>("playerAppleCount").ForEach(localizedTextElement =>
            {
                localizedTextElement.variables = new object[] { localizationVariables };
            });
            root.Query<LocalizedTextElement>("nameIs").ForEach(localizedTextElement =>
            {
                localizedTextElement.variables = new object[] { localizationVariables };
            });

            root.Q<Button>("default-slide-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Default, NotificationDuration.Short, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("informative-slide-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Informative, NotificationDuration.Short, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("positive-slide-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Positive, NotificationDuration.Short, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("warning-slide-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Warning, NotificationDuration.Short, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("negative-slide-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Negative, NotificationDuration.Short, AnimationMode.Slide, evt.target as VisualElement));


            root.Q<Button>("default-fade-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Default, NotificationDuration.Short, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("informative-fade-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Informative, NotificationDuration.Short, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("positive-fade-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Positive, NotificationDuration.Short, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("warning-fade-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Warning, NotificationDuration.Short, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("negative-fade-short-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Negative, NotificationDuration.Short, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("default-fade-long-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Default, NotificationDuration.Long, AnimationMode.Fade, evt.target as VisualElement));

            root.Q<Button>("default-slide-indef-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Default, NotificationDuration.Indefinite, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("informative-slide-indef-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Informative, NotificationDuration.Indefinite, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("positive-slide-indef-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Positive, NotificationDuration.Indefinite, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("warning-slide-indef-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Warning, NotificationDuration.Indefinite, AnimationMode.Slide, evt.target as VisualElement));

            root.Q<Button>("negative-slide-indef-toast-button")
                .clickable.clickedWithEventInfo += (evt => OpenToast(NotificationStyle.Negative, NotificationDuration.Indefinite, AnimationMode.Slide, evt.target as VisualElement));

            var dropdownSrc = new List<string>();

            for (var i = 1; i <= 100; i++)
            {
                dropdownSrc.Add($"Choice {i}");
            }

            var dropdown1 = root.Q<Dropdown>("dropdown1");
            dropdown1.bindItem = (item, i) => item.label = dropdownSrc[i];
            dropdown1.sourceItems = dropdownSrc;
            dropdown1.SetValueWithoutNotify(new []{ 0 });
            
            var dropdown2 = root.Q<Dropdown>("dropdown2");
            dropdown2.bindItem = (item, i) => item.label = dropdownSrc[i];
            dropdown2.sourceItems = dropdownSrc;
            dropdown2.SetValueWithoutNotify(new []{ 1 });
            
            var dropdown3 = root.Q<Dropdown>("dropdown3");
            dropdown3.bindItem = (item, i) => item.label = dropdownSrc[i];
            dropdown3.sourceItems = dropdownSrc;
            dropdown3.SetValueWithoutNotify(new []{ 1, 2 });

            var img = avatarPicture ? avatarPicture : 
#if UNITY_EDITOR 
                UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.dt.app-ui/PackageResources/Images/example-avatar-pic.png")
#else 
                null
#endif
                    ;

            root.Q<UI.Avatar>("avatar-with-picture").src = Background.FromTexture2D(img);

            root.Q<ColorSlider>("rainbow-slider").colorRange.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0),
                    new GradientColorKey(Color.yellow, 0.2f),
                    new GradientColorKey(Color.green, 0.45f),
                    new GradientColorKey(Color.cyan, 0.55f),
                    new GradientColorKey(Color.blue, 0.66f),
                    new GradientColorKey(Color.magenta, 0.85f),
                    new GradientColorKey(Color.red, 1f),
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                }
            );
            
            root.Q<ActionButton>("menu-action-code").clickable.clickedWithEventInfo += (evt =>
                OpenMenu((VisualElement)evt.target));
            
            var gridViewDefault = root.Q<GridView>("grid-view-default");
            gridViewDefault.columnCount = 3;
            gridViewDefault.itemHeight = 50;
            gridViewDefault.bindItem = (item, i) => item.Q<Text>().text = $"Item {i}";
            gridViewDefault.makeItem = () =>
            {
                var text = new Text();
                var itemRoot = new VisualElement();
                itemRoot.Add(text);
                return itemRoot;
            };
            gridViewDefault.itemsSource = Enumerable.Range(0, 100).ToList();
            gridViewDefault.selectionType = SelectionType.Multiple;
            gridViewDefault.selectionChanged += selection => Debug.Log($"Selection changed: {string.Join(", ", selection)}");
            gridViewDefault.itemsChosen += selection => Debug.Log($"Items chosen: {string.Join(", ", selection)}");
            gridViewDefault.doubleClicked += indexUnderMouse => Debug.Log($"Double clicked: {indexUnderMouse}");

            root.Q<Button>("haptics-light-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.LIGHT);
            root.Q<Button>("haptics-medium-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.MEDIUM);
            root.Q<Button>("haptics-heavy-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.HEAVY);
            root.Q<Button>("haptics-success-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.SUCCESS);
            root.Q<Button>("haptics-error-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.ERROR);
            root.Q<Button>("haptics-warning-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.WARNING);
            root.Q<Button>("haptics-selection-btn").clicked += () => Platform.RunHapticFeedback(HapticFeedbackType.SELECTION);

            var leftDrawer = root.Q<Drawer>("left-drawer");
            root.Q<Button>("open-left-drawer-btn").clicked += leftDrawer.Open;

            var rightDrawer = root.Q<Drawer>("right-drawer");
            root.Q<Button>("open-right-drawer-btn").clicked += rightDrawer.Open;

            var permanentDrawer = root.Q<Drawer>("permanent-drawer");
            var temporaryDrawer = root.Q<Drawer>("temporary-drawer");
            var drawerVariantSwitcher = root.Q<RadioGroup>("drawer-variant-switcher");
            var openTempDrawerBtn = root.Q<Button>("open-temp-drawer-btn");
            var openSnapTempDrawerBtn = root.Q<Button>("open-snap-temp-drawer-btn");
            var closeSnapTempDrawerBtn = root.Q<Button>("close-snap-temp-drawer-btn");
            openTempDrawerBtn.clicked += temporaryDrawer.Open;
            openSnapTempDrawerBtn.clicked += () => temporaryDrawer.isOpen = true;
            closeSnapTempDrawerBtn.clicked += () => temporaryDrawer.isOpen = false;
            temporaryDrawer.AddToClassList(Styles.hiddenUssClassName);
            drawerVariantSwitcher.SetValueWithoutNotify(1);
            drawerVariantSwitcher.RegisterValueChangedCallback(evt =>
            {
                switch (evt.newValue)
                {
                    case 0:
                        permanentDrawer.AddToClassList(Styles.hiddenUssClassName);
                        temporaryDrawer.RemoveFromClassList(Styles.hiddenUssClassName);
                        openTempDrawerBtn.SetEnabled(true);
                        openSnapTempDrawerBtn.SetEnabled(true);
                        closeSnapTempDrawerBtn.SetEnabled(true);
                        break;
                    case 1:
                        temporaryDrawer.AddToClassList(Styles.hiddenUssClassName);
                        permanentDrawer.RemoveFromClassList(Styles.hiddenUssClassName);
                        openTempDrawerBtn.SetEnabled(false);
                        openSnapTempDrawerBtn.SetEnabled(false);
                        closeSnapTempDrawerBtn.SetEnabled(false);
                        break;
                }
            });
            
            var badge1 = root.Q<Badge>("badge-1");
            var badge2 = root.Q<Badge>("badge-2");
            var badge3 = root.Q<Badge>("badge-3");
            var badge4 = root.Q<Badge>("badge-4");

            var hAnchorBadge = root.Q<RadioGroup>("h-anchor-badge");
            hAnchorBadge.RegisterValueChangedCallback(evt =>
            {
                badge1.horizontalAnchor = (HorizontalAnchor)evt.newValue;
                badge2.horizontalAnchor = (HorizontalAnchor)evt.newValue;
                badge3.horizontalAnchor = (HorizontalAnchor)evt.newValue;
                badge4.horizontalAnchor = (HorizontalAnchor)evt.newValue;
            });
            hAnchorBadge.value = (int)HorizontalAnchor.Right;
            var vAnchorBadge = root.Q<RadioGroup>("v-anchor-badge");
            vAnchorBadge.RegisterValueChangedCallback(evt =>
            {
                badge1.verticalAnchor = (VerticalAnchor)evt.newValue;
                badge2.verticalAnchor = (VerticalAnchor)evt.newValue;
                badge3.verticalAnchor = (VerticalAnchor)evt.newValue;
                badge4.verticalAnchor = (VerticalAnchor)evt.newValue;
            });
            vAnchorBadge.value = (int)VerticalAnchor.Top;

            var swipeViewH = root.Q<SwipeView>("swipeview-horizontal");
            var swipePrevButtonH = root.Q<ActionButton>("swipeview-h-prev");
            var swipeNextButtonH = root.Q<ActionButton>("swipeview-h-next");
            var swipeFiveButtonH = root.Q<ActionButton>("swipeview-h-five");
            var swipeFiveSnapButtonH = root.Q<ActionButton>("swipeview-h-five-snap");

            swipePrevButtonH.SetEnabled(swipeViewH.canGoToPrevious);

            swipeViewH.RegisterValueChangedCallback(evt =>
            {
                swipePrevButtonH.SetEnabled(swipeViewH.canGoToPrevious);
                swipeNextButtonH.SetEnabled(swipeViewH.canGoToNext);
            });
            swipePrevButtonH.RegisterCallback<ClickEvent>(evt =>
                swipeViewH.GoToPrevious());
            swipeNextButtonH.RegisterCallback<ClickEvent>(evt =>
                swipeViewH.GoToNext());
            swipeFiveButtonH.RegisterCallback<ClickEvent>(evt => swipeViewH.GoTo(4));
            swipeFiveSnapButtonH.RegisterCallback<ClickEvent>(evt => swipeViewH.SnapTo(4));


            var swipeViewHW = root.Q<SwipeView>("swipeview-horizontal-wrap");
            var swipePrevButtonHW = root.Q<ActionButton>("swipeview-hw-prev");
            var swipeNextButtonHW = root.Q<ActionButton>("swipeview-hw-next");

            swipePrevButtonHW.SetEnabled(swipeViewHW.canGoToPrevious);

            swipeViewHW.RegisterValueChangedCallback(evt =>
            {
                swipePrevButtonHW.SetEnabled(swipeViewHW.canGoToPrevious);
                swipeNextButtonHW.SetEnabled(swipeViewHW.canGoToNext);
            });
            swipePrevButtonHW.RegisterCallback<ClickEvent>(evt =>
                swipeViewHW.GoToPrevious());
            swipeNextButtonHW.RegisterCallback<ClickEvent>(evt =>
                swipeViewHW.GoToNext());


            var swipeViewV = root.Q<SwipeView>("swipeview-vertical");
            var swipePrevButtonV = root.Q<ActionButton>("swipeview-v-prev");
            var swipeNextButtonV = root.Q<ActionButton>("swipeview-v-next");

            swipePrevButtonV.SetEnabled(false);

            swipeViewV.RegisterValueChangedCallback(evt =>
            {
                swipePrevButtonV.SetEnabled(swipeViewV.canGoToPrevious);
                swipeNextButtonV.SetEnabled(swipeViewV.canGoToNext);
            });
            swipePrevButtonV.RegisterCallback<ClickEvent>(evt =>
                swipeViewV.GoToPrevious());
            swipeNextButtonV.RegisterCallback<ClickEvent>(evt =>
                swipeViewV.GoToNext());

            var swipeViewD = root.Q<SwipeView>("swipeview-distance");
            swipeViewD.beingSwiped += OnBeingSwiped;
            
            root.Q<Chip>("filled-chip-ornament").ornament = new Image
            {
                image = img,
                style =
                {
                    width = new StyleLength(new Length(100, LengthUnit.Percent)),
                    height = new StyleLength(new Length(100, LengthUnit.Percent))
                }
            };

            root.Q<Chip>("outlined-chip-ornament").ornament = new Image
            {
                image = img,
                style =
                {
                    width = new StyleLength(new Length(100, LengthUnit.Percent)),
                    height = new StyleLength(new Length(100, LengthUnit.Percent))
                }
            };

            root.Query<AvatarGroup>("avatar-group-1").ForEach(avatarGroup1 =>
            {
                avatarGroup1.sourceItems = Enumerable.Range(0, 10).ToList();
                avatarGroup1.bindItem = (item, i) =>
                {
                    var text = new Text($"A{i}");
                    item.Add(text);
                    // random color 
                    var color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                    item.backgroundColor = color;
                    // text color in contrast with background color
                    var grayscale = color.grayscale;
                    text.style.color = grayscale > 0.5f ? Color.black : Color.white;
                };
            });

            var tabViewTabs = root.Q<Tabs>("tabview-tabs");
            var tabViewSwiperView = root.Q<SwipeView>("tabview-swipeview");
            tabViewSwiperView.swipeable = false;
            tabViewTabs.RegisterValueChangedCallback(evt =>
            {
                tabViewSwiperView.SetValueWithoutNotify(evt.newValue);
            });
            tabViewSwiperView.RegisterValueChangedCallback(evt =>
            {
                tabViewTabs.SetValueWithoutNotify(evt.newValue);
            });

            var stackView = root.Q<StackView>("stack-view");
            stackView.Push(new Text("1"));
            stackView.pushEnterAnimation = new AnimationDescription
            {
                durationMs = 500,
                easing = Easing.OutBack,
                callback = (element, f) =>
                {
                    var progress = 1f - f;
                    element.style.rotate = new StyleRotate(new Rotate(30f * progress));
                    element.style.left = progress * stackView.resolvedStyle.width;
                }
            };
            stackView.popExitAnimation = new AnimationDescription
            {
                durationMs = 500,
                easing = Easing.InBack,
                callback = (element, f) =>
                {
                    var progress = 1f - f;
                    element.style.rotate = new StyleRotate(new Rotate(-30f * progress));
                    var scale = f;
                    element.style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1.0f)));
                    element.style.top = progress * stackView.resolvedStyle.height;
                }
            };
            root.Q<ActionButton>("sv-push-btn").clickable.clicked += () =>
            {
                var next = stackView.depth + 1;
                stackView.Push(new Text(next.ToString()));
            };
            root.Q<ActionButton>("sv-pop-btn").clickable.clicked += () =>
            {
                stackView.Pop();
            };
            root.Q<ActionButton>("sv-pop2-btn").clickable.clicked += () =>
            {
                if (stackView.depth >= 3)
                    stackView.Pop(stackView.ElementAt(stackView.depth - 3) as StackViewItem);
            };
            root.Q<ActionButton>("sv-pop-null-btn").clickable.clicked += () =>
            {
                stackView.Pop(null);
            };
            root.Q<ActionButton>("sv-clear-btn").clickable.clicked += () =>
            {
                stackView.ClearStack();
            };

            root.Q<RangeSliderFloat>("rsfm").RegisterValueChangedCallback(evt =>
            {
                Debug.Log($"RangeSliderFloat Changed: {evt.newValue}");
            });
            
            root.Q<RangeSliderFloat>("rsfm").RegisterValueChangingCallback(evt =>
            {
                Debug.Log($"RangeSliderFloat Changing: {evt.newValue}");
            });
            
            root.Q<SliderFloat>("sfm").RegisterValueChangedCallback(evt =>
            {
                Debug.Log($"SliderFloat Changed: {evt.newValue}");
            });
            
            root.Q<SliderFloat>("sfm").RegisterValueChangingCallback(evt =>
            {
                Debug.Log($"SliderFloat Changing: {evt.newValue}");
            });
        }

        static void OpenToast(NotificationStyle style, NotificationDuration duration, AnimationMode animationMode, VisualElement ve)
        {
            var toast = Toast.Build(ve, "A Toast Message", duration)
                .SetStyle(style)
                .SetAnimationMode(animationMode);

            if (style == NotificationStyle.Informative)
                toast.SetIcon("info");

            if (duration == NotificationDuration.Indefinite)
                toast.SetAction(DISMISS_ACTION, "Dismiss", () => Debug.Log("Dismissed"));

            toast.Show();
        }

        static void OpenMenu(VisualElement anchor)
        {
            MenuBuilder.Build(anchor)
                .AddAction(123, "An Item", "info", evt => Debug.Log("Item clicked"))
                .PushSubMenu(456, "My Sub Menu", "help")
                    .AddAction(789, "Sub Menu Item", "info", evt => Debug.Log("Sub Item clicked"))
                    .PushSubMenu(3455, "Another Sub Menu", "help")
                        .AddAction(7823129, "Another Sub Menu Item", "info", evt => Debug.Log("Other Item clicked"))
                    .Pop()
                .Pop()
                .Show();
        }

        static void OnBeingSwiped(SwipeViewItem element, float distance)
        {
            var child = element.ElementAt(0);

            if (child == null)
                return;

            var minOpacity = 0.33f;
            var maxOpacity = 1.0f;
            var newOpacity = Mathf.Lerp(minOpacity, maxOpacity, Mathf.Clamp01(1.0f - Mathf.Abs(distance)));
            child.style.opacity = newOpacity;

            var minScale = 0.8f;
            var maxScale = 1.0f;
            var newScale = Mathf.Lerp(minScale, maxScale, Mathf.Clamp01(1.0f - Mathf.Abs(distance)));
            child.style.scale = new Scale(new Vector3(newScale, newScale, 1.0f));
        }
    }

}
