import numpy as np
import matplotlib.pyplot as plt
import cv2
import os.path
import time

from PIL import Image

from VisionUtils import *
from BasicComm import addToLog, addData, createDataFile
import AmaresConfig as config


def analyzeBlueLine(file, desired_width=0.3):
    # using a microscope scale standard and ImageJ, measured
    # 350 pixels per mm (350 pix/mm)
    try:
        start = time.time()
        config.data['analyzer'] = "analyzeBlueLine"
        if(config.data["Expt#"] == 1): createDataFile()
        scale = 350 #pix/mm
        d_w = int(desired_width * scale)
        # read in and scale test image:
        src, scale = do_Rescale(io.imread(file),scale,0.5)
        src2 = src.copy()
        height,width = src2.shape[0:2]

        # binarize image and get overall outline of feature:
        binary = getBinary(src) # converts image to binary appropriately
        contours = getContours(binary) # gets filtered contours of binary image
        x,y,w,h = getBoundingRectangle(binary, contours, d_w)

        if (x==y==w==h==0):
            config.data["result"] = 0.0
        else:
            # get portion of feature outside of desired region:
            excluded = drawTopFilletRectangle(binary.copy(), (x, y),
                                              (x + w, y + h), color=(0),
                                              thickness=-1)
            excl_contours = getContours(excluded)
            excl_area = 0
            for contour in excl_contours:
                excl_area += cv2.contourArea(contour)

            # get portion of feature inside of desired region:
            included = np.subtract(excluded, binary)
            incl_contours = getContours(included)
            incl_area = 0
            for contour in incl_contours:
                incl_area += cv2.contourArea(contour)
            eff_area = incl_area - excl_area
            if(eff_area < 0): eff_area = 0
            
            # draw bounding rectangle and contours:
            src2 = drawTopFilletRectangle(src2, (x, y), (x + w, y + h),
                                          color=(50, 100, 255), thickness=2)
            cv2.drawContours(src2, excl_contours, -1, color=(255,0,0), thickness=2)
            cv2.drawContours(src2, incl_contours, -1, color=(255,255,0), thickness=2)
            
            desiredArea = topFilletRectArea((x, y), (x + w, y + h))
            config.data["result"] = round(eff_area/desiredArea,6)
            font = cv2.FONT_HERSHEY_SIMPLEX
            text = (config.paths["dataset"])
            cv2.putText(src2,text,(10, height-60), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            text = ("Expt# " + str(config.data["Expt#"]))
            cv2.putText(src2,text,(10, height-40), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            text = ("Result = " + str(config.data["result"]))
            cv2.putText(src2,text,(10, height-20), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            # write image file:
            try:
                #newfile = os.path.splitext(file)[0] + "_res" + os.path.splitext(file)[1]
                newfile = config.paths["campaigndatapath"] + "Expt_" + str(config.data["Expt#"]) + os.path.splitext(file)[1]
                io.imsave(newfile,src2)
            except Exception as ex:
                addToLog("Problem saving analyzed image file ") #+ str(newfile))
        duration = str(round(time.time()-start,2))
        addToLog("Anal. complete. (" + duration + "s)")
        addToLog("Result = " + str(config.data["result"]))
        addData() # adds experiment results to CSV file
        return(config.data["result"])
    except Exception as ex:
        addToLog(str(ex))
        return(0)

def analyzeWhiteOnBlack(file, desired_width=0):
    # using a microscope scale standard and ImageJ, measured
    # 350 pixels per mm (350 pix/mm)
    try:
        start = time.time()
        if(desired_width == 0):
            desired_width = config.dispenser["diameter"]
        config.data['Analyzer'] = "analyzeWhiteOnBlack"
        config.data['Analyzer Parameters'] = {'Desired Width': desired_width}
        config.data['Result Descriptor'] = "Modified Fill Factor"
        if(config.data["Expt#"] == 1): createDataFile()
        scale = 350 #pix/mm
        d_w = int(desired_width * scale)
        # read in and scale test image:
        src, scale = do_Rescale(io.imread(file),scale,0.5)
        src2 = src.copy()
        height,width = src2.shape[0:2]

        # binarize image and get overall outline of feature:
        binary = getBinary(src) # converts image to binary appropriately
        contours = getContours(binary) # gets filtered contours of binary image
        x,y,w,h = getBoundingRectangle(binary, contours, d_w)

        if (x==y==w==h==0):
            config.data["result"] = 0.0
        else:
            # get portion of feature outside of desired region:
            excluded = drawTopFilletRectangle(binary.copy(), (x, y),
                                              (x + w, y + h), color=(0),
                                              thickness=-1)
            excl_contours = getContours(excluded)
            excl_area = 0
            for contour in excl_contours:
                excl_area += cv2.contourArea(contour)

            # get portion of feature inside of desired region:
            included = np.subtract(excluded, binary)
            incl_contours = getContours(included)
            incl_area = 0
            for contour in incl_contours:
                incl_area += cv2.contourArea(contour)
            eff_area = incl_area - excl_area
            if(eff_area < 0): eff_area = 0
            
            # draw bounding rectangle and contours:
            src2 = drawTopFilletRectangle(src2, (x, y), (x + w, y + h),
                                          color=(50, 100, 255), thickness=2)
            cv2.drawContours(src2, excl_contours, -1, color=(255,0,0), thickness=2)
            cv2.drawContours(src2, incl_contours, -1, color=(255,255,0), thickness=2)
            
            desiredArea = topFilletRectArea((x, y), (x + w, y + h))
            config.data["result"] = round(eff_area/desiredArea,6)
            font = cv2.FONT_HERSHEY_SIMPLEX
            text = (config.paths["dataset"])
            cv2.putText(src2,text,(10, height-60), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            text = ("Expt# " + str(config.data["Expt#"]))
            cv2.putText(src2,text,(10, height-40), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            text = ("Result = " + str(config.data["result"]))
            cv2.putText(src2,text,(10, height-20), font, 0.6, (255,255,255), 1, cv2.LINE_AA)
            # write image file:
            try:
                #newfile = os.path.splitext(file)[0] + "_res" + os.path.splitext(file)[1]
                newfile = config.paths["campaigndatapath"] + "Expt_" + str(config.data["Expt#"]) + os.path.splitext(file)[1]
                io.imsave(newfile,src2)
            except Exception as ex:
                addToLog("Problem saving analyzed image file ") #+ str(newfile))
        duration = str(round(time.time()-start,2))
        addToLog("Anal. complete. (" + duration + "s)")
        addToLog("Result = " + str(config.data["result"]))
        addData() # adds experiment results to CSV file
        return(config.data["result"])
    except Exception as ex:
        addToLog(str(ex))
        return(0)


