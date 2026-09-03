/* ============================================================
   RACE DAY DATABASE
   ============================================================ */


/* ============================================================
   1. CREATE DATABASE
   ============================================================ */

IF DB_ID('RaceDayDb') IS NULL
BEGIN
    EXEC('CREATE DATABASE RaceDayDb');
END;


/* ============================================================
   2. USE DATABASE
   ============================================================ */

USE RaceDayDb;


/* ============================================================
   3. DELETE OLD STORED PROCEDURES
   ============================================================ */

IF OBJECT_ID('RegisterUser', 'P') IS NOT NULL
    DROP PROCEDURE RegisterUser;

IF OBJECT_ID('LoginUser', 'P') IS NOT NULL
    DROP PROCEDURE LoginUser;

IF OBJECT_ID('UpdateUserProfile', 'P') IS NOT NULL
    DROP PROCEDURE UpdateUserProfile;

IF OBJECT_ID('CreateEvent', 'P') IS NOT NULL
    DROP PROCEDURE CreateEvent;

IF OBJECT_ID('UpdateEvent', 'P') IS NOT NULL
    DROP PROCEDURE UpdateEvent;

IF OBJECT_ID('DeleteEvent', 'P') IS NOT NULL
    DROP PROCEDURE DeleteEvent;

IF OBJECT_ID('CreateCategory', 'P') IS NOT NULL
    DROP PROCEDURE CreateCategory;

IF OBJECT_ID('AddEventCategory', 'P') IS NOT NULL
    DROP PROCEDURE AddEventCategory;

IF OBJECT_ID('AddRoute', 'P') IS NOT NULL
    DROP PROCEDURE AddRoute;

IF OBJECT_ID('AddWeatherRecord', 'P') IS NOT NULL
    DROP PROCEDURE AddWeatherRecord;

IF OBJECT_ID('EnrolParticipant', 'P') IS NOT NULL
    DROP PROCEDURE EnrolParticipant;

IF OBJECT_ID('CancelEnrolment', 'P') IS NOT NULL
    DROP PROCEDURE CancelEnrolment;

IF OBJECT_ID('AddResult', 'P') IS NOT NULL
    DROP PROCEDURE AddResult;

IF OBJECT_ID('GetEvents', 'P') IS NOT NULL
    DROP PROCEDURE GetEvents;

IF OBJECT_ID('GetEventCategories', 'P') IS NOT NULL
    DROP PROCEDURE GetEventCategories;

IF OBJECT_ID('GetUserEnrolments', 'P') IS NOT NULL
    DROP PROCEDURE GetUserEnrolments;

IF OBJECT_ID('GetEventResults', 'P') IS NOT NULL
    DROP PROCEDURE GetEventResults;


/* ============================================================
   4. DELETE OLD TABLES
   ============================================================ */

IF OBJECT_ID('Results', 'U') IS NOT NULL
    DROP TABLE Results;

IF OBJECT_ID('Enrolments', 'U') IS NOT NULL
    DROP TABLE Enrolments;

IF OBJECT_ID('WeatherRecords', 'U') IS NOT NULL
    DROP TABLE WeatherRecords;

IF OBJECT_ID('Routes', 'U') IS NOT NULL
    DROP TABLE Routes;

IF OBJECT_ID('EventCategories', 'U') IS NOT NULL
    DROP TABLE EventCategories;

IF OBJECT_ID('Categories', 'U') IS NOT NULL
    DROP TABLE Categories;

IF OBJECT_ID('Events', 'U') IS NOT NULL
    DROP TABLE Events;

IF OBJECT_ID('Users', 'U') IS NOT NULL
    DROP TABLE Users;


/* ============================================================
   5. CREATE USERS TABLE
   ============================================================ */

CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,

    FirstName VARCHAR(50) NOT NULL,

    LastName VARCHAR(50) NOT NULL,

    Email VARCHAR(100) NOT NULL UNIQUE,

    PasswordHash VARCHAR(255) NOT NULL,

    Role VARCHAR(20) NOT NULL,

    Phone VARCHAR(20),

    CreatedAt DATETIME DEFAULT GETDATE(),

    CHECK
    (
        Role = 'Organiser'
        OR Role = 'Participant'
    )
);


