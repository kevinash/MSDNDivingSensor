import cv2
import numpy as np
from datetime import timedelta
import time
from scipy.spatial import distance as dist
 
# video to capture
cap = cv2.VideoCapture("P1170013.MP4")
 
# Lucas kanade params
lk_params = dict(winSize = (15, 15),
                 maxLevel = 4,
                 criteria = (cv2.TERM_CRITERIA_EPS | cv2.TERM_CRITERIA_COUNT, 10, 0.03))
 
# GLOBAL VARIABLE
point_selected = 0
point = ()
old_points = np.array([[]])
# Calculate the distance between 2 points based on the reference distrance of the markers on the board
def distance(x1, y1, x2, y2):
    refDist = 128.249756335
    #print(dist.euclidean((x1,y1), (x2,y2))/refDist)
    return dist.euclidean((x1,y1), (x2,y2))/refDist
    
# Mouse function
def select_point(event, x, y, flags, params):
    global point, point_selected, old_points, frame
    if event == cv2.EVENT_LBUTTONDOWN:
        if not point_selected:
            point = (x, y)
            old_points = np.array([[x, y]], dtype=np.float32)
            point_selected = True
        else:
            new = np.array([[x, y]], dtype=np.float32)
            points = np.append(old_points, new, axis=0)
            old_points = points
        print(frame[y][x])
 
cv2.namedWindow("Frame")
cv2.setMouseCallback("Frame", select_point)

color = np.random.randint(0,255,(100,3))

# Create old frame
_, frame = cap.read()
old_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
mask = np.zeros_like(frame)

lowest_point = 0
start = None
THRESH = 50
while True:
    _, frame = cap.read()
    if frame is None:
        break
    gray_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
 
    if point_selected is True:
        # cv2.circle(frame, point, 5, (0, 0, 255), 2)
 
        new_points, status, error = cv2.calcOpticalFlowPyrLK(old_gray, gray_frame, old_points, None, **lk_params)
        old_gray = gray_frame.copy()
        # x, y = new_points.ravel()
        # cv2.circle(frame, (x, y), 5, (0, 255, 0), -1)
        for i,(new,old) in enumerate(zip(new_points,old_points)):
            a,b = new.ravel()
            if start is None:
                start = a
            c,d = old.ravel()
            mask = cv2.line(mask, (a,b),(c,d), color[i].tolist(), 2)
            frame = cv2.circle(frame,(a,b),5,color[i].tolist(),-1)
            if b > lowest_point and abs(a - start) <= THRESH:
                # print('ms:', timedelta(milliseconds=cap.get(cv2.CAP_PROP_POS_MSEC)))
                low = np.zeros_like(frame)
                low = cv2.circle(low, (a,b), 5, color[i+1].tolist(), -1)
                lowest_point = b
        img = cv2.add(frame,mask) # essey
        img = cv2.add(img, low)
        cv2.imshow('Frame',img)
        old_points = new_points.reshape(-1,2)
    else:
        cv2.imshow("Frame", frame)
    key = cv2.waitKey(1)
    if key == 27:
        break
 
cap.release()
cv2.destroyAllWindows()
exit()