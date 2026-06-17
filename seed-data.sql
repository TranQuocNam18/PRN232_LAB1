-- =============================================
-- LMS SEED DATA SCRIPT
-- =============================================

-- Clear existing data (order matters for FK)
DELETE FROM Enrollments;
DELETE FROM Courses;
DELETE FROM Subjects;
DELETE FROM Students;
DELETE FROM Semesters;

-- Reset IDENTITY counters so IDs start from 1
DBCC CHECKIDENT ('Semesters', RESEED, 0);
DBCC CHECKIDENT ('Subjects', RESEED, 0);
DBCC CHECKIDENT ('Students', RESEED, 0);
DBCC CHECKIDENT ('Courses', RESEED, 0);
DBCC CHECKIDENT ('Enrollments', RESEED, 0);

-- SEED SEMESTERS (5 semesters)
INSERT INTO Semesters (SemesterName, StartDate, EndDate) VALUES
('Fall 2024',   '2024-09-01', '2024-12-31'),
('Spring 2025', '2025-01-15', '2025-05-31'),
('Summer 2025', '2025-06-01', '2025-08-31'),
('Fall 2025',   '2025-09-01', '2025-12-31'),
('Spring 2026', '2026-01-15', '2026-05-31');

-- SEED SUBJECTS (10 subjects)
INSERT INTO Subjects (SubjectCode, SubjectName, Credit) VALUES
('CS101',   'Introduction to Programming', 3),
('CS201',   'Data Structures',             4),
('CS202',   'Web Development',             3),
('CS301',   'Database Design',             4),
('CS302',   'Software Engineering',        3),
('MATH101', 'Calculus I',                  4),
('MATH201', 'Linear Algebra',              3),
('ENG101',  'English Communication',       3),
('BUS101',  'Business Management',         3),
('PHY101',  'Physics I',                   4);

