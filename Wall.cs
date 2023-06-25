using Physics;

class Wall : GXPEngine.LineSegment {
  private Collider _collider;
  
  public Wall(Vec2 startPos, Vec2 endPos, uint color = 0xffffffff, uint width = 1U) : base(startPos, endPos, color, width) {
    // Create collider, and register it (as trigger!):
    _collider = new LineSegment(this, startPos, endPos);
    ColliderManager.Main.AddSolidCollider(_collider);
  }
  
  void Update() {
    //
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.AddSolidCollider(_collider);
  }
}