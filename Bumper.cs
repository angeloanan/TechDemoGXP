using Physics;

class Bumper : GXPEngine.LineSegment {
  private Collider _collider;
  
  public Bumper(Vec2 startPos, Vec2 endPos) : base(startPos, endPos) {
    // Create collider, and register it (as trigger!):
    _collider = new LineSegment(this, startPos, endPos);
    ColliderManager.Main.AddSolidCollider(_collider);
  }
  
  void Update() {
    //
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveSolidCollider(_collider);
  }
}