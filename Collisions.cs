
using OpenTK;
using System;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public struct CollisionResult
    {
        public bool DidCollide;
        public Vector2d NewCirclePosition;
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
                //var newVelocity = (circlePosition - pointPosition).Normalized() * velocityMagnitude;
                Vector2d relativeCirclePosition = circlePosition - pointPosition;
                double relativeAngleOfCircleRadians = Math.Atan2(relativeCirclePosition.Y, relativeCirclePosition.X) - Math.PI/2;
                var rotationMatrix = Matrix4d.CreateRotationZ(relativeAngleOfCircleRadians).Inverted();

                Vector3d rotatedCirclePosition = Vector3d.Transform(new Vector3d(relativeCirclePosition.X, relativeCirclePosition.Y, 0.0), rotationMatrix);
                Vector3d rotatedCircleVelocity = Vector3d.Transform(new Vector3d(circleVelocity.X, circleVelocity.Y, 0.0), rotationMatrix);

                // make the circle's Y velocity positive in this frame of reference,
                Vector3d postCollisionRotatedCircleVelocity = new Vector3d(rotatedCircleVelocity.X, Math.Abs(rotatedCircleVelocity.Y), 0);

                // Rotate the new velocity back.
                Vector3d postCollisionCircleVelocity = Vector3d.Transform(postCollisionRotatedCircleVelocity, rotationMatrix.Inverted());

                var newVelocity = postCollisionCircleVelocity.Xy;

                // Return as a positive collision result.
                return new CollisionResult {
                    DidCollide = true,
                    NewCirclePosition = circlePosition + newVelocity,
                    NewCircleVelocity = newVelocity
                };
            }
            else
            {
                // Return original velocity
                return new CollisionResult {
                    DidCollide = false,
                    NewCirclePosition = circlePosition,
                    NewCircleVelocity = circleVelocity
                };
            }
        }
        // public static (CollisionResult, CollisionResult) CircleVsCircle(
        //     double circleRadius1,
        //     double circleRadius2,
        //     Vector2d circlePosition1,
        //     Vector2d circlePosition2,
        //     Vector2d circleVelocity1,
        //     Vector2d circleVelocity2
        // )
        // {
        //     Vector2d circle2RelativePosition = circlePosition2 - circlePosition1;
        //     double distanceBetweenCentres = circle2RelativePosition.Length;
        //     if (distanceBetweenCentres == 0)
        //     {
        //         return (
        //             new CollisionResult {
        //                 DidCollide = false,
        //                 NewCirclePosition = circlePosition1 - new Vector2d(circleRadius1, 0),
        //                 NewCircleVelocity = circleVelocity1
        //             },
        //             new CollisionResult {
        //                 DidCollide = false,
        //                 NewCirclePosition = circlePosition2 + new Vector2d(circleRadius1, 0),
        //                 NewCircleVelocity = circleVelocity2
        //             }
        //         );
        //     }

        //     bool colliding = distanceBetweenCentres < circleRadius1 + circleRadius2;
        //     if (!colliding)
        //     {
        //         // Return original velocity
        //         return (
        //             new CollisionResult {
        //                 DidCollide = false,
        //                 NewCirclePosition = circlePosition1,
        //                 NewCircleVelocity = circleVelocity1
        //             },
        //             new CollisionResult {
        //                 DidCollide = false,
        //                 NewCirclePosition = circlePosition2,
        //                 NewCircleVelocity = circleVelocity2
        //             }
        //         );
        //     }

        //     Vector2d collisionPoint = circlePosition1 + (circle2RelativePosition.Normalized()) * circleRadius1;


        //     double relativeAngleOfCircle2Radians = Math.Atan2(circle2RelativePosition.Y, circle2RelativePosition.X) - Math.PI/2;
        //     var rotationMatrix = Matrix4d.CreateRotationZ(relativeAngleOfCircle2Radians).Inverted();

        //     Vector3d rotatedCircle1Velocity = Vector3d.Transform(new Vector3d(circleVelocity1.X, circleVelocity1.Y, 0.0), rotationMatrix);
        //     Vector3d rotatedCircle2Position = Vector3d.Transform(new Vector3d(circle2RelativePosition.X, circle2RelativePosition.Y, 0.0), rotationMatrix);
        //     Vector3d rotatedCircle2Velocity = Vector3d.Transform(new Vector3d(circleVelocity2.X, circleVelocity2.Y, 0.0), rotationMatrix);

        //     // Make the first circle's Y velocity negative, and the second circle's Y velocity positive.
        //     Vector3d postCollisionRotatedCircleVelocity1 = new Vector3d(rotatedCircle1Velocity.X, Math.Abs(rotatedCircle1Velocity.Y), 0);
        //     Vector3d postCollisionRotatedCircleVelocity2 = new Vector3d(rotatedCircle2Velocity.X, -Math.Abs(rotatedCircle2Velocity.Y), 0);

        //     // Rotate the new velocities back.
        //     Vector3d postCollisionCircleVelocity1 = Vector3d.Transform(postCollisionRotatedCircleVelocity1, rotationMatrix.Inverted());
        //     Vector3d postCollisionCircleVelocity2 = Vector3d.Transform(postCollisionRotatedCircleVelocity2, rotationMatrix.Inverted());

        //     var newVelocity1 = postCollisionCircleVelocity1.Xy;
        //     var newVelocity2 = postCollisionCircleVelocity2.Xy;

        //     // Return positive collision result with updated velocities.
        //     return (
        //         new CollisionResult {
        //             DidCollide = true,
        //             NewCirclePosition = circlePosition1 - circle2RelativePosition.Normalized() * (circleRadius1 + circleRadius2),
        //             NewCircleVelocity = newVelocity1
        //         },
        //         new CollisionResult {
        //             DidCollide = true,
        //             NewCirclePosition = circlePosition2 + circle2RelativePosition.Normalized() * (circleRadius1 + circleRadius2),
        //             NewCircleVelocity = newVelocity2
        //         }
        //     );
        // }

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
                    NewCirclePosition = circlePosition,
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

                var newVelocity = postCollisionCircleVelocity.Xy;

                // Return as a positive collision result.
                return new CollisionResult {
                    DidCollide = true,
                    NewCirclePosition = circlePosition + newVelocity,
                    NewCircleVelocity = newVelocity
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
                NewCirclePosition = circlePosition,
                NewCircleVelocity = origCircleVelocity
            };
        }
    }
}