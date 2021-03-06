﻿using ExtendCSharp.Interfaces;
using System;
using System.Drawing;

namespace ExtendCSharp.Services
{
    public class MathService:IService
    {
        public double RadToGrad(double Rad)
        {
            return Rad * (180 / Math.PI);
        }
        public double GradToRad(double Grad)
        {
            return (Math.PI / 180) * Grad;
        }

        /// <summary>
        /// calcola la differenza di angoli in senso orario (in gradi)
        /// </summary>
        /// <param name="FirstAngle"></param>
        /// <param name="SecondAngle"></param>
        /// <returns></returns>
        public double AngleDifNormalized(double FirstAngle, double SecondAngle)
        {
            double dif = SecondAngle - FirstAngle;
            while (dif < 0)
                dif += 360;
            return dif;
        }
        public float AngleDifNormalized(float FirstAngle, float SecondAngle)
        {
            return (float)AngleDifNormalized((double)FirstAngle, (double)SecondAngle);
        }


      


        public float NormalizeAngle(float Angle)
        {
            Angle= Angle % 360;
            if (Angle < 0)
                Angle += 360;
            return Angle;
        }


        public decimal Max(params decimal[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("Passare almeno un elemento");
            else if (values.Length == 1)
                return values[0];
            else
            {
                decimal? max = null;

                foreach (decimal v in values)
                {
                    if (max == null || v > max)
                        max = v;
                }

                return max.Value;

            }
        }
        public decimal Min(params decimal[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("Passare almeno un elemento");
            else if (values.Length == 1)
                return values[0];
            else
            {
                decimal? min = null;

                foreach (decimal v in values)
                {
                    if (min == null || v < min)
                        min = v;
                }

                return min.Value;

            }
        }


        public float Max(params float[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("Passare almeno un elemento");
            else if (values.Length == 1)
                return values[0];
            else
            {
                float? max = null;

                foreach (float v in values)
                {
                    if (max == null || v > max)
                        max = v;
                }

                return max.Value;

            }
        }
        public float Min(params float[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("Passare almeno un elemento");
            else if (values.Length == 1)
                return values[0];
            else
            {
                float? min = null;

                foreach (float v in values)
                {
                    if (min == null || v < min)
                        min = v;
                }

                return min.Value;

            }
        }


        /// <summary>
        /// Permette di trovare il centro di una circonferenza dati due punti sulla circonferenza ed il raggio
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public PointF CenterRadius(PointF p1, PointF p2, float radius)
        {
            double radsq = radius * radius;
            double q = Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
            double x3 = (p1.X + p2.X) / 2;
            double y3 = (p1.Y + p2.Y) / 2;

            x3 = x3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((p1.Y - p2.Y) / q);
            y3 = y3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((p2.X - p1.X) / q);


            return new PointF((float)x3, (float)y3);
        }





        public PointF Midpoint(Point p1, Point p2)
        {
            return new PointF((float)((p1.X + p2.X) / 2.0), (float)((p1.Y + p2.Y) / 2.0));
        }
        public PointF Midpoint(System.Windows.Point p1, System.Windows.Point p2)
        {
            return new PointF((float)((p1.X + p2.X) / 2.0), (float)((p1.Y + p2.Y) / 2.0));
        }

        public PointF Midpoint(PointF p1, PointF p2)
        {
            return new System.Drawing.PointF((float)((p1.X + p2.X) / 2.0), (float)((p1.Y + p2.Y) / 2.0));
        }
        public PointF Midpoint(params PointF[] plist)
        {
            double x = 0;
            double y = 0;

            plist.ForEach((PointF p) => { x += p.X; y += p.Y;  });
           
            x /= plist.Length;
            y /= plist.Length;

            return new PointF((float)x, (float)y);

        }



        public int map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
