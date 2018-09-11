import cv2
import numpy as np
from datetime import timedelta
import time
 
# video to capture
cap = cv2.VideoCapture("P1170013.MP4")

cv2.namedWindow("Frame")
_, frame = cap.read()
hsv = frame# cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
# lower = np.array([0,0,10])
# upper = np.array([5,10,20])
# 
# mask = cv2.inRange(hsv,lower, upper)
# res = cv2.bitwise_and(frame, frame, mask=mask)
point = list()
static = None
# flowpt = 
def select_point(event, x, y, flags, params):
    global point, static
    if event == cv2.EVENT_LBUTTONDOWN:
        if len(point) < 2:
            point.append((x, y))
        elif static is None:
            static = (x,y)

cv2.setMouseCallback("Frame", select_point)

# set up 2 board points
x = cv2.waitKey(1)
color = np.random.randint(0,255,(100,3))
while len(point) < 2:
    cv2.imshow('Frame', frame)
    for i,new in enumerate(point):
        a,b = new
        frame = cv2.circle(frame,(a,b),5,color[i].tolist(),-1)
    x = cv2.waitKey(1)
old = None
prev = None
for i,new in enumerate(point):
    a,b = new
    if old is not None:
        frame = cv2.line(frame, (a,b),old, color[i].tolist(), 2)
    frame = cv2.circle(frame,(a,b),5,color[i].tolist(),-1)
    cv2.imshow('Frame', frame)
    old = new
    if prev is None:
        prev = old
    x = cv2.waitKey(1)

import math
print('distance: ', math.sqrt(pow(float(a - prev[0]), 2) + pow(float(b - prev[1]), 2)))

# setup static point below diver's feet
while static is None:
    x = cv2.waitKey(1)
a,b = static
frame = cv2.circle(frame, (a,b), 5, color[15].tolist(), -1)
cv2.imshow('Frame', frame)
print('set up static point')

# TODO: setup flow on diver
while x < 0:
    x = cv2.waitKey(1)
