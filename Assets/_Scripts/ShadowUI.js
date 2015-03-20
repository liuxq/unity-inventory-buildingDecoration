var lights : Light[];

function OnGUI()
{
	GUILayout.BeginArea( Rect( 2, Screen.height-130, 150, 128 ), "Settings", GUI.skin.window  );
	for( var l in lights )
	{
		l.enabled = GUILayout.Toggle( l.enabled, l.name );
	}
	GUILayout.EndArea();
}
