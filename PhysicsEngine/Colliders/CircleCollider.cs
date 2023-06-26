using System;
using GXPEngine;

namespace Physics {
  public class CircleCollider : Collider {
    public const float Tolerance = 0.01f;
    public readonly float Radius;

    public CircleCollider(GameObject owner, Vec2 position, float radius = 64) : base(owner, position) {
      this.Radius = radius;
    }

    public override bool IsOverlapping(Collider other) {
      switch (other) {
        case CircleCollider circle:
          return IsOverlapping(circle);
        case AxisAlignedBoundingBox box:
          return IsOverlapping(box);
        default:
          throw new NotImplementedException();
      }
    }

    bool IsOverlapping(CircleCollider other) {
      var distance = (Position - other.Position).Length();
      return distance < Radius + other.Radius;
    }

    bool IsOverlapping(AxisAlignedBoundingBox other) {
      var distance = (Position - other.Position).Length();
      return distance < Radius + other.HalfHeight || distance < Radius + other.HalfWidth;
    }

    public override CollisionDetail GetCollisionTime(Collider other, Vec2 velocity) {
      switch (other) {
        case CircleCollider circle:
          return GetCollisionTime(circle, velocity);
        case AxisAlignedBoundingBox box:
          return GetCollisionTime(box, velocity);
        case HorizontalLineSegment segment:
          return GetCollisionTime(segment, velocity);
        case LineSegment line:
          return GetCollisionTime(line, velocity);
        default:
          throw new NotImplementedException();
      }
    }

    // TODO: REWRITING USING ABC
    private CollisionDetail GetCollisionTime(CircleCollider otherCircle, Vec2 velocity) {
      if (velocity.X == 0 && velocity.Y == 0) return null; // Velocity is zero, no collision
      if (this == otherCircle) return null;

      var v = velocity;
      var u = this.Position - otherCircle.Position;
      
      var a = v.Length() * v.Length();
      var b = Vec2.Dot(2 * u, v);
      var c = (u.Length() * u.Length()) - Math.Pow((this.Radius + otherCircle.Radius), 2);
      
      if (c < 0) {
        if (b < 0) {
          return new CollisionDetail(velocity.Normalized(), otherCircle, 0); // Overlapping
        }
        else {
          return null; // Moving away from each other
        }
      }
      
      var D = (b * b) - (4 * a * c);
      if (D < 0) return null;

      var t = (-b - Math.Sqrt(D)) / (2 * a);
      if (t >= 1) return null;
      return new CollisionDetail(velocity.Normalized(), otherCircle, (float)t);

      // // Calculate the distance between the two circles
      // var distance = (this.Position - otherCircle.Position).Length();
      // var totalRadius = this.Radius + otherCircle.Radius + Tolerance;
      //
      // // Only collide if the circles are overlapping
      // if (distance > totalRadius) return null;
      //
      // // Calculate the time to collision
      // var timeToCollision = (totalRadius - distance + Tolerance) / velocity.Length();
      // return new CollisionDetail(velocity.Normalized(), otherCircle, timeToCollision);
    }

    // TESTED, WORKING PROPERLY
    private CollisionDetail GetCollisionTime(AxisAlignedBoundingBox box, Vec2 velocity) {
      if (velocity.X == 0 && velocity.Y == 0) return null; // Not moving at all

      // Sweep test
      if (this.Position.X + this.Radius + velocity.X < box.Position.X - box.HalfWidth || // Right of box
          this.Position.X - this.Radius + velocity.X > box.Position.X + box.HalfWidth || // Left of box
          this.Position.Y + this.Radius + velocity.Y < box.Position.Y - box.HalfHeight || // Above box
          this.Position.Y - this.Radius + velocity.Y > box.Position.Y + box.HalfHeight) { // Below box
        return null; // Never collide
      } 
      
      // Calculate time of impact
      var normalX = new Vec2(-1);
      
      // Calculate time of impact for x-axis
      var timeOfImpactX = 0f;
      if (velocity.X > 0) {
        timeOfImpactX = (box.Position.X - box.HalfWidth - (this.Position.X + this.Radius)) / velocity.X;
        normalX = new Vec2(-1);
      } else if (velocity.X < 0) {
        timeOfImpactX = (box.Position.X + box.HalfWidth - (this.Position.X - this.Radius)) / velocity.X;
        normalX = new Vec2(1);
      }
      
      // Calculate time of impact for y-axis
      var timeOfImpactY = 0f;
      var normalY = new Vec2(0, 1);
      if (velocity.Y > 0) {
        timeOfImpactY = (box.Position.Y - box.HalfHeight - (this.Position.Y + this.Radius)) / velocity.Y;
        normalY = new Vec2(0, 1);
      } else if (velocity.Y < 0) {
        timeOfImpactY = (box.Position.Y + box.HalfHeight - (this.Position.Y - this.Radius)) / velocity.Y;
        normalY = new Vec2(0, -1);
      }
      
      // Calculate time of impact
      // Console.WriteLine("{0}, {1} | {2}, {3}", timeOfImpactX, timeOfImpactY, normalX, normalY);
      var timeOfImpact = Mathf.Min(timeOfImpactX, timeOfImpactY);
      var finalNormal = timeOfImpactX < timeOfImpactY ? normalY : normalX;
      
      // Already colliding, moving towards deeper collision
      if (timeOfImpact < 0) return new CollisionDetail(finalNormal, box, 0);
    
      return new CollisionDetail(finalNormal, this, timeOfImpact);
    }

    // TESTED, WORKING PROPERLY
    private CollisionDetail GetCollisionTime(HorizontalLineSegment horizontalLine, Vec2 velocity) {
      if (velocity.Y == 0) return null; // Not moving vertically
      if (velocity.Y * horizontalLine.Normal.Y >= 0) return null; // Going away from the line
      
      // Distance between the line and the circle
      var distance = (Position.Y - horizontalLine.Position.Y - this.Radius);
      var timeOfImpact = distance / velocity.Y * -1;
      
      if (timeOfImpact < 0) return new CollisionDetail(horizontalLine.Normal, horizontalLine, 0);
      return new CollisionDetail(horizontalLine.Normal, horizontalLine, timeOfImpact);
    }

    CollisionDetail GetCollisionTime(LineSegment line, Vec2 velocity) {
      var lineVector = line.EndPosition - line.StartPosition;
      var differenceVector = this.Position - line.StartPosition;

      var projectedLength = differenceVector.ProjectLength(lineVector);
      var closestPoint = line.StartPosition + (lineVector.Normalized() * projectedLength);

      var distanceToLine = (closestPoint - this.Position).Length() - this.Radius;

      // DEBUG DRAWING
      // Gizmos.DrawLine(line.StartPosition.X, line.StartPosition.Y, line.StartPosition.X + differenceVector.X,
      //                   line.StartPosition.Y + differenceVector.Y);
      // Gizmos.DrawLine(line.StartPosition.X, line.StartPosition.Y, closestPoint.X, closestPoint.Y, null, 0xffff0000);
      // Gizmos.DrawLine(closestPoint.X, closestPoint.Y, this.Position.X, this.Position.Y);

      var timeOfImpact = distanceToLine / velocity.Length(); // that's the bug
      // Console.WriteLine(distanceToLine);
      
      
      if (projectedLength < 0 || projectedLength > lineVector.Length()) {
        return null; // Outside of line segment
      } else if (distanceToLine > 0) {
        return null; // Not colliding
      } else {
        return new CollisionDetail(lineVector.UnitNormal(), line, timeOfImpact - (float)0.1);
      }
    }
  }
}