using System;

public struct Vec2 {
  public float X;
  public float Y;

  // Constructor
  public Vec2(float x = 0, float y = 0) {
    X = x;
    Y = y;
  }

  /// <summary>
  ///	Calculates the current length / magnitude of vector 
  /// </summary>
  /// <returns>Length of Vector</returns>
  public float Length() {
    return (float)Math.Sqrt(X * X + Y * Y);
  }

  /// <summary>
  /// Calculates the normal / squared length of the vector.
  /// <br />
  /// (Rescales the vector to a length of 1, not to be confused with <see cref="UnitNormal"/>)
  /// </summary>
  /// <returns></returns>
  public Vec2 Normalized() {
    var vectorLength = this.Length();

    return !(vectorLength > 0)
      ? new Vec2()
      : new Vec2(X / vectorLength, Y / vectorLength);
  }

  /// <summary>
  /// Normalizes the current vector
  /// </summary>
  public void Normalize() {
    this = this.Normalized();
  }

  /// <summary>
  /// Simulate a vector rotation by the given angle
  /// </summary>
  /// <param name="angleRadians">Angle in radians</param>
  /// <returns>Rotated vector</returns>
  public Vec2 Rotated(float angleRadians) {
    var rotatedX = (float)(Math.Cos(angleRadians) * this.X - Math.Sin(angleRadians) * this.Y);
    var rotatedY = (float)(Math.Sin(angleRadians) * this.X + Math.Cos(angleRadians) * this.Y);

    return new Vec2(rotatedX, rotatedY);
  }

  /// <summary>
  /// Same with <see cref="Rotated" /> but takes in degrees instead of radians.
  /// </summary>
  /// <param name="angleDegrees">Angle in degrees to be rotated</param>
  /// <returns></returns>
  public Vec2 RotatedDegrees(float angleDegrees) {
    return this.Rotated((float)(angleDegrees * Math.PI / 180));
  }

  /// <summary>
  /// Rotates the current vector by the given angle.
  /// </summary>
  /// <param name="angleRadians">Angle in radians</param>
  public void Rotate(float angleRadians) {
    var rotated = this.Rotated(angleRadians);

    this.X = rotated.X;
    this.Y = rotated.Y;
  }

  /// <summary>
  /// Same with <see cref="Rotate" /> but takes in degrees instead of radians.
  /// </summary>
  /// <param name="angleDegrees"></param>
  public void SetAngleDegrees(float angleDegrees) {
    this.SetAngle((float)(angleDegrees * Math.PI / 180));
  }
  
  /// <summary>
  /// Sets the angle of the vector.
  /// </summary>
  /// <param name="angleRadians"></param>
  public void SetAngle(float angleRadians) {
    var length = this.Length();

    this.X = (float)(Math.Cos(angleRadians) * length);
    this.Y = (float)(Math.Sin(angleRadians) * length);
  }
  
  public float GetAngleDegrees() {
    return (float)(this.GetAngle() * 180 / Math.PI);
  }
  
  public float GetAngle() {
    return (float)Math.Atan2(this.Y, this.X);
  }
  
  public void RotateAroundDegrees(Vec2 point, float angleDegrees) {
    this.RotateAround(point, (float)(angleDegrees * Math.PI / 180));
  }
  
  public void RotateAround(Vec2 point, float angleRadians) {
    var rotatedX = (float)(Math.Cos(angleRadians) * (this.X - point.X) - Math.Sin(angleRadians) * (this.Y - point.Y) + point.X);
    var rotatedY = (float)(Math.Sin(angleRadians) * (this.X - point.X) + Math.Cos(angleRadians) * (this.Y - point.Y) + point.Y);

    this.X = rotatedX;
    this.Y = rotatedY;
  }
  
  /// <summary>
  /// Calculates the normal (perpendicular in 2D) of the vector.
  /// </summary>
  /// <returns>Unit normal</returns>
  public Vec2 UnitNormal() {
    var normalized = this.Normalized();

    return normalized.Rotated((float)Math.PI / 2);
  }

  /// <summary>
  /// Returns the unit tangent of the vector.
  /// </summary>
  /// <returns></returns>
  public Vec2 UnitTangent() {
    var normalized = this.Normalized();

    return normalized.Rotated((float)Math.PI);
  }
  
  public Vec2 Perpendicular() {
    return new Vec2(-this.Y, this.X);
  }
  
