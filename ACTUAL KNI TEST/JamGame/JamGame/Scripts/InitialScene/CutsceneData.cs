using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace JamGame;

public class CutsceneData 
{
	public bool dataComplete = false;

	public Sprite background;

	public int currentDialogueIndex = -1;

	public string[] dialogue;
	public Vector2[] dialoguePositions;
	public Color[] dialogueColor;

	public float[] dialogueTimings; // This should always be one larger than dialogue; 
	// index 0 represents the delay between data switching and the first text appearing.
	// Times are representative of what point in the song (overall game time) the text should appear.

	public SpriteFont font;

	public CutsceneData(string spriteName, string[] dialogue, Vector2[] dialoguePositions, Color[] dialogueColor, 
		float[] dialogueTimings, ContentManager contentManager)
	{
		this.dialogue = dialogue;
		this.dialoguePositions = dialoguePositions;
		this.dialogueColor = dialogueColor;
		this.dialogueTimings = dialogueTimings;

		this.background = new Sprite(new Vector2(640, 480), spriteName, contentManager);

		this.font = contentManager.Load<SpriteFont>("Low Gothic Cutscene");
	}

	public void Update(float sceneTime)
	{
		if (currentDialogueIndex >= dialogue.Length) {
			dataComplete = true;
			return;
		}

		if (sceneTime >= dialogueTimings[currentDialogueIndex + 1]) currentDialogueIndex += 1;
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		background.Draw(_spriteBatch);
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		if (currentDialogueIndex > -1 && currentDialogueIndex < dialogue.Length) {
			_spriteBatch.DrawString(font, $"{dialogue[currentDialogueIndex]}", dialoguePositions[currentDialogueIndex], dialogueColor[currentDialogueIndex]);
		}
	}
}