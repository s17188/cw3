--INSERT INTO Studies VALUES (2,'studia');
--INSERT INTO Enrollment VALUES (3,3,2,'2009-01-01');
--INSERT INTO Student VALUES ('s17787','Damian','Testowy','2091-09-12',3);
SELECT IndexNumber,FirstName,LastName,e.IdEnrollment,e.Semester,e.IdStudy FROM Student AS s INNER JOIN Enrollment as e ON s.IdEnrollment = e.IdEnrollment WHERE s.IndexNumber = 's17787';