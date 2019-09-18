SELECT B.building_id, B.name, B.admin FROM Building B WHERE B.rooms > 20;

INSERT INTO Student VALUES(2013011082, 'password', '이상옥', 'male', 3, 2001032070, 3);


update building set name = '정보통신관' where name = 'IT / BT';


SELECT Building.building_id, Building.name
FROM Building
    INNER JOIN Room
    ON Building.building_id = Room.building_id AND Room.capacity > 100;


SELECT  name
FROM    major, (
    SELECT major_id, count(course_id) AS count
    FROM class
    GROUP BY major_id
    ORDER BY count(course_id) DESC
    limit 10
    ) AS M
WHERE   major.major_id = M.major_id
ORDER BY count DESC
