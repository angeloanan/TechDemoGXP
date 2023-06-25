using System;
using GXPEngine;
using Physics;

class Player : AnimationSprite {
  const float Speed = 3;
  const float ColliderWidth = 0.7f;
  const float ColliderHeight = 0.7f;
  const float Gravity = 10f;
  const float MaxFallSpeed = 10f;
  
  private readonly Collider _collider;
  private Vec2 _acceleration;
  private Vec2 _velocity;
  private bool _isJumpPressed;

  // Constructor
  public Player(Vec2 startPosition) : base("character.png", 4, 2) {
    var cWidthPixels = width * ColliderWidth;
    var cHeightPixels = height * ColliderHeight;
    SetOrigin(width / 2, height - (cHeightPixels / 2));

    // Create collider, and register it (as trigger!):
    _collider = new AxisAlignedBoundingBox(this, startPosition, cWidthPixels / 2, cHeightPixels / 2);
    ColliderManager.Main.AddTriggerCollider(_collider);
    this._acceleration = new Vec2();
    this._velocity = new Vec2();
  }

  private void Move() {
    if (Input.GetKey(Key.A)) {
      this._acceleration.X = -Speed;
    } else if (Input.GetKey(Key.E)) {
      this._acceleration.X = Speed;
    } else {
      this._acceleration.X = 0;
    }

    if (!_isJumpPressed && Input.GetKey(Key.COMMA)) {
      _isJumpPressed = true;
      this._acceleration.Y = -Speed * 2F;
    } else if (_isJumpPressed && Input.GetKeyUp(Key.COMMA)) {
      _isJumpPressed = false;
    } else if (_isJumpPressed) {
      this._acceleration.Y = 0;
    }

    // Calculate velocity and apply gravity
    this._velocity += this._acceleration + (new Vec2(0, Gravity) * 1 / 60);
   
    // Limit fall speed
    if (this._velocity.Y > MaxFallSpeed) {
      this._velocity.Y = MaxFallSpeed;
    }
    
    // Let the physics engine / collider manager move the collider:
    var colliderManager = ColliderManager.Main;
    colliderManager.MoveUntilCollision(_collider, new Vec2(this._velocity.X));

    var floorCollision = colliderManager.MoveUntilCollision(_collider, new Vec2(0, this._velocity.Y));
    if (floorCollision != null) {
      this._velocity.Y = 0;
      this._acceleration.Y = 0;
    }

    // Don't forget to update the sprite position too!
    x = _collider.Position.X;
    y = _collider.Position.Y;
    
    // Clamp velocity
    if (Math.Abs(this._velocity.X) < 0.001) {
      this._velocity.X = 0;
    } else {
      // Apply friction properly
      this._velocity.X *= 0.9F;
    }


    // Console.WriteLine("A {1}, V {0}", _velocity, _acceleration);

    // This is just to determine shoot direction:
    // if (vx != 0 || vy != 0) {
    //   _lastVx = vx;
    //   _lastVy = vy;
    // }
  }

  void Shoot() {
    if (Input.GetMouseButtonDown(0)) {
      // Get Angle of mouse relative to player
      var mousePos = new Vec2(Input.mouseX, Input.mouseY);
      var angle = (mousePos - _collider.Position + _velocity).UnitTangent().RotatedDegrees(180);
      
      // Also contribute player's velocity to the bullet
      var additionalVelocity = _velocity * 0.5F;
      
      var bulletVelocity = angle * 15 + additionalVelocity;
      
      parent.AddChild(new Ball(_collider.Position, bulletVelocity));
    }
  }

  void PredictAim() {
    var mousePos = new Vec2(Input.mouseX, Input.mouseY);
    var angle = (mousePos - _collider.Position + _velocity).UnitTangent().RotatedDegrees(180);
      
    // Also contribute player's velocity to the bullet
    var additionalVelocity = _velocity * 0.5F;
    var bulletVelocity = angle * 15 + additionalVelocity;
    
    // Simulate bullet
    var bulletPos = _collider.Position;
    
    // Draw the bullet's path
    for (int i = 0; i < 60; i++) {
      bulletVelocity += new Vec2(0, 10) / 60;
      bulletPos += bulletVelocity ;
      
      if (i % 2 == 0) {
        Gizmos.DrawCross(bulletPos.X, bulletPos.Y, 2F, null, 0xff0000 + 0x88000000);
      }
    }
  }

  // Implementing true aiming where the mouse is the target
  // https://en.wikipedia.org/wiki/Equations_of_motion#Kinematic_equations_for_one_particle
  void PredictTrueAim() {
    var playerPos = _collider.Position;
    var mousePos = new Vec2(Input.mouseX, Input.mouseY);
    var arcHighest = mousePos.Y - playerPos.Y;

    var displacement = playerPos - mousePos;
    var timeHighest = Math.Sqrt(-2 * arcHighest / 10);
    var timeLowest = Math.Sqrt(2 * (displacement.Y - arcHighest) / 10);

    var vX = displacement.X / (float)(timeHighest + timeLowest);
    var vY = (float) Math.Sqrt(-2 * 10 * arcHighest);
    
    // Simulate bullet
    var bulletVelocity = new Vec2(vX, -vY);
    var bulletPos = _collider.Position;
    
    // Draw the bullet's path
    for (int i = 0; i < 60; i++) {
      bulletVelocity += new Vec2(0, 10) / 60;
      bulletPos += bulletVelocity;
      
      if (i % 2 == 0) {
        Gizmos.DrawCross(bulletPos.X, bulletPos.Y, 2F, null, 0xffffff + 0xff000000);
      }
    }
  }
  
  void Update() {
    Move();
    PredictAim();
    // PredictTrueAim();
    Shoot();
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveTriggerCollider(_collider);
  }
}