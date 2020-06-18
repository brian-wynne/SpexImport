CREATE TABLE IF NOT EXISTS searchattributevalues_temp
(
	valueid INT,
	value TEXT,
	absolutevalue DOUBLE,
	unitid INT,
	isabsolute SMALLINT,
	PRIMARY KEY(valueid),
	FOREIGN KEY(unitid) REFERENCES units(unitid)
);