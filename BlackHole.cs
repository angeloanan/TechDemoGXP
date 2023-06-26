using GXPEngine;
using Physics;

public class BlackHole : EasyDraw {
  private readonly CircleCollider _collider;
  private readonly float _force;
  
  public BlackHole(Vec2 center, float radius, float force = 1F) : base((int)radius * 2, (int) radius * 2) {
    _collider = new CircleCollider(this, center, radius);
    _force = force;

    ColliderManager.Main.AddTriggerCollider(_collider);

    this.x = center.X;
    this.y = center.Y;
    SetOrigin(radius, radius);
    Fill(100, 100, 100);
    Ellipse(radius, radius, radius * 2, radius * 2);
  }
  
  void Update() {
    Gizmos.DrawCross(_collider.Position.X, _collider.Position.Y, 10, null, 0xA0FFFFFF);
    
    var blackHoleOverlaps = ColliderManager.Main.GetOverlaps(_collider);
    // Filter out all non-ball colliders:
    var overlappingBalls = blackHoleOverlaps.FindAll(c => c.Owner is Ball);
    
    foreach (var overlappingCollider in overlappingBalls) {
      var ball = (Ball)overlappingCollider.Owner;
      
      // Gravity will be stronger when the ball is closer to the center of the black hole:
      var gravity = (_collider.Position - overlappingCollider.Position).Normalized() * _force;
      ball.Velocity += gravity;
    }
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveTriggerCollider(_collider);
  }
}