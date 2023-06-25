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
  
  // Code below are only used for Line Segment <> Other Collider collision detection

  //   public override CollisionDetail GetCollisionTime(Collider other, Vec2 velocity) {
  //     switch (other) {
  //       case AxisAlignedBoundingBox box:
  //         return GetEarliestCollision(box, velocity);
  //       case HorizontalLineSegment segment:
  //         return GetCollisionTime(segment, velocity);
  //
  //       default:
  //         throw new NotImplementedException();
  //     }
  //   }
  //   
  //   CollisionDetail GetEarliestCollision(AxisAlignedBoundingBox box, Vec2 velocity) {
  //     if (velocity.X == 0 && velocity.Y == 0) {
  //       return null;
  //     }
  //
  //     var differenceVector = new Vec2(box.Position.X - this.Position.X, box.Position.Y - this.Position.Y);
  //     var dotProduct = Vec2.Dot(differenceVector, this.Normal);
  //     
  //     var timeToCollision = dotProduct / Vec2.Dot(velocity, this.Normal);
  //     Console.WriteLine(timeToCollision);
  //
  //     return new CollisionDetail(velocity.Normalized(), this, timeToCollision);
  //   }
  }
}