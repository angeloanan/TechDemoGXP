using System;
using GXPEngine;
using Physics;

class FuzzerTarget : AnimationSprite {
  Collider _collider;

  public FuzzerTarget(Vec2 position) : base("tilesheet.png", 14, 7) {
    SetFrame(14 * 4 + 5);
    SetOrigin(width / 2, height / 2);

    // Create collider, and register it (as solid / collision object):
    _collider = new AxisAlignedBoundingBox(this, position, width / 2, height / 2);
    ColliderManager.Main.AddTriggerCollider(_collider);

    x = position.X;
    y = position.Y;
  }
  
  void Update() {
    var overlaps = ColliderManager.Main.GetOverlaps(_collider).FindAll(c => c.Owner is Ball);
    if (overlaps.Count <= 0) return;
    
    Console.WriteLine("Fuzzer Target hit with {0}. Scrambling velocity...", overlaps[0].Owner);
    ((Ball)overlaps[0].Owner).scrambleDirection();
    
    this.LateDestroy();
    ((TechDemo)game).ShouldSpawnTarget = true;
  }
  
  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveTriggerCollider(_collider);
  }
}