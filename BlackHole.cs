using Physics;

public class BlackHole : GXPEngine.GameObject {
  private readonly CircleCollider _collider;
  private readonly float _force;
  
  public BlackHole(Vec2 center, float radius, float force = 0.1f) {
    _collider = new CircleCollider(this, center, radius);
    _force = force;

    ColliderManager.Main.AddTriggerCollider(_collider);
  }
  
  void Update() {
    var overlappingCollider = ColliderManager.Main.GetOverlaps(_collider);
    // Filter out all non-ball colliders:
    var overlappingBalls = overlappingCollider.FindAll(c => c.Owner is Ball);
    
    foreach (var collider in overlappingBalls) {
      var ball = (Ball)collider.Owner;
      
      // Gravity will be stronger when the ball is closer to the center of the black hole:
      var gravity = (_collider.Position - collider.Position).Normalized() * _force;
      ball.Velocity += gravity;
    }
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveTriggerCollider(_collider);
  }
}