USE master;
GO

-- Force all users off the database and set it to single-user mode
ALTER DATABASE [TraCuuTuyenSinh] 
SET SINGLE_USER 
WITH ROLLBACK IMMEDIATE;
GO

-- Permanently delete the database
DROP DATABASE [TraCuuTuyenSinh];
GO
