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
        public Entity BallObject;
        public SphereShape BallShape;
        public RigidBodyConstructionInfo BallConstructionInfo;
        public RigidBody Ball;
    }

    public class BoxInfo
    {
        public Entity BoxObject;
        public Box2DShape BoxShape;
        public RigidBodyConstructionInfo BoxConstructionInfo;
        public RigidBody Box;
        public OpenTK.Vector3 DefaultPosition;
    }

    public struct CollisionResult
    {
        public bool DidCollide;
        public Vector2d NewCirclePosition;
        public Vector2d NewCircleVelocity;
    }

    // public class Test : ContactResultCallback
    // {
    //     // public override float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
    //     // {
    //     //     if ((colObj0Wrap.CollisionObject.UserObject as Entity).Name.StartsWith("ball")
    //     //     && (colObj1Wrap.CollisionObject.UserObject as Entity).Name.StartsWith("bat"))
    //     //         SoundSingleton.PlaySound("assets/193831__ligidium__door-knock-15.wav", 1.0f, false);

    //     //     return 0; // not actually sure if return value is used for anything...?
    //     // }
    // }

    public static class Collisions
    {
        public static DefaultCollisionConfiguration collisionConfig;
        public static CollisionDispatcher collisionDispatcher;
        public static DbvtBroadphase broadphase;
        public static DiscreteDynamicsWorld colWorld;

        // Stores a name for each ball to identify them.
        public static List<BallInfo> balls;
        public static List<BoxInfo> boxes;
        public static Action CollisionAction;

        static Collisions()
        {
            collisionConfig = new DefaultCollisionConfiguration();
            collisionDispatcher = new CollisionDispatcher(collisionConfig);
            broadphase = new DbvtBroadphase();
            colWorld = new DiscreteDynamicsWorld(collisionDispatcher, broadphase, null, collisionConfig);
            colWorld.Gravity = new BulletSharp.Math.Vector3(0, 0, 0);
            CollisionAction = null;

            

            //Prepare simulation parameters, try 25 steps to see the ball mid air
            // var simulationIterations = 125;
            // var simulationTimestep = 1f / 60f;

            // //Step through the desired amount of simulation ticks
            // for (var i = 0; i < simulationIterations; i++)
            // {
            //     Update(simulationTimestep);
            // }

            balls = new List<BallInfo>();
            boxes = new List<BoxInfo>();
        }

        public static void SetCollisionAction(Action collisionAction)
        {
            CollisionAction = collisionAction;
        }

        public static void AddPaddle(Entity boxObject, OpenTK.Vector3 position, double width, double height, double paddleAngle)
        {
            Box2DShape boxShape = new Box2DShape((float)(width / 2), (float)(height / 2), (0.0f));
            RigidBodyConstructionInfo boxConstructionInfo = new RigidBodyConstructionInfo(10000f, new DefaultMotionState(
                Matrix.Translation(position.X, position.Y, 0) * Matrix.RotationZ((float)paddleAngle)
            ), boxShape);
            RigidBody box = new RigidBody(boxConstructionInfo);
            box.LinearFactor = new BulletSharp.Math.Vector3(1, 1, 0);
            box.AngularFactor = new BulletSharp.Math.Vector3(0, 0, 1);
            box.SetDamping(0,0);
            box.Restitution = 1f;
            box.Friction = 0.0f;
            box.UserObject = boxObject;

            foreach (var box2 in boxes)
            {
                box.SetIgnoreCollisionCheck(box2.Box, true);
            }

            boxes.Add(new BoxInfo{
                BoxObject = boxObject,
                BoxShape = boxShape,
                BoxConstructionInfo = boxConstructionInfo,
                Box = box,
                DefaultPosition = position
            });

            //Add box to the world
            colWorld.AddCollisionObject(box);
        }

        public static void SetPaddleRotation(string name, double paddleAngle)
        {
            var paddle = boxes.First(x => (x.BoxObject as Entity).Name == name);
            paddle.Box.MotionState = new DefaultMotionState(
                Matrix.Translation(
                    paddle.DefaultPosition.X,
                    paddle.DefaultPosition.Y,
                    0) * Matrix.RotationZ((float)paddleAngle)
            );
        }

        public static OpenTK.Matrix4 GetPaddleRotation(string name)
        {
            var paddle = boxes.First(x => (x.BoxObject as Entity).Name == name);
            var mat = paddle.Box.MotionState.WorldTransform;

            return new OpenTK.Matrix4(
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
            );
        }
        public static void NearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatcherInfo)
        {
            if (CollisionAction != null)
                CollisionAction();
                
            CollisionDispatcher.DefaultNearCallback(collisionPair, dispatcher, dispatcherInfo);
        }

        public static void TickCallback(DynamicsWorld world, float timeStep)
        {

            int numManifolds = colWorld.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = colWorld.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject obA = contactManifold.Body0 as CollisionObject;
                CollisionObject obB = contactManifold.Body1 as CollisionObject;

                int numContacts = contactManifold.NumContacts;
                for (int j = 0; j < numContacts; j++)
                {
                    ManifoldPoint pt = contactManifold.GetContactPoint(j);
                    if (pt.Distance < 1.0f)
                    {
                        if (CollisionAction != null)
                            CollisionAction();
                        return;
                    }
                }
            }
        }

        public static void AddBall(Entity ballObject, OpenTK.Vector3 position, OpenTK.Vector3 velocity, double radius)
        {
            SphereShape ballShape = new SphereShape((float)radius);
            RigidBodyConstructionInfo ballConstructionInfo = new RigidBodyConstructionInfo(5f, new DefaultMotionState(
                Matrix.Translation(position.X, position.Y, 0)
            ), ballShape);
            RigidBody ball = new RigidBody(ballConstructionInfo);
            ball.LinearFactor = new BulletSharp.Math.Vector3(1, 1, 0);
            ball.AngularFactor = new BulletSharp.Math.Vector3(0, 0, 1);
            ball.LinearVelocity = new BulletSharp.Math.Vector3(velocity.X, velocity.Y, 0);
            ball.SetDamping(0,0);
            ball.Restitution = 1.0f;
            ball.Friction = 0.0f;
            ball.UserObject = ballObject;

            balls.Add(new BallInfo{
                BallObject = ballObject,
                BallShape = ballShape,
                BallConstructionInfo = ballConstructionInfo,
                Ball = ball
            });

            //Add ball to the world
            colWorld.AddCollisionObject(ball);
            colWorld.SetInternalTickCallback(TickCallback);
            //collisionDispatcher.NearCallback += NearCallback;
        }

        public static void Update(float timeElapsed, Action collisionAction)
        {
            colWorld.StepSimulation(timeElapsed);
            foreach (var ball in balls)
            {
                BulletSharp.Math.Vector3 linearVelocity = ball.Ball.LinearVelocity;
                linearVelocity.Normalize();
                ball.Ball.LinearVelocity = linearVelocity * 180;
            }
        }

        public static OpenTK.Vector3? GetBall(string name)
        {
            RigidBody ball = balls?.FirstOrDefault(x => (x.BallObject as Entity).Name == name)?.Ball;
            if (ball == null)
                return null;
            return new OpenTK.Vector3(
                ball.MotionState.WorldTransform.Origin.X,
                ball.MotionState.WorldTransform.Origin.Y,
                0.0f
            );
        }

        public static void SetBallPosition(string name, OpenTK.Vector3 position)
        {
            RigidBody ball = balls?.FirstOrDefault(x => (x.BallObject as Entity).Name == name)?.Ball;
            if (ball == null)
                return;
            ball.MotionState = new DefaultMotionState(
                Matrix.Translation(
                    new BulletSharp.Math.Vector3(position.X, position.Y, 0)
                )
            );
        }

        public static OpenTK.Vector3? GetBox(string name)
        {
            RigidBody box = boxes?.FirstOrDefault(x => (x.BoxObject as Entity).Name == name)?.Box;
            if (box == null)
                return null;
            return new OpenTK.Vector3(
                box.MotionState.WorldTransform.Origin.X,
                box.MotionState.WorldTransform.Origin.Y,
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