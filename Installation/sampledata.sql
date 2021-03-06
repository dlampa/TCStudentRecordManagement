USE [dbtest]
GO
SET IDENTITY_INSERT [dbo].[attendance_states] ON 

INSERT [dbo].[attendance_states] ([StateID], [Description]) VALUES (1, N'Present')
INSERT [dbo].[attendance_states] ([StateID], [Description]) VALUES (2, N'Absent with excuse')
INSERT [dbo].[attendance_states] ([StateID], [Description]) VALUES (3, N'Absent without excuse')
SET IDENTITY_INSERT [dbo].[attendance_states] OFF
GO
SET IDENTITY_INSERT [dbo].[users] ON 

INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (1, N'Super', N'Admin', N'super@altx.dev', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (2, N'John', N'Smith', N'john@abc.com', 0, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (3, N'Anne', N'Smitherson', N'anne.smitherson@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (9, N'Simonne', N'Harbaugh', N'simone@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (10, N'Harmony', N'Newbill', N'harmony.newbill@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (12, N'Jarrett', N'Morones', N'jarett@abc.com', 0, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (13, N'Wendie', N'Earls', N'earls2@abc.com', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (14, N'Staff', N'Member', N'lampa@altx.dev', 1, N'eyJhbGciOiJSUzI1NiIsImtpZCI6IjE3OGFiMWRjNTkxM2Q5MjlkMzdjMjNkY2FhOTYxODcyZjhkNzBiNjgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwiYXpwIjoiNTUzMjI4NzIxMTE5LWRwMXA5bTI0ZDJicjJpdDEydW41N3BlcDNnb210Z3AxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiNTUzMjI4NzIxMTE5LWRwMXA5bTI0ZDJicjJpdDEydW41N3BlcDNnb210Z3AxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTA2NzQ5Mzk4NTU3MjMyNjc0MjY1IiwiaGQiOiJhbHR4LmRldiIsImVtYWlsIjoibGFtcGFAYWx0eC5kZXYiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6Im04SE0yV3NVZW1pekx2eGFpWExCc2ciLCJuYW1lIjoiRGFtaXIgTGFtcGEiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FPaDE0R2k0OEZUY2lxMXBxOFNmMXhrR01tQS1jekEzNjdEMHI2MDByaVR3PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IkRhbWlyIiwiZmFtaWx5X25hbWUiOiJMYW1wYSIsImxvY2FsZSI6ImVuLUdCIiwiaWF0IjoxNjAzMjY4NTY2LCJleHAiOjE2MDMyNzIxNjYsImp0aSI6IjZiMzBlN2QyZDJiZjAxNjE5ZjM3OGVhMDQwZDQ1MjY2MjVlNmFhMjYifQ.qftHzVXx8DkDsMXRChmlzmqIb09G0N2W19Olj3eb1odsHRrG6E5oP_6W0dJCAlldJyTvzhv1tjMh1W_w09NVF-o-3ZEdxYUUXz6hiI0B_VlxA0Jm1nT9zbpeJ2Q59rPEcq7Hfk3tPpO7cfplwEYeD5xVpPdKm59BDCNPVF8LJpnHHST2w0D8CopzElkmblG_Sa8XXWZsCfDw00Kd7WfooA8lkn5XfLpGWK7ghPwXZV68aZCcDmZtYjJQNXhUkuMhcbG7k-JP24tD2PqFSc8r1uQmSYA0eZl4vUp08FsoMkCwpIHMCzPzAv1AgKEiYC7-rkTG49s_ZckV5z9zaZo3iA')
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (15, N'Ordinary', N'User', N'ordinary@ualberta.ca', 1, N'eyJhbGciOiJSUzI1NiIsImtpZCI6IjE3OGFiMWRjNTkxM2Q5MjlkMzdjMjNkY2FhOTYxODcyZjhkNzBiNjgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwiYXpwIjoiNTUzMjI4NzIxMTE5LWRwMXA5bTI0ZDJicjJpdDEydW41N3BlcDNnb210Z3AxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiNTUzMjI4NzIxMTE5LWRwMXA5bTI0ZDJicjJpdDEydW41N3BlcDNnb210Z3AxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTA2NzQ5Mzk4NTU3MjMyNjc0MjY1IiwiaGQiOiJhbHR4LmRldiIsImVtYWlsIjoibGFtcGFAYWx0eC5kZXYiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6IkJ4Z0xoNVBwcVM1NmFWMVBvZjR6WkEiLCJuYW1lIjoiRGFtaXIgTGFtcGEiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FPaDE0R2k0OEZUY2lxMXBxOFNmMXhrR01tQS1jekEzNjdEMHI2MDByaVR3PXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IkRhbWlyIiwiZmFtaWx5X25hbWUiOiJMYW1wYSIsImxvY2FsZSI6ImVuLUdCIiwiaWF0IjoxNjAzMjY3MTI0LCJleHAiOjE2MDMyNzA3MjQsImp0aSI6IjA4YzM1Yzc0MWQ2YjEyODNlNzAxNmFiZWU2YjQwODI0Zjc5MDMzNWEifQ.o2dFwHNqOca8gVBOr6QAsBxryR7iib3KpMXDy78tc-60cqExpWQBanS8hzLEd6cAKaV_jrriia2CNJeYsTrURQMJ89j7QCjyL9RlaCQa8S5WmI-U0zAnHONAlYhiB8S4dXTv3FsZ2pbRfFjCgS5JP0bWFjQGapstsjCp-3c36LG80-0OjIZCqr_0jqtPVqdKVB794ez83b-FsHJj1b-gyHnMerZI8bVtdFKiG4PcgXlORnv-KChFaBwHdJVou9FtMnTYUtgecrmXGFAzfswKXQSshIPil0sU6OnEDcL783kHU1g992K2tpB19bntlUHqCL2WGGVHILToy6nKOb7lTg')
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (17, N'John', N'Doe', N'jdoe@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (18, N'Shirley', N'Patt', N'patt@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (19, N'Carlton', N'Carlson', N'Carl@ualberta.ca', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (20, N'Josefine', N'Cox', N'jc@abc.com', 1, NULL)
INSERT [dbo].[users] ([UserID], [Firstname], [Lastname], [Email], [Active], [ActiveToken]) VALUES (21, N'Vanesa', N'Random', N'random@ualberta.ca', 1, NULL)
SET IDENTITY_INSERT [dbo].[users] OFF
GO
SET IDENTITY_INSERT [dbo].[staff] ON 

INSERT [dbo].[staff] ([StaffID], [UserID], [SuperUser]) VALUES (2, 1, 1)
INSERT [dbo].[staff] ([StaffID], [UserID], [SuperUser]) VALUES (3, 14, 0)
SET IDENTITY_INSERT [dbo].[staff] OFF
GO
SET IDENTITY_INSERT [dbo].[cohorts] ON 

INSERT [dbo].[cohorts] ([CohortID], [Name], [StartDate], [EndDate]) VALUES (25, N'5.2 Autumn - Winter 2020', CAST(N'2020-08-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-11-01T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[cohorts] ([CohortID], [Name], [StartDate], [EndDate]) VALUES (26, N'5.1 Summer - Winter 2020', CAST(N'2020-06-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-12-01T00:00:00.0000000' AS DateTime2))
INSERT [dbo].[cohorts] ([CohortID], [Name], [StartDate], [EndDate]) VALUES (27, N'5.0 Winter - Summer 2020', CAST(N'2020-02-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-08-01T00:00:00.0000000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[cohorts] OFF
GO
SET IDENTITY_INSERT [dbo].[students] ON 

INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (4, 2, 27, N'1234')
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (5, 3, 27, N'5664')
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (7, 9, 25, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (8, 10, 25, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (12, 12, 26, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (13, 13, 26, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (14, 15, 26, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (15, 17, 25, N'123')
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (17, 18, 25, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (18, 19, 25, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (19, 20, 25, NULL)
INSERT [dbo].[students] ([StudentID], [UserID], [CohortID], [BearTracksID]) VALUES (20, 21, 25, NULL)
SET IDENTITY_INSERT [dbo].[students] OFF
GO
SET IDENTITY_INSERT [dbo].[attendance] ON 

INSERT [dbo].[attendance] ([RecordID], [StudentID], [AttendanceStateID], [StaffID], [Date], [Comment]) VALUES (1, 13, 1, 2, CAST(N'2020-10-14T00:00:00.0000000' AS DateTime2), N'New Comment!')
INSERT [dbo].[attendance] ([RecordID], [StudentID], [AttendanceStateID], [StaffID], [Date], [Comment]) VALUES (2, 12, 1, 2, CAST(N'2020-10-10T00:00:00.0000000' AS DateTime2), N'This is a comment')
INSERT [dbo].[attendance] ([RecordID], [StudentID], [AttendanceStateID], [StaffID], [Date], [Comment]) VALUES (3, 13, 1, 2, CAST(N'2020-10-05T00:00:00.0000000' AS DateTime2), N'New Comment!')
SET IDENTITY_INSERT [dbo].[attendance] OFF
GO
SET IDENTITY_INSERT [dbo].[task_types] ON 

INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (2, N'Practice')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (3, N'Weekend practice')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (4, N'Assignment')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (5, N'Milestone')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (7, N'Capstone')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (8, N'Classroom lectures')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (9, N'Online self-study')
INSERT [dbo].[task_types] ([TypeID], [Description]) VALUES (10, N'Break')
SET IDENTITY_INSERT [dbo].[task_types] OFF
GO
SET IDENTITY_INSERT [dbo].[units] ON 

INSERT [dbo].[units] ([UnitID], [Description]) VALUES (1, N'C# introduction')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (2, N'Javascript introduction')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (3, N'React')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (4, N'Redux')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (5, N'HTML5 and CSS')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (6, N'PHP')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (7, N'PHP API')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (8, N'Javascript API/AJAX')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (9, N'C# OOP')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (10, N'SQL')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (11, N'C# LINQ')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (12, N'ASP.NET Core')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (13, N'PHP Wordpress')
INSERT [dbo].[units] ([UnitID], [Description]) VALUES (14, N'Generic')
SET IDENTITY_INSERT [dbo].[units] OFF
GO
SET IDENTITY_INSERT [dbo].[tasks] ON 

INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (2, 1, 3, 26, N'C# introduction review project - Tic Tac Toe', CAST(N'2020-06-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-06-05T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (3, 2, 3, 26, N'Javascript todo challenge', CAST(N'2020-06-12T00:00:00.0000000' AS DateTime2), CAST(N'2020-06-15T00:00:00.0000000' AS DateTime2), N'https:/non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (4, 2, 5, 26, N'Milestone 1 - Resume', CAST(N'2020-06-15T00:00:00.0000000' AS DateTime2), CAST(N'2020-06-21T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (5, 1, 2, 26, N'AJAX API', CAST(N'2020-06-25T00:00:00.0000000' AS DateTime2), CAST(N'2020-06-26T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (6, 1, 2, 26, N'React Calculator', CAST(N'2020-06-30T00:00:00.0000000' AS DateTime2), CAST(N'2020-07-01T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (7, 1, 3, 26, N'React/Redux/Router Calculator', CAST(N'2020-07-08T00:00:00.0000000' AS DateTime2), CAST(N'2020-07-11T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (8, 1, 5, 26, N'Milestone 2 - React', CAST(N'2020-07-14T00:00:00.0000000' AS DateTime2), CAST(N'2020-07-21T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (9, 1, 4, 26, N'PHP API assignment', CAST(N'2020-07-23T00:00:00.0000000' AS DateTime2), CAST(N'2020-07-24T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (10, 1, 4, 26, N'PHP Blog assignment', CAST(N'2020-07-27T00:00:00.0000000' AS DateTime2), CAST(N'2020-07-28T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (11, 1, 2, 26, N'PHP Wordpress', CAST(N'2020-08-05T00:00:00.0000000' AS DateTime2), CAST(N'2020-08-06T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (12, 1, 2, 26, N'SQL Basic AdventureWorks Queries', CAST(N'2020-08-06T00:00:00.0000000' AS DateTime2), CAST(N'2020-08-07T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (13, 1, 2, 26, N'C# OOP practice - Pen class', CAST(N'2020-08-24T00:00:00.0000000' AS DateTime2), CAST(N'2020-08-25T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (14, 1, 2, 26, N'C# OOP practice - Car class', CAST(N'2020-08-27T00:00:00.0000000' AS DateTime2), CAST(N'2020-08-28T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (15, 1, 3, 26, N'C# OOP practice - LINQ', CAST(N'2020-09-09T00:00:00.0000000' AS DateTime2), CAST(N'2020-09-11T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (16, 1, 3, 26, N'C# Fundamentals - Computer shop', CAST(N'2020-10-11T00:00:00.0000000' AS DateTime2), CAST(N'2020-09-12T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (17, 1, 2, 26, N'ASP.NET Core - Library tracker', CAST(N'2020-10-14T00:00:00.0000000' AS DateTime2), CAST(N'2020-09-20T00:00:00.0000000' AS DateTime2), N'http://non-existent')
INSERT [dbo].[tasks] ([TaskID], [UnitID], [TypeID], [CohortID], [Title], [StartDate], [EndDate], [DocURL]) VALUES (18, 1, 3, 26, N'ASP.NET Core - Inventory tracker', CAST(N'2020-10-15T00:00:00.0000000' AS DateTime2), CAST(N'2020-10-22T00:00:00.0000000' AS DateTime2), N'http://non-existent')
SET IDENTITY_INSERT [dbo].[tasks] OFF
GO
SET IDENTITY_INSERT [dbo].[timesheets] ON 

INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (2, 13, 3, CAST(N'2020-10-14' AS Date), CAST(1.75 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (3, 13, 3, CAST(N'2020-10-14' AS Date), CAST(1.75 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (4, 13, 3, CAST(N'2020-10-14' AS Date), CAST(1.75 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (5, 13, 3, CAST(N'2020-10-14' AS Date), CAST(1.75 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (6, 13, 3, CAST(N'2020-10-13' AS Date), CAST(1.75 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (9, 13, 2, CAST(N'2020-10-15' AS Date), CAST(2.00 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (10, 13, 3, CAST(N'2020-10-04' AS Date), CAST(2.00 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (14, 13, 3, CAST(N'2020-10-18' AS Date), CAST(2.00 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (16, 13, 4, CAST(N'2020-10-18' AS Date), CAST(6.00 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (17, 13, 2, CAST(N'2020-10-18' AS Date), CAST(4.00 AS Decimal(3, 2)), CAST(N'00:00:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (57, 14, 17, CAST(N'2020-10-20' AS Date), CAST(0.25 AS Decimal(3, 2)), CAST(N'00:22:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (58, 14, 17, CAST(N'2020-10-20' AS Date), CAST(0.75 AS Decimal(3, 2)), CAST(N'00:45:00' AS Time), CAST(N'00:00:00' AS Time))
INSERT [dbo].[timesheets] ([RecordID], [StudentID], [AssignmentID], [Date], [TimeAllocation], [EndTime], [StartTime]) VALUES (61, 14, 17, CAST(N'2020-10-21' AS Date), CAST(0.50 AS Decimal(3, 2)), CAST(N'00:30:00' AS Time), CAST(N'00:00:00' AS Time))
SET IDENTITY_INSERT [dbo].[timesheets] OFF
GO
