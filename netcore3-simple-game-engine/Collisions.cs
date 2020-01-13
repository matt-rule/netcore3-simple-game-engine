using BulletSharp;
using BulletSharp.Math;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public class BallInfo
    {
        public SphereShape BallShape;
        public RigidBodyConstructionInfo BallConstructionInfo;
        public RigidBody Ball;
        public string Name;
    }

    public struct CollisionResult
    {
        public bool DidCollide;
        public Vector2d NewCirclePosition;
        public Vector2d NewCircleVelocity;
    }

    public static class Collisions
    {
        public static DefaultCollisionConfiguration collisionConfig;
        public static CollisionDispatcher collisionDispatcher;
        public static DbvtBroadphase broadphase;
        public static DiscreteDynamicsWorld colWorld;

        // Stores a name for each ball to identify them.
        public static List<BallInfo> balls;

        static Collisions()
        {
            collisionConfig = new DefaultCollisionConfiguration();
            collisionDispatcher = new CollisionDispatcher(collisionConfig);
            broadphase = new DbvtBroadphase();
            colWorld = new DiscreteDynamicsWorld(collisionDispatcher, broadphase, null, collisionConfig);
            colWorld.Gravity = new BulletSharp.Math.Vector3(0, 0, 0);

            //Prepare simulation parameters, try 25 steps to see the ball mid air
            // var simulationIterations = 125;
            // var simulationTimestep = 1f / 60f;

            // //Step through the desired amount of simulation ticks
            // for (var i = 0; i < simulationIterations; i++)
            // {
            //     Update(simulationTimestep);
            // }

            balls = new List<BallInfo>();
        }

        public static void AddBall(string name, OpenTK.Vector3 position)
        {
            SphereShape ballShape = new SphereShape(2f);
            RigidBodyConstructionInfo ballConstructionInfo = new RigidBodyConstructionInfo(5f, new DefaultMotionState(
                Matrix.Translation(position.X, position.Y, position.Z)
            ), ballShape);
            RigidBody ball = new RigidBody(ballConstructionInfo);
            ball.LinearFactor = new BulletSharp.Math.Vector3(1, 1, 0);
            ball.AngularFactor = new BulletSharp.Math.Vector3(0, 0, 1);

            balls.Add(new BallInfo{
                BallShape = ballShape,
                BallConstructionInfo = ballConstructionInfo,
                Ball = ball,
                Name = name
            });

            //Add ball to the world
            colWorld.AddCollisionObject(ball);
        }

        public static void Update(float timeElapsed)
        {
            colWorld.StepSimulation(timeElapsed);
        }

        public static OpenTK.Vector3? GetBall(string name)
        {
            RigidBody ball = balls?.FirstOrDefault(x => x.Name == name)?.Ball;
            if (ball == null)
                return null;
            return new OpenTK.Vector3(
                ball.MotionState.WorldTransform.Origin.X,
                ball.MotionState.WorldTransform.Origin.Y,
                0.0f
            );
        }
        
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

                var newVelocity = postCollisionCircleVelocity.Xy * 1.005;

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

                var newVelocity = postCollisionCircleVelocity.Xy * 1.005;

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
            .Select(x => (Vector2d)(new OpenTK.Vector4((float)x.X, (float)x.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy)
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
                (Vector2d)(new OpenTK.Vector4((float)x.Item1.X, (float)x.Item1.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy,
                (Vector2d)(new OpenTK.Vector4((float)x.Item2.X, (float)x.Item2.Y, (float)0.0, (float)1.0) * rectangleTransform).Xy
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