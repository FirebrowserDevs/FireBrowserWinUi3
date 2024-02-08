-- Script Date: 2/7/2024 4:15 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
-- Database information:
-- Database: C:\Users\admin\OneDrive\Documents\FireBrowserUserCore\Users\dizzler\Database\History.db
-- ServerVersion: 3.40.0
-- DatabaseSize: 8 KB
-- Created: 2/6/2024 8:37 PM

-- User Table information:
-- Number of tables: 1
-- urls: -1 row(s)

SELECT 1;
PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [urls] (
  [id] bigint NOT NULL
, [last_visit_time] text NULL
, [url] text NULL
, [title] text NULL
, [visit_count] bigint DEFAULT (0) NOT NULL
, [typed_count] bigint DEFAULT (0) NOT NULL
, [hidden] bigint DEFAULT (0) NOT NULL
, CONSTRAINT [sqlite_master_PK_urls] PRIMARY KEY ([id])
);
COMMIT;

