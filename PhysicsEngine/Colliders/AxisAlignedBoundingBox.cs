using System;
using System.Drawing;
using GXPEngine;

namespace Physics {
  class AxisAlignedBoundingBox : Collider {
    public readonly float HalfWidth;
    public readonly float HalfHeight;

    // Error range for floating point rounding errors
    private const float Epsilon = 0.001f;

    public AxisAlignedBoundingBox(GameObject owner, Vec2 startPosition, float pHalfWidth, float pHalfHeight) : base(
      owner, startPosition) {
      HalfWidth = pHalfWidth;
      HalfHeight = pHalfHeight;
    }

    public override CollisionDetail GetCollisionTime(Collider other, Vec2 velocity) {
      switch (other) {
        case AxisAlignedBoundingBox box:
          return GetEarliestCollision(box, velocity);
        case HorizontalLineSegment segment:
          return GetEarliestCollision(segment, velocity);
        case LineSegment line:
          return GetEarliestCollision(line, velocity);
        case CircleCollider circle:
          return GetEarliestCollision(circle, velocity);

        default:
          throw new NotImplementedException();
      }
    }


    // TESTED, WORKING PROPERLY
    private CollisionDetail GetEarliestCollision(AxisAlignedBoundingBox box, Vec2 velocity) {
      if (velocity.X == 0 && velocity.Y == 0) {
        return null; // Not moving at all
      }
      if (this == box) {
        return null; // Never collide with self
      }

      // Sweep test
      if (this.Position.X + HalfWidth + velocity.X < box.Position.X - box.HalfWidth || // Right of box
          this.Position.X - HalfWidth + velocity.X > box.Position.X + box.HalfWidth || // Left of box
          this.Position.Y + HalfHeight + velocity.Y < box.Position.Y - box.HalfHeight || // Above box
          this.Position.Y - HalfHeight + velocity.Y > box.Position.Y + box.HalfHeight) { // Below box
        return null; // Never collide
      } 
      
      // Calculate time of impact
      var normalX = new Vec2(-1);
      
      // Calculate time of impact for x-axis
      var timeOfImpactX = 0f;
      if (velocity.X > 0) {
        timeOfImpactX = (box.Position.X - box.HalfWidth - (this.Position.X + HalfWidth)) / velocity.X;
        normalX = new Vec2(-1);
      } else if (velocity.X < 0) {
        timeOfImpactX = (box.Position.X + box.HalfWidth - (this.Position.X - HalfWidth)) / velocity.X;
        normalX = new Vec2(1);
      }
      
      // Calculate time of impact for y-axis
      var timeOfImpactY = 0f;
      var normalY = new Vec2(0, 1);
      if (velocity.Y > 0) {
        timeOfImpactY = (box.Position.Y - box.HalfHeight - (this.Position.Y + HalfHeight)) / velocity.Y;
        normalY = new Vec2(0, 1);
      } else if (velocity.Y < 0) {
        timeOfImpactY = (box.Position.Y + box.HalfHeight - (this.Position.Y - HalfHeight)) / velocity.Y;
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
    private CollisionDetail GetEarliestCollision(HorizontalLineSegment horizontalLine, Vec2 velocity) {
      if (velocity.Y == 0) return null; // Not moving vertically
      if (velocity.Y * horizontalLine.Normal.Y >= 0) return null; // Going away
      
      var distanceToLine = horizontalLine.Position.Y - this.Position.Y - this.HalfHeight;
      var timeOfImpact = distanceToLine / velocity.Y;
      
      // Already colliding, moving towards deeper collision
      if (timeOfImpact < 0) return new CollisionDetail(horizontalLine.Normal, horizontalLine, 0);
      
      return new CollisionDetail(horizontalLine.Normal, this, timeOfImpact);
    }

    // NEED TO FIX NORMAL, STILL STICKING TO WALLS BECAUSE OF AABB EXTRENUMS
    private CollisionDetail GetEarliestCollision(LineSegment line, Vec2 velocity) {
      var lineVector = line.EndPosition - line.StartPosition;
      var differenceVector = this.Position - line.StartPosition;

      var projectedLength = differenceVector.ProjectLength(lineVector);
      var closestPoint = line.StartPosition + (lineVector.Normalized() * projectedLength);

      var distanceToLine = (closestPoint - this.Position).Length() - this.HalfWidth;

      // DEBUG DRAWING
      // Gizmos.DrawLine(line.StartPosition.X, line.StartPosition.Y, line.StartPosition.X + differenceVector.X,
      //                   line.StartPosition.Y + differenceVector.Y);
      // Gizmos.DrawLine(line.StartPosition.X, line.StartPosition.Y, closestPoint.X, closestPoint.Y);
      // Gizmos.DrawLine(closestPoint.X, closestPoint.Y, this.Position.X, this.Position.Y);

      var timeOfImpact = distanceToLine / velocity.Length();
      // Console.WriteLine(distanceToLine);
      
      if (projectedLength < 0 || projectedLength > lineVector.Length()) {
        return null; // Outside of line segment
      } else if (distanceToLine > 0) {
        return null; // Not colliding
      } else {
        Console.WriteLine("Bounce!");
        return new CollisionDetail(lineVector.UnitNormal(), line, timeOfImpact - (float)0.1);
      }
    }

    private CollisionDetail GetEarliestCollision(CircleCollider circle, Vec2 velocity) {
      if (velocity.X == 0 && velocity.Y == 0) {
        return null; // Not moving at all
      }
      
      // Sweep test
      if (this.Position.X + HalfWidth + velocity.X < circle.Position.X - circle.Radius || // Right of circle
          this.Position.X - HalfWidth + velocity.X > circle.Position.X + circle.Radius || // Left of circle
          this.Position.Y + HalfHeight + velocity.Y < circle.Position.Y - circle.Radius || // Above circle
          this.Position.Y - HalfHeight + velocity.Y > circle.Position.Y + circle.Radius) { // Below circle
        return null; // Never collide
      } 
      
      // Calculate time of impact
      var normalX = new Vec2(-1);
      
      // Calculate time of impact for x-axis
      var timeOfImpactX = 0f;
      if (velocity.X > 0) {
        timeOfImpactX = (circle.Position.X - circle.Radius - (this.Position.X + HalfWidth)) / velocity.X;
        normalX = new Vec2(-1);
      } else if (velocity.X < 0) {
        timeOfImpactX = (circle.Position.X + circle.Radius - (this.Position.X - HalfWidth)) / velocity.X;
        normalX = new Vec2(1);
      }
      
      // Calculate time of impact for y-axis
      var timeOfImpactY = 0f;
      var normalY = new Vec2(0, 1);
      if (velocity.Y > 0) {
        timeOfImpactY = (circle.Position.Y - circle.Radius - (this.Position.Y + HalfHeight)) / velocity.Y;
        normalY = new Vec2(0, 1);
      } else if (velocity.Y < 0) {
        timeOfImpactY = (circle.Position.Y + circle.Radius - (this.Position.Y - HalfHeight)) / velocity.Y;
        normalY = new Vec2(0, -1);
      }
      
      var timeOfImpact = Mathf.Min(timeOfImpactX, timeOfImpactY);
      
      // Already colliding, moving towards deeper collision
      if (timeOfImpact < 0) return new CollisionDetail(normalY, circle, 0);
      
      return new CollisionDetail(normalY, this, timeOfImpact);
    }
    
    public override bool IsOverlapping(Collider other) {
      switch (other) {
        case AxisAlignedBoundingBox box:
          return IsOverlapping(box);
        case HorizontalLineSegment segment:
          return IsOverlapping(segment);
        case LineSegment line:
          return IsOverlapping(line);
        case CircleCollider circle:
          return IsOverlapping(circle);
        default:
          throw new NotImplementedException();
      }
    }
    
    // TESTED, WORKING PROPERLY
    private bool IsOverlapping(AxisAlignedBoundingBox box) {
      return
        box.Position.X - box.HalfWidth < Position.X + HalfWidth &&
        box.Position.X + box.HalfWidth > Position.X - HalfWidth &&
        box.Position.Y - box.HalfHeight < Position.Y + HalfHeight &&
        box.Position.Y + box.HalfHeight > Position.Y - HalfHeight;
    }

    // TESTED, WORKING PROPERLY
    private bool IsOverlapping(HorizontalLineSegment line) {
      return
        line.Position.Y < Position.Y + HalfHeight &&
        line.Position.Y > Position.Y - HalfHeight &&
        line.RightPoint.X > Position.X - HalfWidth &&
        line.LeftPoint.X < Position.X + HalfWidth;
    }
    
    private bool IsOverlapping(LineSegment line) {
      var lineVector = line.EndPosition - line.StartPosition;
      var differenceVector = this.Position - line.StartPosition;

      var projectedLength = differenceVector.ProjectLength(lineVector);
      var closestPoint = line.StartPosition + (lineVector.Normalized() * projectedLength);

      var distanceToLine = (closestPoint - this.Position).Length() - this.HalfWidth;

      return projectedLength >= 0 && projectedLength <= lineVector.Length() && distanceToLine <= 0;
    }

    private bool IsOverlapping(CircleCollider circle) {
      return circle.Position.X - circle.Radius < Position.X + HalfWidth &&
             circle.Position.X + circle.Radius > Position.X - HalfWidth &&
             circle.Position.Y - circle.Radius < Position.Y + HalfHeight &&
             circle.Position.Y + circle.Radius > Position.Y - HalfHeight;
    }
  }
}