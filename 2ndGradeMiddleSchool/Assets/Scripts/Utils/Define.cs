public enum UIEvent
{
    Click,
    Drag,
}

public enum MouseEvent
{
    Press,
    Click,
}
public enum Sound
{
    Bgm,
    Effect,
    MaxCount
}
public enum PlayerMoveDirection
{
    Up,
    Down,
    Right,
    Left,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft,
    None
}
public enum PlayerLookingDirection
{
    Right,
    Left,
    None
}
public enum PlayerState
{
    None,
    Move,
    Jump,
    Dash,
    Attack
}