  /// <summary>
  /// Projects this vector onto the given vector (`other`).
  /// </summary>
  /// <param name="other"></param>
  /// <returns>The length of the projected Vector</returns>
  public float ProjectLength(Vec2 other) {
    other = other.Normalized();

    return (Dot(this, other));
  }
  
  /// <summary>
  /// Reflects this vector off the given normal. 
  /// </summary>
  /// <param name="normal">The normal of the reflection</param>
  /// <param name="elasticity">From 0 to 1, how "elastic" should the reflection be</param>
  /// <returns></returns>
  public Vec2 Reflect(Vec2 normal, float elasticity = 1) {
    normal = normal.Normalized();
    
    return this - (1 + elasticity) * Dot(this, normal) * normal;
  }
  
  // Generic helper for operations
  // Equality
  public override bool Equals(object obj) {
    return obj is Vec2 vec2 && this == vec2;
  }

  public override int GetHashCode() {
    unchecked {
      return (X.GetHashCode() * 397) ^ Y.GetHashCode();
    }
  }

  public static bool operator ==(Vec2 a, Vec2 b) {
    return Math.Abs(a.X - b.X) < 0.00001 && Math.Abs(a.Y - b.Y) < 0.00001;
  }

  public static bool operator !=(Vec2 a, Vec2 b) {
    return !(a == b);
  }

  public static Vec2 operator +(Vec2 left, Vec2 right) {
    return new Vec2(left.X + right.X, left.Y + right.Y);
  }

  public static Vec2 operator -(Vec2 left, Vec2 right) {
    return new Vec2(left.X - right.X, left.Y - right.Y);
  }

  public static Vec2 operator *(Vec2 v, float scalar) {
    return new Vec2(v.X * scalar, v.Y * scalar);
  }

  public static Vec2 operator *(float scalar, Vec2 v) {
    return new Vec2(v.X * scalar, v.Y * scalar);
  }

  public static Vec2 operator /(Vec2 v, float scalar) {
    return new Vec2(v.X / scalar, v.Y / scalar);
  }

  public override string ToString() {
    return $"({X}, {Y})";
  }
  
  // Static methods below
  
  /// <summary>
  /// Returns a vector with all elements set to `value`.
  /// </summary>
  public static Vec2 Splat(float value) {
    return new Vec2(value, value);
  }

  /// <summary>
  /// Computes the Dot Product of two vectors
  /// </summary>
  /// <param name="left"></param>
  /// <param name="right"></param>
  /// <returns></returns>
  public static float Dot(Vec2 left, Vec2 right) {
    return (left.X * right.X) + (left.Y * right.Y);
  }
  
  /// <summary>
  /// Computes the distance between two vectors
  /// </summary>
  /// <param name="left"></param>
  /// <param name="right"></param>
  /// <returns></returns>
  public static float Distance(Vec2 left, Vec2 right) {
    return (left - right).Length();
  }

  /// <summary>
  /// Also known as the Wedge Product, Perpendicular Dot Product, 2D Cross Product or the Determinant.
  /// </summary>
  /// <param name="left"></param>
  /// <param name="right"></param>
  /// <returns></returns>
  public static float Cross(Vec2 left, Vec2 right) {
    return left.X * right.Y - left.Y * right.X;
  }

  /// <summary>
  /// Calculate the angle between two vectors
  /// </summary>
  /// <param name="left"></param>
  /// <param name="right"></param>
  /// <returns></returns>
  public static float AngleBetween(Vec2 left, Vec2 right) {
    return (float)Math.Acos(Dot(left, right) / (left.Length() * right.Length()));
  }
  
  public static float Deg2Rad(float degrees) {
    return degrees * (float)Math.PI / 180;
  }
  
  public static float Rad2Deg(float radians) {
    return radians * 180 / (float)Math.PI;
  }
  
  public static Vec2 GetUnitVectorDeg(float degrees) {
    return GetUnitVectorRad(Deg2Rad(degrees));
  }
  
  public static Vec2 GetUnitVectorRad(float radians) {
    return new Vec2((float)Math.Cos(radians), (float)Math.Sin(radians));
  }
  
  // public static Vec2 RandomUnitVector() {
  //   return GetUnitVectorRad((float)new Random.NextDouble() * (float)Math.PI * 2);
  // }
  
  // Unit test for the above methods
}