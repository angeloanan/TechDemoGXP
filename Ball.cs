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

    _ballCollider = new CircleCollider(this, startPosition, width / 2);
    _engine = ColliderManager.Main;
    _engine.AddSolidCollider(_ballCollider);
    _engine.AddTriggerCollider(_ballCollider);

    Velocity = initialVelocity;
  }

  void Move() {
    // Gravity
    Velocity += new Vec2(0, 10) / 60;

    var shouldStillMove = true;
    var targetTime = 1F;
    // Repetitively move the ball THIS frame
    while (shouldStillMove || targetTime < 0.001F) {
      // TODO: Refactor MoveUntilCollision to use out variables
      // TODO: Use recursion instead of a while loop
      var collision = _engine.MoveUntilCollision(_ballCollider, Velocity, targetTime);

      if (collision != null) {
        Console.WriteLine(
          $"Ball collided with {collision.Other.Owner} at {collision.Other.Position} with normal {collision.Normal}");

        switch (collision.Other.Owner) {
          case Ball _:
            // Special case - Ball Ball collision
            HandleBallBallCollision(collision); 
            break;

          default: {
            // Reflect velocity, considering the normal of the collision:
            Velocity = Velocity.Reflect(collision.Normal, collision.Other.Owner is Platform ? 0.75f : 0.98f);
            Gizmos.DrawArrow(this.x, this.y, collision.Normal.X * 50, collision.Normal.Y * 50);
            Gizmos.DrawArrow(this.x, this.y, Velocity.X, Velocity.Y, 0.25F, null, 0xffff0000, 2);
            break;
          }
        }

        _bounces += 1;
        targetTime -= collision.TimeOfImpact;
      }
      else {
        shouldStillMove = false;
      }

      x = _ballCollider.Position.X;
      y = _ballCollider.Position.Y;

      if (_bounces >= MaxBounces) {
        Console.WriteLine("Removing projectile");
        LateDestroy();
        break;
      }
    }
  }

  private void HandleBallBallCollision(CollisionDetail collision) {
    var otherBall = (Ball)collision.Other.Owner;

    var totalVelocity = Velocity.Length() + otherBall.Velocity.Length();
    var eachBallVelocity = totalVelocity / 2; // CHEAT: we assume both balls have the same mass
    eachBallVelocity *= 0.98F;

    var collisionNormal = collision.Normal;

    Gizmos.DrawLine(_ballCollider.Position.X, _ballCollider.Position.Y,
      _ballCollider.Position.X + collisionNormal.X, _ballCollider.Position.Y + collisionNormal.Y,
      null, 0xffff0000);

    Velocity =
      collisionNormal.Normalized() * eachBallVelocity;
    otherBall.Velocity = collisionNormal.Normalized().RotatedDegrees(180) * eachBallVelocity;
  }

  public void scrambleDirection() {
    var randomAngle = new Random().Next(0, 360);

    Velocity.SetAngleDegrees(randomAngle);
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