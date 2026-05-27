-- Clear existing data
DELETE FROM Enrollments;
DELETE FROM Courses;
DELETE FROM Subjects;
DELETE FROM Students;
DELETE FROM Semesters;

-- SEED SEMESTERS (5 semesters)
INSERT INTO Semesters (SemesterName, StartDate, EndDate) VALUES
('Fall 2024', '2024-09-01', '2024-12-31'),
('Spring 2025', '2025-01-15', '2025-05-31'),
('Summer 2025', '2025-06-01', '2025-08-31'),
('Fall 2025', '2025-09-01', '2025-12-31'),
('Spring 2026', '2026-01-15', '2026-05-31');

-- SEED SUBJECTS (10 subjects)
INSERT INTO Subjects (SubjectCode, SubjectName, Credit) VALUES
('CS101', 'Introduction to Programming', 3),
('CS201', 'Data Structures', 4),
('CS202', 'Web Development', 3),
('CS301', 'Database Design', 4),
('CS302', 'Software Engineering', 3),
('MATH101', 'Calculus I', 4),
('MATH201', 'Linear Algebra', 3),
('ENG101', 'English Communication', 3),
('BUS101', 'Business Management', 3),
('PHY101', 'Physics I', 4);

-- SEED STUDENTS (50 students)
INSERT INTO Students (FullName, Email, DateOfBirth) VALUES
('Nguy?n V?n An', 'nguyenan@email.com', '2004-01-15'),
('Tr?n Th? B?o', 'tranbao@email.com', '2003-05-20'),
('Ph?m Minh Châu', 'phamminhchau@email.com', '2004-03-10'),
('Lê Hoàng ??c', 'lehoangduc@email.com', '2003-07-25'),
('V? Th? H??ng', 'vuthuong@email.com', '2004-02-14'),
('??ng V?n Kiên', 'dangvankien@email.com', '2003-11-08'),
('Hoàng Th? Linh', 'hoangthilinh@email.com', '2004-04-22'),
('T? Qu?c Minh', 'taquocminh@email.com', '2003-09-30'),
('Bùi Th? Nga', 'buithinga@email.com', '2004-06-18'),
('Cao V?n Phong', 'caovanphong@email.com', '2003-12-05'),
('D??ng Th? Qu?nh', 'duongthiqunh@email.com', '2004-08-13'),
('?inh V?n Rôm', 'dinhvanrom@email.com', '2003-10-27'),
('Giang Th? S??ng', 'giangthisuong@email.com', '2004-01-31'),
('Hy V?n Tâm', 'hyvantam@email.com', '2003-03-19'),
('Khánh Th? Uyên', 'khanhthiuyen@email.com', '2004-05-16'),
('L??ng V?n V?', 'luongvanvu@email.com', '2003-07-08'),
('M?nh Th? X??ng', 'manthixuong@email.com', '2004-09-23'),
('Nh? V?n Yên', 'nhuvanyen@email.com', '2003-11-12'),
('?n Th? Zoa', 'onthizoa@email.com', '2004-02-28'),
('Phúc V?n Á', 'phucvana@email.com', '2003-04-17'),
('Quân Th? Bình', 'quanthibinh@email.com', '2004-06-09'),
('Ráp V?n Chi', 'rapvanchí@email.com', '2003-08-30'),
('Sâu Th? ??t', 'sauthidat@email.com', '2004-03-21'),
('T?n V?n Em', 'tanvanem@email.com', '2003-10-14'),
('?ng Th? Phát', 'ungthiphat@email.com', '2004-12-02'),
('Vi?t V?n Gái', 'vietvangai@email.com', '2003-01-25'),
('Xuy?n Th? Hào', 'xuyenthihao@email.com', '2004-07-11'),
('Yên V?n H?i', 'yenvanhai@email.com', '2003-09-06'),
('Zing Th? Ích', 'zingthiich@email.com', '2004-04-19'),
('Ân V?n K?', 'anvanky@email.com', '2003-02-28'),
('Bá Th? L?i', 'bathiloi@email.com', '2004-08-07'),
('C? V?n M?u', 'covanmau@email.com', '2003-12-13'),
('??m Th? Nói', 'damth??oi@email.com', '2004-05-24'),
('?u V?n ?n', 'euvanon@email.com', '2003-06-17'),
('Phê Th? Phích', 'phethiphich@email.com', '2004-10-01'),
('Qu? V?n Qu?ng', 'quevanguang@email.com', '2003-03-08'),
('R? Th? Riêng', 'rethiriteng@email.com', '2004-09-15'),
('S? V?n Sanh', 'sosansanh@email.com', '2003-11-22'),
('T?c Th? T?n', 'tacttan@email.com', '2004-01-19'),
('?c V?n ?ng', 'ucvanuing@email.com', '2003-07-26'),
('Võ Th? Vân', 'vothivan@email.com', '2004-04-04'),
('X? V?n X??ng', 'xavanxuong@email.com', '2003-09-12'),
('Yêu Th? Yêm', 'yeuthiyem@email.com', '2004-02-06'),
('Zai V?n Zâu', 'zaivanzau@email.com', '2003-08-29'),
('? Th? ?n', 'athian@email.com', '2004-06-23'),
('B?ng V?n B?u', 'bangvanbau@email.com', '2003-04-10'),
('C?t Th? C?u', 'cathicuu@email.com', '2004-10-18'),
('??c V?n ??ng', 'dacvandang@email.com', '2003-12-30'),
('? Th? ?n', 'ethien@email.com', '2004-03-27');

