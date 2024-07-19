using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

namespace Engine.Core;

public static class KeyboardExtended
{
	private static KeyboardState keyboardState;
	private static KeyboardState previousKeyboardState;

	// Helper methods to detect if a key is pressed or released, extending the Keyboard class of 
	public static bool KeyPressed(Keys key)
	{
		bool keyState = keyboardState.IsKeyDown(key);
		return (keyState != previousKeyboardState.IsKeyDown(key) && keyState == true);
	}

	public static bool KeyReleased(Keys key)
	{
		bool keyState = keyboardState.IsKeyDown(key);
		return (keyState != previousKeyboardState.IsKeyDown(key) && keyState == false);
	}

	// THIS MUST BE SET BEFORE ANY UPDATES INVOLVING KEYS
	public static void SetState()
	{
		keyboardState = Keyboard.GetState();
	}

	// THIS MUST BE SET AFTER ANY UPDATES INVOLVING KEYS
	public static void SetPreviousState()
	{
		previousKeyboardState = keyboardState;
	}
}