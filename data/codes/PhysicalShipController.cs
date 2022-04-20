using Unigine;

[Component(PropertyGuid = "ab6d46a3610b7ba6c07e5ecd6ea471c87ca66a87")]
public class PhysicalShipController : Component
{
	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move forward key")]
	private Input.KEY forwardKey = Input.KEY.W;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move backward key")]
	private Input.KEY backwardKey = Input.KEY.S;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move right key")]
	private Input.KEY rightKey = Input.KEY.D;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move left key")]
	private Input.KEY leftKey = Input.KEY.A;
    
	BodyRigid rigid;

	private vec2 horizontalMoveDirection;
	private float turnSpeed;
	
	private static readonly vec2 Forward = new vec2(0, 1);
	private static readonly vec2 Right = new vec2(1, 0);
	
	private void Init()
	{
		// write here code to be called on component initialization
		rigid = node.ObjectBodyRigid;
		rigid.AngularScale = new vec3(0.0f, 0.0f, 0.0f); // restricting the rotation
		rigid.LinearScale = new vec3(1.0f, 1.0f, 0.0f); // restricting Z movement
		rigid.MaxLinearVelocity = 8.0f; // clamping the max linear velocity
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		UpdateMovement();
		var impulse = new vec3(1f, 1f, 1f) * 1000000f;
		rigid.AddLinearImpulse(impulse);
		Console.WriteLine(impulse);
		node.Rotate(0.0f, 0.0f, turnSpeed * Game.IFps);
	}

	private void UpdateMovement()
	{
		// update horizontal direction
		if (Input.IsKeyPressed(forwardKey))
			horizontalMoveDirection += Forward;

		if (Input.IsKeyPressed(backwardKey))
			horizontalMoveDirection -= Forward;

		if (Input.IsKeyPressed(rightKey))
			turnSpeed -= 1;

		if (Input.IsKeyPressed(leftKey))
			turnSpeed += 1;
	}
}