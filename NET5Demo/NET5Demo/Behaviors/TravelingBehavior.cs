﻿using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Input.Keyboard;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace NET5Demo.Behaviors
{
    public class TravelingBehavior : Behavior
    {
        private Transform3D CameraTransform;
        private Camera3D Camera;
        private List<Transform3D> points;

        private Vector3 velocity;
        private bool traveling;

        private int current = 0;
        private Transform3D to;

        public float SmoothTime { get; set; }

        public TravelingBehavior()
        {
            this.points = new List<Transform3D>();
            this.velocity = Vector3.Zero;
            this.SmoothTime = 3.0f;
        }

        protected override bool OnAttached()
        {
            var result = base.OnAttached();
            var transforms = this.Owner.FindComponentsInChildren<Transform3D>(skipOwner: true);
            this.CameraTransform = transforms.First();
            this.Camera = this.Owner.ChildEntities.First().FindComponent<Camera3D>();
            foreach (var transform in transforms.Skip(1))
            {
                this.points.Add(transform);
            }

            return result;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var display = this.Camera.Display;
            if (display == null)
            {
                return;
            }

            var keyboardDispacher = display.KeyboardDispatcher;

            if (keyboardDispacher == null)
            {
                return;
            }

            if (keyboardDispacher.IsKeyDown(Keys.T))
            {
                this.current = 0;
                this.CameraTransform.Position = this.points[current].Position;
                this.CameraTransform.Rotation = this.points[current].Rotation;
                this.to = this.points[current + 1];
                this.traveling = true;
            }

            if (traveling)
            {

                this.CameraTransform.Position = Vector3.SmoothDamp(this.CameraTransform.Position, to.Position, ref velocity, this.SmoothTime, (float)gameTime.TotalSeconds);

                if (Vector3.DistanceSquared(this.CameraTransform.Position, to.Position) < 0.05f)
                {
                    current += 2;
                    if (current < this.points.Count)
                    {
                        this.CameraTransform.Position = this.points[current].Position;
                        this.CameraTransform.Rotation = this.points[current].Rotation;
                        to = this.points[current + 1];
                    }
                    else
                    {
                        //traveling = false;
                        this.current = 0;
                        this.CameraTransform.Position = this.points[current].Position;
                        this.CameraTransform.Rotation = this.points[current].Rotation;
                        this.to = this.points[current + 1];
                    }
                }
            }
        }
    }
}