-- SEED COURSES (20 courses)
INSERT INTO Courses (CourseName, SemesterId, SubjectId) VALUES
-- Semester 1 (Fall 2024)
('Programming Basics A', 1, 1),
('Programming Basics B', 1, 1),
('Calculus I A', 1, 6),
('Calculus I B', 1, 6),
('English Communication A', 1, 8),
-- Semester 2 (Spring 2025)
('Data Structures A', 2, 2),
('Web Development A', 2, 3),
('Database Design A', 2, 4),
('Linear Algebra A', 2, 7),
('Business Management A', 2, 9),
-- Semester 3 (Summer 2025)
('Software Engineering A', 3, 5),
('Physics I A', 3, 10),
('Programming Advanced', 3, 1),
('Web Development B', 3, 3),
-- Semester 4 (Fall 2025)
('Data Structures B', 4, 2),
('Database Design B', 4, 4),
('Software Engineering B', 4, 5),
('Physics I B', 4, 10),
-- Semester 5 (Spring 2026)
('Advanced Programming', 5, 1),
('Machine Learning Basics', 5, 2);

-- SEED ENROLLMENTS (500 enrollments)
-- Script to generate 500 enrollments randomly
DECLARE @i INT = 1;
DECLARE @StudentId INT;
DECLARE @CourseId INT;
DECLARE @EnrollDate DATETIME;
DECLARE @Status VARCHAR(20);
DECLARE @Statuses TABLE (Status VARCHAR(20));

INSERT INTO @Statuses VALUES ('Active'), ('Completed'), ('Dropped'), ('Pending');

WHILE @i <= 500
BEGIN
    SET @StudentId = ((@i - 1) % 50) + 1;
    SET @CourseId = ((@i - 1) % 20) + 1;
    SET @EnrollDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 300), GETDATE());
    SET @Status = (SELECT TOP 1 Status FROM @Statuses ORDER BY NEWID());
    
    -- Avoid duplicate enrollments for same student-course combination
    IF NOT EXISTS (
        SELECT 1 FROM Enrollments 
        WHERE StudentId = @StudentId AND CourseId = @CourseId
    )
    BEGIN
        INSERT INTO Enrollments (StudentId, CourseId, EnrollDate, Status)
        VALUES (@StudentId, @CourseId, @EnrollDate, @Status);
        SET @i = @i + 1;
    END
    ELSE
    BEGIN
        -- If duplicate, try next combination
        SET @CourseId = ((@CourseId % 20) + 1);
        SET @StudentId = ((@StudentId % 50) + 1);
    END
END

-- Verify data inserted
SELECT 'Semesters' AS TableName, COUNT(*) AS RecordCount FROM Semesters
UNION ALL
SELECT 'Subjects', COUNT(*) FROM Subjects
UNION ALL
SELECT 'Students', COUNT(*) FROM Students
UNION ALL
SELECT 'Courses', COUNT(*) FROM Courses
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM Enrollments;