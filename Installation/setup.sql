USE [dbtest]
GO
/****** Object:  Table [dbo].[attendance]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[attendance](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[StudentID] [int] NOT NULL,
	[AttendanceStateID] [int] NOT NULL,
	[StaffID] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Comment] [nvarchar](max) NULL,
 CONSTRAINT [PK_attendance] PRIMARY KEY CLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[attendance_states]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[attendance_states](
	[StateID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_attendance_states] PRIMARY KEY CLUSTERED 
(
	[StateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cohorts]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cohorts](
	[CohortID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_cohorts] PRIMARY KEY CLUSTERED 
(
	[CohortID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notices]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notices](
	[NoticeID] [int] IDENTITY(1,1) NOT NULL,
	[CohortID] [int] NOT NULL,
	[StaffID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[Markdown] [nvarchar](max) NULL,
	[HTML] [nvarchar](max) NULL,
 CONSTRAINT [PK_notices] PRIMARY KEY CLUSTERED 
(
	[NoticeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[staff]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[staff](
	[StaffID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[SuperUser] [bit] NOT NULL,
 CONSTRAINT [PK_staff] PRIMARY KEY CLUSTERED 
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[students]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[students](
	[StudentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[CohortID] [int] NOT NULL,
	[BearTracksID] [varchar](10) NULL,
 CONSTRAINT [PK_students] PRIMARY KEY CLUSTERED 
(
	[StudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_types]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_types](
	[TypeID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_task_types] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tasks]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tasks](
	[TaskID] [int] IDENTITY(1,1) NOT NULL,
	[UnitID] [int] NOT NULL,
	[TypeID] [int] NOT NULL,
	[CohortID] [int] NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[DocURL] [varchar](255) NULL,
 CONSTRAINT [PK_tasks] PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[timesheets]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[timesheets](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[StudentID] [int] NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[TimeAllocation] [decimal](3, 2) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[StartTime] [time](7) NOT NULL,
 CONSTRAINT [PK_timesheets] PRIMARY KEY CLUSTERED 
(
	[RecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[units]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[units](
	[UnitID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_units] PRIMARY KEY CLUSTERED 
(
	[UnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2020-10-21 2:28:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Firstname] [nvarchar](50) NOT NULL,
	[Lastname] [nvarchar](50) NOT NULL,
	[Email] [varchar](320) NOT NULL,
	[Active] [bit] NOT NULL,
	[ActiveToken] [varchar](2048) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[timesheets] ADD  DEFAULT ('00:00:00') FOR [EndTime]
GO
ALTER TABLE [dbo].[timesheets] ADD  DEFAULT ('00:00:00') FOR [StartTime]
GO
ALTER TABLE [dbo].[attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_AttendanceState] FOREIGN KEY([AttendanceStateID])
REFERENCES [dbo].[attendance_states] ([StateID])
GO
ALTER TABLE [dbo].[attendance] CHECK CONSTRAINT [FK_Attendance_AttendanceState]
GO
ALTER TABLE [dbo].[attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Staff] FOREIGN KEY([StaffID])
REFERENCES [dbo].[staff] ([StaffID])
GO
ALTER TABLE [dbo].[attendance] CHECK CONSTRAINT [FK_Attendance_Staff]
GO
ALTER TABLE [dbo].[attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Student] FOREIGN KEY([StudentID])
REFERENCES [dbo].[students] ([StudentID])
GO
ALTER TABLE [dbo].[attendance] CHECK CONSTRAINT [FK_Attendance_Student]
GO
ALTER TABLE [dbo].[notices]  WITH CHECK ADD  CONSTRAINT [FK_Notice_Cohort] FOREIGN KEY([CohortID])
REFERENCES [dbo].[cohorts] ([CohortID])
GO
ALTER TABLE [dbo].[notices] CHECK CONSTRAINT [FK_Notice_Cohort]
GO
ALTER TABLE [dbo].[notices]  WITH CHECK ADD  CONSTRAINT [FK_Notice_Staff] FOREIGN KEY([StaffID])
REFERENCES [dbo].[staff] ([StaffID])
GO
ALTER TABLE [dbo].[notices] CHECK CONSTRAINT [FK_Notice_Staff]
GO
ALTER TABLE [dbo].[staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[staff] CHECK CONSTRAINT [FK_Staff_User]
GO
ALTER TABLE [dbo].[students]  WITH CHECK ADD  CONSTRAINT [FK_Student_Cohort] FOREIGN KEY([CohortID])
REFERENCES [dbo].[cohorts] ([CohortID])
GO
ALTER TABLE [dbo].[students] CHECK CONSTRAINT [FK_Student_Cohort]
GO
ALTER TABLE [dbo].[students]  WITH CHECK ADD  CONSTRAINT [FK_Student_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[students] CHECK CONSTRAINT [FK_Student_User]
GO
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_Task_Cohort] FOREIGN KEY([CohortID])
REFERENCES [dbo].[cohorts] ([CohortID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_Task_Cohort]
GO
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_Task_TaskType] FOREIGN KEY([TypeID])
REFERENCES [dbo].[task_types] ([TypeID])
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_Task_TaskType]
GO
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_Task_Unit] FOREIGN KEY([UnitID])
REFERENCES [dbo].[units] ([UnitID])
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_Task_Unit]
GO
ALTER TABLE [dbo].[timesheets]  WITH CHECK ADD  CONSTRAINT [FK_Timesheet_Student] FOREIGN KEY([StudentID])
REFERENCES [dbo].[students] ([StudentID])
GO
ALTER TABLE [dbo].[timesheets] CHECK CONSTRAINT [FK_Timesheet_Student]
GO
ALTER TABLE [dbo].[timesheets]  WITH CHECK ADD  CONSTRAINT [FK_Timesheet_Task] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tasks] ([TaskID])
GO
ALTER TABLE [dbo].[timesheets] CHECK CONSTRAINT [FK_Timesheet_Task]
GO
