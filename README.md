# ImGuiWrapper
This is a Simple Wrapper for ImGui.

Demo:
```csharp
using LocoSoftware.ImGuiWrapper;
using ImGuiNET;

public class State 
{ 
	public bool BuiltInDemoWindowIsOpen { get;set; }
}

public static class Program
{
	public static void Style(ImGuiStylePtr stylePtr, ImGuiIOPtr ioPtr) 
	{
		// Add a Font
		if (!Path.Exists("JetBrainsMono-Regular.ttf")) 
		{
			throw new FileNotFoundException("Font File: JetBrainsMono-Regular.ttf not found!")
		}
		io.Fonts.AddFontFromFileTTF("JetBrainsMono-Regular.ttf", 16);
		style.WindowBorderSize = 0;
        style.FrameBorderSize = 0;
        style.PopupBorderSize = 0;
        style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
        style.ScrollbarSize = 6.0f;
        style.FrameRounding = 5.0f;
        style.WindowRounding = 5.0f;
        style.ChildRounding = 5.0f;
        style.PopupRounding = 5.0f;
	}

	public static void UI(State state) 
	{ 
		if (state.BuiltInDemoWindowIsOpen)
		{
			ImGui.ShowDemoWindow;
		}

		if (ImGui.Button("Show Demo Window"))
		{
			state.BuiltInDemoWindowIsOpen = true;
		}
	}

	public static void Main()
	{
		var state = new State();
		var runner = new UIController<State>(
			UI, /* UI Method */
			Style, /* Style Method */
			state, /* State - Gets passed to UI Method on each Render Iteration */
			new Vector2(500, 500), /* Window Size */
			new Vector2(100, 100), /* Window starting Position */
			"LocoSoftware.ImGuiWrapper Demo Window", /* Window Title */
			new Vector3(0.0f, 0.0f, 0.0f) /* Window Clear Color - background Color */
		);
	}
}
```