import glfw
from OpenGL.GL import *
import numpy as np


p0 = np.array([200.,200.])
p1 = np.array([400.,400.])
pv0= np.array([300.,350.])
pv1= np.array([500.,550.])

gEditingPoint = ''

def render():
    global p0, p1
    glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT)

    glEnable(GL_DEPTH_TEST)

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    glOrtho(0,640, 0,640, -1, 1)

    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()

    glColor3ub(255, 255, 255)
    glBegin(GL_LINE_STRIP)
    
    InitHermit(pv0,p0,p1,pv1);

    for t in np.arange(0,1,.01):
        # p = (1-t)*p0 + t*p1
        p = HermitPoint(t)
        glVertex2fv(p)
    glEnd()

    glPointSize(20.)
    
    glBegin(GL_POINTS)
    glVertex2fv(p0)
    glVertex2fv(p1)
    glEnd()

    glColor3ub(0,255,0);

    glBegin(GL_LINES)
    glVertex2fv(pv0)
    glVertex2fv(p0)
    glVertex2fv(p1)
    glVertex2fv(pv1)
    
    glEnd()
    glBegin(GL_POINTS)
    glVertex2fv(pv1)
    glVertex2fv(pv0)
    glEnd()


hermitMat = np.array([])

def InitHermit(pv0,p0,p1,pv1):
    v0 = pv0 - p0
    v1 = pv1 - p1
    rev = np.array([
        [+2, -2, +1, +1],
        [-3, +3, -2, -1],
        [+0, +0, +1, +0],
        [+1, 00, 00, 00]
        ])
    global hermitMat
    hermitMat = rev @ np.array([p0,p1,v0,v1])
    
def HermitPoint(t):
    global hermitMat
    return np.array([t*t*t, t*t, t, 1]) @ hermitMat



def button_callback(window, button, action, mod):
    global p0, p1, gEditingPoint
    if button==glfw.MOUSE_BUTTON_LEFT:
        x, y = glfw.get_cursor_pos(window)
        y = 640 - y
        if action==glfw.PRESS:
            if np.abs(x-p0[0])<10 and np.abs(y-p0[1])<10:
                gEditingPoint = 'p0'
            elif np.abs(x-p1[0])<10 and np.abs(y-p1[1])<10:
                gEditingPoint = 'p1'
            if np.abs(x-pv0[0])<10 and np.abs(y-pv0[1])<10:
                gEditingPoint = 'pv0'
            elif np.abs(x-pv1[0])<10 and np.abs(y-pv1[1])<10:
                gEditingPoint = 'pv1'


        elif action==glfw.RELEASE:
            gEditingPoint = ''

def cursor_callback(window, xpos, ypos):
    global p0, p1, gEditingPoint
    ypos = 640 - ypos
    if gEditingPoint=='p0':
        p0[0]=xpos; p0[1]=ypos
    elif gEditingPoint=='p1':
        p1[0]=xpos; p1[1]=ypos
    if gEditingPoint=='pv0':
        pv0[0]=xpos; pv0[1]=ypos
    elif gEditingPoint=='pv1':
        pv1[0]=xpos; pv1[1]=ypos


def main():
    if not glfw.init():
        return
    window = glfw.create_window(640,640,'2013011082', None,None)
    if not window:
        glfw.terminate()
        return
    glfw.make_context_current(window)
    glfw.set_mouse_button_callback(window, button_callback)
    glfw.set_cursor_pos_callback(window, cursor_callback)

    while not glfw.window_should_close(window):
        glfw.poll_events()
        render()
        glfw.swap_buffers(window)

    glfw.terminate()

if __name__ == "__main__":
    main()




