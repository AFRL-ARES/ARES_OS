import serial
import time
import AmaresConfig as config #need to import AmaresConfig in all modules
from BasicComm import addToLog

import numpy as np
import matplotlib.pyplot as plt
import cv2

from PIL import Image

from skimage import data, img_as_float, img_as_uint, io, segmentation, color, exposure
from skimage.color import rgb2gray, rgb2hsv
from skimage.transform import rescale, resize
from skimage.morphology import reconstruction

def openLightPort():
    try:
        config.ser_comm["lights"].open()
        addToLog("Opening lights on " + str(config.ser_comm["lights"]))
        while (config.ser_comm["lights"].in_waiting < 7):
            pass
        addToLog("Lights port opened")
    except Exception as ex:
        addToLog("ERROR in VisionUtils.openLightPort")
        addToLog("\t" + str(ex))

def closeLightPort():
    try:
        config.ser_comm["lights"].close()
        time.sleep(0.2)
        addToLog("Lights port closed")
    except Exception as ex:
        addToLog("ERROR in VisionUtils.closeLightPort")
        addToLog("\t" + str(ex))
    
def setLight(index, value):
    try:
        if (config.ser_comm["lights"].isOpen() == False):
            openLightPort()
        if (index==1) and (value!=0):
            addToLog("Accessing process lights...")
        if (index==2) and (value!=0):
            addToLog("Accessing alignment lights...")
       
        value = (index*1000) + value
        config.ser_comm["lights"].write(b'%d' % value)
        time.sleep(0.05)
    except Exception as ex:
        addToLog("ERROR in VisionUtils.setLight")
        addToLog("\t" + str(ex))

def processLights(value=255, storeVal=True):
    try:
        if(storeVal==True):
            config.tool_vars["procLightVal"] = value # store new value
        setLight(1,value)
        addToLog("Process lights set to " + str(value))
    except Exception as ex:
        addToLog("ERROR in VisionUtils.processLights")
        addToLog("\t" + str(ex))
        
def alignmentLights(value=255, storeVal=True):
    try:
        if(storeVal==True):
            config.tool_vars["alignLightVal"] = value # store new value
        setLight(2,value)
        addToLog("Alignment lights set to " + str(value))
    except Exception as ex:
        addToLog("ERROR in VisionUtils.alignmentLights")
        addToLog("\t" + str(ex))


## Image Analysis Helpers:

def excMessage(functionName, exception=""):
    message = "Exception in " + str(functionName) + "\n"
    message = message + "\t" + str(exception)
    return(message)

def showImage(img_data, title="Image"):
    plt.imshow(img_data)
    plt.title(title)
    plt.show()

def do_Rescale(image, current_scale, factor):
    start = time.time()
    image_rescaled = cv2.resize(image, dsize=(int(image.shape[1]*factor),int(image.shape[0]*factor)),
                                 interpolation=cv2.INTER_CUBIC)
    new_scale = int(current_scale * factor)
    return(image_rescaled, new_scale)

def getBackgroundSpecs(img_data, crop=0.1, buffer=30):
    # gets average background pixel, brightest background pixel, and dimmest
    # background pixel for left and right regions for an image of a centered
    # vertical line
    # 'crop' defines the percent of image width used to define left and right
    # background regions
    # buffer adds or subtracts an integer to lower and upper values
    try:
        img = cv2.GaussianBlur(img_data, (15,15),0)
        leftBound = int(img.shape[1]*crop)
        rightBound = int(img.shape[1]*(1-crop))
        height = img.shape[0]
        leftRegion = img[0:height,0:leftBound]
        rightRegion = img[0:height,rightBound:img.shape[1]]
        bothRegions = np.concatenate((leftRegion,rightRegion),axis=1)
        # get average background pixel:
        r,g,b = cv2.split(bothRegions)
        rAvg = np.average(r)
        gAvg = np.average(g)
        bAvg = np.average(b)
        avgBgPix = np.array([rAvg,gAvg,bAvg])
        avgBgPix = np.clip(avgBgPix,0,255).astype('uint8')
        # get largest background values:
        rMax = (np.amax(r)+buffer)
        gMax = (np.amax(g)+buffer)
        bMax = (np.amax(b)+buffer)
        maxBgPix = np.array([rMax, gMax, bMax])
        maxBgPix = np.clip(maxBgPix,0,255).astype('uint8')
        # get dimmest background values:
        rMin = (np.amin(r)-buffer)
        gMin = (np.amin(g)-buffer)
        bMin = (np.amin(b)-buffer)
        minBgPix = np.array([rMin, gMin, bMin])
        minBgPix = np.clip(minBgPix,0,255).astype('uint8')
        return(avgBgPix,maxBgPix,minBgPix)
    except Exception as ex:
        addToLog(excMessage("getBackgroundSpecs", ex))


