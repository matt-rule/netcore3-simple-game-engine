
using OpenTK;
using System;
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
        }
        public static CollisionResult CircleVsUnmovableLineSegment(
            // lineV1 and lineV2 are specified clockwise.
            Vector2d lineV1,
            Vector2d lineV2,
            Vector2d circlePosition,
            Vector2d circleVelocity,
            double circleRadius
        )
        {
            // Get the relative position of the circle centre to v1.
            Vector2d relativeCirclePosition = circlePosition - lineV1;

            // Rotate everything so that the line segment is lying flat along the X axis with v1 at the origin.
            // Figure out the angle to rotate by.
            Vector2d v2RelativePosition = lineV2 - lineV1;
            double angleToRotateByRadians = Math.Atan2(v2RelativePosition.Y, v2RelativePosition.X);
            // Produce a matrix to rotate everything by that amount.
            var rotationMatrix = Matrix4d.CreateRotationZ(angleToRotateByRadians).Inverted();

            double lineLength = v2RelativePosition.Length;

            // Perform the rotation.
            Vector3d rotatedCirclePosition = Vector3d.Transform(new Vector3d(relativeCirclePosition.X, relativeCirclePosition.Y, 0.0), rotationMatrix);
            Vector3d rotatedCircleVelocity = Vector3d.Transform(new Vector3d(circleVelocity.X, circleVelocity.Y, 0.0), rotationMatrix);

            // Determine whether the circle is intersecting.
            bool intersection =
                rotatedCirclePosition.X > 0
                && rotatedCirclePosition.X < lineLength
                && rotatedCirclePosition.Y - circleRadius < 0
                && rotatedCirclePosition.Y + circleRadius > 0;
            if (!intersection)
            {
                // Return a negative collision result.
                return new CollisionResult {
                    DidCollide = false,
                    // Use the original circle velocity.
                    NewCircleVelocity = circleVelocity
                };     
            }
            else
            {
                // make the circle's Y velocity positive in this frame of reference,
                Vector3d postCollisionRotatedCircleVelocity = new Vector3d(rotatedCircleVelocity.X, Math.Abs(rotatedCircleVelocity.Y), 0);

                // Rotate the new velocity back.
                Vector3d postCollisionCircleVelocity = Vector3d.Transform(postCollisionRotatedCircleVelocity, rotationMatrix.Inverted());

                // Return as a positive collision result.
                return new CollisionResult {
                    DidCollide = true,
                    NewCircleVelocity = postCollisionCircleVelocity.Xy * 1.005
                };     
            }
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

            var pointCollisionResults = rectangleCorners.Select(x => CircleVsUnmovablePoint(
                circleRadius,
                x,
                circlePosition,
                origCircleVelocity
            ));

            if (pointCollisionResults.Any(x => x.DidCollide))
                return (pointCollisionResults.First(x => x.DidCollide));

            // If any of four edge tests were a collision, pass on the first positive result
            var rectangleEdges = new(Vector2d, Vector2d)[] {
                (new Vector2d(-rectangleWidth / 2, -rectangleHeight / 2), new Vector2d(-rectangleWidth / 2, +rectangleHeight / 2)),
                (new Vector2d(-rectangleWidth / 2, +rectangleHeight / 2), new Vector2d(+rectangleWidth / 2, +rectangleHeight / 2)),
                (new Vector2d(+rectangleWidth / 2, +rectangleHeight / 2), new Vector2d(+rectangleWidth / 2, -rectangleHeight / 2)),
                (new Vector2d(+rectangleWidth / 2, -rectangleHeight / 2), new Vector2d(-rectangleWidth / 2, -rectangleHeight / 2))
            }
            .Select(x => (
                (Vector2d)(new Vector4((float)x.Item1.X, (float)x.Item1.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy,
                (Vector2d)(new Vector4((float)x.Item2.X, (float)x.Item2.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy
            ))
            .ToArray();

            var edgeCollisionResults = rectangleEdges.Select(x => CircleVsUnmovableLineSegment(
                x.Item1,
                x.Item2,
                circlePosition,
                origCircleVelocity,
                circleRadius
            ));

            if (edgeCollisionResults.Any(x => x.DidCollide))
                return (edgeCollisionResults.First(x => x.DidCollide));
            
            // Return original velocity
            return new CollisionResult {
                DidCollide = false,
                NewCircleVelocity = origCircleVelocity
            };
        }
    }
}