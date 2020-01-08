
using OpenTK;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public struct CollisionResult
    {
        public bool DidCollide;
        public Vector2 NewCircleVelocity;
    }

    public static class Collisions
    {
        public static CollisionResult CircleVsUnmovablePoint(
            double circleRadius,
            // Point transformation for game coords.
            // Includes all data about the point's position and rotation.
            // We can apply this in inverse to the circle to get a nice coordinate system.
            Matrix4 pointTransform,
            // Circle transformation for game coords.
            Matrix4 circleTransform,
            Vector2 circleVelocity
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
            Vector2 origCircleVelocity
        )
        {
            // A translation matrix stores its data in the last row.
            Vector2 rectanglePosition = rectangleTransform.Row3.Xy;
            Vector2 circlePosition = circleTransform.Row3.Xy;

            // If any of four corner tests were a collision, return the first positive result
            Vector2d[] rectangleCorners = new Vector2d[] {
                new Vector2d(rectanglePosition.X - rectangleWidth / 2, rectanglePosition.Y - rectangleHeight / 2),
                new Vector2d(rectanglePosition.X - rectangleWidth / 2, rectanglePosition.Y + rectangleHeight / 2),
                new Vector2d(rectanglePosition.X + rectangleWidth / 2, rectanglePosition.Y - rectangleHeight / 2),
                new Vector2d(rectanglePosition.X + rectangleWidth / 2, rectanglePosition.Y + rectangleHeight / 2)
            };
            
            var collisionResults = rectangleCorners.Select(x => CircleVsUnmovablePoint(
                circleRadius,
                rectangleTransform * Matrix4.CreateTranslation((float)x.X, (float)x.Y, 0.0f),
                circleTransform,
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