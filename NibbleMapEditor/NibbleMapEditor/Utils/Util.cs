using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NibbleMapEditor.Utils
{
    public class Util
    {
        /// <summary>
        /// Atan2
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleFrom(Vector2 point1, Vector2 point2)
        {
            return (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }
        /// <summary>
        /// Atan2
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleFrom(ref Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        static Vector2 temp = Vector2.Zero;
        public static Vector2 AngleFrom(float angle)
        {
            temp.X = (float)Math.Cos(angle);
            temp.Y = (float)Math.Sin(angle);
            return temp;
        }

        /// <summary>
        /// Gets a normalized unit vector direction
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Vector2 DirectionFrom(Vector2 point1, Vector2 point2)
        {
            float angle = AngleFrom(point1, point2);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            direction.Normalize();
            return direction;
        }
        public static Vector2 DirectionFrom(Vector2 point1, Vector2 point2, out float angle)
        {
            angle = AngleFrom(point1, point2);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            direction.Normalize();
            return direction;
        }

        private static Random rand = new Random();

        /// <summary>
        /// Recreates the random object with a new seed
        /// </summary>
        /// <param name="seed"></param>
        public static void SetSeed(int seed)
        {
            rand = new Random(seed);
        }
        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Last value</param>
        /// <param name="weight">Weight</param>
        /// <returns>Returns the interpolated value</returns>

        public static float Lerp(float value1, float value2, float weight)
        {
            return value1 + (value2 - value1) * weight;
        }
        /// <summary>
        /// Linear interpolation for large angles
        /// </summary>
        /// <param name="from">From angle</param>
        /// <param name="to">To angle</param>
        /// <param name="weight">Weigth</param>
        /// <returns>Returns the interpolated value</returns>
        public static float Clerp(float from, float to, float weight)
        {
            float t = ((MathHelper.WrapAngle(to - from) * (weight)));
            return from + t;
        }
        /// <summary>
        /// Gets a random float.
        /// </summary>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random float value</returns>
        public static float Rand(float max)
        {
            return (float)rand.NextDouble() * max;
        }
        /// <summary>
        /// Gets a random float.
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random float value</returns>
        public static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
        /// <summary>
        /// Gets a random integer.
        /// </summary>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random integer</returns>
        public static int Rand(int max)
        {
            return rand.Next(max);
        }
        /// <summary>
        /// Gets a random integer.
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Returns a random integer</returns>
        public static int Rand(int min, int max)
        {
            return rand.Next(min, max);
        }

        /// <summary>
        /// Rotates a vector2 around an origin using matrices
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="origin">Origin</param>
        /// <param name="rotation">Rotation amount</param>
        /// <returns>Returns the rotated vector</returns>
        public static Vector2 RotatePoint(Vector2 point, Vector2 origin, float rotation)
        {
            Matrix m =
                Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *      // translate back to rotate about (0,0) 
                Matrix.CreateRotationZ(rotation) *                            // rotate 
                Matrix.CreateTranslation(new Vector3(origin, 0.0f));        // translate back to origin 

            return Vector2.Transform(point, m);
        }

        /// <summary>
        /// Creates a rectangle that finds the smallest possible rectangle without going outside the bounds of the rectangle
        /// </summary>
        /// <param name="size">Size, usually origin</param>
        /// <param name="offset">Position of the rectangle</param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Rectangle RotateRectangle(Vector2 size, Vector2 offset, float scale, float rotation)
        {
            float minX = -size.X * scale;
            float maxX = size.X * scale;
            float minY = -size.Y * scale;
            float maxY = size.Y * scale;

            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(minX, minY); //Top left
            points[1] = new Vector2(minX, maxY); //Bottom left
            points[2] = new Vector2(maxX, minY); //Top Right
            points[3] = new Vector2(maxX, maxY); //Bottom right

            Vector2[] destination = new Vector2[4];

            Vector2 _offset = offset;

            Matrix rotMatrix = Matrix.CreateRotationZ(rotation);
            Vector2.Transform(points, ref rotMatrix, destination);

            for (int i = 0; i < destination.Length; i++)
                destination[i] += _offset;

            //Rotated
            float x = destination[0].X, y = destination[0].Y, r = destination[0].X, b = destination[0].Y;
            for (int i = 0; i < 4; i++)
            {
                //Find lowest X
                if (x > destination[i].X)
                    x = destination[i].X;

                //Find lowest Y
                if (y > destination[i].Y)
                    y = destination[i].Y;

                //Find highest X
                if (r < destination[i].X)
                    r = destination[i].X;

                //Find highest Y
                if (b < destination[i].Y)
                    b = destination[i].Y;
            }

            //Rotated
            int width = (int)(r - x);
            int height = (int)(b - y);
            int tX = (int)x;
            int tY = (int)y;

            return new Rectangle(tX, tY, width, height);
        }
    }
}
