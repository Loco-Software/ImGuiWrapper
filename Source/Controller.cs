using System;
using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace LocoSoftware.ImGuiWrapper;

/// <summary>
/// UI Controller to run a UI Function
/// </summary>
/// <typeparam name="TState">Type of State Object that gets passed to UI Function on each Render Pass</typeparam>
public class UIController<TState> where TState : new()
{
    private readonly Sdl2Window window;
    private readonly GraphicsDevice graphicsDevice;
    private readonly CommandList commandList;
    private readonly ImGuiRenderer renderer;
    private readonly Action<TState> uiFunction;
    private readonly Vector3 clearColor;
    private readonly TState state;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="uiFunction">User-provided UI</param>
    /// <param name="styleFunction">User-provided Function that configures Styles at Startup. Usage primarley for Fonts</param>
    /// <param name="state">State of Type TState</param>
    /// <param name="size">Initial Window Size</param>
    /// <param name="pos">Initial Window Position</param>
    /// <param name="title">Window Title</param>
    /// <param name="clearColor">Clear Color (Background Color)</param>
    /// <param name="windowState">Window Startup State</param>
    public UIController(Action<TState> uiFunction, Action<ImGuiStylePtr, ImGuiIOPtr> styleFunction, TState state, Vector2 size, Vector2 pos, string title, Vector3 clearColor, WindowState windowState = WindowState.Normal)
    {
        this.uiFunction = uiFunction;
        this.state = state;
        this.clearColor = clearColor;
        // Create window, GraphicsDevice, and all resources necessary for the demo.
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, windowState, title),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out window,
            out graphicsDevice);
        window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
            renderer.WindowResized(window.Width, window.Height);
        };
        commandList = graphicsDevice.ResourceFactory.CreateCommandList();
        renderer = new ImGuiRenderer(graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, new Vector2(window.Width, window.Height), styleFunction);

        var stopwatch = Stopwatch.StartNew();
        float deltaTime = 0f;
        // Main application loop
        while (window.Exists)
        {
            deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            InputSnapshot snapshot = window.PumpEvents();
            if (!window.Exists) { break; }
            renderer.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            this.uiFunction(this.state);

            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
            commandList.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, 1f));
            renderer.Render(graphicsDevice, commandList);
            commandList.End();
            graphicsDevice.SubmitCommands(commandList);
            graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
        }

        // Clean up Veldrid resources
        graphicsDevice.WaitForIdle();
        renderer.Dispose();
        commandList.Dispose();
        graphicsDevice.Dispose();
    }
}