-- SEED STUDENTS (50 students)
INSERT INTO Students (FullName, Email, DateOfBirth) VALUES
('Nguyen Van An',       'nguyenan@email.com',      '2004-01-15'),
('Tran Thi Bao',        'tranbao@email.com',        '2003-05-20'),
('Pham Minh Chau',      'phamminhchau@email.com',   '2004-03-10'),
('Le Hoang Duc',        'lehoangduc@email.com',     '2003-07-25'),
('Vu Thi Huong',        'vuthuong@email.com',       '2004-02-14'),
('Dang Van Kien',       'dangvankien@email.com',    '2003-11-08'),
('Hoang Thi Linh',      'hoangthilinh@email.com',   '2004-04-22'),
('Ta Quoc Minh',        'taquocminh@email.com',     '2003-09-30'),
('Bui Thi Nga',         'buithinga@email.com',      '2004-06-18'),
('Cao Van Phong',       'caovanphong@email.com',    '2003-12-05'),
('Duong Thi Quynh',     'duongthiquynh@email.com',  '2004-08-13'),
('Dinh Van Son',        'dinhvanson@email.com',     '2003-10-27'),
('Giang Thi Suong',     'giangthisuong@email.com',  '2004-01-31'),
('Hy Van Tam',          'hyvantam@email.com',       '2003-03-19'),
('Khanh Thi Uyen',      'khanhthiuyen@email.com',   '2004-05-16'),
('Luong Van Vu',        'luongvanvu@email.com',     '2003-07-08'),
('Manh Thi Xuong',      'manhthixuong@email.com',   '2004-09-23'),
('Nhu Van Yen',         'nhuvanyen@email.com',      '2003-11-12'),
('On Thi Zoa',          'onthizoa@email.com',       '2004-02-28'),
('Phuc Van A',          'phucvana@email.com',       '2003-04-17'),
('Quan Thi Binh',       'quanthibinh@email.com',    '2004-06-09'),
('Rap Van Chi',         'rapvanchi@email.com',      '2003-08-30'),
('Sau Thi Dat',         'sauthidat@email.com',      '2004-03-21'),
('Tan Van Em',          'tanvanem@email.com',       '2003-10-14'),
('Ung Thi Phat',        'ungthiphat@email.com',     '2004-12-02'),
('Viet Van Gai',        'vietvangai@email.com',     '2003-01-25'),
('Xuyen Thi Hao',       'xuyenthihao@email.com',    '2004-07-11'),
('Yen Van Hai',         'yenvanhai@email.com',      '2003-09-06'),
('Zing Thi Ich',        'zingthiich@email.com',     '2004-04-19'),
('An Van Ky',           'anvanky@email.com',        '2003-02-28'),
('Ba Thi Loi',          'bathiloi@email.com',       '2004-08-07'),
('Co Van Mau',          'covanmau@email.com',       '2003-12-13'),
('Dam Thi Noi',         'damthinoi@email.com',      '2004-05-24'),
('Eu Van On',           'euvanon@email.com',        '2003-06-17'),
('Phe Thi Phich',       'phethiphich@email.com',    '2004-10-01'),
('Que Van Quang',       'quevanguang@email.com',    '2003-03-08'),
('Re Thi Rieng',        'rethirieng@email.com',     '2004-09-15'),
('So Van Sanh',         'sovansanh@email.com',      '2003-11-22'),
('Tac Thi Tan',         'tacthitan@email.com',      '2004-01-19'),
('Uc Van Ung',          'ucvanung@email.com',       '2003-07-26'),
('Vo Thi Van',          'vothivan@email.com',       '2004-04-04'),
('Xa Van Xuong',        'xavanxuong@email.com',     '2003-09-12'),
('Yeu Thi Yem',         'yeuthiyem@email.com',      '2004-02-06'),
('Zai Van Zau',         'zaivanzau@email.com',      '2003-08-29'),
('A Thi An',            'athian@email.com',         '2004-06-23'),
('Bang Van Bau',        'bangvanbau@email.com',     '2003-04-10'),
('Cat Thi Cuu',         'cathicuu@email.com',       '2004-10-18'),
('Dac Van Dang',        'dacvandang@email.com',     '2003-12-30'),
('E Thi En',            'ethien@email.com',         '2004-03-27'),
('Pha Van Pheo',        'phavanpheo@email.com',     '2003-06-05');

-- SEED COURSES (20 courses)
INSERT INTO Courses (CourseName, SemesterId, SubjectId) VALUES
('Programming Basics A',    1, 1),
('Programming Basics B',    1, 1),
('Calculus I A',            1, 6),
('Calculus I B',            1, 6),
('English Communication A', 1, 8),
('Data Structures A',       2, 2),
('Web Development A',       2, 3),
('Database Design A',       2, 4),
('Linear Algebra A',        2, 7),
('Business Management A',   2, 9),
('Software Engineering A',  3, 5),
('Physics I A',             3, 10),
('Programming Advanced',    3, 1),
('Web Development B',       3, 3),
('Data Structures B',       4, 2),
('Database Design B',       4, 4),
('Software Engineering B',  4, 5),
('Physics I B',             4, 10),
('Advanced Programming',    5, 1),
('Machine Learning Basics', 5, 2);

-- SEED ENROLLMENTS (500 enrollments)
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
    SET @CourseId  = ((@i - 1) % 20) + 1;
    SET @EnrollDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 300), GETDATE());
    SET @Status = (SELECT TOP 1 Status FROM @Statuses ORDER BY NEWID());

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
        SET @CourseId  = ((@CourseId  % 20) + 1);
        SET @StudentId = ((@StudentId % 50) + 1);
    END
END

-- Verify
SELECT 'Semesters'  AS TableName, COUNT(*) AS RecordCount FROM Semesters
UNION ALL SELECT 'Subjects',   COUNT(*) FROM Subjects
UNION ALL SELECT 'Students',   COUNT(*) FROM Students
UNION ALL SELECT 'Courses',    COUNT(*) FROM Courses
UNION ALL SELECT 'Enrollments',COUNT(*) FROM Enrollments;
