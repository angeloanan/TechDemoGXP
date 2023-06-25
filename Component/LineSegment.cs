using GXPEngine.Core;

namespace GXPEngine {
  class LineSegment : GameObject {
    public readonly Vec2 StartPosition;
    public readonly Vec2 EndPosition;
    public readonly uint color;
    public readonly uint lineWidth;

    // Constructor
    public LineSegment(Vec2 startPosition, Vec2 endPosition, uint color = 0xffffffff, uint lineWidth = 1) {
      this.StartPosition = startPosition;
      this.EndPosition = endPosition;
      this.color = color;
      this.lineWidth = lineWidth;
    }

    protected override void RenderSelf(GLContext glContext) {
      if (game != null) {
        Gizmos.RenderLine(this.StartPosition.X, this.StartPosition.Y, EndPosition.X, EndPosition.Y, color, lineWidth, true);
      }
    }
  }
}