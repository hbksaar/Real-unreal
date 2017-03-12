/*
Thomas Sanchez Lengeling
http://codigogenerativo.com/
How to use multiple Kinects v2 in the same sketch.
Should work up n number of Kinects v2 connected to the USB 3.0 port.
https://github.com/shiffman/OpenKinect-for-Processing
http://shiffman.net/p5/kinect/
*/

//ToDo: Bessere Referenz

//Funktioniert nicht, weil 64 bit AMD Architekturen sich nicht gut mit 32 bit Intel dlls vertragen. Geil.
//import hypermedia.video.*;

import processing.net.*;
import processing.opengl.*;
import org.openkinect.processing.*;
import gab.opencv.*;
import blobDetection.*;
import KinectPV2.*;
import java.awt.*;

//Network stuff
int iPort = 5050;

//Reserve bytes for starting and ending the package
int startByte = 254;
int endByte = 255;

Server server;
Client client;
int dataIn;
byte[] byteBuffer = new byte[12];

//Initialize blob variables
int blobslength;
int centroidx;
int centroidy;

OpenCV opencf;
Blob[] blobs;
BlobDetection theBlobDetection;

// Kinect depth resolution: 512×424
int camw = 512;
int camh = 424;
//Values to define the image to be computed
int x_offset = 30;
int y_offset = 30;

//radius to blur
int blurradius = 10;
// threshold 
int threshold = 52;

final boolean maxPerformance = false;
final boolean forceDrawing = true;

// Difference tests
PImage before, after, grayDiff;
PImage reference;

Kinect2 kinect2a;
Kinect2 kinect2b;

OpenCV opencv;
OpenCV diff;

void setup() {
  size(1536, 848, P2D);

  //Network
  server = new Server(this, iPort);
  client = new Client(this, "127.0.0.1", iPort);
  
  kinect2a = new Kinect2(this);
  kinect2a.initDepth();
  kinect2a.initVideo();
  kinect2a.initIR();

  kinect2b = new Kinect2(this);
  kinect2b.initDepth();
  kinect2b.initVideo();
  kinect2b.initIR();


  //Start tracking each kinect
  kinect2a.initDevice(0); //index 0
  kinect2b.initDevice(1); //index 1
  
  //initialize reference image
  reference = createImage(camw*2, camh, RGB);
  
  //Amazing blob detection gtfo
  BlobDetection.setConstants(100, 4000, 500);
  
  //Restrict area where to map
  theBlobDetection = new BlobDetection(camw - x_offset*2, camh * 2 - y_offset*2);
  
  theBlobDetection.setPosDiscrimination(true);
  theBlobDetection.setThreshold(map(threshold, 0, 255, 0.0f, 1.0f));
  theBlobDetection.activeCustomFilter(this);
  
  // Initializing OpenCV and allocating memory
  opencv = new OpenCV( this, camw - x_offset * 2, camh * 2 - y_offset * 2 );
  //opencv = new OpenCV( this);
  //opencv.allocate(camw, camh*2);
  background(0);
}

void draw() {
  background(0);
    // Calculating blobs
  blobRoutine();
 
  //Show blobs
  drawBlobsAndEdges(true, false);
  
  // Sending blobs to all clients
  sendBlobs();
  
  // FPS tracking to properly identify lag issues
  surface.setTitle(int(frameRate) + "fps");
 
}

