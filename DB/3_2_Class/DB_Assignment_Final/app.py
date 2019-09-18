from flask import Flask, render_template, redirect, request
import psycopg2 as pg
import psycopg2.extras
import codecs


app        = Flask(__name__)
db_connect = {
    'host'      : '127.0.0.1'   ,
    'dbname'    : 'postgres'    ,
    'port'      : '5432'        ,
    'user'      : 'postgres'    ,
    'password'  : 'password'
}
CONNECT_STR = "host={host} user={user} dbname={dbname} password={password} port={port}".format(**db_connect)

LOGIN_TABLE_NAME = ['Customer', 'Seller', 'Delivery']   # 순서대로 로그인 우선순위
LOGIN_ID         = [-1,-1,-1]

def Sstr(val):              # val -> 'val'
    return "'" + val + "'"

##-----------------------------------------------------##
##                      공통 부분                       ##
##-----------------------------------------------------##

@app.route('/')
def init_page():
    print("init page!------------------------------")
    return render_template("loginPage.html")

@app.route('/p/<page_name>')
def static_page(page_name):
    return render_template(page_name + ".html")


@app.route('/signIn', methods=['POST'] )
def SignIn():
    print ('try sign in')
    
    table    = request.form.get('accountType')
    name     = Sstr(request.form.get('name' ))
    email    = Sstr(request.form.get('email'))
    passwd   = Sstr(request.form.get('password'))
    phone    = Sstr(request.form.get('phone'))
    
    sql = ("INSERT INTO " + table
        + " (email, passwd, name, phone)"
        + "VALUES ("
        + email+","+passwd+","+name+","+phone+");"
    )
    print(sql)

    conn     = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()

    return render_template("loginPage.html")


@app.route('/logIn', methods=['POST'] )
def LogIn():
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)

    email    = Sstr(request.form.get('email' ))
    passwd   = Sstr(request.form.get('passwd'))

    global LOGIN_ID   
    global LOGIN_TABLE_NAME
    LOGIN_ID = [-1,-1,-1]

    for i in range(len(LOGIN_TABLE_NAME)) :
        table = LOGIN_TABLE_NAME[i]
        
        sql = "SELECT * FROM " + table + " WHERE email=" + email + " AND passwd =" +passwd      # [;] 필요없네?
        cur.execute(sql)
        rows = cur.fetchall()
        if len(rows) != 0 :
            LOGIN_ID[i] = rows[0][0]      # save id
        print("Login result as "+ table + " =====================")
        print(rows)
    print("total result======================" )
    print(LOGIN_ID)
    conn.commit()
    conn.close()

    if LOGIN_ID[0]==-1 and LOGIN_ID[1]==-1 and LOGIN_ID[2]==-1 :
        return redirect('/')

    return redirect('/main')


