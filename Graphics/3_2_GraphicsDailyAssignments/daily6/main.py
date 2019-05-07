import glfw
from OpenGL.GL import *
import numpy as np

globalT  = np.array([[1.,.0,.0],[0.,1.,.0],[.0,.0,1.]])
moveXp01 = np.array([[1.,.0,+.1],[0.,1.,.0],[.0,.0,1.]])
moveXn01 = np.array([[1.,.0,-.1],[0.,1.,.0],[.0,.0,1.]])
d10 = np.radians(10);  #10도
clock10  = np.array([[np.cos(-d10), - np.sin(-d10), .0],[np.sin(-d10),np.cos(-d10), .0], [.0, .0, 1.]])
cclock10 = np.array([[np.cos(+d10), - np.sin(+d10), .0],[np.sin(+d10),np.cos(+d10), .0], [.0, .0, 1.]])
initT = globalT

#scaleX09 = np.array([[ .9,0.],[0.,1.]])
#scaleX11 = np.array([[1.1,0.],[0.,1.]])

#shearXp01= np.array([[1., +0.1], [0.,1.]])
#shearXn01= np.array([[1., -0.1], [0.,1.]])
#reflectX = np.array([[1., 0.], [0.,-1.]])



def key_callback(window, key, scancode, action, mods):
    if action == glfw.PRESS:
        global globalT
        if   (key == glfw.KEY_Q):
            global moveXn01
            globalT = moveXn01 @ globalT
        elif (key == glfw.KEY_E):
            global moveXp01
            globalT = moveXp01 @ globalT
        elif (key == glfw.KEY_A):
            global cclock10
            globalT = cclock10 @ globalT
        elif (key == glfw.KEY_D):
            global clock10
            globalT = clock10 @ globalT
        elif (key == glfw.KEY_1):
            global initT
            globalT = initT
        
        #print(globalT)


def render(T):
    glClear(GL_COLOR_BUFFER_BIT)
    glLoadIdentity()

    #draw coordinate
    glBegin(GL_LINES)
    glColor3ub(255,0,0)
    glVertex2fv(np.array([0.,0.]))
    glVertex2fv(np.array([1.,0.]))
    glColor3ub(0,255,0)
    glVertex2fv(np.array([0.,0.]))
    glVertex2fv(np.array([0.,1.]))
    glEnd()

    # draw triangle
    glBegin(GL_TRIANGLES)
    glColor3ub(255, 255, 255)
    glVertex2fv((T @ np.array([0., .5, 1.]))[:-1])
    glVertex2fv((T @ np.array([0., 0., 1.]))[:-1])
    glVertex2fv((T @ np.array([.5, 0., 1.]))[:-1])
    glEnd()

def main():
    if not glfw.init():
        return
    window = glfw.create_window(480, 480, "2013011082 이상옥", None, None)
    if not window:
        glfw.terminate()
        return
    glfw.make_context_current(window)
    
    count = 0
    T = np.array([[1.,0.],[0.,1.]])
    while not glfw.window_should_close(window):
        glfw.poll_events()
        glfw.set_key_callback(window, key_callback)
        global globalT
        render(globalT)
        glfw.swap_buffers(window)

    glfw.terminate()

main()
