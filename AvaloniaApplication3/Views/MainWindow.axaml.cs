using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;

namespace AvaloniaApplication3.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowCaptionButtons.Attach(this);
        
        
        this.GetPropertyChangedObservable(WindowStateProperty).AddClassHandler<Visual>((t, args) =>
        {
            if (OperatingSystem.IsWindows())
            {
                if (args.GetNewValue<WindowState>() == WindowState.Maximized)
                {
                    var screen = Screens.ScreenFromWindow(this);
                    if (screen?.WorkingArea.Height < ClientSize.Height * screen?.Scaling)
                    {
                        ClientSize = screen.WorkingArea.Size.ToSize(screen.Scaling);

                        if (Position.X < 0 || Position.Y < 0)
                        {
                            Position = screen.WorkingArea.Position;
                            WindowHelper.FixAfterMaximizing(TryGetPlatformHandle()!.Handle, screen);
                        }
                    }
                }
            }
        });

    }
    
    private void Grid_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount > 1)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        BeginMoveDrag(e);
    }

}

public static class WindowHelper
{
    [Flags]
    private enum SetWindowPosFlags : uint
    {
        HideWindow = 128,
        ShowWindow = 64
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
        uint uFlags);

    public static void FixAfterMaximizing(IntPtr hWnd, Screen screen)
    {
        SetWindowPos(hWnd, IntPtr.Zero, screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width,
            screen.WorkingArea.Height, (uint) SetWindowPosFlags.ShowWindow);
    }
}