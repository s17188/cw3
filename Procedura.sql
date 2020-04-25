CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN

	DECLARE @IdStudies INT = (select IdStudy from Studies where Name=@Studies);
	IF @IdStudies IS NULL
	BEGIN
		print 'Study does not exist';
	END

	DECLARE @IdEnrollment INT = (select IdEnrollment from Enrollment WHERE IdStudy=@IdStudies);
	IF @IdEnrollment IS NULL
	BEGIN
		print 'Enrollment does not exist';
	END

	DECLARE @NewIdEnrollment INT = (select IdEnrollment from Enrollment WHERE Semester=@Semester+1 AND IdStudy=@IdStudies);
	IF @NewIdEnrollment IS NULL		
	BEGIN
		INSERT INTO Enrollment VALUES(@IdEnrollment+1,@Semester+1,@IdStudies,'2020-02-02');
	END

	UPDATE Student SET
	IdEnrollment=@IdEnrollment+1

	COMMIT

	RETURN @IdEnrollment+1;
END