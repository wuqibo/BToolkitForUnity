using UnityEngine;
using System.Collections;

namespace BToolkit
{
    public class BMath
    {
        /// <summary>
        /// 获取贝塞尔曲线上的点
        /// </summary>
        public static Vector3 GetBezierPoint(Vector3 p1,Vector3 p2,Vector3 p3,float t)
        {
            //t范围0到1,对应的贝塞尔起点到终点;
            float x = CalculateQuadSpline(p1.x,p2.x,p3.x,t);
            float y = CalculateQuadSpline(p1.y,p2.y,p3.y,t);
            float z = CalculateQuadSpline(p1.z,p2.z,p3.z,t);
            return new Vector3(x,y,z);
        }
        /// <summary>
        /// 获取贝塞尔曲线上的点
        /// </summary>
        public static Vector2 GetBezierPoint(Vector2 p1,Vector2 p2,Vector2 p3,float t)
        {
            //t范围0到1,对应的贝塞尔起点到终点;
            float x = CalculateQuadSpline(p1.x,p2.x,p3.x,t);
            float y = CalculateQuadSpline(p1.y,p2.y,p3.y,t);
            return new Vector2(x,y);
        }
        /// <summary>
        /// 获取贝塞尔曲线上所有的点
        /// </summary>
        public static Vector3[] GetBezierPoints(Vector3 p1,Vector3 p2,Vector3 p3,int detailCount)
        {
            Vector3[] newVector2 = new Vector3[detailCount + 1];
            float tStep = 1 / ((float)detailCount);
            float t = 0f;
            for(int ik = 0;ik <= detailCount;ik++)
            {
                float x = CalculateQuadSpline(p1.x,p2.x,p3.x,t);
                float y = CalculateQuadSpline(p1.y,p2.y,p3.y,t);
                float z = CalculateQuadSpline(p1.z,p2.z,p3.z,t);
                newVector2[ik] = new Vector3(x,y,z);
                t = t + tStep;
            }
            return newVector2;
        }
        public static Vector2[] GetBezierPoints(Vector2 p1,Vector2 p2,Vector2 p3,int detailCount)
        {
            Vector2[] newVector2 = new Vector2[detailCount + 1];
            float tStep = 1 / ((float)detailCount);
            float t = 0f;
            for(int ik = 0;ik <= detailCount;ik++)
            {
                float x = CalculateQuadSpline(p1.x,p2.x,p3.x,t);
                float y = CalculateQuadSpline(p1.y,p2.y,p3.y,t);
                newVector2[ik] = new Vector2(x,y);
                t = t + tStep;
            }
            return newVector2;
        }
        static float CalculateQuadSpline(float z0,float z1,float z2,float t)
        {
            float a1 = (float)((1.0 - t) * (1.0 - t) * z0);
            float a2 = (float)(2.0 * t * (1 - t) * z1);
            float a3 = (float)(t * t * z2);
            float a4 = a1 + a2 + a3;
            return a4;
        }

        /// <summary>
        /// 绕任意轴任意角度旋转向量 
        /// </summary>
        public static Vector3 RotateRound(Vector3 position,Vector3 center,Vector3 axis,float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle,axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

        /// <summary>
        /// 直线方程
        /// </summary>
        public class Line
        {
            public float k, b;
            public Vector2 point1, point2;
            public float radian;
            public Line(Vector2 point1,Vector2 point2) : this(point1.x,point1.y,point2.x,point2.y) { }
            public Line(float x1,float y1,float x2,float y2)
            {
                this.point1 = new Vector2(x1,y1);
                this.point2 = new Vector2(x2,y2);
                k = (y2 - y1) / (x2 - x1);
                b = y1 - k * x1;
                radian = Mathf.Atan2(y2 - y1,x2 - x1);
            }
            public Line(Vector2 point1,float radian)
            {
                this.point1 = point1;
                this.point2.x = this.point1.x + Mathf.Cos(radian);
                this.point2.y = this.point1.y + Mathf.Sin(radian);
                k = (point2.y - point1.y) / (point2.x - point1.x);
                b = point1.y - k * point1.x;
                this.radian = radian;
            }
            public float GetX(float y)
            {
                return (y - b) / k;
            }
            public float GetY(float x)
            {
                return k * x + b;
            }
        }
        /// <summary>
        /// 获取两条直线的交点
        /// </summary>
        public static Vector2 GetCrossPointOf2Line(Line line1,Line line2)
        {
            float a = 0, b = 0;
            int state = 0;
            if(line1.point1.x != line1.point2.x)
            {
                a = (line1.point2.y - line1.point1.y) / (line1.point2.x - line1.point1.x);
                state |= 1;
            }
            if(line2.point1.x != line2.point2.x)
            {
                b = (line2.point2.y - line2.point1.y) / (line2.point2.x - line2.point1.x);
                state |= 2;
            }
            switch(state)
            {
                case 0://L1与L2都平行Y轴
                    {
                        if(line1.point1.x == line2.point1.x)
                        {
                            return new Vector2(0,0);
                        }
                        else
                        {
                            return new Vector2(0,0);
                        }
                    }
                case 1://L1存在斜率, L2平行Y轴
                    {
                        float x = line2.point1.x;
                        float y = (line1.point1.x - x) * (-a) + line1.point1.y;
                        return new Vector2(x,y);
                    }
                case 2://L1平行Y轴，L2存在斜率
                    {
                        float x = line1.point1.x;
                        float y = (line2.point1.x - x) * (-b) + line2.point1.y;
                        return new Vector2(x,y);
                    }
                case 3://L1，L2都存在斜率
                    {
                        if(a == b)
                        {
                            return new Vector2(0,0);
                        }
                        float x = (a * line1.point1.x - b * line2.point1.x - line1.point1.y + line2.point1.y) / (a - b);
                        float y = a * x - a * line1.point1.x + line1.point1.y;
                        return new Vector2(x,y);
                    }
            }
            return new Vector2(0,0);
        }

        /// <summary>
        /// 获取两点连线的中点
        /// </summary>
        public static Vector2 GetCenterBetween2Points(Vector2 point1,Vector2 point2)
        {
            return new Vector2((point1.x + point2.x) * 0.5f,(point1.y + point2.y) * 0.5f);
        }

        /// <summary>
        /// 按小数点后几位向下取
        /// </summary>
        public static string FloorToString(double value,int decimals = 2)
        {
            double pointOffset = System.Math.Pow(10,decimals);
            double a = value * pointOffset;
            int b = (int)a;
            double c = b / pointOffset;
            string format = "0.00";
            switch(decimals)
            {
                case 0:
                    format = "0";
                    break;
                case 1:
                    format = "0.0";
                    break;
                case 2:
                    format = "0.00";
                    break;
                case 3:
                    format = "0.000";
                    break;
                case 4:
                    format = "0.0000";
                    break;
            }
            return c.ToString(format);
        }
    }
}