using System.Numerics;

public abstract class GameObject
{
    public Vector2 Position;
    public Vector2 Velocity;

    public float ColliderRadius;

    public abstract void Update(double deltaTime);  
}