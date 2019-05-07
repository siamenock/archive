import glfw
from OpenGL.GL import *
import numpy as np
d10 = np.radians(10);
globalT  = np.array([[1.,0.],[0.,1.]])
scaleX09 = np.array([[ .9,0.],[0.,1.]])
scaleX11 = np.array([[1.1,0.],[0.,1.]])
clock10  = np.array([[np.cos(-d10), - np.sin(-d10)],[np.sin(-d10),np.cos(-d10)]])
cclock10 = np.array([[np.cos(+d10), - np.sin(+d10)],[np.sin(+d10),np.cos(+d10)]])
shearXp01= np.array([[1., +0.1], [0.,1.]])
shearXn01= np.array([[1., -0.1], [0.,1.]])
reflectX = np.array([[1., 0.], [0.,-1.]])
initT = globalT


def key_callback(window, key, scancode, action, mods):
    if action == glfw.PRESS:
        global globalT
        if   (key == glfw.KEY_W):
            global scaleX09
            globalT = scaleX09 @ globalT
        elif (key == glfw.KEY_E):
            global scaleX11
            globalT = scaleX11 @ globalT
        elif (key == glfw.KEY_S):
            global cclock10
            globalT = cclock10 @ globalT
        elif (key == glfw.KEY_D):
            global clock10
            globalT = clock10 @ globalT
        elif (key == glfw.KEY_X):
            global shearXn01
            globalT = shearXn01 @ globalT
        elif (key == glfw.KEY_C):
            global shearXp01
            globalT = shearXp01 @ globalT
        elif (key == glfw.KEY_R):
            global reflectX
            globalT = reflectX @ globalT
        elif (key == glfw.KEY_1):
            global initT
            globalT = initT
        print(globalT)


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
    glVertex2fv(T @ np.array([0., .5]))
    glVertex2fv(T @ np.array([0., 0.]))
    glVertex2fv(T @ np.array([.5, 0.]))
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