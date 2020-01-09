
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
            // Point position in game coords.
            // This is also the collision point.
            Vector2d pointPosition,
            // Circle position in game coords.
            Vector2d circlePosition,
            Vector2d circleVelocity
        )
        {
            Vector2d collisionPoint = pointPosition;
            double velocityMagnitude = circleVelocity.Length;

            if ((circlePosition - pointPosition).Length < circleRadius)
            {
                return new CollisionResult {
                    DidCollide = true,
                    NewCircleVelocity = (circlePosition - pointPosition).Normalized() * velocityMagnitude
                };
            }
            else
            {
                // Return original velocity
                return new CollisionResult {
                    DidCollide = false,
                    NewCircleVelocity = circleVelocity
                };                
            }

        //newVelX = (circleVelocity.x * (5 – 5000) + (2 * 5000 * 0)) / (5 + 5000);

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
            .Select(x => (Vector2d)(new Vector4((float)x.X, (float)x.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy)
            .ToArray();

            
            var collisionResults = rectangleCorners.Select(x => CircleVsUnmovablePoint(
                circleRadius,
                x,
                circlePosition,
                origCircleVelocity
            ));

            if (collisionResults.Any(x => x.DidCollide))
                return (collisionResults.First(x => x.DidCollide));

            // If any of four edge tests were a collision, pass on the first positive result
            
            
            // Return original velocity
            return new CollisionResult {
                DidCollide = false,
                NewCircleVelocity = origCircleVelocity
            };
        }
    }
}