using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Core;

namespace Engine.Animations;

// Data for this class will probably be loaded via XML.
public class AnimationClip
{
	public bool loop;
	public bool playing = false;
	public bool lastKeyframeActive = false;

	// Values are modified by each individual animation: they send their current values to a valueID represented by the array index.
	// This is similar to Unity's animation system, where each parameter can be modified by different keyframes in the animation.
	// It's more general than the Unity system however, as these values do not correspond to specific values in the gameobject the 
	// animation is on.
	public float[] values;

	public float animTime = 0;

	public float[] keyframeTimes;
	public Animation[][] keyframeData;

	public float currentTime = 0;
	public float animLength;

	private int currentKeyframeIndex = 0;

	private List<Animation> activeAnimations;
	private List<int> completedAnimations;

	public AnimationClip()
	{

	}

	public AnimationClip(int valueCount, float[] keyframeTimes, Animation[][] keyframeData, float animLength, bool loop)
	{
		this.values = new float[valueCount];

		this.loop = loop;
		this.keyframeTimes = keyframeTimes;
		this.keyframeData = keyframeData;
		this.animLength = animLength;

		activeAnimations = new List<Animation>();
		completedAnimations = new List<int>();

		foreach (Animation[] keyframes in keyframeData) {
			foreach (Animation data in keyframes) {
				data.clip = this;
			}
		}
	}

	public void Update(GameTime gameTime)
	{
		if (!playing) return;

		currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

		// Check if the animation is completed, and handle looping or stopping.
		if (currentTime >= animLength) {
			if (loop) {
				ResetAnimation();
			}
			else {
				currentTime = 1;
				playing = false;
			}
		}

		animTime = currentTime / animLength;

		// Check if any new keyframes need to be added this frame
		// this needs to be rewritten to check if multiple keyframes are activated in one frame.
		if (!lastKeyframeActive && currentTime >= keyframeTimes[currentKeyframeIndex]) {
			if (currentKeyframeIndex == keyframeTimes.Length - 1) lastKeyframeActive = true;

			foreach (Animation animation in keyframeData[currentKeyframeIndex]) {
				animation.playing = true;
				activeAnimations.Add(animation);
			}

			currentKeyframeIndex += 1;
		}

		// Handle all active animations
		for (int i = 0; i < activeAnimations.Count; i++) {
			activeAnimations[i].UpdateAnimation(gameTime);

			if (!activeAnimations[i].playing) completedAnimations.Add(i);
		}

		// Remove finished animation from the active animation list.
		for (int i = 0; i < completedAnimations.Count; i++) {
			activeAnimations.RemoveAt(i);
		}

		// Clear the completed animations for next frame.
		completedAnimations.Clear();
	}

	public void ResetAnimation()
	{
		currentTime = 0;
		animTime = 0;
		lastKeyframeActive = false;
		currentKeyframeIndex = 0;

		foreach(Animation[] keyframe in keyframeData) {
			for (int i = 0; i < keyframe.Length; i++) {
				keyframe[i].ResetAnimation();
			}
		}
	}

	// Each animation has a valueID, which updates the relevant value in the animation clip. This returns the current value of that ID.
	public float GetValue(int valueID)
	{
		return values[valueID];
	}
}