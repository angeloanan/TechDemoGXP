using System;
using GXPEngine;

namespace Physics {
  class LineSegment : Collider {
    public readonly Vec2 StartPosition;
    public readonly Vec2 EndPosition;
    public readonly float Length;
    public readonly Vec2 Normal;

    // Constructor
    public LineSegment(GameObject owner, Vec2 startPosition, Vec2 endPosition) : base(owner, startPosition) {
      this.StartPosition = startPosition;
      this.EndPosition = endPosition;
      this.Length = (endPosition - startPosition).Length();
      this.Normal = (endPosition - startPosition).UnitNormal();
    }
  }
}