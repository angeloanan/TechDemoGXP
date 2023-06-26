using System.Collections.Generic;

namespace Physics {
  // This class holds our own colliders - similar to the GXPEngine's CollisionManager, but
  //  we can put any shape here.
  // Using MoveUntilCollision, you can move colliders (even those that aren't registered in this class!),
  //  while checking against collisions with the registered colliders. 
  // Using GetOverlaps you can get all overlapping trigger colliders that are registered.

  class ColliderManager {
    private static ColliderManager _main;

    private readonly List<Collider> _solidColliders;
    private readonly List<Collider> _triggerColliders;

    // Constructor
    private ColliderManager() {
      _solidColliders = new List<Collider>();
      _triggerColliders = new List<Collider>();
    }

    // Singleton
    public static ColliderManager Main =>
      _main ?? (_main = new ColliderManager());

    // Register / unregister colliders
    public void AddSolidCollider(Collider col) {
      _solidColliders.Add(col);
    }

    public void RemoveSolidCollider(Collider col) {
      _solidColliders.Remove(col);
    }

    public void AddTriggerCollider(Collider col) {
      _triggerColliders.Add(col);
    }

    public void RemoveTriggerCollider(Collider col) {
      _triggerColliders.Remove(col);
    }
    
    public int TriggerColliderCount() {
      return _triggerColliders.Count;
    }
    
    public int SolidColliderCount() {
      return _solidColliders.Count;
    }
    
    /// <summary>
    ///	Moves the given collider while checking for collisions with all solid colliders.
    /// If a collision is detected, the collider will be moved to the point of collision.
    /// <br/>
    /// Also known as just `move()` in other engines.
    /// </summary>
    /// <param name="collider">The collider to be moved; Doesn't need to be in SolidCollider / TriggerCollider</param>
    /// <param name="velocity">Velocity (pixel) of how much collider should move</param>
    /// <param name="maxTime">The amount of time (in frames) to move</param>
    /// <returns>The detail of collision OR null</returns>
    public CollisionDetail MoveUntilCollision(Collider collider, Vec2 velocity, float maxTime = 1) {
      CollisionDetail earliestCollision = null;

      // Loop over all solid colliders, and find the earliest collision on current frame
      foreach (var other in _solidColliders) {
        if (other == collider) continue; // Don't check against self

        var collision = collider.GetCollisionTime(other, velocity);
        if (collision == null) continue; // No collision
        if (collision.TimeOfImpact > maxTime) continue; // Collision is not processed in this call

        if (earliestCollision == null || earliestCollision.TimeOfImpact > collision.TimeOfImpact) {
          earliestCollision = collision;
        }
      }

      // MOVING the collider
      if (earliestCollision == null) {
        // No collision, move the collider normally
        collider.Position += velocity;
      }
      else {
        // Move the collider until the collision
        collider.Position += velocity * earliestCollision.TimeOfImpact;
      }

      return earliestCollision;
    }

    /// <summary>
    /// Returns all trigger colliders that overlaps with the given collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public List<Collider> GetOverlaps(Collider collider) {
      var overlaps = new List<Collider>();

      foreach (var other in _triggerColliders) {
        if (other != collider && collider.IsOverlapping(other)) {
          overlaps.Add(other);
        }
      }

      return overlaps;
    }
  }
}