using Godot;
using System;

public partial class ConsoleDisplay : RichTextLabel
{
	public static string text = "";

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Text != text) 
		{ 
			Text = text;
			ScrollToLine(GetLineCount());
		}
	}
}
