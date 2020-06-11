import numpy
import cv2
from BasicComm import addToLog


def analyzeBlueLine(file):
    #TODO: figure out how to get the image directly from C# as src
    try:
        addToLog("Loading image from C#: " + str(file))
        src = cv2.imread(file)
        lowEdge = 100
        highEdge = 100
        lowerBlue = numpy.array([50, 50, 50])
        upperBlue = numpy.array([130, 255, 255])
        hsv = cv2.cvtColor(src, cv2.COLOR_BGR2HSV)
        mask = cv2.inRange(hsv, lowerBlue, upperBlue)
        blueFiltered = cv2.bitwise_and(src, src, mask=mask)
        gray = cv2.cvtColor(blueFiltered, cv2.COLOR_BGR2GRAY)
        edges = cv2.Canny(gray, lowEdge, highEdge)
        x, y, w, h = cv2.boundingRect(edges)
        cv2.rectangle(src, (x, y), (x + w, y + h), (0, 0, 255), 2)
        cropped = src[y:y + h, x:x + w]
        lowerBlue = numpy.array([60, 0, 50])
        upperBlue = numpy.array([110, 255, 255])
        mask = cv2.inRange(hsv[y:y + h, x:x + w], lowerBlue, upperBlue)
        blueRemoved = cv2.bitwise_and(cropped, cropped, mask=mask)
        removedGray = cv2.cvtColor(blueRemoved, cv2.COLOR_BGR2GRAY)
        totalPixels = w * h
        goodPixels = cv2.countNonZero(removedGray)
        lineStraightness = goodPixels / totalPixels
        addToLog("Blue Line: " + str(lineStraightness))
        return lineStraightness
    except:
        return 0