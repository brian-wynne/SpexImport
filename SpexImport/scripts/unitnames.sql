CREATE TABLE IF NOT EXISTS unitnames_temp
(
	unitid INT,
	name TEXT,
	localeid SMALLINT,
	FOREIGN KEY(unitid) REFERENCES units(unitid)
);