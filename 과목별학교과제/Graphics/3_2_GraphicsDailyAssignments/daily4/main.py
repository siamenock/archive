import glfw
from OpenGL.GL import *
import numpy as np

display_mode = GL_LINE_LOOP


def key_callback(window, key, scancode, action, mods):
    global display_mode
    display_mode = {   #switch
        glfw.KEY_1 : GL_POINTS,
        glfw.KEY_2 : GL_LINES,
        glfw.KEY_3 : GL_LINE_STRIP,
        glfw.KEY_4 : GL_LINE_LOOP,
        glfw.KEY_5 : GL_TRIANGLES,
        glfw.KEY_6 : GL_TRIANGLE_STRIP,
        glfw.KEY_7 : GL_TRIANGLE_FAN,
        glfw.KEY_8 : GL_QUADS,
        glfw.KEY_9 : GL_QUAD_STRIP,
        glfw.KEY_0 : GL_POLYGON
    }[key]
    print(display_mode)


def render(type):
    glClear(GL_COLOR_BUFFER_BIT)
    glLoadIdentity()
    glBegin(type)

    pol_edg = 12
    angle = np.pi * 2 / pol_edg

    for i in range(0, pol_edg):
        glVertex2f(np.cos(angle*i), np.sin(angle*i))

    
    #glColor3f(1,0,0)

    glEnd()

def main():
    if not glfw.init():
        return
    window = glfw.create_window(480,480,"2013011082 이상옥", None, None)
    if not window:
        glfw.terminate()
        return
    glfw.make_context_current(window)

    type = GL_LINE_LOOP;
    while not glfw.window_should_close(window):
        glfw.poll_events()
        glfw.set_key_callback(window, key_callback) # it chage global var display_mode
        render(display_mode)    
        glfw.swap_buffers(window)

    glfw.terminate()

main()