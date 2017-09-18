Use XorHub

CREATE TABLE Batch (
	BatchId int identity(1,1) primary key,
	Name varchar(10) not null
)

CREATE TABLE LoginInfo (
	Username varchar(10) primary key,
	Passwd varchar(15) not null,
	Usertype varchar(1) not null,
	Stat bit not null,
	Name varchar(20) not null,
	BatchId int foreign key references Batch(BatchId)
)

INSERT INTO Batch VALUES ( 'Fresh2k17')
INSERT INTO Batch VALUES ( 'Fresh2k18')

CREATE TABLE Assignment (
	AssignmentId int identity(1,1) primary key,
	Title varchar(50) not null,
	PostedDate datetime not null,
	TeacherName varchar(10) foreign key references LoginInfo(Username),
	Deadline datetime not null,
	BatchId int foreign key references Batch(BatchId),
	Document varchar(100)
)

CREATE TABLE Solution (
	SolutionId int identity(1,1) primary key,
	AssignmentId int foreign key references Assignment(AssignmentId),
	Username varchar(10) foreign key references LoginInfo(Username),
	Stat varchar(1) not null,
	UploadedOn datetime not null,
	Document varchar(100) not null,
	Comment varchar(200) not null
)

