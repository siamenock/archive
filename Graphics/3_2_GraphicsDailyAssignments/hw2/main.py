import glfw
from OpenGL.GL  import *
from OpenGL.GLU import *
import numpy as np

count = 0
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
            global rotYp90
            globalM = globalM @ rotYp90
        elif (key == glfw.KEY_D):
            global rotYn90
            globalM = globalM @ rotYn90

        elif (key == glfw.KEY_W):
            global rotXn90
            globalM = globalM @ rotXn90
        elif (key == glfw.KEY_S):
            global rotXp90
            globalM = globalM @ rotXp90

        elif (key == glfw.KEY_1):
            camAng -= np.radians(10)
        elif (key == glfw.KEY_3):
            camAng += np.radians(10)

        print(globalM)

def randomMove():
    global count
    x = (count%200)/200
    y = (count%900)/900
    z = (count%500)/500

    x -= .5
    y -= .5
    z -= .5
    glTranslatef(x,y,z)

def randomRotate():
    glRotatef(count, 0,1,0)


def drawCube():
    glBegin(GL_QUADS);
    glVertex3fv(np.array([+1., +1.,-1.]))
    glVertex3fv(np.array([-1., +1.,-1.]))
    glVertex3fv(np.array([-1., -1.,-1.]))
    glVertex3fv(np.array([+1., -1.,-1.]))

    glVertex3fv(np.array([+1., +1.,+1.]))
    glVertex3fv(np.array([-1., +1.,+1.]))
    glVertex3fv(np.array([-1., -1.,+1.]))
    glVertex3fv(np.array([+1., -1.,+1.]))
    
    glVertex3fv(np.array([+1., +1., +1.]))
    glVertex3fv(np.array([+1., -1., +1.]))
    glVertex3fv(np.array([+1., -1., -1.]))
    glVertex3fv(np.array([+1., +1., -1.]))

    glVertex3fv(np.array([-1., +1., +1.]))
    glVertex3fv(np.array([-1., -1., +1.]))
    glVertex3fv(np.array([-1., -1., -1.]))
    glVertex3fv(np.array([-1., +1., -1.]))

    glVertex3fv(np.array([+1., -1., +1.]))
    glVertex3fv(np.array([-1., -1., +1.]))
    glVertex3fv(np.array([-1., -1., -1.]))
    glVertex3fv(np.array([+1., -1., -1.]))

    glVertex3fv(np.array([+1., +1., +1.]))
    glVertex3fv(np.array([-1., +1., +1.]))
    glVertex3fv(np.array([-1., +1., -1.]))
    glVertex3fv(np.array([+1., +1., -1.]))
    glEnd()


def drawBox():
    glBegin(GL_QUADS)
    glVertex3fv((np.array([+1., +1.,0., 1.]))[:-1])
    glVertex3fv((np.array([-1., +1.,0., 1.]))[:-1])
    glVertex3fv((np.array([-1., -1.,0., 1.]))[:-1])
    glVertex3fv((np.array([+1., -1.,0., 1.]))[:-1])
    glEnd()

def drawBoxPlus():
    drawBox()
    z = 0
    move = count%200
    if move < 100 :
        z = (count % 100)/100
    else :
        reverse = 200 - count
        z = (reverse % 200)/100
    
    z = z*4 -2
    glTranslate(0,0,z)
    glScale( .2, .2, .2)
    drawCube()

def drawBoxHios():
    global count
    glPushMatrix()

    glRotate(count,0,0,1)
    drawBoxPlus()

    
    glPopMatrix()


def drawBoxHiosPairUpDown():
    glPushMatrix()
    glScalef(.501, .501, .501,)

    glTranslatef(0,0,+2)
    drawBoxHios()
    glTranslatef(0,0,-4)
    drawBoxHios()
    glPopMatrix()

def drawCubeHios():
    glPushMatrix()          #0
    glScalef(.999,.999,.999)
    glColor3ub(128,128,128)
    drawCube()
    glPopMatrix()           #0

    glColor3ub(255,0,0)
    glPushMatrix()          #1
    drawBoxHiosPairUpDown()
    glPopMatrix()           #1
    
    glColor3ub(0,255,0)
    glPushMatrix()          #2
    glRotatef(90., 0., 1., 0.)
    drawBoxHiosPairUpDown()
    glPopMatrix()           #2

    glColor3ub(0,0, 255)
    glPushMatrix()          #3
    glRotatef(90., 1., 0., 0,)
    drawBoxHiosPairUpDown()
    glPopMatrix()           #3


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

    glPushMatrix()                  #0
    randomMove()
    randomRotate()

    glPushMatrix()                  #1
    glScalef(.25, .25, .25)         # all items will be made in 1/4 size
    
    glPushMatrix()                  #2
    drawCubeHios()

    glPopMatrix()                   #2
    glPopMatrix()                   #1
    glPopMatrix()                   #0

    drawFrame()




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
        render(camAng, count)
        count += 1
        glfw.swap_buffers(window)

    glfw.terminate()

main()