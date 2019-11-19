CREATE TABLE Building(
    building_id  INT      PRIMARY KEY,
    name         CHAR(20) ,
    admin        CHAR(20) ,
    rooms	     INT      
);

CREATE TABLE Room(
    room_id      INT  PRIMARY KEY,
    building_id  INT  REFERENCES Building NOT NULL,
    capacity	 INT  
);

CREATE TABLE Course(
    course_id CHAR( 7) PRIMARY KEY,
    name      CHAR(20),
    credit    INT
);

CREATE TABLE Major(
    major_id INT PRIMARY KEY,
    name     CHAR(20)
);

CREATE TABLE Instructor(
    instructor_id INT PRIMARY KEY,
    name          CHAR(30) ,
    major_id      INT REFERENCES Major
);


CREATE TABLE Student(
    sid      INT      PRIMARY KEY,
    password CHAR(20) NOT NULL,
    sname     CHAR(20),
    sex      CHAR(06),
    major_id INT      REFERENCES Major,
    tutor_id INT      REFERENCES Instructor,
    grade    INT
);

CREATE TABLE Class(
    class_id      INT      PRIMARY KEY,
    class_no      INT  ,
    course_id     CHAR(07) REFERENCES Course,
    name          CHAR(20) ,
    major_id      INT      REFERENCES Major,
    grade         INT,
    credit        INT, 
    instructor_id INT      REFERENCES Instructor,
    capacity      INT,
    opened        INT,
    room_id       INT      REFERENCES Room
);

CREATE TABLE Credits(
    credit_id   INT PRIMARY KEY,
    student_id  INT REFERENCES Student(sid),
    class_id    INT REFERENCES Class,
    grade       char(2)

);