@app.route('/main')
def MainPage():
    global LOGIN_ID   
    global LOGIN_TABLE_NAME
    print("total result======================" )
    print(LOGIN_ID)
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)

    data = []
    for i in range(len(LOGIN_TABLE_NAME)):
        sql = "SELECT email, passwd, name, phone FROM "+LOGIN_TABLE_NAME[i]+" WHERE id"+LOGIN_TABLE_NAME[i]+"= " + str(LOGIN_ID[i])
        print(sql)
        cur.execute(sql)
        conn.commit()
        rows = cur.fetchall()

        if(len(rows) == 0):
            data.append(None)
            continue
        data.append(rows[0])
    

    dataCustomer= [data[0]]
    dataSeller  = [data[1]]
    dataDelivery= [data[2]]
    print("data list =======================" )
    print(dataCustomer)
    print(dataSeller)
    print(dataDelivery)

    if(dataCustomer[0] != None):
        sql = "SELECT idOrder, idStore, idDelivery, idPaymethod, price FROM Order_ WHERE idCustomer=" + str(LOGIN_ID[0])
        print(sql)
        cur.execute(sql)
        order_list = cur.fetchall()
        
        for order in order_list:
            sql = "SELECT name FROM Store WHERE idStore=" + str(order[1])
            cur.execute(sql)
            order[1] = cur.fetchall()[0][0]

            if order[2] != None :
                sql = "SELECT name FROM Delivery WHERE idDelivery=" + str(order[2])
                cur.execute(sql)
                order[2] = cur.fetchall()[0][0]
            

            sql = "SELECT name FROM PayMethod WHERE idPayMethod=" + str(order[3])
            cur.execute(sql)
            order[3] = cur.fetchall()[0][0]

            

            sql = "SELECT idMenu, quantity FROM Choose WHERE idOrder="+str(order[0])
            cur.execute(sql)
            choose_list = cur.fetchall()
            for choose in choose_list :
                sql = "SELECT name FROM Menu WHERE idMenu="+str(choose[0])
                cur.execute(sql)
                choose[0] = cur.fetchall()[0][0]
            
            order.append(choose_list)
        
        
        dataCustomer.append(order_list)
        print(dataCustomer)    

    else :
        dataCustomer = []

    if(dataSeller[0] != None):
        sql = "SELECT idStore, name, descript, longitude, latitude FROM Store WHERE idSeller=" + str(LOGIN_ID[1])
        print(sql)
        cur.execute(sql)
        rows = cur.fetchall()
        dataSeller.append(rows)
    else :
        dataSeller = []

    if(dataDelivery[0] != None):
        sql = "SELECT idOrder, idStore, idCustomer, idPaymethod, price, longitude, latitude FROM Order_ WHERE idDelivery=" + str(LOGIN_ID[2])
        print(sql)
        cur.execute(sql)
        order_list = cur.fetchall()
        
        for order in order_list:
            sql = "SELECT name FROM Store WHERE idStore=" + str(order[1])
            cur.execute(sql)
            order[1] = cur.fetchall()[0][0]

            if order[2] != None :
                sql = "SELECT name FROM Customer WHERE idCustomer=" + str(order[0])
                cur.execute(sql)
                order[2] = cur.fetchall()[0][0]
            

            sql = "SELECT name FROM PayMethod WHERE idPayMethod=" + str(order[3])
            cur.execute(sql)
            order[3] = cur.fetchall()[0][0]

            

            sql = "SELECT idMenu, quantity FROM Choose WHERE idOrder="+str(order[0])
            cur.execute(sql)
            choose_list = cur.fetchall()
            for choose in choose_list :
                sql = "SELECT name FROM Menu WHERE idMenu="+str(choose[0])
                cur.execute(sql)
                choose[0] = cur.fetchall()[0][0]
            
            order.append(choose_list)
        
        
        dataDelivery.append(order_list)
    else :
        dataDelivery = []

    conn.close()

    print("getData===========")
    print(dataCustomer)
    print(dataSeller)
    print(dataDelivery)

    return render_template("mainPage.html",
                            data_customer=dataCustomer, data_seller=dataSeller, data_delivery=dataDelivery,
                            login = LOGIN_ID
                            )

@app.route('/updatePersonalInfo', methods=['POST'])
def UpdatePersonalInfo():
    idType  = int(request.form.get('idType'))
    name    = Sstr(request.form.get('name' ))
    passwd  = Sstr(request.form.get('passwd'))
    phone   = Sstr(request.form.get('phone'))

    sql = ("UPDATE "+LOGIN_TABLE_NAME[idType]+" "
            + "SET passwd="+passwd+", name="+name+", phone="+phone
            + "WHERE id"+LOGIN_TABLE_NAME[idType]+"="+str(LOGIN_ID[idType])
    )
    print (sql)
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)
    
    cur.execute(sql)
    conn.commit()

    conn.close()
    return redirect('/main')

