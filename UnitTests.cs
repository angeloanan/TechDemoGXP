using System;

namespace GXPEngine {
  public class UnitTests {
    public static void RunAll() {
      Console.WriteLine("Running Unit Tests...");
      AdditionTest();
      LengthTest();
      NormalizedTest();
      RotateTest();
      // UnitNormalTest();
      // UnitTangentTest();
      // ReflectionTest();
      ProjectionTest();
      DistanceTest();
      CrossTest();
      SplatTest();
      DotTest();
      AngleBetweenTest();
      Console.WriteLine("All tests passed!");
    }
    
    public static void AdditionTest() {
      var u = new Vec2(1, 2);
      var v = u + u;
      Assert.AreEqual(new Vec2(2, 4), v);
    }
    
    public static void LengthTest() {
      Vec2 vec = new Vec2(3, 4);
      Assert.AreEqual(5, vec.Length());

      vec = new Vec2(6, 8);
      Assert.AreEqual(10, vec.Length());
    }
    
    public static void NormalizedTest() {
      Vec2 vec = new Vec2(8, 6);
      Assert.AreEqual(new Vec2(0.8f, 0.6f), vec.Normalized());

      vec = new Vec2(3, 4);
      Assert.AreEqual(new Vec2(0.6f, 0.8f), vec.Normalized());
    }
    
    public static void RotateTest() {
      Vec2 vec = new Vec2(3, 4);
      Assert.AreEqual(new Vec2(-4, 3), vec.Rotated((float)Math.PI / 2));

      vec = new Vec2(6, 8);
      Assert.AreEqual(new Vec2(-8, 6), vec.RotatedDegrees(90.0f));
    }
    
    public static void UnitNormalTest() {
      Vec2 vec = new Vec2(3, 4);
      Assert.AreEqual(new Vec2(-4/5f, 3/5f), vec.UnitNormal());
    
      vec = new Vec2(2, 3);
      Assert.AreEqual(new Vec2(-3/5f, 2/5f), vec.UnitNormal());
    }
    
    public static void UnitTangentTest() {
      Vec2 vec = new Vec2(3, 4);
      Assert.AreEqual(new Vec2(-3/5f, 4/5f), vec.UnitTangent());
    
      vec = new Vec2(2, 3);
      Assert.AreEqual(new Vec2(-2/5f, 3/5f), vec.UnitTangent());
    }

    public static void ReflectionTest() {
      var vec = new Vec2(1.2F, 3.4F);
      var normal = new Vec2(0, 1);
      
      Assert.AreEqual(new Vec2(-1.2f, 3.4f), vec.Reflect(normal));

      vec = new Vec2(1,1);
      Assert.AreEqual(new Vec2(-1f, 0.5f), vec.Reflect(normal, 0.5f));
    }
  
    public static void ProjectionTest() {
      Vec2 vec = new Vec2(-4, 3);
      Vec2 other = new Vec2(1, 0);
      Assert.AreEqual(-4f, vec.ProjectLength(other));
    }
    
    public static void SplatTest() {
      Assert.AreEqual(new Vec2(7,7), Vec2.Splat(7));
    }

    public static void DotTest() {
      Vec2 v1 = new Vec2(0.8F, 0.6F);
      Vec2 v2 = new Vec2(2F, -11F);
      Assert.AreEqual(-5F, Vec2.Dot(v1, v2));
    }    

    public static void DistanceTest() {
      Vec2 v1 = new Vec2(0,10);
      Vec2 v2 = new Vec2(0,0);
      Assert.AreEqual(10, Vec2.Distance(v1, v2));
    }

    public static void CrossTest() {
      Vec2 v1 = new Vec2(2,3);
      Vec2 v2 = new Vec2(5,1);
      Assert.AreEqual(-13, Vec2.Cross(v1, v2));
    }

    public static void AngleBetweenTest() {
      Vec2 v1 = new Vec2(-1,1);
      Vec2 v2 = new Vec2(-3,2);
    }
  }

  public static class Assert {
    public static void AreEqual(float expected, float actual) {
      if (Math.Abs(expected - actual) > 0.0001f) {
        throw new Exception($"Expected {expected}, got {actual}");
      } 
    }
    
    public static void AreEqual<T>(T expected, T actual) {
      if (!expected.Equals(actual)) {
        throw new Exception($"Expected {expected}, got {actual}");
      } 
    }
    
    public static void AreSame<T>(T expected, T actual) {
      if (!ReferenceEquals(expected, actual)) {
        throw new Exception($"Expected {expected}, got {actual}");
      } 
    }
    
  }
}
