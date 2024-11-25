using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class Theme : IUIItem
	{
		[SerializeField] private string _name;

		[SerializeField] private string _id;

		[SerializeField] private List<ColorProperty> _colors;

		[SerializeField] private List<SpriteProperty> _sprites;

		[SerializeField] private List<FontProperty> _fonts;

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		public List<ColorProperty> Colors
		{
			get => _colors;
			set => _colors = value;
		}

		public List<SpriteProperty> Sprites
		{
			get => _sprites;
			set => _sprites = value;
		}

		public List<FontProperty> Fonts
		{
			get => _fonts;
			set => _fonts = value;
		}

		public ColorProperty GetColorProperty(string id)
		{
			return Colors.FirstOrDefault(x => x.Id == id);
		}

		public SpriteProperty GetSpriteProperty(string id)
		{
			return Sprites.FirstOrDefault(x => x.Id == id);
		}

		public FontProperty GetFontProperty(string id)
		{
			return Fonts.FirstOrDefault(x => x.Id == id);
		}

		public Theme()
		{
			Id = Guid.NewGuid().ToString();
			Name = "New Theme";

			Colors = new List<ColorProperty>();
			Sprites = new List<SpriteProperty>();
			Fonts = new List<FontProperty>();
		}

		public Theme(Theme source)
		{
			Id = Guid.NewGuid().ToString();
			Name = $"{source._name} (Clone)";

			Colors = new List<ColorProperty>();
			foreach (var sourceProp in source._colors)
			{
				var prop = new ColorProperty
				{
					Id = sourceProp.Id,
					Name = sourceProp.Name,
					Value = sourceProp.Value
				};

				Colors.Add(prop);
			}

			Sprites = new List<SpriteProperty>();
			foreach (var sourceProp in source._sprites)
			{
				var prop = new SpriteProperty
				{
					Id = sourceProp.Id,
					Name = sourceProp.Name,
					Value = sourceProp.Value
				};

				Sprites.Add(prop);
			}

			Fonts = new List<FontProperty>();
			foreach (var sourceProp in source.Fonts)
			{
				var prop = new FontProperty
				{
					Id = sourceProp.Id,
					Name = sourceProp.Name,
					Value = sourceProp.Value
				};

				Fonts.Add(prop);
			}
		}
	}
}