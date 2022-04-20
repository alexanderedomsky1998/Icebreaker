using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "52c97c60dd98843f2c05c492f74e1ac987b36594")]
public class ShipController : Component
{

	//public properties
	public float AirDrag = 1;
	public float WaterDrag = 10;
	public bool AffectDirection = true;
	// public boTransol AttachToSurface = false;
	// public [] FloatPoints;



	//used components
	protected BodyRigid BodyRigid;


	//water line
	protected float WaterLine;
	protected vec3[] WaterLinePoints;


	//help Vectors
	protected vec3 centerOffset;

	public vec3 Center { get { return  + centerOffset;  } }


	private void Init()
	{
		// write here code to be called on component initialization
		
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		
	}
}