/* ============================================================
   6. EVENTS TABLE
   ============================================================ */

CREATE TABLE Events
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,

    OrganiserID INT NOT NULL,

    EventName VARCHAR(100) NOT NULL,

    EventDate DATE NOT NULL,

    Location VARCHAR(100) NOT NULL,

    Description VARCHAR(500),

    Status VARCHAR(20) DEFAULT 'Draft',

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (OrganiserID)
        REFERENCES Users(UserID),

    CHECK
    (
        Status = 'Draft'
        OR Status = 'Published'
        OR Status = 'Completed'
        OR Status = 'Cancelled'
    )
);


/* ============================================================
   7. CATEGORIES TABLE
   ============================================================ */

CREATE TABLE Categories
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,

    CategoryName VARCHAR(100) NOT NULL UNIQUE,

    Description VARCHAR(255)
);


/* ============================================================
   8. EVENT CATEGORIES TABLE
   ============================================================ */

CREATE TABLE EventCategories
(
    EventCategoryID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    CategoryID INT NOT NULL,

    EntryFee DECIMAL(10,2) NOT NULL,

    MaximumParticipants INT NOT NULL,

    FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    FOREIGN KEY (CategoryID)
        REFERENCES Categories(CategoryID),

    UNIQUE(EventID, CategoryID),

    CHECK (EntryFee >= 0),

    CHECK (MaximumParticipants > 0)
);


/* ============================================================
   9. ROUTES TABLE
   ============================================================ */

CREATE TABLE Routes
(
    RouteID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    RouteName VARCHAR(100) NOT NULL,

    DistanceKm DECIMAL(6,2) NOT NULL,

    RouteDescription VARCHAR(500),

    FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    UNIQUE(EventID),

    CHECK (DistanceKm > 0)
);


/* ============================================================
   10. WEATHER RECORDS TABLE
   ============================================================ */

CREATE TABLE WeatherRecords
(
    WeatherRecordID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    Temperature DECIMAL(5,2),

    WeatherCondition VARCHAR(100),

    WindSpeed DECIMAL(5,2),

    RecordedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    CHECK
    (
        WindSpeed IS NULL
        OR WindSpeed >= 0
    )
);


/* ============================================================
   11. ENROLMENTS TABLE
   ============================================================ */

CREATE TABLE Enrolments
(
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,

    EventCategoryID INT NOT NULL,

    ParticipantID INT NOT NULL,

    EnrolmentDate DATETIME DEFAULT GETDATE(),

    Status VARCHAR(20) DEFAULT 'Pending',

    FOREIGN KEY (EventCategoryID)
        REFERENCES EventCategories(EventCategoryID),

    FOREIGN KEY (ParticipantID)
        REFERENCES Users(UserID),

    UNIQUE(ParticipantID, EventCategoryID),

    CHECK
    (
        Status = 'Pending'
        OR Status = 'Confirmed'
        OR Status = 'Cancelled'
    )
);


/* ============================================================
   12. RESULTS TABLE
   ============================================================ */

CREATE TABLE Results
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,

    EnrolmentID INT NOT NULL,

    FinishTime TIME,

    Position INT,

    ResultStatus VARCHAR(20) NOT NULL,

    FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolments(EnrolmentID),

    UNIQUE(EnrolmentID),

    CHECK
    (
        Position IS NULL
        OR Position > 0
    ),

    CHECK
    (
        ResultStatus = 'Finished'
        OR ResultStatus = 'Did Not Finish'
        OR ResultStatus = 'Disqualified'
    )
);


/* ============================================================
   13. REGISTER USER
   ============================================================ */

