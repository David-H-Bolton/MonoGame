using System;
using System.Collections.Generic;
using System.Linq;
using Java.Interop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace ScreenManager
{

	public class MenuEventArgs : EventArgs { 
		public int Index { get; set; }
		public MenuEventArgs(int index)
		{
			Index = index;
		}
	}
	public delegate void MenuEventHandler(object sender, MenuEventArgs e);
	public class MenuItem
    {
		public Rectangle TouchArea { get; set; }
		public MenuEventHandler AnEvent { get;set; }
		public int Index { get; set; }

		public string ItemStr { get; set; }
		public bool Flash { get; set; }
    }
	public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
	{
		string[] menuItems;
		int selectedIndex;

		Color normal = Color.White;
		Color hilite = Color.Yellow;

		KeyboardState keyboardState;
		KeyboardState oldKeyboardState;
		TouchCollection touchState;
		List<MenuItem> items = new List<MenuItem>();

		SpriteBatch spriteBatch;
		SpriteFont spriteFont;

		Vector2 position;
		float width = 0f;
		float height = 0f;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				if (selectedIndex < 0)
					selectedIndex = 0;
				if (selectedIndex >= menuItems.Length)
					selectedIndex = menuItems.Length - 1;
			}
		}

		public MenuComponent(Game game, 
			SpriteBatch spriteBatch, 
			SpriteFont spriteFont, 
			string[] menuItems)
			: base(game)
		{
			this.spriteBatch = spriteBatch;
			this.spriteFont = spriteFont;
			this.menuItems = menuItems;
			MeasureMenu();
		}

		private void MeasureMenu()
		{
			height = 0;
			width = 0;
			var _index = 0;
			foreach (string item in menuItems)
			{
				Vector2 size = spriteFont.MeasureString(item);
				if (size.X > width)
					width = size.X;
				height += spriteFont.LineSpacing + 5;
			}
			position = new Vector2(
				(Game.Window.ClientBounds.Width - width) / 2,
				(Game.Window.ClientBounds.Height - height) / 2);
			var tempPos = position;
			foreach (string item in menuItems)
			{
				MenuItem mItem = new MenuItem()
				{
					AnEvent = Click,
					TouchArea = new Rectangle((int)tempPos.X, (int)tempPos.Y, (int)width, (int)spriteFont.LineSpacing),
					Index = _index++,
					ItemStr = item,
					Flash = false
				};
				items.Add(mItem);
				tempPos.Y += spriteFont.LineSpacing + 5;
			}
		}

		public void Click(object sender,MenuEventArgs args)
        {
			selectedIndex = args.Index;
			items[selectedIndex].Flash = true;
        }

		public override void Initialize()
		{
			base.Initialize();
		}

		private bool CheckKey(Keys theKey)
		{
			return keyboardState.IsKeyUp(theKey) && 
				oldKeyboardState.IsKeyDown(theKey);
		}

		public override void Update(GameTime gameTime)
		{
			keyboardState = Keyboard.GetState();

			if (CheckKey(Keys.Down))
			{
				selectedIndex++;
				if (selectedIndex == menuItems.Length)
					selectedIndex = 0;
			}
			if (CheckKey(Keys.Up))
			{
				selectedIndex--;
				if (selectedIndex < 0)
					selectedIndex = menuItems.Length - 1;
			}
			touchState = TouchPanel.GetState();

			foreach (var touch in touchState)
			{
				if (touch.State == TouchLocationState.Pressed)
				{
					var x = touch.Position.X;
					var y = touch.Position.Y;
					foreach(var mItem in items)
                    {
						if (mItem.TouchArea.Contains(x, y))
                        {
							mItem.AnEvent.Invoke(this, new MenuEventArgs(mItem.Index));
							break;
                        }
                    }
					//do what you want here when users tap the screen
				}
			}
			base.Update(gameTime);

			oldKeyboardState = keyboardState;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			Vector2 location = position;
			Color tint;
			spriteBatch.DrawString(spriteFont, $"PiSys Game Menu", new Vector2(120, 100), Color.Black);
			for (int i = 0; i < menuItems.Length; i++)
			{
				if (i == selectedIndex)
					tint = hilite;
				else
					tint = normal;
				if (items[i].Flash)
                {
					tint = Color.Red;
                }
				spriteBatch.DrawString(
					spriteFont,
					items[i].ItemStr,
					location,
					tint);
				location.Y += spriteFont.LineSpacing + 5;
				items[i].Flash = false;
			}
		}
	}
}
