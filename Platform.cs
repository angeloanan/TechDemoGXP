using GXPEngine;
using Physics;


class Platform : AnimationSprite {
  private readonly Collider _collider;

  // Constructor
  public Platform(Vec2 position) : base("tilesheet.png", 14, 7) {
    this.SetFrame(28);

    // Create collider, and register it (as solid / collision object):
    // (Note: to get the platform behavior, we use a one sided line segment as collider)
    _collider = new HorizontalLineSegment(this, position, width);
    
    ColliderManager.Main.AddSolidCollider(_collider);

    x = position.X;
    y = position.Y;
  }

  // Remove the collider when the sprite is destroyed:
  protected override void OnDestroy() {
    ColliderManager.Main.RemoveSolidCollider(_collider);
  }
}