void blobRoutine() {
  //background(0);

 // image(kinect2a.getDepthImage(), 0, 0);
 // image(kinect2a.getIrImage(), 512, 0);
 // image(kinect2a.getVideoImage(), 512*2, 0, 512, 424);

 // image(kinect2b.getDepthImage(), 0, 424);
 // image(kinect2b.getIrImage(), 512, 424);
 // image(kinect2b.getVideoImage(), 512*2, 424, 512, 424);
 
 PImage image1 = kinect2a.getDepthImage();
 PImage image2 = kinect2b.getDepthImage();
 
 
  PImage bothCams = createImage (camw, camh*2, RGB);
  bothCams.set(0, 0, image1);
  for(int i = 0; i < camw; i++) {
      for(int j = 0; j < camh; j++) {
        bothCams.set(camw-i, camh*2-j, bothCams.get(i, j));
      }
    }
    
  bothCams.set(0, 0, image2);
  //bothCams.filter(BLUR, blurradius);
    

  //opencv.copy(bothCams);
  
  //before = loadImage("before.jpg");
  //after = loadImage("after.jpg");
  
  //diff = new OpenCV(this, before);
  //diff.diff(after);
  //grayDiff = diff.getSnapshot(); 
  
  PImage resize = bothCams;
  int scaleby = 3;
  resize.resize(0, camh/scaleby);
  resize.resize(0, camh*2);

  bothCams = resize;
  
  if(keyPressed && (key == ' ')) { 
      reference = bothCams;
      reference = bothCams.get(x_offset, y_offset, camw - x_offset*2, camh * 2 - y_offset * 2);
      //reference.filter(BLUR, blurradius);
      println("reference set to image from frame " + frameCount);
    }
    
  PImage crop = bothCams.get(x_offset, y_offset, camw - x_offset*2, camh * 2 - y_offset * 2);
  
  opencv.loadImage(crop);
  opencv.diff(reference);
  opencv.threshold(threshold);
  //opencv.sub();
  //opencv.blur(blurradius);
  
  //Es könnte alles so einfach sein, intel aber nicht.
  //opencv.blobs();
  
  theBlobDetection.computeBlobs(opencv.getSnapshot().pixels);
  
  //println(theBlobDetection.getBlobNb());
  
  if(!maxPerformance) {
    //Live image
    image(crop, 0, 0);
    //Reference image
    image(reference, camw, 0);
    //Difference image
    image(opencv.getSnapshot(), camw * 2, 0); 
    
    text("threshold: " + threshold, camw*2 + 30, 30);
  }

}

void drawBlobsAndEdges(boolean drawBlobs, boolean drawEdges)
{
  if(maxPerformance && !forceDrawing) { return; }
  noFill();
  Blob b;
  EdgeVertex eA, eB;
  for (int n=0 ; n<theBlobDetection.getBlobNb() ; n++)
  {
    b=theBlobDetection.getBlob(n);
    if (b!=null)
    {
      // Edges
      if (drawEdges)
      {
        strokeWeight(2);
        stroke(0, 255, 0);
        for (int m=0;m<b.getEdgeNb();m++)
        {
          eA = b.getEdgeVertexA(m);
          eB = b.getEdgeVertexB(m);
          if (eA !=null && eB !=null)
            line(
            eA.x*width / 3 + camw * 2 + x_offset, eA.y*height, 
            eB.x*width / 3 + camw * 2 + x_offset, eB.y*height
              );
        }
      }

      // Blobs
      // This is slightly off due to x & y offsets
      if (drawBlobs)
      {
        strokeWeight(1);
        stroke(255, 0, 0);
        rect(
        (b.xMin*width / 3)  + camw * 2, b.yMin*height, 
        (b.w*width / 3), b.h*height
          );
      }
    }
  }
}

public boolean newBlobDetectedEvent(Blob b) {
  //println("w: " + b.w + ", h: " + b.h);
  //return true;
  
  //Check blob size (>10%)
  if(b.w < 0.1 && b.h < 0.15) {
    return false;
  }
  //Check blob position (not the outer corners)
  else if(b.x < 0.1 || b.x > 0.9 || b.y < 0.1 || b.y > 0.9) {
    return false; 
  }
  
  //Acknowledge the worthy blob
  else {
    return true;
  }

}

void sendBlobs() {

    String data = "";
    data += theBlobDetection.getBlobNb();
    
    Blob b;
      for (int n=0 ; n<theBlobDetection.getBlobNb() ; n++) {

        b=theBlobDetection.getBlob(n);
        if (b!=null || true) {

          //centroidx = b.x;
          //centroidy = b.y;
      
          data += (" " + b.x + " " + b.y);
          
        }  
  }
  
  server.write(data + "\n");
    
    //Works better with startByte than endByte
    //server.write((int) endByte);
}

// Set Threshold

void mouseDragged() {
  threshold = int( map(mouseX, 0, width, 0, 255) );
  theBlobDetection.setThreshold(map(threshold, 0, 255, 0.0f, 1.0f));
  println(threshold);
}