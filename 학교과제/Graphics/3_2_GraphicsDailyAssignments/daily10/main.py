import glfw
from OpenGL.GL  import *
from OpenGL.GLU import *
import numpy as np


eye = np.array([.0, .1, .1])
at  = np.array([0.2,0.2,0.2])
up  = np.array([0,1,0])

count = 0
camAng = 0

def key_callback(window, key, scancode, action, mods):
    if action == glfw.PRESS or action == glfw.REPEAT:
        global camAng, eye, at, up
        if (key == glfw.KEY_1):
            camAng -= 10
        elif (key == glfw.KEY_3):
            camAng += 10
        rad = np.radians(camAng)
        eye = np.array([np.sin(rad), 1., np.cos(rad)])
        




def drawFrame():
    glBegin(GL_LINES)
    glColor3ub(255,0,0)
    glVertex3fv(np.array([0.,0.,0.]))
    glVertex3fv(np.array([1.,0.,0.]))
    glColor3ub(0,255,0)
    glVertex3fv(np.array([0.,0.,0.]))
    glVertex3fv(np.array([0.,1.,0.]))
    glColor3ub(0,0,255)
    glVertex3fv(np.array([0.,0.,0.]))
    glVertex3fv(np.array([0.,0.,1.]))
    glEnd()


def myLookAt(eye, at, up):
    w = eye - at
    w /= np.sqrt(np.dot(w,w))
    u = np.cross(up, w)
    u /= np.sqrt(np.dot(u,u))
    v = np.cross(w, u)

    M = np.identity(4)
    M[0,:3] = u
    M[1,:3] = v
    M[2,:3] = w
    a = np.dot(-u, eye)
    b = np.dot(-v, eye)
    c = np.dot(-w, eye)
    M[3,:3] = np.array([a,b,c])

    glMultMatrixf(M.T)

def render():
    global eye, at, up, camAng
    myLookAt(eye, at, up)
    # depth test??
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glEnable(GL_DEPTH_TEST)
    glLoadIdentity()

    # orthogonal projection   ???
    glOrtho(-1,+1, -1,+1, -1,+1)



    # rotate camera pos
    

    
    glPushMatrix()                  #0
    myLookAt(eye, at, up)

    drawFrame()

    glPopMatrix()                   #0

    




def main():
    if not glfw.init():
        return
    window = glfw.create_window(640, 640, "2013011082 (히오스 마크가 필요해)", None, None)
    if not window:
        glfw.terminate()
        return
    glfw.make_context_current(window)
    glfw.swap_interval(1)
    
    
    #translate by (.4, 0., .2)  // 수평이동
    global camAng
    global count
    count = 0
    camAng = np.sin(10)

    while not glfw.window_should_close(window):
        glfw.poll_events()
        glfw.set_key_callback(window, key_callback)
        render()
        count += 1
        glfw.swap_buffers(window)

    glfw.terminate()

main()