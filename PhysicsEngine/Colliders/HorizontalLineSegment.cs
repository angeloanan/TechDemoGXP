using GXPEngine;

namespace Physics {
  // Example collider:
  // A simple horizontal, one-way collision line segment collider.
  class HorizontalLineSegment : Collider {
    public readonly float Length;
    public readonly Vec2 Normal;

    // Create a line segment between position and position + (pDeltaX,0).
    // Convention: the normal points up if and only if pDeltaX is positive.
    // Constructor
    public HorizontalLineSegment(GameObject owner, Vec2 position, float length) : base(owner, position) {
      this.Length = length;
      this.Normal = new Vec2(0, -Mathf.Sign(Length));
    }

    public Vec2 LeftPoint =>
      Length < 0
        ? Position - new Vec2(Length)
        : Position;

    public Vec2 RightPoint =>
      Length < 0
        ? Position
        : Position + new Vec2(Length);

    // TODO: If you also want to call GetOverlaps and GetEarliestCollision for line segments (moving line segments?),
    // Implement those methods here!
  }
}