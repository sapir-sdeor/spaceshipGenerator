using UnityEngine;
using System.Collections.Generic;

namespace Avrahamy.Math {
    public static class VectorExtensions {
        /// <summary>
        /// Returns the angle between the vector and positive X axis (right).
        /// Angle is in range (-180, 180].
        /// Positive is counter clock wise.
        /// </summary>
        public static float GetAngle(this Vector2 direction) {
            return GetAngle2D(direction.x, direction.y);
        }

        /// <summary>
        /// Returns the angle between the vector and positive X axis (right) on
        /// the XY plane (ignoring Z).
        /// Angle is in range (-180, 180].
        /// Positive is counter clock wise.
        /// </summary>
        public static float GetAngleXY(this Vector3 direction) {
            return GetAngle2D(direction.x, direction.y);
        }

        /// <summary>
        /// Returns the angle between the vector and positive Z axis (forward) on
        /// the XZ plane (ignoring Y).
        /// Angle is in range (-180, 180].
        /// Positive is clock wise.
        /// </summary>
        public static float GetAngleXZ(this Vector3 direction) {
            return GetAngle2D(direction.z, direction.x);
        }

        /// <param name="zeroAxisValue">The value on the axis that points to 0
        /// degrees rotation</param>
        /// <param name="upAxisValue">The value on the axis that points to 90
        /// degrees rotation</param>
        private static float GetAngle2D(float zeroAxisValue, float upAxisValue) {
            var angle = Mathf.Atan2(upAxisValue, zeroAxisValue) * Mathf.Rad2Deg;
            if (angle > 180f) {
                angle -= 360f;
            } else if (angle <= -180f) {
                angle += 360f;
            }
            return angle;
        }

        /// <summary>
        /// Returns the angle between the vector and an axis.
        /// Angle is in range (-180, 180].
        /// Positive is counter clock wise.
        /// </summary>
        public static float GetAngle(this Vector2 direction, Vector2 axis) {
            var angle = direction.GetAngle() - axis.GetAngle();
            if (angle > 180f) {
                angle -= 360f;
            } else if (angle <= -180f) {
                angle += 360f;
            }
            return angle;
        }

        /// <summary>
        /// Returns the equivalent acute angle between two angles.
        /// Example: from = 0, to = 200, sets to = -160
        /// Angle is in range (-180, 180]
        /// </summary>
        public static void NormalizeToAcuteAngle(float from, ref float to) {
            while (to - from > 180f) {
                to -= 360f;
            }
            while (to - from < -180f) {
                to += 360f;
            }
        }

        /// <summary>
        /// Converts a Vector3 to a Vector2 using x and z values
        /// </summary>
        public static Vector2 ToVector2XZ(this Vector3 vector3) {
            return new Vector2(vector3.x, vector3.z);
        }

        /// <summary>
        /// Converts an array of Vector3 to array of Vector2 using x and z values
        /// </summary>
        public static Vector2[] ToVector2XZ(this Vector3[] arr) {
            var vecArray = new Vector2[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                vecArray[i] = new Vector2(arr[i].x, arr[i].z);
            }

            return vecArray;
        }

        /// <summary>
        /// Converts an array of Vector3 to array of Vector2 using x and y values
        /// </summary>
        public static Vector2[] ToVector2XY(this Vector3[] arr) {
            var vecArray = new Vector2[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                vecArray[i] = new Vector2(arr[i].x, arr[i].y);
            }

            return vecArray;
        }

        public static Vector3 ToVector3XZ(this Vector2 vector2) {
            return new Vector3(vector2.x, 0f, vector2.y);
        }

        public static Vector3[] ToVector3XZ(this Vector2[] arr) {
            var vecArray = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                vecArray[i] = new Vector3(arr[i].x, 0f, arr[i].y);
            }

            return vecArray;
        }

        public static Vector3[] ToVector3XY(this Vector2[] arr) {
            var vecArray = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++) {
                vecArray[i] = (Vector3)arr[i];
            }

