using Microsoft.Xna.Framework;
using System;

namespace Engine.Animations;

public enum AnimationCurve {
	LINEAR,
	SQUARED // IDK What to call it rn.
}

public class Animation 
{
	public int valueID;
	public AnimationClip clip; // The clip this animation is part of.

	public bool playing = false;
	
	public float animTime = 0; // This is a float between 0 and 1 representing where the animation is up to.

	public float minValue;
	public float maxValue;
	public float currentValue;
	
	public float animLength;
	public float currentTime = 0;

	public AnimationCurve curve = AnimationCurve.LINEAR;

	public Animation(int valueID, float minValue, float maxValue, float animLength)
	{
		this.valueID = valueID;
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.animLength = animLength;

		ProcessAnimation();
	}

	public void UpdateAnimation(GameTime gameTime) // Processes all the updates, then returns what the value of the animation is.
	{
		if (playing == false) return;

		currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (currentTime > animLength) {
			currentTime = animLength;
			playing = false;
		}

		animTime = currentTime / animLength;

		ProcessAnimation();

		clip.values[valueID] = currentValue;
	}

	public void ResetAnimation()
	{
		currentTime = 0;
		currentValue = minValue;
		animTime = 0;
		playing = false;

		ProcessAnimation();
		Console.WriteLine($"minValue is {minValue} and clip.values[0] is {clip.values[valueID]}");
	}

	private void ProcessAnimation()
	{
		switch (curve) {
			case AnimationCurve.LINEAR: 
				currentValue = minValue + (maxValue - minValue) * animTime; 
				break;
			//case AnimationCurve.SQUARED:
		}
	}
}