"""

import glfw
from OpenGL.GL import*
from OpenGL.GLU import*
import numpy as np
from OpenGL.arrays import vbo
import ctypes

gCamAng = np.radians(30)
gCamHeight = 1.
distance = 5.

def  drawUnitCube_glVertex():
    glBegin(GL_TRIANGLES)
    glNormal3f(0,1,0)# v0, v1, ...  v5 normal
    glVertex3f(0.5,0.5,-0.5)# v0 position
    glVertex3f(-0.5,0.5,-0.5)# v1 position
    glVertex3f(-0.5,0.5,0.5)# v2 position
    
    glVertex3f(0.5,0.5,-0.5)# v3 position
    glVertex3f(-0.5,0.5,0.5)# v4 position
    glVertex3f(0.5,0.5,0.5)# v5 position
    
    glNormal3f(0,-1,0)# v6, v7, ...  v11 normal
    glVertex3f(0.5,-0.5,0.5)# v6 position
    glVertex3f(-0.5,-0.5,0.5)# v7 position
    glVertex3f(-0.5,-0.5,-0.5)# v8 position

    glVertex3f(0.5,-0.5,0.5)# v9 position
    glVertex3f(-0.5,-0.5,-0.5)# v10 position
    glVertex3f(0.5,-0.5,-0.5)# v11 position

    glNormal3f(0,0,1)
    glVertex3f(0.5,0.5,0.5)
    glVertex3f(-0.5,0.5,0.5)
    glVertex3f(-0.5,-0.5,0.5)
    
    glVertex3f(0.5,0.5,0.5)
    glVertex3f(-0.5,-0.5,0.5)
    glVertex3f(0.5,-0.5,0.5)

    glNormal3f(0,0,-1)
    glVertex3f(0.5,-0.5,-0.5)
    glVertex3f(-0.5,-0.5,-0.5)
    glVertex3f(-0.5,0.5,-0.5)
    
    glVertex3f(0.5,-0.5,-0.5)
    glVertex3f(-0.5,0.5,-0.5)
    glVertex3f(0.5,0.5,-0.5)

    glNormal3f(-1,0,0)
    glVertex3f(-0.5,0.5,0.5)
    glVertex3f(-0.5,0.5,-0.5)
    glVertex3f(-0.5,-0.5,-0.5)
    
    glVertex3f(-0.5,0.5,0.5)
    glVertex3f(-0.5,-0.5,-0.5)
    glVertex3f(-0.5,-0.5,0.5)

    glNormal3f(1,0,0)
    glVertex3f(0.5,0.5,-0.5)
    glVertex3f(0.5,0.5,0.5)
    glVertex3f(0.5,-0.5,0.5)
    
    glVertex3f(0.5,0.5,-0.5)
    glVertex3f(0.5,-0.5,0.5)
    glVertex3f(0.5,-0.5,-0.5)

    glEnd()


def  createVertexArraySeparate():
    varr = np.array([[0,1,0],# v0 normal
    [0.5,0.5,-0.5],# v0 position
    [0,1,0],# v1 normal
    [-0.5,0.5,-0.5],# v1 position
    [0,1,0],# v2 normal
    [-0.5,0.5,0.5],# v2 position
    
    [0,1,0],# v3 normal
    [0.5,0.5,-0.5],# v3 position
    [0,1,0],# v4 normal
    [-0.5,0.5,0.5],# v4 position
    [0,1,0],# v5 normal
    [0.5,0.5,0.5],# v5 position
    
    [0,-1,0],# v6 normal
    [0.5,-0.5,0.5],# v6 position
    [0,-1,0],# v7 normal
    [-0.5,-0.5,0.5],# v7 position
    [0,-1,0],# v8 normal
    [-0.5,-0.5,-0.5],# v8 position
    
    [0,-1,0],
    [0.5,-0.5,0.5],
    [0,-1,0],
    [-0.5,-0.5,-0.5],
    [0,-1,0],
    [0.5,-0.5,-0.5],
    
    [0,0,1],
    [0.5,0.5,0.5],
    [0,0,1],
    [-0.5,0.5,0.5],
    [0,0,1],
    [-0.5,-0.5,0.5],
    
    [0,0,1],
    [0.5,0.5,0.5],
    [0,0,1],
    [-0.5,-0.5,0.5],
    [0,0,1],
    [0.5,-0.5,0.5],
    
    [0,0,-1],
    [0.5,-0.5,-0.5],
    [0,0,-1],
    [-0.5,-0.5,-0.5],
    [0,0,-1],
    [-0.5,0.5,-0.5],
    
    [0,0,-1],
    [0.5,-0.5,-0.5],
    [0,0,-1],
    [-0.5,0.5,-0.5],
    [0,0,-1],
    [0.5,0.5,-0.5],
    
    [-1,0,0],
    [-0.5,0.5,0.5],
    [-1,0,0],
    [-0.5,0.5,-0.5],
    [-1,0,0],
    [-0.5,-0.5,-0.5],
    
    [-1,0,0],
    [-0.5,0.5,0.5],
    [-1,0,0],
    [-0.5,-0.5,-0.5],
    [-1,0,0],
    [-0.5,-0.5,0.5],
    
    [1,0,0],
    [0.5,0.5,-0.5],
    [1,0,0],
    [0.5,0.5,0.5],
    [1,0,0],
    [0.5,-0.5,0.5],
    
    [1,0,0],
    [0.5,0.5,-0.5],
    [1,0,0],
    [0.5,-0.5,0.5],
    [1,0,0],
    [0.5,-0.5,-0.5],],'float32')
    
    return varr

 
def render(ang):
    global gCamAng,gCamHeight, distance
    
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glEnable(GL_DEPTH_TEST)
    
    glMatrixMode(GL_PROJECTION) # use projection matrix stack for projection transformation for correct
                                # lighting
    glLoadIdentity()
    gluPerspective(45,1,1,100)
    
    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()
    

    gluLookAt(distance * np.sin(gCamAng), distance * gCamHeight,distance * np.cos(gCamAng),0,0,0,0,1,0)

    drawFrame()

    glEnable(GL_LIGHTING)# try to uncomment: no lighting
    glEnable(GL_LIGHT0)
    glEnable(GL_LIGHT1)
    # light position
    
    glPushMatrix()

    #glRotatef(ang,0,1,0) # try to uncomment: rotate light
    lightPos1 = (1.,2.,3.,1.)# try to change 4th element to 0.  or 1.
    glLightfv(GL_LIGHT0,GL_POSITION,lightPos1)
    
    glPopMatrix()
    glPushMatrix()
    lightPos2 = (-1.,-2.,-3.,1.)# try to change 4th element to 0.  or 1.
    glLightfv(GL_LIGHT1,GL_POSITION,lightPos2)
    glPopMatrix()

    # light intensity for each color channel
    ambientLightColor0 = (.1,.1,.1,1.)
    diffuseLightColor0 = (1.,1.,1.,1.)
    specularLightColor0 = (1.,1.,1.,1.)
    glLightfv(GL_LIGHT0,GL_AMBIENT,ambientLightColor0)
    glLightfv(GL_LIGHT0,GL_DIFFUSE,diffuseLightColor0)
    glLightfv(GL_LIGHT0,GL_SPECULAR,specularLightColor0)
    
    ambientLightColor1 = (.1,.1,.1,1.)
    diffuseLightColor1 = (1.,1.,1.,1.)
    specularLightColor1 = (1.,1.,1.,1.)
    glLightfv(GL_LIGHT1,GL_AMBIENT,ambientLightColor1)
    glLightfv(GL_LIGHT1,GL_DIFFUSE,diffuseLightColor1)
    glLightfv(GL_LIGHT1,GL_SPECULAR,specularLightColor1)

    # material reflectance for each color channel
    diffuseObjectColor = (1.,1.,1.,1.)
    specularObjectColor = (1.,1.,1.,1.)
    glMaterialfv(GL_FRONT,GL_AMBIENT_AND_DIFFUSE,diffuseObjectColor)

    # glMaterialfv(GL_FRONT, GL_SPECULAR, specularObjectColor)
    glPushMatrix()

    # glRotatef(ang,0,1,0) # try to uncomment: rotate object
    #glColor3ub(0,0,255) # glColor*() is ignored if lighting is enabled

    # drawUnitCube_glVertex()
    # drawUnitCube_glDrawArray()
    drawUnitCube_glDrawElements()
    glPopMatrix()

    glDisable(GL_LIGHTING)


def drawUnitCube_glDrawArray():
    global gVertexArraySeparate
    
    varr = gVertexArraySeparate
    glEnableClientState(GL_VERTEX_ARRAY)
    glEnableClientState(GL_NORMAL_ARRAY)
    glNormalPointer(GL_FLOAT,6 * varr.itemsize,varr)
    glVertexPointer(3,GL_FLOAT,6 * varr.itemsize,ctypes.c_void_p(varr.ctypes.data + 3 * varr.itemsize))
    glDrawArrays(GL_TRIANGLES,0,int(varr.size / 6))

def drawFrame():
    glBegin(GL_LINES)
    glColor3ub(255,0,0)
    glVertex3fv(np.array([0.,0.,0.]))
    glVertex3fv(np.array([1.,0.,0.]))
    glColor3ub(0,255,0)
    glVertex3fv(np.array([0.,0.,0.]))
    glVertex3fv(np.array([0.,1.,0.]))
    glColor3ub(0,0,255)
    glVertex3fv(np.array([0.,0.,0]))
    glVertex3fv(np.array([0.,0.,1.]))
    glEnd()

def key_callback(window,key,scancode,action,mods):
    global gCamAng,gCamHeight, distance
    if action == glfw.PRESS or action == glfw.REPEAT:
        if key == glfw.KEY_1:
            gCamAng+=np.radians(-10)
        elif key == glfw.KEY_3:
            gCamAng+=np.radians(10)
        elif key == glfw.KEY_2:
            gCamHeight+=.1
        elif key == glfw.KEY_W:
            gCamHeight+=-.1
        elif key == glfw.KEY_S:
            distance +=1
        elif key == glfw.KEY_A:
            distance -= 1
            if(distance < 1):
                distance = 1
        
def drop_callback(window, cbfun):



         
    gVertexArraySeparate = None



 
def createVertexAndIndexArrayIndexed():
    varr=np.array([
    normalized([+1 ,+1, -1]),
    [0.5,0.5,-0.5],
    normalized([-1 ,+1, -1]),
    [-0.5,0.5,-0.5],
    normalized([-1 ,+1, +1]),
    [-0.5,0.5,0.5],
    normalized([+1 ,+1, +1]),
    [0.5,0.5,0.5],
    normalized([+1 ,-1, +1]),
    [0.5,-0.5,0.5],
    normalized([-1 , -1, +1]),
    [-0.5,-0.5,0.5],
    normalized([-1 , -1, -1]),
    [-0.5,-0.5,-0.5],
    normalized([+1, -1 , -1]),
    [0.5,-0.5,-0.5],
    ],'float32')
    
    iarr=np.array([
    [0,1,2],
    [0,2,3],
    [4,5,6],
    [4,6,7],
    [3,2,5],
    [3,5,4],
    [7,6,1],
    [7,1,0],
    [2,1,6],
    [2,6,5],
    [0,3,4],
    [0,4,7],
    ])

    return varr,iarr

def l2norm(v):
    return np.sqrt(np.dot(v,v))

def normalized(v):
    l =l2norm(v)
    return 1/l *np.array(v)

def drawUnitCube_glDrawElements():
    global gVertexArrayIndexed,gIndexArray
    varr=gVertexArrayIndexed
    iarr=gIndexArray

    glEnableClientState(GL_VERTEX_ARRAY)
    glEnableClientState(GL_NORMAL_ARRAY)

    glNormalPointer(GL_FLOAT,6*varr.itemsize,varr)
    glVertexPointer(3,GL_FLOAT,6*varr.itemsize,ctypes.c_void_p(varr.ctypes.data+3*varr.itemsize))

    glDrawElements(GL_TRIANGLES,iarr.size,GL_UNSIGNED_INT,iarr)
    
gVertexArrayIndexed=None
gIndexArray=None

def main():
    global gVertexArraySeparate
    global gVertexArrayIndexed,gIndexArray

    if not glfw.init():
        return

    window =glfw.create_window(640,640,'2013011082',None,None)

    if not window:
        glfw.terminate()
        return

    glfw.make_context_current(window)
    glfw.set_key_callback(window,key_callback)
    glfw.swap_interval(1)

    gVertexArraySeparate=createVertexArraySeparate()
    gVertexArrayIndexed,gIndexArray=createVertexAndIndexArrayIndexed()
    count =0

    while not glfw.window_should_close(window):
        glfw.poll_events()
        ang=count %360
        render(ang)
        count +=1
        glfw.swap_buffers(window)

    glfw.terminate()


if __name__ == "__main__":
    main()

 """