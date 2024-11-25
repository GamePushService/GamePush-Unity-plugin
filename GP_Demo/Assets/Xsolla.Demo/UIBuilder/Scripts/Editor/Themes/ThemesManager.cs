using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public static class ThemesManager
	{
		#region Themes

		public static void CreateTheme()
		{
			var themes = ThemesLibrary.Themes;
			var current = ThemesLibrary.Current;

			Theme newTheme;
			if (current != null)
			{
				newTheme = new Theme(current);
			}
			else
			{
				newTheme = new Theme();

				newTheme.Colors.Add(new ColorProperty
				{
					Name = "Main Color",
					Value = Color.white
				});

				newTheme.Colors.Add(new ColorProperty
				{
					Name = "Second Color",
					Value = Color.black
				});
			}

			themes.Add(newTheme);

			if (ThemesLibrary.Current == null)
			{
				ThemesLibrary.Current = newTheme;
			}
		}

		public static void DeleteTheme(string id)
		{
			var themes = ThemesLibrary.Themes;
			var theme = themes.FirstOrDefault(x => x.Id == id);
			if (theme != null)
			{
				themes.Remove(theme);
			}

			if (themes.Count == 0)
			{
				ThemesLibrary.Current = null;
			}
			else
			{
				if (ThemesLibrary.Current == null)
				{
					ThemesLibrary.Current = themes.FirstOrDefault();
				}
			}
		}

		public static void ChangeThemeName(string id, string name)
		{
			if (string.IsNullOrEmpty(id))
				throw new InvalidOperationException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(name));

			var theme = ThemesLibrary.Themes.FirstOrDefault(x => x.Id == id);
			if (theme != null)
			{
				theme.Name = name;
			}
		}

		public static void ChangeThemesOrder(int oldIndex, int newIndex)
		{
			SwapElementsInList(ThemesLibrary.Themes, oldIndex, newIndex);
		}

		#endregion

		#region Colors

		public static void CreateColor()
		{
			var id = Guid.NewGuid().ToString();
			var name = "New Color";
			var color = Color.white;

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = new ColorProperty
				{
					Id = id,
					Name = name,
					Value = color
				};

				theme.Colors.Add(prop);
			}
		}

		public static void DeleteColor(string id)
		{
			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetColorProperty(id);
				if (prop != null)
				{
					theme.Colors.Remove(prop);
				}
			}
		}

		public static void ChangeColorName(string id, string name)
		{
			if (string.IsNullOrEmpty(id))
				throw new InvalidOperationException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(name));

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetColorProperty(id);
				if (prop != null)
				{
					prop.Name = name;
				}
			}
		}

		public static void ChangeColorsOrder(int oldIndex, int newIndex)
		{
			foreach (var theme in ThemesLibrary.Themes)
			{
				SwapElementsInList(theme.Colors, oldIndex, newIndex);
			}
		}

		#endregion

		#region Sprites

		public static void CreateSprite()
		{
			var id = Guid.NewGuid().ToString();
			var name = "New Sprite";

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = new SpriteProperty
				{
					Id = id,
					Name = name
				};

				theme.Sprites.Add(prop);
			}
		}

		public static void DeleteSprite(string id)
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetSpriteProperty(id);
				if (prop != null)
				{
					theme.Sprites.Remove(prop);
				}
			}
		}

		public static void ChangeSpriteName(string id, string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(name));

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetSpriteProperty(id);
				if (prop != null)
				{
					prop.Name = name;
				}
			}
		}

		public static void ChangeSpritesOrder(int oldIndex, int newIndex)
		{
			foreach (var theme in ThemesLibrary.Themes)
			{
				SwapElementsInList(theme.Sprites, oldIndex, newIndex);
			}
		}

		#endregion

		#region Fonts

		public static void CreateFont()
		{
			var id = Guid.NewGuid().ToString();
			var name = "New Font";

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = new FontProperty()
				{
					Id = id,
					Name = name
				};

				theme.Fonts.Add(prop);
			}
		}

		public static void DeleteFont(string id)
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetFontProperty(id);
				if (prop != null)
				{
					theme.Fonts.Remove(prop);
				}
			}
		}

		public static void ChangeFontName(string id, string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new InvalidOperationException(nameof(name));

			foreach (var theme in ThemesLibrary.Themes)
			{
				var prop = theme.GetFontProperty(id);
				if (prop != null)
				{
					prop.Name = name;
				}
			}
		}

		public static void ChangeFontsOrder(int oldIndex, int newIndex)
		{
			foreach (var theme in ThemesLibrary.Themes)
			{
				SwapElementsInList(theme.Fonts, oldIndex, newIndex);
			}
		}

		#endregion

		#region Utils

		private static void SwapElementsInList<T>(List<T> list, int oldIndex, int newIndex)
		{
			if (oldIndex < 0 || oldIndex >= list.Count)
				throw new ArgumentOutOfRangeException($"{nameof(oldIndex)}: {oldIndex}");

			if (newIndex < 0 || newIndex >= list.Count)
				throw new ArgumentOutOfRangeException($"{nameof(newIndex)}: {newIndex}");
			
			var item = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, item);
		}

		#endregion
	}
}