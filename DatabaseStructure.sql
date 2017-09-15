USE XorHub

CREATE TABLE Batch (
	BatchId numeric primary key,
	Name varchar(10) not null
)

CREATE TABLE LoginInfo (
	Username varchar(10) primary key,
	Passwd varchar(15) not null,
	Usertype varchar(1) not null,
	Stat bit not null,
	Name varchar(20) not null,
	BatchId numeric foreign key references Batch(BatchId)
)

INSERT INTO Batch VALUES (101, 'Fresh2k17')
INSERT INTO Batch VALUES (102, 'Fresh2k18')

CREATE TABLE Assignment (
	AssignmentId numeric primary key,
	Title varchar(50) not null,
	PostedDate datetime not null,
	TeacherId varchar(10) foreign key references LoginInfo(Username),
	Deadline datetime not null,
	BatchId numeric foreign key references Batch(BatchId),
	Document varchar(100)
)

CREATE TABLE Solution (
	SolutionId numeric primary key,
	AssignmentId numeric foreign key references Assignment(AssignmentId),
	Username varchar(10) foreign key references LoginInfo(Username),
	Stat varchar(1) not null,
	UploadedOn datetime not null,
	Document varchar(100) not null,
	Comment varchar(200) not null
)




