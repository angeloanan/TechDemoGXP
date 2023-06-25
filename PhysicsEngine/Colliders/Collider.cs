using System;
using GXPEngine;

namespace Physics {
  /// <summary>
  /// A superclass for all shapes in our physics engine
  /// </summary>
  public class Collider {
    public readonly GameObject Owner;
    public Vec2 Position;

    protected Collider(GameObject owner, Vec2 position) {
      Owner = owner;
      Position = position;
    }

    /// <summary>
    /// Returns the earliest collision between this collider and another collider. 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="velocity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual CollisionDetail GetCollisionTime(Collider other, Vec2 velocity) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns whether this collider overlaps a given provider
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual bool IsOverlapping(Collider other) {
      throw new NotImplementedException();
    }
  }
}