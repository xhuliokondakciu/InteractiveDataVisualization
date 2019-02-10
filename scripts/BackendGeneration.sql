
CREATE TABLE [dbo].[TestTable](
	[Id] [int] NOT NULL,
	[Data] [varchar](256) NULL
) ON [PRIMARY]
GO

CREATE PROCEDURE [dbo].[GetTestData_SP]
AS
BEGIN

	SELECT
		[Data]
	FROM
		[dbo].[TestTable]
END
GO

GRANT EXECUTE ON [dbo].[GetTestData_SP] TO [MachineLearningGUIUser] AS [dbo]
GO