##-----------------------------------------------------##
##                     Customer용                      ##
##-----------------------------------------------------##
@app.route('/searchStore', methods=['POST'])
def SearchStore():
    tag  = Sstr(request.form.get('tag' ))
    name = Sstr(request.form.get('name'))

    sql =("SELECT * FROM Store "                            ## 오작동하기는 하는데, 일단 테스트용으론 ㄱㅊ
        + "WHERE idStore in ( ("
                + "SELECT idStore "
                + "FROM Store "
                + "WHERE name LIKE '%' || "+name+" || '%'"
            + ") UNION ("
                + "SELECT idStore "
                + "FROM Tag "
                + "WHERE name = "+tag
            + ")"
        +");"
    )
    print(sql)
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)
    
    cur.execute(sql)
    conn.commit()
    rows = cur.fetchall()

    conn.close()

    return render_template("searchStore.html", store_list=rows)

@app.route('/visitStore', methods=['POST'])
def VisitStore():
    idStore  = Sstr(request.form.get('idStore' ))
    
    sql1 = "SELECT * FROM Store WHERE idStore ="+idStore
    sql2 = "SELECT * FROM Menu  WHERE idStore ="+idStore
    sql3 = "SELECT * FROM PayMethod"
    print(sql1)
    print(sql2)
    print(sql3)
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)
    
    cur.execute(sql1)
    store_data = cur.fetchall()
    cur.execute(sql2)
    menu_data = cur.fetchall()
    cur.execute(sql3)
    pay_data = cur.fetchall()
    
    conn.close()
    return render_template("visitStore.html",
            store           = store_data,
            menu_list       = menu_data, 
            menu_len        = len(menu_data),
            pay_method_list = pay_data
        )

@app.route('/makeOrder', methods=['POST'])
def MakeOrder():
    conn     = pg.connect(CONNECT_STR)
    cur      = conn.cursor(cursor_factory=pg.extras.DictCursor)

    idStore    = Sstr(request.form.get('idStore' ))
    menuLen    = int (request.form.get('menuLen' ))
    idPayMethod= int (request.form.get('idPayMethod'))

    priceTotal = 0
    order = []
    sql = ""
    for i in range(menuLen) :
        idMenu = Sstr(request.form.get('idMenu'+str(i) ))
        count  = int (request.form.get('count' +str(i) ))
        order.append([idMenu, count])
    
    for o in order :
        sql = "SELECT price FROM Menu WHERE idMenu=" +o[0]
        print(sql)
        cur.execute(sql)
        price = int(cur.fetchall()[0][0])
        priceTotal += price * o[1]

    sql = ("INSERT INTO Order_(idCustomer,   idStore,     idPayMethod,    price, longitude, latitude) "
        +  "VALUES ("        +str(LOGIN_ID[0])+","+idStore+","+str(idPayMethod)+","+str(priceTotal)+",0,0) "
        +  "RETURNING idOrder;"
    )
    print(sql)
    cur.execute(sql)

    conn.commit()
    idOrder = Sstr(str(cur.fetchall()[0][0]))
    for o in order :
        sql = ("INSERT INTO Choose(  idOrder,    idMenu,quantity)"
            +  "VALUES            ("+idOrder+","+o[0]+","+Sstr(str(o[1]))+")")
        print(sql)
        cur.execute(sql)

    conn.commit()
    conn.close()
    return redirect('/main')

@app.route('/finOrder', methods=['POST'])
def FinOrder():
    idOrder = Sstr(request.form.get('idOrder' ))
    page_from= request.form.get('from')
    sql     = "DELETE FROM Order_ WHERE idOrder="+idOrder
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()
    if page_from == 'main' :
        return redirect('/main')
    if page_from == 'manageStore':
        return ManageStorePage(request.form.get('idStore' ))


##-----------------------------------------------------##
##                      Seller용                       ##
##-----------------------------------------------------##

@app.route('/addStore', methods=['POST'])
def AddStore():
    global LOGIN_ID
    name    = Sstr(request.form.get('name' ))
    descript= Sstr(request.form.get('descript'))

    sql     = ("INSERT INTO Store"
        + " (idSeller, name, descript, longitude, latitude)"
        + "VALUES ("
        + str(LOGIN_ID[1])+","+name+","+descript+",0,0);"
    )
    print(sql)
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()
    
    return redirect('/main')

