using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Console = Unigine.Console;

[Component(PropertyGuid = "2ac76a533f076b96e6532dfd90b0173803187137")]
public class BaseTrigger : Component
{
	private WorldTrigger worldTrigger;
	
	private void Init()
	{
		// write here code to be called on component initialization
		worldTrigger = node as WorldTrigger;
		if (worldTrigger != null)
		{
			worldTrigger.AddEnterCallback(EnterCallback);
		}
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		
	}

	void EnterCallback(Node target)
	{
		Console.WriteLine("Hello");
		
	}
}