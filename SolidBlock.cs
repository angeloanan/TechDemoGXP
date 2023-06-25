using System;
using System.Collections.Generic;
using GXPEngine;
using Physics;

class SolidBlock : AnimationSprite {
  Collider _collider;
  ColliderManager _colliderManager;

  public SolidBlock(Vec2 position) : base("tilesheet.png", 14, 7) {
    SetFrame(17);
    SetOrigin(width / 2, height / 2);

    // Create collider, and register it (as solid / collision object):
    _collider = new AxisAlignedBoundingBox(this, position, width / 2, height / 2);
    _colliderManager = ColliderManager.Main;
    _colliderManager.AddSolidCollider(_collider);
    x = position.X;
    y = position.Y;
  }

  void Update() {
    // Check for collisions with the player:
    List<Collider> overlaps = _colliderManager.GetOverlaps(_collider);

    if (overlaps.Count > 1) {
      _colliderManager.RemoveSolidCollider(_collider);
      this.game.RemoveChild(this);
    }
  }
  protected override void OnDestroy() {
    // Remove the collider when the sprite is destroyed:
    _colliderManager.RemoveSolidCollider(_collider);
  }
}