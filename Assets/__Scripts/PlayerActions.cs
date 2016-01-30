using System;
using InControl;


public class PlayerActions : PlayerActionSet {
	public PlayerAction Green;
	public PlayerAction Red;
	public PlayerAction Blue;
	public PlayerAction Yellow;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerAction RTrigger;
	public PlayerAction LTrigger;
	public PlayerAction Start;
	public PlayerTwoAxisAction Rotate;


	public PlayerActions()
	{
		Green = CreatePlayerAction( "Green" );
		Red = CreatePlayerAction( "Red" );
		Blue = CreatePlayerAction( "Blue" );
		Yellow = CreatePlayerAction( "Yellow" );
		Left = CreatePlayerAction( "Left" );
		Right = CreatePlayerAction( "Right" );
		Up = CreatePlayerAction( "Up" );
		Down = CreatePlayerAction( "Down" );
		RTrigger = CreatePlayerAction( "RTrigger" );
		LTrigger = CreatePlayerAction( "LTrigger" );
		Start = CreatePlayerAction( "Start" );
		Rotate = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
	}


	public static PlayerActions CreateWithKeyboardBindings1()
	{
		var actions = new PlayerActions();

		actions.Green.AddDefaultBinding( Key.O );
		actions.Red.AddDefaultBinding( Key.P );
		actions.Blue.AddDefaultBinding( Key.I );

		actions.Up.AddDefaultBinding( Key.UpArrow );
		actions.Down.AddDefaultBinding( Key.DownArrow );
		actions.Left.AddDefaultBinding( Key.LeftArrow );
		actions.Right.AddDefaultBinding( Key.RightArrow );
		actions.Start.AddDefaultBinding( Key.Return );

		return actions;
	}

	public static PlayerActions CreateWithKeyboardBindings2()
	{
		var actions = new PlayerActions();

		actions.Green.AddDefaultBinding( Key.V );
		actions.Red.AddDefaultBinding( Key.B );
		actions.Blue.AddDefaultBinding( Key.N );

		actions.Up.AddDefaultBinding( Key.W );
		actions.Down.AddDefaultBinding( Key.S );
		actions.Left.AddDefaultBinding( Key.A );
		actions.Right.AddDefaultBinding( Key.D );
		actions.Start.AddDefaultBinding( Key.Return );

		return actions;
	}


	public static PlayerActions CreateWithJoystickBindings()
	{
		var actions = new PlayerActions();

		actions.Green.AddDefaultBinding( InputControlType.Action1 );
		actions.Red.AddDefaultBinding( InputControlType.Action2 );
		actions.Blue.AddDefaultBinding( InputControlType.Action3 );
		actions.Yellow.AddDefaultBinding( InputControlType.Action4 );

		actions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
		actions.Down.AddDefaultBinding( InputControlType.LeftStickDown );
		actions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
		actions.Right.AddDefaultBinding( InputControlType.LeftStickRight );

		actions.Up.AddDefaultBinding( InputControlType.DPadUp );
		actions.Down.AddDefaultBinding( InputControlType.DPadDown );
		actions.Left.AddDefaultBinding( InputControlType.DPadLeft );
		actions.Right.AddDefaultBinding( InputControlType.DPadRight );

		actions.RTrigger.AddDefaultBinding( InputControlType.RightTrigger );
		actions.LTrigger.AddDefaultBinding( InputControlType.LeftTrigger );

		actions.Start.AddDefaultBinding( InputControlType.Command );

		return actions;
	}
}