def npAvg(a,b):
    a = a.astype('float32')
    b = b.astype('float32')
    return(((a+b)/2).astype('uint8'))

def turnDarkRed2LightRed(img_data,redThresh=100,otherThresh=40,veryLightRedThresh=200):
    # turns dark red to average background AND
    # turns very light red to average background
    img = img_data.copy()
    avgPix, upperBG, lowerBG = getBackgroundSpecs(img)
    for y in range(img.shape[1]-1):
        for x in range(img.shape[0]-1):
            if (img[x,y][1]<redThresh and img[x,y][2]<redThresh and     # if blue and green dark but red bright
                img[x,y][0]>redThresh):                                 
                img[x,y] = avgPix                               # make avg red
            elif (img[x,y][1]<otherThresh and img[x,y][2]<otherThresh and       # if blue and green very dark
                  img[x,y][0]>(2*otherThresh) and img[x,y][0]>(2*otherThresh)): # but red not very dark
                img[x,y] = avgPix
            elif ((img[x,y][0]>veryLightRedThresh) and
                  (img[x,y][1]<veryLightRedThresh) and
                  (img[x,y][2]<veryLightRedThresh)):
                img[x,y] = avgPix
    return(img)

def flattenBG(img_data, thresh=85, similarity=0.8, bg_color=[0,0,0]):
    # for white ink on black background
    img = img_data.copy()
    for y in range(img.shape[1]):
        for x in range(img.shape[0]):
            if ((img[x,y][0]<=thresh) and (img[x,y][1]<=thresh) and (img[x,y][2]<=thresh)):
                img[x,y] = np.array(bg_color).astype('uint8')
    return(img)

def removeBackground(img_data):
    try:
        img = img_data.copy()
        black = np.zeros(img.shape).astype('uint8')
        avgBg, upperBg, lowerBg = getBackgroundSpecs(img)
        mask = cv2.inRange(img, lowerBg, upperBg)
        maskRGB = cv2.cvtColor(mask, cv2.COLOR_GRAY2RGB)
        bgGone = cv2.bitwise_and(img, ~maskRGB)
        addToLog("Background Removed.")
        return(bgGone)
    except Exception as ex:
        addToLog("removeBackground: " + str(ex))

def uintDiff(number1,number2):
    # finds and returns the absolute difference between two 'uint8' values
    num1 = int(number1)
    num2 = int(number2)
    diff = abs(num1-num2)
    return(diff)

def removeExtremeSpots(img_data,factor=2):
    img = img_data.copy()
    bgVal, upperBg, _, = getBackgroundSpecs(img)
    bgVal = sum(bgVal)
    upperBg = sum(upperBg)
    for y in range(img.shape[1]-1):
        for x in range(img.shape[0]-1):
            if (sum(img[x,y])>(2*bgVal) or sum(img[x,y])>upperBg):
                img[x,y] = np.true_divide(img[x,y],factor).astype('uint8')
            elif (sum(img[x,y])<(int(bgVal/5))):
                img[x,y] = np.multiply(img[x,y],factor).astype('uint8')
                img[x,y] = np.clip(img[x,y], 0, 255).astype('uint8')
    return(img)

def removeBoundaryContours(contours, img):
    # removes contours that touch left or right edges
    try:
        newContours = []
        for cont in range(len(contours)):        # loops through list of np.arrays containing contour coordinates
            isgood = True
            for index in range(contours[cont].shape[0]):   # loops through coordinates in each array
                if (contours[cont][index,0][0] >= (img.shape[1]-1) or
                    contours[cont][index,0][0] <= 1):  # checks each "x" coordinate or column index
                    isgood = False
                    break
            if (isgood == True):
                newContours.append(contours[cont])
        return(newContours)
    except Exception as ex:
        addToLog("removeBoundaryContours: " + str(ex))

def removeSmallContours(contours, area=100):
    # removes contours with small areas (as defined by area in pix^2)
    try:
        newContours = []
        for cont in range(len(contours)):
            if (cv2.contourArea(contours[cont]) > area):
                newContours.append(contours[cont])
        return(newContours)
    except Exception as ex:
        addToLog("removeSmallcontours: " + str(ex))
                
def dilate(img_data,kernel_size=5,iterations=1):
    img=img_data
    ks = kernel_size
    itr = iterations
    kernel = np.ones((ks,ks),np.uint8)
    dilated = cv2.dilate(img,kernel,iterations = itr)
    return(dilated)

def erode(img_data,kernel_size=5,iterations=1):
    img=img_data
    ks = kernel_size
    itr = iterations
    kernel = np.ones((ks,ks),np.uint8)
    eroded = cv2.erode(img,kernel,iterations = itr)
    return(eroded)