EXEC('
CREATE PROCEDURE RegisterUser

    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255),
    @Role VARCHAR(20),
    @Phone VARCHAR(20)

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM Users
        WHERE Email = @Email
    )
    BEGIN

        PRINT ''An account with this email already exists.'';

        RETURN;

    END;


    INSERT INTO Users
    (
        FirstName,
        LastName,
        Email,
        PasswordHash,
        Role,
        Phone
    )

    VALUES
    (
        @FirstName,
        @LastName,
        @Email,
        @PasswordHash,
        @Role,
        @Phone
    );


    SELECT *
    FROM Users
    WHERE UserID = SCOPE_IDENTITY();

END
');


/* ============================================================
   14. LOGIN USER
   ============================================================ */

EXEC('
CREATE PROCEDURE LoginUser

    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255)

AS
BEGIN

    SELECT
        UserID,
        FirstName,
        LastName,
        Email,
        Role,
        Phone

    FROM Users

    WHERE Email = @Email
    AND PasswordHash = @PasswordHash;

END
');


/* ============================================================
   15. UPDATE USER PROFILE
   ============================================================ */

EXEC('
CREATE PROCEDURE UpdateUserProfile

    @UserID INT,
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Phone VARCHAR(20)

AS
BEGIN

    UPDATE Users

    SET
        FirstName = @FirstName,
        LastName = @LastName,
        Phone = @Phone

    WHERE UserID = @UserID;


    SELECT *
    FROM Users

    WHERE UserID = @UserID;

END
');


/* ============================================================
   16. CREATE EVENT
   ============================================================ */

EXEC('
CREATE PROCEDURE CreateEvent

    @OrganiserID INT,
    @EventName VARCHAR(100),
    @EventDate DATE,
    @Location VARCHAR(100),
    @Description VARCHAR(500)

AS
BEGIN

    INSERT INTO Events
    (
        OrganiserID,
        EventName,
        EventDate,
        Location,
        Description
    )

    VALUES
    (
        @OrganiserID,
        @EventName,
        @EventDate,
        @Location,
        @Description
    );


    SELECT *
    FROM Events

    WHERE EventID = SCOPE_IDENTITY();

END
');


/* ============================================================
   17. UPDATE EVENT
   ============================================================ */

EXEC('
CREATE PROCEDURE UpdateEvent

    @EventID INT,
    @OrganiserID INT,
    @EventName VARCHAR(100),
    @EventDate DATE,
    @Location VARCHAR(100),
    @Description VARCHAR(500),
    @Status VARCHAR(20)

AS
BEGIN

    UPDATE Events

    SET
        EventName = @EventName,
        EventDate = @EventDate,
        Location = @Location,
        Description = @Description,
        Status = @Status

    WHERE EventID = @EventID
    AND OrganiserID = @OrganiserID;


    SELECT *
    FROM Events

    WHERE EventID = @EventID;

END
');


/* ============================================================
   18. DELETE EVENT
   ============================================================ */

EXEC('
CREATE PROCEDURE DeleteEvent

    @EventID INT,
    @OrganiserID INT

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM EventCategories
        WHERE EventID = @EventID
    )
    BEGIN

        PRINT ''Cannot delete the event because it has categories.'';

        RETURN;

    END;


    IF EXISTS
    (
        SELECT *
        FROM Routes
        WHERE EventID = @EventID
    )
    BEGIN

        PRINT ''Cannot delete the event because it has a route.'';

        RETURN;

    END;


    IF EXISTS
    (
        SELECT *
        FROM WeatherRecords
        WHERE EventID = @EventID
    )
    BEGIN

        PRINT ''Cannot delete the event because it has weather records.'';

        RETURN;

    END;


    DELETE FROM Events

    WHERE EventID = @EventID
    AND OrganiserID = @OrganiserID;


    PRINT ''Event deleted successfully.'';

END
');


/* ============================================================
   19. CREATE CATEGORY
   ============================================================ */

EXEC('
CREATE PROCEDURE CreateCategory

    @CategoryName VARCHAR(100),
    @Description VARCHAR(255)

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM Categories
        WHERE CategoryName = @CategoryName
    )
    BEGIN

        PRINT ''This category already exists.'';

        RETURN;

    END;


    INSERT INTO Categories
    (
        CategoryName,
        Description
    )

    VALUES
    (
        @CategoryName,
        @Description
    );


    SELECT *
    FROM Categories

    WHERE CategoryID = SCOPE_IDENTITY();

END
');


/* ============================================================
   20. ADD EVENT CATEGORY
   ============================================================ */

EXEC('
CREATE PROCEDURE AddEventCategory

    @EventID INT,
    @CategoryID INT,
    @EntryFee DECIMAL(10,2),
    @MaximumParticipants INT

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM EventCategories
        WHERE EventID = @EventID
        AND CategoryID = @CategoryID
    )
    BEGIN

        PRINT ''This category is already linked to this event.'';

        RETURN;

    END;


    INSERT INTO EventCategories
    (
        EventID,
        CategoryID,
        EntryFee,
        MaximumParticipants
    )

    VALUES
    (
        @EventID,
        @CategoryID,
        @EntryFee,
        @MaximumParticipants
    );


    SELECT *
    FROM EventCategories

    WHERE EventCategoryID = SCOPE_IDENTITY();

END
');


/* ============================================================
   21. ADD ROUTE
   ============================================================ */

EXEC('
CREATE PROCEDURE AddRoute

    @EventID INT,
    @RouteName VARCHAR(100),
    @DistanceKm DECIMAL(6,2),
    @RouteDescription VARCHAR(500)

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM Routes
        WHERE EventID = @EventID
    )
    BEGIN

        PRINT ''This event already has a route.'';

        RETURN;

    END;


    INSERT INTO Routes
    (
        EventID,
        RouteName,
        DistanceKm,
        RouteDescription
    )

    VALUES
    (
        @EventID,
        @RouteName,
        @DistanceKm,
        @RouteDescription
    );


    SELECT *
    FROM Routes

    WHERE RouteID = SCOPE_IDENTITY();

END
');


/* ============================================================
   22. ADD WEATHER RECORD
   ============================================================ */

EXEC('
CREATE PROCEDURE AddWeatherRecord

    @EventID INT,
    @Temperature DECIMAL(5,2),
    @WeatherCondition VARCHAR(100),
    @WindSpeed DECIMAL(5,2)

AS
BEGIN

    INSERT INTO WeatherRecords
    (
        EventID,
        Temperature,
        WeatherCondition,
        WindSpeed
    )

    VALUES
    (
        @EventID,
        @Temperature,
        @WeatherCondition,
        @WindSpeed
    );


    SELECT *
    FROM WeatherRecords

    WHERE WeatherRecordID = SCOPE_IDENTITY();

END
');


/* ============================================================
   23. ENROLE PARTICIPANT
   ============================================================ */

EXEC('
CREATE PROCEDURE EnrolParticipant

    @EventCategoryID INT,
    @ParticipantID INT

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM Enrolments

        WHERE EventCategoryID = @EventCategoryID
        AND ParticipantID = @ParticipantID
        AND Status <> ''Cancelled''
    )
    BEGIN

        PRINT ''The participant is already enrolled.'';

        RETURN;

    END;


    INSERT INTO Enrolments
    (
        EventCategoryID,
        ParticipantID,
        Status
    )

    VALUES
    (
        @EventCategoryID,
        @ParticipantID,
        ''Confirmed''
    );


    SELECT *
    FROM Enrolments

    WHERE EnrolmentID = SCOPE_IDENTITY();

END
');


/* ============================================================
   24. CANCEL ENROLMENT
   ============================================================ */

EXEC('
CREATE PROCEDURE CancelEnrolment

    @EnrolmentID INT

AS
BEGIN

    UPDATE Enrolments

    SET Status = ''Cancelled''

    WHERE EnrolmentID = @EnrolmentID;


    SELECT *
    FROM Enrolments

    WHERE EnrolmentID = @EnrolmentID;

END
');


/* ============================================================
   25. ADD RESULT
   ============================================================ */

EXEC('
CREATE PROCEDURE AddResult

    @EnrolmentID INT,
    @FinishTime TIME,
    @Position INT,
    @ResultStatus VARCHAR(20)

AS
BEGIN

    IF EXISTS
    (
        SELECT *
        FROM Results
        WHERE EnrolmentID = @EnrolmentID
    )
    BEGIN

        PRINT ''A result already exists for this enrolment.'';

        RETURN;

    END;


    INSERT INTO Results
    (
        EnrolmentID,
        FinishTime,
        Position,
        ResultStatus
    )

    VALUES
    (
        @EnrolmentID,
        @FinishTime,
        @Position,
        @ResultStatus
    );


    SELECT *
    FROM Results

    WHERE ResultID = SCOPE_IDENTITY();

END
');


/* ============================================================
   26. GET ALL EVENTS
   ============================================================ */

EXEC('
CREATE PROCEDURE GetEvents

AS
BEGIN

    SELECT
        e.EventID,
        e.EventName,
        e.EventDate,
        e.Location,
        e.Description,
        e.Status,
        u.FirstName + '' '' + u.LastName AS Organiser

    FROM Events e

    INNER JOIN Users u
        ON e.OrganiserID = u.UserID

    ORDER BY e.EventDate;

END
');


/* ============================================================
   27. GET EVENT CATEGORIES
   ============================================================ */

EXEC('
CREATE PROCEDURE GetEventCategories

    @EventID INT

AS
BEGIN

    SELECT
        e.EventName,
        c.CategoryName,
        ec.EntryFee,
        ec.MaximumParticipants

    FROM EventCategories ec

    INNER JOIN Events e
        ON ec.EventID = e.EventID

    INNER JOIN Categories c
        ON ec.CategoryID = c.CategoryID

    WHERE e.EventID = @EventID;

END
');


/* ============================================================
   28. GET USER ENROLMENTS
   ============================================================ */

EXEC('
CREATE PROCEDURE GetUserEnrolments

    @ParticipantID INT

AS
BEGIN

    SELECT
        en.EnrolmentID,
        e.EventName,
        c.CategoryName,
        e.EventDate,
        e.Location,
        ec.EntryFee,
        en.EnrolmentDate,
        en.Status

    FROM Enrolments en

    INNER JOIN EventCategories ec
        ON en.EventCategoryID = ec.EventCategoryID

    INNER JOIN Events e
        ON ec.EventID = e.EventID

    INNER JOIN Categories c
        ON ec.CategoryID = c.CategoryID

    WHERE en.ParticipantID = @ParticipantID

    ORDER BY e.EventDate;

END
');


/* ============================================================
   29. GET EVENT RESULTS
   ============================================================ */

EXEC('
CREATE PROCEDURE GetEventResults

    @EventID INT

AS
BEGIN

    SELECT
        e.EventName,

        c.CategoryName,

        u.FirstName + '' '' + u.LastName
            AS Participant,

        r.FinishTime,

        r.Position,

        r.ResultStatus

    FROM Results r

    INNER JOIN Enrolments en
        ON r.EnrolmentID = en.EnrolmentID

    INNER JOIN EventCategories ec
        ON en.EventCategoryID = ec.EventCategoryID

    INNER JOIN Events e
        ON ec.EventID = e.EventID

    INNER JOIN Categories c
        ON ec.CategoryID = c.CategoryID

    INNER JOIN Users u
        ON en.ParticipantID = u.UserID

    WHERE e.EventID = @EventID

    ORDER BY
        c.CategoryName,
        r.Position;

END
');


/* ============================================================
   30. CHECK THE TABLES
   ============================================================ */

SELECT *
FROM Users;

SELECT *
FROM Events;

SELECT *
FROM Categories;

SELECT *
FROM EventCategories;

SELECT *
FROM Routes;

SELECT *
FROM WeatherRecords;

SELECT *
FROM Enrolments;

SELECT *
FROM Results;


/* ============================================================
   END OF SCRIPT
   ============================================================ */