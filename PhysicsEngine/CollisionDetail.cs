namespace Physics {
  /// <summary>
  /// A struct that holds information about a potential collision.
  /// </summary>
  public class CollisionDetail {
    /// <summary>
    /// The normal of the collision.
    /// </summary>
    public readonly Vec2 Normal;

    /// <summary>
    /// The collider that was hit.
    /// </summary>
    public readonly Collider Other;

    /// <summary>
    /// The time of impact (in frames) of the collision.
    ///	<br />
    ///	<br />
    /// If the time of impact is 0, the collision is happening right now.
    /// <br />
    /// If the time of impact is 1, the collision will happen next frame. 
    /// </summary>
    public readonly float TimeOfImpact;

    public CollisionDetail(Vec2 normal, Collider other, float timeOfImpact) {
      this.Normal = normal;
      this.Other = other;
      this.TimeOfImpact = timeOfImpact;
    }
  }
}