@app.route('/manageStore', methods=['POST'] )
def ManageStore():
    idStore     = Sstr(request.form.get('idStore' ))
    print("---------------manage " + idStore)
    return ManageStorePage(idStore)


def ManageStorePage(idStore):
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)

    sql = "SELECT * from Store WHERE idStore = "+idStore
    print(sql)
    cur.execute(sql)
    data_store = cur.fetchall()
    
    sql = "SELECT * from Menu WHERE idStore = "+idStore
    cur.execute(sql)
    data_menu = cur.fetchall()

    sql = "SELECT idDelivery,name from Delivery"
    cur.execute(sql)
    data_delivery = cur.fetchall()

    print(data_delivery)
    
    sql = "SELECT idOrder, idDelivery, idPaymethod, price FROM Order_ WHERE idStore=" + idStore
    print(sql)
    cur.execute(sql)
    order_list = cur.fetchall()
        
    for order in order_list:
        if order[1] != None :
            sql = "SELECT name FROM Delivery WHERE idDelivery=" + str(order[1])
            cur.execute(sql)
            order[1] = cur.fetchall()[0][0]

        sql = "SELECT name FROM PayMethod WHERE idPayMethod=" + str(order[2])
        cur.execute(sql)
        order[2] = cur.fetchall()[0][0]

            

        sql = "SELECT idMenu, quantity FROM Choose WHERE idOrder="+str(order[0])
        cur.execute(sql)
        choose_list = cur.fetchall()
        for choose in choose_list :
            sql = "SELECT name FROM Menu WHERE idMenu="+str(choose[0])
            cur.execute(sql)
            choose[0] = cur.fetchall()[0][0]
            
        order.append(choose_list)
        
        


    return render_template("manageStore.html", store=data_store, menu_list=data_menu, order_list=order_list, delivery_list = data_delivery)
    

@app.route('/addMenu', methods=['POST'])
def AddMenu():
    global LOGIN_ID
    idStore = Sstr(request.form.get('idStore'))
    name    = Sstr(request.form.get('name' ))
    price   = Sstr(request.form.get('price'))

    sql= ("INSERT INTO Menu (idStore,   name,     price) "
        + "VALUES ("       + idStore+","+name+","+price+");"
    )
    print(sql)
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()
    return ManageStorePage(idStore)

@app.route('/delMenu', methods=['POST'] )
def DelMenu():
    global LOGIN_ID

    idMenu = Sstr(request.form.get('idMenu'))
    idStore= Sstr(request.form.get('idStore'))
    
    sql     = "DELETE FROM Menu WHERE idMenu="+idMenu+");"
    
    print(sql)
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()
    return ManageStorePage(idStore)


@app.route('/assignDelivery', methods=['POST'] )
def AssginDelivery():
    idOrder    = Sstr(request.form.get('idOrder'))
    idStore    = Sstr(request.form.get('idStore'))
    idDelivery = Sstr(request.form.get('idDelivery'))  
    sql     = ("UPDATE Order_ "
            +   "SET idDelivery="+idDelivery+" "
            +   "WHERE idOrder="+idOrder
    )
    print(sql)

    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)
    cur.execute(sql)
    conn.commit()
    conn.close()
    return ManageStorePage(idStore)


@app.route('/reset', methods=['POST'])
def Reset():
    conn    = pg.connect(CONNECT_STR)
    cur     = conn.cursor(cursor_factory=pg.extras.DictCursor)

    with codecs.open('dropAll.sql','r',encoding='utf8') as f:
        sql = f.read()
    print (sql)    
    cur.execute(sql)
    conn.commit()
    
    with codecs.open('create.sql','r',encoding='utf8') as f:
        sql = f.read()
    print (sql)    
    cur.execute(sql)
    conn.commit()
    
    conn.close()
    return redirect('/')


if __name__ == '__main__':
    app.run(debug=True)
