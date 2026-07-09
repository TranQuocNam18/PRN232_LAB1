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
-- (Not strictly needed if we insert explicit IDs, but kept for consistency)
DBCC CHECKIDENT ('Semesters', RESEED, 0);
DBCC CHECKIDENT ('Subjects', RESEED, 0);
DBCC CHECKIDENT ('Students', RESEED, 0);
DBCC CHECKIDENT ('Courses', RESEED, 0);
DBCC CHECKIDENT ('Enrollments', RESEED, 0);

-- SEED SEMESTERS (5 semesters)
SET IDENTITY_INSERT Semesters ON;
INSERT INTO Semesters (SemesterId, SemesterName, StartDate, EndDate) VALUES
(1, 'Fall 2024',   '2024-09-01', '2024-12-31'),
(2, 'Spring 2025', '2025-01-15', '2025-05-31'),
(3, 'Summer 2025', '2025-06-01', '2025-08-31'),
(4, 'Fall 2025',   '2025-09-01', '2025-12-31'),
(5, 'Spring 2026', '2026-01-15', '2026-05-31');
SET IDENTITY_INSERT Semesters OFF;

-- SEED SUBJECTS (10 subjects)
SET IDENTITY_INSERT Subjects ON;
INSERT INTO Subjects (SubjectId, SubjectCode, SubjectName, Credit) VALUES
(1, 'CS101',   'Introduction to Programming', 3),
(2, 'CS201',   'Data Structures',             4),
(3, 'CS202',   'Web Development',             3),
(4, 'CS301',   'Database Design',             4),
(5, 'CS302',   'Software Engineering',        3),
(6, 'MATH101', 'Calculus I',                  4),
(7, 'MATH201', 'Linear Algebra',              3),
(8, 'ENG101',  'English Communication',       3),
(9, 'BUS101',  'Business Management',         3),
(10, 'PHY101',  'Physics I',                   4);
SET IDENTITY_INSERT Subjects OFF;

-- SEED STUDENTS (50 students)
SET IDENTITY_INSERT Students ON;
INSERT INTO Students (StudentId, FullName, Email, DateOfBirth) VALUES
(1, 'Nguyen Van An',       'nguyenan@email.com',      '2004-01-15'),
(2, 'Tran Thi Bao',        'tranbao@email.com',        '2003-05-20'),
(3, 'Pham Minh Chau',      'phamminhchau@email.com',   '2004-03-10'),
(4, 'Le Hoang Duc',        'lehoangduc@email.com',     '2003-07-25'),
(5, 'Vu Thi Huong',        'vuthuong@email.com',       '2004-02-14'),
(6, 'Dang Van Kien',       'dangvankien@email.com',    '2003-11-08'),
(7, 'Hoang Thi Linh',      'hoangthilinh@email.com',   '2004-04-22'),
(8, 'Ta Quoc Minh',        'taquocminh@email.com',     '2003-09-30'),
(9, 'Bui Thi Nga',         'buithinga@email.com',      '2004-06-18'),
(10, 'Cao Van Phong',       'caovanphong@email.com',    '2003-12-05'),
(11, 'Duong Thi Quynh',     'duongthiquynh@email.com',  '2004-08-13'),
(12, 'Dinh Van Son',        'dinhvanson@email.com',     '2003-10-27'),
(13, 'Giang Thi Suong',     'giangthisuong@email.com',  '2004-01-31'),
(14, 'Hy Van Tam',          'hyvantam@email.com',       '2003-03-19'),
(15, 'Khanh Thi Uyen',      'khanhthiuyen@email.com',   '2004-05-16'),
(16, 'Luong Van Vu',        'luongvanvu@email.com',     '2003-07-08'),
(17, 'Manh Thi Xuong',      'manhthixuong@email.com',   '2004-09-23'),
(18, 'Nhu Van Yen',         'nhuvanyen@email.com',      '2003-11-12'),
(19, 'On Thi Zoa',          'onthizoa@email.com',       '2004-02-28'),
(20, 'Phuc Van A',          'phucvana@email.com',       '2003-04-17'),
(21, 'Quan Thi Binh',       'quanthibinh@email.com',    '2004-06-09'),
(22, 'Rap Van Chi',         'rapvanchi@email.com',      '2003-08-30'),
(23, 'Sau Thi Dat',         'sauthidat@email.com',      '2004-03-21'),
(24, 'Tan Van Em',          'tanvanem@email.com',       '2003-10-14'),
(25, 'Ung Thi Phat',        'ungthiphat@email.com',     '2004-12-02'),
(26, 'Viet Van Gai',        'vietvangai@email.com',     '2003-01-25'),
(27, 'Xuyen Thi Hao',       'xuyenthihao@email.com',    '2004-07-11'),
(28, 'Yen Van Hai',         'yenvanhai@email.com',      '2003-09-06'),
(29, 'Zing Thi Ich',        'zingthiich@email.com',     '2004-04-19'),
(30, 'An Van Ky',           'anvanky@email.com',        '2003-02-28'),
(31, 'Ba Thi Loi',          'bathiloi@email.com',       '2004-08-07'),
(32, 'Co Van Mau',          'covanmau@email.com',       '2003-12-13'),
(33, 'Dam Thi Noi',         'damthinoi@email.com',      '2004-05-24'),
(34, 'Eu Van On',           'euvanon@email.com',        '2003-06-17'),
(35, 'Phe Thi Phich',       'phethiphich@email.com',    '2004-10-01'),
(36, 'Que Van Quang',       'quevanguang@email.com',    '2003-03-08'),
(37, 'Re Thi Rieng',        'rethirieng@email.com',     '2004-09-15'),
(38, 'So Van Sanh',         'sovansanh@email.com',      '2003-11-22'),
(39, 'Tac Thi Tan',         'tacthitan@email.com',      '2004-01-19'),
(40, 'Uc Van Ung',          'ucvanung@email.com',       '2003-07-26'),
(41, 'Vo Thi Van',          'vothivan@email.com',       '2004-04-04'),
(42, 'Xa Van Xuong',        'xavanxuong@email.com',     '2003-09-12'),
(43, 'Yeu Thi Yem',         'yeuthiyem@email.com',      '2004-02-06'),
(44, 'Zai Van Zau',         'zaivanzau@email.com',      '2003-08-29'),
(45, 'A Thi An',            'athian@email.com',         '2004-06-23'),
(46, 'Bang Van Bau',        'bangvanbau@email.com',     '2003-04-10'),
(47, 'Cat Thi Cuu',         'cathicuu@email.com',       '2004-10-18'),
(48, 'Dac Van Dang',        'dacvandang@email.com',     '2003-12-30'),
(49, 'E Thi En',            'ethien@email.com',         '2004-03-27'),
(50, 'Pha Van Pheo',        'phavanpheo@email.com',     '2003-06-05');
SET IDENTITY_INSERT Students OFF;

