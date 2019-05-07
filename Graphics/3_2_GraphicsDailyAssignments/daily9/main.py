import glfw
from OpenGL.GL  import *
from OpenGL.GLU import *
import numpy as np


d10 = np.radians(10);
mvXp01   = np.array([[1., 0., 0.,+.1],
                     [0., 1., 0., 0.],
                     [0., 0., 1., 0.],
                     [0., 0., 0., 1.]])
mvXn01   = np.array([[1., 0., 0.,-.1],
                     [0., 1., 0., 0.],
                     [0., 0., 1., 0.],
                     [0., 0., 0., 1.]])

rotXn10  = np.array([[1.,           0.,           0., 0.],
                     [0., np.cos(-d10),-np.sin(-d10), 0.],
                     [0., np.sin(-d10), np.cos(-d10), 0.],
                     [0.,           0.,           0., 1.]])
rotXp10  = np.array([[1.,           0.,           0., 0.],
                     [0., np.cos(+d10),-np.sin(+d10), 0.],
                     [0., np.sin(+d10), np.cos(+d10), 0.],
                     [0.,           0.,           0., 1.]])
rotYn10  = np.array([[np.cos(-d10), 0.,-np.sin(-d10), 0.],
                     [0.,           1.,           0., 0.],
                     [np.sin(-d10), 0., np.cos(-d10), 0.],
                     [0.,           0.,           0., 1.]])
rotYp10  = np.array([[np.cos(+d10), 0.,-np.sin(+d10), 0.],
                     [0.,           1.,           0., 0.],
                     [np.sin(+d10), 0., np.cos(+d10), 0.],
                     [0.,           0.,           0., 1.]])

globalM  = np.array([[1.,0.,0.,0.],
                     [0.,1.,0.,0.],
                     [0.,0.,1.,0.],
                     [0.,0.,0.,1.]])
camAng  = 0.

def key_callback(window, key, scancode, action, mods):
    if action == glfw.PRESS or action == glfw.REPEAT:
        global globalM
        global camAng
        if   (key == glfw.KEY_Q):
            global mvXn01
            globalM = mvXn01 @ globalM
        elif (key == glfw.KEY_E):
            global mvXp01
            globalM = mvXp01 @ globalM

        elif (key == glfw.KEY_A):
            global rotYp10
            globalM = globalM @ rotYp10
        elif (key == glfw.KEY_D):
            global rotYn10
            globalM = globalM @ rotYn10

        elif (key == glfw.KEY_W):
            global rotXn10
            globalM = globalM @ rotXn10
        elif (key == glfw.KEY_S):
            global rotXp10
            globalM = globalM @ rotXp10

        elif (key == glfw.KEY_1):
            camAng -= np.radians(10)
        elif (key == glfw.KEY_3):
            camAng += np.radians(10)

        print(globalM)

def drawBox():
    glBegin(GL_QUADS)
    glVertex3fv((np.array([+1., +1.,0., 1.]))[:-1])
    glVertex3fv((np.array([-1., +1.,0., 1.]))[:-1])
    glVertex3fv((np.array([-1., -1.,0., 1.]))[:-1])
    glVertex3fv((np.array([+1., -1.,0., 1.]))[:-1])
    glEnd()

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

def render(camAng, count):
    # depth test??
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glEnable(GL_DEPTH_TEST)
    glLoadIdentity()

    # orthogonal projection   ???
    glOrtho(-1,+1, -1,+1, -1,+1)

    # rotate camera pos
    gluLookAt(.1 * np.sin(camAng), .1, .1*np.cos(camAng),
                0,0,0,
                0,1,0)

    drawFrame()
    glPushMatrix()

    #pos of blue box
    glTranslatef(-.5 + (count%360) *.003, 0, 0)

    #draw blue box
    glPushMatrix()
    glScalef(.2, .2, .2)
    glColor3ub(0,0,255)
    drawBox()
    glPopMatrix()

    #red arm pos, rot
    glPushMatrix()
    drawFrame()
    glRotatef(count%360,0,0,1)
    glTranslatef(.5,0.,.01)

    #red arm draw (and scale)
    glPushMatrix()
    drawFrame()
    glScalef(.5,.1,.1)
    glColor3ub(255,0,0)
    drawBox()
    glPopMatrix()

    #green
    glTranslatef(.5,0.,.01)
    glPushMatrix()
    drawFrame()
    glRotatef(count%360,0,0,1)
    glScalef(.2, .2, .2)
    glPushMatrix()
    glColor3ub(0, 255,0)
    drawBox()
    glPopMatrix()

    glPopMatrix()
    glPopMatrix()
    glPopMatrix()



def main():
    if not glfw.init():
        return
    window = glfw.create_window(640, 640, "2013011082 이상옥", None, None)
    if not window:
        glfw.terminate()
        return
    glfw.make_context_current(window)
    glfw.swap_interval(1)
    count = 0
    #translate by (.4, 0., .2)  // 수평이동
    global camAng
    global globalM
    camAng = 0.
    while not glfw.window_should_close(window):
        glfw.poll_events()
        glfw.set_key_callback(window, key_callback)
        render(camAng, count)
        count += 1
        glfw.swap_buffers(window)

    glfw.terminate()

main()