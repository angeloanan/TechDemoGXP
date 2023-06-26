using System;
using System.Collections.Generic;
using GXPEngine;
using Physics;

class Ball : AnimationSprite {
  private const int MaxBounces = 10;

  private readonly CircleCollider _ballCollider;
  public Vec2 Velocity;
  private int _bounces;

  private readonly ColliderManager _engine;

  // Constructor
  public Ball(Vec2 startPosition, Vec2 initialVelocity) : base("tilesheet.png", 14, 7) {
    SetOrigin(width / 2, height / 2);
    SetFrame(25);
    scale = 0.5f;

    // Create collider, and register it (as trigger!):
    // (Note: in the current code, this collider is not used, so it could be omitted.)
    _ballCollider = new CircleCollider(this, startPosition, width / 2);
    _engine = ColliderManager.Main;
    _engine.AddSolidCollider(_ballCollider);
    _engine.AddTriggerCollider(_ballCollider);

    Velocity = initialVelocity;
  }

  void Move() {
    // Gravity
    Velocity += new Vec2(0, 10) / 60;

    CollisionDetail collision = _engine.MoveUntilCollision(_ballCollider, Velocity);
    if (collision != null) {
      Console.WriteLine("Ball collided with " + collision.Other.Owner + " at " + collision.Other.Position +
                        " with normal " + collision.Normal + "");
      if (collision.Other.Owner is Ball) {
        // Special case - Ball Ball collision
        var otherBall = (Ball)collision.Other.Owner;

        var totalVelocity = Velocity.Length() + otherBall.Velocity.Length();
        var eachBallVelocity = totalVelocity / 2;

        var collisionNormal = (collision.Other.Position - this._ballCollider.Position).Perpendicular().RotatedDegrees(90);

        Gizmos.DrawLine(_ballCollider.Position.X, _ballCollider.Position.Y,
          _ballCollider.Position.X + collisionNormal.X, _ballCollider.Position.Y + collisionNormal.Y, 
          null, 0xffff0000);
        this.Velocity = collisionNormal.Normalized() * eachBallVelocity; // bug? need to change both velocities at the same time
        _bounces += 1;
      }
      else {
        // Reflect velocity, considering the normal of the collision:
        Velocity = Velocity.Reflect(collision.Normal, collision.Other.Owner is Platform ? 0.75f : 0.98f);
        _ballCollider.Position += Velocity * (1 - collision.TimeOfImpact);
        Gizmos.DrawArrow(this.x, this.y, collision.Normal.X * 10, collision.Normal.Y * 10);
        Gizmos.DrawArrow(this.x, this.y, Velocity.X, Velocity.Y, 0.25F, null, 0xffff0000, 0);
        _bounces += 1;
      }
    }

    // Console.WriteLine("Ball velocity" + _velocity);

    x = _ballCollider.Position.X;
    y = _ballCollider.Position.Y;

    if (_bounces >= MaxBounces || x < 0 || x > game.width || y < 0 || y > game.height) {
      LateDestroy();
      Console.WriteLine("Removing projectile");
    }
  }

  public void scrambleDirection() {
    var randomAngle = new Random().Next(0, 360);

    Velocity = Velocity.RotatedDegrees(randomAngle);
  }

  void Update() {
    Move();
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    _engine.RemoveSolidCollider(_ballCollider);
    _engine.RemoveTriggerCollider(_ballCollider);
  }
}