            return vecArray;
        }

        /// <summary>
        /// Rotate a vector by specified degrees.
        /// </summary>
        public static Vector2 RotateInDegrees(this Vector2 v, float degrees) {
            return v.Rotate(degrees * Mathf.Deg2Rad);
        }

        public static Vector3 RotateInDegreesAroundX(this Vector3 v, float degrees) {
            return Quaternion.Euler(degrees, 0, 0) * v;
        }

        public static Vector3 RotateInDegreesAroundY(this Vector3 v, float degrees) {
            return Quaternion.Euler(0, degrees, 0) * v;
        }

        public static Vector3 RotateInDegreesAroundZ(this Vector3 v, float degrees) {
            return Quaternion.Euler(0, 0, degrees) * v;
        }

        /// <summary>
        /// Rotate a vector by specified degrees (in radians).
        /// </summary>
        public static Vector2 Rotate(this Vector2 v, float radians) {
            if (radians == 0f) return v;
            var sin = Mathf.Sin(radians);
            var cos = Mathf.Cos(radians);

            var tx = cos * v.x - sin * v.y;
            var ty = sin * v.x + cos * v.y;

            return new Vector2(tx, ty);
        }

        public static Vector3 Rotate(this Vector3 v, float degrees, Vector3 axis) {
            return Quaternion.AngleAxis(degrees, axis) * v;
        }

        public static float DirectionToSlope(this Vector3 direction) {
            return ((Vector2)direction).DirectionToSlope();
        }

        public static float DirectionToSlope(this Vector2 direction) {
            if (Mathf.Approximately(direction.x, 0f)) {
                return direction.y >= 0f ? float.PositiveInfinity : float.NegativeInfinity;
            }
            return direction.y / direction.x;
        }

        public static Vector2 GetPerpendicularCW(this Vector2 v) {
            return new Vector2(v.y, -v.x);
        }

        /// <summary>
        /// Returns negative if the point is to the left of the vector and positive
        /// if it is to the right.
        /// In fact, it returns the signed distance of the point to the vector times the
        /// magnitude of the vector.
        /// </summary>
        public static float PointDirection(this Vector2 v, Vector2 point) {
            return -v.x * point.y + v.y * point.x;
        }

        public static Vector2 GetWithMagnitude(this Vector2 v, float magnitude) {
            return v.normalized * magnitude;
        }

        public static Vector3 GetWithMagnitude(this Vector3 v, float magnitude) {
            return v.normalized * magnitude;
        }

        public static Vector3 MultiplyChannels(this Vector3 lhs, Vector3 rhs) {
            lhs.x *= rhs.x;
            lhs.y *= rhs.y;
            lhs.z *= rhs.z;
            return lhs;
        }

        public static Vector3 MultiplyChannels(this Vector3 lhs, Vector2 rhs) {
            lhs.x *= rhs.x;
            lhs.y *= rhs.y;
            return lhs;
        }

        public static Vector2 MultiplyChannels(this Vector2 lhs, Vector2 rhs) {
            lhs.x *= rhs.x;
            lhs.y *= rhs.y;
            return lhs;
        }

        public static Vector2 MultiplyChannels(this Vector2 lhs, float x, float y) {
            lhs.x *= x;
            lhs.y *= y;
            return lhs;
        }

        public static Vector3 MultiplyChannels(this Vector3 lhs, float x, float y) {
            lhs.x *= x;
            lhs.y *= y;
            return lhs;
        }

        public static Vector3 MultiplyChannels(this Vector3 lhs, float x, float y, float z) {
            lhs.x *= x;
            lhs.y *= y;
            lhs.z *= z;
            return lhs;
        }

        public static Vector2 DivideChannels(this Vector2 lhs, Vector2 rhs) {
            lhs.x /= rhs.x;
            lhs.y /= rhs.y;
            return lhs;
        }

        public static Vector2 ClampChannels(this Vector2 value, Vector2 min, Vector2 max) {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            return value;
        }

        public static Vector3 ClampChannels(this Vector3 value, Vector3 min, Vector3 max) {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
            return value;
        }

        public static Vector2 Abs(this Vector2 v) {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            return v;
        }

        public static bool Approximately(this Vector3 lhs, Vector3 rhs, float precision = 0.001f) {
            return (lhs - rhs).sqrMagnitude <= precision;
        }

        /// <summary>
        /// Gets the closest vector in a list of vectors to given point.
        /// </summary>
        public static Vector3 GetClosest(this List<Vector3> list, Vector3 point) {
            var closest = point;
            var minDistance = float.MaxValue;
            foreach (var position in list) {
                var distance = (point - position).sqrMagnitude;
                if (distance >= minDistance) continue;
                minDistance = distance;
                closest = position;
            }
            return closest;
        }

        /// <summary>
        /// Formula taken from:
        /// https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
        /// </summary>
        public static float DistanceFromLine(this Vector2 point, Vector2 pointOnLine1, Vector2 pointOnLine2) {
            var vector = pointOnLine2 - pointOnLine1;
            var lineLength = vector.magnitude;
            return Mathf.Abs(vector.y * point.x - vector.x * point.y + pointOnLine2.x * pointOnLine1.y - pointOnLine2.y * pointOnLine1.x)
                / lineLength;
        }

        /// <summary>
        /// A version of DistanceFromLine that does not divide by line length.
        /// It is faster and can be used to easily compare the distance of a point
        /// compared to the distance of another point.
        /// NOTE: The distance can be negative depending on the "side" the point
        /// is on.
        /// To get the actual distance, ABS the result and divide by line length.
        /// NOTE: The calculation is done in the XZ plane. Any Y values will be
        /// flatten to 0.
        /// </summary>
        public static float FastComparableDistanceFromLineXZ(this Vector3 point, Vector3 pointOnLine1, Vector3 pointOnLine2) {
            var vector = pointOnLine2 - pointOnLine1;
            return vector.z * point.x - vector.x * point.z + pointOnLine2.x * pointOnLine1.z - pointOnLine2.z * pointOnLine1.x;
        }
    }
}
