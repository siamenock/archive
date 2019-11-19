CREATE TABLE Seller(
    idSeller    SERIAL PRIMARY KEY,
    email       varchar(30)    NOT NULL UNIQUE,
    passwd      varchar(15)    NOT NULL,
    name        varchar(30),
    phone       varchar(15)
);

CREATE TABLE Customer(
    idCustomer  SERIAL PRIMARY KEY,
    email       varchar(30)    NOT NULL UNIQUE,
    passwd      varchar(15)    NOT NULL,
    name        varchar(30),
    phone       varchar(15)
);

CREATE TABLE Delivery(
    idDelivery  SERIAL PRIMARY KEY,
    email       varchar(30)    NOT NULL UNIQUE,
    passwd      varchar(15)    NOT NULL,
    name        varchar(30),
    phone       varchar(15)
);

CREATE TABLE Store(
    idStore     SERIAL PRIMARY KEY,
    idSeller    int, FOREIGN KEY(idSeller) REFERENCES Seller ON DELETE CASCADE ON UPDATE CASCADE,
    name        varchar(10) not null,
    descript    varchar(60),
    longitude   FLOAT NOT NULL,
    latitude    FLOAT NOT NULL
);

CREATE TABLE Menu(
    idMenu     SERIAL PRIMARY KEY,
    idStore    int NOT NULL, FOREIGN KEY(idStore) REFERENCES Store ON DELETE CASCADE ON UPDATE CASCADE,
    name       varchar(30) NOT NULL,
    price      int NOT NULL
);

CREATE TABLE PayMethod(
    idpayMethod SERIAL PRIMARY KEY,
    name        varchar(15) NOT NULL
);

CREATE TABLE Order_(
    idOrder     SERIAL PRIMARY KEY,
    idCustomer  int NOT NULL, FOREIGN KEY(idCustomer ) REFERENCES Customer  ON DELETE CASCADE ON UPDATE CASCADE,
    idDelivery  int         , FOREIGN KEY(idDelivery ) REFERENCES Delivery  ON DELETE CASCADE ON UPDATE CASCADE,
    idStore     int NOT NULL, FOREIGN KEY(idStore    ) REFERENCES Store     ON DELETE CASCADE ON UPDATE CASCADE,
    idpayMethod int NOT NULL, FOREIGN KEY(idpayMethod) REFERENCES PayMethod ON DELETE CASCADE ON UPDATE CASCADE,
    
    orderTime   TIME,
    price       int NOT NULL,
    longitude   FLOAT NOT NULL,
    latitude    FLOAT NOT NULL
);

CREATE TABLE Choose(
    idChoose    SERIAL PRIMARY KEY,
    idOrder     int NOT NULL, FOREIGN KEY(idOrder) REFERENCES Order_ ON DELETE CASCADE ON UPDATE CASCADE,
    idMenu      int NOT NULL, FOREIGN KEY(idMenu)  REFERENCES Menu   ON DELETE CASCADE ON UPDATE CASCADE,
    quantity    int NOT NULL
);

CREATE TABLE AddrCustomer(
    idCustomer  int NOT NULL, FOREIGN KEY(idCustomer) REFERENCES Customer ON DELETE CASCADE ON UPDATE CASCADE,
    addr        varchar(30) NOT NULL
);

CREATE TABLE Tag(
    idTag       SERIAL PRIMARY KEY,
    name        varchar(10) NOT NULL
);

CREATE TABLE TagAssign(
    idTag       int NOT NULL, FOREIGN KEY(idTag      ) REFERENCES Tag   ON DELETE CASCADE ON UPDATE CASCADE,
    idStore     int NOT NULL, FOREIGN KEY(idStore    ) REFERENCES Store ON DELETE CASCADE ON UPDATE CASCADE
);


INSERT INTO PayMethod(name) VALUES ('카드결제');
INSERT INTO PayMethod(name) VALUES ('현금지불');


INSERT INTO Tag(name) VALUES ('버거');
INSERT INTO Tag(name) VALUES ('게살');
INSERT INTO Tag(name) VALUES ('치킨');
INSERT INTO Tag(name) VALUES ('양념치킨');
INSERT INTO Tag(name) VALUES ('족발');
INSERT INTO Tag(name) VALUES ('보쌈');
INSERT INTO Tag(name) VALUES ('피자');
INSERT INTO Tag(name) VALUES ('포테이토피자');


INSERT INTO Customer(email, passwd, name, phone) VALUES('a@a.com', 'a', '손님은 왕 김모씨', '010-0000-1111');
INSERT INTO Seller  (email, passwd, name, phone) VALUES('a@a.com', 'a', '주인은 왕 김모씨', '010-0000-0000');
INSERT INTO Delivery(email, passwd, name, phone) VALUES('a@a.com', 'a', '배달왕 김모씨'   , '010-0000-2222');
INSERT INTO Delivery(email, passwd, name, phone) VALUES('b@b.com', 'b', '배달부 이모씨'   , '010-0000-2222');

INSERT INTO Store (idSeller, name, descript, longitude, latitude) VALUES(1, '집게리아', '집게사장맛 게살버거', 0,0);

INSERT INTO Menu  (idStore, name, price) VALUES(1, '게살버거'    , 5000);
INSERT INTO Menu  (idStore, name, price) VALUES(1, '게살버거세트', 7000);















