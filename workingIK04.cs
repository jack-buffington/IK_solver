using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;



namespace WindowsFormsApplication1
   {
   public partial class Form1 : Form
      {
      int[,] clickPoints;
      int clickIndex;


      public bool mouseIsDown;
      public int clickedOn; // what was clicked on

      public struct point3D
         {
         public double X; // for cartesian
         public double Y;
         public double Z;
         public double radius; // for polar
         public double yaw;
         public double pitch;
         public double roll;
         }
      public struct obstacle
         {
         public double X;
         public double Y;
         public double Z;
         public double radius;
         public bool   collsionDetected;
         }
      public struct segment
         {
         public double length;
         public double radius;
         public double defaultRoll;  // all of the angles are relative to the previous joint, except for the root joint, which is relative to the coordinate system.
         public double defaultPitch; // this is the one that I am using for 2D.
         public double defaultYaw;
         public double tempDefaultRoll;
         public double tempDefaultPitch;
         public double tempDefaultYaw;
         public double roll;
         public double pitch;
         public double yaw;
         public double minYaw;
         public double maxYaw;
         public double minPitch;
         public double maxPitch;
         public double minRoll;
         public double maxRoll;
         public double rootX;
         public double rootY;
         public double rootZ;
         public double tipX;
         public double tipY;
         public double tipZ;
            // Collision stuff
         public double shortestCollisionDistance;
         public double collisionPercentage; // where a collision is relative to the splines
         public point3D obstacleAvoidanceDirection; // this is really being used as a vector
         //public double directionX;  // direction to move in case of collision
         //public double directionY;
         //public double directionZ;
         public bool hadCollision;
         }

      point3D[] adjustedJointLocations;

      public point3D[] splinePoints;
      public int numberOfSplines;

      public segment[] segments;

      public obstacle[] obstacles;

      public int numberOfSegments;
      public int numberOfObstacles;
      


      public Form1()
         {
         
         InitializeComponent();

         numberOfSplines = 2;
         splinePoints = new point3D[5];

         splinePoints[0].X = 10; // set up some initial data
         splinePoints[0].Y = 200;
         splinePoints[1].X = 150;
         splinePoints[1].Y = 100;
         splinePoints[2].X = 250;
         splinePoints[2].Y = 200;
         splinePoints[3].X = 350;
         splinePoints[3].Y = 300;
         splinePoints[4].X = 450;
         splinePoints[4].Y = 200;

         mouseIsDown = false;

         clickPoints = new int[20, 2];
         for (int I = 0; I < 20; I++)
            {
            clickPoints[I, 0] = 0;
            clickPoints[I, 1] = 0;
            }

         adjustedJointLocations = new point3D[20];
         
         for (int I = 0; I < 20; I++)
            adjustedJointLocations[I] = giveMeAnEmptyPoint3D();


         clickIndex = 0;
         numberOfObstacles = 5;
         obstacles = new obstacle[numberOfObstacles];    
         randomizeObstacles();

         segments = new segment[19];
         numberOfSegments = 0;

         }
      private void randomizeObstacles()
         {
         //int numberOfObstacles = obstacles.GetLength(0);
         //Debug.WriteLine(numberOfObstacles.ToString());

         Random myRand = new Random();


         for (int I = 0; I < numberOfObstacles; I++)
            {
            obstacles[I].X = (int)(myRand.NextDouble() * (double)drawingArea.Width);
            obstacles[I].Y = (int)(myRand.NextDouble() * (double)drawingArea.Height);
            obstacles[I].radius = (float)myRand.NextDouble() * 25 + 14;
            string mystring = "X :" + obstacles[I].X + "  Y:" + obstacles[I].Y;
            Debug.WriteLine(mystring);
            }

         detectCollisions();
         drawPicture();

         }
      private void drawPicture()
         {
         // draws everything and calculates the base and tip positions of the segments. 

         // clear the drawing area
         Graphics g = drawingArea.CreateGraphics();

         Bitmap myBitmap = new Bitmap(this.Width, this.Height);
         Graphics gdb = Graphics.FromImage(myBitmap);
         gdb.Clear(Color.White);

         SolidBrush redBrush = new SolidBrush(Color.Red);

         Pen p = new Pen(Color.Gray, 1);
         Pen pr = new Pen(Color.Red, 2);

         // Draw the obstacles
         if(showObstaclesCheckBox.Checked == true)
            {
            for (int I = 0; I < numberOfObstacles; I++)
               {
               // draw the obstacles as circles 
               int x = (int)obstacles[I].X;
               int y = (int)obstacles[I].Y;
               float rad = (float)obstacles[I].radius;
               if (obstacles[I].collsionDetected == true)
                  gdb.FillEllipse(redBrush, x - rad, y - rad, rad * 2, rad * 2);
               else
                  gdb.DrawEllipse(pr, x - rad, y - rad, rad * 2, rad * 2);

               }
            }


         // draw just the first click point
         gdb.DrawRectangle(p, clickPoints[0, 0] - 1, clickPoints[0, 1] - 1, 2, 2);

         //// draw the otherclick points
         if (showClickPointsCheckBox.Checked == true)
            {
            for (int I = 1; I < clickIndex; I++)
               {
               int X = clickPoints[I, 0];
               int Y = clickPoints[I, 1];
               gdb.DrawRectangle(p, X - 1, Y - 1, 2, 2);
               }
            }

         if (showAdjustedJointLocationsCheckBox.Checked == true)
            {
            // draw the spline adjusted joint locations
            for (int I = 0; I < numberOfSegments; I++)
               {
               int X = (int)adjustedJointLocations[I].X;
               int Y = (int)adjustedJointLocations[I].Y;
               gdb.DrawRectangle(pr, X - 1, Y - 1, 2, 2);
               }
            }

         // draw the bones using the data from segments.
         point3D lastPoint;
         lastPoint.X = clickPoints[0,0];
         lastPoint.Y = clickPoints[0, 1];
         lastPoint.Z = 0;


         double segmentAngle = 0;
         for (int I = 0; I < numberOfSegments; I++)
            {
            double startX = segments[I].rootX;
            double startY = segments[I].rootY;
            double startZ = segments[I].rootZ;

            double segmentLength = segments[I].length;
            segmentAngle += segments[I].pitch;  // this is the global angle for the segment

            // create the points representing the bone.
            const double boneLength = 8;
            double boneScale = segmentLength / boneLength; 
            point3D point1, point2, point3, point4; // bone points
            point3D point5, point6, point7, point8; // segment edges


            // the lines that indicate the bone
            point1.X = 0;
            point1.Y = 0;
            point2.radius = 1.4f * boneScale;
            point2.pitch = Math.PI / 4 + segmentAngle;
            point3.radius = 1.4f * boneScale;
            point3.pitch = -Math.PI / 4 + segmentAngle;
            point4.radius = boneLength * boneScale;
            point4.pitch = segmentAngle;

            // the parallel lines that indicate the radius of the arm
            point5.pitch = Math.PI / 2 + segmentAngle;
            point5.radius = segments[I].radius;
            double parallelAngle = Math.Atan2(segments[I].radius, boneLength*boneScale) ;
            double boneLengthSquared = boneLength * boneLength * boneScale * boneScale;
            double parallelRadius = Math.Sqrt(segments[I].radius * segments[I].radius + boneLengthSquared);
            point6.pitch = parallelAngle + segmentAngle;
            point6.radius = parallelRadius;
            point7.pitch = -Math.PI / 2 + segmentAngle;
            point7.radius = segments[I].radius;
            point8.pitch = -parallelAngle + segmentAngle;
            point8.radius = parallelRadius;


            // They are now rotated and scaled appropriately so calculate their  X & Y coordinates

            point2.X = point2.radius * Math.Cos(point2.pitch);
            point2.Y = point2.radius * Math.Sin(point2.pitch);
            point3.X = point3.radius * Math.Cos(point3.pitch);
            point3.Y = point3.radius * Math.Sin(point3.pitch);
            point4.X = point4.radius * Math.Cos(point4.pitch);
            point4.Y = point4.radius * Math.Sin(point4.pitch);

            point5.X = point5.radius * Math.Cos(point5.pitch);
            point5.Y = point5.radius * Math.Sin(point5.pitch);
            point6.X = point6.radius * Math.Cos(point6.pitch);
            point6.Y = point6.radius * Math.Sin(point6.pitch);
            point7.X = point7.radius * Math.Cos(point7.pitch);
            point7.Y = point7.radius * Math.Sin(point7.pitch);
            point8.X = point8.radius * Math.Cos(point8.pitch);
            point8.Y = point8.radius * Math.Sin(point8.pitch);

            // offset the points so that they start at the end of the previous segment
            segments[I].rootX = lastPoint.X;
            segments[I].rootY = lastPoint.Y;
            segments[I].rootZ = lastPoint.Z;

            point1.X += lastPoint.X;
            point1.Y += lastPoint.Y;
            point2.X += lastPoint.X;
            point2.Y += lastPoint.Y;
            point3.X += lastPoint.X;
            point3.Y += lastPoint.Y;
            point4.X += lastPoint.X;
            point4.Y += lastPoint.Y;

            point5.X += lastPoint.X;
            point5.Y += lastPoint.Y;
            point6.X += lastPoint.X;
            point6.Y += lastPoint.Y;
            point7.X += lastPoint.X;
            point7.Y += lastPoint.Y;
            point8.X += lastPoint.X;
            point8.Y += lastPoint.Y;

            lastPoint.X = point4.X;
            lastPoint.Y = point4.Y;

            segments[I].tipX = point4.X;
            segments[I].tipY = point4.Y;
            segments[I].tipZ = 0;

            // draw the bone
            gdb.DrawLine(p, (int)point1.X, (int)point1.Y, (int)point2.X, (int)point2.Y);
            gdb.DrawLine(p, (int)point1.X, (int)point1.Y, (int)point3.X, (int)point3.Y);
            gdb.DrawLine(p, (int)point1.X, (int)point1.Y, (int)point4.X, (int)point4.Y);

            gdb.DrawLine(p, (int)point2.X, (int)point2.Y, (int)point3.X, (int)point3.Y);
            gdb.DrawLine(p, (int)point2.X, (int)point2.Y, (int)point4.X, (int)point4.Y);

            gdb.DrawLine(p, (int)point3.X, (int)point3.Y, (int)point4.X, (int)point4.Y);

            //draw the parallel lines
            gdb.DrawLine(pr, (int)point5.X, (int)point5.Y, (int)point6.X, (int)point6.Y);
            gdb.DrawLine(pr, (int)point7.X, (int)point7.Y, (int)point8.X, (int)point8.Y);
            }

         
         if (numberOfSegments > 1)
            {
            drawSpline(splinePoints[0], splinePoints[1], splinePoints[2], gdb);
            drawSpline(splinePoints[2], splinePoints[3], splinePoints[4], gdb);
            }




         g.DrawImage(myBitmap, 0, 0);
         g.Dispose();
         gdb.Dispose();
         myBitmap.Dispose();
         }
      private void Form1_Paint(object sender, PaintEventArgs e)
         {// makes it so that it redraws if another program is moved over top of it.  
          // doesn't deal with if context is switched back to this
         drawPicture();
         }
      private void timer1_Tick(object sender, EventArgs e)
         {// make it draw at startup
         drawPicture();
         timer1.Enabled = false;
         }
      private void randomizeObstaclesClick(object sender, EventArgs e)
         {
         randomizeObstacles();
         }
      private void drawingArea_MouseDown(object sender, MouseEventArgs e)
         {
         // calculate the distance to all of the clickable points
         double clickDistance = 20;
         // determine what, if anything was clicked on.   
         //double dist1 = findDistance(e.X, e.Y, segments[numberOfSegments - 1].tipX, segments[numberOfSegments - 1].tipY);

         double dist1 = findDistance(e.X, e.Y, splinePoints[4].X, splinePoints[4].Y);
         double dist2 = findDistance(e.X, e.Y, splinePoints[1].X, splinePoints[1].Y);
         double dist3 = findDistance(e.X, e.Y, splinePoints[3].X, splinePoints[3].Y);



         if (dist1 < clickDistance) // end of chain
            {
            mouseIsDown = true;
            clickedOn = 1;
            }
         else if (dist2 < clickDistance) // spline point closer to root
            {
            mouseIsDown = true;
            clickedOn = 2;
            }
         else if (dist3 < clickDistance) // spline point clower to tip
            {
            mouseIsDown = true;
            clickedOn = 3;
            }
         else // create a new segment
            {
            clickPoints[clickIndex, 0] = e.X;
            clickPoints[clickIndex, 1] = e.Y;
            clickIndex++;

            // calculate some information about the segments so that it can be used for IK purposes.  

            if (clickIndex >= 2)
               {
               float deltaX = clickPoints[clickIndex - 1, 0] - clickPoints[clickIndex - 2, 0];
               float deltaY = clickPoints[clickIndex - 1, 1] - clickPoints[clickIndex - 2, 1];

               segments[numberOfSegments].length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
               segments[numberOfSegments].radius = 10f;


               segments[numberOfSegments].defaultRoll = 0;
               segments[numberOfSegments].defaultYaw = 0;

               double thisAngle = Math.Atan2(deltaY, deltaX);

               if (clickIndex == 2)
                  {// the first joint has nothing to reference so just make its reference be zero.
                  segments[numberOfSegments].defaultPitch = thisAngle;
                  }
               else
                  {// all subsequent segments reference the previous segment
                  // Find the angle of this segment  Angles are as follows:
                  // Segments pointing to the right are 0
                  // segments going upwards are negative 0->negative pi
                  // segments going downward are positive 0->pi



                  deltaX = clickPoints[clickIndex - 2, 0] - clickPoints[clickIndex - 3, 0];
                  deltaY = clickPoints[clickIndex - 2, 1] - clickPoints[clickIndex - 3, 1];
                  double lastAngle = Math.Atan2(deltaY, deltaX);
                  segments[numberOfSegments].defaultPitch = (float)(thisAngle - lastAngle);

                  if (segments[numberOfSegments].defaultPitch > Math.PI)
                     segments[numberOfSegments].defaultPitch -= (float)(2 * Math.PI);
                  else if (segments[numberOfSegments].defaultPitch < -Math.PI)
                     segments[numberOfSegments].defaultPitch += (float)(2 * Math.PI);

                  }

               segments[numberOfSegments].roll = segments[numberOfSegments].defaultRoll;
               segments[numberOfSegments].pitch = segments[numberOfSegments].defaultPitch;
               segments[numberOfSegments].yaw = segments[numberOfSegments].defaultYaw;

               segments[numberOfSegments].rootX = clickPoints[numberOfSegments, 0];
               segments[numberOfSegments].rootY = clickPoints[numberOfSegments, 1];
               segments[numberOfSegments].rootZ = 0;




               numberOfSegments++;
               evaluateChain();
               detectCollisions();
               // set up the spline handles 
               splinePoints[0].X = segments[0].rootX;
               splinePoints[0].Y = segments[0].rootY;
               splinePoints[0].Z = segments[0].rootZ;
               splinePoints[4].X = segments[numberOfSegments - 1].tipX;
               splinePoints[4].Y = segments[numberOfSegments - 1].tipY;
               splinePoints[4].Z = segments[numberOfSegments - 1].tipZ;
               splinePoints[2] = findMiddle(splinePoints[0], splinePoints[4]);
               splinePoints[1] = findMiddle(splinePoints[0], splinePoints[2]);
               splinePoints[3] = findMiddle(splinePoints[2], splinePoints[4]);

               }// end of if clickIndex > 2
            drawPicture();
            }
   
         
         }
      private double findDistance(double X1, double Y1, double X2, double Y2)
         {
         double deltaX = X1 - X2;
         double deltaY = Y1 - Y2;

         return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
         }
      private void drawingArea_MouseUp(object sender, MouseEventArgs e)
         {
         mouseIsDown = false;
         }   
      private void drawingArea_MouseMove(object sender, MouseEventArgs e)
         {
         if (mouseIsDown)
            {
            if(clickedOn == 1) // the end effector of the chain
               {// then move the chain around.  
               // find the distance to the goal from the root.  
               point3D targetPoint = giveMeAnEmptyPoint3D();
               targetPoint.X = e.X;
               targetPoint.Y = e.Y;

               stretchAndOrientSegments(targetPoint,false);
               orientSegmentsAvoidingObstacles(targetPoint);

               

               splinePoints[0].X = segments[0].rootX;
               splinePoints[0].Y = segments[0].rootY;
               splinePoints[0].Z = segments[0].rootZ;
               splinePoints[4].X = segments[numberOfSegments - 1].tipX;
               splinePoints[4].Y = segments[numberOfSegments - 1].tipY;
               splinePoints[4].Z = segments[numberOfSegments - 1].tipZ;
               splinePoints[2] = findMiddle(splinePoints[0], splinePoints[4]);
               splinePoints[1] = findMiddle(splinePoints[0], splinePoints[2]);
               splinePoints[3] = findMiddle(splinePoints[2], splinePoints[4]);

               bool collisionDetected = detectCollisions();
               if(avoidObstaclesCheckBox.Checked == true)
                  avoidObstacles();


               }// end of if the tip of the chain is being clicked on
            else if (clickedOn == 2) // The control point closest to the root
               { // the first spline point
               splinePoints[1].X = e.X;
               splinePoints[1].Y = e.Y;
               splinePoints[2] = findMiddle(splinePoints[1], splinePoints[3]);
               point3D tipPosition = giveMeAnEmptyPoint3D();
               tipPosition.X = segments[numberOfSegments - 1].tipX;
               tipPosition.Y = segments[numberOfSegments - 1].tipY;
               tipPosition.Z = segments[numberOfSegments - 1].tipZ;

               stretchAndOrientSegmentsUsingSplines(tipPosition);
               detectCollisions();
               }
            else if (clickedOn == 3) // The control point closest to the end effector
               {// the second spline point
               splinePoints[3].X = e.X;
               splinePoints[3].Y = e.Y;
               splinePoints[2] = findMiddle(splinePoints[1], splinePoints[3]);
               point3D tipPosition = giveMeAnEmptyPoint3D();
               tipPosition.X = segments[numberOfSegments - 1].tipX;
               tipPosition.Y = segments[numberOfSegments - 1].tipY;
               tipPosition.Z = segments[numberOfSegments - 1].tipZ;

               stretchAndOrientSegmentsUsingSplines(tipPosition);
               detectCollisions();
               
               }

            double splineLength = findSplineLength();
            label3.Text = "Spline length: " + splineLength.ToString();
            drawPicture();
            } // of if mouseIsDown
         }// end of mouseMove
      private point3D findMiddle(point3D p1, point3D p2)
         {// finds the point 1/2 way between the two points
         point3D p;
         p.X = p1.X + (p2.X - p1.X) * .5f;
         p.Y = p1.Y + (p2.Y - p1.Y) * .5f;
         p.Z = p1.Z + (p2.Z - p1.Z) * .5f;
         p.pitch = 0;
         p.radius = 0;
         p.roll = 0;
         p.yaw = 0;
         return p;
         }
      private void evaluateChain()
         {// looks at all of the segments' angles and determines where the start and end of every segment is.
          // It stores this back in the segments array

         point3D lastPoint;
         lastPoint.X = clickPoints[0, 0];
         lastPoint.Y = clickPoints[0, 1];
         lastPoint.Z = 0;

         double chainPitch = 0;


         for (int I = 0; I < numberOfSegments; I++)
            {
            segments[I].rootX = lastPoint.X;
            segments[I].rootY = lastPoint.Y;
            segments[I].rootZ = lastPoint.Z;

            double segmentLength = segments[I].length;
            chainPitch += segments[I].pitch;

            // rotate and translate the tip
            segments[I].tipX = segmentLength * Math.Cos(chainPitch) + segments[I].rootX;
            segments[I].tipY = segmentLength * Math.Sin(chainPitch) + segments[I].rootY;
            segments[I].tipZ = 0;

            lastPoint.X = segments[I].tipX;
            lastPoint.Y = segments[I].tipY;
            lastPoint.Z = segments[I].tipZ;
            }
         }
      private double findChainLength(int startSegment, int endSegment)
         {// finds the distance from the root of the start segment to the tip of the end segment
         // I broke this out because there are cases where you want to find the distance between segments that are something 
         // other than the length of the whole chain. 

         double rootX = segments[startSegment].rootX;
         double rootY = segments[startSegment].rootY;
         double rootZ = segments[startSegment].rootZ;
         double tipX = segments[endSegment].tipX;
         double tipY = segments[endSegment].tipY;
         double tipZ = segments[endSegment].tipZ;



         double deltaX = rootX - tipX;
         double deltaY = rootY - tipY;
         double deltaZ = rootZ - tipZ;

         double chainLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

         return chainLength;
         }
      private void removeChainButtonClick(object sender, EventArgs e)
         {
         numberOfSegments = 0;
         clickIndex = 0;

         drawPicture();
         }
      private bool detectCollisions()
         {// returns true if a collision is detected.
         // This code uses the strategy where it splits the arm segments up into multiple points and then checks 
         // for intersections by comparing the armRadius + obstacleRadius to the distance from the arm point to the 
         // obstacle center.
         // This function also stores some information about the collision in the obstacles.
         // Specifically, it stores the percent along the chain the collision is and what direction
         // that segment should move in order to get out of the collision.  
         bool foundCollision = false;
         int numberOfObstacles = obstacles.GetLength(0);

         // mark all obstacles as not having a collision
         for (int I = 0; I < numberOfObstacles; I++)
            {
            obstacles[I].collsionDetected = false;
            }

         // mark all segments as not having a collision
         for (int I = 0; I < numberOfSegments; I++)
            {
            segments[I].hadCollision = false;
            segments[I].shortestCollisionDistance = 100000;
            }

         // go through all segments and see if they collide
         for (int I = 0; I < numberOfSegments; I++)
            {
            //if (foundCollision)
            //   break;
            point3D segmentStart, segmentEnd;
            segmentStart.X = segments[I].rootX;
            segmentStart.Y = segments[I].rootY;
            segmentStart.Z = segments[I].rootZ;
            segmentEnd.X = segments[I].tipX;
            segmentEnd.Y = segments[I].tipY;
            segmentEnd.Z = segments[I].tipZ;

            // figure out how many points to divide the segment into based on the radius. If I put points spaced at the same 
            // distance as the segment radius then I have a potential error of 14% if a sharp obstacle were to come into contact
            // with the arm.  This drops to 2% if I space them at 1/2 of the arm radius.  

            int numberOfPoints = (int)Math.Ceiling(segments[I].length / (segments[I].radius / 2));
            //double shortestDistance = 10000;
            //double whichObstacle = -1;
            //point3D whichPoint;

            for (int J = 0; J < numberOfPoints; J++)
               {
               // find the position of this current point
               point3D currentPoint;
               currentPoint.X = segmentStart.X + ((double)J/numberOfPoints) * (segmentEnd.X - segmentStart.X);
               currentPoint.Y = segmentStart.Y + ((double)J/numberOfPoints) * (segmentEnd.Y - segmentStart.Y);
               currentPoint.Z = segmentStart.Z + ((double)J/numberOfPoints) * (segmentEnd.Z - segmentStart.Z);

               for (int K = 0; K < numberOfObstacles; K++)
                  {
                  double minimumSpacing = segments[I].radius + obstacles[K].radius;
                  double deltaX = currentPoint.X - obstacles[K].X;
                  double deltaY = currentPoint.Y - obstacles[K].Y;
                  double deltaZ = currentPoint.Z - obstacles[K].Z;

                  // actualSpacing is the distance between the point on the segment and the obstacle
                  double actualSpacing = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                  if (actualSpacing <= minimumSpacing)
                     {
                     foundCollision = true;
                     obstacles[K].collsionDetected = true;
                     segments[I].hadCollision = true;
                     if (segments[I].shortestCollisionDistance > actualSpacing)
                        {
                        segments[I].shortestCollisionDistance = actualSpacing;

                        // figure out what percent along the spline this collsion is
                        segments[I].collisionPercentage = findSplinePercentage02(obstacles[K]);
                        // figure out what direction the segment should move
                        segments[I].obstacleAvoidanceDirection = findAvoidanceDirection(segments[I], obstacles[K]);
                        }// end of if this was a shorter distance for the collision
                     }// end of if a detection was found
                  }// end of going through all of the obstacles
               }// end of going through all of the sub-points in the current segment
            }// end of going through all of the segments
         return foundCollision;
         } // end of detectCollisions
      private void drawSpline(point3D p1, point3D p2, point3D p3, Graphics g)
         {// draws just one section of a degree 2 spline
         point3D lastPoint = p1;
         point3D pointA, pointB, pointC; // these are the interpolated points
         Pen p = new Pen(Color.Aquamarine, 2);
         SolidBrush b = new SolidBrush(Color.Blue);

         // Draw the control points
         if (showSplineHandlesCheckBox.Checked == true)
            { 
            g.FillEllipse(b, (float)p2.X - 4, (float)p2.Y - 4, 8, 8);
            }
         if (showSplineCheckBox.Checked == true)
            {
            g.FillEllipse(b, (float)p3.X - 2, (float)p3.Y - 2, 4, 4);
            g.FillEllipse(b, (float)p1.X - 2, (float)p1.Y - 2, 4, 4);
            for (int I = 1; I < 11; I++)
               {// draw the spline in 10 segments
               double percentage = (double)I / 10;
               pointA = interpolateBetweenPoints(p1, p2, percentage);
               pointB = interpolateBetweenPoints(p2, p3, percentage);
               pointC = interpolateBetweenPoints(pointA, pointB, percentage);

               // draw this segment of the spline
               g.DrawLine(p, (float)lastPoint.X, (float)lastPoint.Y, (float)pointC.X, (float)pointC.Y);
               lastPoint = pointC;
               }
            }
         }
      private double findSplineLength()
         {// figures out the length of the spline.  This includes the entire spline (all segments)
         double splineLength = 0;
         for (int I = 0; I < numberOfSplines; I++)
            {
            point3D p1,p2,p3;
            p1 = splinePoints[I * 2];
            p2 = splinePoints[I * 2 + 1];
            p3 = splinePoints[I * 2 + 2];

            point3D lastPoint = p1;
            point3D pointA, pointB, pointC; // these are the interpolated points

            for (int J = 1; J < 11; J++)
               {// figure out the length based on 10 segments
               double percentage = (double)J / 10;
               pointA = interpolateBetweenPoints(p1, p2, percentage);
               pointB = interpolateBetweenPoints(p2, p3, percentage);
               pointC = interpolateBetweenPoints(pointA, pointB, percentage);


               double deltaX = pointC.X - lastPoint.X;
               double deltaY = pointC.Y - lastPoint.Y;
               double deltaZ = pointC.Z - lastPoint.Z;

               splineLength += Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

               lastPoint = pointC;
               }
            }
         return splineLength;
         }
      private point3D interpolateBetweenPoints(point3D p1, point3D p2, double percentage)
         {
         point3D interpolatedPoint;
         interpolatedPoint.X = p1.X + (p2.X - p1.X) * percentage;
         interpolatedPoint.Y = p1.Y + (p2.Y - p1.Y) * percentage;
         interpolatedPoint.Z = p1.Z + (p2.Z - p1.Z) * percentage;
         interpolatedPoint.pitch = 0;
         interpolatedPoint.roll = 0;
         interpolatedPoint.yaw = 0;
         interpolatedPoint.radius = 0;
         return interpolatedPoint;
         }
      private void stretchAndOrientSegments(point3D targetPoint, bool useTempAngles)
         {// Basically does what its name suggests.  It makes the spline be the requested 
          //length and makes the angle from root to tip be the requested angle.

         double rootX = segments[0].rootX;
         double rootY = segments[0].rootY;
         double deltaX = rootX - targetPoint.X;
         double deltaY = rootY - targetPoint.Y;
         double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
         double goalAngle = Math.Atan2(deltaY, deltaX);

         // reset the chain to its default position because that is where things are being adjusted from
         for (int I = 0; I < numberOfSegments; I++)
            {
            if (useTempAngles)
               {
               segments[I].pitch = segments[I].tempDefaultPitch;
               segments[I].roll = segments[I].tempDefaultRoll;
               segments[I].yaw = segments[I].tempDefaultYaw;
               }
            else
               {
               segments[I].pitch = segments[I].defaultPitch;
               segments[I].roll = segments[I].defaultRoll;
               segments[I].yaw = segments[I].defaultYaw;
               }
            }                                

         evaluateChain();

               

         // do a binary search to find the proper joint angles to go that distance
         double error = 1000;

         double adjustmentAmount = .5; // how much of the available range to adjust by
         double adjustment = 0;     // how the angles are adjusted



         while (Math.Abs(error) > 1 && adjustmentAmount > .00001)
            {
            evaluateChain();
            double chainLength = findChainLength(0, numberOfSegments - 1);
            if (chainLength > length)
               adjustment -= adjustmentAmount;
            else
               adjustment += adjustmentAmount;

            // now adjust the length of the chain

            // calculate the new joint angles.
            for (int I = 0; I < numberOfSegments; I++)
               {
               double defaultAngle;
               if (useTempAngles)
                  defaultAngle = segments[I].tempDefaultPitch;
               else
                  defaultAngle = segments[I].defaultPitch;
               

               if (adjustment > 0) // the chain needs to get longer
                  segments[I].pitch = (1 - adjustment) * defaultAngle;

               else
                  {// the chain needs to get shorter
                  double range;
                  if (defaultAngle > 0)
                     range = Math.PI - defaultAngle;
                  else
                     range = -Math.PI - defaultAngle;

                  segments[I].pitch = defaultAngle - range * adjustment;
                  }// end of if the chain needs to get shorter
               }// end of calculating the new joint angles for this iteration
            adjustmentAmount /= 2;
            }// end of while the error is greater than 1 and the adjustmentAmount is too high

               



         // Now it needs to adjust the angle of the root joint to make the tip be in the correct position

               

         //double rootX = segments[0].rootX;
         //double rootY = segments[0].rootY;

         deltaX = rootX - segments[numberOfSegments - 1].tipX;
         deltaY = rootY - segments[numberOfSegments - 1].tipY;

         double actualAngle = Math.Atan2(deltaY, deltaX);

         segments[0].pitch += goalAngle - actualAngle;

         evaluateChain();

         }// end of stretchAndOrientSpline
      private point3D giveMeAnEmptyPoint3D()
         {// fills everything in with zero so that I don't have to manually fill things in when passing to a function.  
         // The compiler tells me that the value is unassigned....
         point3D retVal;
         retVal.pitch = 0;
         retVal.radius = 0;
         retVal.roll = 0;
         retVal.X = 0;
         retVal.Y = 0;
         retVal.yaw = 0;
         retVal.Z = 0;
         return retVal;
         }
      private void stretchAndOrientSegmentsUsingSplines(point3D targetPoint)
         {
         double splineLength = findSplineLength();

         // stretch the chain to meet the spline length and orient it at 0 degrees
         point3D tempTarget = giveMeAnEmptyPoint3D();
         tempTarget.X = findSplineLength() + segments[0].rootX;
         tempTarget.Y = segments[0].rootY;
         stretchAndOrientSegments(tempTarget, false);

         // for each joint, record where it is relative to the cartesian coordinates
         double[] yOffsets = new double[numberOfSegments];
         double[] percentsAlongTheSpline = new double[numberOfSegments];

         yOffsets[numberOfSegments - 1] = 0;
         percentsAlongTheSpline[numberOfSegments - 1] = .999;

         for (int I = 0; I < numberOfSegments - 1; I++)
            {
            yOffsets[I] = (segments[I].tipY - segments[0].rootY);                          // Offset from spline is in actual units
            double xPos = Math.Abs(segments[I].tipX - segments[0].rootX);
            percentsAlongTheSpline[I] = xPos / splineLength;         // The percentAlongSpline is always 0->1 no matter how many 
            // splines there are in total
            }



         // Now find where those points are relative to the bent spline.
         for (int I = 0; I < numberOfSegments; I++)
            adjustedJointLocations[I] = findCurvedSplineJointLocation(percentsAlongTheSpline[I], yOffsets[I]);



   
         // for each joint, figure out where those offset points are relative to the curved spline.
         // for each joint successively, make them point towards their respective offset point.  
         for(int I = 0; I < numberOfSegments; I++)
            {

            point3D idealPoint = adjustedJointLocations[I];
            // Make the segment point at the point 
            // find the global angle for the segment
            double deltaX = idealPoint.X - segments[I].rootX;
            double deltaY = idealPoint.Y - segments[I].rootY;

            double globalAngle = Math.Atan2(deltaY,deltaX);

            
            if(I==0)
               {// this is the first segment so just apply the angle
               segments[0].tempDefaultPitch = globalAngle;
               }
            else
               {// adjust that angle to be relative to the previous joint.  
                // To do that, find the global angle of the previous point...
               deltaX = segments[I-1].tipX - segments[I-1].rootX;
               deltaY = segments[I-1].tipY - segments[I-1].rootY;
               double previousAngle = Math.Atan2(deltaY,deltaX);
               segments[I].tempDefaultPitch = globalAngle - previousAngle;
               }

            // Update the tip location of the joint and the root location of the next joint
            segments[I].tipX = segments[I].length * Math.Cos(globalAngle) + segments[I].rootX;
            segments[I].tipY = segments[I].length * Math.Sin(globalAngle) + segments[I].rootY;

            if (I != numberOfSegments - 1)
               {
               segments[I + 1].rootX = segments[I].tipX;
               segments[I + 1].rootY = segments[I].tipY;
               }
            

            }

         // Consider the new angles to be the temporary default angles and stretch/orient the chain to meet the last spline point.
         stretchAndOrientSegments(targetPoint,true);
          
         }
      private point3D findCurvedSplineJointLocation(double percent, double offset)
         {// Given the two bits of data, returns the 2D location that is the same offset from the spline
         // TODO:  This will need to be modified for 3D use so that it offsets in the direction opposite of the greatest curvature
         //  This could possibly be done by forming a plane out of pointOnSpline, higherPoint, and lowerPoint and then putting the 
         // offset point on that plane.  
         // TODO: need to change the code that rotates the offset point

         if (percent > .99) // sometimes it gets values slightly above 1
            percent = .99;
         point3D thePoint = giveMeAnEmptyPoint3D(); 

         // find the location of the point (on the spline) that is the given percent along the spline 
         // figure out what percentage each spline represents
         double splinePercentage = 1.0f / numberOfSplines;
         int whichSpline = (int)Math.Floor(percent / splinePercentage);

         point3D p1 = splinePoints[whichSpline * 2];
         point3D p2 = splinePoints[whichSpline * 2 + 1];
         point3D p3 = splinePoints[whichSpline * 2 + 2];

         //double adjustedSplinePercentage = percent % splinePercentage;
         double adjustedSplinePercentage = percent - whichSpline * splinePercentage;
         adjustedSplinePercentage /= splinePercentage;

         point3D pointA = interpolateBetweenPoints(p1, p2, adjustedSplinePercentage);
         point3D pointB = interpolateBetweenPoints(p2, p3, adjustedSplinePercentage);
         point3D pointOnSpline = interpolateBetweenPoints(pointA, pointB, adjustedSplinePercentage); // this is the point on the spline



         // Find the slope of the spline at that point
         // I'll find it by finding the slope between two points just on either side of that point.
         // This *SHOULD* work if I overrun the ends of the spline
         pointA = interpolateBetweenPoints(p1, p2, adjustedSplinePercentage + .01);
         pointB = interpolateBetweenPoints(p2, p3, adjustedSplinePercentage + .01);
         point3D higherPoint = interpolateBetweenPoints(pointA, pointB, adjustedSplinePercentage + .01);
 
         pointA = interpolateBetweenPoints(p1, p2, adjustedSplinePercentage - .01);
         pointB = interpolateBetweenPoints(p2, p3, adjustedSplinePercentage - .01);
         point3D lowerPoint = interpolateBetweenPoints(pointA, pointB, adjustedSplinePercentage - .01);

         double deltaX = higherPoint.X - lowerPoint.X;
         double deltaY = higherPoint.Y - lowerPoint.Y;

         double splineDirection = Math.Atan2(deltaY, deltaX);


         // find the perpendicular direction
         // TODO: This direction might be backwards
         double perpendicularDirection = splineDirection + Math.PI / 2;
         
         // move the requested offset from the spline along the perpendicular slope
         // rotate the point
         point3D offsetPoint = giveMeAnEmptyPoint3D();
         offsetPoint.X = offset * Math.Cos(perpendicularDirection);
         offsetPoint.Y = offset * Math.Sin(perpendicularDirection);
         // Now add in the pointOnSpline

         thePoint.X = offsetPoint.X + pointOnSpline.X;
         thePoint.Y = offsetPoint.Y + pointOnSpline.Y;
         thePoint.Z = offsetPoint.Z + pointOnSpline.Z;



         return thePoint;
         }
      private point3D checkForSegmentCollision(segment s)
         { // Checks a single segment for collisions with all of the obstacles
           // Returns a point3D that has radius = 0 if no collision is found
           // If a collision is found then it returns the amount of overlap and the direction that it needs to go to not be overlapping
         point3D p = giveMeAnEmptyPoint3D();
         //bool foundCollision = false;

         point3D segmentStart, segmentEnd;
         segmentStart.X = s.rootX;
         segmentStart.Y = s.rootY;
         segmentStart.Z = s.rootZ;
         segmentEnd.X = s.tipX;
         segmentEnd.Y = s.tipY;
         segmentEnd.Z = s.tipZ;
         int numberOfPoints = (int)Math.Ceiling(s.length / (s.radius / 2));

         for (int J = 0; J < numberOfPoints; J++)
            {
            // find the position of this current point
            point3D currentPoint;
            currentPoint.X = segmentStart.X + ((double)J / numberOfPoints) * (segmentEnd.X - segmentStart.X);
            currentPoint.Y = segmentStart.Y + ((double)J / numberOfPoints) * (segmentEnd.Y - segmentStart.Y);
            currentPoint.Z = segmentStart.Z + ((double)J / numberOfPoints) * (segmentEnd.Z - segmentStart.Z);

            for (int K = 0; K < numberOfObstacles; K++)
               {
               double minimumSpacing = s.radius + obstacles[K].radius;
               double deltaX = currentPoint.X - obstacles[K].X;
               double deltaY = currentPoint.Y - obstacles[K].Y;
               double deltaZ = currentPoint.Z - obstacles[K].Z;

               double actualSpacing = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

               if (actualSpacing <= minimumSpacing)
                  {
                  //foundCollision = true;
                  obstacles[K].collsionDetected = true;
                  //break;
                  }

               }// end of going through all of the obstacles
            }// end of going through all of the sub-points in the current segment

         return p;
         } // end of checkForSegmentCollision
      private void orientSegmentsAvoidingObstacles(point3D targetPoint)
         {
         // orient the chain as if there were no obstacles.

         // check for collisions

         // The general strategy that I want to follow is to for each collision, create a vector that leads the segment to be
         // out of a collision state the quickest.  This shoud take into account obstacles in the direction that it wants to move.  
         // If there are multiple collisions then add the vectors together and step in that direction.  
         // Move the handles by a weighted amount based on where the collision is within the chain.  


         }
      private void checkboxClick(object sender, EventArgs e)
          {
          drawPicture();
          }
      private void avoidObstacles()
         {// This function looks at the collisions reported by the segments and figures out how to
          // reposition the spline handles so that the chain can avoid them.   
         point3D h1move = giveMeAnEmptyPoint3D();
         point3D h2move = giveMeAnEmptyPoint3D(); // how much to move handle 1 and handle 2
         int iterations = 0;
         int maxIterations = 30;
         double stepSize = 2; // how far to step each time in pixels
         bool stillWorking = true;
         bool collisionsDetected = detectCollisions();

         while(stillWorking)
            {
            collisionsDetected = detectCollisions();
            if (collisionsDetected)
               {
               // check to see if the spline length is greater than the total chain length or if 
               // the maximum number of iterations have happened
               double splineLength = findSplineLength();
               double chainLength = findChainLength();
               if (splineLength > chainLength || iterations > maxIterations)
                  stillWorking = false;
               else
                  {//  figure out how it should move the control points, move them, and redraw
                  for (int I = 0; I < numberOfSegments; I++)
                     {
                     if (segments[I].hadCollision == true)
                        {
                        // This isn't correct.   
                        double percent = segments[I].collisionPercentage;
                        h2move.X += percent * segments[I].obstacleAvoidanceDirection.X;
                        h1move.X += (1 - percent) * segments[I].obstacleAvoidanceDirection.X;
                        h2move.Y += percent * segments[I].obstacleAvoidanceDirection.Y;
                        h1move.Y += (1 - percent) * segments[I].obstacleAvoidanceDirection.Y;
                        h2move.Z += percent * segments[I].obstacleAvoidanceDirection.Z;
                        h1move.Z += (1 - percent) * segments[I].obstacleAvoidanceDirection.Z;
                        }
                     }// end of going through all of the segments looking for collisions

                  // multiply the move amounts by the step size
                  h1move.X *= stepSize;
                  h1move.Y *= stepSize;
                  h1move.Z *= stepSize;
                  h2move.X *= stepSize;
                  h2move.Y *= stepSize;
                  h2move.Z *= stepSize;

                  // move the handles
                  splinePoints[1].X += h1move.X;
                  splinePoints[1].Y += h1move.Y;
                  splinePoints[1].Z += h1move.Z;

                  splinePoints[3].X += h2move.X;
                  splinePoints[3].Y += h2move.Y;
                  splinePoints[3].Z += h2move.Z;

                  // Make sure that the splines still have tangency
                  splinePoints[2] = findMiddle(splinePoints[1], splinePoints[3]);

                  point3D tipPosition = giveMeAnEmptyPoint3D();
                  tipPosition.X = segments[numberOfSegments - 1].tipX;
                  tipPosition.Y = segments[numberOfSegments - 1].tipY;
                  tipPosition.Z = segments[numberOfSegments - 1].tipZ;
                  stretchAndOrientSegmentsUsingSplines(tipPosition);

                  //drawPicture();
                  //Thread.Sleep(200);  // delay for 1/5th of a second

                  iterations++;
                  }
               }// end of if a collision was detected
            else
               {
               stillWorking = false;
               drawPicture();
               }
            
            }// end of if it is still working
         }// end of avoid obstacles
      private point3D findAvoidanceDirection(segment s, obstacle o)
         {
         // find the closest point on the line to the obstacle 
         point3D p; // point on the line
         p.X = s.rootX;
         p.Y = s.rootY;
         p.Z = s.rootZ;

         point3D v; // line vector
         v.X = s.tipX - s.rootX;
         v.Y = s.tipY - s.rootY;
         v.Z = s.tipZ - s.rootZ;

         point3D g; // general point  (just to make it work out with my notes)
         g.X = o.X;
         g.Y = o.Y;
         g.Z = o.Z;

         // solve for t
         double t = (v.X * (g.X - p.X) + v.Y * (g.Y - p.Y) + v.Z * (g.Z - p.Z)) / (v.X * v.X + v.Y * v.Y + v.Z * v.Z);

         // solve for the point on the line that is closest to the obstacle
         point3D closestPoint;
         closestPoint.X = p.X + t * v.X;
         closestPoint.Y = p.Y + t * v.Y;
         closestPoint.Z = p.Z + t * v.Z;

         // create a vector from the obstacle to the point that was found
         point3D ov = giveMeAnEmptyPoint3D(); //output vector
         ov.X = closestPoint.X - g.X;
         ov.Y = closestPoint.Y - g.Y;
         ov.Z = closestPoint.Z - g.Z;

         // Make the vector be a unit vector
         double vectorLength = Math.Sqrt(ov.X * ov.X + ov.Y * ov.Y + ov.Z * ov.Z);
         ov.X /= vectorLength;
         ov.Y /= vectorLength;
         ov.Z /= vectorLength;

         return ov;
         }
      private double findSplinePercentage(obstacle o)
         {// Figures out where the obstacle is relative to the spline so that the control handles can be adjusted
          // appropriately.   This function takes into account that the spine can be curved.

         // To do what it needs to do, it divides up the spine into twenty line segments.  From the center of each segment
         // it projects a perpendicular plane.  It checks to see which plane is closest to the obstacle.  That plane 
         // determines the winner and the percentage is found in 5 percent increments based on which plane won.

         // figure out what percentage of the total length each spline represents
         double eachSplinesPercentage = 1 / numberOfSplines;
         point3D pointA, pointB;
         point3D startPoint, endPoint, midPoint, planeVector, o2pVector; // object2planeVector
         double lowestDistance = 10000;
         double lowestDistancePercentage = 1;

         for (int I = 0; I < numberOfSplines; I++)
            {
            point3D p1 = splinePoints[2 * I];
            point3D p2 = splinePoints[(2 * I) + 1];
            point3D p3 = splinePoints[(2 * I) + 2];

            for (int J = 0; J < 10; J++)
               {// divide each spline into 10 segments

               // find the start point of this segment
               double percentage = (double)J / 10;
               pointA = interpolateBetweenPoints(p1, p2, percentage);
               pointB = interpolateBetweenPoints(p2, p3, percentage);
               startPoint = interpolateBetweenPoints(pointA, pointB, percentage);

               // find the end point of this segment
               percentage = (double)(J+1) / 10;
               pointA = interpolateBetweenPoints(p1, p2, percentage);
               pointB = interpolateBetweenPoints(p2, p3, percentage);
               endPoint = interpolateBetweenPoints(pointA, pointB, percentage);

               // find the mid point, which will be the point used to construct the plane.
               midPoint = interpolateBetweenPoints(startPoint, endPoint, .5);

               // find the vector that will define the plane.
               planeVector.X = endPoint.X - startPoint.X;
               planeVector.Y = endPoint.Y - startPoint.Y;
               planeVector.Z = endPoint.Z - startPoint.Z;
               // normalize the vector
               double vectorLength = Math.Sqrt(planeVector.X * planeVector.X + planeVector.Y * planeVector.Y + planeVector.Z * planeVector.Z);
               planeVector.X /= vectorLength;
               planeVector.Y /= vectorLength;
               planeVector.Z /= vectorLength;

               // Now that I have all of the information needed to construct the plane, find the distance
               // between the object's center and the plane.
               o2pVector.X = midPoint.X - o.X;
               o2pVector.Y = midPoint.Y - o.Y;
               o2pVector.Z = midPoint.Z - o.Z;

               // the dot product of these two vectors is the distance to between them
               double thisDistance = Math.Abs(o2pVector.X * planeVector.X + o2pVector.Y * planeVector.Y + o2pVector.Z * planeVector.Z);

               if (thisDistance < lowestDistance)
                  {
                  lowestDistance = thisDistance;
                  lowestDistancePercentage = eachSplinesPercentage * (double)I + (eachSplinesPercentage / (double)10) * J;
                  }

               }// end of going through all segments in the spline section
            }// end of going through the splines
         Debug.WriteLine("The intersection's percentage is " + lowestDistancePercentage + "  Lowest distance: " + lowestDistance);
         return lowestDistancePercentage;
         }// end of findSplinePercentage
      private double findSplinePercentage02(obstacle o)
         {// finds the percent along a straight-line path between the root and the tip 
          // where the obstacle is closest to that line.
         point3D p1;
         p1.X = segments[0].rootX;
         p1.Y = segments[0].rootY;
         p1.Z = segments[0].rootZ;

         point3D p2;
         p2.X = segments[numberOfSegments-1].tipX;
         p2.Y = segments[numberOfSegments - 1].tipY;
         p2.Z = segments[numberOfSegments - 1].tipZ;

         point3D v1;
         v1.X = p2.X - p1.X;
         v1.Y = p2.Y - p1.Y;
         v1.Z = p2.Z - p1.Z;

         point3D v2;
         v2.X = o.X - p1.X;
         v2.Y = o.Y - p1.Y;
         v2.Z = o.Z - p1.Z;

         // project the v2 vector onto v1 using the dot product
         double rootToTipLength = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y + v1.Z * v1.Z);
         double projectedLength = (v2.X * v1.X + v2.Y * v1.Y + v2.Z * v1.Z) / rootToTipLength;

         return projectedLength / rootToTipLength;
         }// end of findSplinePercentage02
      private double findChainLength()
         {// sums up the lengths of the chain's segments
         double chainLength = 0;
         for (int I = 0; I < numberOfSegments; I++)
            {
            chainLength += segments[I].length;
            }
         return chainLength;
         }

      private void AvoidObstaclesCheckStateChanged(object sender, EventArgs e)
         {
         if (avoidObstaclesCheckBox.Checked)
            {
            avoidObstacles();
            }
         }
      }// end of Form1 class
   }// end of WindowsFormsApplication1
