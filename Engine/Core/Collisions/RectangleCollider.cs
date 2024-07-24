using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Engine.Core;

// You'll probably end up in the core engine...
public class RectangleCollider //: Collider
{
	// I'm not sure I like that this has a transform. The position could be updated from the other object, with an offset applied?
	public Transform transform;

	// Collision Variables
    private Point colliderOffset;                   // (0, 0) is the top left pixel of the sprite.
    public Rectangle collider;

    private List<RectangleCollider> currentlyIntersectingColliders;

	public RectangleCollider(Transform transform, Point colliderOffset, Point size)
	{
		this.transform = transform;
		this.colliderOffset = colliderOffset;
        this.collider = new Rectangle(new Point((int)transform.position.X, (int)transform.position.Y) + colliderOffset, size);
        currentlyIntersectingColliders = new List<RectangleCollider>();
	}

	// This constructor should be used on static colliders which aren't associated with colliders (such as triggers?)
	public RectangleCollider(Point position, Point size)
	{
		this.collider = new Rectangle(position, size);
		currentlyIntersectingColliders = new List<RectangleCollider>();
	}

	// Should this be named Update for consistency or UpdateColliderfor clarity?
	// Moving objects with a collider must update the collider each frame. This should be done after the frame's movement
	public void UpdateCollider()
	{
		collider.Location = new Point((int)transform.position.X, (int)transform.position.Y) + colliderOffset;

		// Update the colliders which are in the list.
		for (int i = 0; i < currentlyIntersectingColliders.Count - 1; i++) {
			if (CheckForCollision(currentlyIntersectingColliders[i]) == false) {
				currentlyIntersectingColliders.RemoveAt(i);
				i -= 1;
			}
		}
	}

	// Should this be named Update for consistency or UpdateTrigger for clarity?
	// Triggers are colliders that don't handle collision logic; ie a trigger must overlap with a normal collider for something to happen.
	public void UpdateTrigger()
	{
		collider.Location = new Point((int)transform.position.X, (int)transform.position.Y) + colliderOffset;
	}

	// Useful for checking a collision constantly
	public bool CheckForCollision(RectangleCollider other)
	{
		return this.collider.Intersects(other.collider);
	}

	public bool CollisionEntered(RectangleCollider other, GameTime gameTime)
	{
		if (CheckForCollision(other)) {
			for (int i = 0; i < currentlyIntersectingColliders.Count; i++) {
				if (other == currentlyIntersectingColliders[i]) {
					return false;
				}
			}

			currentlyIntersectingColliders.Add(other);
			return true;
		}

		return false;
	}
}