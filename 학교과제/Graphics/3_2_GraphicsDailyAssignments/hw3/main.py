import glfw
import numpy as np
from OpenGL.GL import*
from OpenGL.GLU import*
from OpenGL.arrays import vbo

gWireFrameMode = False
gCamAng = np.radians(30)
gCamHeight = 1.
distance = 5.

gFaceWith3 = 0
gFaceWith4 = 0
gFaceWith4More= 0

def main():
    global gVertexArrayIndexed,gIndexArray

    if not glfw.init():
        return

    window =glfw.create_window(640,640,'2013011792',None,None)

    if not window:
        glfw.terminate()
        return

    glfw.make_context_current(window)
    glfw.set_key_callback(window,key_callback)
    glfw.set_drop_callback(window,drop_callback)
    glfw.swap_interval(1)

    gVertexArrayIndexed,gIndexArray = createVertexAndIndexArrayIndexed()
    count =0

    while not glfw.window_should_close(window):
        glfw.poll_events()
        ang=count %360
        render(ang)
        count +=1
        glfw.swap_buffers(window)

    glfw.terminate()


def SetLightMaterial():
    glEnable(GL_LIGHTING)
    glEnable(GL_LIGHT0)
    glEnable(GL_LIGHT1)
    
    lightPos1 = (10.,0.,0.,1.)
    glLightfv(GL_LIGHT0,GL_POSITION,lightPos1)
    
    
    lightPos2 = (0.,10.,0.,1.)
    glLightfv(GL_LIGHT1,GL_POSITION,lightPos2)
    

    # light intensity for each color channel
    
    ambientLightColor0 = (.1,0.,0.,1.)
    diffuseLightColor0 = (1.,0.,0.,1.)
    specularLightColor0= (1.,0.,0.,1.)
    glLightfv(GL_LIGHT0,GL_AMBIENT,ambientLightColor0)
    glLightfv(GL_LIGHT0,GL_DIFFUSE,diffuseLightColor0)
    glLightfv(GL_LIGHT0,GL_SPECULAR,specularLightColor0)

    ambientLightColor1 = (0.,0.,.1,1.)
    diffuseLightColor1 = (0.,0.,1.,1.)
    specularLightColor1= (0.,0.,1.,1.)
    glLightfv(GL_LIGHT1,GL_AMBIENT,ambientLightColor1)
    glLightfv(GL_LIGHT1,GL_DIFFUSE,diffuseLightColor1)
    glLightfv(GL_LIGHT1,GL_SPECULAR,specularLightColor1)
    
    # material reflectance for each color channel
    diffuseObjectColor = (1.,1.,1.,1.)
    specularObjectColor = (1.,1.,1.,1.)
    glMaterialfv(GL_FRONT,GL_AMBIENT_AND_DIFFUSE,diffuseObjectColor)
    
 
def SetCam():
    global gCamAng,gCamHeight, distance
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glEnable(GL_DEPTH_TEST)
    
    glMatrixMode(GL_PROJECTION)
    
    glLoadIdentity()
    gluPerspective(45,1,1,100)
    
    glMatrixMode(GL_MODELVIEW)
    glLoadIdentity()

    gluLookAt(distance * np.sin(gCamAng), distance * gCamHeight,distance * np.cos(gCamAng),0,0,0,0,1,0)


def DrawFile():
    global gVertexArrayIndexed,gIndexArray, gWireFrameMode
    varr=gVertexArrayIndexed
    iarr=gIndexArray

    glEnableClientState(GL_VERTEX_ARRAY)
    glEnableClientState(GL_NORMAL_ARRAY)

    glNormalPointer(GL_FLOAT,6*varr.itemsize,varr)
    glVertexPointer(3,GL_FLOAT,6*varr.itemsize,ctypes.c_void_p(varr.ctypes.data+3*varr.itemsize))
    
    if(gWireFrameMode) :
        glPolygonMode(GL_FRONT_AND_BACK, GL_LINE)
    else :
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL)

    glDrawArrays(GL_TRIANGLES,0,int(varr.size/6))

def render(ang):
    SetCam()
    SetLightMaterial()

    DrawFile()
    glDisable(GL_LIGHTING)  # drawed file will be affected by SetLightMaterial
    


def key_callback(window,key,scancode,action,mods):
    global gCamAng,gCamHeight,distance, gWireFrameMode
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
            distance += .1
        elif key == glfw.KEY_A:
            distance -= .1
            if distance < 0.:
                distance = 0.
        elif key == glfw.KEY_Z:
            gWireFrameMode = not gWireFrameMode


def drop_callback(window, path):
    global gVertexArrayIndexed, gIndexArray, gFaceWith3, gFaceWith4, gFaceWith4More
    gFaceWith3 = 0
    gFaceWith4 = 0
    gFaceWith4More = 0

    v = []
    vn = []
    varr = []
    n = []
    iarr = []
    
    with open(path[0], 'r') as f:
        for line in f:
            if line[:2] == "v ":
                index1 = line.find(" ") + 1
                index2 = line.find(" ", index1 + 1)
                index3 = line.find(" ", index2 + 1)

                vertex = [float(line[index1:index2]), float(line[index2:index3]), float(line[index3:-1])]
                v.append(vertex)
                
            elif line[:3] == "vn ":
                index1 = line.find(" ") + 1
                index2 = line.find(" ", index1 + 1)
                index3 = line.find(" ", index2 + 1)

                vertex = [float(line[index1:index2]), float(line[index2:index3]), float(line[index3:-1])]
                vn.append(vertex)
                
                
            elif line[0] == "f":
                string = line.replace("//", " ")
                i = string.find(" ") + 1
                face = []
                for item in range(string.count(" ")):
                    if string.find(" ", i) == -1:
                        face.append(string[i:-1])
                        break
                    face.append(string[i:string.find(" ", i)])
                    i = string.find(" ", i) + 1
                if(len(face) == 6): 
                    gFaceWith3 +=1
                elif(len(face) == 8):
                    gFaceWith4 +=1
                else:
                    gFaceWith4More +=1

                while (True):
                    l = len(face)
                    if(l == 6):
                        n.append(tuple(face))
                        break;
                    subf = []
                    subf.append(face[0])        #vertex[0]   옮김
                    subf.append(face[1])
                    
                    subf.append(face[-4])       #vertex[-2] 옮김
                    subf.append(face[-3])       #vertex[-2] 옮김
                    subf.append(face[-2])       #vertex[-1] 옮김
                    subf.append(face[-1])       #vertex[-1] 옮김
                    
                    
                    n.append(subf)              #옮긴 3개 점으로 삽입
                    face = face[0 : -2]         #vertex[-1] 제거
                

    for i in range(0, len(n)):
        for j in range(0, len(n[i])):
            if j%2 == 1:
                varr.append(v[int(n[i][j-1])-1])
            elif j%2 ==0:
                varr.append(vn[int(n[i][j+1])-1])

    varr = np.asarray(varr,'float32')
    gVertexArrayIndexed, gIndexArray = varr, iarr
    
    i = 0
    while(True):
        j = path[0].find("\\", i+1)
        if(j == -1 or i > len(path[0])):
            break
        else :
            i = j

    file_name = path[0][i+1:-1]
    print("file name        :" + file_name)
    print(("face total      :%d" % (gFaceWith3 + gFaceWith4 + gFaceWith4More)))
    print(("face with 3     :%d" % gFaceWith3))
    print(("face with 4     :%d" % gFaceWith4))
    print(("face More Than5 :%d" % gFaceWith4More))



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

    
   
gVertexArrayIndexed=None
gIndexArray=None         
 

if __name__ == "__main__":
    main()
 