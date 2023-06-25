using System;
using GXPEngine;
using Physics;

class Target : AnimationSprite {
  Collider _collider;

  public Target(Vec2 position) : base("tilesheet.png", 14, 7) {
    SetFrame(14 * 3 + 5);
    SetOrigin(width / 2, height / 2);

    // Create collider, and register it (as solid / collision object):
    _collider = new AxisAlignedBoundingBox(this, position, width / 2, height / 2);
    ColliderManager.Main.AddTriggerCollider(_collider);

    x = position.X;
    y = position.Y;
  }
  
  void Update() {
    var overlaps = ColliderManager.Main.GetOverlaps(_collider);
    if (overlaps.Count <= 0) return;
    if (overlaps[0].Owner is Player) return;
    
    Console.WriteLine("Target hit with {0}", overlaps[0].Owner);
    this.LateDestroy();
    ((TechDemo)game).ShouldSpawnTarget = true;
  }
  
   protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    ColliderManager.Main.RemoveTriggerCollider(_collider);
  }
}