def getBinary(raw_test_img):
    # Analyzes and returns thresholded binary image
    try:
        kernel = np.ones((5,5),np.uint8)
        test_img = raw_test_img.copy()  # duplicate, keep original loaded
        quiet1 = cv2.bilateralFilter(test_img,9,250,75)
        quiet2 = cv2.pyrMeanShiftFiltering(quiet1,31,65)    # denoise the image (method 1)
        flatten = flattenBG(quiet2, similarity=0.8)
        gray_img = cv2.cvtColor(~flatten, cv2.COLOR_RGB2GRAY)
        try:
            binary_img = cv2.threshold(gray_img, 254, 255, cv2.THRESH_BINARY)[1]
        except Exception as ex1:
            addToLog("binary_img: " + str(ex1))
        try:
            closed_img = erode(binary_img,kernel_size=3,iterations=5)
            closed_img2 = dilate(closed_img,kernel_size=3,iterations=3)
        except Exception as ex2:
            addToLog("closed_img: " + str(ex2))
        
        # use inverted binary as input for contour finding (contours around white)
        edges = ~closed_img2.copy()
        return(edges)
    except Exception as ex:
        addToLog(excMessage("getBinary",ex))


def getContours(binaryImage):
    # returns filtered contours of binary image
    try:
        # get contours (works for different versions of openCV)
        try:
            contours, hierarchy = cv2.findContours(binaryImage,
                                                   cv2.RETR_TREE,
                                                   cv2.CHAIN_APPROX_SIMPLE)
        except:
            im2, contours, hierarchy = cv2.findContours(binaryImage,
                                                        cv2.RETR_TREE,
                                                        cv2.CHAIN_APPROX_SIMPLE)
        # filter contours:
        contours = removeBoundaryContours(contours,binaryImage)
        contours = removeSmallContours(contours, area = 200)
        return(contours)
    except Exception as ex:
        addToLog(excMessage("getContours",ex))

def getBoundingRectangle(binaryImage, contours, desired_width):
    try:       
        # get bounding rectangle(using contours):
        d_w = desired_width
        height, width = binaryImage.shape[0:2]
        min_x, min_y = width, int(height/2) # bounding rectangle must at least reach
                                            # center of FOV
        max_x = max_y = 0

        for contour in contours:
            x, y, w, h = cv2.boundingRect(contour)
            min_x, min_y = min(min_x, x), min(min_y, y)
            max_x, max_y = max(max_x, x+w), max(max_y, y+h)
        max_y = height # rectangle must extend to bottom of FOV (since it's the end of the line)
        w = max_x-min_x
        min_x = int((min_x + (w/2))-(d_w/2)) # centers the rectangle on the feature
        return(min_x,min_y,d_w,max_y-min_y)
    except Exception as ex:
        x = int(width / 2) - int(d_w/2)
        y = int(height / 2)
        w = d_w
        h = int(height / 2)
        return(x,y,w,h)
        addToLog(excMessage("getBoundingRectangle",ex))

def drawTopFilletRectangle(img,top_left,bottom_right,color=(50, 100, 255),thickness=2):
    #top-left and bottom-right are of the format (x,y)
    try:
        thickness = int(thickness)
        radius = int((bottom_right[0]-top_left[0])/2)
        top_left = (top_left[0], top_left[1]+radius)
        bottom_left = (top_left[0],bottom_right[1])
        top_right = (bottom_right[0],top_left[1])
        arc_center = (int((top_left[0]+top_right[0])/2),top_left[1])
        if (thickness>0):
            # draw outline of filleted rectangle
            cv2.line(img, top_left, bottom_left, color, thickness)
            cv2.line(img, bottom_left, bottom_right, color, thickness)
            cv2.line(img, bottom_right, top_right, color, thickness)
            cv2.ellipse(img, arc_center, (radius,radius), 0, 180, 360,
                        color, thickness)
        else:
            # draw a filled rectangle and circle
            thickness = -1
            cv2.rectangle(img, top_left, bottom_right, color, thickness)
            cv2.circle(img, arc_center, radius, color, thickness)
        return(img)
    except Exception as ex:
        addToLog(excMessage("drawTopFilletRectangle",ex))

def topFilletRectArea(top_left, bottom_right):
    # returns the area of a top-filleted rectangle that fits within the specified
    # rectangle; radius of fillet is half the width of the rectangle
    radius = int((bottom_right[0]-top_left[0])/2)
    width = bottom_right[0] - top_left[0]
    height = bottom_right[1] - top_left[1] - radius
    semicircleArea = (np.pi * radius * radius)/2
    rectangleArea = width * height
    return(semicircleArea + rectangleArea)





    
