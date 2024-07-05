using UnityEngine;

public enum ControlType
{
    Mouse,
    XboxController,
    PSController,
    OtherController
}

public enum ButtonIcon
{
    MouseLeft,
    MouseRight,
    MouseMiddle,

    TouchPad, // ps4/5 controller
    Minus, // ps4/5 controller

    Home,
    ButtonWest,
    ButtonEast,
    ButtonNorth,
    ButtonSouth,
    Start,
    Select,
    L1,
    L2,
    R1,
    R2,
    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight,
    LeftStick,
    RightStick
}

public static class ControlIconSelector
{
    public static Sprite GetIconForControl(ControlType control, ButtonIcon icon)
    {
        if (control == ControlType.Mouse)
        {
            var basePath = "ControlIcons/Mouse";
            return icon switch
            {
                ButtonIcon.MouseLeft => CachedResources.Load<Sprite>($"{basePath}/mouse_left"),
                ButtonIcon.MouseRight => CachedResources.Load<Sprite>($"{basePath}/mouse_right"),
                ButtonIcon.MouseMiddle => CachedResources.Load<Sprite>($"{basePath}/mouse_middle"),
                _ => null,
            };
        }
        else
        {
            var commonPath = "ControlIcons/Common";
            var basePath = "ControlIcons/";
            switch (control)
            {
                case ControlType.PSController:
                    basePath += "PSController";
                    return icon switch
                    {
                        ButtonIcon.Home => CachedResources.Load<Sprite>($"{basePath}/button_ps"),
                        ButtonIcon.ButtonWest => CachedResources.Load<Sprite>($"{basePath}/button_square"),
                        ButtonIcon.ButtonEast => CachedResources.Load<Sprite>($"{basePath}/button_circle"),
                        ButtonIcon.ButtonNorth => CachedResources.Load<Sprite>($"{basePath}/button_triangle"),
                        ButtonIcon.ButtonSouth => CachedResources.Load<Sprite>($"{basePath}/button_cross"),
                        ButtonIcon.Start => CachedResources.Load<Sprite>($"{basePath}/button_options"),
                        ButtonIcon.Select => CachedResources.Load<Sprite>($"{basePath}/button_share"),
                        ButtonIcon.L1 => CachedResources.Load<Sprite>($"{basePath}/bumper1_l1"),
                        ButtonIcon.L2 => CachedResources.Load<Sprite>($"{basePath}/trigger1_l2"),
                        ButtonIcon.R1 => CachedResources.Load<Sprite>($"{basePath}/bumper1_r1"),
                        ButtonIcon.R2 => CachedResources.Load<Sprite>($"{basePath}/trigger1_r2"),
                        ButtonIcon.DPadUp => CachedResources.Load<Sprite>($"{basePath}/dpad2_up"),
                        ButtonIcon.DPadDown => CachedResources.Load<Sprite>($"{basePath}/dpad2_down"),
                        ButtonIcon.DPadLeft => CachedResources.Load<Sprite>($"{basePath}/dpad2_left"),
                        ButtonIcon.DPadRight => CachedResources.Load<Sprite>($"{basePath}/dpad2_right"),
                        ButtonIcon.TouchPad => CachedResources.Load<Sprite>($"{basePath}/touchpad"),
                        ButtonIcon.Minus => CachedResources.Load<Sprite>($"{basePath}/button_minus"),
                        ButtonIcon.LeftStick => CachedResources.Load<Sprite>($"{commonPath}/joystick1_left"),
                        ButtonIcon.RightStick => CachedResources.Load<Sprite>($"{commonPath}/joystick1_right"),
                        _ => null,
                    };
                case ControlType.XboxController:
                case ControlType.OtherController:
                    basePath += "XboxController";
                    return icon switch
                    {
                        ButtonIcon.Home => CachedResources.Load<Sprite>($"{basePath}/xbox"),
                        ButtonIcon.ButtonWest => CachedResources.Load<Sprite>($"{basePath}/button_x"),
                        ButtonIcon.ButtonEast => CachedResources.Load<Sprite>($"{basePath}/button_b"),
                        ButtonIcon.ButtonNorth => CachedResources.Load<Sprite>($"{basePath}/button_y"),
                        ButtonIcon.ButtonSouth => CachedResources.Load<Sprite>($"{basePath}/button_a"),
                        ButtonIcon.Start => CachedResources.Load<Sprite>($"{basePath}/button_menu"),
                        ButtonIcon.Select => CachedResources.Load<Sprite>($"{basePath}/button_view"),
                        ButtonIcon.L1 => CachedResources.Load<Sprite>($"{basePath}/lb"),
                        ButtonIcon.L2 => CachedResources.Load<Sprite>($"{basePath}/lt"),
                        ButtonIcon.R1 => CachedResources.Load<Sprite>($"{basePath}/rb"),
                        ButtonIcon.R2 => CachedResources.Load<Sprite>($"{basePath}/rt"),
                        ButtonIcon.DPadUp => CachedResources.Load<Sprite>($"{basePath}/dpad_up"),
                        ButtonIcon.DPadDown => CachedResources.Load<Sprite>($"{basePath}/dpad_down"),
                        ButtonIcon.DPadLeft => CachedResources.Load<Sprite>($"{basePath}/dpad_left"),
                        ButtonIcon.DPadRight => CachedResources.Load<Sprite>($"{basePath}/dpad_right"),
                        ButtonIcon.LeftStick => CachedResources.Load<Sprite>($"{commonPath}/joystick1_left"),
                        ButtonIcon.RightStick => CachedResources.Load<Sprite>($"{commonPath}/joystick1_right"),
                        _ => null,
                    };
                default:
                    return null;
            }
        }
    }
}