-- SEED COURSES (20 courses)
SET IDENTITY_INSERT Courses ON;
INSERT INTO Courses (CourseId, CourseName, SemesterId, SubjectId) VALUES
(1, 'Programming Basics A',    1, 1),
(2, 'Programming Basics B',    1, 1),
(3, 'Calculus I A',            1, 6),
(4, 'Calculus I B',            1, 6),
(5, 'English Communication A', 1, 8),
(6, 'Data Structures A',       2, 2),
(7, 'Web Development A',       2, 3),
(8, 'Database Design A',       2, 4),
(9, 'Linear Algebra A',        2, 7),
(10, 'Business Management A',   2, 9),
(11, 'Software Engineering A',  3, 5),
(12, 'Physics I A',             3, 10),
(13, 'Programming Advanced',    3, 1),
(14, 'Web Development B',       3, 3),
(15, 'Data Structures B',       4, 2),
(16, 'Database Design B',       4, 4),
(17, 'Software Engineering B',  4, 5),
(18, 'Physics I B',             4, 10),
(19, 'Advanced Programming',    5, 1),
(20, 'Machine Learning Basics', 5, 2);
SET IDENTITY_INSERT Courses OFF;

-- SEED ENROLLMENTS (500 enrollments)
INSERT INTO Enrollments (StudentId, CourseId, EnrollDate, Status)
SELECT TOP 500
    s.StudentId,
    c.CourseId,
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 300), GETDATE()) AS EnrollDate,
    CASE ABS(CHECKSUM(NEWID()) % 4)
        WHEN 0 THEN 'Active'
        WHEN 1 THEN 'Completed'
        WHEN 2 THEN 'Dropped'
        ELSE 'Pending'
    END AS Status
FROM Students s
CROSS JOIN Courses c
ORDER BY NEWID();


-- Verify
SELECT 'Semesters'  AS TableName, COUNT(*) AS RecordCount FROM Semesters
UNION ALL SELECT 'Subjects',   COUNT(*) FROM Subjects
UNION ALL SELECT 'Students',   COUNT(*) FROM Students
UNION ALL SELECT 'Courses',    COUNT(*) FROM Courses
UNION ALL SELECT 'Enrollments',COUNT(*) FROM Enrollments;
