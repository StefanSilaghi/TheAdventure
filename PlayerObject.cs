using System.Formats.Asn1;
using Silk.NET.Maths;
using TheAdventure;

namespace TheAdventure.Models;

public class PlayerObject : RenderableGameObject
{
    public enum PlayerStateDirection{
        None = 0,
        Down,
        Up,
        Left,
        Right,
    }
    public enum PlayerState{
        None = 0,
        Idle,
        Move,
        Attack,
        GameOver
    }

    public bool StatusLife = true;
    private int _pixelsPerSecond = 192;
    private Audio _audio;
    private string audiopath = "Assets/sound.mp3";
    public int Health { get; private set; }


    public (PlayerState State, PlayerStateDirection Direction) State{ get; private set; }

    public PlayerObject(SpriteSheet spriteSheet, int x, int y) : base(spriteSheet, (x, y))
    {
        Health = 100;
        _audio = new Audio(audiopath);
        SetState(PlayerState.Idle, PlayerStateDirection.Down);
    }
    public void RenderHealthBar(GameRenderer renderer)
    {
        int barWidth = 100; // Width of health bar
        int barHeight = 10; // Height of health bar
        int x = 10; // X position of health bar
        int y = 10; // Y position of health bar
        if (StatusLife = false)
        {
            
            renderer.SetDrawColor(255, 0, 0, 255); // Red color for health bar background
            renderer.FillRectangle(new Rectangle<int>(x, y, barWidth, barHeight));
        }
        else
        {
            int healthWidth = (int)(barWidth * (Health / 100.0));
            renderer.SetDrawColor(0, 255, 0, 255); // Green color for current health
            renderer.FillRectangle(new Rectangle<int>(x, y, healthWidth, barHeight));
        }

    }

    public void SetState(PlayerState state, PlayerStateDirection direction)
    {
        if(State.State == PlayerState.GameOver) return;
        if(State.State == state && State.Direction == direction){
            return;
        }
        else if(state == PlayerState.None && direction == PlayerStateDirection.None){
            SpriteSheet.ActivateAnimation(null);
        }
        else if(state == PlayerState.GameOver){
            SpriteSheet.ActivateAnimation(Enum.GetName(state));
        }
        else{
            var animationName = Enum.GetName<PlayerState>(state) + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        State = (state, direction);
    }

    public void GameOver(GameRenderer renderer){
        SetState(PlayerState.GameOver, PlayerStateDirection.None);
        StatusLife = false;
        RenderHealthBar(renderer);
        _audio.PlayGameOverSound();
    }

    public void Attack(bool up, bool down, bool left, bool right)
    {
        if(State.State == PlayerState.GameOver) return;
        var direction = State.Direction;
        if(up){
            direction = PlayerStateDirection.Up;
        }
        else if (down)
        {
            direction = PlayerStateDirection.Down;
        }
        else if (right)
        {
            direction = PlayerStateDirection.Right;
        }
        else if (left){
            direction = PlayerStateDirection.Left;
        }
        SetState(PlayerState.Attack, direction);
    }

    public void UpdatePlayerPosition(double up, double down, double left, double right, int width, int height,
        double time)
    {
        if(State.State == PlayerState.GameOver) return;
        if (up <= double.Epsilon &&
            down <= double.Epsilon &&
            left <= double.Epsilon &&
            right <= double.Epsilon &&
            State.State == PlayerState.Idle){
            return;
        }

        var pixelsToMove = time * _pixelsPerSecond;

        var x = Position.X + (int)(right * pixelsToMove);
        x -= (int)(left * pixelsToMove);

        var y = Position.Y - (int)(up * pixelsToMove);
        y += (int)(down * pixelsToMove);

        if (x < 10)
        {
            x = 10;
        }

        if (y < 24)
        {
            y = 24;
        }

        if (x > width - 10)
        {
            x = width - 10;
        }

        if (y > height - 6)
        {
            y = height - 6;
        }



        if (y < Position.Y){
            SetState(PlayerState.Move, PlayerStateDirection.Up);
        }
        if (y > Position.Y ){
            SetState(PlayerState.Move, PlayerStateDirection.Down);
        }
        if (x > Position.X ){
            SetState(PlayerState.Move, PlayerStateDirection.Right);
        }
        if (x < Position.X){
            SetState(PlayerState.Move, PlayerStateDirection.Left);
        }
        if (x == Position.X &&
            y == Position.Y){
            SetState(PlayerState.Idle, State.Direction);
        }

        Position = (x, y);
    }
}