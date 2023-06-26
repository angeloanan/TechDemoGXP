using GXPEngine;
using Physics;

public class TechDemo : Game {
  const int SpriteSize = 64;
  
  private readonly EasyDraw _debugText;
  
  public bool ShouldSpawnTarget = true;
  
  private TechDemo() : base(800, 600, false, false) {
    // Generate floor below
    for (var i = -this.width; i < this.width * 2; i += 64) {
     var pos = new Vec2(i, this.height - 32);
      AddChild(new Platform(pos));

      pos.Y -= 64 * 9;
      AddChild(new SolidBlock(pos));
    }
    
    for (var i = 0; i < this.height * 2; i += 64) {
      var pos = new Vec2(0, i);
      AddChild(new SolidBlock(pos));
      AddChild(new SolidBlock(new Vec2(this.width, i)));

      // if (i < 5 * 64) continue;
      // pos.X += 160;
      // AddChild(new SolidBlock(pos));
    }
    // Add random block
    AddChild(new SolidBlock(new Vec2(400, 300)));
    
    // Add player
    AddChild(new Player(new Vec2(80, 520)));
    
    // Diagonal Line TL
    AddChild(new Wall(new Vec2(150, 50), new Vec2(30, 170)));
    
    // Random line Right
    AddChild(new Wall(new Vec2(770, 100), new Vec2(600, 300)));
    
    AddChild(new BlackHole(new Vec2(220, 350), 100));
    
    // DEBUG
    _debugText = new EasyDraw(500, 24);
    _debugText.x = 32;
    _debugText.y = 32;
    AddChild(_debugText);
  }
  
  private void SpawnTarget() {
    // Randomly choose between fuzzer and target
    var fuzzer = Utils.Random(0, 2) == 0;
    
    if (fuzzer) {
      var target = new FuzzerTarget(new Vec2(Utils.Random(128, width - 128), Utils.Random(128, height - 128)));
      AddChild(target);
    }
    else {
      var target = new Target(new Vec2(Utils.Random(128, width - 128), Utils.Random(128, height - 128)));
      AddChild(target);
    }

    ShouldSpawnTarget = false;
  }

  void Update() {
    this.targetFps = Input.GetKey(Key.S) ? 3: 60;
    
    _debugText.Clear(0x00);
    _debugText.TextAlign(CenterMode.Min, CenterMode.Min);

    var debugString =
      $"Mouse: {Input.mouseX}, {Input.mouseY} | Trigger: {ColliderManager.Main.TriggerColliderCount()}, Solid: {ColliderManager.Main.SolidColliderCount()}";

    _debugText.Text(debugString, 0, 0);
    

    if (ShouldSpawnTarget) {
      SpawnTarget();
      ShouldSpawnTarget = false;
    }
  }

  private static void Main() {
    UnitTests.RunAll();
    
    new TechDemo().Start();
  }
}