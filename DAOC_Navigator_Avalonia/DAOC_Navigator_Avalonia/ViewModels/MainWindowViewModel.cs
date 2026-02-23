/*
 * DAOC Navigator - The free open source DAOC game navigator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <https://www.gnu.org/licenses/>
 *
 */

using Avalonia.Media;
using System;
using ReactiveUI;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Diagnostics;

namespace DAOC_Navigator_Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private float _yaw = 128;
    public float Yaw
    {
        get => _yaw;
        set => this.RaiseAndSetIfChanged(ref _yaw, value);
    }

    private float _pitch = -30;
    public float Pitch
    {
        get => _pitch;
        set => this.RaiseAndSetIfChanged(ref _pitch, value);
    }

    private bool _wireframe = false;
    public bool WireFrame
    {
        get => _wireframe;
        set => this.RaiseAndSetIfChanged(ref _wireframe, value);
    }

    private float _maxYaw = 180;
    public float MaxYaw
    {
        get => _maxYaw;
        set => this.RaiseAndSetIfChanged(ref _maxYaw, value);
    }

    private float _maxPitch = 360;
    public float MaxPitch
    {
        get => _maxPitch;
        set => this.RaiseAndSetIfChanged(ref _maxPitch, value);
    }

    private string _status = "Initial status";
    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    private float _sensitivity = 0.1f;
    public float Sensitivity
    {
        get => _sensitivity;
        set => this.RaiseAndSetIfChanged(ref _sensitivity, value);
    }

    private float _speed = 1.0f;
    public float Speed
    {
        get => _speed;
        set => this.RaiseAndSetIfChanged(ref _speed, value);
    }

    private float _monsterId = 815;
    public float MonsterId
    {
        get => _monsterId;
        set => this.RaiseAndSetIfChanged(ref _monsterId, value);
    }

    private float _cameraX = 50;
    public float CameraX
    {
        get => _cameraX;
        set => this.RaiseAndSetIfChanged(ref _cameraX, value);
    }

    private float _cameraY = -55;
    public float CameraY
    {
        get => _cameraY;
        set => this.RaiseAndSetIfChanged(ref _cameraY, value);
    }

    private float _cameraZ = 82;
    public float CameraZ
    {
        get => _cameraZ;
        set => this.RaiseAndSetIfChanged(ref _cameraZ, value);
    }

    /// <summary>
    /// Command wired to all toolbar / menu buttons. The CommandParameter string
    /// identifies the action: "next", "prev", "folder", "open", "nif", "model".
    /// </summary>
    public ICommand MyActionCommand { get; }

    private MainControlWindowGLRendering? _myControl;

    public MainWindowViewModel()
    {
        MyActionCommand = new RelayCommand<object>(ExecuteMyAction);
    }

    private void ExecuteMyAction(object? parameter)
    {
        if (_myControl == null)
        {
            Debug.WriteLine("MyControl is not initialized");
            return;
        }

        string action = parameter as string ?? string.Empty;
        switch (action)
        {
            case "next":   _myControl.NextMonster();          break;
            case "prev":   _myControl.PrevMonster();          break;
            case "folder": _ = _myControl.defineDAOCFolder(); break;
            case "open":   _myControl.openNPK();              break;
            case "nif":    _myControl.openNIF();              break;
            case "model":  _myControl.openModel();            break;
            default:
                Debug.WriteLine($"Unknown action: {action}");
                break;
        }
    }

    /// <summary>Sets the OpenGL control reference after the view has been created.</summary>
    public void SetMyControl(MainControlWindowGLRendering myControl)
    {
        _myControl = myControl;
    }

    private Color _selectedColor = new Color(255, 51, 51, 51);
    public Color SelectedColor
    {
        get => _selectedColor;
        set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
    }
}