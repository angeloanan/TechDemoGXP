using Physics;

class Wall : GXPEngine.LineSegment {
  private readonly Collider _collider;
  private Collider _reverseWall;
  private readonly Collider _startPointCollider;
  private readonly Collider _endPointCollider;

  public Wall(Vec2 startPos, Vec2 endPos, uint color = 0xffffffff, uint width = 1U) : base(startPos, endPos, color,
    width) {
    // Create collider, and register it (as trigger!):
    _collider = new LineSegment(this, startPos, endPos);
    ColliderManager.Main.AddSolidCollider(_collider);

    // Create colliders for the start and end points:
    _startPointCollider = new CircleCollider(this, startPos, 0f);
    _endPointCollider = new CircleCollider(this, endPos, 0f);

    // Register the start and end point colliders:
    ColliderManager.Main.AddSolidCollider(_startPointCollider);
    ColliderManager.Main.AddSolidCollider(_endPointCollider);
  }

  void Update() {
    // noop
  }

  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveSolidCollider(_collider);
    ColliderManager.Main.RemoveSolidCollider(_startPointCollider);
    ColliderManager.Main.RemoveSolidCollider(_endPointCollider);
  }
}
