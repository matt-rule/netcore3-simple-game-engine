
using OpenTK;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public struct CollisionResult
    {
        public bool DidCollide;
        public Vector2d NewCircleVelocity;
    }

    public static class Collisions
    {
        public static CollisionResult CircleVsUnmovablePoint(
            double circleRadius,
            // Point transformation for game coords.
            Vector2d pointPosition,
            // Circle transformation for game coords.
            Vector2d circlePosition,
            Vector2d circleVelocity
        )
        {
            // Return original velocity
            return new CollisionResult {
                DidCollide = false,
                NewCircleVelocity = circleVelocity
            };
        }

        // Treat the rectangle as an unmovable object and perform a collision test
        // against a circle, then return only the altered velocity of the circle.
        public static CollisionResult CircleVsUnmovableRectangle(
            double rectangleWidth,
            double rectangleHeight,
            double circleRadius,
            // Rectangle transformation for game coords.
            // We can apply this in inverse to the circle to get a nice coordinate system.
            Matrix4 rectangleTransform,
            // Circle transformation for game coords.
            Matrix4 circleTransform,
            Vector2d origCircleVelocity
        )
        {
            // A translation matrix stores its data in the last row.
            Vector2d rectanglePosition = (Vector2d)(rectangleTransform.Row3.Xy);
            Vector2d circlePosition = (Vector2d)circleTransform.Row3.Xy;

            // If any of four corner tests were a collision, return the first positive result
            Vector2d[] rectangleCorners = new Vector2d[] {
                new Vector2d(-rectangleWidth / 2, -rectangleHeight / 2),
                new Vector2d(-rectangleWidth / 2, +rectangleHeight / 2),
                new Vector2d(+rectangleWidth / 2, -rectangleHeight / 2),
                new Vector2d(+rectangleWidth / 2, +rectangleHeight / 2)
            }
            .Select(x => (new Vector4((float)x.X, (float)x.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy)
            .Select(x => new Vector2d((double)x.X, (double)x.Y))
            .ToArray();

            
            var collisionResults = rectangleCorners.Select(x => CircleVsUnmovablePoint(
                circleRadius,
                x,
                circlePosition,
                origCircleVelocity
            ));

            // If any of four edge tests were a collision, pass on the first positive result
            
            
            // Return original velocity
            return new CollisionResult {
                DidCollide = false,
                NewCircleVelocity = origCircleVelocity
            };
        }
    }
}